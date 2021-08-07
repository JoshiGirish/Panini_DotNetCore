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

        private Page _currentPage = new Page();

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

        public MainViewModel()
        {
            aboutVM = new AboutViewModel();
            configVM = new ConfigViewModel();
            loadVM = new LoadViewModel();
            resultsVM = new ResultsViewModel();
            summaryVM = new SummaryViewModel();
        }

        public BaseViewModel AboutViewModel { get { return aboutVM; } }
        public BaseViewModel baseViewModel { get { return configVM; } }
        public BaseViewModel LoadViewModel { get { return loadVM; } }
        public BaseViewModel ResultsViewModel { get { return resultsVM; } }
        public BaseViewModel SummaryViewModel { get { return summaryVM; } }
    }

}
