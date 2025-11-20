using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;



namespace Projekat_A_KafeBar
{
    public partial class Admin : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public Admin()
        {
            InitializeComponent();
            UcitajKorisnickePreferencije();
        }

        private void UcitajKorisnickePreferencije()
        {
            int id = MainWindow.TrenutniKorisnikId;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Tema, Jezik FROM zaposleni WHERE IdZaposleni=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string tema = reader["Tema"] != DBNull.Value ? reader["Tema"].ToString() : "OrangeTheme";
                        string jezik = reader["Jezik"] != DBNull.Value ? reader["Jezik"].ToString() : "Srpski";

                        // Postavi selectione u combobox-e (ako se nalaze ComboBoxItem-ovi sa Tag-om)
                        SelectComboBoxItemByTag(TemaComboBox, tema);
                        SelectComboBoxItemByTag(JezikComboBox, jezik);
                    }
                }
            }
        }

        private void SelectComboBoxItemByTag(ComboBox cb, string tagValue)
        {
            if (cb == null || tagValue == null) return;
            foreach (ComboBoxItem item in cb.Items)
            {
                if ((item.Tag?.ToString() ?? "") == tagValue)
                {
                    cb.SelectedItem = item;
                    break;
                }
            }
        }

        private void TemaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(TemaComboBox.SelectedItem is ComboBoxItem selectedItem)) return;
            string novaTema = selectedItem.Tag.ToString();

            // Primijeni odmah
            App.ChangeTheme(novaTema);

            // Spremi u bazu
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE zaposleni SET Tema=@tema WHERE IdZaposleni=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@tema", novaTema);
                cmd.Parameters.AddWithValue("@id", MainWindow.TrenutniKorisnikId);
                cmd.ExecuteNonQuery();
            }
        }

        private void JezikComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(JezikComboBox.SelectedItem is ComboBoxItem selectedItem)) return;
            string noviJezik = selectedItem.Tag.ToString();

            // Primijeni odmah
            App.ChangeLanguage(noviJezik);

            if (MainFrame.Content is ProizvodiPage proizvodiPage)
            {
                proizvodiPage.UpdateDataGridHeaders();
            }

            if (MainFrame.Content is RadniciPage radniciPage)
            {
                radniciPage.UpdateDataGridHeaders();
            }

            if (MainFrame.Content is ZatvoreniRacuniPage zatvoreni)
            {
                zatvoreni.ApplyTranslations(); // osvježava tekstove i zaglavlja
            }

            // Spremi u bazu
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE zaposleni SET Jezik=@jezik WHERE IdZaposleni=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@jezik", noviJezik);
                cmd.Parameters.AddWithValue("@id", MainWindow.TrenutniKorisnikId);
                cmd.ExecuteNonQuery();
            }
        }

        // --- Ostatak event handler-a za navigaciju (preuzmi iz svog postojećeg Admin.xaml.cs) ---
        private void ListBoxItem_Selected_Radnici(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RadniciPage());
        }

        private void ListBoxItem_Selected_Proizvodi(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ProizvodiPage("Admin");
        }

        private void btnRacuni_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.ContextMenu != null)
            {
                btn.ContextMenu.PlacementTarget = btn;
                btn.ContextMenu.IsOpen = true;
            }
        }

        private void OtvoreniRacuni_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new OtvoreniRacuniPage());
        private void ZatvoreniRacuni_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ZatvoreniRacuniPage());
        private void ListBoxItem_Selected_Kategorije(object sender, RoutedEventArgs e) { this.MainFrame.Content = new KategorijePage();  }
        private void ListBoxItem_Selected_Stolovi(object sender, RoutedEventArgs e) { this.MainFrame.Content = new StoloviPage("Admin"); }

        private void ListBoxItem_Selected_Odjava(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
