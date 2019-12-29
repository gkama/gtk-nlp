using System;
using System.Collections.Generic;
using System.Text.Json;

namespace nlp.data
{
    public interface ICategory
    {
        public string Mame { get; set; }
        public int TotalWeight { get; }
        public ICollection<IMatched> Matched { get; set; }
    }

    public interface IMatched
    {
        public string Value { get; set; }
        public int Weight { get; set; }
    }
}
