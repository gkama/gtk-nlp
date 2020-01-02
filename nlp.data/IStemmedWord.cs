using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public interface IStemmedWord
    {
        public string Value { get; set; }
        public string Unstemmed { get; set; }
    }
}
