using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TFIDF
{
    public class Logging: INotifyPropertyChanged
    {
        private string _status;

        public string Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged(); }
        }

        private List<string> log;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> Log
        {
            get { return log; }
            set { log = value; RaisePropertyChanged(); }
            
        }

        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
            // If you use PropertyChanged? directly instead of handler? the app may crash if you implement multi-threading.
        }
    }
}
