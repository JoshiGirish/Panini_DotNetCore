using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panini.Pages;
using Panini.ViewModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Panini.Commands;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Collections;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Specialized;

namespace Panini.ViewModel
{
    class MainViewModel : BaseViewModel
    {

        private readonly AboutViewModel aboutVM;
        private readonly ConfigViewModel configVM;
        private readonly LoadViewModel loadVM;
        private readonly ResultsViewModel resultsVM;
        private readonly SummaryViewModel summaryVM;


        private AboutPage aboutPage = new AboutPage();
        private ConfigPage configPage = new ConfigPage();
        private LoadPage loadPage = new LoadPage();
        private ResultsPage resultsPage = new ResultsPage();
        private SummaryPage summaryPage = new SummaryPage();

        private Page _currentPage = new AboutPage();

        private readonly DataCache dataCache = DataCache.Instance;
        public Page CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; RaisePropertyChanged(); }
        }

        private ListViewItem _currentItem;

        public ListViewItem CurrentItem
        {
            get { return _currentItem; }
            set { _currentItem = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<string> ViewState
        {
            get { return dataCache.ViewState; }
            set { dataCache.ViewState = value; RaisePropertyChanged(); }
        }


        #region Switch View command callback
        /// <summary>
        /// This command changes the view depending on the selected navigation item.
        /// </summary>
        private ICommand _switchView;

        public ICommand SwitchView
        {
            get { return _switchView ?? (_switchView = new ParameterCommandHandler((obj) => SwitchPage(obj), () => { return true; })); }
        }

        private void SwitchPage(object name)
        {
            var listItemName = (string)name;

            switch (listItemName)
            {
                case "About":
                    CurrentPage = aboutPage;
                    break;

                case "Load":
                    CurrentPage = loadPage;
                    break;

                case "Config":
                    CurrentPage = configPage;
                    break;

                case "Results":
                    CurrentPage = resultsPage;
                    break;

                case "Summary":
                    CurrentPage = summaryPage;
                    break;

            }
        }
        #endregion

        #region Update View delegate
        public void update_view_status(object sender, NotifyCollectionChangedEventArgs e)
        {
            ViewState = dataCache.ViewState;
        }
        #endregion

        public MainViewModel()
        {
            aboutVM = new AboutViewModel();
            configVM = new ConfigViewModel();
            loadVM = new LoadViewModel();
            resultsVM = new ResultsViewModel();
            summaryVM = new SummaryViewModel();

            dataCache.ViewState.CollectionChanged += update_view_status;
        }

        public BaseViewModel AboutViewModel { get { return aboutVM; } }
        public BaseViewModel baseViewModel { get { return configVM; } }
        public BaseViewModel LoadViewModel { get { return loadVM; } }
        public BaseViewModel ResultsViewModel { get { return resultsVM; } }
        public BaseViewModel SummaryViewModel { get { return summaryVM; } }
    }

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
