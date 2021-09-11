using System;
using System.Linq;
using System.Windows.Data;
using System.Globalization;

namespace Panini.ViewModel
{
    /// <summary>
    /// The class that is used for conversion of view state.
    /// </summary>
    public class ViewStateConverter : IValueConverter
    {
        private readonly DataCache dataCache = DataCache.Instance;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return dataCache.ViewState.Contains(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? parameter : null;
        }
    }
}
