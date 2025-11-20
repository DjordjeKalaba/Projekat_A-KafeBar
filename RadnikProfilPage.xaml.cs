using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Projekat_A_KafeBar
{
    public partial class RadnikProfilPage : Page
    {
        private int radnikId;
        private Radnici parentWindow;
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public RadnikProfilPage(int idZaposleni, Radnici Parent)
        {
            InitializeComponent();
            radnikId = idZaposleni;
            parentWindow = Parent;
            LoadRadnik();
        }

        private string T(string key)
        {
            return (string)Application.Current.Resources[key];
        }

        private void LoadRadnik()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Ime, Prezime, KorisničkoIme, Lozinka, Uloga, BrojTelefona, Plata FROM zaposleni WHERE IdZaposleni=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", radnikId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ImeBox.Text = reader.GetString("Ime");
                        PrezimeBox.Text = reader.GetString("Prezime");
                        KorisnickoImeBox.Text = reader.GetString("KorisničkoIme");
                        LozinkaBox.Password = "";
                        BrojTelefonaBox.Text = reader.IsDBNull(reader.GetOrdinal("BrojTelefona")) ? "" : reader.GetString("BrojTelefona");
                        UlogaBox.Text = reader.GetString("Uloga");
                        PlataBox.Text = reader.GetDecimal("Plata").ToString("0.00");
                    }
                }
            }
        }

        private bool KorisnickoImePostoji(string korisnickoIme)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM zaposleni WHERE KorisničkoIme=@korIme AND IdZaposleni<>@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@korIme", korisnickoIme);
                cmd.Parameters.AddWithValue("@id", radnikId);

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string ime = ImeBox.Text.Trim();
            string prezime = PrezimeBox.Text.Trim();
            string korisnickoIme = KorisnickoImeBox.Text.Trim();
            string lozinka = LozinkaBox.Password.Trim();
            string brojTelefona = BrojTelefonaBox.Text.Trim();

            if (string.IsNullOrEmpty(ime) || string.IsNullOrEmpty(prezime) || string.IsNullOrEmpty(korisnickoIme))
            {
                MessageBox.Show(T("GreskaPraznaPolja"), T("Greska"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (KorisnickoImePostoji(korisnickoIme))
            {
                MessageBox.Show(T("GreskaKorimePostoji"), T("Greska"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd;

                if (string.IsNullOrEmpty(lozinka))
                {
                    cmd = new MySqlCommand(
                        "UPDATE zaposleni SET Ime=@ime, Prezime=@prezime, KorisničkoIme=@korIme, BrojTelefona=@brojTelefona WHERE IdZaposleni=@id",
                        conn
                    );
                }
                else
                {
                    cmd = new MySqlCommand(
                        "UPDATE zaposleni SET Ime=@ime, Prezime=@prezime, KorisničkoIme=@korIme, Lozinka=@lozinka, BrojTelefona=@brojTelefona WHERE IdZaposleni=@id",
                        conn
                    );
                    cmd.Parameters.AddWithValue("@lozinka", lozinka);
                }

                cmd.Parameters.AddWithValue("@ime", ime);
                cmd.Parameters.AddWithValue("@prezime", prezime);
                cmd.Parameters.AddWithValue("@korIme", korisnickoIme);
                cmd.Parameters.AddWithValue("@brojTelefona", brojTelefona);
                cmd.Parameters.AddWithValue("@id", radnikId);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show(T("UspjehSacuvano"), T("Uspjeh"), MessageBoxButton.OK, MessageBoxImage.Information);

            
            LoadRadnik();
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            LoadRadnik();
            MessageBox.Show(
                T("PonistenePromjene"),
                T("Info"),
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
