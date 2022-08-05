using LibraryBotUtn.Common.Cards;
using LibraryBotUtn.Common.Models;
using LibraryBotUtn.Common.Models.BotState;
using LibraryBotUtn.Services.BotConfig;
using LibraryBotUtn.Services.LuisAi;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.LibraryBotUtn.QnA;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Dialogs.Authenticate
{
    public class AuthUserDialog : ComponentDialog

    {
        public readonly ILuisAIService _luisAIServices;
        private readonly IDataservices _dataservices;
        public readonly IQnAMakerServices _qnmakerService;
       public static ClienteVerificado userData = new ClienteVerificado();

        private static string EMAIL_USER_PROMPT = "EMAIL_USER_PROMPT";
        private string WATER_fULL_STEP_REGISTER = "REGISTER_USER";
        private string WATER_fULL_STEP_AUTH = "WATER_fULL_STEP_AUTH";
        public static string keyClient = "userClient";
        private readonly IStatePropertyAccessor<AuthStateModel> _userState;



        public AuthUserDialog(IDataservices dataservices, UserState userState, IQnAMakerServices qnmakerService, ILuisAIService luisAIServices) : base(nameof(AuthUserDialog))
        {
            _luisAIServices = luisAIServices;
            _dataservices = dataservices;
            _userState = userState.CreateProperty<AuthStateModel>(keyClient);
            _qnmakerService = qnmakerService;

            //AddDialog(new MenuDialog(dataservices, qnmakerService,luisAIServices));
            // USER AUTH
            var waterfullStepAuth = new WaterfallStep[]
            {
                SolicitarEmail,
                ValidateUser,
                SolicitarNombre,
                RegistrarCliente,
                FinalAuth

            };
            AddDialog(new WaterfallDialog(WATER_fULL_STEP_AUTH, waterfullStepAuth));
            AddDialog(new TextPrompt(EMAIL_USER_PROMPT, EmailValidator));
            AddDialog(new TextPrompt("texto"));
          
            InitialDialogId = WATER_fULL_STEP_AUTH;
        }

        private async Task<DialogTurnResult> SolicitarEmail(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Por favor ingresa tu correo electrónico", "soicitar correo", InputHints.ExpectingInput),
                RetryPrompt = MessageFactory.Text("Debe ingresar un correo valido"),
            };

            return await stepContext.PromptAsync(EMAIL_USER_PROMPT, promptOptions, cancellationToken);
        }


        private async Task<DialogTurnResult> ValidateUser(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                string correo = stepContext.Context.Activity.Text?.Trim();
                userData = await _dataservices.ClienteRepositori.Auth(correo);
                var userStateData = await _userState.GetAsync(stepContext.Context, () => new AuthStateModel());
                if (userData.token != null)
                {
                    userStateData.IsAutenticate = true;
                    // var chat = await getChat(userData.cliente, MainDialog._bot.bot);
                    var token = LibraryBot<MainDialog>._bot.token;
                    var fraceEntity = await _dataservices.FracesRepositori.Frace("Bienvenida", token);
                    string wecome = fraceEntity.frace.Replace("#user", userData.cliente.nombre);
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(wecome), cancellationToken);
                }
                else
                {
                    userData.cliente = new ClienteEntity();
                    userData.cliente.correo = correo;   
                }
            }
            catch (Exception e)
            {
                await stepContext.Context.SendActivityAsync(e.Message.ToString(), cancellationToken: cancellationToken);
            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);

        }



        private async Task<DialogTurnResult> SolicitarNombre(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userStateData = await _userState.GetAsync(stepContext.Context, () => new AuthStateModel());
            if (userStateData.IsAutenticate)
            {
                return await stepContext.NextAsync(cancellationToken: cancellationToken);
            }
            else
            {
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Ahora igrese su nombre", "soicitar nombre", InputHints.ExpectingInput),
                };

                return await stepContext.PromptAsync("texto", promptOptions, cancellationToken);
            }

        }

        private async Task<DialogTurnResult> RegistrarCliente(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userStateData = await _userState.GetAsync(stepContext.Context, () => new AuthStateModel());
            if (!userStateData.IsAutenticate)
            {           
                userData.cliente.nombre = stepContext.Context.Activity.Text?.Trim();
                var respuesta = await _dataservices.ClienteRepositori.NewUser(userData.cliente, LibraryBot<MainDialog>._bot.token);
                if (respuesta.exito)
                {
                    ClienteEntity cliente = ClienteEntity.fromJson(respuesta.data.ToString());
                    var resultado = await _dataservices.ClienteRepositori.Auth(cliente.correo);

                    if (resultado != null)
                    {
                        userStateData.IsAutenticate = true;
                       // var chat = await getChat(resultado.cliente, MainDialog._bot.bot);
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Ok {cliente.nombre} Empecemos!!"), cancellationToken);
                    }
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Lo siento no se ha podido iniciar esta conversacion 😢"), cancellationToken);
                }
            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);

        }

      

        private async Task<DialogTurnResult> FinalAuth(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await Task.Delay(1000);
            await stepContext.Context.SendActivityAsync("¿En qué puedo ayudarte?", cancellationToken: cancellationToken);
            //return await MenuOptions.ToShow(stepContext, cancellationToken);
           return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
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
        private async Task<ChatEntity> getChat(ClienteEntity cliente, BotEntity bot)
        {
            var interaction = new InteractionEntity
            {
                usuario_created = bot.id,
                usuario_interacted = cliente.Id
            };
            return await _dataservices.ChatRepositori.Interaction(interaction, userData.token);
        }

    }
}
