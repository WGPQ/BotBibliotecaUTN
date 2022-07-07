using LibraryBotUtn.Common.Cards;
using LibraryBotUtn.Services.BotConfig;
using LibraryBotUtn.Services.LuisAi;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.LibraryBotUtn.QnA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Dialogs
{
    public class MenuDialog: ComponentDialog
    {
        private string WATER_fULL_STEP_MAIN_MENU = "MAIN_MENU";

        public readonly IQnAMakerServices _qnmakerService;
        private readonly IDataservices _dataservices;
        public readonly ILuisAIService _luisAIServices;
        public MenuDialog(IDataservices dataservices, IQnAMakerServices qnmakerService, ILuisAIService luisAIServices)
        {
            _luisAIServices = luisAIServices;
            _dataservices = dataservices;
            _qnmakerService = qnmakerService;
            var waterfullStepMainMenu = new WaterfallStep[]
            {
            
                InitialProcess,
                FinalProcess
            };
            AddDialog(new WaterfallDialog(WATER_fULL_STEP_MAIN_MENU, waterfullStepMainMenu));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt("algo mas"));
  
            InitialDialogId = WATER_fULL_STEP_MAIN_MENU;

        }

        private async Task<DialogTurnResult> InitialProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Elija"),cancellationToken);

            var luisResult = await _luisAIServices._luisRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            return await ManageIntentions(stepContext, luisResult, cancellationToken);
        }

        private async Task<DialogTurnResult> ManageIntentions(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var topIntent = luisResult.GetTopScoringIntent();
            switch (topIntent.intent)
            {
                case "Saludar":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Hola"), cancellationToken);
                    //  await IntentSaludar(stepContext, luisResult, cancellationToken);
                    break;
                case "Agradecer":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Agradeciste"), cancellationToken);
                    // await IntentAgradecer(stepContext, luisResult, cancellationToken);
                    break;
                default:
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("No entendi"), cancellationToken);
                    break;
            }
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        

        private async Task<DialogTurnResult> Eleccion(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var opt = stepContext.Context.Activity.Text;
            switch (opt)
            {
                case "Repositorio":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Repositorio"), cancellationToken);
                    break;
                case "Biblioteca":
                    return await stepContext.BeginDialogAsync("WATER_fULL_STEP_CONSULTA", null, cancellationToken);
                case "Ayuda":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Ayuda"), cancellationToken);
                    break;
            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }


    }
}
