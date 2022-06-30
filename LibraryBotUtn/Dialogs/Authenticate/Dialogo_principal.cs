using LibraryBotUtn.Common.Models;
using LibraryBotUtn.Dialogs.Authenticate;
using LibraryBotUtn.Services.BotConfig;
using LibraryBotUtn.Services.LuisAi;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.LibraryBotUtn.QnA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Dialogs
{
    public class Dialogo_principal : Dialogo_Logout
    {
        protected readonly ILogger _logger;
        public readonly ILuisAIService _luisAIServices;
        public readonly IQnAMakerServices _qnmakerService;
        protected readonly BotState _tokenBot;
        public Dialogo_principal(IConfiguration configuration, UserState bb, ILogger<Dialogo_principal> logger, ILuisAIService luisAIServices, IQnAMakerServices qnmakerService)
            : base(nameof(Dialogo_principal), configuration["ConnectionName"])
        {
            _logger = logger;
            _luisAIServices = luisAIServices;
            _qnmakerService = qnmakerService;
            _tokenBot = bb;


            AddDialog(new OAuthPrompt(
                nameof(OAuthPrompt),
                new OAuthPromptSettings
                {
                    ConnectionName = ConnectionName,
                    Text = "Please login",
                    Title = "Login",
                    Timeout = 300000, // User has 5 minutes to login
                }));

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                MostarOpciones,

                ElelcionInicio,

                //LoginStepAsync,
                //CommandStepAsync,
                PromptStepAsync
                
                //ProcessStepAsync
            }));
         
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }



    

        private async Task<DialogTurnResult> mostrarMensaje(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var tokenBotAccessors = _tokenBot.CreateProperty<TokenEntity>(nameof(TokenEntity));
            var tokenBotUTN = await tokenBotAccessors.GetAsync(stepContext.Context, () => new TokenEntity());

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Hola Mundo {tokenBotUTN.token}"), cancellationToken);
            return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);

        }

        private async Task<DialogTurnResult> ElelcionInicio(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DialogoPrincipal.EleccionInicio");
            // Cards are sent as Attachments in the Bot Framework.
            // So we need to create a list of attachments for the reply activity.
            var attachments = new List<Attachment>();

            // Reply to the activity we received with an activity.
            var reply = MessageFactory.Attachment(attachments);
            var userOption = stepContext.Context.Activity.Text.ToLower();
            switch (userOption)
            {
                case "utn":

                 return   await IrAut(stepContext, cancellationToken);
                    
                case "invitado":

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("fdfdfdf"), cancellationToken);

                    break;
                default:
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Ingreso no valido"), cancellationToken);
                    break;
            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
            //return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> IrAut(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(AuthUserDialog), cancellationToken: cancellationToken);

        }
        private Activity OpcionInicio()
        {
            var reply = MessageFactory.Text("¿Como desea ingresar?");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Invitado", Type = ActionTypes.ImBack, Value = "invitado", Image = "https://d2gg9evh47fn9z.cloudfront.net/800px_COLOURBOX8747303.jpg", ImageAltText = "R" },
                    new CardAction() { Title = "Miembro UTN", Type = ActionTypes.ImBack, Value = "utn", Image = "https://pbs.twimg.com/profile_images/1423739628176162820/nQh_hwvA_400x400.jpg", ImageAltText = "Y" },

                },
            };
            return reply;
        }

        private async Task<DialogTurnResult> MostarOpciones(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var option = new PromptOptions()
            {
                Prompt = OpcionInicio(),
                RetryPrompt = MessageFactory.Text("")
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), option, cancellationToken);
        }

        private async Task<DialogTurnResult> PromptStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(AuthUserDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> LoginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the token from the previous step. Note that we could also have gotten the
            // token directly from the prompt itself. There is an example of this in the next method.
            var tokenResponse = (TokenResponse)stepContext.Result;
            if (tokenResponse != null)
            {
                await OAuthHelpers.ListMeAsync(stepContext.Context, tokenResponse);

                var messageText = stepContext.Options?.ToString() ?? $"¿En qué puedo ayudarte hoy?\nDi algo como \"Quiero un libro de matematicas";
                var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);

                // await stepContext.Context.SendActivityAsync(MessageFactory.Text("You are now logged in."), cancellationToken);
                //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("¿En qué te puedo ayudar?") }, cancellationToken);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Login was not successful please try again."), cancellationToken);
            return await stepContext.EndDialogAsync();
        }

        //private async Task<DialogTurnResult> CommandStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    stepContext.Values["command"] = stepContext.Result;
        //    return await stepContext.BeginDialogAsync(nameof(OAuthPrompt), null, cancellationToken);
        //}

        private async Task<DialogTurnResult> ProcessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var luisResult = await _luisAIServices._luisRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            // var entities= luisResult.Entities;
            var topIntent = luisResult.GetTopScoringIntent();
            switch (topIntent.intent)
            {
                case "Saludar":
                    var getWeatherMessageText = "Uste saludo";
                    var getWeatherMessage = MessageFactory.Text(getWeatherMessageText, getWeatherMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(getWeatherMessage, cancellationToken);
                    break;
                //  case ReconocerIntecion.Intent.SugerenciaLibros:
                case "SugerenciaLibros":
                    var mensaje2 = "Uste Quiere libros";
                    var mensaj22 = MessageFactory.Text(mensaje2, mensaje2, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(mensaj22, cancellationToken);
                    break;
                case "Cancelar":
                    var mensaje3 = "Uste Cancelo";
                    var mensaje33 = MessageFactory.Text(mensaje3, mensaje3, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(mensaje33, cancellationToken);
                    break;

                case "None":
                    // var command = ((string)stepContext.Values["command"] ?? string.Empty).Trim().ToLowerInvariant();
                    var qnamakerResult = await _qnmakerService._qnamaker.GetAnswersAsync(stepContext.Context);
                    await qnMaker(stepContext, qnamakerResult, cancellationToken);
                    break;
            }
            return await stepContext.NextAsync(null, cancellationToken);

            /*
            //if (command == "logout")
            //{
            //    // The bot adapter encapsulates the authentication processes.
            //    var userTokenClient = stepContext.Context.TurnState.Get<UserTokenClient>();
            //    await userTokenClient.SignOutUserAsync(stepContext.Context.Activity.From.Id, ConnectionName, stepContext.Context.Activity.ChannelId, cancellationToken).ConfigureAwait(false);

            //    await stepContext.Context.SendActivityAsync(MessageFactory.Text("You have been signed out."), cancellationToken);
            //    return await stepContext.EndDialogAsync(cancellationToken);
            //}


            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(command)
                }
                );
            /* if (stepContext.Result != null)
             {
                 // We do not need to store the token in the bot. When we need the token we can
                 // send another prompt. If the token is valid the user will not need to log back in.
                 // The token will be available in the Result property of the task.
                 var tokenResponse = stepContext.Result as TokenResponse;

                 // If we have the token use the user is authenticated so we may use it to make API calls.
                 if (tokenResponse?.Token != null)
                 {
                     var command = ((string)stepContext.Values["command"] ?? string.Empty).Trim().ToLowerInvariant();

                     if (command == "me")
                     {
                         await OAuthHelpers.ListMeAsync(stepContext.Context, tokenResponse);
                     }
                     else if (command.StartsWith("email"))
                     {
                         await OAuthHelpers.ListEmailAddressAsync(stepContext.Context, tokenResponse);
                     }
                     else
                     {
                         await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your token is: {tokenResponse.Token}"), cancellationToken);
                     }
                 }
             }
             else
             {
                 await stepContext.Context.SendActivityAsync(MessageFactory.Text("We couldn't log you in. Please try again later."), cancellationToken);
             }

             return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);*/
        }

        private async Task qnMaker(WaterfallStepContext stepContext, QueryResult[] qnamakerResult, CancellationToken cancellationToken)
        {
            if (qnamakerResult.Any())
            {
                await stepContext.Context.SendActivityAsync("Biblioteca: " + qnamakerResult.First().Answer, cancellationToken: cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Lo siento no encontre una respuesta :(", cancellationToken: cancellationToken);
            }
        }
    }
}


