using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ILGPU;
using ILGPU.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Panini.ViewModel
{
    /// <summary>
    /// Manages the behavior of the <c>Settings View</c>.
    /// </summary>
    public class ConfigViewModel : BaseViewModel
    {
        /// <summary>
        /// Single instance of <see cref="DataCache"/>.
        /// </summary>
        /// <value>Used for sharing resources between view models.</value>
        private readonly DataCache dataCache = DataCache.Instance;

        /// <summary>
        /// A list of ending strings to invalidate web-topics.
        /// </summary>
        /// <value>Represents a set of substrings which when matched at the end of the web-topic names, results in the topic being ignored from the TFIDF analysis.</value>
        private List<string> ignoreTopicNameEndsWith = new List<string> { "-r-wn", "-sb", "-c-ov", "-c-AccessToContent", "-r-rg", "-r-ui-ContextMenus", "-ActBar" };

        /// <summary>
        /// A list of starting strings to ivalidate web-topics.
        /// </summary>
        /// <value>Represents a set of substrings which when matched at the beginning of the web-topic names, results in the topic being ignored from the TFIDF analysis.</value>
        private List<string> ignoreTopicNameStartsWith = new List<string> { "help-rc" };

        /// <summary>
        /// A list of strings to invalidate web-topics.
        /// </summary>
        /// <value>Represents a set of substrings which when matched anywhere in the web-topic names, results in the topic being ignored from the TFIDF analysis.</value>
        private List<string> ignoreTopicNameContains = new List<string> { "-ActBar", "-ContexMenus-" };

        /// <summary>
        /// A list of topic names used to invalidate web-topics.
        /// </summary>
        /// <value>Represents a set of substrings which when matched at the end of the web-topic names, results in the topic being ignored from the TFIDF analysis.</value>
        private List<string> ignoreTopicName = new List<string> { "Sitemap" };

        /// <summary>
        /// A list of candidate HTML heading tags for topic title
        /// </summary>
        private List<string> titleTags = new List<string> { "h1", "h2", "h3", "h4", "h5", "h6" };

        public List<string> TitleTags
        {
            get { return titleTags; }
        }


        private string _titleTag = "h1";

        /// <summary>
        /// The HTML tag of the topic title.
        /// </summary>
        /// <value>This property binds to the <c>tagCombo</c> drop-down list in the settings view.</value>
        public string TitleTag
        {
            get { return _titleTag; }
            set
            {
                _titleTag = value; 
                RaisePropertyChanged();
                dataCache.corpus.titleTag = value;
            }
        }

        private bool _isTitleRequested = true;

        /// <summary>
        /// Flag for enabling title extranction.
        /// </summary>
        /// <value>This property binds to the <c>IsChecked</c> property of the <c>activateTitleMode</c> checkbox in the settings view.</value>
        public bool IsTitleRequested
        {
            get { return _isTitleRequested; }
            set { _isTitleRequested = value; dataCache.Config["IsTitleRequested"] = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// A list of ending strings to invalidate web-topics.
        /// </summary>
        /// <value>This property binds the text of the <c>ignoreEndWith</c> text box in the settings view.</value>
        public string IgnoreTopicNameEndsWith 
        { 
            get => string.Join(",", ignoreTopicNameEndsWith); 
            set 
            { 
                ignoreTopicNameEndsWith = value.Replace(" ", string.Empty).Split(',').ToList(); 
                dataCache.corpus.ignoreTopicNameEndsWith = ignoreTopicNameEndsWith; 
                RaisePropertyChanged(); 
            } 
        }

        /// <summary>
        /// A list of strings to invalidate web-topics.
        /// </summary>
        /// <value>This property binds the text in the <c>ignoreContains</c> text box in the settings view.</value>
        public string IgnoreTopicNameContains 
        { 
            get => string.Join(",",ignoreTopicNameContains); 
            set 
            { 
                ignoreTopicNameContains = value.Replace(" ", string.Empty).Split(',').ToList(); 
                dataCache.corpus.ignoreTopicNameContains = ignoreTopicNameContains; 
                RaisePropertyChanged(); 
            } 
        }

        /// <summary>
        /// A list of strings to invalidate web-topics.
        /// </summary>
        public string IgnoreTopicName 
        { 
            get => string.Join(",",ignoreTopicName); 
            set 
            { 
                ignoreTopicName = value.Replace(" ", string.Empty).Split(',').ToList(); 
                dataCache.corpus.ignoreTopicName = ignoreTopicName; 
                RaisePropertyChanged();  
            } 
        }

        /// <summary>
        /// A list of starting strings to ivalidate web-topics.
        /// </summary>
        /// <value>This property binds the text in the <c>ignoreStartWith</c> text box in the settings view.</value>
        public string IgnoreTopicNameStartsWith 
        { 
            get => string.Join(",", ignoreTopicNameStartsWith);  
            set 
            { 
                ignoreTopicNameStartsWith = value.Replace(" ", string.Empty).Split(',').ToList(); 
                dataCache.corpus.ignoreTopicNameStartsWith = ignoreTopicNameStartsWith; 
                RaisePropertyChanged(); 
            } 
        }

        /// <summary>
        /// Color scheme backing field.
        /// </summary>
        /// <value>Backing field for the color scheme of the heatmap.
        /// <para>The default value is <c>Default</c>.</para></value>
        private string _colorScheme = "Default";

        /// <summary>
        /// Accelerators backing field.
        /// </summary>
        /// <value>Backing field for the accelerators proposed for parallel computing.</value>
        private List<string> _accelerators;

        // Hyperlink configuration properties
        private string _selectionMode = "InnerText";

        /// <summary>
        /// Selection mode backing field. This property binds to the selected item of <c>Selectors</c> radio button group in the settings view.
        /// </summary>
        /// <value>Selection mode for the parent container tag which encapsulates are related links.
        /// <list type="bullet">
        ///     <item><term><c>InnerText</c></term>
        ///         <description>Lets you select the tag using the <c>innerText</c> attribute value of the tag.</description>    
        ///     </item>
        ///     <item><term><c>CSS</c></term>
        ///         <description>Lets you select the tag using the <c>CSS Selector</c>.</description>
        ///     </item>
        /// </list>
        /// <para>The default value is <c>InnerText</c>.</para></value>
        public string SelectionMode
        {
            get { return _selectionMode; }
            set { _selectionMode = value; RaisePropertyChanged(); }
        }

        #region Get Parent Checkbox
       private bool _isGetParentTagActive = false;

        /// <summary>
        /// Flag to look for parent element when selecting the container for related links. This property binds to <c>getParentTagCheckbox</c> in the settings view.
        /// </summary>
        /// <value>
        ///     <list type="bullet">
        ///         <item><term><c>true</c></term>
        ///             <description>Looks for a parent tag of the selected tag, as the container for related links.</description>
        ///         </item>
        ///         <item><term><c>false</c></term>
        ///             <description>Considers the selected tag as container for related links.</description>
        ///         </item>
        ///     </list>
        /// </value>
        public bool IsGetParentTagActive
        {
            get { return _isGetParentTagActive; }
            set { _isGetParentTagActive = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Selection Query string
        private string _queryString = "See Also, In Other Guides";

        /// <summary>
        /// String input for passing the innert text or the CSS selector. This property binds to <c>queryTextBox</c> in the settings view.
        /// </summary>
        /// <value>Represents following two scenarios:
        ///     <list type="bullet">
        ///         <item><description>When <see cref="SelectionMode"/> is <c>InnerText</c>, this property expects the headings of the related links section e.g. <c>See Also</c> or <c>Related Topics</c>.</description></item>
        ///         <item><description>When <see cref="SelectionMode"/> is <c>CSS</c>, this property expects the CSS selector of the parent element of the related links e.g. <c>#See_also</c>.</description></item>
        ///     </list>
        /// </value>
        public string QueryString
        {
            get { return _queryString; }
            set { _queryString = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Get Parent Level
        private int _parentLevel;

        /// <summary>
        /// The parent level of the selected tag. This property binds to the text of <c>parentLevel</c> text box in the settings view.
        /// </summary>
        /// <value>The <c>n</c>th level parent of the selected tag to be considered as the container for related links.</value>
        public int ParentLevel
        {
            get { return _parentLevel; }
            set { _parentLevel = value; RaisePropertyChanged(); }
        }
        #endregion

        /// <summary>
        /// List of accelerators devices. This property binds to <c>acceleratorsList</c> list box in the settings view.
        /// </summary>
        /// <value>List of accelerators available for parallel computing e.g. <c>CPU</c> or <c>GPU</c>.</value>
        public List<string> Devices
        {
            get { return _accelerators; }
            set { _accelerators = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Selected accelerator. This property binds to the selected item of <c>acceleratorsList</c> list box in the settings view.
        /// </summary>
        /// <value>The selected accelerator for parallel computing e.g. <c>CPU</c> or <c>GPU</c>.</value>
        public string AccleratorDevice
        {
            set { dataCache.Config["acclerator"] = value; }
        }

        /// <summary>
        /// Color scheme of the heatmap. This property binds to <c>ColorScheme</c> radio button group.
        /// </summary>
        /// <value>Color scheme of the heatmap of similarity scores.
        ///     <list type="bullet">
        ///         <item><term><c>Default</c></term>
        ///             <description>Uses a purple gradient color pallete.</description>
        ///         </item>
        ///         <item><term><c>Hot</c></term>
        ///             <description>Uses a red gradient color pallete.</description>
        ///         </item>
        ///         <item><term><c>Cold</c></term>
        ///             <description>Uses a blue gradient color pallete.</description>
        ///         </item>
        ///         <item><term><c>HotCold</c></term>
        ///             <description>Uses a mixed gradient color pallete of red and blue colors.</description>
        ///         </item>
        ///     </list>
        /// </value>
        public string ColorScheme
        {
            get { return _colorScheme; }
            set { _colorScheme= value; RaisePropertyChanged(); dataCache.Config["colorScheme"] = value; }
        }

        private int _keywordCount = 5;

        /// <summary>
        /// Number of keywords to be displayed in the results. This property binds to <c>keywordCount</c> text box in the settings view.
        /// </summary>
        /// <value>The default value is <c>5</c>.</value>
        public int KeywordCount
        {
            get { return _keywordCount; }
            set { _keywordCount = value; RaisePropertyChanged(); dataCache.Config["keywordCount"] = value; }
        }

        private int _similarTopicCount = 5;

        /// <summary>
        /// Number of similar topic suggestions to be displayed in the results. This property binds to <c>similarTopicCount</c> text box in the settings view.
        /// </summary>
        /// <value>The default value is <c>5</c>.</value>
        public int SimilarTopicCount
        {
            get { return _similarTopicCount; }
            set { _similarTopicCount = value; RaisePropertyChanged(); dataCache.Config["similarTopicCount"] = value; }
        }

        private int _maxVocabSize = 500;

        /// <summary>
        /// Maximum vocabulary size of the lexicon. This property binds to <c>vocabSize</c> text box in the settings view.
        /// </summary>
        /// <value>Represents the maximum number of words allowed to be part of the lexicon. The <c>n</c> most popular words from all topic are extracted and used for TFIDF and Similarity score computation.
        /// <para>Important: The larger the <c>MaxVocabSize</c> the longer will be the TFIDF vectors and the longer it will take to compute the scores.</para></value>
        public int MaxVocabSize
        {
            get { return _maxVocabSize; }
            set { _maxVocabSize = value; RaisePropertyChanged(); dataCache.Config["maxVocabSize"] = value;}
        }

        /// <summary>
        /// Initializes the settings view with default values.
        /// </summary>
        public ConfigViewModel()
        {
            find_accelerators();
            PropertyChanged += update_config;

        }

        /// <summary>
        /// Event handler for updating configuration settings when the <see cref="SelectionMode"/> is changed.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Object containing the event data.</param>
        private void update_config(object sender, PropertyChangedEventArgs e)
        {
            dataCache.corpus.ignoreData = new Dictionary<string, List<string>>(){{"end",  ignoreTopicNameEndsWith},
                                                                {"start", ignoreTopicNameStartsWith},
                                                                {"contains", ignoreTopicNameContains},
                                                                {"name", ignoreTopicName } };
        
            if (SelectionMode == "CSS")
            {
                dataCache.corpus.CSSSelector = QueryString;
                dataCache.corpus.Mode = "cssSelection";
            }
            else
            {
                dataCache.corpus.InnerText = QueryString;
                dataCache.corpus.Mode = "innerText";
            }
            dataCache.corpus.AncestorLevel = ParentLevel;
        }


        #region Get Accelerators
        /// <summary>
        /// Finds the available accelerators for parallel computing of the scores.
        /// </summary>
        public void find_accelerators()
        {
            Devices = new List<string>();
            Context cont = new Context();
            foreach(var acceleratorId in Accelerator.Accelerators)
            {
                using (var accl = Accelerator.Create(cont, acceleratorId))
                {
                    Devices.Add($" [{accl.AcceleratorType}] - {accl.Name}");
                }
            }
        }
        #endregion
    }
}
