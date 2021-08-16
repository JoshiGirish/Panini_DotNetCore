using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    public class InlineLinksModel: BaseModel
    {
        public string linkedFile { get; set; }
        public string linkURI { get; set; }

        private readonly DataCache dataCache = DataCache.Instance;

        public InlineLinksModel(string topicName, string linkUri)
        {
            linkURI = linkUri.ToString();
            linkedFile = Path.GetFileName(linkUri.ToString()).Split('#')[0];
        }
    }
}
