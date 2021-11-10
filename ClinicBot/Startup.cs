// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.14.0

using ClinicBot.Data;
using ClinicBot.Dialogs;
using ClinicBot.Infrastructure.Luis;
using ClinicBot.Infrastructure.QnMakerAI;
using ClinicBot.Infrastructure.SendGridEmail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure.Blobs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicBot
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
            var storage = new BlobsStorage(
                Configuration.GetSection("StorageConnectionString").Value,
                Configuration.GetSection("StorageContainer").Value
                );

            var userState = new UserState(storage);
            services.AddSingleton(userState);

            var conversationState = new ConversationState(storage);
            services.AddSingleton(conversationState);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddDbContext<DataBaseService>(options => {
                options.UseCosmos(
                    Configuration["CosmosEndPoint"],
                    Configuration["CosmosKey"],
                    Configuration["CosmosDataBase"]
                );
            });
            services.AddScoped<IDataBaseService, DataBaseService>();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            services.AddSingleton<ISendGridEmailService, SendGridEmailService>();
            services.AddSingleton<ILuisService, LuisService>();
            services.AddSingleton<IQnMakerAIService, QnMakerAIService>();
            services.AddTransient<RootDialog>();
            
            services.AddTransient<IBot, ClinicBot<RootDialog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseEndpoints( endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
