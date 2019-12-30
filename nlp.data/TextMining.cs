﻿using System;
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

            Text.ToList().ForEach(x =>
            {
                if (x.EndsWith("'"))
                    TextToReturn.Add(x.RemoveLast(1));
                else if (x.EndsWith("ed")
                    || x.EndsWith("ly")
                    || x.EndsWith("'s"))
                    TextToReturn.Add(x.RemoveLast(2));
                else if (x.EndsWith("ing")
                    || x.EndsWith("'s'"))
                    TextToReturn.Add(x.RemoveLast(3));
                else
                    TextToReturn.Add(x);
            });

            return TextToReturn;
        }

        public static string RemoveLast(this string Str, int Characters)
        {
            return Str.Remove(Str.Length - Characters);
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
