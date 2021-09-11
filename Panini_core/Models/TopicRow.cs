using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    /// <summary>
    /// Model for displaying a row in list of topics displayed in <c>Load View</c>.
    /// </summary>
    public class TopicRow
    {
        private string _topicName;
        /// <summary>
        /// Name of the topic.
        /// </summary>
        /// <value>The name of the topic to be displayed in the list.</value>
        public string TopicName
        {
            get { return _topicName; }
            set { _topicName = value; }
        }

    }
}
