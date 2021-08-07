using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    public class TopicResultItem : BaseModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public float simScore { get; set; }
        public List<string> words { get; set; }
        private string _isLinked = "Collapsed";

        private int _numOfInlineLinks;

        public int NumOfInlineLinks
        {
            get { return _numOfInlineLinks; }
            set { _numOfInlineLinks = value; RaisePropertyChanged(); }
        }

        private int _numOfRelatedLinks;

        public int NumOfRelatedLinks
        {
            get { return _numOfRelatedLinks; }
            set { _numOfRelatedLinks = value; RaisePropertyChanged(); }
        }

        public string IsLinked
        {
            get { return _isLinked ; }
            set { _isLinked = value; }
        }

        private List<string> _keywords = new List<string>();
        public List<string> Keywords
        {
            get { return _keywords; }
            set { _keywords = value; RaisePropertyChanged(); }
        }
        //public string keywordsString { get { return string.Join("    ", keywords); } set{ } }

        public TopicResultItem()
        {
        }

        public TopicResultItem(string name, string path, float simScore, List<string> words, List<string> keywords, string isLinked)
        {
            Name = name;
            Path = path;
            this.simScore = simScore;
            this.words = words;
            this.Keywords = keywords;
            this.IsLinked = isLinked;
        }

    }
}
