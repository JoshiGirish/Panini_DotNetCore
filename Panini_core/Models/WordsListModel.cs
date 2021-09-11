using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    /// <summary>
    /// Model for displaying the words data in the <c>Results View</c>.
    /// </summary>
    public class WordsListModel: BaseModel
    {
        /// <summary>
        /// Word in the topic.
        /// </summary>
        /// <value>Word in the topic.</value>
        public string word { get; set; }

        /// <summary>
        /// Term-Frequency of the word.
        /// </summary>
        /// <value>Term-Frequency of the word.</value>
        public float tf { get; set; }

        /// <summary>
        /// Inverse Document-Frequency of the word.
        /// </summary>
        /// <value>Inverse Document-Frequency of the word.</value>
        public float idf { get; set; }

        /// <summary>
        /// Term-Frequency Inverse Document-Frequency of the word.
        /// </summary>
        /// <value>Term-Frequency Inverse Document-Frequency of the word.</value>
        public float tfidf { get; set; }

        private readonly DataCache dataCache = DataCache.Instance;

        /// <summary>
        /// Instantiates <see cref="WordsListModel"/> class instance for each word.
        /// </summary>
        /// <param name="topicName">Name of the topic to which the word belongs.</param>
        /// <param name="token">Word for which the model is to be instantiated.</param>
        public WordsListModel(string topicName,string token)
        {
            word = token;
            tf = (float) Math.Round(dataCache.corpus.concDict[topicName].tfidf.tfVector[token],2);
            idf = (float)Math.Round(dataCache.corpus.concDict[topicName].tfidf.idfVector[token],2);
            tfidf = (float)Math.Round(dataCache.corpus.concDict[topicName].tfidf.tfidfVector[token],2);
        }
    }
}
