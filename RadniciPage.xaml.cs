using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Projekat_A_KafeBar
{
    public partial class RadniciPage : Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public RadniciPage()
        {
            InitializeComponent();
            UcitajRadnike();
            UpdateDataGridHeaders();
        }

        public class Radnik
        {
            public int Id { get; set; }
            public string Ime { get; set; }
            public string Prezime { get; set; }
            public string KorisnickoIme { get; set; }
            public string Uloga { get; set; }
            public string BrojTelefona { get; set; }
            public decimal Plata { get; set; }
        }

        private void UcitajRadnike()
        {
            try
            {
                List<Radnik> radnici = new List<Radnik>();
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT IdZaposleni AS Id, Ime, Prezime, KorisničkoIme AS KorisnickoIme, Uloga, BrojTelefona, Plata FROM zaposleni";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            radnici.Add(new Radnik
                            {
                                Id = reader.GetInt32("Id"),
                                Ime = reader.GetString("Ime"),
                                Prezime = reader.GetString("Prezime"),
                                KorisnickoIme = reader.GetString("KorisnickoIme"),
                                Uloga = reader.GetString("Uloga"),
                                BrojTelefona = reader.IsDBNull(reader.GetOrdinal("BrojTelefona")) ? "" : reader.GetString("BrojTelefona"),
                                Plata = reader.IsDBNull(reader.GetOrdinal("Plata")) ? 0 : reader.GetDecimal("Plata")
                            });
                        }
                    }
                }
                RadniciDataGrid.ItemsSource = radnici;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    $"{Application.Current.Resources["GreskaUcitanjaText"]}: {ex.Message}",
                    (string)Application.Current.Resources["GreskaText"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void DodajRadnika_Click(object sender, RoutedEventArgs e)
        {
            DodajIzmijeniRadnikaWindow dodajWindow = new DodajIzmijeniRadnikaWindow();
            bool? result = dodajWindow.ShowDialog();
            if (result == true)
                UcitajRadnike();
        }

        private void ObrisiRadnika_Click(object sender, RoutedEventArgs e)
        {
            if (RadniciDataGrid.SelectedItem is Radnik radnik)
            {
                string pitanje = string.Format(
                    (string)Application.Current.Resources["PitanjeObrisiRadnikaText"],
                    radnik.Ime,
                    radnik.Prezime);

                if (MessageBox.Show(pitanje,
                    (string)Application.Current.Resources["PotvrdaText"],
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM zaposleni WHERE IdZaposleni=@Id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", radnik.Id);
                        cmd.ExecuteNonQuery();
                    }
                    UcitajRadnike();
                }
            }
            else
            {
                MessageBox.Show(
                    (string)Application.Current.Resources["OdaberiteRadnikaText"],
                    (string)Application.Current.Resources["InfoText"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void IzmijeniRadnika_Click(object sender, RoutedEventArgs e)
        {
            if (RadniciDataGrid.SelectedItem is Radnik radnik)
            {
                DodajIzmijeniRadnikaWindow izmijeni = new DodajIzmijeniRadnikaWindow(radnik);
                if (izmijeni.ShowDialog() == true)
                    UcitajRadnike();
            }
            else
            {
                MessageBox.Show(
                    (string)Application.Current.Resources["OdaberiteRadnikaIzmjenaText"],
                    (string)Application.Current.Resources["InfoText"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        // --- Prevod za DataGrid zaglavlja ---
        public void UpdateDataGridHeaders()
        {
            RadniciDataGrid.Columns[0].Header = (string)Application.Current.Resources["ImeText"];
            RadniciDataGrid.Columns[1].Header = (string)Application.Current.Resources["PrezimeText"];
            RadniciDataGrid.Columns[2].Header = (string)Application.Current.Resources["KorisnickoImeText"];
            RadniciDataGrid.Columns[3].Header = (string)Application.Current.Resources["UlogaText"];
            RadniciDataGrid.Columns[4].Header = (string)Application.Current.Resources["BrojTelefonaText"];
            RadniciDataGrid.Columns[5].Header = (string)Application.Current.Resources["PlataText"];
        }
    }
}
