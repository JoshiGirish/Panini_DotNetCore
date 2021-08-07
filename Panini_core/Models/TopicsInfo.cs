using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    public class TopicsInfo : BaseModel
    {
        #region Path property

        /// <summary>
        /// Define the root path of the published topics
        /// </summary>
        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Total Topics property

        /// <summary>
        /// The total number of topics found in the the root path
        /// </summary>
        private int _numTopics = 0;

        public int NumTopics
        {
            get { return _numTopics; }
            set { _numTopics = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Valid Topics property
        /// <summary>
        /// Number of valid topics considered for TF-IDF analysis
        /// </summary>
        private int _numOfValidTopics = 0;

        public int NumOfValidTopics
        {
            get { return _numOfValidTopics; }
            set { _numOfValidTopics = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Ignored Topics property
        /// <summary>
        /// Number of Ignored topics based on the settings in the Config tab
        /// </summary>
        private int _numOfIgnoredTopics = 0;

        public int NumOfIgnoredTopics
        {
            get { return _numOfIgnoredTopics; }
            set { _numOfIgnoredTopics = value; RaisePropertyChanged(); }
        }

        #endregion

        #region IsExpanded Property
        private bool _isExpanded = false;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded =  value; RaisePropertyChanged(); }
        }
        #endregion

        #region IsVisible Property
        private string _isVisible = "Collapsed";

        public string IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; RaisePropertyChanged(); }
        }
        #endregion
    }
}
