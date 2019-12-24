using System;
using System.Collections.Generic;
using System.Text.Json;

using nlp.data;

namespace nlp.services
{
    public interface INlpRepository<T>
    {
        public string Categorize(dynamic Request);
        public IModelSettings<T> Parse(dynamic Request);
        public IEnumerable<T> GetModels();
        public IModel<T> GetModel(string Id);
        public IEnumerable<IModelSettings<T>> GetModelsSettings();
        public IModelSettings<T> GetModelSettings(string Id);
    }
}
