using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    public class LinksInfo : BaseModel
    {
        #region Number of Exiting Related Links
        /// <summary>
        /// This property stores the total number of existing links in all valid topics.
        /// </summary>
        private int _numOfExistingRelLinks;

        public int NumOfExistingRelLinks
        {
            get { return _numOfExistingRelLinks; }
            set { _numOfExistingRelLinks = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Number of Existing Inline Links
        /// <summary>
        /// This property stores the total number of inline links from all the valid topics.
        /// </summary>
        private int _numOfExistingInlineLinks;

        public int NumOfExistingInlineLinks
        {
            get { return _numOfExistingInlineLinks; }
            set { _numOfExistingInlineLinks = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Number of Proposed Links
        /// <summary>
        /// This property stores the total number of links proposed by the NLP system.
        /// </summary>
        private int _numOfProposedLinks;

        public int NumOfProposedLinks
        {
            get { return _numOfProposedLinks; }
            set { _numOfProposedLinks = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Number of Existing Links
        /// <summary>
        /// This property stores the total number of existing links from all the topics (related links +  inline links).
        /// </summary>
        private int _numOfExistingLinks;

        public int NumOfExistingLinks
        {
            get { return _numOfExistingLinks; }
            set { _numOfExistingLinks = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Number of Matching Links
        /// <summary>
        /// This property stores the total number of matching (common) links, that is, the links that are both proposed by the system and also exist in the topics.
        /// </summary>
        private int _numOfMatchingLinks;

        public int NumOfMatchingLinks
        {
            get { return _numOfMatchingLinks; }
            set { _numOfMatchingLinks = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Number of Links to be Integrated
        /// <summary>
        /// This property stores the total number of links that need to be integrated in the topics.
        /// </summary>
        private int _numOfLinksNeedsIntegration;

        public int NumOfLinksNeedsIntegration
        {
            get { return _numOfLinksNeedsIntegration; }
            set { _numOfLinksNeedsIntegration = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Number of Links to be Removed
        /// <summary>
        /// This property stores the total number of links that need to be replaced.
        /// </summary>
        private int _numOfObseleteLinks;

        public int NumOfObsoleteLinks
        {
            get { return _numOfObseleteLinks; }
            set { _numOfObseleteLinks = value; RaisePropertyChanged(); }
        }

        #endregion

        #region IsExpanded Flag
        /// <summary>
        /// This property binds to the IsExpanded property of the expandeder in the SummaryPage.
        /// </summary>
        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; RaisePropertyChanged(); }
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
