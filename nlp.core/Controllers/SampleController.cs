using System;
using System.Collections.Generic;
using System.Net;
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
        private readonly IStemmer _stemmer;

        public SampleController(INlpRepository<Model> repo, ISummarizer summarizer, IStemmer stemmer)
        {
            _repo = repo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(repo));
            _summarizer = summarizer ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(summarizer));
            _stemmer = stemmer ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(stemmer));
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
        [Route("summary")]
        public IActionResult SummarizerSample()
        {
            return Ok(_summarizer.Sample());
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("stem")]
        public IActionResult StemSample()
        {
            return Ok(_stemmer.Sample());
        }
    }
}