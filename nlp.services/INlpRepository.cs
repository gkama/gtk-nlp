using System;
using System.Collections.Generic;
using System.Text.Json;

using nlp.data;

namespace nlp.services
{
    public interface INlpRepository<T>
    {
        public string Categorize(dynamic Request);
        public IModelSettings<Model> Parse(dynamic Request);
        public IEnumerable<IModelSettings<Model>> GetModels();
        public IModelSettings<Model> GetModel(string Id);
    }
}
