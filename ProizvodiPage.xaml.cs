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
    public partial class ProizvodiPage : Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private bool canEdit;

        public ProizvodiPage()
        {
            InitializeComponent();
            UpdateDataGridHeaders();
            UcitajKategorije();
            UcitajProizvode();
        }

        public void UpdateDataGridHeaders()
        {
            ProizvodiDataGrid.Columns[0].Header = Application.Current.FindResource("Proizvodi_Col_Naziv");
            ProizvodiDataGrid.Columns[1].Header = Application.Current.FindResource("Proizvodi_Col_Kategorija");
            ProizvodiDataGrid.Columns[2].Header = Application.Current.FindResource("Proizvodi_Col_Cijena");
            ProizvodiDataGrid.Columns[3].Header = Application.Current.FindResource("Proizvodi_Col_Kolicina");
        }

        public ProizvodiPage(string uloga)
        {
            InitializeComponent();
            canEdit = uloga.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            DodajProizvodButton.IsEnabled = canEdit;
            IzmijeniProizvodButton.IsEnabled = canEdit;
            ObrisiProizvodButton.IsEnabled = canEdit;
            UcitajProizvode();
            UcitajKategorije();
        }

        public class Proizvod
        {
            public int Id { get; set; }
            public string Naziv { get; set; }
            public decimal Cijena { get; set; }
            public int Kolicina { get; set; }
            public string Kategorija { get; set; }
        }

        #region Ucitavanje kategorija
        private void UcitajKategorije()
        {
            List<Tuple<int, string>> kategorije = new List<Tuple<int, string>>();

            // Opcionalno: prva stavka za prikaz svih proizvoda
            kategorije.Add(new Tuple<int, string>(0, "Sve kategorije"));

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT IdKategorije, Naziv FROM kategorija";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kategorije.Add(new Tuple<int, string>(
                            reader.GetInt32("IdKategorije"),
                            reader.GetString("Naziv")));
                    }
                }
            }

            KategorijeComboBox.ItemsSource = kategorije;
            KategorijeComboBox.DisplayMemberPath = "Item2"; // prikaz naziva
            KategorijeComboBox.SelectedValuePath = "Item1";  // ID kategorije
            KategorijeComboBox.SelectedIndex = 0; // default: sve kategorije
        }
        #endregion

        #region Ucitavanje proizvoda
        private void UcitajProizvode()
        {
            UcitajProizvode(0); // prikaz svih proizvoda
        }

        private void UcitajProizvode(int idKategorije)
        {
            try
            {
                List<Proizvod> proizvodi = new List<Proizvod>();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT p.IdProizvoda AS Id, 
                               p.Naziv, 
                               p.Cijena, 
                               p.Količina AS Kolicina, 
                               k.Naziv AS Kategorija
                        FROM proizvod p
                        JOIN kategorija k ON p.Kategorija_IDKategorije = k.IdKategorije";

                    if (idKategorije != 0)
                        query += " WHERE k.IdKategorije=@id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (idKategorije != 0)
                        cmd.Parameters.AddWithValue("@id", idKategorije);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            proizvodi.Add(new Proizvod
                            {
                                Id = reader.GetInt32("Id"),
                                Naziv = reader.GetString("Naziv"),
                                Cijena = reader.GetDecimal("Cijena"),
                                Kolicina = reader.GetInt32("Kolicina"),
                                Kategorija = reader.GetString("Kategorija")
                            });
                        }
                    }
                }

                ProizvodiDataGrid.ItemsSource = proizvodi;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Greška pri učitavanju proizvoda: " + ex.Message);
            }
        }
        #endregion

        #region Filter po kategoriji
        private void KategorijeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KategorijeComboBox.SelectedValue != null)
            {
                int idKategorije = (int)KategorijeComboBox.SelectedValue;
                UcitajProizvode(idKategorije);
            }
        }
        #endregion

        #region CRUD
        private void DodajProizvod_Click(object sender, RoutedEventArgs e)
        {
           
                DodajIzmijeniProizvodWindow window = new DodajIzmijeniProizvodWindow();
                if (window.ShowDialog() == true)
                    UcitajProizvode((int)KategorijeComboBox.SelectedValue);
            
            
        }

        private void IzmijeniProizvod_Click(object sender, RoutedEventArgs e)
        {
            if (ProizvodiDataGrid.SelectedItem is Proizvod proizvod)
            {
                DodajIzmijeniProizvodWindow window = new DodajIzmijeniProizvodWindow(proizvod);
                if (window.ShowDialog() == true)
                    UcitajProizvode((int)KategorijeComboBox.SelectedValue);
            }
            else
            {
                MessageBox.Show(
            (string)Application.Current.FindResource("Proizvodi_Msg_OdaberiIzmjena"),
            (string)Application.Current.FindResource("Proizvodi_Msg_PotvrdaNaslov"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
            }
        }

        /*private void ObrisiProizvod_Click(object sender, RoutedEventArgs e)
        {
            if (ProizvodiDataGrid.SelectedItem is Proizvod proizvod)
            {
                if (MessageBox.Show($"Da li ste sigurni da želite obrisati proizvod '{proizvod.Naziv}'?",
                    "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM proizvodi WHERE IdProizvoda=@id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", proizvod.Id);
                        cmd.ExecuteNonQuery();
                    }
                    UcitajProizvode((int)KategorijeComboBox.SelectedValue);
                }
            }
            else
            {
                MessageBox.Show("Odaberite proizvod za brisanje!");
            }
        }*/

        private void ObrisiProizvod_Click(object sender, RoutedEventArgs e)
        {
            if (ProizvodiDataGrid.SelectedItem is Proizvod proizvod)
            {
                string poruka = string.Format((string)Application.Current.FindResource("Proizvodi_Msg_PotvrdaBrisanja"), proizvod.Naziv);

                if (MessageBox.Show(
                    poruka,
                    (string)Application.Current.FindResource("Proizvodi_Msg_PotvrdaNaslov"),
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM proizvodi WHERE IdProizvoda=@id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", proizvod.Id);
                        cmd.ExecuteNonQuery();
                    }
                    UcitajProizvode((int)KategorijeComboBox.SelectedValue);
                }
            }
            else
            {
                MessageBox.Show(
                    (string)Application.Current.FindResource("Proizvodi_Msg_OdaberiBrisanje"),
                    (string)Application.Current.FindResource("Proizvodi_Msg_PotvrdaNaslov"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
        #endregion
    }
}
