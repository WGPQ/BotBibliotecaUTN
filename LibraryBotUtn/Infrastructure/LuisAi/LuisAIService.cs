using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;

namespace LibraryBotUtn.Services.LuisAi
{
    public class LuisAIService : ILuisAIService
    {
        public LuisRecognizer _luisRecognizer { get; }

        public LuisAIService(IConfiguration configuration)
        {
            var luisIsConfigured = !string.IsNullOrEmpty(configuration["LuisAppId"]) && !string.IsNullOrEmpty(configuration["LuisAPIKey"]) && !string.IsNullOrEmpty(configuration["LuisAPIHostName"]);
            if (luisIsConfigured)
            {
                var luisApplication = new LuisApplication(
                    configuration["LuisAppId"],
                    configuration["LuisAPIKey"],
                    configuration["LuisAPIHostName"]
                );

                var recognzerOptions = new LuisRecognizerOptionsV3(luisApplication)
                {
                    PredictionOptions = new Microsoft.Bot.Builder.AI.LuisV3.LuisPredictionOptions()
                    {
                        IncludeInstanceData = true
                    }

                };
                _luisRecognizer = new LuisRecognizer(recognzerOptions);
            }
        }

        // Devuelve verdadero si luis está configurado en appsettings.json e inicializado.
        public virtual bool IsConfigured => _luisRecognizer != null;



    }
}
