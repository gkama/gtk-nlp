using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using nlp.data;
using nlp.services;
using nlp.services.text;

namespace nlp.tests
{
    public class Helper<T>
    {
        private IServiceProvider Provider()
        {
            var services = new ServiceCollection();

            services.AddScoped(typeof(INlpRepository<>), typeof(NlpRepository<>));
            services.AddScoped(typeof(IModelRepository<>), typeof(ModelRepository<>));
            services.AddScoped(typeof(ITextMiningRepository<>), typeof(TextMiningRepository<>));
            services.AddScoped<IStemmer, Stemmer>();
            services.AddScoped<ISummarizer, Summarizer>();
            services.AddSingleton(typeof(Models<>));

            services.AddLogging();
            services.AddMemoryCache();

            return services.BuildServiceProvider();
        }

        public T GetService()
        {
            return Provider().GetService<T>();
        }
    }
}
