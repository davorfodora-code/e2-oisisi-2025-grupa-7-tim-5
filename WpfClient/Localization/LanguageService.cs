using System;
using System.Windows;

namespace WpfClient.Localization
{
    public static class LanguageService
    {
        public static void SetLanguage(string lang)
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri(
                    lang == "sr"
                        ? "Resources/Strings.sr.xaml"
                        : "Resources/Strings.en.xaml",
                    UriKind.Relative)
            };

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}
