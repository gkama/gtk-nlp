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
        private readonly IModelRepository<BulgarianModel> _modelrepo;

        public BulgarianController(INlpRepository<BulgarianModel> repo, IModelRepository<BulgarianModel> modelrepo)
        {
            _repo = repo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(repo));
            _modelrepo = modelrepo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(modelrepo));
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
            return Ok(_modelrepo.GetModel(id));
        }

        [HttpPost]
        [Route("model/add")]
        public IActionResult AddModel([FromBody]NlpRequest<BulgarianModel> request)
        {
            return Ok(_modelrepo.AddModel(request));
        }
    }
}