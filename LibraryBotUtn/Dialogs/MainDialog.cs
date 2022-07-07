using LibraryBotUtn.Common.Cards;
using LibraryBotUtn.Common.Models;
using LibraryBotUtn.Common.Models.BotState;
using LibraryBotUtn.Dialogs.Authenticate;
using LibraryBotUtn.Services.BotConfig;
using LibraryBotUtn.Services.LuisAi;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.LibraryBotUtn.QnA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        public readonly ILuisAIService _luisAIServices;
        public readonly IQnAMakerServices _qnmakerService;
        private readonly IDataservices _dataservices;
        private readonly ILogger _logger;
        private static string INITIAL_WATERFALL = "INITIAL_WATERFALL_STEPS";
        public static string keyBot = "userBot";
        public static string keyClient = "userClient";
        public static BotVerificadoEntity _bot = new BotVerificadoEntity();
        private readonly IStatePropertyAccessor<AuthStateModel> _botState;
        private readonly IStatePropertyAccessor<AuthStateModel> _userState;



        public MainDialog(ILuisAIService luisAIServices, UserState botState, UserState userState, IQnAMakerServices qnmakerService, ILogger<MainDialog> logger, IDataservices dataservices) : base(nameof(MainDialog))
        {
            _luisAIServices = luisAIServices;
            _qnmakerService = qnmakerService;
            _logger = logger;
            _dataservices = dataservices;
            _botState = botState.CreateProperty<AuthStateModel>(keyBot);
            _userState = userState.CreateProperty<AuthStateModel>(keyClient);


            var waterfallSteps = new WaterfallStep[]
            {

                //IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            };
            AddDialog(new WaterfallDialog(INITIAL_WATERFALL, waterfallSteps));
            AddDialog(new AuthUserDialog(dataservices, userState, qnmakerService, luisAIServices));
            AddDialog(new FinalizarDialog(userState));
            AddDialog(new CalificarDialog());
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));


            InitialDialogId = INITIAL_WATERFALL;



        }



        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _bot = await _dataservices.AuthRepositori.Auth("bibliochatutn@outlook.com");
            if (_bot.token != null)
            {
                var botStateData = await _botState.GetAsync(stepContext.Context, () => new AuthStateModel());
            }

            var fraceEntity = await _dataservices.FracesRepositori.Frace("Bienvenida", _bot.token);
            if (fraceEntity != null)
            {

                //await OnBoarding.ToShow(fraceEntity.frace, stepContext, cancellationToken);
            }

            return await stepContext.BeginDialogAsync(nameof(AuthUserDialog), null, cancellationToken);
            //var weekLaterDate = DateTime.Now.AddDays(7).ToString("MMMM d, yyyy");
            //var messageText = stepContext.Options?.ToString() ?? $"What can I help you with today?\nSay something like \"Book a flight from Paris to Berlin on {weekLaterDate}\"";
            //var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }




        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var luisResult = await _luisAIServices._luisRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            return await ManageIntentions(stepContext, luisResult, cancellationToken);
        }

        private async Task<DialogTurnResult> ManageIntentions(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var userStateData = await _userState.GetAsync(stepContext.Context, () => new AuthStateModel());

            if (!userStateData.IsAutenticate)
            {
                return await stepContext.BeginDialogAsync(nameof(AuthUserDialog), null, cancellationToken);
            }

            var topIntent = luisResult.GetTopScoringIntent();
            switch (topIntent.intent)
            {
                case "Iniciar":
                    return await IntentIniciar(stepContext, userStateData, luisResult, cancellationToken);
                case "Terminos":
                    await IntentTerminos(stepContext, luisResult, cancellationToken);
                    break;
                case "Acerca":
                    await IntentAcerca(stepContext, luisResult, cancellationToken);
                    break;
                case "Saludar":
                    await IntentSaludar(stepContext, luisResult, cancellationToken);
                    break;
                case "Opciones":
                    await IntentOpciones(stepContext, luisResult, cancellationToken);
                    break;
                case "Repositorio":
                    await IntentRepositorio(stepContext, luisResult, cancellationToken);
                    break;
                case "Agradecer":
                    await IntentAgradecer(stepContext, luisResult, cancellationToken);
                    break;
                case "Calificar":
                    return await IntentCalificar(stepContext, luisResult, cancellationToken);
                case "Despedirse":
                    await IntentDespedirse(stepContext, luisResult, cancellationToken);
                    break;
                case "None":
                    await IntentNone(stepContext, luisResult, cancellationToken);
                    break;
                default:
                    await IntentDefault(stepContext, luisResult, cancellationToken);
                    break;
            }
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task IntentRepositorio(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Este es nuestro catalogo", cancellationToken: cancellationToken);

        }

        private async Task IntentOpciones(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync($"Aquí tengo mis opciones", cancellationToken: cancellationToken);
            await MenuOptions.ToShow(stepContext, cancellationToken);
            //await stepContext.Context.SendActivityAsync("Este es nuestro menú.", cancellationToken: cancellationToken);

        }

        private async Task IntentDefault(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Disculpa no entendi. ¿Puesdes escribirlo de otra manera?", cancellationToken: cancellationToken);

        }

        private async Task IntentDespedirse(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Espero verte pronto", cancellationToken: cancellationToken);

        }

        private async Task<DialogTurnResult> IntentIniciar(WaterfallStepContext stepContext, AuthStateModel userStateData, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            if (!userStateData.IsAutenticate)
            {
                return await stepContext.BeginDialogAsync(nameof(AuthUserDialog), null, cancellationToken);
            }
            return await stepContext.BeginDialogAsync(nameof(FinalizarDialog), null, cancellationToken);
        }



        private async Task IntentTerminos(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Estos son nuestros terminos", cancellationToken: cancellationToken);
        }

        private async Task IntentAcerca(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Esto es Acerca de:", cancellationToken: cancellationToken);

        }

        private async Task IntentSaludar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Hola, que gusto verte.", cancellationToken: cancellationToken);

        }


        private async Task IntentAgradecer(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("No te preocupes me gusta ayudar.", cancellationToken: cancellationToken);

        }

        private async Task<DialogTurnResult> IntentCalificar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(CalificarDialog), null, cancellationToken);
        }

        private async Task IntentNone(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var resultQnA = await _qnmakerService._qnamaker.GetAnswersAsync(stepContext.Context);

            var score = resultQnA.FirstOrDefault()?.Score;
            string response = resultQnA.FirstOrDefault()?.Answer;

            if (score >= 0.5)
            {
                await stepContext.Context.SendActivityAsync(response, cancellationToken: cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync($"No entiendo lo que me dices", cancellationToken: cancellationToken);
                await Task.Delay(1000);
                await IntentOpciones(stepContext, luisResult, cancellationToken);
            }

        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

    }
}
