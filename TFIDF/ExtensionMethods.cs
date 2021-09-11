using LemmaSharp.Classes;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TFIDF
{
    /// <summary>
    /// A class for defining extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Tokenize
        /// <summary>
        /// Tokeninzes a given text string into words.
        /// </summary>
        /// <param name="text">The text being tokanized.</param>
        /// <returns>An array of tokenized words.</returns>
        public static string[] tokenize(this string text)
        {
            var pipeline = new MLPipeline();
            var engine = pipeline.mlengine;
            var data = new TextData() { Text = text.ToLower() };
            var prediction = engine.Predict(data);
            return prediction.Words;
        }
        #endregion

        #region Lemmatize
        /// <summary>
        /// Lemmatizes a given array of words.
        /// </summary>
        /// <param name="words">An array of words which needs to be lemmatized.</param>
        /// <returns></returns>
        public static List<string> lemmatize(this string[] words)
        {
            //var dataFilepath = @"D:\Dev\C sharp\TFIDF\full7z-mlteast-en-modified.lem";
            //var stream = File.OpenRead(dataFilepath);
            var byteArray = Properties.Resources.full7z_mlteast_en_modified;
            var stream = new MemoryStream(byteArray);
            
            var lemmatizer = new Lemmatizer(stream);
            var lemmatizedWords = new List<string>();
            foreach(string word in words)
            {
                lemmatizedWords.Add(lemmatizer.Lemmatize(word));
            }
            return lemmatizedWords;
        }
        #endregion

        #region Clean Links
        /// <summary>
        /// Takes a list of strings as argument and generates a set out of the strings by skipping empty strings.
        /// </summary>
        /// <param name="links">A list of strings.</param>
        /// <returns>A set of strings which has no empty strings.</returns>
        public static List<string> clean_links(this List<string> links)
        {
            List<string> newLinks = new List<string>();
            var cleanedLinks = links.Distinct();
            foreach (var link in cleanedLinks)
            {
                if (link != string.Empty) newLinks.Add(link);
            }
            return newLinks;
        }
        #endregion

        #region Split string
        /// <summary>
        /// Splits a given string (<paramref name="inputString"/>) 
        /// into substrings using a set of characters (<paramref name="delimiters"/>).
        /// </summary>
        /// <param name="inputString">The string to be split.</param>
        /// <param name="delimiters">The delimiting characters for splitting the string.</param>
        /// <returns></returns>
        public static IEnumerable<string> SplitAndKeep(
                              this string inputString, params char[] delimiters)
        {
            int start = 0, index;

            while ((index = inputString.IndexOfAny(delimiters, start)) != -1)
            {
                if (index - start > 0)
                    yield return inputString.Substring(start, index - start);

                yield return inputString.Substring(index, 1);

                start = index + 1;
            }

            if (start < inputString.Length)
            {
                yield return inputString.Substring(start);
            }
        }
        #endregion
    }
}
