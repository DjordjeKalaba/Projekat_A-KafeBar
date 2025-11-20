/*using MySql.Data.MySqlClient;
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
   
    public partial class KategorijePage : Page
    {

        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public class Kategorija
        {
            public int Id { get; set; }
            public string Naziv { get; set; }
        }

        public KategorijePage()
        {
            InitializeComponent();
            UcitajKategorije();
        }

        private void UcitajKategorije()
        {
            try
            {
                List<Kategorija> kategorije = new List<Kategorija>();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT IdKategorije AS Id, Naziv FROM kategorija";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            kategorije.Add(new Kategorija
                            {
                                Id = reader.GetInt32("Id"),
                                Naziv = reader.GetString("Naziv")
                            });
                        }
                    }
                }

                KategorijeDataGrid.ItemsSource = kategorije;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju kategorija: " + ex.Message);
            }
        }

        private void DodajKategoriju_Click(object sender, RoutedEventArgs e)
        {
            DodajIzmijeniKategorijuWindow dodajWindow = new DodajIzmijeniKategorijuWindow();
            if (dodajWindow.ShowDialog() == true)
                UcitajKategorije();
        }

        private void ObrisiKategoriju_Click(object sender, RoutedEventArgs e)
        {
            if (KategorijeDataGrid.SelectedItem is Kategorija kat)
            {
                if (MessageBox.Show($"Obriši kategoriju '{kat.Naziv}'?", "Potvrda", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM kategorija WHERE IdKategorije=@id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", kat.Id);
                        cmd.ExecuteNonQuery();
                    }
                    UcitajKategorije();
                }
            }
            else
            {
                MessageBox.Show("Odaberite kategoriju za brisanje!");
            }
        }

        private void IzmijeniKategoriju_Click(object sender, RoutedEventArgs e)
        {
            if (KategorijeDataGrid.SelectedItem is Kategorija kat)
            {
                DodajIzmijeniKategorijuWindow izmijeni = new DodajIzmijeniKategorijuWindow(kat);
                if (izmijeni.ShowDialog() == true)
                    UcitajKategorije();
            }
            else
            {
                MessageBox.Show("Odaberite kategoriju za izmjenu!");
            }
        }
    }
}*/

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Projekat_A_KafeBar
{
    public partial class KategorijePage : Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public class Kategorija
        {
            public int Id { get; set; }
            public string Naziv { get; set; }
        }

        public KategorijePage()
        {
            InitializeComponent();
            UcitajKategorije();
        }

        private void UcitajKategorije()
        {
            try
            {
                List<Kategorija> kategorije = new List<Kategorija>();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT IdKategorije AS Id, Naziv FROM kategorija";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            kategorije.Add(new Kategorija
                            {
                                Id = reader.GetInt32("Id"),
                                Naziv = reader.GetString("Naziv")
                            });
                        }
                    }
                }

                KategorijeDataGrid.ItemsSource = kategorije;
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}: {1}",
                    Application.Current.Resources["Kategorija_Msg_GreskaUcitaj"].ToString(),
                    ex.Message);
                MessageBox.Show(msg, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DodajKategoriju_Click(object sender, RoutedEventArgs e)
        {
            DodajIzmijeniKategorijuWindow dodajWindow = new DodajIzmijeniKategorijuWindow();
            if (dodajWindow.ShowDialog() == true)
                UcitajKategorije();
        }

        private void ObrisiKategoriju_Click(object sender, RoutedEventArgs e)
        {
            if (KategorijeDataGrid.SelectedItem is Kategorija kat)
            {
                string potvrdaMsg = string.Format(
                    Application.Current.Resources["Kategorija_Msg_PotvrdaBrisanja"].ToString(), kat.Naziv);

                if (MessageBox.Show(potvrdaMsg,
                    Application.Current.Resources["Kategorija_Msg_PotvrdaNaslov"].ToString(),
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM kategorija WHERE IdKategorije=@id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", kat.Id);
                        cmd.ExecuteNonQuery();
                    }
                    UcitajKategorije();
                }
            }
            else
            {
                MessageBox.Show(
                    Application.Current.Resources["Kategorija_Msg_OdabirBrisanje"].ToString(),
                    Application.Current.Resources["Kategorija_Msg_UpozorenjeNaslov"].ToString(),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void IzmijeniKategoriju_Click(object sender, RoutedEventArgs e)
        {
            if (KategorijeDataGrid.SelectedItem is Kategorija kat)
            {
                DodajIzmijeniKategorijuWindow izmijeni = new DodajIzmijeniKategorijuWindow(kat);
                if (izmijeni.ShowDialog() == true)
                    UcitajKategorije();
            }
            else
            {
                MessageBox.Show(
                    Application.Current.Resources["Kategorija_Msg_OdabirIzmjena"].ToString(),
                    Application.Current.Resources["Kategorija_Msg_UpozorenjeNaslov"].ToString(),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }
}

