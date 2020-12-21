using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acre.Backend.Ons.Abstractions;
using Acre.Backend.Ons.Data;
using Acre.Backend.Ons.Models.Configurations;
using Acre.Backend.Ons.Services;
using Acre.Backend.Ons.Services.Parsers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Acre.Backend.Ons
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
            services.AddControllers();

            // Config objects
            services.Configure<DatasetConfig>(Configuration.GetSection("Datasets"));
            services.Configure<PostcodesIOConfig>(Configuration.GetSection("PostcodesIO"));

            // Database
            var dbConnection = Configuration.GetSection("ConnectionString").Get<string>();
            services.AddDbContext<OnsDbContext>(opts => {
                opts.UseSqlite(dbConnection);
                opts.UseLazyLoadingProxies();
            });

            // Services
            services.AddHttpClient<IHttpClient, HttpClientHandler>();
            services.AddScoped<IRegionLookupService, RegionLookupService>();
            services.AddScoped<IOnsRepository, OnsRepository>();
            services.AddSingleton<ICaseRepository, CaseRepository>();

            // Parsing
            var isParsingEnabled = Configuration.GetValue<bool>("seed");
            if(isParsingEnabled) {
                services.AddScoped<IDatasetParser, DatasetParserAge>();
                services.AddScoped<IDatasetParser, DatasetParserRegion>();
                services.AddScoped<IDatasetParser, DatasetParserComposition>();
                services.AddScoped<IParserContext, ParserContext>();
                services.AddScoped<IOnsDbSeeder, OnsDbSeeder>();
                services.AddSingleton<IParserCache, ParserCache>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "/api/{controller}/{action}/{id?}");
                });
        }
    }
}
