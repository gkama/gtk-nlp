using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public interface IModel<T>
        where T : class
    {
        string id { get; set; }
        string name { get; set; }
        string details { get; set; }
        ICollection<T> children { get; set; }
    }
}
