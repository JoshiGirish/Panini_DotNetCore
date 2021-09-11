
using System.IO;
namespace Panini.Models
{
    /// <summary>
    /// Model for displaying the related links data in the <c>Related Links</c> tab of <c>Results View</c>.
    /// </summary>
    public class RelatedLinksModel: BaseModel
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
        /// Instantiates the <see cref="RelatedLinksModel"/> class for related link.
        /// </summary>
        /// <param name="topicName">The name of the topic that contains the related link</param>
        /// <param name="linkUri">URI of the related link.</param>
        public RelatedLinksModel(string topicName, string linkUri)
        {
            linkURI = linkUri.ToString();
            linkedFile = Path.GetFileName(linkUri.ToString()).Split('#')[0];
        }
    }
}
