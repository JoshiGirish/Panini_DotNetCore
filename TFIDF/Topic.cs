﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

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
        #endregion

        #region Constructor
        public Topic(string name, string filePath,  HtmlDocument html, Dictionary<string, List<string>> ignoreData)
        {
            topicName = name;
            path = filePath;
            text = get_topic_text(html);
            words = get_all_words();
            sentCount = CountTokenizedSentences(get_topic_text(html));
            var links = get_existing_links(html);
            xrefs = links["xrefs"];
            relinks = links["relinks"];
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
            string[] labels = { "See Also", "In Other Guides" };

            // Find the related links
            var td_tags = doc.DocumentNode.Descendants("td");
            foreach(var tag in td_tags)
            {
                if (labels.Contains(tag.InnerText))
                {
                    var table_tag = tag.ParentNode.ParentNode;
                    var anchor_tags = table_tag.Descendants("a");
                    foreach(var a_tag in anchor_tags)
                    {
                        var hRef = a_tag.GetAttributeValue("href", "default");
                        if(hRef == string.Empty)
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
            }

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

    }
}
