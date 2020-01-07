using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Moq;

using nlp.data;
using nlp.services;

namespace nlp.tests
{
    public class NlpRepositoryModelTests
    {
        private readonly ILogger<NlpRepository<Model>> _mockLogger;
        private readonly Models<Model> _models;
        private readonly INlpRepository<Model> _repo;

        public NlpRepositoryModelTests()
        {
            _mockLogger = Mock.Of<ILogger<NlpRepository<Model>>>();
            _models = new Models<Model>();
            _repo = new NlpRepository<Model>(_mockLogger, GetServiceProvider().GetService<IMemoryCache>(), _models);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        public void GetModel_EqualsNull(string id)
        {
            var model = _repo.GetModel(id);

            Assert.Null(model);
        }

        [Theory]
        [InlineData("984ce69d-de79-478b-9223-ff6349514e19")]
        [InlineData("0a1dfb9f-9b38-4af2-8cb7-252899ec8304")]
        [InlineData("5d9fd0f0-187a-456d-8798-c682c8f32d5f")]
        public void GetModel_Valid(string id)
        {
            var model = _repo.GetModel(id);

            Assert.NotNull(model);
            Assert.NotNull(model.Name);
            Assert.Equal(model.Id, id);
        }

        [Theory]
        [InlineData("5582adee-5aa5-430c-8ef6-7797d907fa2f")]
        [InlineData("ab9f07d9-a77c-42d1-87f5-cb0c189ee9e7")]
        [InlineData("0a1dfb9f-9b38-4af2-8cb7-252899ec8304")]
        public void GetAnyModel_Valid(string id)
        {
            var model = _repo.GetAnyModel(id);

            Assert.NotNull(model);
            Assert.NotNull(model.Name);
            Assert.Equal(id, model.Id);
        }

        [Fact]
        public void GetModelsSettings_Valid()
        {
            var modelS = _repo.GetModelsSettings();

            Assert.True(modelS.Count() > 0);
        }

        [Theory]
        [InlineData("dd8184ac-2144-47b5-b54f-988605a15682")]
        [InlineData("007781f0-6094-413a-b776-64f6de77949c")]
        [InlineData("b5a6138c-c36c-448a-ba01-5f1fb1dd3694")]
        public void GetModelSettings_Valid(string id)
        {
            var modelSettings = _repo.GetModelSettings(id);

            Assert.NotNull(modelSettings);
            Assert.NotNull(modelSettings.Model);
            Assert.Equal(id, modelSettings.Id);
        }

        [Theory]
        [InlineData("984ce69d-de79-478b-9223-ff6349514e19")]
        [InlineData("0a1dfb9f-9b38-4af2-8cb7-252899ec8304")]
        [InlineData("5d9fd0f0-187a-456d-8798-c682c8f32d5f")]
        public void GetModelSettingsByModelId_Valid(string id)
        {
            var modelSettings = _repo.GetModelSettingsByModelId(id);

            Assert.NotNull(modelSettings);
            Assert.NotNull(modelSettings.Model);
            Assert.Equal(id, modelSettings.Model.Id);
        }

        [Fact]
        public void GetModels_Valid()
        {
            var models = _repo.GetModels();

            Assert.True(models.Count() > 0);
        }

        [Fact]
        public void CategorizeSample_Valid()
        {
            var sample = _repo.CategorizeSample();

            Assert.NotNull(sample);
        }

        private IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
