using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public class NlpRequest<T> : INlpRequest<T>
    {
        public string Content { get; set; }
        public T Model { get; set; }
        public string[] Delimiters { get; set; }
        public string[] StopWords { get; set; }
        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string ModelDetails { get; set; }
        public bool Summarize { get; set; } = false;
    }
}
