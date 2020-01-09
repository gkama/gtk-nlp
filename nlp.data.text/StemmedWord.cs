using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data.text
{
    public class StemmedWord : IStemmedWord
    {
        public string Value { get; set; }
        public string Unstemmed { get; set; }
    }
}
