using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public class Models<T>
        where T : IModel<T>, new()
    {
        public T Vanguard =>
            new T()
            {
                Id = "984ce69d-de79-478b-9223-ff6349514e19",
                Name = "Vanguard",
                Details = "vanguard|vanguard group|the vanguard group",
                Children = {
                    new T()
                    {
                        Id = "5ec6957d-4de7-4199-9373-d4a7fb59d6e1",
                        Name = "Index Funds",
                        Details = "vbiix|vbinx|vbisx|vbltx|vbmfx|vdaix|vdvix|veiex|veurx|vexmx|vfinx|vfsvx|vftsx|vfwix|vgovx|vgtsx|vhdyx|viaix|vigrx|vihix|vimsx|visgx|visvx|vivax|vlacx|vmgix|vmvix|vpacx|vtebx|vtibx|vtipx|vtsax|vtsmx|vtws"
                    }
                }
            };
        public IModelSettings<T> VanguardSettings =>
            new ModelSettings<T>()
            {
                Id = "007781f0-6094-413a-b776-64f6de77949c",
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
                Delimiters = new char[]
                {
                    ' ', ',', ';', '!', '.'
                },
                Model = Vanguard
            };

        public T Bulgarian =>
            new T()
            {
                Id = "5d9fd0f0-187a-456d-8798-c682c8f32d5f",
                Name = "Български Модел",
                Details = "българия|български",
                Children =
                {
                    new T()
                    {
                        Id = "85da6c24-363d-47b2-a120-9934849372fe",
                        Name = "Oбщ",
                        Details = "аз|съм|година"
                    }
                }
            };
        public IModelSettings<T> BulgarianSettings =>
            new ModelSettings<T>()
            {
                Id = "b5a6138c-c36c-448a-ba01-5f1fb1dd3694",
                StopWords = new string[]
                {

                },
                Delimiters = new char[]
                {
                    ' ', '|'
                },
                Model = Bulgarian
            };

        public char[] DefaultDelimiters =>
            new char[]
            {
                ' ', ',', ';', '?', '!', '.', '`', '-', '(', ')', '[', ']', '|'
            };
        public string[] DetaulfStopWords =>
            new string[]
            {
                "the", "a", "and", "i", "am"
            };

        public IEnumerable<IModelSettings<T>> All => new IModelSettings<T>[]
        {
            VanguardSettings,
            BulgarianSettings
        };
    }
}
