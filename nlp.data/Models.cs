using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public class Models<T>
        where T : IModel<T>, new()
    {
        public T Financial =>
            new T()
            {
                Id = "0a1dfb9f-9b38-4af2-8cb7-252899ec8304",
                Name = "Financial",
                Details = "financial|finance",
                Children =
                {
                    Vanguard,
                    Fidelity,
                    new T()
                    {
                        Id = "5582adee-5aa5-430c-8ef6-7797d907fa2f",
                        Name = "Mortgage",
                        Details = "mortgage|house mortgage|apartment mortgage|house pmi|pmi"
                    },
                    new T()
                    {
                        Id = "ab9f07d9-a77c-42d1-87f5-cb0c189ee9e7",
                        Name = "Car Payment",
                        Details = "car payment|car loan|car interest"
                    }
                }
            };

        public T Vanguard =>
            new T()
            {
                Id = "984ce69d-de79-478b-9223-ff6349514e19",
                Name = "Vanguard",
                Details = "vanguard|vanguard group|the vanguard group",
                Children =
                {
                    new T()
                    {
                        Id = "5ec6957d-4de7-4199-9373-d4a7fb59d6e1",
                        Name = "Index Funds",
                        Details = "vbiix|vbinx|vbisx|vbltx|vbmfx|vdaix|vdvix|veiex|veurx|vexmx|vfinx|vfsvx|vftsx|vfwix|vgovx|vgtsx|vhdyx|viaix|vigrx|vihix|vimsx|visgx|visvx|vivax|vlacx|vmgix|vmvix|vpacx|vtebx|vtibx|vtipx|vtsax|vtsmx|vtws"
                    }
                }
            };

        public T Fidelity =>
            new T()
            {
                Id = "5362086a-300a-48aa-86b4-e9f0ed970e35",
                Name = "Fidelity",
                Details = "fidelity|fidelity investments|fidelity investments inc.|fidelity management",
                Children =
                {
                    new T()
                    {
                        Id = "efac956d-e705-48fe-81b4-3254745c41c7",
                        Name = "Index Funds",
                        Details = "fsevx|fusvx|fsivx"
                    },
                    new T()
                    {
                        Id = "34cf611b-6aae-4df6-b0dc-b1820ce6656c",
                        Name = "Exchange Tarded Funds (ETFs)",
                        Details = "fcpi|fsmd|fval|fidi|fdvv|fdlo|fdev|fdem|fldr|fdhy|fiva|fdmo|fqal"
                    }
                }
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
                    },
                    new T()
                    {
                        Id = "d5881aeb-a962-4a4d-ab44-d4ac257099c2",
                        Name = "Пол",
                        Details = "мъжки|женcки"
                    }
                }
            };


        public IModelSettings<T> FinancialSettings =>
            new ModelSettings<T>()
            {
                Id = "dd8184ac-2144-47b5-b54f-988605a15682",
                StopWords = DetaultStopWords,
                Delimiters = new char[]
                {
                    ' ', '|'
                },
                Model = Financial
            };

        public IModelSettings<T> VanguardSettings =>
            new ModelSettings<T>()
            {
                Id = "007781f0-6094-413a-b776-64f6de77949c",
                StopWords = DetaultStopWords,
                Delimiters = DefaultDelimiters,
                Model = Vanguard
            };

        public IModelSettings<T> FidelitySettings =>
            new ModelSettings<T>()
            {
                Id = "ca9e47f9-f3bf-458d-b5ee-afeb61f9dffb",
                StopWords = DetaultStopWords,
                Delimiters = DefaultDelimiters,
                Model = Fidelity
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
                ' ', ',', ';', '?', '!', '.', '`', '-', '(', ')', '[', ']', '|', '$'
            };
        public string[] DetaultStopWords =>
            new string[]
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
            };

        public IEnumerable<T> All =>
            new T[]
            {
                Financial,
                Vanguard,
                Fidelity,
                Bulgarian
            };

        public IEnumerable<IModelSettings<T>> Settings =>
            new IModelSettings<T>[]
            {
                FinancialSettings,
                VanguardSettings,
                FidelitySettings,
                BulgarianSettings
            };
    }
}
