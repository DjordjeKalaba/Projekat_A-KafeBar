using Org.BouncyCastle.Crypto;
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
    
    public partial class IzborStolaWindow : Window
    {
        public Sto IzabraniSto { get; private set; }

        public IzborStolaWindow(List<Sto> stolovi)
        {
            InitializeComponent();
            foreach (var sto in stolovi)
            {
                StoListBox.Items.Add(sto);
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (StoListBox.SelectedItem is Sto sto)
            {
                IzabraniSto = sto;
                DialogResult = true;
                Close();
            }
            else
            {
                //MessageBox.Show("Molimo izaberite sto.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                MessageBox.Show(
            (string)Application.Current.Resources["IzborStola_Msg_NijeIzabran"],
            (string)Application.Current.Resources["IzborStola_Msg_WarningTitle"],
            MessageBoxButton.OK,
            MessageBoxImage.Warning
        );
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
       
    }
}
