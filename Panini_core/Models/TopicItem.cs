using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    public class TopicItem: BaseModel
    {
        public string Name {get; set;}
        public ObservableCollection<TopicResultItem> itemCollection { get; set; }
        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; RaisePropertyChanged(); }
        }

        private string _isVisible = "Visible";

        public string IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; RaisePropertyChanged(); }
        }


        public TopicItem()
        {
        }

        public TopicItem(string name, ObservableCollection<TopicResultItem> itemCollection)
        {
            Name = name;
            this.itemCollection = itemCollection;
        }

        public override string ToString()
        {
            return Name;
        }

        public List<string> get_topic_names()
        {
            List<string> names = new List<string>();
            foreach(var item in itemCollection)
            {
                names.Add(item.Name);
            }
            return names;
        }
    }
}
