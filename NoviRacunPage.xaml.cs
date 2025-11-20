/*using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
   
    public partial class NoviRacunPage : Page
    {

        
        private int _zaposleniId;
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        private int? _odabraniStoId = null;

        public NoviRacunPage(int zaposleniId)
        {
            InitializeComponent();
            _zaposleniId = zaposleniId;
            VrijemeBox.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadZaposleni();
            LoadStolove();
        }

        private void LoadZaposleni()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT CONCAT(Ime, ' ', Prezime) AS ImePrezime FROM zaposleni WHERE IdZaposleni=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", _zaposleniId);

                ZaposleniBox.Text = cmd.ExecuteScalar()?.ToString() ?? "Nepoznato";
            }
        }

        private void LoadStolove()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT IdSto, CONCAT('Sto ', IdSto, ' (', Status, ')') AS Naziv FROM sto WHERE Status IN ('Slobodan', 'Rezervisan')";
                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                StoComboBox.ItemsSource = dt.DefaultView;
                StoComboBox.DisplayMemberPath = "Naziv";
                StoComboBox.SelectedValuePath = "IdSto";
            }
        }

        private void KreirajRacunButton_Click(object sender, RoutedEventArgs e)
        {
            if (StoComboBox.SelectedValue == null)
            {
                MessageBox.Show("Odaberite sto za koji se kreira račun!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int stoId = Convert.ToInt32(StoComboBox.SelectedValue);

            // Provjera statusa stola
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string statusQuery = "SELECT Status FROM sto WHERE IdSto=@id";
                MySqlCommand statusCmd = new MySqlCommand(statusQuery, conn);
                statusCmd.Parameters.AddWithValue("@id", stoId);
                string status = statusCmd.ExecuteScalar()?.ToString();

                if (status == "Rezervisan")
                {
                    var result = MessageBox.Show("Sto je rezervisan. Želite li kreirati račun i postaviti sto na zauzet?",
                                                 "Potvrda", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                        return;
                }

                // Kreiranje novog računa
                string insertRacun = "INSERT INTO račun (Iznos, VrijemeIzdavanja, Zaposleni_IdZaposleni, Sto_IdStola, Status) " +
                                     "VALUES (0, @vrijeme, @zaposleni, @sto, 'Otvoren')";
                MySqlCommand cmd = new MySqlCommand(insertRacun, conn);
                cmd.Parameters.AddWithValue("@vrijeme", DateTime.Now);
                cmd.Parameters.AddWithValue("@zaposleni", _zaposleniId);
                cmd.Parameters.AddWithValue("@sto", stoId);
                cmd.ExecuteNonQuery();

                int idRacun = (int)cmd.LastInsertedId;

                // Promjena statusa stola u Zauzet
                string updateSto = "UPDATE sto SET Status='Zauzet' WHERE IdSto=@id";
                MySqlCommand updateCmd = new MySqlCommand(updateSto, conn);
                updateCmd.Parameters.AddWithValue("@id", stoId);
                updateCmd.ExecuteNonQuery();

                _odabraniStoId = stoId;

                MessageBox.Show($"Račun uspješno kreiran! (ID: {idRacun})", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);

                // Otvaranje stranice za dodavanje stavki
                this.NavigationService.Navigate(new RacunDetaljiPage(idRacun, _zaposleniId));
            }
        }

        private void NazadButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
*/

using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Projekat_A_KafeBar
{
    public partial class NoviRacunPage : Page
    {
        private int _zaposleniId;
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private int? _odabraniStoId = null;

        public NoviRacunPage(int zaposleniId)
        {
            InitializeComponent();
            _zaposleniId = zaposleniId;
            VrijemeBox.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadZaposleni();
            LoadStolove();
        }

        private void LoadZaposleni()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT CONCAT(Ime, ' ', Prezime) AS ImePrezime FROM zaposleni WHERE IdZaposleni=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", _zaposleniId);

                ZaposleniBox.Text = cmd.ExecuteScalar()?.ToString() ??
                    Application.Current.Resources["NoviRacun_Nepoznato"].ToString();
            }
        }

        private void LoadStolove()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT IdSto, CONCAT('Sto ', IdSto, ' (', Status, ')') AS Naziv " +
                               "FROM sto WHERE Status IN ('Slobodan', 'Rezervisan')";
                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                StoComboBox.ItemsSource = dt.DefaultView;
                StoComboBox.DisplayMemberPath = "Naziv";
                StoComboBox.SelectedValuePath = "IdSto";
            }
        }

        private void KreirajRacunButton_Click(object sender, RoutedEventArgs e)
        {
            if (StoComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    Application.Current.Resources["NoviRacun_Msg_OdaberiSto"].ToString(),
                    Application.Current.Resources["NoviRacun_Msg_GreskaNaslov"].ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int stoId = Convert.ToInt32(StoComboBox.SelectedValue);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Provjera statusa stola
                string statusQuery = "SELECT Status FROM sto WHERE IdSto=@id";
                MySqlCommand statusCmd = new MySqlCommand(statusQuery, conn);
                statusCmd.Parameters.AddWithValue("@id", stoId);
                string status = statusCmd.ExecuteScalar()?.ToString();

                if (status == "Rezervisan")
                {
                    var result = MessageBox.Show(
                        Application.Current.Resources["NoviRacun_Msg_StoRezervisan"].ToString(),
                        Application.Current.Resources["NoviRacun_Msg_PotvrdaNaslov"].ToString(),
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                        return;
                }

                // Kreiranje novog računa
                string insertRacun = "INSERT INTO račun (Iznos, VrijemeIzdavanja, Zaposleni_IdZaposleni, Sto_IdStola, Status) " +
                                     "VALUES (0, @vrijeme, @zaposleni, @sto, 'Otvoren')";
                MySqlCommand cmd = new MySqlCommand(insertRacun, conn);
                cmd.Parameters.AddWithValue("@vrijeme", DateTime.Now);
                cmd.Parameters.AddWithValue("@zaposleni", _zaposleniId);
                cmd.Parameters.AddWithValue("@sto", stoId);
                cmd.ExecuteNonQuery();

                int idRacun = (int)cmd.LastInsertedId;

                // Promjena statusa stola u Zauzet
                string updateSto = "UPDATE sto SET Status='Zauzet' WHERE IdSto=@id";
                MySqlCommand updateCmd = new MySqlCommand(updateSto, conn);
                updateCmd.Parameters.AddWithValue("@id", stoId);
                updateCmd.ExecuteNonQuery();

                _odabraniStoId = stoId;

                MessageBox.Show(
                    string.Format(Application.Current.Resources["NoviRacun_Msg_RacunKreiran"].ToString(), idRacun),
                    Application.Current.Resources["NoviRacun_Msg_UspjehNaslov"].ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Otvaranje stranice za dodavanje stavki
                this.NavigationService.Navigate(new RacunDetaljiPage(idRacun, _zaposleniId));
            }
        }

        private void NazadButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
