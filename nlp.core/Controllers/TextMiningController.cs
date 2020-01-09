using System;
using System.Text.Json;
using System.Net;

using Microsoft.AspNetCore.Mvc;

using nlp.data;
using nlp.services.text;

namespace nlp.core.Controllers
{
    [Route("nlp/mine")]
    [ApiController]
    public class TextMiningController : ControllerBase
    {
        private readonly ITextMiningRepository<Model> _repo;

        public TextMiningController(ITextMiningRepository<Model> repo)
        {
            _repo = repo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(repo));
        }

        [HttpPost]
        public IActionResult Mine([FromBody]dynamic Request)
        {
            return Ok(_repo.Mine(((JsonElement)Request).GetProperty("content").GetString()));
        }

        [HttpPost]
        [Route("stem")]
        public IActionResult Stem([FromBody]dynamic Request)
        {
            return Ok(_repo.Stem(((JsonElement)Request).GetProperty("content").GetString()));
        }

        [HttpPost]
        [Route("summarize")]
        public IActionResult Summarize([FromBody]dynamic Request)
        {
            return Ok(_repo.Summarize(((JsonElement)Request).GetProperty("content").GetString()));
        }
    }
}