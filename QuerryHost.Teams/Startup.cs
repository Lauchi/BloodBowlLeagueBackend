﻿using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microwave.DependencyInjectionExtensions;
using MongoDB.Bson.Serialization;
using Querries.Teams;
using Querries.Teams.DomainEvents;

namespace QuerryHost.Teams
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
            services.AddMvc();
            services.AddMicrowaveReadModels(Assembly.GetAssembly(typeof(TeamReadModel)), Configuration);

            BsonClassMap.RegisterClassMap<TeamCreated>();
            BsonClassMap.RegisterClassMap<RaceCreated>();
            BsonClassMap.RegisterClassMap<PlayerBought>();

            BsonClassMap.RegisterClassMap<TeamReadModel>();
            BsonClassMap.RegisterClassMap<CounterQuery>();
            BsonClassMap.RegisterClassMap<RaceReadModel>();
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

            app.RunMicrowaveQueries();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}