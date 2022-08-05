
using LibraryBotUtn.Common.Cards;
using LibraryBotUtn.Common.Models;
using LibraryBotUtn.Common.Models.BotState;
using LibraryBotUtn.RecursosBot;
using LibraryBotUtn.Services.BotConfig;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Dialogs
{
    public class LibraryBot<T> : ActivityHandler where T : Dialog
    {
        public const string WelcomeText = @"Para poder acceder al servcio debe eligir cual sera su modo de ingreso, para obtener mas informacion hacerca del modo de ingreso viste https://wwww.mododeingreso.com ";
        private readonly IDataservices _dataservices;
        protected readonly BotState _conversationState;
        protected readonly BotState _userState;
        protected readonly Dialog _dialog;
        private readonly Microsoft.BotBuilderSamples.IStore _store;
        public static string keyClient = "userClient";
        private readonly IStatePropertyAccessor<AuthStateModel> _clientState;
        public static string keyBot = "userBot";
        public static BotVerificadoEntity _bot = new BotVerificadoEntity();
        public readonly IStatePropertyAccessor<AuthStateModel> _botState;

        public LibraryBot(ConversationState conversationState, UserState userState, UserState botState, UserState clientState, T dialog, IDataservices dataservices, Microsoft.BotBuilderSamples.IStore store)

        {
            _conversationState = conversationState;
            _clientState = clientState.CreateProperty<AuthStateModel>(keyClient);
            _botState = botState.CreateProperty<AuthStateModel>(keyBot);
            _userState = userState;
            _dialog = dialog;
            _dataservices = dataservices;
            _store = store;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    _bot = await _dataservices.AuthRepositori.Auth("bibliochatutn@outlook.com");
                    if (_bot.token != null)
                    {
                        var botStateData = await _botState.GetAsync(turnContext, () => new AuthStateModel());
                    }

                    var fraceEntity = await _dataservices.FracesRepositori.Frace("Introduccion", _bot.token);
                    if (fraceEntity != null)
                    {

                        await OnBoarding.ToShow(fraceEntity.frace, turnContext, cancellationToken);
                    }

                  //  await turnContext.SendActivityAsync(MessageFactory.Text($"Bienvenido"), cancellationToken);

                   // await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
                }
            }
        }

      

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);

        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            var key = $"{turnContext.Activity.ChannelId}/conversations/{turnContext.Activity.Conversation?.Id}";

            var clientStateData = await _clientState.GetAsync(turnContext, () => new AuthStateModel());


            if (clientStateData.IsAutenticate)
            {

                // The execution sits in a loop because there might be a retry if the save operation fails.
                while (true)
                {
                    // Load any existing state associated with this key
                    var (oldState, etag) = await _store.LoadAsync(key);

                    // Run the dialog system with the old state and inbound activity, the result is a new state and outbound activities.
                    var (activities, newState) = await DialogHost.RunAsync(_dialog, turnContext.Activity, oldState, cancellationToken);

                    // Save the updated state associated with this key.
                    var success = await _store.SaveAsync(key, newState, etag);

                    // Following a successful save, send any outbound Activities, otherwise retry everything.
                    if (success)
                    {
                        if (activities.Any())
                        {
                            // This is an actual send on the TurnContext we were given and so will actual do a send this time.
                            //await turnContext.SendActivitiesAsync(activities, cancellationToken);

                        }

                        break;
                    }
                }
            }
            await _dialog.RunAsync(
              turnContext,
              _conversationState.CreateProperty<DialogState>(nameof(DialogState)),
              cancellationToken
            );
        }
        private Attachment CreateAdaptiveCardAttachment()
        {

            var cardResourcePath = GetType().Assembly.GetManifestResourceNames().First(name => name.EndsWith("welcomeCard.json"));

            using (var stream = GetType().Assembly.GetManifestResourceStream(cardResourcePath))
            {
                using (var reader = new StreamReader(stream))
                {
                    var adaptiveCard = reader.ReadToEnd();
                    return new Attachment()
                    {
                        ContentType = "application/vnd.microsoft.card.adaptive",
                        Content = JsonConvert.DeserializeObject(adaptiveCard),
                    };
                }
            }
        }
        private static Attachment GetInlineAttachment()
        {
            var imagePath = Path.Combine(Environment.CurrentDirectory, @"Resources", "logo.png");
            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            return new Attachment
            {

                Name = @"Resources\architecture-resize.png",
                ContentType = "image/png",
                ContentUrl = $"data:image/png;base64,{imageData}",
            };
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
