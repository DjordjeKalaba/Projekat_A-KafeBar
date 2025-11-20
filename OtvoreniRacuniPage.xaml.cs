using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekat_A_KafeBar
{
    
    public partial class OtvoreniRacuniPage : Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public OtvoreniRacuniPage()
        {
            InitializeComponent();
            this.Loaded += OtvoreniRacuniPage_Loaded;
        }

        private void OtvoreniRacuniPage_Loaded(object sender, RoutedEventArgs e)
        {
            UcitajRacune(); // ponovo učitava samo otvorene račune
        }

        private void UcitajRacune()
        {
            var racuni = new List<RacunViewModel>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT r.IdRačuna, r.Iznos, r.VrijemeIzdavanja, s.IdSto, z.Ime, z.Prezime
                                 FROM račun r
                                 INNER JOIN sto s ON r.Sto_IdStola = s.IdSto
                                 INNER JOIN zaposleni z ON r.Zaposleni_IdZaposleni = z.IdZaposleni
                                 WHERE r.Status='Otvoren'";

                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    racuni.Add(new RacunViewModel
                    {
                        IdRačuna = reader.GetInt32("IdRačuna"),
                        Sto = $"Sto #{reader.GetInt32("IdSto")}",
                        Iznos = reader.GetDecimal("Iznos"),
                        VrijemeIzdavanja = reader.GetDateTime("VrijemeIzdavanja"),
                        Zaposleni = $"{reader.GetString("Ime")} {reader.GetString("Prezime")}"
                    });
                }
            }

            RacuniDataGrid.ItemsSource = racuni;
        }

        private void AzurirajRacun_Click(object sender, RoutedEventArgs e)
        {
            var odabraniRacun = RacuniDataGrid.SelectedItem as RacunViewModel;
            if (odabraniRacun == null)
            {
                        MessageBox.Show(
                 (string)Application.Current.FindResource("OtvoreniRacuni_Msg_OdaberiRacun"),
                 (string)Application.Current.FindResource("OtvoreniRacuni_Msg_Upozorenje"),
                 MessageBoxButton.OK,
                 MessageBoxImage.Warning);
                return;
            }

            // Otvara RacunDetaljiPage sa odabranim računom
            this.NavigationService.Navigate(new RacunDetaljiPage(odabraniRacun.IdRačuna, 1)); // 1 = ID zaposlenog, po potrebi zamijeni
        }
    }

    public class RacunViewModel
    {
        public int IdRačuna { get; set; }
        public string Sto { get; set; }
        public decimal Iznos { get; set; }
        public DateTime VrijemeIzdavanja { get; set; }
        public string Zaposleni { get; set; }
    }

}
