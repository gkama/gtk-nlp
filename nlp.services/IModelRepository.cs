using System;
using System.Collections.Generic;
using System.Text;

using nlp.data;

namespace nlp.services
{
    public interface IModelRepository<T>
        where T : IModel<T>, new()
    {
        public Models<T> _models { get; }

        public IEnumerable<T> GetModels();
        public IModel<T> GetModel(string Id);
        public IModel<T> GetModel(Guid PublicKey);
        public IModel<T> GetAnyModel(string Id);
        public IEnumerable<IModelSettings<T>> GetModelsSettings();
        public IModelSettings<T> GetModelSettings(string Id);
        public IModelSettings<T> GetModelSettingsByModelId(string Id);
        public T AddModel(dynamic Request);
        public T AddModel(INlpRequest<T> Request);
        public T AddModel(T Model);
    }
}
