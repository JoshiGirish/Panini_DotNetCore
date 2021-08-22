using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFIDF
{
    public class TFIDF
    {
        public SortedDictionary<string, double> tfidfVector = new SortedDictionary<string, double>(); // size => no of words in lexicon
        public SortedDictionary<string, double> tfVector = new SortedDictionary<string, double>(); // size => no of words in lexicon
        public SortedDictionary<string, double> idfVector = new SortedDictionary<string, double>(); // size => no of words in lexicon
        public SortedDictionary<string, int> wordCountVector = new SortedDictionary<string, int>(); // size => no of words in lexicon
        public Dictionary<string, double> similarityScores = new Dictionary<string, double>(); // size => no of topics

        public TFIDF(Topic topic, List<Topic> topics)
        {
            // This loop calculates the occurances of each word and adds it to the wordCountVector.
            // The maximum value of word count is used to normalize the term frequency value.
            foreach (var word in Lexicon.words)
            {
                var count = topic.words.Where(n => n == word).Count();
                wordCountVector.Add(word, count);
            }
            int max = wordCountVector.Values.Max();

            // This loops calculates the tfidf value for each word in the topic and adds it to the tfidfVector.
            foreach(var word in Lexicon.words)
            {
                // Calculate term frequency
                float tf = (float) wordCountVector[word]/max;

                // Calculate inverse document frequency
                float cn = topics.Where(top => top.words.Contains(word)).Count(); // number of documents that contain this word
                float dn = topics.Count(); // number of total documents
                double idf = Math.Log10( dn / (1.0 + cn));

                // Caluculate TFIDF value
                var tfidf = tf * idf;

                tfVector.Add(word, tf);
                idfVector.Add(word, idf);
                tfidfVector.Add(word, tfidf);
            }

            Console.WriteLine("TFIDF Nonzero value count : " + tfidfVector.Values.Where(n => n != 0).Count());
            //tfidfVector.Values.Where(n => n != 0).Select(n =>n).ToList().ForEach(Console.Write);
            //Console.WriteLine(string.Join(" ",tfidfVector.Values.ToList()));
        }
    }
}
