﻿using System;
using System.Collections.Generic;
using System.Text.Json;

using nlp.data;

namespace nlp.services
{
    public interface INlpRepository<T>
    {
        public INlpResponse Categorize(INlpRequest<T> Request, string Id = null);
        public IModelSettings<T> Parse(INlpRequest<T> Request, string Id = null);
        public IEnumerable<string> Tokenize(string Content, char[] Delimiters, string[] StopWords);
        public object GetNlpRequestSchema();
        public object Sample();
    }
}
