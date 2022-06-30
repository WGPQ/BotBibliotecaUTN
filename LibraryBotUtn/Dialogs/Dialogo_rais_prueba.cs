using LibraryBotUtn.Common.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Dialogs
{
    public class Dialogo_rais_prueba : ComponentDialog
    {

        public Dialogo_rais_prueba()
        {


            var waterfallStep = new WaterfallStep[]
            {
                SetName,
                SetAge,
                ShowDate
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), ValidateAge));
        }

        private async Task<bool> ValidateAge(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            return await Task.FromResult(
                promptContext.Recognized.Succeeded &&
                promptContext.Recognized.Value > 0 &&
                promptContext.Recognized.Value < 150
                );
        }


        private async Task<DialogTurnResult> SetName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //await stepContext.Context.SendActivityAsync("Para iniciar la cnversacion necesitamos alguno de tus datos ", cancellationToken: cancellationToken);
            //await Task.Delay(1000);
            //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Porfavor ingresa tu nombre") }, cancellationToken);
            List<SetsModels> sets = new List<SetsModels>();
            sets = await new BusquedaTitulo().BuscarTitulo();

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("What card would you like to see? You can click or type the card name"),
                RetryPrompt = MessageFactory.Text("That was not a valid choice, please select a card or number from 1 to 9."),
                Choices = GetChoices(sets),
            };

          return  await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> SetAge(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var name = stepContext.Context.Activity.Text;
           
            //for (int i = 0; i < 5; i++)
            //{
            //    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(sets[i].setName) }, cancellationToken);
            //}
            //foreach (var item in sets)
            //{
            //    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(item.setName) }, cancellationToken);
            //}
            return await stepContext.PromptAsync(
                nameof(NumberPrompt<int>),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text($"Bien {name} ahora ingresa tu edad"),
                    RetryPrompt = MessageFactory.Text($"{name}, Por favor ingresa una edad valida.")
                },
                cancellationToken
                );
        }

        private async Task<DialogTurnResult> ShowDate(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Genial gracias por registrar todos tus datos", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private IList<Choice> GetChoices(List<SetsModels> sets)
        {
            var cardOptions = new List<Choice>();
            for (int i = 0; i < 5; i++)
            {
                cardOptions.Add(
                    new Choice()
                    {
                        Value = sets[i].setSpec,
                        Synonyms = new List<string>() { sets[i].setName }
                    }
                    );
            };

            return cardOptions;
        }

    }
}
