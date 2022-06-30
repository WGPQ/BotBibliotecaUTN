using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBotUtn.Common.Models
{
    public class SlotDetails
    {
        public SlotDetails(string name, string dialogId, string prompt = null, string retryPrompt = null)
          : this(name, dialogId, new PromptOptions
          {
              Prompt = MessageFactory.Text(prompt),
              RetryPrompt = MessageFactory.Text(retryPrompt),
          })
        {
        }

        public SlotDetails(string name, string dialogId, PromptOptions options)
        {
            Name = name;
            DialogId = dialogId;
            Options = options;
        }

        public string Name { get; set; }

        public string DialogId { get; set; }

        public PromptOptions Options { get; set; }
    }
}
