using Panini.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TFIDF;

namespace Panini
{
    public class DataCache
    {
        private static DataCache singletonInstance;
        public Corpus corpus { get; set; }
        public string DirPath;
        private List<string> _listOfFiles;
        public BindingList<TopicItem> TopicCollection = new BindingList<TopicItem>();
        public List<string> ListOfFiles
        {
            get { return _listOfFiles; }
            set { _listOfFiles = value; }
        }
        public EventHandler OnMessageChanged;
        public Dictionary<string, object> Config;
        public Dictionary<string, object> NLPData;

        private DataCache()
        {
            // Initialize config data
            Config = new Dictionary<string, object>();
            Config["keywordCount"] = 5;
            Config["similarTopicCount"] = 5;
            Config["colorScheme"] = "Default";
            Config["maxVocabSize"] = 500;

            // Initialize NLP data
            NLPData = new Dictionary<string, object>();
            NLPData["tokenizer"] = "Regexp Tokenizer";
            NLPData["similarityMeasure"] = "Term-Frequency Inverse-Document-Frequency (TFIDF)";
            NLPData["TFIDFVectorLength"] = 0;
            NLPData["numOfSentences"] = 0;
            NLPData["numOfTokens"] = 0;

        }

        public static DataCache Instance
        {
            get
            {
                return singletonInstance ?? (singletonInstance = new DataCache());
            }
        }
    }
}
