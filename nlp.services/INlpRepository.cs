using System;
using System.Collections.Generic;
using System.Text.Json;

using nlp.data;

namespace nlp.services
{
    public interface INlpRepository<T>
    {
        public IEnumerable<ICategory> Categorize(dynamic Request, string Id = null, bool Summarize = false);
        public IModelSettings<T> Parse(dynamic Request, string Id = null);
        public IEnumerable<T> GetModels();
        public IModel<T> GetModel(string Id);
        public IModel<T> GetAnyModel(string Id);
        public IEnumerable<IModelSettings<T>> GetModelsSettings();
        public IModelSettings<T> GetModelSettings(string Id);
        IModelSettings<T> GetModelSettingsByModelId(string Id);
        public object CategorizeSample();
    }
}
