using System;
using System.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nlp.data;
using nlp.services;

namespace nlp.core.Controllers
{
    [Authorize]
    [ApiController]
    [Route("nlp")]
    public class NlpController : ControllerBase
    {
        private readonly INlpRepository<Model> _repo;
        private readonly IModelRepository<Model> _modelrepo;

        public NlpController(INlpRepository<Model> repo, IModelRepository<Model> modelrepo)
        {
            _repo = repo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(repo));
            _modelrepo = modelrepo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(modelrepo));
        }

        [HttpGet]
        [Route("request/schema")]
        public IActionResult GetNlpRequestModel()
        {
            return Ok(_repo.GetNlpRequestSchema());
        }

        [HttpPost]
        [Route("categorize")]
        public IActionResult Categorize([FromBody]NlpRequest<Model> request)
        {
            return Ok(_repo.Categorize(request));
        }

        [HttpPost]
        [Route("categorize/{id}")]
        public IActionResult CategorizeWithModelId([FromRoute]string id, [FromBody]NlpRequest<Model> request)
        {
            return Ok(_repo.Categorize(request, id));
        }

        [HttpGet]
        [Route("models")]
        public IActionResult GetModels()
        {
            return Ok(_modelrepo.GetModels());
        }

        [HttpGet]
        [Route("model/{id}")]
        public IActionResult GetModel([FromRoute]string id)
        {
            return Ok(_modelrepo.GetModel(id));
        }

        [HttpPost]
        [Route("model/add")]
        public IActionResult AddModel([FromBody]NlpRequest<Model> request)
        {
            return Ok(_modelrepo.AddModel(request));
        }

        [HttpGet]
        [Route("models/settings")]
        public IActionResult GetModelsSettings()
        {
            return Ok(_modelrepo.GetModelsSettings());
        }

        [HttpGet]
        [Route("model/settings/{id}")]
        public IActionResult GetModelSettings([FromRoute]string id)
        {
            return Ok(_modelrepo.GetModelSettings(id));
        }
    }
}
