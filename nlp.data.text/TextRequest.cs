using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data.text
{
    public class TextRequest : ITextRequest
    {
        public string Content { get; set; }
        public int NumberOfSentences { get; set; }
        public IEnumerable<string> StopWords { get; set; }
    }
}
