﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ILGPU;
using ILGPU.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Panini.ViewModel
{
    class ConfigViewModel: BaseViewModel
    {

        private readonly DataCache dataCache = DataCache.Instance;

        private List<string> ignoreTopicNameEndsWith = new List<string> { "-r-wn", "-sb", "-c-ov", "-c-AccessToContent", "-r-rg", "-r-ui-ContextMenus", "-ActBar" };
        private List<string> ignoreTopicNameStartsWith = new List<string> { "help-rc" };
        private List<string> ignoreTopicNameContains = new List<string> { "-ActBar", "-ContexMenus-" };
        private List<string> ignoreTopicName = new List<string> { "Sitemap" };
        public string IgnoreTopicNameEndsWith { get => string.Join(",", ignoreTopicNameEndsWith); set { ignoreTopicNameEndsWith = value.Replace(" ", string.Empty).Split(',').ToList(); dataCache.corpus.ignoreTopicNameEndsWith = ignoreTopicNameEndsWith; RaisePropertyChanged(); } }
        public string IgnoreTopicNameContains { get => string.Join(",",ignoreTopicNameContains); set { ignoreTopicNameContains = value.Replace(" ", string.Empty).Split(',').ToList(); dataCache.corpus.ignoreTopicNameContains = ignoreTopicNameContains; RaisePropertyChanged(); } }
        public string IgnoreTopicName { get => string.Join(",",ignoreTopicName); set { ignoreTopicName = value.Replace(" ", string.Empty).Split(',').ToList(); dataCache.corpus.ignoreTopicName = ignoreTopicName; RaisePropertyChanged();  } }

        public string IgnoreTopicNameStartsWith { get => string.Join(",", ignoreTopicNameStartsWith);  set { ignoreTopicNameStartsWith = value.Replace(" ", string.Empty).Split(',').ToList(); dataCache.corpus.ignoreTopicNameStartsWith = ignoreTopicNameStartsWith; RaisePropertyChanged(); } }
        private string _colorScheme = "Default";

        private List<string> _accelerators;

        public List<string> Devices
        {
            get { return _accelerators; }
            set { _accelerators = value; RaisePropertyChanged(); }
        }
        public string AccleratorDevice
        {
            set { dataCache.Config["acclerator"] = value; }
        }

        public string ColorScheme
        {
            get { return _colorScheme; }
            set { _colorScheme= value; RaisePropertyChanged(); dataCache.Config["colorScheme"] = value; }
        }

        private int _keywordCount = 5;

        public int KeywordCount
        {
            get { return _keywordCount; }
            set { _keywordCount = value; RaisePropertyChanged(); dataCache.Config["keywordCount"] = value; }
        }

        private int _similarTopicCount = 5;

        public int SimilarTopicCount
        {
            get { return _similarTopicCount; }
            set { _similarTopicCount = value; RaisePropertyChanged(); dataCache.Config["similarTopicCount"] = value; }
        }

        private int _maxVocabSize = 500;

        public int MaxVocabSize
        {
            get { return _maxVocabSize; }
            set { _maxVocabSize = value; RaisePropertyChanged(); dataCache.Config["maxVocabSize"] = value;}
        }


        public ConfigViewModel()
        {
            find_accelerators();
            PropertyChanged += update_config;
        }

        private void update_config(object sender, PropertyChangedEventArgs e)
        {
            dataCache.corpus.ignoreData = new Dictionary<string, List<string>>(){{"end",  ignoreTopicNameEndsWith},
                                                                {"start", ignoreTopicNameStartsWith},
                                                                {"contains", ignoreTopicNameContains},
                                                                {"name", ignoreTopicName } };
        }

        #region Get Accelerators
        public void find_accelerators()
        {
            Devices = new List<string>();
            Context cont = new Context();
            foreach(var acceleratorId in Accelerator.Accelerators)
            {
                using (var accl = Accelerator.Create(cont, acceleratorId))
                {
                    Devices.Add($" [{accl.AcceleratorType}] - {accl.Name}");
                }
            }
        }
        #endregion
    }
    public class ColorSchemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((string)parameter == (string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? parameter : null;
        }
    }
}
