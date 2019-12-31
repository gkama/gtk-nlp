using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using nlp.data;
using nlp.services;

namespace nlp.core.Controllers
{
    [ApiController]
    [Route("nlp/bulgarian")]
    public class BulgarianController : ControllerBase
    {
        private readonly INlpRepository<BulgarianModel> _repo;

        public BulgarianController(INlpRepository<BulgarianModel> repo)
        {
            _repo = repo;
        }

        [HttpPost]
        [Route("categorize/{id}")]
        public IActionResult CategorizeWithModelId([FromRoute]string id, [FromBody]dynamic request)
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