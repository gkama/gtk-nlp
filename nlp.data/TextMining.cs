using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nlp.data
{
    public static class TextMining
    {

        /// <summary>
        /// Stemming. An important pre-processing step before indexing input documents for text mining is the stemming of words.
        /// The term stemming refers to the reduction of words to their roots so that, for example, 
        /// different grammatical forms or declinations of verbs are identified and indexed (counted) as the same word. 
        /// For example, stemming will ensure that both "travel" and "traveled" will be recognized by the program as the same word. 
        /// For more information, see Manning and Schütze (2002).
        /// https://en.wikipedia.org/wiki/Stemming
        /// </summary>
        public static List<string> Stem(this List<string> Text)
        {
            /*
             Suffix-stripping algorithms
             Suffix stripping algorithms do not rely on a lookup table that consists of inflected forms and root form relations. Instead, a typically smaller list of "rules" is stored which provides a path for the algorithm, given an input word form, to find its root form. Some examples of the rules include:
                if the word ends in 'ed', remove the 'ed'
                if the word ends in 'ing', remove the 'ing'
                if the word ends in 'ly', remove the 'ly'
            */
            var TextToReturn = new List<string>();

            foreach (var word in Text)
            {
                if (string.IsNullOrWhiteSpace(word) || word.Length <= 2)
                { TextToReturn.Add(word); continue; }

                //Step 1: get rid of plurals and -ed or -ing
                if (word.EndsWith("'"))
                { TextToReturn.Add(word.RemoveLast(1)); continue; }
                else if (word.EndsWith("ies")
                    || word.EndsWith("sses")
                    || word.EndsWith("ed")
                    || word.EndsWith("ly")
                    || word.EndsWith("'s"))
                { TextToReturn.Add(word.RemoveLast(2)); continue; }
                else if (word.EndsWith("ing")
                    || word.EndsWith("'s'"))
                { TextToReturn.Add(word.RemoveLast(3)); continue; }

                //Step 4: deals with -ic-, -full, -ness etc. similar strategy to step3
                switch (word.LastLetter())
                {
                    case 'e':
                        if (word.EndsWith("icate")) { TextToReturn.Add(word.ReplaceEndIndex(5, "ic")); break; }
                        if (word.EndsWith("ative")) { TextToReturn.Add(word.ReplaceEndIndex(5, "")); break; }
                        if (word.EndsWith("alize")) { TextToReturn.Add(word.ReplaceEndIndex(5, "al")); }
                        break;
                    case 'i':
                        if (word.EndsWith("iciti")) { TextToReturn.Add(word.ReplaceEndIndex(5, "ic")); }
                        break;
                    case 'l':
                        if (word.EndsWith("ical")) { TextToReturn.Add(word.ReplaceEndIndex(4, "ic")); break; }
                        if (word.EndsWith("ful")) { TextToReturn.Add(word.ReplaceEndIndex(3, "")); }
                        break;
                    case 's':
                        if (word.EndsWith("ness")) { TextToReturn.Add(word.ReplaceEndIndex(4, "")); }
                        break;
                }

                //If all passes, then simply add the word
                TextToReturn.Add(word);
            };

            return TextToReturn;
        }

        public static string RemoveLast(this string Str, int Characters)
        {
            return Str.Remove(Str.Length - Characters);
        }
        public static char LastLetter(this string Str)
        {
            return Str[Str.Length - 1];
        }

        public static string ReplaceEndIndex(this string Str, int EndIndex, string Replacement)
        {
            return $"{Str.Substring(0, Str.Length - EndIndex)}{Replacement}";
        }

        public static bool IsConsonant(char Letter)
        {
            return !Vowels.Contains(Letter.ToString());
        }

        public static string[] Vowels =>
            new string[]
            {
                "a",
                "e",
                "i",
                "o",
                "u",
                "y"
            };
        public static string[] DoubleConsonants =>
            new string[]
            {
                "bb",
                "dd",
                "ff",
                "gg",
                "mm",
                "nn",
                "pp",
                "rr",
                "tt"
            };
        public static string[] LiEnding =>
            new string[]
            {
                "c",
                "d",
                "e",
                "g",
                "h",
                "k",
                "m",
                "n",
                "r",
                "t"
            };
    }
}
