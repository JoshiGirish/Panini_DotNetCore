using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Panini.Commands;
using TFIDF;
using Panini.Models;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Windows.Threading;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Panini.ViewModel
{
    /// <summary>
    /// Manages the behavior of <c>Load View</c>.
    /// </summary>
    public class LoadViewModel : BaseViewModel
    {
        #region Property Declaration
        /// <summary>
        /// Single instance of <see cref="DataCache"/>.
        /// </summary>
        /// <value>Used for sharing resources between view models.</value>
        private readonly DataCache dataCache = DataCache.Instance;

        private ObservableCollection<string> _listOfFiles;
        /// <summary>
        /// A collection of the files in the directory. This property binds to the <c>ItemsSource</c> attribute of <c>filesList</c> list box in the laod view.
        /// </summary>
        /// <value>The names of the files read from the the selected directory are stored in this list.</value>
        public ObservableCollection<string> ListOfFiles
        {
            get { return _listOfFiles; }
            set { _listOfFiles = value; }
        }

        private int NumOfFiles = 0;

        private string _fileCountField;

        /// <summary>
        /// File count. This property binds to the text of <c>fileCountField</c> text block in the load view.
        /// </summary>
        /// <value>Message containing the number of files found in the selected directory.</value>
        public string FileCountField
        {
            get { return _fileCountField; }
            set { _fileCountField = value; RaisePropertyChanged(); }
        }


        private static ObservableCollection<TopicRow> _topicList;
        /// <summary>
        /// A collection of <see cref="TopicRow"/> instances.
        /// </summary>
        /// <value>Contains the <see cref="TopicRow"/> instance for each topic read from the directory.</value>
        public static ObservableCollection<TopicRow> TopicList
        {
            get { return _topicList; }
            set
            {
                _topicList = value;
            }
        }

        public string _status;
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
        /// Status color. This property binds to <c>statusColorBrush</c> in the laod view.
        /// </summary>
        /// <value>Color of the status bar.</value>
        public Color StatusBarColor
        {
            get { return _statusBarColor; }
            set { 
                    _statusBarColor = value; 
                    RaisePropertyChanged(); 
                }
        }

        private string directory { get; set; }

        private static string _path;
        /// <summary>
        /// Path of the directory. This property binds to the text of <c>dirPath</c> text box in the load view.
        /// </summary>
        public string DirectoryPath
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Browse Command CallBack
        private ICommand _selectDirectory;
        /// <summary>
        /// Selects a folder from local storage. This ICommand binds to <c>browseBtn</c> in the laod view. 
        /// </summary>
        public ICommand SelectDirectory
        {
            get
            {
                return _selectDirectory ?? (_selectDirectory = new ButtonCommandHandler(() => Browse(), () => {return true;}));
            }
        }
        /// <summary>
        /// Command handler for <see cref="SelectDirectory"/>.
        /// </summary>
        private void Browse()
        {

            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Select Directory";
            dlg.IsFolderPicker = true;
            
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DirectoryPath = dlg.FileName;  //selected folder path
                dataCache.DirPath = DirectoryPath;
            }
        }
        #endregion

        #region Load Topics Command CallBack
       private ICommand _loadTopics;
        /// <summary>
        /// Parses the <c>HTML</c> files. This <c>ICommand</c> binds to the <c>loadBtn</c> button in the load view. 
        /// </summary>
        public ICommand LoadTopics
        {
            get
            {
                return _loadTopics ?? (_loadTopics = new ButtonCommandHandler(() => Load(), () => { return true; }));
            }
        }

        /// <summary>
        /// Command handler for <see cref="LoadTopics"/>. This method launches a new thread for reading the web-topics from the directory.
        /// </summary>
        private void Load()
        {
            ListOfFiles.Clear();
            if (DirectoryPath==null)
            {
                // Display a warning if the directory path textbox is empty
                MessageBoxResult result = System.Windows.MessageBox.Show("Select a valid directory.", "Invalid Directory !", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Thread createThread = new Thread(new ThreadStart(task));
            createThread.IsBackground = true;
            createThread.Start();
        }
        /// <summary>
        /// Thread task that reads the files from the directory.
        /// </summary>
        private void task()
        {
            dataCache.corpus = new Corpus(DirectoryPath, (int)dataCache.Config["maxVocabSize"]);
            IEnumerable<string> names = dataCache.corpus.read_files(DirectoryPath);
            int fileCount = 0;
            foreach (var name in names)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ListOfFiles.Add(name);
                });
                fileCount++;
            }
            if(fileCount == 0)
            {
                Status = $"No files found in the selected directory. Make sure you have .htm or .html files in the directory.";
                StatusBarColor = StatusColors["Error"];
            }
            else
            {
                //Status = $"{fileCount} files found in the selected directory.";
                NumOfFiles = fileCount;
                FileCountField = $"File Count : {fileCount}";
                //StatusBarColor = StatusColors["Success"];
                dataCache.ViewState.Add("ConfigViewEnabled");
                dataCache.ViewState.Add("ResultsViewEnabled");
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the load view.
        /// </summary>
        public LoadViewModel()
        {
            StatusBarColor = StatusColors["Default"];
            TopicList = new ObservableCollection<TopicRow>();
            ListOfFiles = new ObservableCollection<string>();
            ListOfFiles.CollectionChanged += initialize_datacache;
        }
        #endregion

        #region Initialize DataCache Properties
        /// <summary>
        /// Initializes the properties of the singleton <see cref="DataCache"/> Instance.
        /// </summary>
        private void initialize_datacache(object sender, NotifyCollectionChangedEventArgs e)
        {
            dataCache.ListOfFiles = ListOfFiles.ToList();
        }
        #endregion

    }
}
