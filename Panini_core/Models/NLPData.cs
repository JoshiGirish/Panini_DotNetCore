namespace Panini.Models
{
    public class NLPData : BaseModel
    {
        #region Tokinzer
        /// <summary>
        /// This property represents tht tokenizer used for tokenizing the sentences.
        /// </summary>
        private string _tokenizer = "Regexp Tokenizer";

        public string Tokenizer
        {
            get { return _tokenizer; }
            set { _tokenizer = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Tagger
        /// <summary>
        /// This property represents the POS tagger used to tag the words in the sentences.
        /// </summary>
        private string _tagger;

        public string Tagger
        {
            get { return _tagger; }
            set { _tagger = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Similarity Measure
        /// <summary>
        /// This property represents the similarity measure (TF-IDF) used for co-relating the topics.
        /// </summary>
        private string _similarityMeasure = "Term-Frequency Inverse-Document-Frequency (TFIDF)";

        public string SimilarityMeasure
        {
            get { return _similarityMeasure; }
            set { _similarityMeasure = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Length of TF-IDF Vectors
        /// <summary>
        /// This property represents the size of the TF-IDF vector array for each topic.
        /// </summary>
        private int _tfidfVectorLength;

        public int TfIdfVectorLength
        {
            get { return _tfidfVectorLength; }
            set { _tfidfVectorLength = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Sentences in guide
        /// <summary>
        /// This property represents the total number of sentences in all the topics.
        /// </summary>
        private int _numOfSentences;

        public int NumOfSentences
        {
            get { return _numOfSentences; }
            set { _numOfSentences = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Tokens in Lexicon
        /// <summary>
        /// This property represents the total number of tokens in the lexicon.
        /// </summary>
        private int _numOfTokens;

        public int NumOfTokens
        {
            get { return _numOfTokens; }
            set { _numOfTokens = value; RaisePropertyChanged(); }
        }
        #endregion

        #region IsVisible property
        /// <summary>
        /// This property binds to the Visibility property of the NLP Data expander in the Summary page.
        /// </summary>
        private string _isVisible;

        public string IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; RaisePropertyChanged(); }
        }
        #endregion

        #region IsExpanded property
        /// <summary>
        /// This property binds to the IsExpanded property of the NLP Data expander in the Summary page.
        /// </summary>
        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; RaisePropertyChanged(); }
        }
        #endregion

    }
}
