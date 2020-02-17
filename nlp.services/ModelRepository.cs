using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net;
using System.Linq;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using nlp.data;

namespace nlp.services
{
    public class ModelRepository<T> : IModelRepository<T>
        where T : IModel<T>, new()
    {
        private readonly ILogger<ModelRepository<T>> _logger;
        private readonly IMemoryCache _cache;

        public Models<T> _models { get; }

        public ModelRepository(ILogger<ModelRepository<T>> logger, IMemoryCache cache, Models<T> models)
        {
            _logger = logger;
            _cache = cache;
            _models = models;
        }

        public IEnumerable<T> GetModels()
        {
            return _cache.GetOrCreate(_models.All, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return _models.All;
            });
        }
        public IModel<T> GetModel(string Id)
        {
            var model = _cache.GetOrCreate(Id, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return _models.All
                    .FirstOrDefault(x => x.Id == Id);
            });

            if (model == null)
            {
                Guid.TryParse(Id, out Guid PublicKey);

                return GetModel(PublicKey);
            }

            return model;
        }
        public IModel<T> GetModel(Guid PublicKey)
        {
            return _cache.GetOrCreate(PublicKey, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return _models.All
                    .FirstOrDefault(x => x.PublicKey == PublicKey);
            });
        }
        public IModel<T> GetAnyModel(string Id)
        {
            return _cache.GetOrCreate(Id, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                var models = new Stack<T>(_models.All);

                while (models.Any())
                {
                    var model = models.Pop() as IModel<T>;

                    if (model.Id == Id) return model;

                    if (model.Children.Any())
                        model.Children
                            .ToList()
                            .ForEach(x =>
                            {
                                models.Push(x);
                            });
                }

                return null;
            });
        }
        public IEnumerable<IModelSettings<T>> GetModelsSettings()
        {
            return _cache.GetOrCreate(_models.Settings, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return _models.Settings;
            });
        }
        public IModelSettings<T> GetModelSettings(string Id)
        {
            return _cache.GetOrCreate(Id, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return _models.Settings
                    .FirstOrDefault(x => x.Id == Id);
            });
        }
        public IModelSettings<T> GetModelSettingsByModelId(string Id)
        {
            return _cache.GetOrCreate(Id, e =>
            {
                e.SlidingExpiration = TimeSpan.FromSeconds(_models.DefaultCacheTimeSpan);

                return _models.Settings
                    .FirstOrDefault(x => x.Model.Id == Id);
            });
        }

        public T AddModel(dynamic Request)
        {
            var jsonRequest = (JsonElement)Request;
            var model = jsonRequest.GetProperty("model")
                .GetRawText()
                .DeserializeSelfReferencing<T>();

            _cache.Set(model.PublicKey, model, DateTimeOffset.Now.AddSeconds(_models.DefaultCacheTimeSpan));

            return model;
        }
        public T AddModel(INlpRequest<T> Request)
        {
            _cache.Set(Request.Model.PublicKey, Request.Model, DateTimeOffset.Now.AddSeconds(_models.DefaultCacheTimeSpan));

            return Request.Model;
        }
        public T AddModel(T Model)
        {
            _cache.Set(Model.PublicKey, Model, DateTimeOffset.Now.AddSeconds(_models.DefaultCacheTimeSpan));

            return Model;
        }

        public void DeleteModel(string Id)
        {
            var model = GetModel(Id);

            try { _cache.Remove(model?.Id); } catch (Exception) { throw new NlpException(HttpStatusCode.BadRequest, $"couldn't delete model with id={Id}"); }
            try { _cache.Remove(model?.PublicKey); } catch (Exception) { throw new NlpException(HttpStatusCode.BadRequest, $"couldn't delete model with public key={model?.PublicKey}"); }
        }
        public void DeleteModel(string Id, Guid PublicKey)
        {
            try { _cache.Remove(Id); } catch (Exception) { throw new NlpException(HttpStatusCode.BadRequest, $"couldn't delete model with id={Id}"); }
            try { _cache.Remove(PublicKey); } catch (Exception) { throw new NlpException(HttpStatusCode.BadRequest, $"couldn't delete model with public key={PublicKey}"); }
        }

        public T Sample()
        {
            return _models.Vanguard;
        }
    }
}
