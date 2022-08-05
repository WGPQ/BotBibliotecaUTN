using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Common.Cards
{
    public class OnBoarding
    {
        public static async Task ToShow(string subtitle, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Attachment(Opciones(subtitle).ToAttachment());
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
        private static HeroCard Opciones(string subtitle)

        {
            var card = new HeroCard
            {
                Title = "Biblioteca Universitaria UTN",
                Text = subtitle,
                Images = new List<CardImage> { new CardImage("https://i.ytimg.com/vi/ucj5hAICSUE/maxresdefault.jpg") },
                Buttons = new List<CardAction>
              {
                  new CardAction(){ Title="Facebook",Value="https://www.facebook.com/utnbiblioteca",Type= ActionTypes.OpenUrl},
                  new CardAction(){ Title="Youtube",Value="https://www.youtube.com/channel/UCLVtk5s9uj9FcTwOdGQYseA",Type= ActionTypes.OpenUrl},
                  new CardAction(){ Title="Sitio Web",Value="https://biblioteca.utn.edu.ec",Type= ActionTypes.OpenUrl},
              },
            };
            return card;
        }
    }
}
