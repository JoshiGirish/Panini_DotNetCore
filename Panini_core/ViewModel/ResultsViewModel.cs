﻿using Panini.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Panini.Models;
using System.Collections.Specialized;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using TFIDF;
using System.Threading.Tasks;
using Microsoft.Win32;
using ICSharpCode.AvalonEdit;
using System.Runtime.InteropServices;
using System.Diagnostics;
using LiveCharts;
using LiveCharts.Wpf;

namespace Panini.ViewModel
{
    class ResultsViewModel : BaseViewModel
    {
        // Declaration
        #region Property Declarations
        private readonly DataCache dataCache = DataCache.Instance;
        private BindingList<TopicItem> _topicCollection = new BindingList<TopicItem>();
        public BindingList<TopicItem> TopicCollection
        { get => _topicCollection; set { _topicCollection = value; RaisePropertyChanged(); } }

        private BindingList<TopicItem> _dummyTopicCollection = new BindingList<TopicItem>();

        public TopicResultItem selectedResultItem { get; set; }
        public int maxVisibleSimilarTopics { get { return maxVisibleSimilarTopics; } set { maxVisibleSimilarTopics = value; RaisePropertyChanged(); } }
        private float _progress;
        


        #region ProgressBar Visibility
        /// <summary>
        /// Progress bar in the status bar.
        /// </summary>
        private string _progressBarVisibility = "Collapsed";

        public string ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set { _progressBarVisibility = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Results Visibility
        /// <summary>
        /// Controls visiblity of the result topic list and the details view.
        /// </summary>
        private string _resultsVisibility = "Collapsed";

        public string ResultsVisibility
        {
            get { return _resultsVisibility; }
            set { _resultsVisibility = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Processing Flag
        /// <summary>
        /// Flag to display the processing prompt.
        /// </summary>
        private string _processingMessageVisibility = "Collapsed";

        public string ProcessingMessageVisibility
        {
            get { return _processingMessageVisibility; }
            set { _processingMessageVisibility = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Processing Stage
        private string _processingStage;

        public string ProcessingStage
        {
            get { return _processingStage; }
            set { _processingStage = value; RaisePropertyChanged(); }
        }

        #endregion

        public string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged();
            }
        }

        private Color _statusBarColor = new Color();

        public Color StatusBarColor
        {
            get { return _statusBarColor; }
            set
            {
                _statusBarColor = value;
                RaisePropertyChanged();
            }
        }

        public float Progress
        {
            get { return _progress; }
            set { _progress = value; RaisePropertyChanged(); }
        }

        private TextEditor _textEditor;

        public TextEditor XMLEditor
        {
            get { return _textEditor; }
            set { _textEditor = value; RaisePropertyChanged(); }
        }

        public Stopwatch stopwatch = new Stopwatch();

        private long _topicGenerationTime;

        public long TopicGenerationTime
        {
            get { return _topicGenerationTime; }
            set { _topicGenerationTime = value; RaisePropertyChanged(); }
        }

        private long _tfidfComputationTime;

        public long TFIDFComputationTime
        {
            get { return _tfidfComputationTime; }
            set { _tfidfComputationTime = value; RaisePropertyChanged(); }
        }

        private long _simScoreComputationTime;

        public long SimScoreComputationTime
        {
            get { return _simScoreComputationTime; }
            set { _simScoreComputationTime = value; RaisePropertyChanged(); }
        }
        #endregion


        // Execution Methods
        #region Run TF-IDF CallBack
        /// <summary>
        /// This ICommand binds to the Run (Play icon) command in the ResultsPage.
        /// </summary>
        private ICommand _run;
        public ICommand Run
        {
            get { return _run ?? (_run = new ButtonCommandHandler(() => RunCallback(), () => RunCanExecute)); }
        }

        public bool RunCanExecute { get { return true; }}

        private void RunCallback()
        {
            dataCache.corpus.reset_corpus((int)dataCache.Config["maxVocabSize"]);
            TopicCollection.Clear();
            Thread generateThread = new Thread(new ThreadStart(compute_results));
            generateThread.IsBackground = true;
            generateThread.Start();
        }

        #endregion

        #region OpenFile callback
   //     private ICommand _openXMLFile;

   //     public ICommand OpenXMLFile
   //     {
   //         get
   //         {
   //             return _openXMLFile ?? (_openXMLFile = new ButtonCommandHandler(() => open_xml_file(), () => { return true; }));
   //         }
   //     }
   //     string currentFileName;
   //     private void open_xml_file()
   //     {
   //         OpenFileDialog dlg = new OpenFileDialog();
			//dlg.CheckFileExists = true;
			//if (dlg.ShowDialog() ?? false) {
			//	currentFileName = dlg.FileName;
			//	textEditor.Load(currentFileName);
			//	textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
			//}
   //     }
        #endregion

        #region Compute Results Method
        /// <summary>
        /// This method invokes the topic creation, TF-IDF computation, and similarity score computation methods on the corpus.
        /// </summary>
        private void compute_results()
        {

            // Display the processing prompt and progress bar
            ResultsVisibility = "Collapsed"; // Hide the previous results if already computed
            ProcessingMessageVisibility = "Visible";
            ProgressBarVisibility = "Visible";
            Status = $"Please wait ...";
            //StatusBarColor = StatusColors["Warning"];     
            SearchText = string.Empty;


            stopwatch = Stopwatch.StartNew();
            ProcessingStage = "Generating Lexicon and Topic Instances";
            dataCache.corpus.generate_topics_async();
            dataCache.corpus.compute_max();
            stopwatch.Stop();
            TopicGenerationTime = stopwatch.ElapsedMilliseconds / 1000;

            stopwatch = Stopwatch.StartNew();
            ProcessingStage = "Calculating TFIDF Scores";
            dataCache.corpus.calculate_tfidf_scores_async();
            stopwatch.Stop();
            TFIDFComputationTime = stopwatch.ElapsedMilliseconds / 1000;

            stopwatch = Stopwatch.StartNew();
            ProcessingStage = "Calculating Cosine Similarity Scores";
            dataCache.corpus.calculate_topic_similarity_scores_async();
            stopwatch.Stop();
            SimScoreComputationTime = stopwatch.ElapsedMilliseconds / 1000;

            ProcessingMessageVisibility = "Collapsed";
            ProgressBarVisibility = "Collapsed";
            Status = "";
            foreach (var topic in dataCache.corpus.concDict.Values)
            {
                var enumItems = dataCache.corpus.get_similar_topics(topic, (int)dataCache.Config["similarTopicCount"])
                                    .Select(top => new TopicResultItem()
                                    {
                                        Name = top.topicName,
                                        Path = top.path,
                                        simScore = (float)dataCache.corpus.get_similarity_score(topic, top),
                                        words = dataCache.corpus.get_similar_words(topic, top, 100),
                                        Keywords = dataCache.corpus.get_similar_words(topic, top, (int)dataCache.Config["keywordCount"]),
                                        NumOfInlineLinks = top.xrefs.Count,
                                        NumOfRelatedLinks = top.relinks.Count,
                                        IsLinked = topic.get_all_link_names().Contains(top.topicName) == true ? "Visible" : "Collapsed"
                                    });
                var itemColl = new ObservableCollection<TopicResultItem>();
                foreach (var item in enumItems)
                {
                    itemColl.Add(item);
                }
                App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        TopicCollection.Add(new TopicItem(topic.topicName, "Visible", true ,itemColl));
                    });
            }
            _dummyTopicCollection = TopicCollection;
            dataCache.TopicCollection = TopicCollection;
            ResultsVisibility = "Visible"; // display the results
            dataCache.ViewState.Add("SummaryViewEnabled");
        }
        #endregion

        // View Management Methods
        #region ExpandAll Command CallBack
        /// <summary>
        /// This ICommand binds to the ExpandAll button in the ResultsPage. All topic expanders are expanded.
        /// </summary>
        private ICommand _expandAll;
        public ICommand ExpandAll
        {
            get { return _expandAll ?? (_expandAll = new ButtonCommandHandler(() => ExpandAllCallback(), () => ExpandAllCanExecute)); }
        }

        private void ExpandAllCallback()
        {
            Parallel.ForEach(TopicCollection, (topItem) =>
            {
                topItem.IsExpanded = true;
            });
        }
        public bool ExpandAllCanExecute { get { return true; } }
        #endregion

        #region CollapseAll Command CallBack
        /// <summary>
        /// This ICommand binds to the CollapseAll button on the ResultsPage. It collapses all the topic expanders at once.
        /// </summary>
        private ICommand _collapseAll;
        public ICommand CollapseAll
        {
            get { return _collapseAll ?? (_collapseAll = new ButtonCommandHandler(() => CollapseAllCallBack(), () => CollapseAllCanExecute)); }
        }

        private void CollapseAllCallBack()
        {
            foreach (var topItem in TopicCollection)
            {
                topItem.IsExpanded = false;
            }
        }
        public bool CollapseAllCanExecute { get { return true; } }
        #endregion

        #region Search Command CallBack
        /// <summary>
        /// This ICommand updates the TopicCollection depending on the search query. The topic expander headers which do not contain the search text are hidden (Visibility => Collapsed)
        /// </summary>
        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; RaisePropertyChanged(); }
        }

        private ICommand _search;

        public ICommand Search
        {
            get { return _search ?? (_search = new ButtonCommandHandler(() => TopicCollection_CollectionChanged(), () => SearchCanExecute)); }
        }

        private void TopicCollection_CollectionChanged()
        {
            //var visibleTopics = _dummyTopicCollection.Where(n => n.Name.ToLower().Contains(SearchText.ToLower())).OrderBy(n => n.Name);
            //TopicCollection = new ObservableCollection<TopicItem>();
            //foreach (var topItem in visibleTopics)
            //{
            //    TopicCollection.Add(topItem);
            //}
            foreach(var topItem in TopicCollection)
            {
                if (!topItem.Name.ToLower().Contains(SearchText.ToLower()))
                {
                    topItem.IsVisible = "Collapsed";
                }
                else
                {
                    topItem.IsVisible = "Visible";
                }
            }
        }
        public bool SearchCanExecute { get { return true; } }
        #endregion

        #region Open Topic Command CallBack
        /// <summary>
        /// This ICommand binds to the file icon of each TopicResultItem. It opens the corresponding topic HTML page in the default browser.
        /// </summary>
        private ICommand _openFile;

        public ICommand OpenFile
        {
            get { return _openFile ?? (_openFile = new ParameterCommandHandler((grid) => open_file(grid), () => { return true; })); }
        }

        public void open_file(object gridObj)
        {
            DataGrid grid = (DataGrid)gridObj;
            TopicResultItem item = (TopicResultItem)grid.SelectedItem;
            //System.Diagnostics.Process.Start($"{item.Path}");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo($"{item.Path}") { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", $"{item.Path}");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", $"{item.Path}");
            }
            else
            {
                // throw 
            }
        }
        #endregion

        // Config Methods
        #region Update Keywords Method
        /// <summary>
        /// This method updates the keywords in the TopicResultItem of the ResultsPage based on the Keyword count config setting.
        /// </summary>
        private ICommand _updateKeywords;
        public ICommand UpdateKeywords
        {
            get { return _updateKeywords ?? (_updateKeywords = new ButtonCommandHandler(() => update_similar_words(), () => { return true; })); }
        }

        private void update_similar_words()
        {
            foreach(var topicItem in TopicCollection)
            {
                if (topicItem.IsVisible == "Visible")
                {
                   foreach(var resultItem in topicItem.itemCollection)
                    {
                        var enumKeywords = resultItem.words.Take((int)dataCache.Config["keywordCount"]);
                        resultItem.Keywords.Clear();
                        foreach(var keyword in enumKeywords)
                        {
                            resultItem.Keywords.Add(keyword);
                        }
                    }
                }
            }
        }
        #endregion

    }
}
