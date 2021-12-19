using ILGPU.Runtime.CPU;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFIDF;

namespace Panini.Models
{
    /// <summary>
    /// Model for displaying topics in the <c>Topics List</c> section of the <c>Results View</c>.
    /// </summary>
    public class TopicItem: BaseModel
    {
        #region Property Declarations
        /// <summary>
        /// Single instance of <see cref="DataCache"/>.
        /// </summary>
        /// <value>Used for sharing resources between view models.</value>
        private readonly DataCache dataCache = DataCache.Instance;
        /// <summary>
        /// Name of the topic.
        /// </summary>
        /// <value>Name of the topic.</value>
        public string Name {get; set;}
        /// <summary>
        /// Filename of the topic.
        /// </summary>
        /// <value>The filename of the topic.</value>
        public string fileName { get; set; }
        /// <summary>
        /// Display name of the topic item.
        /// </summary>
        /// <value>The name to be displayed in the topic item entry (topic title or file name).</value>
        public string displayName { get; set; }
        /// <summary>
        /// Path of the topic.
        /// </summary>
        /// <value>Path of the topic.</value>
        public string path { get; set; }
        /// <summary>
        /// Collection of <see cref="TopicResultItem"/> instances.
        /// </summary>
        /// <value>Collection used for displaying the similar topic suggetions.</value>
        public ObservableCollection<TopicResultItem> itemCollection { get; set; }
        /// <summary>
        /// Ratio of number of words of the topic to the total number of words in the corpus.
        /// </summary>
        /// <value>Ratio of words of the topic to the total words in the corpus.</value>
        public int wordsRatio { get; set; }
        /// <summary>
        /// Tooltip for displaying the <see cref="wordsRatio"/>.
        /// </summary>
        /// <value>Tooltip for displaying the <see cref="wordsRatio"/>.</value>
        public string wordsRatioTooltip { get; set; }
        /// <summary>
        /// Ratio of number of inline links in the topic to the total number of inline links in all topics.
        /// </summary>
        /// <value>Ratio of number of inline links in the topic to the total number of inline links in all topics.</value>
        public int xrefsRatio { get; set; }
        /// <summary>
        /// Tooltip for displaying the <see cref="xrefsRatio"/>.
        /// </summary>
        /// <value>Tooltip for displaying the <see cref="xrefsRatio"/>.</value>
        public string xrefsRatioTooltip { get; set; }
        /// <summary>
        /// Ratio of number of related links in the topic to the total number of relted links in all topics.
        /// </summary>
        /// <value>Ratio of number of related links in the topic to the total number of relted links in all topics.</value>
        public int relinksRatio { get; set; }
        /// <summary>
        /// Tooltip for displaying the <see cref="relinksRatio"/>.
        /// </summary>
        /// <value>Tooltip for displaying the <see cref="relinksRatio"/>.</value>
        public string relinksRatioTooltip { get; set; }
        /// <summary>
        /// The partition size (grid width) for stacked bar display.
        /// </summary>
        /// <value>The partition size (grid width) for stacked bar display.</value>
        public float partition = 100;

        /// <summary>
        /// Number of sentences in the topic.
        /// </summary>
        /// <value>Number of sentences in the topic.</value>
        public int NumOfSentences
        {
            get { return dataCache.corpus.concDict[fileName].sentCount; }
        }
        /// <summary>
        /// Number of words in the topic.
        /// </summary>
        /// <value>Number of words in the topic.</value>
        public int NumOfUniqueWords
        {
            get { return dataCache.corpus.concDict[fileName].words.Count; }
        }
        /// <summary>
        /// Number of inline links in the topic.
        /// </summary>
        /// <value>Number of inline links in the topic.</value>
        public int NumOfInlineLinks

        {
            get { return dataCache.corpus.concDict[fileName].xrefs.Count; }
        }
        /// <summary>
        /// Number of related links in the topic.
        /// </summary>
        /// <value>Number of related links in the topic.</value>
        public int NumOfRelatedLinks
{
            get { return dataCache.corpus.concDict[fileName].relinks.Count; }
        }

        private string _isVisible = "Visible";
        /// <summary>
        /// Controls the visibility of the topic in the <c>Topics List</c> section of the results view.
        /// </summary>
        /// <value>Controls the visibility of the topic in the <c>Topics List</c> section of the results view.</value>
        public string IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<WordsListModel> _wordsList;
        /// <summary>
        /// Words common to the topic and the lexicon.
        /// </summary>
        /// <value>Words common to the topic and the lexicon.</value>
        public ObservableCollection<WordsListModel> WordsList
        {
            get { return _wordsList; }
            set { _wordsList = value; RaisePropertyChanged(); }
        }


        private ObservableCollection<InlineLinksModel> _inlineLinksList;
        /// <summary>
        /// List of inline links in the topic.
        /// </summary>
        /// <value>List of inline links in the topic.</value>
        public ObservableCollection<InlineLinksModel> InlineLinksList
        {
            get { return _inlineLinksList; }
            set { _inlineLinksList = value; RaisePropertyChanged(); }
        }


        private ObservableCollection<RelatedLinksModel> _relatedLinksModels;
        /// <summary>
        /// List of related links in the topic.
        /// </summary>
        /// <value>List of related links in the topic.</value>
        public ObservableCollection<RelatedLinksModel> RelatedLinksList
        {
            get { return _relatedLinksModels; }
            set { _relatedLinksModels = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiates the <see cref="TopicItem"/> class for a given topic.
        /// </summary>
        /// <param name="name">Display name of the topic.</param>
        /// <param name="isVisible">Visibility of the topic in the <c>Topics List</c> of the results view.
        /// <para>This flag is used to hide topic from the list when the list is filter against a search query.</para></param>
        /// <param name="itemCollection">A collection of <see cref="TopicResultItem"/> instances for displaying them as 
        /// similar topic suggestions in the <c>Similar Topics</c> section of the results view.</param>
        public TopicItem(string name, string isVisible, ObservableCollection<TopicResultItem> itemCollection)
        {
            fileName = name;
            Name = dataCache.corpus.concDict[fileName].topicName;
            displayName = (bool)dataCache.Config["IsTitleRequested"] ? Name : fileName;
            path = dataCache.corpus.concDict[fileName].path;
            this.itemCollection = itemCollection;
            IsVisible = isVisible;
            WordsList = new ObservableCollection<WordsListModel>();
            foreach (var word in dataCache.corpus.concDict[fileName].tfidf.tfidfVector.OrderByDescending(n=>n.Value).Take(50).Select(n => n.Key))
            {
                if(dataCache.corpus.concDict[fileName].words.Contains(word)) WordsList.Add(new WordsListModel(fileName, word));
            }

            InlineLinksList = new ObservableCollection<InlineLinksModel>();
            foreach(var tag in dataCache.corpus.concDict[fileName].xrefTags)
            {
                InlineLinksList.Add(new InlineLinksModel(fileName, tag));
            }

            RelatedLinksList = new ObservableCollection<RelatedLinksModel>();
            foreach (var tag in dataCache.corpus.concDict[fileName].relinkTags)
            {
                RelatedLinksList.Add(new RelatedLinksModel(fileName, tag));
            }

            compute_ratios();
        }

        /// <summary>
        /// Overrides the <c>ToString()</c> method of the class, to display the name of the topic instead.
        /// </summary>
        /// <returns>The name of the topic to which the <see cref="TopicItem"/> class instance is associated.</returns>
        public override string ToString()
        {
            return fileName;
        }

        /// <summary>
        /// Gets the topic names from the similar topic suggestions.
        /// </summary>
        /// <returns>Names of the similar topics.</returns>
        public List<string> get_topic_names()
        {
            List<string> names = new List<string>();
            foreach(var item in itemCollection)
            {
                names.Add(item.fileName);
            }
            return names;
        }
        #endregion

        #region Compute Ratios
        /// <summary>
        /// Computes the ratios required to generate the stacked bar chart in the topics list of the results view.
        /// </summary>
        public void compute_ratios()
        {
            // Calculate fraction of words compared to max
            float wordCount = dataCache.corpus.concDict[fileName].words.Count();
            wordsRatioTooltip = $"Words in topic : {(int)wordCount}";
            float maxWordCount = dataCache.corpus.wordsMax;
            if(wordCount != 0.0f && maxWordCount != 0.0f)
            {
            wordsRatio = (int)((wordCount / maxWordCount)*partition);
            }
            else { wordsRatio = 0; }

            // Calculate fraction of xrefs compared to max
            float xrefCount = dataCache.corpus.concDict[fileName].xrefs.Count();
            xrefsRatioTooltip = $"Inline links in topic : {(int)xrefCount}";
            float maxXrefCount = dataCache.corpus.xrefsMax;
            xrefsRatio = (xrefCount != 0 && maxXrefCount != 0) ? (int)((xrefCount / maxXrefCount)*partition*0.5) : 0;

            // Calculate fraction of relinks compared to max
            float relinkCount = dataCache.corpus.concDict[fileName].relinks.Count();
            relinksRatioTooltip = $"Related links in topic : {(int)relinkCount}";
            float maxrelinkCount = dataCache.corpus.relinksMax;
            relinksRatio = (relinkCount != 0 && maxrelinkCount != 0) ? (int)((relinkCount / maxrelinkCount)*partition*0.5) : 0;
        }
        #endregion
    }
}
