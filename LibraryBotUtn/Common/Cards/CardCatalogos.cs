using LibraryBotUtn.Common.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Common.Cards
{
    public class CardCatalogos
    {
        public static async Task ToShowCatalogo(List<SetsModels> catalogos, DialogContext dc, CancellationToken cancellationToken)
        {
 
            await dc.Context.SendActivityAsync(activity: Opciones(catalogos), cancellationToken);
        }
        private static Activity Opciones(List<SetsModels> listcatalogos)

        {
            var optionsAttachments = new List<Attachment>();

            foreach (var item in listcatalogos)
            {
                var card = new HeroCard
                {
                    Title = item.setName,
                    Subtitle = "35",
                    Images = new List<CardImage> { new CardImage("http://repositorio.utn.edu.ec:8080/retrieve/a30ce849-b0fc-476d-8588-335163151cd5") },
                    Buttons = new List<CardAction>()
                   {
                 new CardAction(){Title="Opciones",Value="Opciones",Type=ActionTypes.ImBack},
                 new CardAction(){Title="Ver informacion",Value="http://repositorio.utn.edu.ec:8080/retrieve/a30ce849-b0fc-476d-8588-335163151cd5",Type=ActionTypes.OpenUrl}
                 }
                };
                optionsAttachments.Add(card.ToAttachment());
            }

            var reply = MessageFactory.Attachment(optionsAttachments);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            return reply as Activity;
        }
    }
}
