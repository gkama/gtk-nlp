using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

using nlp.data;

namespace nlp.services
{
    public class ModelRepository<T> : IModelRepository<T>
        where T : IModel<T>, new()
    {
        private readonly ILogger<ModelRepository<T>> _logger;
        private readonly Models<T> _models;

        public ModelRepository(ILogger<ModelRepository<T>> logger, Models<T> models)
        {
            _logger = logger;
            _models = models;
        }
    }
}
