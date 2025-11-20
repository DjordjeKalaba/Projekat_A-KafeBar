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
using static Projekat_A_KafeBar.KategorijePage;

namespace Projekat_A_KafeBar
{
  
    public partial class DodajIzmijeniKategorijuWindow : Window
    {
        private Kategorija trenutnaKategorija = null;
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public DodajIzmijeniKategorijuWindow()
        {
            InitializeComponent();
            TitleTextBlock.SetResourceReference(TextBlock.TextProperty, "Kategorija_Title_Add");
        }

        public DodajIzmijeniKategorijuWindow(Kategorija kat)
        {
            InitializeComponent();
            trenutnaKategorija = kat;
            TitleTextBlock.SetResourceReference(TextBlock.TextProperty, "Kategorija_Title_Edit");
            NazivBox.Text = kat.Naziv;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string naziv = NazivBox.Text.Trim();

            if (string.IsNullOrEmpty(naziv))
            {
                MessageBox.Show((string)Application.Current.Resources["Msg_Kategorija_Prazno"]);
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd;

                if (trenutnaKategorija == null)
                {
                    cmd = new MySqlCommand("INSERT INTO kategorija (Naziv) VALUES (@naziv)", conn);
                }
                else
                {
                    cmd = new MySqlCommand("UPDATE kategorija SET Naziv=@naziv WHERE IdKategorije=@id", conn);
                    cmd.Parameters.AddWithValue("@id", trenutnaKategorija.Id);
                }

                cmd.Parameters.AddWithValue("@naziv", naziv);
                

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062) // DUPLICATE KEY
                    {
                        string msg = (string)Application.Current.Resources["Msg_Kategorija_Duplikat"];
                        MessageBox.Show(msg);
                        return;
                    }

                    MessageBox.Show("Greška: " + ex.Message);
                    return;
                }
            }

            MessageBox.Show((string)Application.Current.Resources["Msg_Kategorija_Sacuvana"]);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
