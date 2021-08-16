using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    public class WordsListModel: BaseModel
    {
        public string word { get; set; }
        public float tf { get; set; }
        public float idf { get; set; }
        public float tfidf { get; set; }

        private readonly DataCache dataCache = DataCache.Instance;

        public WordsListModel(string topicName,string token)
        {
            word = token;
            tf = (float) Math.Round(dataCache.corpus.concDict[topicName].tfidf.tfVector[token],2);
            idf = (float)Math.Round(dataCache.corpus.concDict[topicName].tfidf.idfVector[token],2);
            tfidf = (float)Math.Round(dataCache.corpus.concDict[topicName].tfidf.tfidfVector[token],2);
        }
    }
}
