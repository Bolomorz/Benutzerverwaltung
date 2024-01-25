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
using static Benutzerverwaltung.MainWindow;

namespace Benutzerverwaltung
{
    /// <summary>
    /// Interaktionslogik für ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        MainWindow parent;
        int id;
        View view;
        Mode mode;
        DataBaseConnection dbc;
        Grid GridStatics;
        Grid GridVariables;
        List<Static> statics;
        List<Variable> variables;
        User? user;

        List<TextBox> boxes;

        public ConfigWindow(MainWindow _parent, int _id, View _view, Mode _mode, DataBaseConnection _dbc, List<Static> _statics, List<Variable> _variables, User? _user)
        {
            InitializeComponent();
            this.parent = _parent;
            this.view = _view;
            this.mode = _mode;
            this.id = _id;
            this.boxes = new List<TextBox>();
            this.dbc = _dbc;
            this.GridStatics = new Grid();
            this.GridVariables = new Grid();
            this.statics = _statics;
            this.variables = _variables;
            this.user = _user;
        }

        private void CreateControls()
        {

        }

        private void CCBenutzer()
        {
            this.boxes.Clear();
            ControlsGrid.Children.Clear();
            ControlsGrid.RowDefinitions.Clear();

            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //0Name
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //1Vorname
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //2Strasse
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //3PLZ
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //4Ort
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //5Geburtsdatum
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //6Eintrittsdatum
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(150, GridUnitType.Pixel) });      //7Statics
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(150, GridUnitType.Pixel) });      //8Variables
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //9AcceptButton

            CreateTextBlock(ControlsGrid, "Name", 0, 0); CreateTextBlock(ControlsGrid, "Vorname", 0, 1); CreateTextBlock(ControlsGrid, "Strasse", 0, 2); 
            CreateTextBlock(ControlsGrid, "PLZ", 0, 3); CreateTextBlock(ControlsGrid, "Ort", 0, 4);
            CreateTextBlock(ControlsGrid, "Geburtsdatum (YYYYMMDD)", 0, 5); CreateTextBlock(ControlsGrid, "Eintrittsdatum (YYYYMMDD)", 0, 6); 
            CreateTextBlock(ControlsGrid, "Statische Posten", 0, 7); CreateTextBlock(ControlsGrid, "Variable Posten", 0, 8); CreateButton("Annehmen", 0, 9);
            if(user is null)
            {

            }
            else
            {

            }
        }
        private void CCBenutzerStatics()
        {
            GridStatics.Children.Clear();
            GridStatics.ColumnDefinitions.Clear();
            GridStatics.RowDefinitions.Clear();

            GridStatics.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });         //ID
            GridStatics.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });         //Name
            GridStatics.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });         //Wert
            GridStatics.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });         //Checkbox

            for(int i = 0; i < statics.Count + 2; i++) GridStatics.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });

            CreateTextBlock(GridStatics, "ID", 0, 0); CreateTextBlock(GridStatics, "Name", 1, 0); CreateTextBlock(GridStatics, "Wert", 2, 0); CreateTextBlock(GridStatics, "Aktivieren", 3, 0);
            int row = 1;
            foreach(var s in statics)
            {
                CreateTextBlock(GridStatics, s.Id.ToString(), 0, row);
                CreateTextBlock(GridStatics, s.Name, 1, row);
                CreateTextBlock(GridStatics, s.Wert.ToString(), 2, row);
                row++;
            }
        }

        private void CreateSubGrid(Grid grid, int column, int row)
        {
            ScrollViewer sv = new ScrollViewer()
            {
                Margin = new Thickness(5),
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                Content = grid
            };
            Border bd = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = Brushes.Gainsboro,
                Margin = new Thickness(5),
                Child = sv
            };
            Grid.SetColumn(bd, column);
            Grid.SetRow(bd, row);
            ControlsGrid.Children.Add(bd);
        }
        private void CreateButton(string text, int column, int row)
        {
            Button bt = new Button()
            {
                Margin = new Thickness(5),
                Content = text,
                Background = (mode == Mode.CreateNew) ? CreateBackground : AdminBackground,
                Foreground = (mode == Mode.CreateNew) ? CreateForeground : AdminForeground
            };
            bt.Click += ClickAccept;
            Border bd = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = (mode == Mode.CreateNew) ? CreateForeground : AdminForeground,
                Background = (mode == Mode.CreateNew) ? CreateBackground : AdminBackground,
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = bt
            };
            Grid.SetColumn(bd, column);
            Grid.SetRow(bd, row);
            ControlsGrid.Children.Add(bd);
        }
        private void CreateTextBlock(Grid grid, string text, int column, int row)
        {
            TextBlock tb = new TextBlock()
            {
                Margin = new Thickness(5),
                Text = text
            };
            Border bd = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = Brushes.Gainsboro,
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = tb
            };
            Grid.SetColumn(bd, column);
            Grid.SetRow(bd, row);
            grid.Children.Add(bd);
        }
        private void CreateTextBox(Grid grid, string text, string name, int column, int row) 
        {
            TextBox tb = new TextBox()
            {
                Margin = new Thickness(5),
                Text = text,
                Name = name
            };
            Border bd = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = Brushes.Gainsboro,
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = tb
            };
            Grid.SetColumn(bd, column);
            Grid.SetRow(bd, row);
            grid.Children.Add(bd);
            boxes.Add(tb);
        }
        private void CreateCheckBox(Grid grid, string text, string name, bool ischecked, int column, int row)
        {
            CheckBox cb = new CheckBox()
            {
                Margin = new Thickness(5),
                Content = text,
                Name = name,
                IsChecked = ischecked
            };
            Border bd = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = Brushes.Gainsboro,
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = cb
            };
            Grid.SetColumn(bd, column);
            Grid.SetRow(bd, row);
            grid.Children.Add(bd);
        }
        private string? SearchTextBoxes(string name)
        {
            foreach(var tb in boxes)
            {
                if(tb.Name == name) return tb.Text;
            }
            return null;
        }

        private void ClickCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ClickAccept(object sender, RoutedEventArgs e)
        {

        }

    }
}
