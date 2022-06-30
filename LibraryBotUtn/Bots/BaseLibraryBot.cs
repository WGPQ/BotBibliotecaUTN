// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryBotUtn.Common.Models;
using LibraryBotUtn.RecursosBot;
using LibraryBotUtn.Services.BotConfig;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace LibraryBotUtn.Bots
{
    // This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
    // to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
    // each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
    // The ConversationState is used by the Dialog system. The UserState isn't, however, it might have been used in a Dialog implementation,
    // and the requirement is that all BotState objects are saved at the end of a turn.
    public class BaseLibraryBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly BotState ConversationState;
        protected readonly Dialog _dialog;
        protected readonly ILogger _logger;
        protected readonly BotState _botState;
        private readonly IDataservices _dataservices;
        private readonly Microsoft.BotBuilderSamples.IStore _store;


        public BaseLibraryBot(Microsoft.BotBuilderSamples.IStore store, ConversationState conversationState, BotState userState, T dialog, ILogger<BaseLibraryBot<T>> logger, IDataservices dataservices)
        {
            ConversationState = conversationState;
            _botState = userState;
            _dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
            _logger = logger;
            _store = store;
            _dataservices = dataservices;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _botState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");
            // Create the storage key for this conversation.


            var key = $"{turnContext.Activity.ChannelId}/conversations/{turnContext.Activity.Conversation?.Id}";

            
           
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
            

            // Run the Dialog with the new message Activity.
            await _dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);

        }
    }
}