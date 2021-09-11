using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    /// <summary>
    /// Model for displaying the summary of topics in <c>Summary View</c>.
    /// </summary>
    public class TopicsInfo : BaseModel
    {
        #region Path property

        
        private string _path;
        /// <summary>
        /// Root path of the web-topics.
        /// </summary>
        /// <value>Root path of the web-topic.</value>
        public string Path
        {
            get { return _path; }
            set { _path = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Total Topics property

        private int _numTopics = 0;
        /// <summary>
        /// Number of topics found at the root path.
        /// </summary>
        /// <value>Number of topics found at the root path.</value>
        public int NumTopics
        {
            get { return _numTopics; }
            set { _numTopics = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Valid Topics property
        private int _numOfValidTopics = 0;
        /// <summary>
        /// Number of valid topics considered for TF-IDF analysis.
        /// </summary>
        /// <value>Number of valid topics considered for TF-IDF analysis.</value>
        public int NumOfValidTopics
        {
            get { return _numOfValidTopics; }
            set { _numOfValidTopics = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Ignored Topics property
        private int _numOfIgnoredTopics = 0;
        /// <summary>
        /// Number of ignored topics based on the settings in the settings view.
        /// </summary>
        /// <value> Number of ignored topics based on the settings in the settings view.</value>
        public int NumOfIgnoredTopics
        {
            get { return _numOfIgnoredTopics; }
            set { _numOfIgnoredTopics = value; RaisePropertyChanged(); }
        }

        #endregion

        #region IsExpanded Property
        private bool _isExpanded = false;
        /// <summary>
        /// Binds to the IsExpanded property of the expander in the Summary Page.
        /// </summary>
        /// <value>Binds to the IsExpanded property of the expander in the Summary Page.</value>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded =  value; RaisePropertyChanged(); }
        }
        #endregion

        #region IsVisible Property
        private string _isVisible = "Collapsed";
        /// <summary>
        /// Controls the visibility of the expander which displays the topics summary.
        /// </summary>
        /// <value>Controls the visibility of the expander which displays the topics summary.</value>
        public string IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; RaisePropertyChanged(); }
        }
        #endregion
    }
}
