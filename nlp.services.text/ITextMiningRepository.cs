﻿using System;
using System.Collections.Generic;
using System.Text;

using nlp.data.text;

namespace nlp.services.text
{
    public interface ITextMiningRepository<T>
    {
        public object Mine(string Content);
        public IEnumerable<IStemmedWord> Stem(ITextRequest Request);
        public string Summarize(string Content);
        public string Summarize(ITextRequest Request);
        public IEnumerable<string> ToSentences(string Content, IEnumerable<string> StopWords = null);
    }
}
