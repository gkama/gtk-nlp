using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public class NlpResponse : INlpResponse
    {
        public bool Summarized { get; set; } = false;
        public int? SummarizedLength { get; set; }
        public int Length { get; set; }
        public ICollection<ICategory> Categories { get; set; } = new List<ICategory>();
    }
}
