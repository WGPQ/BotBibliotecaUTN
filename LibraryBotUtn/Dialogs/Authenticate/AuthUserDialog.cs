using LibraryBotUtn.Common.Models;
using LibraryBotUtn.Services.BotConfig;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.LibraryBotUtn.QnA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Dialogs.Authenticate
{
    public class AuthUserDialog : ComponentDialog

    {
        private readonly IDataservices _dataservices;
        public readonly IQnAMakerServices _qnmakerService;


        private static string EMAIL_USER_PROMPT = "EMAIL_USER_PROMPT";
        private string WATER_fULL_STEP_REGISTER = "REGISTER_USER";
        private string WATER_fULL_STEP_CONSULTA = "CONSULTA";
        private string WATER_fULL_STEP_INITIAL = "WATER_fULL_STEP_INITIAL";
        protected readonly BotState _botState;


        public AuthUserDialog(IDataservices dataservices, UserState botState, IQnAMakerServices qnmakerService):base(nameof(AuthUserDialog))
        {
            _dataservices = dataservices;
            _botState = botState;
            _qnmakerService = qnmakerService;

            // USER INTRODUCTION
            var waterfullStepIntriduction = new WaterfallStep[]
            {
                RequestEmailUser,
                ValidateUser,

            };
            AddDialog(new WaterfallDialog(WATER_fULL_STEP_INITIAL, waterfullStepIntriduction));
            AddDialog(new TextPrompt("texto"));
            AddDialog(new TextPrompt(EMAIL_USER_PROMPT, EmailValidator));
            //REGISTER USER


            var waterfullStepAuth = new WaterfallStep[]
           {
               SolicitarNombre,
                VerificarRegistro
           };
            AddDialog(new WaterfallDialog(WATER_fULL_STEP_REGISTER, waterfullStepAuth));

            var waterfullConsulta = new WaterfallStep[]
       {
               Preguntar,
               Responder
       };
            AddDialog(new WaterfallDialog(WATER_fULL_STEP_CONSULTA, waterfullConsulta));






        }

        private async Task<DialogTurnResult> Responder(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var qnamakerResult = await _qnmakerService._qnamaker.GetAnswersAsync(stepContext.Context);

            return await Intentions(stepContext, qnamakerResult, cancellationToken);
        }

        private async Task<DialogTurnResult> Intentions(WaterfallStepContext stepContext, QueryResult[] qnamakerResult, CancellationToken cancellationToken)
        {
            if (qnamakerResult.Any())
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Biblioteca: " + qnamakerResult.First().Answer), cancellationToken);
                return await stepContext.BeginDialogAsync(WATER_fULL_STEP_CONSULTA, null, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Lo siento no encontre una respuesta :("), cancellationToken);
                return await stepContext.BeginDialogAsync(WATER_fULL_STEP_CONSULTA, null, cancellationToken);


            }
        }

        private async Task<DialogTurnResult> Preguntar(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Cual es su inquietud sobre la biblioteca", "soicitar pregunta", InputHints.ExpectingInput),
            };

            return await stepContext.PromptAsync("texto", promptOptions, cancellationToken);
        }


        private async Task<DialogTurnResult> SolicitarNombre(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Ahora igrese su nombre", "soicitar nombre", InputHints.ExpectingInput),
            };

            return await stepContext.PromptAsync("texto", promptOptions, cancellationToken);

        }

        private async Task<DialogTurnResult> VerificarRegistro(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var message = "";
            var userStateAccessors = _botState.CreateProperty<ClienteEntity>("userCliente");
            var userProfile = await userStateAccessors.GetAsync(stepContext.Context, () => new ClienteEntity());

            var userBotStateAccessors = _botState.CreateProperty<UsuarioEntity>("userBot");
            var userbot = await userBotStateAccessors.GetAsync(stepContext.Context, () => new UsuarioEntity());

            userProfile.nombre = stepContext.Context.Activity.Text?.Trim();

            var tokenBotAccessors = _botState.CreateProperty<TokenEntity>(nameof(TokenEntity));
            var tokenBot = await tokenBotAccessors.GetAsync(stepContext.Context, () => new TokenEntity());


            var respuesta = await _dataservices.AuthRepositori.NewUser(userProfile, tokenBot.token);
            if (respuesta.exito)
            {
                message = respuesta.message;
                ClienteEntity cliente = ClienteEntity.fromJson(respuesta.data.ToString());
                var resultado = await _dataservices.ClienteRepositori.Auth(cliente.correo);

                TokenEntity tokenResult = new TokenEntity();
                     if (resultado!=null)
                {
                    tokenResult.token = resultado.newToken;
                    cliente = resultado.cliente;
                    var interaction = new InteractionEntity
                    {
                        usuario_created = userbot.id,
                        usuario_interacted = cliente.Id
                    };
                    ChatEntity chat = await _dataservices.ChatRepositori.Interaction(interaction, tokenBot.token);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Ok {cliente.nombre} Empecemos!! chat {chat.chat}"), cancellationToken);
                }
            }
           return await stepContext.BeginDialogAsync(WATER_fULL_STEP_CONSULTA, null, cancellationToken);
            //return await stepContext.PromptAsync("texto", new PromptOptions { Prompt = MessageFactory.Text(userProfile.correo) }, cancellationToken);
        }

        private async Task<DialogTurnResult> RequestEmailUser(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Para poder iteractuar en el chat debe ingresar su correo", "soicitar correo", InputHints.ExpectingInput),
                RetryPrompt = MessageFactory.Text("Ingrese un correo valido"),
            };

            return await stepContext.PromptAsync(EMAIL_USER_PROMPT, promptOptions, cancellationToken);
        }


        private async Task<DialogTurnResult> ValidateUser(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var correo = stepContext.Context.Activity.Text?.Trim();
            var userStateAccessors = _botState.CreateProperty<ClienteEntity>("userCliente");
            var userProfile = await userStateAccessors.GetAsync(stepContext.Context, () => new ClienteEntity());
            var userBotStateAccessors = _botState.CreateProperty<UsuarioEntity>("userBot");
            var userbot = await userBotStateAccessors.GetAsync(stepContext.Context, () => new UsuarioEntity());
            var tokenBotAccessors = _botState.CreateProperty<TokenEntity>(nameof(TokenEntity));
            var tokenBotUTN = await tokenBotAccessors.GetAsync(stepContext.Context, () => new TokenEntity());
            ClienteVerificado clienteVerificado = await _dataservices.ClienteRepositori.Auth(correo);
            TokenEntity tokenUser = new TokenEntity();
            if (clienteVerificado.cliente!=null)
            {
                tokenUser.token = clienteVerificado.newToken;
                userProfile.correo = clienteVerificado.cliente.correo;
                userProfile.Id = clienteVerificado.cliente.Id;
                userProfile.nombre = clienteVerificado.cliente.nombre;
                userProfile.conectedAt = clienteVerificado.cliente.conectedAt;
                userProfile.rol = clienteVerificado.cliente.rol;
                var interaction = new InteractionEntity
                {
                    usuario_created = userbot.id,
                    usuario_interacted = userProfile.Id
                };
                ChatEntity chat = await _dataservices.ChatRepositori.Interaction(interaction, tokenBotUTN.token);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Hola {userProfile.nombre} es un gusto tenerte aqui! y este es us chat {chat.chat}"), cancellationToken);
                return await stepContext.BeginDialogAsync(WATER_fULL_STEP_CONSULTA, null, cancellationToken);
            }
            else
            {
                userProfile.correo = correo;
                return await stepContext.BeginDialogAsync(WATER_fULL_STEP_REGISTER, null, cancellationToken);
            }

        }

        private Task<bool> EmailValidator(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            bool isOk = IsValidEmail(promptContext.Recognized.Value);

            return Task.FromResult(promptContext.Recognized.Succeeded && isOk);
        }
        static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
