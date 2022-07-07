
using LibraryBotUtn.Dialogs;
using LibraryBotUtn.RecursosBot.Services;
using LibraryBotUtn.Services.BotConfig;
using LibraryBotUtn.Services.LuisAi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.LibraryBotUtn.QnA;

namespace LibraryBotUtn
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();
            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            services.AddSingleton<IDataservices, Dataservices>();
            services.AddSingleton<ILuisAIService, LuisAIService>();
            services.AddSingleton<IQnAMakerServices, QnAMakerServices>();
            services.AddSingleton<IStorage, MemoryStorage>();
            var storage = new MemoryStorage();
            var userState = new UserState(storage);
            services.AddSingleton(userState);
            services.AddSingleton<Microsoft.BotBuilderSamples.IStore, MemoryStore>();

            services.AddSingleton<UserState>();
            services.AddSingleton<ConversationState>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddSingleton<MainDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, LibraryBot<MainDialog>>();

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(options => options.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader());

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });


            // app.UseHttpsRedirection();
        }
    }
}
