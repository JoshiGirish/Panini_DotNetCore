using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    /// <summary>
    /// Model for displaying an entry in the table of <c>Similar Topics</c> section in <c>Results View</c>.
    /// </summary>
    public class TopicResultItem : BaseModel
    {
        /// <summary>
        /// Name of the similar topic.
        /// </summary>
        /// <value>Name of the similar topic.</value>
        public string Name { get; set; }
        /// <summary>
        /// Name of the topic/file.
        /// </summary>
        /// <value>The name of the file associated with the topic.</value>
        public string fileName { get; set; }
        /// <summary>
        /// Display name of the topic item.
        /// </summary>
        /// <value>The name to be displayed in the topic item entry (topic title or file name).</value>
        public string displayName { get; set; }
        /// <summary>
        /// Name of the similar topic with <c>XML</c> extension.
        /// </summary>
        /// <value>Name of the similar topic with <c>XML</c> extension.</value>
        public string sourceName { get; set; }
        /// <summary>
        /// Path of the similar topic.
        /// </summary>
        /// <value>Path of the similar topic.</value>
        public string Path { get; set; }
        /// <summary>
        /// Similarity scores of the similar topic.
        /// </summary>
        /// <value>Similarity scores of the similar topic.</value>
        public float simScore { get; set; }
        /// <summary>
        /// Common keywords from the similar topic.
        /// </summary>
        /// <value>Common keywords from the similar topic.
        /// <para>This list contains top <c>100</c> common keywords.</para></value>
        public List<string> words { get; set; }

        private string _isLinked = "Collapsed";

        private int _numOfInlineLinks;
        /// <summary>
        /// Number of inline links in the similar topic.
        /// </summary>
        /// <value>Number of inline links in the similar topic.</value>
        public int NumOfInlineLinks
        {
            get { return _numOfInlineLinks; }
            set { _numOfInlineLinks = value; RaisePropertyChanged(); }
        }

        private int _numOfRelatedLinks;
        /// <summary>
        /// Number of related links in the similar topic.
        /// </summary>
        /// <value>Number of related links in the similar topic.</value>
        public int NumOfRelatedLinks
        {
            get { return _numOfRelatedLinks; }
            set { _numOfRelatedLinks = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// Flag to represent if the similar topic is already linked to the topic.
        /// </summary>
        /// <value>Flag to represent if the similar topic is already linked to the topic.</value>
        public string IsLinked
        {
            get { return _isLinked ; }
            set { _isLinked = value; }
        }

        private List<string> _keywords = new List<string>();
        /// <summary>
        /// Keywords displayed for the similar topic.
        /// </summary>
        /// <value>NKeywords displayed for the similar topic.
        /// <para>The number of keywords is specified by the user in the settings view.</para></value>
        public List<string> Keywords
        {
            get { return _keywords; }
            set { _keywords = value; RaisePropertyChanged(); }
        }
        //public string keywordsString { get { return string.Join("    ", keywords); } set{ } }

        /// <summary>
        /// Instantiates the <see cref="TopicResultItem"/> class for each similar topic recommendation.
        /// </summary>
        public TopicResultItem()
        {
        }
    }
}
