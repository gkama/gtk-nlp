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
        public IEnumerable<IModelSettings<T>> GetModels();
        public IModelSettings<T> GetModel(string Id);
    }
}
