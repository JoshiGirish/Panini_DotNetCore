using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    /// <summary>
    /// Model for displaying inline link data in the <c>Inline Links</c> tab of <c>Results View</c>.
    /// </summary>
    public class InlineLinksModel: BaseModel
    {
        /// <summary>
        /// Linked target topic.
        /// </summary>
        /// <value>Name of the linked topic extracted from the <c>href</c> attribute value of the link.</value>
        public string linkedFile { get; set; }

        /// <summary>
        /// Link URI
        /// </summary>
        /// <value>URI extracted from the <c>href</c> attribute of the link.</value>
        public string linkURI { get; set; }

        /// <summary>
        /// Single instance of <see cref="DataCache"/>.
        /// </summary>
        /// <value>Used for sharing resources between view models.</value>
        private readonly DataCache dataCache = DataCache.Instance;

        /// <summary>
        /// Instantiates the <see cref="InlineLinksModel"/> class for inline link.
        /// </summary>
        /// <param name="topicName">The name of the topic that contains the inline link</param>
        /// <param name="linkUri">URI of the inline link.</param>
        public InlineLinksModel(string topicName, string linkUri)
        {
            linkURI = linkUri.ToString();
            linkedFile = Path.GetFileName(linkUri.ToString()).Split('#')[0];
        }
    }
}
