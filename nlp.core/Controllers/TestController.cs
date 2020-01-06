using System;
using System.Net;

using Microsoft.AspNetCore.Mvc;

using nlp.data;
using nlp.services;

namespace nlp.core.Controllers
{
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
        public IActionResult Get()
        {
            return Ok(_repo.GetModels());
        }
    }
}