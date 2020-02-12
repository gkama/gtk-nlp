using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using nlp.data;
using nlp.services;

namespace nlp.core.Controllers
{
    [ApiController]
    [Route("sample")]
    public class SampleController : ControllerBase
    {
        private readonly INlpRepository<Model> _repo;

        public SampleController(INlpRepository<Model> repo)
        {
            _repo = repo;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("nlp/categorize")]
        public IActionResult CategorizeSample()
        {
            return Ok(_repo.CategorizeSample());
        }
    }
}