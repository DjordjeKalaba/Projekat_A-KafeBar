using MySql.Data.MySqlClient;
using System.Configuration;
using System.Text;
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
    public partial class MainWindow : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        // Globalna statička varijabla za trenutno prijavljenog korisnika
        public static int TrenutniKorisnikId { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Prijava_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Unesite korisničko ime i lozinku!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT IdZaposleni, Uloga FROM zaposleni WHERE KorisničkoIme=@username AND Lozinka=@password";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int idZaposleni = reader.GetInt32("IdZaposleni");
                                string uloga = reader.GetString("Uloga");

                                // Postavi globalni ID trenutno prijavljenog korisnika
                                TrenutniKorisnikId = idZaposleni;


                                /*
                                // Dohvati temu korisnika iz baze
                                string tema = DohvatiTemuZaKorisnika(TrenutniKorisnikId);

                                // Primjeni temu odmah
                                App.ChangeTheme(tema);
                                */

                                // Dohvati i primijeni temu i jezik prije otvaranja glavnog prozora
                                ApplyUserPreferences(idZaposleni);

                                if (uloga.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                                {
                                    //MainWindow.TrenutniKorisnikId = idZaposleni;
                                    Admin adminWindow = new Admin();
                                    adminWindow.Show();
                                    this.Close();
                                }
                                else if (uloga.Equals("Radnik", StringComparison.OrdinalIgnoreCase))
                                {
                                    //MainWindow.TrenutniKorisnikId = idZaposleni;
                                    Radnici radnikWindow = new Radnici(idZaposleni);
                                    radnikWindow.Show();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Nepoznata uloga korisnika.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Neispravni kredencijali. Pokušajte ponovo.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri konekciji na bazu: " + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /*
        // Metoda za dohvat teme iz baze za trenutno prijavljenog korisnika
        private string DohvatiTemuZaKorisnika(int korisnikId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Tema FROM zaposleni WHERE IdZaposleni=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", korisnikId);
                var result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "OrangeTheme"; // default tema
            }
        }*/

        private void ApplyUserPreferences(int korisnikId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Tema, Jezik FROM zaposleni WHERE IdZaposleni=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", korisnikId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var temaObj = reader["Tema"];
                        var jezikObj = reader["Jezik"];

                        string tema = temaObj != DBNull.Value ? temaObj.ToString() : "OrangeTheme";
                        string jezik = jezikObj != DBNull.Value ? jezikObj.ToString() : "Srpski";

                        // Primijeni odmah
                        App.ChangeTheme(tema);
                        App.ChangeLanguage(jezik);
                    }
                }
            }
        }
    }
}