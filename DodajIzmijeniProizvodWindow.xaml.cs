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
using System.Windows.Shapes;

namespace Projekat_A_KafeBar
{
  
    public partial class DodajIzmijeniProizvodWindow : Window
    {

        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private ProizvodiPage.Proizvod trenutniProizvod = null;

        public DodajIzmijeniProizvodWindow(ProizvodiPage.Proizvod proizvod = null)
        {
            InitializeComponent();
            trenutniProizvod = proizvod;
            UcitajKategorije();

            if (trenutniProizvod != null)
            {
                NazivBox.Text = proizvod.Naziv;
                CijenaBox.Text = proizvod.Cijena.ToString();
                KolicinaBox.Text = proizvod.Kolicina.ToString();
                KategorijaComboBox.SelectedValue = proizvod.Kategorija;
                //this.Title = "Izmijeni proizvod";
                this.SetResourceReference(Window.TitleProperty, "Proizvod_Title_Edit");
            }
            else
            {
                //this.Title = "Dodaj proizvod";
                this.SetResourceReference(Window.TitleProperty, "Proizvod_Title_Add");
            }
        }

        private void UcitajKategorije()
        {
            List<string> kategorije = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Naziv FROM kategorija";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kategorije.Add(reader.GetString("Naziv"));
                    }
                }
            }

            KategorijaComboBox.ItemsSource = kategorije;
        }

        private void Sacuvaj_Click(object sender, RoutedEventArgs e)
        {
            string naziv = NazivBox.Text.Trim();
            string cijenaText = CijenaBox.Text.Trim();
            string kolicinaText = KolicinaBox.Text.Trim();
            string kategorija = KategorijaComboBox.SelectedItem as string;

            if (string.IsNullOrEmpty(naziv) || string.IsNullOrEmpty(cijenaText) || string.IsNullOrEmpty(kolicinaText) || string.IsNullOrEmpty(kategorija))
            {
                MessageBox.Show((string)Application.Current.Resources["Msg_Proizvod_PopuniSvaPolja"]);
                return;
            }

            if (!decimal.TryParse(cijenaText, out decimal cijena))
            {
                MessageBox.Show((string)Application.Current.Resources["Msg_Proizvod_CijenaBroj"]);
                return;
            }

            if (!int.TryParse(kolicinaText, out int kolicina))
            {
                MessageBox.Show((string)Application.Current.Resources["Msg_Proizvod_KolicinaCijeliBroj"]);
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd;

                if (trenutniProizvod == null)
                {
                    // INSERT
                    string insertQuery = @"
                INSERT INTO proizvod (Naziv, Cijena, Količina, Kategorija_IDKategorije)
                VALUES (@naziv, @cijena, @kolicina, (SELECT IdKategorije FROM kategorija WHERE Naziv=@kat))";
                    cmd = new MySqlCommand(insertQuery, conn);
                }
                else
                {
                    // UPDATE
                    string updateQuery = @"
                UPDATE proizvod 
                SET Naziv=@naziv, Cijena=@cijena, Količina=@kolicina, Kategorija_IDKategorije=(SELECT IdKategorije FROM kategorija WHERE Naziv=@kat)
                WHERE IdProizvoda=@id";
                    cmd = new MySqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@id", trenutniProizvod.Id);
                }

                cmd.Parameters.AddWithValue("@naziv", naziv);
                cmd.Parameters.AddWithValue("@cijena", cijena);
                cmd.Parameters.AddWithValue("@kolicina", kolicina);
                cmd.Parameters.AddWithValue("@kat", kategorija);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show((string)Application.Current.Resources["Msg_Proizvod_Sacuvan"]);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    // Duplicate entry
                    if (ex.Number == 1062)
                    {
                        MessageBox.Show((string)Application.Current.Resources["Msg_Proizvod_Duplicate"]);
                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void Otkazi_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
