using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Panini.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            var handler = PropertyChanged; 
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
            // If you use PropertyChanged? directly instead of handler? the app may crash if you implement multi threading.
        }

        public static Dictionary<string, Color> StatusColors = new Dictionary<string, Color>()
        {
            {"Error", Color.FromRgb(255,186,186) },
            {"Warning", Color.FromRgb(255,168,95)},
            {"Success", Color.FromRgb(186,255,186)},
            {"Default", Colors.Transparent}
        };
    }
}
