﻿using System;
using System.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nlp.data;
using nlp.services;

namespace nlp.core.Controllers
{
    [Authorize]
    [ApiController]
    [Route("nlp")]
    public class NlpController : ControllerBase
    {
        private readonly INlpRepository<Model> _repo;

        public NlpController(INlpRepository<Model> repo)
        {
            _repo = repo ?? throw new NlpException(HttpStatusCode.InternalServerError, nameof(repo));
        }

        [HttpGet]
        [Route("request/schema")]
        public IActionResult GetNlpRequestModel()
        {
            return Ok(_repo.GetNlpRequestSchema());
        }

        [HttpPost]
        [Route("categorize")]
        public IActionResult Categorize([FromBody]NlpRequest<Model> request)
        {
            return Ok(_repo.Categorize(request));
        }

        [HttpPost]
        [Route("categorize/{id}")]
        public IActionResult CategorizeWithModelId([FromRoute]string id, [FromBody]NlpRequest<Model> request)
        {
            return Ok(_repo.Categorize(request, id));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("categorize/sample")]
        public IActionResult CategorizeSample()
        {
            return Ok(_repo.CategorizeSample());
        }

        [HttpGet]
        [Route("models")]
        public IActionResult GetModels()
        {
            return Ok(_repo.GetModels());
        }

        [HttpGet]
        [Route("model/{id}")]
        public IActionResult GetModel([FromRoute]string id)
        {
            return Ok(_repo.GetModel(id));
        }

        [HttpPost]
        [Route("model/add")]
        public IActionResult AddModel([FromBody]dynamic request)
        {
            return Ok(_repo.AddModel(request));
        }

        [HttpGet]
        [Route("models/settings")]
        public IActionResult GetModelsSettings()
        {
            return Ok(_repo.GetModelsSettings());
        }

        [HttpGet]
        [Route("model/settings/{id}")]
        public IActionResult GetModelSettings([FromRoute]string id)
        {
            return Ok(_repo.GetModelSettings(id));
        }
    }
}
