using System;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using nlp.data;
using nlp.services;

namespace nlp.core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NlpController : ControllerBase
    {
        private readonly INlpRepository<Model> _repo;

        public NlpController(INlpRepository<Model> repo)
        {
            _repo = repo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(repo));
        }

        [HttpPost]
        [Route("categorize")]
        public JsonResult Categorize([FromBody]dynamic Request)
        {
            return new JsonResult(_repo.Categorize(Request));
        }

        [HttpGet]
        public string Get()
        {
            return "here";
        }
    }
}
