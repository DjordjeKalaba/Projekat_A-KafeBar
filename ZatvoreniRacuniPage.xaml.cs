using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Projekat_A_KafeBar
{
    public partial class ZatvoreniRacuniPage : Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public ZatvoreniRacuniPage()
        {
            InitializeComponent();
            LoadZatvoreniRacuni();
            ApplyTranslations();
        }

        private void LoadZatvoreniRacuni()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = @"
                SELECT r.IdRačuna,
                       s.IdSto AS Sto,
                       r.Iznos,
                       r.VrijemeIzdavanja,
                       CONCAT(z.Ime, ' ', z.Prezime) AS Zaposleni
                FROM račun r
                JOIN sto s ON r.Sto_IdStola = s.IdSto
                JOIN zaposleni z ON r.Zaposleni_IdZaposleni = z.IdZaposleni
                WHERE r.Status = 'Zatvoren'
                ORDER BY r.VrijemeIzdavanja DESC";

            var da = new MySqlDataAdapter(query, conn);
            var dt = new DataTable();
            da.Fill(dt);

            RacuniDataGrid.ItemsSource = dt.DefaultView;
        }

        private void PregledajRacun_Click(object sender, RoutedEventArgs e)
        {
            if (RacuniDataGrid.SelectedItem is not DataRowView row) return;

            int idRacuna = Convert.ToInt32(row["IdRačuna"]);
            NavigationService.Navigate(new RacunDetaljiPage(idRacuna, true));
        }

        public void ApplyTranslations()
        {
            // Naslov i dugme
            NaslovTextBlock.Text = (string)Application.Current.Resources["ZatvoreniRacuni_Naslov"];
            PregledajButton.Content = (string)Application.Current.Resources["ZatvoreniRacuni_Pregledaj"];

            // Zaglavlja DataGrid-a
            ColID.Header = (string)Application.Current.Resources["ZatvoreniRacuni_ID"];
            ColSto.Header = (string)Application.Current.Resources["ZatvoreniRacuni_Sto"];
            ColIznos.Header = (string)Application.Current.Resources["ZatvoreniRacuni_Iznos"];
            ColVrijeme.Header = (string)Application.Current.Resources["ZatvoreniRacuni_Vrijeme"];
            ColZaposleni.Header = (string)Application.Current.Resources["ZatvoreniRacuni_Zaposleni"];
        }
    }
}
