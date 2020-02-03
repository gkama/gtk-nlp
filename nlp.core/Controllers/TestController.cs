using System;
using System.Net;

using Microsoft.AspNetCore.Mvc;

using nlp.data;
using nlp.services;

namespace nlp.core.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [ApiController]
    [Route("nlp/test")]
    public class TestController : ControllerBase
    {
        private readonly INlpRepository<TestModel> _repo;

        public TestController(INlpRepository<TestModel> repo)
        {
            _repo = repo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(repo));
        }

        [HttpGet]
        [Route("models")]
        public IActionResult Get()
        {
            return Ok(_repo.GetModels());
        }

        [HttpPost]
        [Route("categorize")]
        public IActionResult Categorize([FromBody]NlpRequest<Model> request)
        {
            return Ok("");
        }

        [HttpGet]
        [Route("exception/400")]
        public IActionResult Throw400NlpException()
        {
            throw new NlpException(HttpStatusCode.BadRequest, $"just threw a bad request");
        }

        [HttpGet]
        [Route("exception/500")]
        public IActionResult Throw500NlpException()
        {
            throw new NlpException(HttpStatusCode.InternalServerError, $"just threw an internal server error");
        }
    }
}