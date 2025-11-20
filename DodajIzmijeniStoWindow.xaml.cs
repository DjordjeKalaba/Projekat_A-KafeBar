using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Projekat_A_KafeBar
{
    public partial class DodajIzmijeniStoWindow : Window
    {
        private Sto trenutniSto = null;
        string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public DodajIzmijeniStoWindow()
        {
            InitializeComponent();
            TitleTextBlock.SetResourceReference(TextBlock.TextProperty, "Sto_Title_Add");
        }

        public DodajIzmijeniStoWindow(Sto sto)
        {
            InitializeComponent();
            trenutniSto = sto;
            TitleTextBlock.SetResourceReference(TextBlock.TextProperty, "Sto_Title_Edit");

            KapacitetBox.Text = sto.Kapacitet.ToString();

            foreach (ComboBoxItem item in StatusBox.Items)
            {
                if (item.Tag.ToString() == sto.Status)
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(KapacitetBox.Text.Trim(), out int kapacitet) || kapacitet <= 0)
            {
                MessageBox.Show((string)Application.Current.Resources["Sto_Msg_Kapacitet"],
                                (string)Application.Current.Resources["Sto_Msg_ErrorTitle"],
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string status = ((ComboBoxItem)StatusBox.SelectedItem).Tag.ToString();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd;

                if (trenutniSto == null)
                {
                    cmd = new MySqlCommand("INSERT INTO sto (Kapacitet, Status) VALUES (@kapacitet, @status)", conn);
                }
                else
                {
                    cmd = new MySqlCommand("UPDATE sto SET Kapacitet=@kapacitet, Status=@status WHERE IdSto=@id", conn);
                    cmd.Parameters.AddWithValue("@id", trenutniSto.IdSto);
                }

                cmd.Parameters.AddWithValue("@kapacitet", kapacitet);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show((string)Application.Current.Resources["Sto_Msg_Saved"],
                            (string)Application.Current.Resources["Sto_Msg_SuccessTitle"],
                            MessageBoxButton.OK, MessageBoxImage.Information);
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
