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

namespace Benutzerverwaltung
{
    /// <summary>
    /// Interaktionslogik für PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        MainWindow parent;
        User user;
        DataBaseConnection dbc;
        public PaymentWindow(MainWindow _parent, User _user, DataBaseConnection _dbc)
        {
            InitializeComponent();
            this.parent = _parent;
            this.user = _user;
            this.dbc = _dbc;
            MainGrid.ShowGridLines = Settings.Default.ShowGridLines;
            MainGrid.RowDefinitions.Clear();
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            this.Height = 4 * Settings.Default.HeightShort + 10;
            FontFamily ff = new FontFamily(Settings.Default.FontFamily);
            BTBuchen.FontFamily = ff;
            BTBuchen.FontSize = Settings.Default.FontSize;
            BTAbbrechen.FontFamily = ff;
            BTAbbrechen.FontSize = Settings.Default.FontSize;
            TBBetrag.FontFamily = ff;
            TBBetrag.FontSize = Settings.Default.FontSize;
            TBBuchen.FontFamily = ff;
            TBBuchen.FontSize= Settings.Default.FontSize;
            TBUser.FontFamily = ff;
            TBUser.FontSize = Settings.Default.FontSize;
            TBUser.Text = string.Format("Betrag buchen für {0} {1}", user.Vorname, user.Name);
        }

        private void ClickCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClickBook(object sender, RoutedEventArgs e)
        {
            var book = Convert.StringToDecimal(TBBetrag.Text);
            var newval = user.Bezahlt + book;
            var err = dbc.CommandNonQuery(string.Format("UPDATE Benutzer SET Bezahlt='{0}' WHERE BID={1}", newval.ToString(ConfigWindow.nfi), user.Id));
            if(err is not null)
            {
                ErrorLogging.Log(err);
                MessageBox.Show("Buchen nicht erfolgreich", "Buchen", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Buchen erfolgreich", "Buchen", MessageBoxButton.OK, MessageBoxImage.Information);
                parent.Reload();
            }
            this.Close();
        }
    }
}
