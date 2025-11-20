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
using static Projekat_A_KafeBar.RadniciPage;

namespace Projekat_A_KafeBar
{
    
    public partial class DodajIzmijeniRadnikaWindow : Window
    {

        private Radnik trenutniRadnik = null;
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public DodajIzmijeniRadnikaWindow()
        {
            InitializeComponent();
            TitleTextBlock.SetResourceReference(TextBlock.TextProperty, "Radnik_Title_Add");
        }
        public DodajIzmijeniRadnikaWindow(Radnik radnik)
        {
            InitializeComponent();
            trenutniRadnik = radnik;
            TitleTextBlock.SetResourceReference(TextBlock.TextProperty, "Radnik_Title_Edit");


            ImeBox.Text = radnik.Ime;
            PrezimeBox.Text = radnik.Prezime;
            KorisnickoImeBox.Text = radnik.KorisnickoIme;
            
            UlogaBox.SelectedItem = radnik.Uloga == "Admin" ? UlogaBox.Items[0] : UlogaBox.Items[1];

            BrojTelefonaBox.Text = radnik.BrojTelefona;
            PlataBox.Text = radnik.Plata.ToString();
        }

        private void KorisnickoImeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string korisnickoIme = KorisnickoImeBox.Text.Trim();
            if (!string.IsNullOrEmpty(korisnickoIme) && KorisnickoImePostoji(korisnickoIme))
            {
                SaveButton.IsEnabled = false;
                //KorisnickoImeBox.ToolTip = "Korisničko ime već postoji!";
                KorisnickoImeBox.ToolTip = Application.Current.Resources["Msg_Radnik_KorisnickoImePostoji"] as string;
            }
            else
            {
                SaveButton.IsEnabled = true;
                KorisnickoImeBox.ToolTip = null;
            }
        }

        private bool KorisnickoImePostoji(string korisnickoIme)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM zaposleni WHERE KorisničkoIme=@korIme";
                if (trenutniRadnik != null)
                    query += " AND IdZaposleni<>@id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@korIme", korisnickoIme);
                if (trenutniRadnik != null)
                    cmd.Parameters.AddWithValue("@id", trenutniRadnik.Id);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string ime = ImeBox.Text.Trim();
            string prezime = PrezimeBox.Text.Trim();
            string korisnickoIme = KorisnickoImeBox.Text.Trim();
            string lozinka = LozinkaBox.Password.Trim();
            string uloga = ((ComboBoxItem)UlogaBox.SelectedItem).Tag.ToString();
            string brojTelefona = BrojTelefonaBox.Text.Trim();
            string plataText = PlataBox.Text.Trim();
            decimal plata = 0;
            

            if (string.IsNullOrEmpty(ime) || string.IsNullOrEmpty(prezime) || string.IsNullOrEmpty(korisnickoIme) ||
                string.IsNullOrEmpty(brojTelefona) || string.IsNullOrEmpty(plataText) || (trenutniRadnik == null && string.IsNullOrEmpty(lozinka)))
            {
                MessageBox.Show((string)Application.Current.Resources["Msg_Radnik_PopuniSvaPolja"]);
                return;
            }

            if (!decimal.TryParse(plataText, out plata))
            {
                MessageBox.Show((string)Application.Current.Resources["Msg_Radnik_PlataBroj"]);
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd;

                if (trenutniRadnik == null) // Dodavanje
                {
                    if(KorisnickoImePostoji(korisnickoIme))
                    {
                        MessageBox.Show((string)Application.Current.Resources["Msg_Radnik_KorisnickoImePostoji"]);
                        return;
                    }
                    cmd = new MySqlCommand("INSERT INTO zaposleni (Ime, Prezime, KorisničkoIme, Lozinka, Uloga, BrojTelefona, Plata) " + 
                        "VALUES (@ime,@prezime,@korIme,@lozinka,@uloga, @brojTelefona, @plata)", conn);
                    cmd.Parameters.AddWithValue("@ime", ime);
                    cmd.Parameters.AddWithValue("@prezime", prezime);
                    cmd.Parameters.AddWithValue("@korIme", korisnickoIme);
                    cmd.Parameters.AddWithValue("@lozinka", lozinka);
                    cmd.Parameters.AddWithValue("@uloga", uloga);
                    cmd.Parameters.AddWithValue("@brojTelefona", brojTelefona);
                    cmd.Parameters.AddWithValue("@plata", plata);
                }
                else // Izmjena
                {
                    if (KorisnickoImePostoji(korisnickoIme))
                    {
                        MessageBox.Show((string)Application.Current.Resources["Msg_Radnik_KorisnickoImePostoji"]);
                        return;
                    }
                    if (string.IsNullOrEmpty(lozinka))
                    {
                        // Ako nije unesena nova lozinka, ne mijenjamo postojeću
                        cmd = new MySqlCommand("UPDATE zaposleni SET Ime=@ime, Prezime=@prezime, KorisničkoIme=@korIme, Uloga=@uloga, BrojTelefona=@brojTelefona, Plata=@plata WHERE IdZaposleni=@id", conn);
                    }
                    else
                    {
                        cmd = new MySqlCommand("UPDATE zaposleni SET Ime=@ime, Prezime=@prezime, KorisničkoIme=@korIme, Lozinka=@lozinka, Uloga=@uloga, BrojTelefona=@brojTelefona, Plata=@plata WHERE IdZaposleni=@id", conn);
                        cmd.Parameters.AddWithValue("@lozinka", lozinka);
                    }
                    cmd.Parameters.AddWithValue("@ime", ime);
                    cmd.Parameters.AddWithValue("@prezime", prezime);
                    cmd.Parameters.AddWithValue("@korIme", korisnickoIme);
                    cmd.Parameters.AddWithValue("@uloga", uloga);
                    cmd.Parameters.AddWithValue("@brojTelefona", brojTelefona);
                    cmd.Parameters.AddWithValue("@plata", plata);
                    cmd.Parameters.AddWithValue("@id", trenutniRadnik.Id);
                }

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show((string)Application.Current.Resources["Msg_Radnik_Sacuvan"]);
                    DialogResult = true;
                    Close();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    if (ex.Number == 1062)
                    {
                        MessageBox.Show((string)Application.Current.Resources["Msg_Radnik_KorisnickoImePostoji"]);
                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
