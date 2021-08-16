using LemmaSharp.Classes;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TFIDF
{
    public static class ExtensionMethods
    {
        public static string[] tokenize(this string text)
        {
            var pipeline = new MLPipeline();
            var engine = pipeline.mlengine;
            var data = new TextData() { Text = text.ToLower() };
            var prediction = engine.Predict(data);
            return prediction.Words;
        }

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

        #region Clean Links
        /// <summary>
        /// This method takes a list of strings as argument and generates a set out of the strings by skipping empty strings.
        /// </summary>
        /// <param name="links"></param>
        /// <returns></returns>
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
    }
}
