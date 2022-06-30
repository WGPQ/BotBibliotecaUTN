
using LibraryBotUtn.Bots;
using LibraryBotUtn.Common.Models;
using LibraryBotUtn.Services.BotConfig;
using LibraryBotUtn.Services.BotConfig.Repositories;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Dialogs
{
    public class LibraryBot<T> : BaseLibraryBot<T> where T : Dialog
    {
        public const string WelcomeText = @"Para poder acceder al servcio debe eligir cual sera su modo de ingreso, para obtener mas informacion hacerca del modo de ingreso viste https://wwww.mododeingreso.com ";
        private readonly IDataservices _dataservices;
        protected readonly BotState _botState;
        public static string idBotuser = "userBot";
        public LibraryBot(Microsoft.BotBuilderSamples.IStore store, ConversationState conversationState, UserState botState, T dialog, ILogger<BaseLibraryBot<T>> logger, IDataservices dataservices, UserState tokenBot)
           : base(store, conversationState, botState, dialog, logger, dataservices)
        {
            this._dataservices = dataservices;
            this._botState = botState;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                   
                    await _dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
                }
            }
        }

        //protected override async Task OnTokenResponseEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    _logger.LogInformation("Running dialog with Token Response Event Activity.");

        //    // Run the Dialog with the new Token Response Event Activity.
        //    await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        //}
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
