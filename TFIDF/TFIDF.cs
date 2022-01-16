using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFIDF
{
    /// <summary>
    /// The class that handles the TFIDF computation for each <c>Topic</c> instance.
    /// </summary>
    public class TFIDF
    {
        /// <summary>
        /// Term-Frequency Inverse Document-Frequency vector for the topic.
        /// </summary>
        /// <value>This vector stores the TFIDF scores of each word of the lexicon that appears in the given topic. 
        /// This vector is later used for computing cosine similarity between topics.
        /// <para>The length of the vector is equal to the maximum vocabulary size of the <see cref="Lexicon"/>.</para></value>
        public SortedDictionary<string, double> tfidfVector = new SortedDictionary<string, double>(); // size => no of words in lexicon

        /// <summary>
        /// Term-Frequency vector for the topic.
        /// </summary>
        /// <value>This vector stores the TF scores of each word of the lexicon that appears in the given topic.
        /// The higher the score of a word in the vector, the more are the occurances of the word in the topic.
        /// <para>The length of the vector is equal to the maximum vocabulary size of the <see cref="Lexicon"/>.</para></value>
        public SortedDictionary<string, double> tfVector = new SortedDictionary<string, double>(); // size => no of words in lexicon

        /// <summary>
        /// Inverse Document-Frequency vector for the topic.
        /// </summary>
        /// <value>This vector stores the IDF scores of each word of the lexicon that appears in the given topic.
        /// The higher the score of a word in the vector, the more unique is the word for the topic.
        /// <para>The length of the vector is equal to the maximum vocabulary size of the <see cref="Lexicon"/>.</para></value>
        public SortedDictionary<string, double> idfVector = new SortedDictionary<string, double>(); // size => no of words in lexicon

        /// <summary>
        /// Word count vector for the topic.
        /// </summary>
        /// <value>This vector stores the number of occurrences of each word of the lexicon that appears in the given topic.
        /// <para>The length of the vector is equal to the maximum vocabulary size of the <see cref="Lexicon"/>.</para></value>
        public SortedDictionary<string, int> wordCountVector = new SortedDictionary<string, int>(); // size => no of words in lexicon

        /// <summary>
        /// Similarity scores of each topic with the given topic.
        /// </summary>
        /// <value>This vector stores the cosine similarity scores of the topic with every topic in the corpus.
        /// <para>The length of the vector is equal to the number of topics in the corpus.</para></value>
        public Dictionary<string, double> similarityScores = new Dictionary<string, double>(); // size => no of topics

        #region Constructor
        /// <summary>
        /// Computes the words counts and TFIDF scores.
        /// </summary>
        /// <param name="topic">The topic being analyzed.</param>
        /// <param name="topics">The list of topics in the corpus.</param>
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

            // This loop calculates the tfidf value for each word in the topic and adds it to the tfidfVector.
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
        #endregion
    }
}
