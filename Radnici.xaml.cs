using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace Projekat_A_KafeBar
{
    public partial class Radnici : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private int trenutniRadnikId;

        public Radnici(int idZaposleni)
        {
            InitializeComponent();
            trenutniRadnikId = idZaposleni;
            UcitajKorisnickePreferencije();
        }

        private void UcitajKorisnickePreferencije()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Tema, Jezik FROM zaposleni WHERE IdZaposleni=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", trenutniRadnikId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string tema = reader["Tema"] != DBNull.Value ? reader["Tema"].ToString() : "OrangeTheme";
                        string jezik = reader["Jezik"] != DBNull.Value ? reader["Jezik"].ToString() : "Srpski";

                        SelectComboBoxItemByTag(TemaComboBox, tema);
                        SelectComboBoxItemByTag(JezikComboBox, jezik);

                        // Primjeni odmah
                        App.ChangeTheme(tema);
                        App.ChangeLanguage(jezik);
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

            App.ChangeTheme(novaTema);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE zaposleni SET Tema=@tema WHERE IdZaposleni=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@tema", novaTema);
                cmd.Parameters.AddWithValue("@id", trenutniRadnikId);
                cmd.ExecuteNonQuery();
            }
        }

        private void JezikComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(JezikComboBox.SelectedItem is ComboBoxItem selectedItem)) return;
            string noviJezik = selectedItem.Tag.ToString();

            App.ChangeLanguage(noviJezik);

            if (MainFrame.Content is ProizvodiPage proizvodiPage)
            {
                proizvodiPage.UpdateDataGridHeaders();
            }

            if (MainFrame.Content is ZatvoreniRacuniPage zatvoreni)
            {
                zatvoreni.ApplyTranslations(); // osvježava tekstove i zaglavlja
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE zaposleni SET Jezik=@jezik WHERE IdZaposleni=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@jezik", noviJezik);
                cmd.Parameters.AddWithValue("@id", trenutniRadnikId);
                cmd.ExecuteNonQuery();
            }
        }



        // --- Navigacija ---
        private void ListBoxItem_Selected_NoviRacun(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new NoviRacunPage(trenutniRadnikId);
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
        private void ListBoxItem_Selected_MeniPica(object sender, RoutedEventArgs e) { MainFrame.Content = new ProizvodiPage("Radnik"); }
        private void ListBoxItem_Selected_Stolovi(object sender, RoutedEventArgs e) { MainFrame.Content = new StoloviPage("Radnik"); }
        private void ListBoxItem_Selected_Profil(object sender, RoutedEventArgs e) { MainFrame.Content = new RadnikProfilPage(trenutniRadnikId, this); }

        private void ListBoxItem_Selected_Odjava(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}


