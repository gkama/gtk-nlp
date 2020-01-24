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

        public NlpResponse(ICollection<ICategory> Categories, int Length)
        {
            this.Categories = Categories;
            this.Length = Length;

            foreach (var c in this.Categories)
            {
                c.TotalWeightPercentage = Math.Round((double)c.TotalWeight / (double)this.Length * 100, 2);

                foreach (var m in c.Matched)
                    m.WeightPercentage = Math.Round((double)m.Weight / (double)this.Length * 100, 2);
            }
        }
    }
}
