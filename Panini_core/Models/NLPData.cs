namespace Panini.Models
{
    /// <summary>
    /// Model for displaying the summary of NLP information in <c>Summary View</c>.
    /// </summary>
    public class NLPData : BaseModel
    {
        #region Tokinzer
        
        private string _tokenizer = "Regexp Tokenizer";
        /// <summary>
        /// Tokenizer used for tokenizing the sentences.
        /// </summary>
        /// <value>Tokenizer used for tokenizing the sentences.</value>
        public string Tokenizer
        {
            get { return _tokenizer; }
            set { _tokenizer = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Tagger
        
        private string _tagger;
        /// <summary>
        /// POS tagger used to tag the words in the sentences.
        /// </summary>
        /// <value>POS tagger used to tag the words in the sentences.</value>
        public string Tagger
        {
            get { return _tagger; }
            set { _tagger = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Similarity Measure
        
        private string _similarityMeasure = "Term-Frequency Inverse-Document-Frequency (TFIDF)";
        /// <summary>
        /// Similarity measure (TF-IDF) used for co-relating the topics.
        /// </summary>
        /// <value>Similarity measure (TF-IDF) used for co-relating the topics.</value>
        public string SimilarityMeasure
        {
            get { return _similarityMeasure; }
            set { _similarityMeasure = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Length of TF-IDF Vectors
        
        private int _tfidfVectorLength;
        /// <summary>
        /// Size of the TF-IDF vector array for each topic.
        /// </summary>
        /// <value>Size of the TF-IDF vector array for each topic.</value>
        public int TfIdfVectorLength
        {
            get { return _tfidfVectorLength; }
            set { _tfidfVectorLength = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Sentences in guide
        
        private int _numOfSentences;
        /// <summary>
        /// Total number of sentences in all the topics.
        /// </summary>
        /// <value>Total number of sentences in all the topics.</value>
        public int NumOfSentences
        {
            get { return _numOfSentences; }
            set { _numOfSentences = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Tokens in Lexicon
        
        private int _numOfTokens;
        /// <summary>
        /// Total number of tokens in the lexicon.
        /// </summary>
        /// <value>Total number of tokens in the lexicon.</value>
        public int NumOfTokens
        {
            get { return _numOfTokens; }
            set { _numOfTokens = value; RaisePropertyChanged(); }
        }
        #endregion

        #region IsVisible property
        
        private string _isVisible;
        /// <summary>
        /// Visibility property of the NLP Data expander in the Summary page.
        /// </summary>
        /// <value>Visibility property of the NLP Data expander in the Summary page.</value>
        public string IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; RaisePropertyChanged(); }
        }
        #endregion

        #region IsExpanded property
        
        private bool _isExpanded;
        /// <summary>
        /// IsExpanded property of the NLP Data expander in the Summary page.
        /// </summary>
        /// <value>IsExpanded property of the NLP Data expander in the Summary page.</value>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; RaisePropertyChanged(); }
        }
        #endregion

    }
}
