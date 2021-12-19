using Panini.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TFIDF;

namespace Panini
{
    /// <summary>
    /// A class for sharing resources between view models.
    /// </summary>
    public class DataCache
    {
        private static DataCache singletonInstance;
        /// <summary>
        /// Corpus of the web-topics.
        /// </summary>
        /// <value>Corpus of the web-topics.</value>
        public Corpus corpus { get; set; }

        /// <summary>
        /// Path of the directory containing the web-topics.
        /// </summary>
        public string DirPath;

        /// <summary>
        /// Collection of <see cref="TopicItem"/> instances.
        /// </summary>
        /// <value>A list of <see cref="TopicItem"/> instances for each valid topic in the corpus.</value>
        public BindingList<TopicItem> TopicCollection = new BindingList<TopicItem>();

        private List<string> _listOfFiles;
        /// <summary>
        /// A collection of the files in the directory. This property binds to the <c>ItemsSource</c> attribute of <c>filesList</c> list box in the load view.
        /// </summary>
        /// <value>The names of the files read from the the selected directory are stored in this list.</value>
        public List<string> ListOfFiles
        {
            get { return _listOfFiles; }
            set { _listOfFiles = value; }
        }

        /// <summary>
        /// Dictionary for storing settings.
        /// </summary>
        /// <value>Dictionary for storing settings.</value>
        public Dictionary<string, object> Config;

        /// <summary>
        /// Dictionary for storing NLP related data.
        /// </summary>
        /// <value>Dictionary for storing NLP related data.</value>
        public Dictionary<string, object> NLPData;

        /// <summary>
        /// State that stores the enabled views.
        /// </summary>
        /// <value>Collection of enabled views.</value>
        public ObservableCollection<string> ViewState;

        private DataCache()
        {
            // Initialize config data
            Config = new Dictionary<string, object>();
            Config["keywordCount"] = 5;
            Config["similarTopicCount"] = 5;
            Config["colorScheme"] = "Default";
            Config["maxVocabSize"] = 500;
            Config["accelerator"] = "";
            Config["IsTitleRequested"] = true;

            // Initialize NLP data
            NLPData = new Dictionary<string, object>();
            NLPData["tokenizer"] = "Regexp Tokenizer";
            NLPData["similarityMeasure"] = "Term-Frequency Inverse-Document-Frequency (TFIDF)";
            NLPData["TFIDFVectorLength"] = 0;
            NLPData["numOfSentences"] = 0;
            NLPData["numOfTokens"] = 0;

            // View state
            ViewState = new ObservableCollection<string>();
            ViewState.Add("AboutViewEnabled");
            ViewState.Add("LoadViewEnabled");
        }

        /// <summary>
        /// Returns a singleton instance of <see cref="DataCache"/> class.
        /// </summary>
        public static DataCache Instance
        {
            get
            {
                return singletonInstance ?? (singletonInstance = new DataCache());
            }
        }


    }
}
