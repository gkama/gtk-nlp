using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using nlp.data;
using nlp.services;
using nlp.services.text;

namespace nlp.core.Controllers
{
    [ApiController]
    [Route("sample")]
    public class SampleController : ControllerBase
    {
        private readonly INlpRepository<Model> _repo;
        private readonly ISummarizer _summarizer;

        public SampleController(INlpRepository<Model> repo, ISummarizer summarizer)
        {
            _repo = repo;
            _summarizer = summarizer;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("nlp/categorize")]
        public IActionResult CategorizeSample()
        {
            return Ok(_repo.CategorizeSample());
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("summarizer")]
        public IActionResult SummarizerSample()
        {
            return Ok(_summarizer.Sample());
        }
    }
}