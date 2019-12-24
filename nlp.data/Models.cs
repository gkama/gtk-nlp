using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public static class Models
    {
        public static IModel<Model> Vanguard =>
            new Model()
            {
                Id = "984ce69d-de79-478b-9223-ff6349514e19",
                Name = "Vanguard",
                Children = {
                    new Model()
                    {
                        Id = "5ec6957d-4de7-4199-9373-d4a7fb59d6e1",
                        Name = "Index Funds",
                        Details = "vbiix|vbinx|vbisx|vbltx|vbmfx|vdaix|vdvix|veiex|veurx|vexmx|vfinx|vfsvx|vftsx|vfwix|vgovx|vgtsx|vhdyx|viaix|vigrx|vihix|vimsx|visgx|visvx|vivax|vlacx|vmgix|vmvix|vpacx|vtebx|vtibx|vtipx|vtsax|vtsmx|vtws"
                    }
                }
            };

        public static IModelSettings<Model> VanguardSettings =>
            new ModelSettings<Model>()
            {
                Id = "007781f0-6094-413a-b776-64f6de77949c",
                ModelId = "984ce69d-de79-478b-9223-ff6349514e19",
                StopWords = new string[]
                {
                    "ourselves", "hers", "between", "yourself", "but", "again", "there", "about", "once", "during",
                    "out", "very", "having", "with", "they", "own", "an", "be", "some", "for", "do", "its", "yours", "such",
                    "into", "of", "most", "itself", "other", "off", "is", "s", "am", "or", "who", "as", "from", "him", "each",
                    "the", "themselves", "until", "below", "are", "we", "these", "your", "his", "through", "don", "nor", "me",
                    "were", "her", "more", "himself", "this", "down", "should", "our", "their", "while", "above", "both", "up",
                    "to", "ours", "had", "she", "all", "no", "when", "at", "any", "before", "them", "same", "and", "been", "have",
                    "in", "will", "on", "does", "yourselves", "then", "that", "because", "what", "over", "why", "so", "can", "did",
                    "not", "now", "under", "he", "you", "herself", "has", "just", "where", "too", "only", "myself", "which", "those",
                    "i", "after", "few", "whom", "t", "being", "if", "theirs", "my", "against", "a", "by", "doing", "it", "how",
                    "further", "was", "here", "than"
                },
                Delimiters = new string[]
                {
                    " ", ",", ";", "!", "."
                }
            };
    }
}
