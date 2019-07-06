﻿using Application.Teams;
using Application.Teams.RaceConfigSeed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microwave;
using Microwave.Persistence.MongoDb;
using Microwave.UI;

namespace Teams.WriteHost.Startup
{
    public class Startup
    {
        readonly MicrowaveConfiguration _writeModelConfig = new MicrowaveConfiguration
        {
            ServiceName = "TeamService"
        };
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTransient<TeamCommandHandler>();
            services.AddTransient<RaceConfigSeedHandler>();
            services.AddMicrowaveUi();
            services.AddMicrowave(_writeModelConfig, new MongoDbPersistenceLayer(new MicrowaveMongoDb("Teams")));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var raceConfigSeedHandler = serviceScope.ServiceProvider.GetService<RaceConfigSeedHandler>();
                raceConfigSeedHandler.EnsureRaceConfigSeed().Wait();
            }

            app.UseMicrowaveUi();
            app.UseMvc();
        }
    }
}
