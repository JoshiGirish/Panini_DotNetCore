using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    /// <summary>
    /// Model for displaying the summary of links in the <c>Summary View</c>.
    /// </summary>
    public class LinksInfo : BaseModel
    {
        #region Number of Exiting Related Links
        private int _numOfExistingRelLinks;
        /// <summary>
        /// Total number of existing links in all valid topics.
        /// </summary>
        /// <value>Total number of existing links in all valid topics.</value>
        public int NumOfExistingRelLinks
        {
            get { return _numOfExistingRelLinks; }
            set { _numOfExistingRelLinks = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Number of Existing Inline Links
        private int _numOfExistingInlineLinks;
        /// <summary>
        /// Total number of inline links from all the valid topics.
        /// </summary>
        /// <value>Total number of inline links from all the valid topics.</value>
        public int NumOfExistingInlineLinks
        {
            get { return _numOfExistingInlineLinks; }
            set { _numOfExistingInlineLinks = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Number of Proposed Links
        private int _numOfProposedLinks;
        /// <summary>
        /// Total number of links proposed by the NLP system.
        /// </summary>
        /// <value>Total number of links proposed by the NLP system.</value>
        public int NumOfProposedLinks
        {
            get { return _numOfProposedLinks; }
            set { _numOfProposedLinks = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Number of Existing Links
        private int _numOfExistingLinks;
        /// <summary>
        /// Total number of existing links from all the topics (related links +  inline links).
        /// </summary>
        /// <value>Total number of existing links from all the topics (related links +  inline links).</value>
        public int NumOfExistingLinks
        {
            get { return _numOfExistingLinks; }
            set { _numOfExistingLinks = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Number of Matching Links
        
        private int _numOfMatchingLinks;
        /// <summary>
        /// Total number of matching (common) links, that is, the links that are both proposed by the system and also exist in the topics.
        /// </summary>
        /// <value>Total number of matching (common) links, that is, the links that are both proposed by the system and also exist in the topics.</value>
        public int NumOfMatchingLinks
        {
            get { return _numOfMatchingLinks; }
            set { _numOfMatchingLinks = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Number of Links to be Integrated
        
        private int _numOfLinksNeedsIntegration;
        /// <summary>
        /// Total number of links that need to be integrated in the topics.
        /// </summary>
        /// <value>Total number of links that need to be integrated in the topics.</value>
        public int NumOfLinksNeedsIntegration
        {
            get { return _numOfLinksNeedsIntegration; }
            set { _numOfLinksNeedsIntegration = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Number of Links to be Removed
        
        private int _numOfObseleteLinks;
        /// <summary>
        /// Total number of links that need to be replaced.
        /// </summary>
        /// <value>Total number of links that need to be replaced.</value>
        public int NumOfObsoleteLinks
        {
            get { return _numOfObseleteLinks; }
            set { _numOfObseleteLinks = value; RaisePropertyChanged(); }
        }

        #endregion

        #region IsExpanded Flag
        
        private bool _isExpanded;
        /// <summary>
        /// Binds to the IsExpanded property of the expander in the Summary Page.
        /// </summary>
        /// <value>Binds to the IsExpanded property of the expander in the Summary Page.</value>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; RaisePropertyChanged(); }
        }

        #endregion

        #region IsVisible Property
        private string _isVisible = "Collapsed";
        /// <summary>
        /// Controls the visibility of the expander which displays the links summary.
        /// </summary>
        /// <value>Controls the visibility of the expander which displays the links summary.</value>
        public string IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; RaisePropertyChanged(); }
        }
        #endregion
    }
}
