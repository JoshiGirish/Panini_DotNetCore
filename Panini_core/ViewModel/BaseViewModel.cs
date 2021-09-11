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
    /// <summary>
    /// Base view model to inherit from
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Property change event.
        /// </summary>
        /// <value>Occurs when a property value changes.</value>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This method is called by the Set accessor of each property. The <c>CallerMemberName</c> attribute 
        /// that is applied to the optional <paramref name="name"/> parameter causes the property 
        /// name of the caller to be substituted as an argument.
        /// </summary>
        /// <param name="name">Name of the property that raised the <see cref="PropertyChanged"/> event.</param>
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            var handler = PropertyChanged; 
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
            // If you use PropertyChanged? directly instead of handler? the app may crash if you implement multi threading.
        }

        /// <summary>
        /// Colors for status bar.
        /// </summary>
        /// <value>A dictionary of status colors representing default, error, warning, and success.</value>
        public static Dictionary<string, Color> StatusColors = new Dictionary<string, Color>()
        {
            {"Error", Color.FromRgb(255,186,186) },
            {"Warning", Color.FromRgb(255,168,95)},
            {"Success", Color.FromRgb(186,255,186)},
            {"Default", Colors.Transparent}
        };
    }
}
