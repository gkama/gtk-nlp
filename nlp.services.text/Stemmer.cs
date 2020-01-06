﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nlp.data;

namespace nlp.services.text
{
    /// <summary>
    /// Based off of the improved Porter2 algorithm:
    /// http://snowball.tartarus.org/algorithms/english/stemmer.html
    /// </summary>
    public class Stemmer : IStemmer
    {
        public char[] Alphabet => Enumerable
                .Range('a', 'z' - 'a' + 1)
                .Select(c => (char)c)
                .Concat(new[] { '\'' }).ToArray();
        public char[] Vowels => "aeiouy".ToArray();
        public string[] Doubles => new string[] { "bb", "dd", "ff", "gg", "mm", "nn", "pp", "rr", "tt" };
        public char[] LiEndings => "cdeghkmnrt".ToArray();
        private char[] _nonShortConsonants => "wxY".ToArray();
        private Dictionary<string, string> _exceptions => 
            new Dictionary<string, string>
            {
                {"skis", "ski"},
                {"skies", "sky"},
                {"dying", "die"},
                {"lying", "lie"},
                {"tying", "tie"},
                {"idly", "idl"},
                {"gently", "gentl"},
                {"ugly", "ugli"},
                {"early", "earli"},
                {"only", "onli"},
                {"singly", "singl"},
                {"sky", "sky"},
                {"news", "news"},
                {"howe", "howe"},
                {"atlas", "atlas"},
                {"cosmos", "cosmos"},
                {"bias", "bias"},
                {"andes", "andes"}
            };

        private string[] _exceptionsPart2 => 
            new string[]
            {
                "inning", "outing", "canning", "herring", "earring",
                "proceed", "exceed", "succeed"
            };

        private string[] _exceptionsRegion1 =>
            new string[]
            {
                "gener", "arsen", "commun"
            };

        public IStemmedWord Stem(string Word)
        {
            var original = Word;
            if (Word.Length <= 2)
            {
                return new StemmedWord()
                {
                    Value = Word,
                    Unstemmed = Word
                };
            }

            Word = TrimStartingApostrophe(Word.ToLowerInvariant());

            string excpt;
            if (_exceptions.TryGetValue(Word, out excpt))
            {
                return new StemmedWord()
                {
                    Value = excpt,
                    Unstemmed = original
                };
            }

            Word = MarkYsAsConsonants(Word);

            var r1 = GetRegion1(Word);
            var r2 = GetRegion2(Word);

            Word = Step0RemoveSPluralSuffix(Word);
            Word = Step1ARemoveOtherSPluralSuffixes(Word);

            if (_exceptionsPart2.Contains(Word))
            {
                return new StemmedWord()
                {
                    Value = Word,
                    Unstemmed = original
                };
            }

            Word = Step1BRemoveLySuffixes(Word, r1);
            Word = Step1CReplaceSuffixYWithIIfPreceededWithConsonant(Word);
            Word = Step2ReplaceSuffixes(Word, r1);
            Word = Step3ReplaceSuffixes(Word, r1, r2);
            Word = Step4RemoveSomeSuffixesInR2(Word, r2);
            Word = Step5RemoveEorLSuffixes(Word, r1, r2);

            return new StemmedWord()
            {
                Value = Word.ToLowerInvariant(),
                Unstemmed = original
            };
        }

        private bool IsVowel(char c)
        {
            return Vowels.Contains(c);
        }

        private bool IsConsonant(char c)
        {
            return !Vowels.Contains(c);
        }

        private static bool SuffixInR1(string word, int r1, string suffix)
        {
            return r1 <= word.Length - suffix.Length;
        }

        private bool SuffixInR2(string word, int r2, string suffix)
        {
            return r2 <= word.Length - suffix.Length;
        }

        private static string ReplaceSuffix(string word, string oldSuffix, string newSuffix = null)
        {
            if (oldSuffix != null)
            {
                word = word.Substring(0, word.Length - oldSuffix.Length);
            }

            if (newSuffix != null)
            {
                word += newSuffix;
            }
            return word;
        }

        private static bool TryReplace(string word, string oldSuffix, string newSuffix, out string final)
        {
            if (word.Contains(oldSuffix))
            {
                final = ReplaceSuffix(word, oldSuffix, newSuffix);
                return true;
            }
            final = word;
            return false;
        }

        /// <summary>
        /// The English stemmer treats apostrophe as a letter, removing it from the beginning of a word, where it might have stood for an opening quote, from the end of the word, where it might have stood for a closing quote, or been an apostrophe following s.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static string TrimStartingApostrophe(string word)
        {
            if (word.StartsWith("'"))
            {
                word = word.Substring(1);
            }
            return word;
        }

        /// <summary>
        /// R1 is the region after the first non-vowel following a vowel, or the end of the word if there is no such non-vowel. 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public int GetRegion1(string word)
        {
            // Exceptional forms
            foreach (var except in _exceptionsRegion1.Where(word.StartsWith))
            {
                return except.Length;
            }
            return GetRegion(word, 0);
        }

        /// <summary>
        /// R2 is the region after the first non-vowel following a vowel in R1, or the end of the word if there is no such non-vowel. 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public int GetRegion2(string word)
        {
            var r1 = GetRegion1(word);
            return GetRegion(word, r1);
        }

        private int GetRegion(string word, int begin)
        {
            var foundVowel = false;
            for (var i = begin; i < word.Length; i++)
            {
                if (IsVowel(word[i]))
                {
                    foundVowel = true;
                    continue;
                }
                if (foundVowel && IsConsonant(word[i]))
                {
                    return i + 1;
                }
            }

            return word.Length;
        }

        /// <summary>
        /// Define a short syllable in a word as either (a) a vowel followed 
        /// by a non-vowel other than w, x or Y and preceded by a non-vowel, 
        /// or * (b) a vowel at the beginning of the word followed by a non-vowel. 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool EndsInShortSyllable(string word)
        {
            if (word.Length < 2)
            {
                return false;
            }

            // a vowel at the beginning of the word followed by a non-vowel
            if (word.Length == 2)
            {
                return IsVowel(word[0]) && IsConsonant(word[1]);
            }

            return IsVowel(word[word.Length - 2])
                   && IsConsonant(word[word.Length - 1])
                   && !_nonShortConsonants.Contains(word[word.Length - 1])
                   && IsConsonant(word[word.Length - 3]);
        }

        /// <summary>
        /// A word is called short if it ends in a short syllable, and if R1 is null.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool IsShortWord(string word)
        {
            return EndsInShortSyllable(word) && GetRegion1(word) == word.Length;
        }

        /// <summary>
        /// Set initial y, or y after a vowel, to Y
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public string MarkYsAsConsonants(string word)
        {
            var chars = word.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (i == 0)
                {
                    if (chars[i] == 'y')
                    {
                        chars[i] = 'Y';
                    }
                }
                else if(Vowels.Contains(chars[i - 1]) && chars[i] == 'y')
                {
                    chars[i] = 'Y';
                }
            }
            return new string(chars);
        }

        public string Step0RemoveSPluralSuffix(string word)
        {
            // Ordered from longest to shortest
            var suffixes = new[] {"'s'", "'s", "'"};
            foreach (var suffix in suffixes)
            {
                if (word.EndsWith(suffix))
                {
                    return ReplaceSuffix(word, suffix);
                }
            }
            return word;
        }

        public string Step1ARemoveOtherSPluralSuffixes(string word)
        {
            if (word.EndsWith("sses"))
            {
                return ReplaceSuffix(word, "sses", "ss");
            }
            if (word.EndsWith("ied") || word.EndsWith("ies"))
            {
                var restOfWord = word.Substring(0, word.Length - 3);
                if (word.Length > 4)
                {
                    return restOfWord + "i";
                }
                return restOfWord + "ie";
            }
            if (word.EndsWith("us") || word.EndsWith("ss"))
            {
                return word;
            }
            if (word.EndsWith("s"))
            {
                if (word.Length < 3)
                {
                    return word;
                }

                // Skip both the last letter ('s') and the letter before that
                for (var i = 0; i < word.Length - 2; i++)
                {
                    if (IsVowel(word[i]))
                    {
                        return word.Substring(0, word.Length - 1);
                    }
                }
            }
            return word;
        }

        public string Step1BRemoveLySuffixes(string word, int r1)
        {
            foreach (var suffix in new [] {"eedly", "eed"}.Where(word.EndsWith))
            {
                if(SuffixInR1(word, r1, suffix))
                {
                    return ReplaceSuffix(word, suffix, "ee");
                }
                return word;
            }

            foreach (var suffix in new [] {"ed", "edly", "ing", "ingly"}.Where(word.EndsWith))
            {
                var trunc = ReplaceSuffix(word, suffix);//word.Substring(0, word.Length - suffix.Length);
                if (trunc.Any(IsVowel))
                {
                    if (new[] {"at", "bl", "iz"}.Any(trunc.EndsWith))
                    {
                        return trunc + "e";
                    }
                    if (Doubles.Any(trunc.EndsWith))
                    {
                        return trunc.Substring(0, trunc.Length - 1);
                    }
                    if (IsShortWord(trunc))
                    {
                        return trunc + "e";
                    }
                    return trunc;
                }
                return word;
            }

            return word;
        }

        public string Step1CReplaceSuffixYWithIIfPreceededWithConsonant(string word)
        {
            if ((word.EndsWith("y") || word.EndsWith("Y"))
                && word.Length > 2
                && IsConsonant(word[word.Length - 2]))
            {
                return word.Substring(0, word.Length - 1) + "i";
            }
            return word;
        }

        public string Step2ReplaceSuffixes(string word, int r1)
        {
            var suffixes = new Dictionary<string, string>
                {
                    {"ization", "ize"},
                    {"ational", "ate"},
                    {"ousness", "ous"},
                    {"iveness", "ive"},
                    {"fulness", "ful"},
                    {"tional", "tion"},
                    {"lessli", "less"},
                    {"biliti", "ble"},
                    {"entli", "ent"},
                    {"ation", "ate"},
                    {"alism", "al"},
                    {"aliti", "al"},
                    {"fulli", "ful"},
                    {"ousli", "ous"},
                    {"iviti", "ive"},
                    {"enci", "ence"},
                    {"anci", "ance"},
                    {"abli", "able"},
                    {"izer", "ize"},
                    {"ator", "ate"},
                    {"alli", "al"},
                    {"bli", "ble"}
                };
            foreach (var suffix in suffixes)
            {
                if (word.EndsWith(suffix.Key))
                {
                    string final;
                    if (SuffixInR1(word, r1, suffix.Key)
                        && TryReplace(word, suffix.Key, suffix.Value, out final))
                    {
                        return final;
                    }
                    return word;
                }
            }

            if (word.EndsWith("ogi") 
                && SuffixInR1(word, r1, "ogi") 
                && word[word.Length - 4] == 'l')
            {
                return ReplaceSuffix(word, "ogi", "og");
            }

            if (word.EndsWith("li") & SuffixInR1(word, r1, "li"))
            {
                if (LiEndings.Contains(word[word.Length - 3]))
                {
                    return ReplaceSuffix(word, "li");
                }
            }

            return word;
        }

        public string Step3ReplaceSuffixes(string word, int r1, int r2)
        {
            var suffixes = new Dictionary<string, string>
                {
                    {"ational", "ate"},
                    {"tional", "tion"},
                    {"alize", "al"},
                    {"icate", "ic"},
                    {"iciti", "ic"},
                    {"ical", "ic"},
                    {"ful", null},
                    {"ness", null}
                };
            foreach (var suffix in suffixes.Where(s => word.EndsWith(s.Key)))
            {
                string final;
                if (SuffixInR1(word, r1, suffix.Key)
                    && TryReplace(word, suffix.Key, suffix.Value, out final))
                {
                    return final;
                }
            }

            if (word.EndsWith("ative"))
            {
                if(SuffixInR1(word, r1, "ative") && SuffixInR2(word, r2, "ative"))
                {
                    return ReplaceSuffix(word, "ative", null);
                }
            }

            return word;
        }

        public string Step4RemoveSomeSuffixesInR2(string word, int r2)
        {
            foreach (var suffix in new[]
                {
                    "al", "ance", "ence", "er", "ic", "able", "ible", "ant", 
                    "ement", "ment", "ent", "ism", "ate", "iti", "ous", 
                    "ive", "ize"
                })
            {
                if (word.EndsWith(suffix))
                {
                    if (SuffixInR2(word, r2, suffix))
                    {
                        return ReplaceSuffix(word, suffix);
                    }
                    return word;
                }
            }

            if (word.EndsWith("ion") && 
                SuffixInR2(word, r2, "ion") &&
                new[] {'s', 't'}.Contains(word[word.Length - 4]))
            {
                return ReplaceSuffix(word, "ion");
            }
            return word;
        }

        public string Step5RemoveEorLSuffixes(string word, int r1, int r2)
        {
            if (word.EndsWith("e") &&
                (SuffixInR2(word, r2, "e") ||
                    (SuffixInR1(word, r1, "e") && 
                        !EndsInShortSyllable(ReplaceSuffix(word, "e")))))
            {
                return ReplaceSuffix(word, "e");
            }

            if (word.EndsWith("l") && 
                SuffixInR2(word, r2, "l") && 
                word.Length > 1 &&
                word[word.Length - 2] == 'l')
            {
                return ReplaceSuffix(word, "l");
            }

            return word;
        }
    }
}
