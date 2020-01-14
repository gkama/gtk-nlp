using System;
using System.Text.Json;
using System.Net;

using Microsoft.AspNetCore.Mvc;

using nlp.data;
using nlp.data.text;
using nlp.services.text;

namespace nlp.core.Controllers
{
    [Authorize]
    [Route("nlp")]
    [ApiController]
    public class TextMiningController : ControllerBase
    {
        private readonly ITextMiningRepository<Model> _repo;

        public TextMiningController(ITextMiningRepository<Model> repo)
        {
            _repo = repo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(repo));
        }

        [HttpPost]
        [Route("mine")]
        public IActionResult Mine([FromBody]dynamic Request)
        {
            return Ok(_repo.Mine(((JsonElement)Request).GetProperty("content").GetString()));
        }

        [HttpPost]
        [Route("stem")]
        public IActionResult Stem([FromBody]TextRequest Request)
        {
            return Ok(_repo.Stem(Request));
        }

        [HttpPost]
        [Route("summarize")]
        public IActionResult Summarize([FromBody]TextRequest Request)
        {
            return Ok(_repo.Summarize(Request));
        }
    }
}