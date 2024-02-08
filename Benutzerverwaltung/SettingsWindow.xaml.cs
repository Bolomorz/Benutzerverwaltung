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
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(MainWindow _parent)
        {
            InitializeComponent();
            MainGrid.RowDefinitions.Clear();
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Settings.Default.HeightShort, GridUnitType.Pixel) });
            this.Height = (MainGrid.RowDefinitions.Count+1) * Settings.Default.HeightShort;
            FontFamily ff = new FontFamily(Settings.Default.FontFamily);
            BTAccept.FontFamily = ff;
            BTAccept.FontSize = Settings.Default.FontSize;
            BTCancel.FontSize = Settings.Default.FontSize;
            BTCancel.FontFamily = ff;
            CBGrid.FontFamily = ff;
            CBGrid.FontSize = Settings.Default.FontSize;
            TBFont.FontFamily = ff;
            TBFont.FontSize = Settings.Default.FontSize;
            TBHL.FontFamily = ff;
            TBHL.FontSize = Settings.Default.FontSize;
            TBHM.FontFamily = ff;
            TBHM.FontSize = Settings.Default.FontSize;
            TBHS.FontFamily = ff;
            TBHS.FontSize = Settings.Default.FontSize;
            TBSize.FontFamily = ff;
            TBSize.FontSize = Settings.Default.FontSize;
            TBWL.FontFamily = ff;
            TBWL.FontSize = Settings.Default.FontSize;
            TBWM.FontFamily = ff;
            TBWM.FontSize = Settings.Default.FontSize;
            TBWS.FontFamily = ff;
            TBWS.FontSize = Settings.Default.FontSize;

            CBGrid.IsChecked = Settings.Default.ShowGridLines;
            TBFont.Text = Settings.Default.FontFamily;
            TBSize.Text = Settings.Default.FontSize.ToString();
            TBWS.Text = Settings.Default.WidthShort.ToString();
            TBWM.Text = Settings.Default.WidthMedium.ToString();
            TBWL.Text = Settings.Default.WidthLong.ToString();
            TBHS.Text = Settings.Default.HeightShort.ToString();
            TBHM.Text = Settings.Default.HeightMedium.ToString();
            TBHL.Text = Settings.Default.HeightLong.ToString();
        }

        private void ClickAccept(object sender, RoutedEventArgs e)
        {
            Settings.Default.FontFamily = TBFont.Text;
            Settings.Default.FontSize = Convert.StringToDouble(TBSize.Text);
            Settings.Default.ShowGridLines = (CBGrid.IsChecked is not null) ? (bool)CBGrid.IsChecked : false;
            Settings.Default.WidthShort = Convert.StringToInt(TBWS.Text);
            Settings.Default.WidthMedium = Convert.StringToInt(TBWM.Text);
            Settings.Default.WidthLong = Convert.StringToInt(TBWL.Text);
            Settings.Default.HeightShort = Convert.StringToInt(TBHS.Text);
            Settings.Default.HeightMedium = Convert.StringToInt(TBHM.Text);
            Settings.Default.HeightLong = Convert.StringToInt(TBHL.Text);
            Settings.Default.Save();
            this.Close();
        }

        private void ClickCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
