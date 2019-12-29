using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nlp.data
{
    public class Category : ICategory
    {
        public string Name { get; set; }
        public int TotalWeight => Matched.Sum(x => x.Weight);
        public ICollection<IMatched> Matched { get; set; } = new List<IMatched>();
    }

    public class Matched : IMatched
    {
        public string Value { get; set; }
        public int Weight { get; set; } = 0;
    }
}
