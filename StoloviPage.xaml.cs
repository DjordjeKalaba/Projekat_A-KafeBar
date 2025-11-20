using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Projekat_A_KafeBar
{
    public partial class StoloviPage : Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private List<Sto> stolovi = new();
        private bool canEdit;

        public StoloviPage(string uloga)
        {
            InitializeComponent();

            canEdit = uloga.Equals("Admin", StringComparison.OrdinalIgnoreCase);

            DodajStoButton.IsEnabled = canEdit;
            IzmijeniStoButton.IsEnabled = canEdit;
            ObrisiStoButton.IsEnabled = canEdit;

            UcitajStolove();
        }

        private void UcitajStolove()
        {
            stolovi.Clear();
            StoloviWrapPanel.Children.Clear();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT IdSto, Kapacitet, Status FROM sto", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    stolovi.Add(new Sto
                    {
                        IdSto = reader.GetInt32("IdSto"),
                        Kapacitet = reader.GetInt32("Kapacitet"),
                        Status = reader.GetString("Status")
                    });
                }
            }

            foreach (var sto in stolovi)
            {
                var dugme = new Button
                {
                    Content = $"Sto #{sto.IdSto}\nKapacitet: {sto.Kapacitet}\nStatus: {sto.Status}",
                    Width = 130,
                    Height = 90,
                    Margin = new Thickness(10),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(sto.Boja)),
                    Tag = sto
                };

                dugme.Click += StoKliknut;
                StoloviWrapPanel.Children.Add(dugme);
            }
        }

        private void StoKliknut(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var sto = btn?.Tag as Sto;
            if (sto == null) return;

            // Sto "Slobodan" → pitaj da li rezervisati
            if (sto.Status == "Slobodan")
            {
                string pitanje = (string)Application.Current.Resources["Stolovi_Msg_PromijeniURezervisan"];

                if (MessageBox.Show(pitanje,
                                    (string)Application.Current.Resources["Stolovi_Title_Info"],
                                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        var cmd = new MySqlCommand("UPDATE sto SET Status='Rezervisan' WHERE IdSto=@id", conn);
                        cmd.Parameters.AddWithValue("@id", sto.IdSto);
                        cmd.ExecuteNonQuery();
                    }

                    sto.Status = "Rezervisan";

                    btn.Content = $"Sto #{sto.IdSto}\nKapacitet: {sto.Kapacitet}\nStatus: {sto.Status}";
                    btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(sto.Boja));
                }
            }
            else
            {
                MessageBox.Show(
                    (string)Application.Current.Resources["Stolovi_Msg_NemogucaPromjena"],
                    (string)Application.Current.Resources["Stolovi_Title_Info"]);
            }
        }

        private void Osvjezi_Click(object sender, RoutedEventArgs e)
        {
            UcitajStolove();
        }

        private void DodajSto_Click(object sender, RoutedEventArgs e)
        {
            if (!canEdit) return;

            var win = new DodajIzmijeniStoWindow();
            if (win.ShowDialog() == true)
            {
                MessageBox.Show(
                    (string)Application.Current.Resources["Stolovi_Msg_DodatOK"],
                    (string)Application.Current.Resources["Stolovi_Title_OK"]);

                UcitajStolove();
            }
        }

        private void IzmijeniSto_Click(object sender, RoutedEventArgs e)
        {
            if (!canEdit) return;

            var izborWin = new IzborStolaWindow(stolovi);
            if (izborWin.ShowDialog() == true && izborWin.IzabraniSto != null)
            {
                var editWin = new DodajIzmijeniStoWindow(izborWin.IzabraniSto);

                if (editWin.ShowDialog() == true)
                {
                    MessageBox.Show(
                        (string)Application.Current.Resources["Stolovi_Msg_IzmjenaOK"],
                        (string)Application.Current.Resources["Stolovi_Title_OK"]);

                    UcitajStolove();
                }
            }
           
        }

        private void ObrisiSto_Click(object sender, RoutedEventArgs e)
        {
            if (!canEdit) return;

            var izborWin = new IzborStolaWindow(stolovi);
            if (izborWin.ShowDialog() == true && izborWin.IzabraniSto != null)
            {
                if (MessageBox.Show(
                    (string)Application.Current.Resources["Stolovi_Msg_PotvrdaBrisanja"],
                    (string)Application.Current.Resources["Stolovi_Title_Info"],
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (var conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        var cmd = new MySqlCommand("DELETE FROM sto WHERE IdSto=@id", conn);
                        cmd.Parameters.AddWithValue("@id", izborWin.IzabraniSto.IdSto);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show(
                        (string)Application.Current.Resources["Stolovi_Msg_BrisanjeOK"],
                        (string)Application.Current.Resources["Stolovi_Title_OK"]);

                    UcitajStolove();
                }
            }
            
        }
    }

    public class Sto
    {
        public int IdSto { get; set; }
        public int Kapacitet { get; set; }
        public string Status { get; set; }

        public string Boja => Status switch
        {
            "Slobodan" => "#90EE90",
            "Rezervisan" => "#FFD700",
            "Zauzet" => "#FF6347",
            _ => "#FFFFFF"
        };

        public string Opis => $"Sto #{IdSto}";
    }
}
