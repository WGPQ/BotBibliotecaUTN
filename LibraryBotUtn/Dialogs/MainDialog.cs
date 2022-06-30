using LibraryBotUtn.Common.Models;
using LibraryBotUtn.Dialogs.Authenticate;
using LibraryBotUtn.Services.BotConfig;
using LibraryBotUtn.Services.LuisAi;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.LibraryBotUtn.QnA;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static string EMAIL_USER_PROMPT = "EMAIL_USER_PROMPT";
        private string WATER_fULL_STEP_REGISTER = "REGISTER_USER";
        private string WATER_fULL_STEP_CONSULTA = "CONSULTA";
        private string WATER_fULL_STEP_INITIAL = "WATER_fULL_STEP_INITIAL";
        public static string idBotuser = "userBot";
        protected readonly UserState _botState;



        public MainDialog(ILuisAIService luisAIServices, UserState botState, IQnAMakerServices qnmakerService, ILogger<MainDialog> logger, IDataservices dataservices) : base(nameof(MainDialog))
        {
            _luisAIServices = luisAIServices;
            _qnmakerService = qnmakerService;
            _logger = logger;
            _dataservices = dataservices;
            _botState = botState;

            var waterfallSteps = new WaterfallStep[]
            {
                bienvenida,
                optionUser
            };
            AddDialog(new WaterfallDialog(INITIAL_WATERFALL, waterfallSteps));
            AddDialog(new AuthUserDialog(dataservices, botState, qnmakerService));


            InitialDialogId = INITIAL_WATERFALL;



        }

        private async Task<DialogTurnResult> bienvenida(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = await _dataservices.AuthRepositori.Auth("bibliochatutn@outlook.com");
            TokenEntity tokenResult = new TokenEntity();
            if (result.exito)
            {
                tokenResult = TokenEntity.formJson(Convert.ToString(result.data));
            }
            var tokenBotAccessors = _botState.CreateProperty<TokenEntity>(nameof(TokenEntity));
            var tokenBotUTN = await tokenBotAccessors.GetAsync(stepContext.Context, () => new TokenEntity());
            tokenBotUTN = tokenResult;


            UsuarioEntity userBot = await _dataservices.AuthRepositori.GetUser(tokenBotUTN);
            var userStateAccessors = _botState.CreateProperty<UsuarioEntity>(idBotuser);
            var userProfile = await userStateAccessors.GetAsync(stepContext.Context, () => new UsuarioEntity());
            userProfile = userBot;
            await tokenBotAccessors.SetAsync(stepContext.Context, tokenBotUTN);
            await userStateAccessors.SetAsync(stepContext.Context, userProfile);
            await _botState.SaveChangesAsync(stepContext.Context);
            var fraceEntity = await _dataservices.FracesRepositori.Frace("Bienvenida", tokenBotUTN.token);
            if (fraceEntity != null)
            {

                var reply = MessageFactory.Attachment(Opciones(fraceEntity.frace).ToAttachment());
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
        }
          
      return await stepContext.ContinueDialogAsync(cancellationToken);
    }

        private async Task<DialogTurnResult> optionUser(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var opt = stepContext.Context.Activity.Text;
            switch (opt)
            {
                case "iniciar":

                    return await GoToAuthUser(stepContext, cancellationToken);
                case "terminos":
                   await stepContext.Context.SendActivityAsync(MessageFactory.Text(opt), cancellationToken);
                    break;
                case "acerca":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(opt), cancellationToken);
                    break;
                default:
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Eleccion incorrecta"), cancellationToken);
                    break;
            }
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> GoToAuthUser(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(AuthUserDialog),null,cancellationToken);
        }
        private HeroCard Opciones(string subtitle)

        {
            var card = new HeroCard
            {
                Title = "Biblioteca Universitaria UTN",
                Text = subtitle,
                Images = new List<CardImage> { new CardImage("https://i.ytimg.com/vi/ucj5hAICSUE/maxresdefault.jpg") },
                Buttons = new List<CardAction>
              {
                  // Note that some channels require different values to be used in order to get buttons to display text.
                  // In this code the emulator is accounted for with the 'title' parameter, but in other channels you may
                  // need to provide a value for other parameters like 'text' or 'displayText'.
                  new CardAction(ActionTypes.ImBack, title: "1. Iniciar", value: "iniciar"),
                  new CardAction(ActionTypes.ImBack, title: "2. Términos y condiciones", value: "terminos"),
                  new CardAction(ActionTypes.ImBack, title: "3. Acerca del bot", value: "acerca"),
              },
            };
            return card;
        }
    }
    }
