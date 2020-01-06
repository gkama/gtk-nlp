using System;
using System.Net;

using Microsoft.AspNetCore.Mvc;

using nlp.data;
using nlp.services;

namespace nlp.core.Controllers
{
    [ApiController]
    [Route("nlp")]
    public class NlpController : ControllerBase
    {
        private readonly INlpRepository<Model> _repo;

        public NlpController(INlpRepository<Model> repo)
        {
            _repo = repo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(repo));
        }

        [HttpPost]
        [Route("categorize")]
        public IActionResult Categorize([FromBody]dynamic request)
        {
            return Ok(_repo.Categorize(request));
        }

        [HttpPost]
        [Route("categorize/{id}")]
        public IActionResult CategorizeWithModelId([FromRoute]string id, [FromBody]dynamic request)
        {
            return Ok(_repo.Categorize(request, id));
        }

        [HttpGet]
        [Route("categorize/sample")]
        public IActionResult CategorizeSample()
        {
            return Ok(_repo.CategorizeSample());
        }

        [HttpGet]
        [Route("models")]
        public IActionResult GetModels()
        {
            return Ok(_repo.GetModels());
        }

        [HttpGet]
        [Route("model/{id}")]
        public IActionResult GetModel([FromRoute]string id)
        {
            return Ok(_repo.GetModel(id));
        }

        [HttpGet]
        [Route("models/settings")]
        public IActionResult GetModelsSettings()
        {
            return Ok(_repo.GetModelsSettings());
        }

        [HttpGet]
        [Route("model/settings/{id}")]
        public IActionResult GetModelSettings([FromRoute]string id)
        {
            return Ok(_repo.GetModelSettings(id));
        }
    }
}
