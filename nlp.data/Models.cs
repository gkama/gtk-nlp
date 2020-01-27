using System;
using System.Collections.Generic;
using System.Text;

namespace nlp.data
{
    public class Models<T>
        where T : IModel<T>, new()
    {
        public int DefaultCacheTimeSpan => 86400;
        public int FiveMinutesCacheTimeSpan => 300;
        public int TenMinutesCacheTimeSpan => 600;
        public int TwentyMinutesCacheTimeSpan => 1200;

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
                    },
                    new T()
                    {
                        Id = "ec884a1b-d640-4e0b-835c-4bd6c93c32c7",
                        Name = "U.S. bond ETFs (Exchange-traded funds)",
                        Details = "u.s. bond etf|u.s. bond etfs|u.s. bond exchange-traded fund|u.s. bond exchange-traded funds|u.s. bond exchange traded fund|u.s. bond exchange traded funds",
                        Children =
                        {
                            new T()
                            {
                                Id = "dd3c4213-8e8b-449d-b136-6957e8c53c8e",
                                Name = "Treasury/Agency",
                                Details = "edv|biv|vgit|blv|vglt|wmbs|bsv|vtip|vgsh|bnd"
                            },
                            new T()
                            {
                                Id = "9548e95b-b47f-414e-a3d5-5a1419755de0",
                                Name = "Investment-grade",
                                Details = "vcit|vclt|vcsh|vtc"
                            },
                            new T()
                            {
                                Id = "b0f0c6b8-6fce-472e-9a52-623b7663ed0c",
                                Name = "Tax-exempt",
                                Details = "vteb"
                            }
                        }
                    },
                    new T()
                    {
                        Id = "a3897931-e20c-47a8-905f-53f72c20e9b5",
                        Name = "U.S. stock ETFs (Exchange-traded funds)",
                        Details = "u.s. stock etf|u.s. stock etfs|u.s. stock exchange-traded fund|u.s. stock exchange-traded funds|u.s. stock exchange traded fund|u.s. stock exchange traded funds",
                        Children =
                        {
                            new T()
                            {
                                Id = "0c24bb1a-7a64-4c37-a0d9-08645922e5fe",
                                Name = "Large-cap",
                                Details = "vig|esgv|vug|vym|vv|mgc|mgk|mgv|vone|vong|vonv|vthr|voo|voog|voov|vti|vtv"
                            },
                            new T()
                            {
                                Id = "55adc388-6339-462d-a0cc-cd984f585c1e",
                                Name = "Mid-cap",
                                Details = "vxf|vo|vot|voe|ivoo|ivog|ivov"
                            },
                            new T()
                            {
                                Id = "7a581f82-1c59-4363-a7a7-bcacec32e171",
                                Name = "Small-cap",
                                Details = "vtwo|vtwg|vtwv|vioo|viog|viov|vb|vbk|vbr"
                            }
                        }
                    },
                    new T()
                    {
                        Id = "87c7e5da-b9dc-48e2-86f5-82805d2cd4fe",
                        Name = "International bond ETFs (Exchange-traded funds)",
                        Details = "international bond etf|international bond etfs|international bond exchange-traded fund|international bond exchange-traded funds|international bond exchange traded fund|international bond exchange traded funds",
                        Children =
                        {
                            new T()
                            {
                                Id = "cb047942-7f45-40c2-895b-4e40a9142c6f",
                                Name = "Global",
                                Details = "bndw"
                            },
                            new T()
                            {
                                Id = "4892ea95-5fc0-420d-a52e-7aa6e59f99d0",
                                Name = "International",
                                Details =  "bndx"
                            },
                            new T()
                            {
                                Id = "f0af7518-4305-44ea-8b7d-c95ad5a3add2",
                                Name = "Emerging markets",
                                Details = "vwob"
                            }
                        }
                    },
                    new T()
                    {
                        Id = "17d77d72-efe5-4b80-b811-418b4c76f3c6",
                        Name = "International stock ETFs (Exchange-traded funds)",
                        Details = "international stock etf|international stock etfs|international stock exchange-traded fund|international stock exchange-traded funds|international stock exchange traded fund|international stock exchange traded funds",
                        Children =
                        {
                            new T()
                            {
                                Id = "fb4b74be-c3e0-4457-8c27-9383749a31d9",
                                Name = "Global",
                                Details = "vt"
                            },
                            new T()
                            {
                                Id = "d3ff00f8-48be-4984-a7e8-8d31e2787c20",
                                Name = "International",
                                Details =  "vgsx|veu|vss|vea|vgk|vpl|vnqi|vigi|vymi|vxus"
                            },
                            new T()
                            {
                                Id = "901ea0b9-5b48-45a4-bfa3-7bd7afed255f",
                                Name = "Emerging markets",
                                Details = "vwo"
                            }
                        }
                    },
                    new T()
                    {
                        Id = "2db984dc-779d-43a4-984b-a4d3c367aa63",
                        Name = "Sector & specialty ETFs (Exchange-traded funds)",
                        Details = "sector & specialty etf|sector & specialty etfs|sector & specialty exchange-traded fund|sector & specialty exchange-traded funds|sector & specialty exchange traded fund|sector & specialty exchange traded funds",
                        Children =
                        {
                            new T()
                            {
                                Id = "107ee6a4-b9d6-4691-bc54-9125187132ec",
                                Name = "Sector & specialty ETFs (Exchange-traded funds)",
                                Details = "vox|vcr|vdc|vde|vfh|vht|vis|vgt|vaw|vnq|vpu"
                            }
                        }
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
                        Details = "мъжки|женcки|мъж|женa"
                    }
                }
            };


        public IModelSettings<T> FinancialSettings =>
            new ModelSettings<T>()
            {
                Id = "dd8184ac-2144-47b5-b54f-988605a15682",
                StopWords = DefaultStopWords,
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
                StopWords = DefaultStopWords,
                Delimiters = DefaultDelimiters,
                Model = Vanguard
            };

        public IModelSettings<T> FidelitySettings =>
            new ModelSettings<T>()
            {
                Id = "ca9e47f9-f3bf-458d-b5ee-afeb61f9dffb",
                StopWords = DefaultStopWords,
                Delimiters = DefaultDelimiters,
                Model = Fidelity
            };

        public IModelSettings<T> BulgarianSettings =>
            new ModelSettings<T>()
            {
                Id = "b5a6138c-c36c-448a-ba01-5f1fb1dd3694",
                StopWords = new string[]
                {
                    "да", "не"
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
        public string[] DefaultStopWords =>
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
