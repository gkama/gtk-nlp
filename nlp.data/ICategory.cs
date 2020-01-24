using System;
using System.Collections.Generic;
using System.Text.Json;

namespace nlp.data
{
    public interface ICategory
    {
        public string Name { get; set; }
        public int TotalWeight { get; }
        public double TotalWeightPercentage { get; set; }
        public ICollection<IMatched> Matched { get; set; }
    }

    public interface IMatched
    {
        public string Value { get; set; }
        public int Weight { get; set; }
        public double WeightPercentage { get; set; }
    }
}
