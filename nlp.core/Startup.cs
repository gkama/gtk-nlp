using System;
using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using nlp.data;
using nlp.services;
using nlp.services.text;
using System.Collections.Generic;

namespace nlp.core
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(typeof(INlpRepository<>), typeof(NlpRepository<>));
            services.AddScoped(typeof(IModelRepository<>), typeof(ModelRepository<>));
            services.AddScoped(typeof(ITextMiningRepository<>), typeof(TextMiningRepository<>));
            services.AddScoped<IStemmer, Stemmer>();
            services.AddScoped<ISummarizer, Summarizer>();
            services.AddSingleton(typeof(Models<>));

            services.AddAuthorization();
            services.AddLogging();
            services.AddHealthChecks();
            services.AddMemoryCache();

            services.AddControllers();
            services.AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.IgnoreNullValues = true;
                    o.JsonSerializerOptions.WriteIndented = true;
                    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseNlpException();
            app.UseHealthChecks("/ping");

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(e =>
            {
                e.MapControllers();
            });
        }
    }
}
