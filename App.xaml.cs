using System;
using System.Configuration;
using System.Data;
using System.Windows;


namespace Projekat_A_KafeBar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        public static void ChangeTheme(string tema)
        {
            ResourceDictionary newTheme = new ResourceDictionary();
            switch (tema)
            {
                case "OrangeTheme":
                    newTheme.Source = new Uri("Teme/OrangeTheme.xaml", UriKind.Relative);
                    break;
                case "DarkTheme":
                    newTheme.Source = new Uri("Teme/DarkTheme.xaml", UriKind.Relative);
                    break;
                case "LightTheme":
                    newTheme.Source = new Uri("Teme/LightTheme.xaml", UriKind.Relative);
                    break;
            }

            // Ukloni prethodnu temu
            var existing = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && (
                    d.Source.ToString().Contains("OrangeTheme.xaml") ||
                    d.Source.ToString().Contains("DarkTheme.xaml") ||
                    d.Source.ToString().Contains("LightTheme.xaml")));

            if (existing != null)
                Application.Current.Resources.MergedDictionaries.Remove(existing);

            Application.Current.Resources.MergedDictionaries.Add(newTheme);
        }

        public static void ChangeLanguage(string jezik)
        {
            if (string.IsNullOrWhiteSpace(jezik)) return;

            var newLang = new ResourceDictionary
            {
                Source = new Uri($"Jezici/{jezik}.xaml", UriKind.Relative)
            };

            // ukloni prethodne jezike (tražimo fajlove iz foldera Jezici)
            var existingLang = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.StartsWith("Jezici/", StringComparison.OrdinalIgnoreCase));

            if (existingLang != null)
                Application.Current.Resources.MergedDictionaries.Remove(existingLang);

            Application.Current.Resources.MergedDictionaries.Add(newLang);
        }
    }

}
