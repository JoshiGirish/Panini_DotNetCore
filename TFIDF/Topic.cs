using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace TFIDF
{
    /// <summary>
    /// The class that is intantiated for each web-topic in the corpus.
    /// </summary>
    public class Topic : BaseModel
    {

        #region Property Declarations
        /// <summary>
        /// Title of the topic.
        /// </summary>
        /// <value>The base name of the topic without the extension.</value>
        public string topicName { get; set; }

        /// <summary>
        /// Name of the topic/file.
        /// </summary>
        /// <value>The name of the file associated with the topic.</value>
        public string fileName { get; set; }

        /// <summary>
        /// Text content of the topic.
        /// </summary>
        /// <value>The text extracted from the web-topic which the <c>Topic</c> instance represents.</value>
        private string text { get; set; }

        /// <summary>
        /// Number of sentences in the topic.
        /// </summary>
        /// <value>The number of sentences extracted from the topic <see cref="text"/>.</value>
        public int sentCount { get; set; }

        /// <summary>
        /// URIs that the inline links (<see cref="xrefs"/>) point to.
        /// </summary>
        /// <value>The list of <c>href</c> attribute values of inline links in the topic. 
        /// Inline links usually appear in the body of the topic, inline with the description.
        /// <para>The DITA specification specifies the use of <c>xref</c> markup element for generating inline links.</para></value>
        public List<string> xrefs { get; set; } = new List<string>();

        /// <summary>
        /// Inline link tags in the topic.
        /// </summary>
        /// <value>The list of inline hyperlinks extracted from the body of the topic.</value>
        public List<string> xrefTags { get; set; } = new List<string>();

        /// <summary>
        /// URIs that the related links (<see cref="relinks"/>) point to.
        /// </summary>
        /// <value>The list of <c>href</c> attribute values of related links in the topic. 
        /// Related links usually appear in separate section after the body of the topic.
        /// <para>The DITA specification specifies the use of <c>related-links</c> markup element for generating related links.</para></value>
        public List<string> relinks { get; set; } = new List<string>();

        /// <summary>
        /// Related link tags in the topic.
        /// </summary>
        /// <value>The list of the related links extracted from a separate section, usually highlighted as <c>See Also</c> or <c>Related Topics</c>.</value>
        public List<string> relinkTags { get; set; } = new List<string>();

        /// <summary>
        /// Words in the topic.
        /// </summary>
        /// <value>The list of all the words in the topic, including multiple occurances.</value>
        public List<string> words = new List<string>();

        /// <summary>
        /// Path of the topic.
        /// </summary>
        /// <value>The path of the topic.</value>
        public string path { get; } = "";

        /// <summary>
        /// <c>TFIDF</c> instance associated with the topic.
        /// </summary>
        /// <value>The <c>TFIDF</c> instance which stores all information about TFIDF analysis related to the topic. </value>
        public TFIDF tfidf { get; set; }
        //public enum SelectionMode
        //{
        //    InnerText,
        //    CSSSelector
        //}

        /// <summary>
        /// The selection mode of the related links container.
        /// </summary>
        /// <value>Represents the mode of selection, for extracting the parent container tag which includes are the anchor tags of the related links.</value>
        public string Mode { get; set; }

        /// <summary>
        /// The comma separated inner texts of the related links containers.
        /// </summary>
        /// <value>Represents a comma separated string of the inner texts of the parent container of the related links.
        /// <para>Usually web-topics contain related links under a section named <c>See Also</c> or <c>Related Topics</c>. 
        /// These section headings are passed using this <c>InnerText</c> field.</para></value>
        public string InnerText { get; set; } = ""; // comma separated string

        /// <summary>
        /// The CSS selector of the parent of the related links.
        /// </summary>
        /// <value>Represents the CSS Selector of the related link tag.</value>
        public string CSSSelector { get; set; } = "";

        /// <summary>
        /// The ancestor level of the selected tag.
        /// </summary>
        /// <value>Sometimes the tag of the section headings for the related links is not their parent tag, but siblings. 
        /// In such cases, the <c>AncestorLevel</c> field lets you specify which level of parent must be selected that will encapsulate all related links.</value>
        public int AncestorLevel { get; set; } = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiates <see cref="Topic"/> class for a web-topic.
        /// </summary>
        /// <param name="name">Base name of the topic without its extension.</param>
        /// <param name="filePath">Path of the topic.</param>
        /// <param name="html"><c>HTML</c> document that represents the topic.</param>
        /// <param name="ignoreData">Configuration settings for invalidating topics.</param>
        /// <param name="selectionOptions">Selection options for the parent container of related links.</param>
        /// <param name="level">Ancestor level of the parent tag which encapsulates all related links.</param>
        public Topic(string name, string filePath,  HtmlDocument html, Dictionary<string, List<string>> ignoreData, Dictionary<string, string> selectionOptions, int level, string titleTag)
        {
            topicName = get_topic_title(html, titleTag);
            fileName = name;
            path = filePath;
            text = get_topic_text(html);
            words = get_all_words();
            sentCount = CountTokenizedSentences(get_topic_text(html));
            Mode = selectionOptions["Mode"];
            InnerText = selectionOptions["InnerText"];
            CSSSelector = selectionOptions["CSSSelector"];
            AncestorLevel = level;
            get_existing_links(html);
        }
        #endregion

        #region Check Topic Validity
        /// <summary>
        /// Checks the validity of a file from its name.
        /// </summary>
        /// <param name="filename">Topic name to be checked for validity.</param>
        /// <param name="ignoreData">Data dictionary containing configured naming conventions.</param>
        /// <returns><c>true</c> if the filename complies with the configured naming conventions, else <c>false</c>.</returns>
        public static bool Is_valid(string filename, Dictionary<string, List<string>> ignoreData)
        {
            var name = Path.GetFileNameWithoutExtension(filename);
            foreach(var term in ignoreData["end"])
            {
                if (name.EndsWith(term) && term != "") return false;
            }
            foreach(var term in ignoreData["start"])
            {
                if (name.StartsWith(term) && term != "") return false;
            }
            foreach(var term in ignoreData["contains"])
            {
                if (name.Contains(term) && term != "") return false;
            }
            //if (ignoreData["name"].Contains(name)) return false;
            return true;
        }
        #endregion

        #region Get Topic Title
        /// <summary>
        /// Extracts the title of the topic (first occurance of the title tag e.g. h1, h2, etc)
        /// </summary>
        /// <param name="doc">The <c>HTML</c> document of the topic.</param>
        /// <param name="titleTag">HTML tag of the topic title</param>
        /// <returns></returns>
        public string get_topic_title(HtmlDocument doc, string titleTag)
        {
            IEnumerable<HtmlNode> tags = doc.DocumentNode.Descendants(titleTag);
            string title;
            int count = 0;
            try
            {
                title = HttpUtility.HtmlDecode(tags.First().InnerText);
            }
            catch (System.InvalidOperationException)
            {

                title = $"Topic_{count}";
                count++;
            }
            return title;
        }
        #endregion

        #region Get Topic Text
        /// <summary>
        /// Extracts the text content from a given <c>HTML</c> document.
        /// </summary>
        /// <param name="doc">The <c>HTML</c> document whose text is requested.</param>
        /// <returns>Topic text in the form of a string.</returns>
        public string get_topic_text(HtmlDocument doc)
        {
            var body = doc.DocumentNode.SelectSingleNode("//body");
            string text = string.Join(" ", doc.DocumentNode.Descendants()
                .Where(n => !n.HasChildNodes && !string.IsNullOrWhiteSpace(n.InnerText))
                .Select(n => n.InnerText));            
            return text;
        }
        #endregion

        #region Get Existing Links
        /// <summary>
        /// Extracts hyperlink data (inline links and related links) from the given <c>HTML</c> document.
        /// </summary>
        /// <param name="doc">The document from which the links are to be extracted.</param>
        /// <returns>A dictionary containing the inline links (<see cref="xrefs"/>) and related links (<see cref="relinks"/>) in the topic.</returns>
        public Dictionary<string, List<string>> get_existing_links(HtmlDocument doc)
        {
            // Find the related links
            get_related_links(doc);

            // Find the inline links
            var all_a_tags = doc.DocumentNode.Descendants("a");
            foreach(var a_tag in all_a_tags)
            {
                var a_link = Path.GetFileName(a_tag.GetAttributeValue("href", "No_href").Split('#')[0]);
                if (!relinks.Contains(a_link) && a_link != "No_href") 
                {
                    var hRef = a_tag.GetAttributeValue("href", "No_href");
                    if(hRef == string.Empty)
                    {
                        xrefTags.Add(topicName);
                    }
                    else { xrefTags.Add(hRef);}
                    xrefs.Add(a_link);
                }
            }

            Dictionary<string, List<string>> links = new Dictionary<string, List<string>>();
            links.Add("xrefs", xrefs.clean_links());
            links.Add("relinks", relinks.clean_links());
            return links;
        }
        #endregion

        #region Get Related Links
        /// <summary>
        /// Extracts all the related links from the <c>HTML</c> document (<paramref name="doc"/>).
        /// </summary>
        /// <param name="doc">The <c>HTML</c> document.</param>
        public void get_related_links(HtmlDocument doc)
        {
            HtmlNode parentTag = null;
            switch (Mode){
                case "cssSelection":
                    parentTag = find_parent_tag_using_selector();
                    break;

                case "innerText":
                    parentTag = find_parent_tag_using_innertext();
                    break;

                default:
                    break;
            }

            if(parentTag != null) 
            { 
                foreach (var a_tag in parentTag.Descendants("a"))
                {
                    var hRef = a_tag.GetAttributeValue("href", "default");
                    if (hRef == string.Empty)
                    {
                        relinkTags.Add(topicName);
                    }
                    else
                    {
                        relinkTags.Add(hRef);
                    }
                    relinks.Add(Path.GetFileName(a_tag.GetAttributeValue("href", "default").Split('#')[0]));
                }
            }



            // Find the parent by CSS Selector
            HtmlNode find_parent_tag_using_selector()
            {
                //var td_tags = doc.DocumentNode.Descendants("td");
                var tags = doc.DocumentNode.Descendants();
                IList<HtmlNode> nodes = doc.QuerySelectorAll(CSSSelector);
                try
                {
                    if (nodes.Count != 0)
                    {
                        return AncestorLevel == 0 ? nodes[0] : get_nth_parent(nodes[0], AncestorLevel);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (NullReferenceException)
                {
                    return null;
                }
                
            }

            // Find the parent by InnerText
            HtmlNode find_parent_tag_using_innertext()
            {
                HtmlNode parent = null;
                string[] labels = InnerText.Split(',').ToArray();
                //var td_tags = doc.DocumentNode.Descendants("td");
                var tags = doc.DocumentNode.Descendants();
                foreach (var tag in tags)
                {
                    if (labels.Contains(tag.GetDirectInnerText()) && tag.GetType() == typeof(HtmlNode))
                    {
                        return AncestorLevel == 0 ? tag : get_nth_parent(tag, AncestorLevel);
                    }
                }
                return parent;

            }
            
        }
        #endregion

        #region Get All Link Names
        /// <summary>
        /// Returns a list of names of all the links in the topic.
        /// </summary>
        /// <returns>A combined list of names of inline and related links.</returns>
        public List<string> get_all_link_names()
        {
            List<string> names = new List<string>();
            names.AddRange(xrefs);
            names.AddRange(relinks);
            return names;
        }
        #endregion

        #region Get All Words
        /// <summary>
        /// Extracts all valid words from the topic text.
        /// </summary>
        /// <returns>List of valid words after removing punctuation, symbols, and numbers from the topic text.</returns>
        public List<string> get_all_words()
        {
            var data = new string(text.Where(c => !char.IsPunctuation(c) && !char.IsSymbol(c)).ToArray()); // remove punctuation and symbols
            data = Regex.Replace(data, @"[^\u0000-\u007F]+", string.Empty); 
            var validWords = data.tokenize()?.lemmatize();
            if(validWords != null)
            {
                return validWords.Where(word => !int.TryParse(word, out _) && word.Length>1).Select(n => n).ToList(); // return words which are not numbers
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region Get All Sentences
        /// <summary>
        /// Extracts all sentences from the topic text.
        /// </summary>
        /// <param name="text">Text to be tokenized into sentences.</param>
        /// <returns>Sentences from the topic text.</returns>
        private int CountTokenizedSentences(string text)
        {
            // remove spaces and split the , . : ; etc..
            return text.Split(new string[] { " ", "   ", "\r\n" }, StringSplitOptions.None)
                .SelectMany(o => o.SplitAndKeep(".,;:\\/?!#$%()=+-*\"'–_`<>&^@{}[]|~'".ToArray())).Count();
                //.Select(o => o.ToLower());
        }
        #endregion

        #region Get nth Parent Node
        /// <summary>
        /// Finds the <c>n</c>th <paramref name="level"/> parent of selected tag (<paramref name="selectedTag"/>). 
        /// </summary>
        /// <param name="selectedTag">The tag selected from the <c>HTML</c> document.</param>
        /// <param name="level">Ancestor level</param>
        /// <returns></returns>
        public HtmlNode get_nth_parent(HtmlNode selectedTag, int level)
        {
            HtmlNode parent = selectedTag;
            for (int i = 0; i < level; i++)
            {
                parent = parent.ParentNode;
            }
            return parent;
        }
        #endregion

    }
}
