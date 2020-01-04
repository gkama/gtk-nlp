using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public class BulgarianModel : IModel<BulgarianModel>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Language => "Български език";
        public string LanguageUs => "Bulgarian";
        public string Details { get; set; }
        public Guid PublicKey => Guid.NewGuid();
        public ICollection<BulgarianModel> Children { get; set; } = new List<BulgarianModel>();
    }
}
