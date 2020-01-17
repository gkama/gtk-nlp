using System;
using System.Net;

using Microsoft.AspNetCore.Mvc;

using nlp.data;
using nlp.services;

namespace nlp.core.Controllers
{
    [Authorize]
    [ApiController]
    [Route("nlp/bulgarian")]
    public class BulgarianController : ControllerBase
    {
        private readonly INlpRepository<BulgarianModel> _repo;

        public BulgarianController(INlpRepository<BulgarianModel> repo)
        {
            _repo = repo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(repo));
        }

        [HttpPost]
        [Route("categorize/{id}")]
        public IActionResult CategorizeWithModelId([FromRoute]string id, [FromBody]NlpRequest<BulgarianModel> request)
        {
            return Ok(_repo.Categorize(request, id));
        }

        [HttpGet]
        [Route("model/{id}")]
        public IActionResult GetModel([FromRoute]string id)
        {
            return Ok(_repo.GetModel(id));
        }
    }
}