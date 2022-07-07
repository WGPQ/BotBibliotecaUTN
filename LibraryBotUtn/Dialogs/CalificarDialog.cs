using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Dialogs
{
    public class CalificarDialog : ComponentDialog
    {
       public CalificarDialog()
        {
            var waterfallSteps = new WaterfallStep[]
           {
                ToShowButton,
                ValidateOption
           };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
        }

        private async Task<DialogTurnResult> ToShowButton(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                    nameof(TextPrompt),
                    new PromptOptions
                    {
                        Prompt = CreateButtonsCalification()
                    },
                    cancellationToken
                  );
        }

        private async Task<DialogTurnResult> ValidateOption(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var option = stepContext.Context.Activity.Text;
            await stepContext.Context.SendActivityAsync($"Gracias por tu {option}", cancellationToken: cancellationToken);
            //GUARDAR CALIFICACIÓN
           // await SaveQualification(stepContext, option);
            await Task.Delay(1000);
            await stepContext.Context.SendActivityAsync("¿En qué más puedo ayudarte?", cancellationToken: cancellationToken);

            return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
        }
        public Activity CreateButtonsCalification()
        {
            var reply = MessageFactory.Text("Califícame por favor");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){Title = "1⭐", Value = "1⭐", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "2⭐", Value = "2⭐", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "3⭐", Value = "3⭐", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "4⭐", Value = "4⭐", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "5⭐", Value = "5⭐", Type = ActionTypes.ImBack}
                }
            };
            return reply as Activity;
        }
    }
}
