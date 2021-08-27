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
    class LoadViewModel : BaseViewModel
    {
        #region Property Declaration
        private readonly DataCache dataCache = DataCache.Instance;
        private ObservableCollection<string> _listOfFiles;
        public ObservableCollection<string> ListOfFiles
        {
            get { return _listOfFiles; }
            set { _listOfFiles = value; }
        }

        private static ObservableCollection<TopicRow> _topicList;
        public static ObservableCollection<TopicRow> TopicList
        {
            get { return _topicList; }
            set
            {
                _topicList = value;
            }
        }

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
            set { 
                    _statusBarColor = value; 
                    RaisePropertyChanged(); 
                }
        }

        private string directory { get; set; }

        private static string _path;
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
        /// <summary>
        /// This ICommand binds to the browse (directory) button. It lets you select a folder from local storage.
        /// </summary>
        private ICommand _selectDirectory;
        public ICommand SelectDirectory
        {
            get
            {
                return _selectDirectory ?? (_selectDirectory = new ButtonCommandHandler(() => Browse(), () => {return true;}));
            }
        }
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
        /// <summary>
        /// This ICommand binds to the ShowTopics button. It parses the HTML files.
        /// </summary>
        private ICommand _loadTopics;
        public ICommand LoadTopics
        {
            get
            {
                return _loadTopics ?? (_loadTopics = new ButtonCommandHandler(() => Load(), () => { return true; }));
            }
        }

        static ConcurrentQueue<Corpus> cq = new ConcurrentQueue<Corpus>();
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
                Status = $"{fileCount} files found in the selected directory.";
                StatusBarColor = StatusColors["Success"];
                dataCache.ViewState.Add("ConfigViewEnabled");
                dataCache.ViewState.Add("ResultsViewEnabled");
            }
        }
        #endregion

        #region Constructor
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
        /// This method initializes the properties of the singleton DataCache Instance.
        /// </summary>
        private void initialize_datacache(object sender, NotifyCollectionChangedEventArgs e)
        {
            dataCache.ListOfFiles = ListOfFiles.ToList();
        }
        #endregion

    }
}
