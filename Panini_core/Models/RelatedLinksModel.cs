using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            linkedFile = linkUri.ToString().Split('#')[0];
        }
    }
}
