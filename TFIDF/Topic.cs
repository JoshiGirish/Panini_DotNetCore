using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace TFIDF
{
    public class Topic : BaseModel
    {
        #region Property Declarations
        public string topicName { get; set; }
        private string text { get; set; }
        public int sentCount { get; set; }
        public List<string> xrefs { get; set; } = new List<string>();
        public List<string> xrefTags { get; set; } = new List<string>();
        public List<string> relinks { get; set; } = new List<string>();
        public List<string> relinkTags { get; set; } = new List<string>();
        public List<string> words = new List<string>();
        private List<double> tfidfVector = new List<double>();
        public string path { get; } = "";
        public TFIDF tfidf { get; set; }
        //public enum SelectionMode
        //{
        //    InnerText,
        //    CSSSelector
        //}
        public string Mode { get; set; }
        public string InnerText { get; set; } = ""; // comma separated string
        public string CSSSelector { get; set; } = "";
        public int AncestorLevel { get; set; } = 0;
        #endregion

        #region Constructor
        public Topic(string name, string filePath,  HtmlDocument html, Dictionary<string, List<string>> ignoreData, Dictionary<string, string> selectionOptions, int level)
        {
            topicName = name;
            path = filePath;
            text = get_topic_text(html);
            words = get_all_words();
            sentCount = CountTokenizedSentences(get_topic_text(html));
            Mode = selectionOptions["Mode"];
            //if ((string)selectionOptions["Mode"] == "cssSelection")
            //{
            //    Mode = SelectionMode.CSSSelector;
            //}
            //else
            //{
            //    Mode = SelectionMode.InnerText;
            //};
            InnerText = selectionOptions["InnerText"];
            CSSSelector = selectionOptions["CSSSelector"];
            AncestorLevel = level;
            get_existing_links(html);
        }
        #endregion

        #region Check Topic Validity
        /// <summary>
        /// Checks the validity of a file from its name.
        /// Returns "true" if the filename complies with the configured naming conventions.
        /// </summary>
        /// <param name="filename">Filename to be checked for validity.</param>
        /// <param name="ignoreData">Data dictionary containing configured naming conventions.</param>
        /// <returns></returns>
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

        #region Get Topic Text
        /// <summary>
        /// Returns the raw text string from a given HTML document.
        /// </summary>
        /// <param name="doc">The HTML document whose text is requested.</param>
        /// <returns></returns>
        public string get_topic_text(HtmlDocument doc)
        {
            var body = doc.DocumentNode.SelectSingleNode("//body");
            string text = string.Join(" ", doc.DocumentNode.Descendants()
                .Where(n => !n.HasChildNodes && !string.IsNullOrWhiteSpace(n.InnerText))
                .Select(n => n.InnerText));
            // Log
            
            return text;
        }
        #endregion

        #region Get Existing Links
        /// <summary>
        /// Returns the inline links and related links from the given HTNL document.
        /// </summary>
        /// <param name="doc">The document from which the links are to be extracted.</param>
        /// <returns></returns>
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
        /// Finds all related links from the HtmlDocument
        /// </summary>
        /// <param name="doc">Source HtmlDocument</param>
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
        /// Return a list of names of all the links in the topic.
        /// </summary>
        /// <returns></returns>
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
        /// Returns valid words from the topic text after removing punctuation, symbols, and numbers.
        /// </summary>
        /// <returns></returns>
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
        /// Returns IEnumerable of sentences from the give text.
        /// </summary>
        /// <param name="text">Text to be tokenized into sentences.</param>
        /// <returns></returns>
        private int CountTokenizedSentences(string text)
        {
            // remove spaces and split the , . : ; etc..
            return text.Split(new string[] { " ", "   ", "\r\n" }, StringSplitOptions.None)
                .SelectMany(o => o.SplitAndKeep(".,;:\\/?!#$%()=+-*\"'–_`<>&^@{}[]|~'".ToArray())).Count();
                //.Select(o => o.ToLower());
        }
        #endregion

        #region Get nth Parent Node
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
