using LibraryBotUtn.Services.BotConfig;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBotUtn.Dialogs.Authenticate
{
    public class RegisterUserDialog : ComponentDialog
    {
        private readonly IDataservices _dataservices;
        private static string OPTION_ENTER_PROMPT = "OPTION_ENTER_PROMPT";

        public RegisterUserDialog(IDataservices dataservices) : base(nameof(RegisterUserDialog))
        {
            _dataservices = dataservices;
          
        }


    }
}
