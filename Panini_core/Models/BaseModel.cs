using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Panini.Models
{
    /// <summary>
    /// Base model class.
    /// </summary>
    public class BaseModel: INotifyPropertyChanged
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
    }
}
