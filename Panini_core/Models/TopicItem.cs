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
    public class TopicItem: BaseModel
    {
        #region Property Declarations
        private readonly DataCache dataCache = DataCache.Instance;
        public string Name {get; set;}
        public string path { get; set; }
        public ObservableCollection<TopicResultItem> itemCollection { get; set; }
        public int wordsRatio { get; set; }
        public string wordsRatioTooltip { get; set; }
        public int xrefsRatio { get; set; }
        public string xrefsRatioTooltip { get; set; }
        public int relinksRatio { get; set; }
        public string relinksRatioTooltip { get; set; }
        public float partition = 100;

        public int NumOfSentences
        {
            get { return dataCache.corpus.concDict[Name].sentCount; }
        }

        public int NumOfUniqueWords
        {
            get { return dataCache.corpus.concDict[Name].words.Count; }
        }

        public int NumOfInlineLinks

        {
            get { return dataCache.corpus.concDict[Name].xrefs.Count; }
        }

        public int NumOfRelatedLinks
{
            get { return dataCache.corpus.concDict[Name].relinks.Count; }
        }


        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; RaisePropertyChanged(); }
        }

        private string _isVisible = "Visible";

        public string IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<WordsListModel> _wordsList;
        public ObservableCollection<WordsListModel> WordsList
        {
            get { return _wordsList; }
            set { _wordsList = value; RaisePropertyChanged(); }
        }


        private ObservableCollection<InlineLinksModel> _inlineLinksList;

        public ObservableCollection<InlineLinksModel> InlineLinksList
        {
            get { return _inlineLinksList; }
            set { _inlineLinksList = value; RaisePropertyChanged(); }
        }


        private ObservableCollection<RelatedLinksModel> _relatedLinksModels;

        public ObservableCollection<RelatedLinksModel> RelatedLinksList
        {
            get { return _relatedLinksModels; }
            set { _relatedLinksModels = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Constructor
        public TopicItem(string name, string isVisible, bool isExpanded, ObservableCollection<TopicResultItem> itemCollection)
        {
            Name = name;
            path = dataCache.corpus.concDict[Name].path;
            this.itemCollection = itemCollection;
            IsVisible = isVisible;
            IsExpanded = IsExpanded;
            WordsList = new ObservableCollection<WordsListModel>();
            foreach (var word in dataCache.corpus.concDict[Name].tfidf.tfidfVector.OrderByDescending(n=>n.Value).Take(50).Select(n => n.Key))
            {
                if(dataCache.corpus.concDict[Name].words.Contains(word)) WordsList.Add(new WordsListModel(Name, word));
            }

            InlineLinksList = new ObservableCollection<InlineLinksModel>();
            foreach(var tag in dataCache.corpus.concDict[Name].xrefTags)
            {
                InlineLinksList.Add(new InlineLinksModel(Name, tag));
            }

            RelatedLinksList = new ObservableCollection<RelatedLinksModel>();
            foreach (var tag in dataCache.corpus.concDict[Name].relinkTags)
            {
                RelatedLinksList.Add(new RelatedLinksModel(Name, tag));
            }

            compute_ratios();
        }
        public override string ToString()
        {
            return Name;
        }

        public List<string> get_topic_names()
        {
            List<string> names = new List<string>();
            foreach(var item in itemCollection)
            {
                names.Add(item.Name);
            }
            return names;
        }
        #endregion

        #region Compute Ratios
        /// <summary>
        /// This method computes the ratios required to generate the stacked bar chart in the topics list of the results view.
        /// </summary>
        public void compute_ratios()
        {
            // Calculate fraction of words compared to max
            float wordCount = dataCache.corpus.concDict[Name].words.Count();
            wordsRatioTooltip = $"Words in topic : {(int)wordCount}";
            float maxWordCount = dataCache.corpus.wordsMax;
            if(wordCount != 0.0f && maxWordCount != 0.0f)
            {
            wordsRatio = (int)((wordCount / maxWordCount)*partition);
            }
            else { wordsRatio = 0; }

            // Calculate fraction of xrefs compared to max
            float xrefCount = dataCache.corpus.concDict[Name].xrefs.Count();
            xrefsRatioTooltip = $"Inline links in topic : {(int)xrefCount}";
            float maxXrefCount = dataCache.corpus.xrefsMax;
            xrefsRatio = (xrefCount != 0 && maxXrefCount != 0) ? (int)((xrefCount / maxXrefCount)*partition*0.5) : 0;

            // Calculate fraction of relinks compared to max
            float relinkCount = dataCache.corpus.concDict[Name].relinks.Count();
            relinksRatioTooltip = $"Related links in topic : {(int)relinkCount}";
            float maxrelinkCount = dataCache.corpus.relinksMax;
            relinksRatio = (relinkCount != 0 && maxrelinkCount != 0) ? (int)((relinkCount / maxrelinkCount)*partition*0.5) : 0;
        }
        #endregion
    }
}
