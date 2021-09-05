
using System.IO;
namespace Panini.Models
{
    public class RelatedLinksModel: BaseModel
    {
        public string linkedFile { get; set; }
        public string linkURI { get; set; }

        private readonly DataCache dataCache = DataCache.Instance;

        public RelatedLinksModel(string topicName, string linkUri)
        {
            linkURI = linkUri.ToString();
            linkedFile = Path.GetFileName(linkUri.ToString()).Split('#')[0];
        }
    }
}
