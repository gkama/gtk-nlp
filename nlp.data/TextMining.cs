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
        public static string Stem(this string Word)
        {
            /*
             Suffix-stripping algorithms
             Suffix stripping algorithms do not rely on a lookup table that consists of inflected forms and root form relations. Instead, a typically smaller list of "rules" is stored which provides a path for the algorithm, given an input word form, to find its root form. Some examples of the rules include:
                if the word ends in 'ed', remove the 'ed'
                if the word ends in 'ing', remove the 'ing'
                if the word ends in 'ly', remove the 'ly'
            */
            if (string.IsNullOrWhiteSpace(Word) || Word.Length <= 2)
                return Word;

                //Step 1: get rid of plurals and -ed or -ing
                if (Word.EndsWith("'")) { return Word.RemoveLast(1); }
                if (Word.EndsWith("ies")
                    || Word.EndsWith("sses")
                    || Word.EndsWith("ed")
                    || Word.EndsWith("ly")
                    || Word.EndsWith("'s")) return Word.RemoveLast(2);
                if (Word.EndsWith("ing")
                    || Word.EndsWith("'s'")) Word.RemoveLast(3);

                //Step3: maps double suffices to single ones. so -ization ( = -ize plus
                //-ation) maps to -ize etc. note that the string before the suffix must give m() > 0.
                if (Word.EndsWith("ational")) return Word.ReplaceEndIndex(7, "ate");
                if (Word.EndsWith("tional")) Word.ReplaceEndIndex(6, "tion");
                if (Word.EndsWith("enci")) Word.ReplaceEndIndex(4, "ence");
                if (Word.EndsWith("anci")) Word.ReplaceEndIndex(4, "ance");
                if (Word.EndsWith("izer")) return Word.ReplaceEndIndex(4, "ize");
                if (Word.EndsWith("bli")) return Word.ReplaceEndIndex(3, "ble");
                if (Word.EndsWith("alli")) return Word.ReplaceEndIndex(4, "al");
                if (Word.EndsWith("entli")) return Word.ReplaceEndIndex(5, "ent");
                if (Word.EndsWith("eli")) return Word.ReplaceEndIndex(3, "e");
                if (Word.EndsWith("ousli")) return Word.ReplaceEndIndex(5, "ous");
                if (Word.EndsWith("ization")) return Word.ReplaceEndIndex(7, "ize");
                if (Word.EndsWith("ation")) return Word.ReplaceEndIndex(5, "ate");
                if (Word.EndsWith("ator")) return Word.ReplaceEndIndex(4, "ate");
                if (Word.EndsWith("alism")) return Word.ReplaceEndIndex(5, "al");
                if (Word.EndsWith("iveness")) return Word.ReplaceEndIndex(7, "ive");
                if (Word.EndsWith("fulness")) return Word.ReplaceEndIndex(7, "ful");
                if (Word.EndsWith("ousness")) return Word.ReplaceEndIndex(7, "ous");
                if (Word.EndsWith("aliti")) return Word.ReplaceEndIndex(5, "al");
                if (Word.EndsWith("iviti")) return Word.ReplaceEndIndex(5, "ive");
                if (Word.EndsWith("biliti")) return Word.ReplaceEndIndex(6, "ble");
                if (Word.EndsWith("logi")) return Word.ReplaceEndIndex(4, "log");

                //Step 4: deals with -ic-, -full, -ness etc. similar strategy to step3
                if (Word.EndsWith("icate")) return Word.ReplaceEndIndex(5, "ic");
                if (Word.EndsWith("ative")) return Word.ReplaceEndIndex(5, "");
                if (Word.EndsWith("alize")) return Word.ReplaceEndIndex(5, "al");
                if (Word.EndsWith("iciti")) return Word.ReplaceEndIndex(5, "ic");
                if (Word.EndsWith("ical")) return Word.ReplaceEndIndex(4, "ic");
                if (Word.EndsWith("ful")) return Word.ReplaceEndIndex(3, "");
                if (Word.EndsWith("ness")) return Word.ReplaceEndIndex(4, "");

                //Step 5:
                //if (Word.EndsWith("ence")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "ence", ""); continue; }
                //if (Word.EndsWith("er")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "er", ""); continue; }
                //if (Word.EndsWith("ic")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "ic", ""); continue; }
                //if (Word.EndsWith("able")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "able", ""); continue; }
                //if (Word.EndsWith("ant")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "ant", ""); continue; }
                //if (Word.EndsWith("ement")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "ement", ""); continue; }
                //if (Word.EndsWith("ent")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "ent", ""); continue; }
                //if (Word.EndsWith("ou")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "ou", ""); continue; }
                //if (Word.EndsWith("ism")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "ism", ""); continue; }
                //if (Word.EndsWith("ate")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "ate", ""); continue; }
                //if (Word.EndsWith("iti")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "iti", ""); continue; }
                //if (Word.EndsWith("ous")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "ous", ""); continue; }
                //if (Word.EndsWith("ive")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "ive", ""); continue; }
                //if (Word.EndsWith("ize")) { TextToReturn.ReplaceAndAddIfEndsWith(word, "ize", ""); continue; }

            return Word;
        }

        public static string RemoveLast(this string Str, int Characters)
        {
            return Str.Remove(Str.Length - Characters);
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
