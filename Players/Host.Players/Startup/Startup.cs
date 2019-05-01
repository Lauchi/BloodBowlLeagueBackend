﻿using Application.Players;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microwave;
using Microwave.Application;
using ServiceConfig;

namespace Host.Players.Startup
{
    public class Startup
    {
        readonly MicrowaveConfiguration _config = new MicrowaveConfiguration
        {
            DatabaseConfiguration = new DatabaseConfiguration() { DatabaseName = "Players"},
            ServiceLocations = ServiceConfiguration.ServiceAdresses,
            ServiceName = "PlayerService"
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddMicrowave(_config);

            services.AddTransient<PlayerConfigSeedHandler>();
            services.AddTransient<PlayerCommandHandler>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var raceConfigSeedHandler = serviceScope.ServiceProvider.GetService<PlayerConfigSeedHandler>();
                raceConfigSeedHandler.EnsurePlayerConfigSeed().Wait();
            }

            app.RunMicrowaveQueries();
            app.UseMvc();
        }
    }
}
