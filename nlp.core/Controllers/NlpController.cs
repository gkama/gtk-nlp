﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            _repo = repo;
        }

        [HttpGet]
        public string Get()
        {
            return "here";
        }
    }
}
