using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using nlp.data;
using nlp.services;

namespace nlp.core.Controllers
{
    [Route("text/mine")]
    [ApiController]
    public class TextMiningController : ControllerBase
    {
        private readonly ITextMiningRepository<Model> _repo;

        public TextMiningController(ITextMiningRepository<Model> repo)
        {
            _repo = repo;
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
    }
}