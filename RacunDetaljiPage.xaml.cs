using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Projekat_A_KafeBar
{
    public partial class RacunDetaljiPage : Page
    {
        private int _racunId;
        private int _zaposleniId;
        private bool _readOnly;
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private List<StavkaRacuna> stavke = new List<StavkaRacuna>();

        public RacunDetaljiPage(int racunId, int zaposleniId)
        {
            InitializeComponent();
            _racunId = racunId;
            _zaposleniId = zaposleniId;
            _readOnly = false;
            UcitajOsnovno();
        }

        public RacunDetaljiPage(int racunId, bool readOnly)
        {
            InitializeComponent();
            _racunId = racunId;
            _readOnly = readOnly;
            UcitajOsnovno();
            if (readOnly) PostaviReadOnlyMod();
        }

        private void UcitajOsnovno()
        {
            RacunIdBox.Text = _racunId.ToString();
            LoadZaposleni();
            LoadKategorije();
            LoadStavke();
            ProvjeriStatusUBazi();
        }

        private void LoadZaposleni()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            string query = @"SELECT CONCAT(z.Ime, ' ', z.Prezime) FROM račun r
                             JOIN zaposleni z ON r.Zaposleni_IdZaposleni = z.IdZaposleni
                             WHERE r.IdRačuna=@id";
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", _racunId);
            ZaposleniBox.Text = cmd.ExecuteScalar()?.ToString();
        }

        private void LoadKategorije()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            string query = "SELECT IdKategorije, Naziv FROM kategorija";
            var da = new MySqlDataAdapter(query, conn);
            var dt = new DataTable();
            da.Fill(dt);
            KategorijaComboBox.ItemsSource = dt.DefaultView;
        }

        private void KategorijaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KategorijaComboBox.SelectedValue != null)
                LoadProizvodi(Convert.ToInt32(KategorijaComboBox.SelectedValue));
        }

        private bool ImaDovoljnoNaStanju(int proizvodId, int kolicina)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT Količina FROM proizvod WHERE IdProizvoda = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", proizvodId);

            object result = cmd.ExecuteScalar();

            if (result == null) return false;

            int stanje = Convert.ToInt32(result);
            return stanje >= kolicina;
        }

        private void SmanjiKolicinuNaStanju(int proizvodId, int kolicina)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string update = @"UPDATE proizvod 
                      SET Količina = Količina - @kol 
                      WHERE IdProizvoda = @id";

            using var cmd = new MySqlCommand(update, conn);
            cmd.Parameters.AddWithValue("@kol", kolicina);
            cmd.Parameters.AddWithValue("@id", proizvodId);

            cmd.ExecuteNonQuery();
        }


        private void LoadProizvodi(int kategorijaId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            string query = "SELECT IdProizvoda, Naziv, Cijena FROM proizvod WHERE Kategorija_IdKategorije=@id";
            var da = new MySqlDataAdapter(query, conn);
            da.SelectCommand.Parameters.AddWithValue("@id", kategorijaId);
            var dt = new DataTable();
            da.Fill(dt);
            ProizvodComboBox.ItemsSource = dt.DefaultView;
        }

        private void LoadStavke()
        {
            stavke.Clear();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            string query = @"SELECT p.Naziv, s.Količina AS Kolicina, s.Cijena, s.Ukupno
                             FROM stavkaračuna s
                             JOIN proizvod p ON s.Proizvod_IdProizvoda = p.IdProizvoda
                             WHERE s.Račun_IdRačuna=@id";
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", _racunId);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                stavke.Add(new StavkaRacuna
                {
                    Naziv = r.GetString("Naziv"),
                    Cijena = r.GetDecimal("Cijena"),
                    Kolicina = r.GetInt32("Kolicina"),
                    Ukupno = r.GetDecimal("Ukupno")
                });
            }
            StavkeDataGrid.ItemsSource = null;
            StavkeDataGrid.ItemsSource = stavke;
            UkupanIznosBox.Text = $"{IzracunajUkupno():0.00} KM";
        }

        private decimal IzracunajUkupno()
        {
            decimal suma = 0;
            foreach (var s in stavke) suma += s.Ukupno;
            return suma;
        }

        private void ProvjeriStatusUBazi()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            string query = "SELECT Status FROM račun WHERE IdRačuna=@id";
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", _racunId);
            string status = cmd.ExecuteScalar()?.ToString();
            if (status == "Zatvoren") PostaviReadOnlyMod();
        }

        private void PostaviReadOnlyMod()
        {
            _readOnly = true;
            DodajButton.IsEnabled = false;
            SacuvajRacunButton.IsEnabled = false;
            ZavrsiRacunButton.IsEnabled = false;
            KolicinaBox.IsEnabled = false;
            ProizvodComboBox.IsEnabled = false;
            KategorijaComboBox.IsEnabled = false;
        }

        

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            if (_readOnly) return;

            // localized messages
            string msgSelect = (string)Application.Current.Resources["SelectProductAndQuantity"];
            string msgNoStock = (string)Application.Current.Resources["NotEnoughStock"];
            string msgWarning = (string)Application.Current.Resources["Warning"];

            if (ProizvodComboBox.SelectedValue == null || string.IsNullOrWhiteSpace(KolicinaBox.Text))
            {
                MessageBox.Show(msgSelect, msgWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int proizvodId = Convert.ToInt32(ProizvodComboBox.SelectedValue);
            int kolicina = int.Parse(KolicinaBox.Text);

            // 1. PROVJERA STANJA
            if (!ImaDovoljnoNaStanju(proizvodId, kolicina))
            {
                MessageBox.Show(msgNoStock, msgWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            // 2. PROVJERA DA LI VEĆ POSTOJI NA RAČUNU
            string provjera = "SELECT COUNT(*) FROM stavkaračuna WHERE Račun_IdRačuna=@racun AND Proizvod_IdProizvoda=@proizvod";
            var pCmd = new MySqlCommand(provjera, conn);
            pCmd.Parameters.AddWithValue("@racun", _racunId);
            pCmd.Parameters.AddWithValue("@proizvod", proizvodId);

            bool postoji = Convert.ToInt32(pCmd.ExecuteScalar()) > 0;

            
            if (postoji)
            {
                string updateKol = @"
                    UPDATE stavkaračuna 
                    SET Količina = Količina + @kol
                    WHERE Račun_IdRačuna=@racun AND Proizvod_IdProizvoda=@proizvod";

                var cmd1 = new MySqlCommand(updateKol, conn);
                cmd1.Parameters.AddWithValue("@kol", kolicina);
                cmd1.Parameters.AddWithValue("@racun", _racunId);
                cmd1.Parameters.AddWithValue("@proizvod", proizvodId);
                cmd1.ExecuteNonQuery();

                string updateUk = @"
                    UPDATE stavkaračuna
                    SET Ukupno = Količina * Cijena
                    WHERE Račun_IdRačuna=@racun AND Proizvod_IdProizvoda=@proizvod";

                var cmd2 = new MySqlCommand(updateUk, conn);
                cmd2.Parameters.AddWithValue("@racun", _racunId);
                cmd2.Parameters.AddWithValue("@proizvod", proizvodId);
                cmd2.ExecuteNonQuery();
            }

            else
            {
                string insert = @"INSERT INTO stavkaračuna (Količina, Cijena, Ukupno, Račun_IdRačuna, Proizvod_IdProizvoda)
                          SELECT @kol, Cijena, @kol * Cijena, @racun, IdProizvoda
                          FROM proizvod WHERE IdProizvoda=@proizvod";

                var cmd = new MySqlCommand(insert, conn);
                cmd.Parameters.AddWithValue("@kol", kolicina);
                cmd.Parameters.AddWithValue("@racun", _racunId);
                cmd.Parameters.AddWithValue("@proizvod", proizvodId);
                cmd.ExecuteNonQuery();
            }

            // 3. SMANJI KOLIČINU U BAZI
            SmanjiKolicinuNaStanju(proizvodId, kolicina);

            // 4. OSVJEŽI TABELU
            LoadStavke();
        }


        private void SacuvajRacun_Click(object sender, RoutedEventArgs e)
        {
            if (_readOnly) return;
            decimal ukupno = IzracunajUkupno();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            string query = "UPDATE račun SET Iznos=@iznos WHERE IdRačuna=@id";
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@iznos", ukupno);
            cmd.Parameters.AddWithValue("@id", _racunId);
            cmd.ExecuteNonQuery();
            string msg = (string)Application.Current.Resources["Msg_RacunSacuvan"];
            string title = (string)Application.Current.Resources["Msg_RacunSacuvan_Title"];

            MessageBox.Show(msg, title, MessageBoxButton.OK);
        }

        private void ZavrsiRacun_Click(object sender, RoutedEventArgs e)
        {
            if (_readOnly) return;
            SacuvajRacun_Click(sender, e);

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            int stoId = 0;
            string querySto = "SELECT Sto_IdStola FROM račun WHERE IdRačuna=@id";
            var cmdSto = new MySqlCommand(querySto, conn);
            cmdSto.Parameters.AddWithValue("@id", _racunId);
            var result = cmdSto.ExecuteScalar();
            if (result != null) stoId = Convert.ToInt32(result);

            string queryZavrsi = "UPDATE račun SET Status='Zatvoren' WHERE IdRačuna=@id";
            var cmdZavrsi = new MySqlCommand(queryZavrsi, conn);
            cmdZavrsi.Parameters.AddWithValue("@id", _racunId);
            cmdZavrsi.ExecuteNonQuery();

            if (stoId > 0)
            {
                string queryStoUpdate = "UPDATE sto SET Status='Slobodan' WHERE IdSto=@stoId";
                var cmdStoUpdate = new MySqlCommand(queryStoUpdate, conn);
                cmdStoUpdate.Parameters.AddWithValue("@stoId", stoId);
                cmdStoUpdate.ExecuteNonQuery();
            }

            string msg = (string)Application.Current.Resources["Msg_RacunZatvoren"];
            string title = (string)Application.Current.Resources["Msg_RacunZatvoren_Title"];

            MessageBox.Show(msg, title, MessageBoxButton.OK);

            NavigationService.GoBack();
        }

        private void Nazad_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        public void UpdateDataGridHeaders()
        {
            ColProizvod.Header = Application.Current.FindResource("RacunDetalji_Col_Proizvod")?.ToString();
            ColCijena.Header = Application.Current.FindResource("RacunDetalji_Col_Cijena")?.ToString();
            ColKolicina.Header = Application.Current.FindResource("RacunDetalji_Col_Kolicina")?.ToString();
            ColUkupno.Header = Application.Current.FindResource("RacunDetalji_Col_Ukupno")?.ToString();

            NaslovTextBlock.Text = Application.Current.FindResource("RacunDetalji_Title")?.ToString();
            RacunIDTextBlock.Text = Application.Current.FindResource("RacunDetalji_Label_RacunID")?.ToString();
            ZaposleniTextBlock.Text = Application.Current.FindResource("RacunDetalji_Label_Zaposleni")?.ToString();
            UkupanIznosTextBlock.Text = Application.Current.FindResource("RacunDetalji_Label_UkupanIznos")?.ToString();
            KategorijaTextBlock.Text = Application.Current.FindResource("RacunDetalji_Label_Kategorija")?.ToString();
            ProizvodTextBlock.Text = Application.Current.FindResource("RacunDetalji_Label_Proizvod")?.ToString();
            KolicinaTextBlock.Text = Application.Current.FindResource("RacunDetalji_Label_Kolicina")?.ToString();
            DodajButton.Content = Application.Current.FindResource("RacunDetalji_Button_Dodaj")?.ToString();
            SacuvajRacunButton.Content = Application.Current.FindResource("RacunDetalji_Button_Sacuvaj")?.ToString();
            ZavrsiRacunButton.Content = Application.Current.FindResource("RacunDetalji_Button_Zavrsi")?.ToString();
            Nazad.Content = Application.Current.FindResource("RacunDetalji_Button_Nazad")?.ToString();
        }
    }

    public class StavkaRacuna
    {
        public string Naziv { get; set; }
        public decimal Cijena { get; set; }
        public int Kolicina { get; set; }
        public decimal Ukupno { get; set; }
    }
}
