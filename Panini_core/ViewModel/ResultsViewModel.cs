using Panini.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Panini.Models;
using System.Threading;
using System;
using System.Windows.Controls;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using TFIDF;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Panini.ViewModel
{
    /// <summary>
    /// Manages the behavior of <c>Results View</c>.
    /// </summary>
    public class ResultsViewModel : BaseViewModel
    {
        // Declaration
        #region Property Declarations
        /// <summary>
        /// Single instance of <see cref="DataCache"/>.
        /// </summary>
        /// <value>Used for sharing resources between view models.</value>
        private readonly DataCache dataCache = DataCache.Instance;


        private BindingList<TopicItem> _topicCollection = new BindingList<TopicItem>();
        /// <summary>
        /// Collection of <see cref="TopicItem"/> instances.
        /// </summary>
        /// <value>A list of <see cref="TopicItem"/> instances for each valid topic in the corpus.</value>
        public BindingList<TopicItem> TopicCollection
        { get => _topicCollection; set { _topicCollection = value; RaisePropertyChanged(); } }

        private BindingList<TopicItem> _dummyTopicCollection = new BindingList<TopicItem>();


        private int _selectedTopicIndex = 0;
        public int SelectedTopicIndex 
        { 
            get { return _selectedTopicIndex; }
            set { _selectedTopicIndex = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Selected similar topic.
        /// </summary>
        /// <value>The <see cref="TopicResultItem"/></value> instance corresponding to the selected similar topic instance.
        public TopicResultItem selectedResultItem { get; set; }

        /// <summary>
        /// Maximum visible similar topics.
        /// </summary>
        /// <value>The highest number of topics to be suggested under the <c>Similar Topics</c> section in the results view.</value>
        public int maxVisibleSimilarTopics { get { return maxVisibleSimilarTopics; } set { maxVisibleSimilarTopics = value; RaisePropertyChanged(); } }
        private float _progress;
        
        #region ProgressBar Visibility
        private string _progressBarVisibility = "Collapsed";
        /// <summary>
        /// Controls visibility of the progress bar.
        /// </summary>
        /// <value>Controls visibility of the progress bar that appears in the status bar when creating lexicon and computing scores.</value>
        public string ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set { _progressBarVisibility = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Results Visibility
        private string _resultsVisibility = "Collapsed";
        /// <summary>
        /// Controls visibility of sections in results view.
        /// </summary>
        /// <value>Controls visiblity of the <c>Topics List</c>, <c>Similar Topics</c>, and <c>Topic Summary</c> sections in the results view.
        /// <para>The sections must be hidden during the processing phase.</para></value>
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
        /// <summary>
        /// Controls visibility of the processing message.
        /// </summary>
        /// <value>Controls the visibility of the processing message in the results view when the topics are being processed and score are being computed.</value>
        public string ProcessingMessageVisibility
        {
            get { return _processingMessageVisibility; }
            set { _processingMessageVisibility = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Processing Stage
        private string _processingStage;
        /// <summary>
        /// Current processing stage message.
        /// </summary>
        /// <value>Current processing stage message from the following:
        ///     <list type="bullet">
        ///         <item>
        ///             <term><c>Generating Lexicon and Topic Instances</c></term>
        ///         </item>
        ///         <item>
        ///             <term><c>Calculating TFIDF Scores</c></term>
        ///         </item>
        ///         <item>
        ///             <term><c>Calculating Cosine Similarity Scores</c></term>
        ///         </item>
        ///     </list>
        /// </value>
        public string ProcessingStage
        {
            get { return _processingStage; }
            set { _processingStage = value; RaisePropertyChanged(); }
        }

        #endregion

        private string _status;
        /// <summary>
        /// Status message. This property binds to <c>status</c> text block in the load view.
        /// </summary>
        /// <value>String representation of the status message to be displayed in the status bar at the bottom.</value>
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
        /// <summary>
        /// Status color. This property binds to <c>statusColorBrush</c> in the load view.
        /// </summary>
        /// <value>Color of the status bar.</value>
        public Color StatusBarColor
        {
            get { return _statusBarColor; }
            set
            {
                _statusBarColor = value;
                RaisePropertyChanged();
            }
        }


        //public float Progress
        //{
        //    get { return _progress; }
        //    set { _progress = value; RaisePropertyChanged(); }
        //}

        //private TextEditor _textEditor;

        //public TextEditor XMLEditor
        //{
        //    get { return _textEditor; }
        //    set { _textEditor = value; RaisePropertyChanged(); }
        //}

        /// <summary>
        /// Stopwatch.
        /// </summary>
        /// <value>Stopwatch for timing the processing stages.</value>
        public Stopwatch stopwatch = new Stopwatch();

        private long _topicGenerationTime;
        /// <summary>
        /// Topic generation and lexicon creation time.
        /// </summary>
        /// <value>Time (in secs) required to instantiate <see cref="Topic"/> classes and create <see cref="Lexicon"/>.</value>
        public long TopicGenerationTime
        {
            get { return _topicGenerationTime; }
            set { _topicGenerationTime = value; RaisePropertyChanged(); }
        }

        private long _tfidfComputationTime;
        /// <summary>
        /// TFIDF score computation time.
        /// </summary>
        /// <value>Time (in secs) required to instantiate <see cref="TFIDF"/> classes for each topic and compute TFIDF scores.</value>
        public long TFIDFComputationTime
        {
            get { return _tfidfComputationTime; }
            set { _tfidfComputationTime = value; RaisePropertyChanged(); }
        }

        private long _simScoreComputationTime;
        /// <summary>
        /// Similarity score computation time.
        /// </summary>
        /// <value>Time (in secs) required to compute cosine similarity scores for each topic.</value>
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
        /// <summary>
        /// Command that invokes the execution of the analysis.
        /// </summary>
        public ICommand Run
        {
            get { return _run ?? (_run = new ButtonCommandHandler(() => RunCallback(), () => RunCanExecute)); }
        }
        /// <summary>
        /// Run command execution flag.
        /// </summary>
        /// <value>Determines if the <see cref="Run"/> command can execute in its current state.</value>
        public bool RunCanExecute { get { return true; }}

        /// <summary>
        /// Command handler for <see cref="Run"/> command.
        /// </summary>
        private void RunCallback()
        {
            if (!is_any_file_valid()) return;
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

            Status = "";
            foreach (var topic in dataCache.corpus.concDict.Values)
            {
                var enumItems = dataCache.corpus.get_similar_topics(topic, (int)dataCache.Config["similarTopicCount"])
                                    .Select(top => new TopicResultItem()
                                    {
                                        Name = top.topicName,
                                        fileName = top.fileName,
                                        displayName = (bool)dataCache.Config["IsTitleRequested"] ? top.topicName : top.fileName,
                                        sourceName = Path.GetFileNameWithoutExtension(top.topicName) + ".xml",
                                        Path = top.path,
                                        simScore = (float)dataCache.corpus.get_similarity_score(topic, top),
                                        words = dataCache.corpus.get_similar_words(topic, top, 100),
                                        Keywords = dataCache.corpus.get_similar_words(topic, top, (int)dataCache.Config["keywordCount"]),
                                        NumOfInlineLinks = top.xrefs.Count,
                                        NumOfRelatedLinks = top.relinks.Count,
                                        IsLinked = topic.get_all_link_names().Contains(top.fileName) == true ? "Visible" : "Collapsed"
                                    });
                var itemColl = new ObservableCollection<TopicResultItem>();
                foreach (var item in enumItems)
                {
                    itemColl.Add(item);
                }
                App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        TopicCollection.Add(new TopicItem(topic.path, "Visible",itemColl));
                    });
            }
            ProcessingMessageVisibility = "Collapsed";
            ProgressBarVisibility = "Collapsed";
            _dummyTopicCollection = TopicCollection;
            dataCache.TopicCollection = TopicCollection;
            ResultsVisibility = "Visible"; // display the results
            SelectedTopicIndex = 0;
            dataCache.ViewState.Add("SummaryViewEnabled");
            dataCache.ViewState.Add("ThemesViewEnabled");
        }
        #endregion



        // View Management Methods
        //#region ExpandAll Command CallBack
        //private ICommand _expandAll;
        ///// <summary>
        ///// This ICommand binds to the ExpandAll button in the ResultsPage. All topic expanders are expanded.
        ///// </summary>
        //public ICommand ExpandAll
        //{
        //    get { return _expandAll ?? (_expandAll = new ButtonCommandHandler(() => ExpandAllCallback(), () => ExpandAllCanExecute)); }
        //}

        //private void ExpandAllCallback()
        //{
        //    Parallel.ForEach(TopicCollection, (topItem) =>
        //    {
        //        topItem.IsExpanded = true;
        //    });
        //}
        //public bool ExpandAllCanExecute { get { return true; } }
        //#endregion

        //#region CollapseAll Command CallBack
        ///// <summary>
        ///// This ICommand binds to the CollapseAll button on the ResultsPage. It collapses all the topic expanders at once.
        ///// </summary>
        //private ICommand _collapseAll;
        //public ICommand CollapseAll
        //{
        //    get { return _collapseAll ?? (_collapseAll = new ButtonCommandHandler(() => CollapseAllCallBack(), () => CollapseAllCanExecute)); }
        //}

        //private void CollapseAllCallBack()
        //{
        //    foreach (var topItem in TopicCollection)
        //    {
        //        topItem.IsExpanded = false;
        //    }
        //}
        //public bool CollapseAllCanExecute { get { return true; } }
        //#endregion

        #region Search Command CallBack
        private string _searchText = string.Empty;
        /// <summary>
        /// Search text.
        /// </summary>
        /// <value>Represents the string entered by the user for search a topic in the <c>Topics List</c> section of the results view.</value>
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; RaisePropertyChanged(); }
        }

        private ICommand _search;
        /// <summary>
        /// Topic search command.
        /// </summary>
        /// <value>Command to update the <see cref="TopicCollection"/> depending on the <see cref="SearchText"/>.</value>
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
                if (!topItem.displayName.ToLower().Contains(SearchText.ToLower()))
                {
                    topItem.IsVisible = "Collapsed";
                }
                else
                {
                    topItem.IsVisible = "Visible";
                }
            }
        }
        /// <summary>
        /// Search command execution flag.
        /// </summary>
        /// <value>Determines if the <see cref="Search"/> command can execute in its current state.</value>
        public bool SearchCanExecute { get { return true; } }
        #endregion

        #region Open Similar Topic Command CallBack
        private ICommand _openSimilarFile;
        /// <summary>
        /// Command to open topic.
        /// </summary>
        /// <value>Command to open the corresponding <c>HTML</c> page of the topic in the default browser.</value>
        public ICommand OpenSimilarFile
        {
            get { return _openSimilarFile ?? (_openSimilarFile = new ParameterCommandHandler((grid) => open_similar_file(grid), () => { return true; })); }
        }

        /// <summary>
        /// Opens the similar topic corresponding the selected row of the <paramref name="gridObj"/> datagrid.
        /// </summary>
        /// <param name="gridObj">The datagrid of the table displayed in the <c>Similar Topics</c> section of the results view.</param>
        public void open_similar_file(object gridObj)
        {
            DataGrid grid = (DataGrid)gridObj;
            TopicResultItem item = (TopicResultItem)grid.SelectedItem;
            launch(item.Path);
            //System.Diagnostics.Process.Start($"{item.Path}");
        }
        #endregion

        #region Open Topic Command CallBack
        private ICommand _openFile;
        /// <summary>
        /// Command that opens a topic.
        /// </summary>
        /// <value>Command that handles the opening of the a topic from a give path.</value>
        public ICommand OpenFile
        {
            get { return _openFile ?? (_openFile = new ParameterCommandHandler((path) => open_file(path), () => { return true; })); }
        }

        /// <summary>
        /// Opens the file from the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path of the file to be launched.</param>
        public void open_file(object path)
        {
            launch((string)path);
        }
        #endregion

        #region DragDrop Command
        private ICommand _dragDrop;
        /// <summary>
        /// Command for handling drag-drop.
        /// </summary>
        /// <value>Command that handles the drag and drop of topics as links, 
        /// when done from the names of similar topic suggestions in the <c>Similar Topics</c> section of the results view.</value>
        public ICommand DragDropCommand
        {
            get { return _dragDrop ?? (_dragDrop = new ParameterCommandHandler((grid) => drag_link_to_file(grid), () => { return true; })); }
        }

        /// <summary>
        /// Handles drag and drop of a topic represented by the selected row in the <paramref name="gridObj"/> datagrid.
        /// </summary>
        /// <param name="gridObj">The datagrid of the table displayed in the <c>Similar Topics</c> section.</param>
        public void drag_link_to_file(object gridObj)
        {
            DataGrid grid = (DataGrid)gridObj;
            TopicResultItem item = (TopicResultItem)grid.SelectedItem;
            
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    try
                    {
                        DragDrop.DoDragDrop(new FrameworkElement(), item.Path.ToString(), DragDropEffects.Copy);
                    } catch (NullReferenceException)
                    {
                        // Do nothing if item is not selected
                    }
                }
                else
                {
                    var href = $"<link href=\"{item.sourceName}\"></link>";
                    DragDrop.DoDragDrop(new FrameworkElement(), href, DragDropEffects.Copy);
                }
            }
        }

        #endregion

        #region Launch file
        /// <summary>
        /// Opens the file referenced by the <paramref name="path"/> parameter.
        /// </summary>
        /// <param name="path">The path of the file to be launched.</param>
        public void launch(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo($"{path}") { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", $"{path}");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", $"{path}");
            }
            else
            {
                // throw 
            }
        }
        #endregion

        #region Naming validity check and prompt
        /// <summary>
        /// Checks if at least two files are valid.
        /// </summary>
        /// <value>Raises an alert if at least two files do not pass the naming guidelines configured in the settings.</value>
        /// <returns>True if at least two files pass the validation.</returns>
        public bool is_any_file_valid()
        {
            int nValidFiles = Corpus.Files.Where(file => Topic.Is_valid(file.Name, dataCache.corpus.ignoreData)).Count();
            
            if(nValidFiles <= 1)
            {
                string message = "At least two files must comply with the naming rules to compute similarity. Re-configure topic name settings.";
                string title = "Topic name check failed !";
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);

                return false;
            }

            return true;

        }
        #endregion

        // Config Methods
        //#region Update Keywords Method
        //private ICommand _updateKeywords;
        ///// <summary>
        ///// This method updates the keywords in the TopicResultItem of the ResultsPage based on the Keyword count config setting.
        ///// </summary>
        //public ICommand UpdateKeywords
        //{
        //    get { return _updateKeywords ?? (_updateKeywords = new ButtonCommandHandler(() => update_similar_words(), () => { return true; })); }
        //}

        //private void update_similar_words()
        //{
        //    foreach(var topicItem in TopicCollection)
        //    {
        //        if (topicItem.IsVisible == "Visible")
        //        {
        //           foreach(var resultItem in topicItem.itemCollection)
        //            {
        //                var enumKeywords = resultItem.words.Take((int)dataCache.Config["keywordCount"]);
        //                resultItem.Keywords.Clear();
        //                foreach(var keyword in enumKeywords)
        //                {
        //                    resultItem.Keywords.Add(keyword);
        //                }
        //            }
        //        }
        //    }
        //}
        //#endregion

    }
}
