using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        List<User> users;
        User? user;

        List<TextBox> boxes;
        List<CheckBox> checkboxes;

        static NumberFormatInfo nfi = new() { NumberDecimalSeparator = "." };

        public ConfigWindow(MainWindow _parent, int _id, View _view, Mode _mode, DataBaseConnection _dbc, List<Static> _statics, List<Variable> _variables, List<User> _users, User? _user)
        {
            InitializeComponent();
            this.parent = _parent;
            this.view = _view;
            this.mode = _mode;
            this.id = _id;
            this.boxes = new List<TextBox>();
            this.checkboxes = new List<CheckBox>();
            this.dbc = _dbc;
            this.GridStatics = new Grid();
            this.GridVariables = new Grid();
            this.statics = _statics;
            this.variables = _variables;
            this.users = _users;
            this.user = _user;
            CreateControls();
        }

        private static DataBaseConnection.Date StringToDate(string input)
        {
            try
            {
                return new DataBaseConnection.Date(input);
            }
            catch (Exception e)
            {
                ErrorLogging.Log(e.ToString());
                return new DataBaseConnection.Date() { day = 77, month = 77, year = 7777 };
            }
        }
        private static decimal StringToDecimal(string input)
        {
            string newval = "";

            foreach (var c in input)
            {
                if (IsNumeric(c))
                {
                    newval += c;
                }
                else if (c == ',' || c == '.')
                {
                    newval += System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                }
            }

            if (newval != "")
            {
                return decimal.Parse(newval, System.Globalization.NumberStyles.AllowDecimalPoint);
            }
            else
            {
                return 0;
            }
        }
        private static int StringToInt(string input)
        {
            string output = "";
            foreach(var c in input) if(IsNumeric(c)) output += c;
            return int.Parse(output);
        }
        public static bool IsNumeric(char input)
        {
            if (input == '0' || input == '1' || input == '2' || input == '3' || input == '4' || input == '5' || input == '6' || input == '7' || input == '8' || input == '9') return true; else return false;
        }

        private void CreateControls()
        {
            switch (this.view)
            {
                case View.Benutzer:
                    CCBenutzer(); break;
                case View.StatischePosten:
                    if (this.mode == Mode.Administrate)
                    {
                        var query = dbc.CommandQueryToList(string.Format("SELECT * FROM StatischeRechnungsPosten WHERE SRPID={0};", id));
                        if (query.error is not null) ErrorLogging.Log(query.error.ToString());
                        else if (query.solution is not null && query.solution.Count > 0)
                        {
                            var s = query.solution[0];
                            var newstatic = new Static((int)s[0], (string)s[1], (decimal)s[2], (bool)s[3]);
                            CCStatic(newstatic);
                        }
                    }
                    else
                    {
                        CCStatic(null);
                    }
                    break;
                case View.VariablePosten:
                    if(this.mode == Mode.Administrate)
                    {
                        var query = dbc.CommandQueryToList(string.Format("SELECT * FROM VariableRechnungsPosten WHERE VRPID={0};", id));
                        if (query.error is not null) ErrorLogging.Log(query.error.ToString());
                        else if (query.solution is not null && query.solution.Count > 0)
                        {
                            var v = query.solution[0];
                            var newvariable = new Variable((int)v[0], (string)v[1], (string)v[2], (decimal)v[3]);
                            CCVariable(newvariable);
                        }
                    }
                    else
                    {
                        CCVariable(null);
                    }
                    break;
                case View.Jubilaeen:
                    if(mode == Mode.Administrate)
                    {
                        var query = dbc.CommandQueryToList(string.Format("SELECT * FROM Jubilaeum WHERE JID={0};", id));
                        if (query.error is not null) ErrorLogging.Log(query.error.ToString());
                        else if (query.solution is not null && query.solution.Count > 0)
                        {
                            var j = query.solution[0];
                            var newjub = new Jubilaeum((int)j[0], (int)j[1]);
                            CCJub(newjub);
                        }
                    }
                    else
                    {
                        CCJub(null);
                    }
                    break;
                case View.GesamtAktuell:
                    CCBenutzer(); break;
            }
        }

        private void CCJub(Jubilaeum? j)
        {
            this.boxes.Clear();
            this.checkboxes.Clear();
            ControlsGrid.Children.Clear();
            ControlsGrid.RowDefinitions.Clear();

            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //0Wert
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });

            CreateTextBlock(ControlsGrid, "Jahre", 0, 0);

            if(j is null)
            {
                CreateTextBox(ControlsGrid, "", "tbjubwert", 1, 0);
                TBView.Text = "Neues Jubiläum";
                CreateButton("Jubiläum erstellen", 0, 1);
            }
            else
            {
                CreateTextBox(ControlsGrid, j.Jahre.ToString(), "tbjubwert", 1, 0);
                TBView.Text = "Jubiläum ändern";
                CreateButton("Jubiläum ändern", 0, 1);
            }
        }
        private void CCStatic(Static? s)
        {
            this.boxes.Clear();
            this.checkboxes.Clear();
            ControlsGrid.Children.Clear();
            ControlsGrid.RowDefinitions.Clear();

            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //0Name
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //1Wert
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //2Default
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });

            CreateTextBlock(ControlsGrid, "Name", 0, 0);
            CreateTextBlock(ControlsGrid, "Wert", 0, 1);
            CreateTextBlock(ControlsGrid, "Standartmäßig aktiviert?", 0, 2);

            if(s is null)
            {
                CreateTextBox(ControlsGrid, "", "tbstaticname", 1, 0);
                CreateTextBox(ControlsGrid, "", "tbstaticwert", 1, 1);
                CreateCheckBox(ControlsGrid, "Aktiviert", "cbstaticdefault", false, 1, 2);
                TBView.Text = "Neuer Statischer Posten";
                CreateButton("Statischen Posten erstellen", 0, 3);
            }
            else
            {
                CreateTextBox(ControlsGrid, s.Name, "tbstaticname", 1, 0);
                CreateTextBox(ControlsGrid, s.Wert.ToString(), "tbstaticwert", 1, 1);
                CreateCheckBox(ControlsGrid, "Aktiviert", "cbstaticdefault", s.Default, 1, 2);
                TBView.Text = "Statischen Posten ändern";
                CreateButton("Statischen Posten ändern", 0, 3);
            }
        }
        private void CCVariable(Variable? v)
        {
            this.boxes.Clear();
            this.checkboxes.Clear();
            ControlsGrid.Children.Clear();
            ControlsGrid.RowDefinitions.Clear();

            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //0Name
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //1Formel
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });       //2Default
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100, GridUnitType.Pixel) });
            ControlsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });

            CreateTextBlock(ControlsGrid, "Name", 0, 0);
            CreateTextBlock(ControlsGrid, "Formel", 0, 1);
            CreateTextBlock(ControlsGrid, "Standart Wert", 0, 2);
            CreateTextBlock(ControlsGrid, "Information zur Formel", 0, 3);
            CreateTextBlock(ControlsGrid, "W ist Platzhalter für den Wert, der in die Formel eingesetzt wird.\n" +
                                            "Zu verwendete Zeichen: (,),+,-,/,*,Zahlen\n" +
                                            "Falls nichts berechnet werden soll -> Formel ist 'W'", 1, 3);
            if(v is null)
            {
                CreateTextBox(ControlsGrid, "", "tbvariablename", 1, 0);
                CreateTextBox(ControlsGrid, "W", "tbvariableformel", 1, 1);
                CreateTextBox(ControlsGrid, "", "tbvariabledefault", 1, 2);
                TBView.Text = "Neuer Variabler Posten";
                CreateButton("Variablen Posten erstellen", 0, 4);
            }
            else
            {
                CreateTextBox(ControlsGrid, v.Name, "tbvariablename", 1, 0);
                CreateTextBox(ControlsGrid, v.Formel, "tbvariableformel", 1, 1);
                CreateTextBox(ControlsGrid, v.Default.ToString(), "tbvariabledefault", 1, 2);
                TBView.Text = "Variablen Posten ändern";
                CreateButton("Variablen Posten ändern", 0, 4);
            }
        }
        private void CCBenutzer()
        {
            this.boxes.Clear();
            this.checkboxes.Clear();
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
            CreateTextBlock(ControlsGrid, "Geburtsdatum", 0, 5, "DatumFormat (DD.MM.YYYY oder YYYY.MM.DD)"); CreateTextBlock(ControlsGrid, "Eintrittsdatum", 0, 6, "DatumFormat (DD.MM.YYYY oder YYYY.MM.DD)"); 
            CreateTextBlock(ControlsGrid, "Statische Posten", 0, 7); CreateTextBlock(ControlsGrid, "Variable Posten", 0, 8); CreateButton("Annehmen", 0, 9);
            if(user is null)
            {
                CreateTextBox(ControlsGrid, "", "tbusername", 1, 0);
                CreateTextBox(ControlsGrid, "", "tbuservorname", 1, 1);
                CreateTextBox(ControlsGrid, "", "tbuserstrasse", 1, 2);
                CreateTextBox(ControlsGrid, "", "tbuserplz", 1, 3);
                CreateTextBox(ControlsGrid, "", "tbuserort", 1, 4);
                CreateTextBox(ControlsGrid, "", "tbusergeburtsdatum", 1, 5);
                CreateTextBox(ControlsGrid, "", "tbusereintrittsdatum", 1, 6);
                CreateSubGrid(GridStatics, 1, 7);
                CCBenutzerStatics();
                CreateSubGrid(GridVariables, 1, 8);
                CCBenutzerVariables();
                TBView.Text = "Neuer Benutzer";
            }
            else
            {
                CreateTextBox(ControlsGrid, user.Name, "tbusername", 1, 0);
                CreateTextBox(ControlsGrid, user.Vorname, "tbuservorname", 1, 1);
                CreateTextBox(ControlsGrid, user.Strasse, "tbuserstrasse", 1, 2);
                CreateTextBox(ControlsGrid, user.PLZ.ToString(), "tbuserplz", 1, 3);
                CreateTextBox(ControlsGrid, user.Ort, "tbuserort", 1, 4);
                CreateTextBox(ControlsGrid, user.Geburtstag.ToString(), "tbusergeburtsdatum", 1, 5);
                CreateTextBox(ControlsGrid, user.Eintrittsdatum.ToString(), "tbusereintrittsdatum", 1, 6);
                CreateSubGrid(GridStatics, 1, 7);
                CCBenutzerStatics(user.statics);
                CreateSubGrid(GridVariables, 1, 8);
                CCBenutzerVariables(user.variables);
                TBView.Text = "Benutzer ändern";
            }
        }
        private void CCBenutzerStatics(List<(Static s, bool b)>? defaults = null)
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
                if (defaults is null)
                {
                    CreateCheckBox(GridStatics, "Aktivieren", string.Format("cbstatic{0}{1}", s.Id, s.Name.Replace(' ', '_')), s.Default, 3, row);
                }
                else
                {
                    CreateCheckBox(GridStatics, "Aktivieren", string.Format("cbstatic{0}{1}", s.Id, s.Name.Replace(' ', '_')), FindStaticDefault(defaults, s), 3, row);
                }
                row++;
            }
        }
        private bool FindStaticDefault(List<(Static s, bool b)> values, Static s)
        {
            foreach (var v in values) if (s == v.s) return v.b;
            return false;
        }
        private void CCBenutzerVariables(List<(Variable v, decimal w)>? defaults = null)
        {
            GridVariables.Children.Clear();
            GridVariables.ColumnDefinitions.Clear();
            GridVariables.RowDefinitions.Clear();

            GridVariables.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });         //ID
            GridVariables.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });         //Name
            GridVariables.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });         //Formel
            GridVariables.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });         //TextBox

            for (int i = 0; i < variables.Count + 2; i++) GridVariables.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });

            CreateTextBlock(GridVariables, "ID", 0, 0); CreateTextBlock(GridVariables, "Name", 1, 0); CreateTextBlock(GridVariables, "Formel", 2, 0); CreateTextBlock(GridVariables, "Wert", 3, 0);
            int row = 1;
            foreach(var v in variables)
            {
                CreateTextBlock(GridVariables, v.Id.ToString(), 0, row);
                CreateTextBlock(GridVariables, v.Name, 1, row);
                CreateTextBlock(GridVariables, v.Formel, 2, row);
                if(defaults is null)
                {
                    CreateTextBox(GridVariables, v.Default.ToString(), string.Format("tbvariable{0}{1}", v.Id, v.Name.Replace(' ', '_')), 3, row);
                }
                else
                {
                    CreateTextBox(GridVariables, FindVariableDefault(defaults, v).ToString(), string.Format("tbvariable{0}{1}", v.Id, v.Name.Replace(' ', '_')), 3, row);
                }
                row++;
            }
        }
        private decimal FindVariableDefault(List<(Variable v, decimal w)> values, Variable v)
        {
            foreach (var value in values) if (value.v == v) return value.w;
            return 0;
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
                Background = Brushes.LightSlateGray,
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
        private void CreateTextBlock(Grid grid, string text, int column, int row, string? tooltip = null)
        {
            TextBlock tb = new TextBlock()
            {
                Margin = new Thickness(5),
                Text = text,
                ToolTip = tooltip
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
                Name = name,
                MinWidth = 50
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
            checkboxes.Add(cb);
        }
        private string? SearchTextBoxes(string name)
        {
            foreach(var tb in boxes)
            {
                if(tb.Name == name) return tb.Text;
            }
            return null;
        }
        private bool? SearchCheckBoxes(string name)
        {
            foreach(var cb in checkboxes)
            {
                if (cb.Name == name) return cb.IsChecked;
            }
            return null;
        }

        private void ClickCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ClickAccept(object sender, RoutedEventArgs e)
        {
            (string aktion, bool erfolgreich) success;
            switch (this.view)
            {
                case View.Benutzer:
                    success = (this.mode == Mode.CreateNew) ? CreateUser() : ChangeUser();
                    if (success.erfolgreich) MessageBox.Show(string.Format("{0} erfolgreich!", success.aktion), success.aktion, MessageBoxButton.OK, MessageBoxImage.Exclamation); 
                    else MessageBox.Show(string.Format("{0} nicht erfolgreich!", success.aktion), success.aktion, MessageBoxButton.OK, MessageBoxImage.Error);
                    parent.Reload();
                    this.Close();
                    break;
                case View.StatischePosten:
                    success = (this.mode == Mode.CreateNew) ? CreateStatic() : ChangeStatic();
                    if (success.erfolgreich) MessageBox.Show(string.Format("{0} erfolgreich!", success.aktion), success.aktion, MessageBoxButton.OK, MessageBoxImage.Exclamation); 
                    else MessageBox.Show(string.Format("{0} nicht erfolgreich!", success.aktion), success.aktion, MessageBoxButton.OK, MessageBoxImage.Error);
                    parent.Reload();
                    this.Close();
                    break;
                case View.VariablePosten:
                    success = (this.mode == Mode.CreateNew) ? CreateVariable() : ChangeVariable();
                    if (success.erfolgreich) MessageBox.Show(string.Format("{0} erfolgreich!", success.aktion), success.aktion, MessageBoxButton.OK, MessageBoxImage.Exclamation); 
                    else MessageBox.Show(string.Format("{0} nicht erfolgreich!", success.aktion), success.aktion, MessageBoxButton.OK, MessageBoxImage.Error);
                    parent.Reload();
                    this.Close();
                    break;
                case View.Jubilaeen:
                    success = (this.mode == Mode.CreateNew) ? CreateJub() : ChangeJub();
                    if (success.erfolgreich) MessageBox.Show(string.Format("{0} erfolgreich!", success.aktion), success.aktion, MessageBoxButton.OK, MessageBoxImage.Exclamation); 
                    else MessageBox.Show(string.Format("{0} nicht erfolgreich!", success.aktion), success.aktion, MessageBoxButton.OK, MessageBoxImage.Error);
                    parent.Reload();
                    this.Close();
                    break;
                case View.GesamtAktuell:
                    success = (this.mode == Mode.CreateNew) ? CreateUser() : ChangeUser();
                    if (success.erfolgreich) MessageBox.Show(string.Format("{0} erfolgreich!", success.aktion), success.aktion, MessageBoxButton.OK, MessageBoxImage.Exclamation); 
                    else MessageBox.Show(string.Format("{0} nicht erfolgreich!", success.aktion), success.aktion, MessageBoxButton.OK, MessageBoxImage.Error);
                    parent.Reload();
                    this.Close();
                    break;
            }
        }

        private int NextId(List<List<object>> list)
        {
            int max = int.MinValue;
            foreach(var item in list)
            {
                int i = (int)item[0];
                if (i > max) max = i;
            }
            return max + 1;
        }
        private bool ChangeUserVariable(int userid, int variableid, decimal value)
        {
            var err = dbc.CommandNonQuery(string.Format(
                "UPDATE BenutzerVariable SET Wert='{0}' WHERE BID='{1}' AND VRPID='{2}';", 
                value, userid, variableid));
            if (err is not null) ErrorLogging.Log(err);
            else return true;
            return false;
        }
        private bool CreateUserVariable(int userid, int variableid, decimal value)
        {
            var query = dbc.CommandQueryToList("SELECT BVID FROM BenutzerVariable;");
            if (query.error is not null) ErrorLogging.Log(query.error);
            else if (query.solution is not null && query.solution.Count > 0)
            {
                var err = dbc.CommandNonQuery(string.Format(
                    "INSERT INTO BenutzerVariable (BVID, BID, VRPID, Wert) VALUES((SELECT MAX(BSID) FROM BenutzerStatisch) + 1, '{0}', '{1}', '{2}');",
                    userid, variableid, value.ToString(nfi)));
                if (err is not null) ErrorLogging.Log(err);
                else return true;
            }
            else
            {
                var err = dbc.CommandNonQuery(string.Format(
                    "INSERT INTO BenutzerVariable (BVID, BID, VRPID, Wert) VALUES(0, '{0}', '{1}', '{2}');",
                    userid, variableid, value.ToString(nfi)));
                if (err is not null) ErrorLogging.Log(err);
                else return true;
            }
            return false;
        }
        private bool ChangeUserStatic(int userid, int staticid, bool value)
        {
            var err = dbc.CommandNonQuery(string.Format(
                "UPDATE BenutzerStatisch SET Aktiv={0} WHERE BID='{1}' AND SRPID='{2}';", 
                value, userid, staticid));
            if (err is not null) ErrorLogging.Log(err);
            else return true;
            return false;
        }
        private bool CreateUserStatic(int userid, int staticid, bool value)
        {
            var query = dbc.CommandQueryToList("SELECT BSID FROM BenutzerStatisch;");
            if(query.error is not null) ErrorLogging.Log(query.error);
            else if(query.solution is not null && query.solution.Count > 0)
            {
                var err = dbc.CommandNonQuery(string.Format(
                    "INSERT INTO BenutzerStatisch (BSID, BID, SRPID, Aktiv) VALUES((SELECT MAX(BSID) FROM BenutzerStatisch) + 1, '{0}', '{1}', {2});", 
                    userid, staticid, value));
                if (err is not null) ErrorLogging.Log(err);
                else return true;
            }
            else
            {
                var err = dbc.CommandNonQuery(string.Format(
                    "INSERT INTO BenutzerStatisch (BSID, BID, SRPID, Aktiv) VALUES(0, '{0}', '{1}', {2});",
                    userid, staticid, value));
                if (err is not null) ErrorLogging.Log(err);
                else return true;
            }
            return false;
        }
        private (string aktion, bool erfolgreich) ChangeVariable()
        {
            var tbvariablename = SearchTextBoxes("tbvariablename");
            var tbvariableformel = SearchTextBoxes("tbvariableformel");
            var tbvariabledefault = SearchTextBoxes("tbvariabledefault");
            if(tbvariablename is not null && tbvariableformel is not null && tbvariabledefault is not null)
            {
                dbc.CommandNonQuery("BEGIN;");
                var err = dbc.CommandNonQuery(string.Format(
                    "UPDATE VariableRechnungsPosten SET Beschreibung='{0}', Formel='{1}', Def='{2}' WHERE VRPID={3};", 
                    tbvariablename, tbvariableformel, StringToDecimal(tbvariabledefault).ToString(nfi), id));
                if (err is not null)
                {
                    dbc.CommandNonQuery("ROLLBACK;"); ErrorLogging.Log(err);
                }
                else
                {
                    dbc.CommandNonQuery("COMMIT;"); return ("Variablen Posten ändern", true);
                }
            }
            return ("Variablen Posten ändern", false);
        }
        private (string aktion, bool erfolgreich) CreateVariable()
        {
            var tbvariablename = SearchTextBoxes("tbvariablename");
            var tbvariableformel = SearchTextBoxes("tbvariableformel");
            var tbvariabledefault = SearchTextBoxes("tbvariabledefault");
            if (tbvariablename is not null && tbvariableformel is not null && tbvariabledefault is not null)
            {
                var query = dbc.CommandQueryToList("SELECT VRPID FROM VariableRechnungsPosten;");
                if (query.error is not null) ErrorLogging.Log(query.error);
                else if (query.solution is not null && query.solution.Count > 0)
                {
                    dbc.CommandNonQuery("BEGIN;");
                    id = NextId(query.solution);
                    var err = dbc.CommandNonQuery(string.Format(
                        "INSERT INTO VariableRechnungsPosten (VRPID, Beschreibung, Formel, Def) VALUES('{3}', '{0}', '{1}', '{2}');",
                         tbvariablename, tbvariableformel, StringToDecimal(tbvariabledefault).ToString(nfi), id));
                    if (err is not null)
                    {
                        dbc.CommandNonQuery("ROLLBACK;"); ErrorLogging.Log(err);
                    }
                    else
                    {
                        foreach (var u in users)
                        {
                            var error = CreateUserVariable(u.Id, id, StringToDecimal(tbvariabledefault));
                            if (!error)
                            {
                                dbc.CommandNonQuery("ROLLBACK;"); return ("Variablen Posten erstellen", false);
                            }
                        }
                        dbc.CommandNonQuery("COMMIT;");
                        return ("Variablen Posten erstellen", true);
                    }
                }
                else
                {
                    dbc.CommandNonQuery("BEGIN;");
                    id = 0;
                    var err = dbc.CommandNonQuery(string.Format(
                        "INSERT INTO VariableRechnungsPosten (VRPID, Beschreibung, Formel, Def) VALUES(0, '{0}', '{1}', {2});",
                         tbvariablename, StringToDecimal(tbvariableformel).ToString(nfi), tbvariabledefault));
                    if (err is not null)
                    {
                        dbc.CommandNonQuery("ROLLBACK;"); ErrorLogging.Log(err);
                    }
                    else
                    {
                        foreach (var u in users)
                        {
                            var error = CreateUserVariable(u.Id, id, StringToDecimal(tbvariabledefault));
                            if (!error)
                            {
                                dbc.CommandNonQuery("ROLLBACK;"); return ("Variablen Posten erstellen", false);
                            }
                        }
                        dbc.CommandNonQuery("COMMIT;");
                        return ("Variablen Posten erstellen", true);
                    }
                }
            }
            return ("Variablen Posten erstellen", false);
        }
        private (string aktion, bool erfolgreich) ChangeStatic()
        {
            var tbstaticname = SearchTextBoxes("tbstaticname");
            var tbstaticwert = SearchTextBoxes("tbstaticwert");
            var cbstaticdefault = SearchCheckBoxes("cbstaticdefault");
            if(tbstaticname is not null && tbstaticwert is not null && cbstaticdefault is not null)
            {
                dbc.CommandNonQuery("BEGIN;");
                var err = dbc.CommandNonQuery(string.Format(
                    "UPDATE StatischeRechnungsPosten SET Beschreibung='{0}', Wert='{1}', Def='{2}' WHERE SRPID={3};", 
                    tbstaticname, StringToDecimal(tbstaticwert).ToString(nfi), cbstaticdefault, id));
                if (err is not null)
                {
                    dbc.CommandNonQuery("ROLLBACK;"); ErrorLogging.Log(err);
                }
                else
                {
                    dbc.CommandNonQuery("COMMIT;"); return ("Statischen Posten ändern", true);
                }
            }
            return ("Statischen Posten ändern", false);
        }
        private (string aktion, bool erfolgreich) CreateStatic()
        {
            var tbstaticname = SearchTextBoxes("tbstaticname");
            var tbstaticwert = SearchTextBoxes("tbstaticwert");
            var cbstaticdefault = SearchCheckBoxes("cbstaticdefault");
            if (tbstaticname is not null && tbstaticwert is not null && cbstaticdefault is not null)
            {
                var query = dbc.CommandQueryToList("SELECT SRPID FROM StatischeRechnungsPosten;");
                if (query.error is not null) ErrorLogging.Log(query.error);
                else if(query.solution is not null && query.solution.Count > 0) 
                {
                    dbc.CommandNonQuery("BEGIN;");
                    id = NextId(query.solution);
                    var err = dbc.CommandNonQuery(string.Format(
                        "INSERT INTO StatischeRechnungsPosten (SRPID, Beschreibung, Wert, Def) VALUES('{3}', '{0}', '{1}', '{2}');",
                        tbstaticname, StringToDecimal(tbstaticwert).ToString(nfi), (bool)cbstaticdefault, id));
                    if (err is not null)
                    {
                        dbc.CommandNonQuery("ROLLBACK;"); ErrorLogging.Log(err);
                    }
                    else
                    {
                        foreach (var u in users)
                        {
                            var error = CreateUserStatic(u.Id, id, (bool)cbstaticdefault);
                            if (!error)
                            {
                                dbc.CommandNonQuery("ROLLBACK;"); return ("Statischen Posten erstellen", false);
                            }
                        }
                        dbc.CommandNonQuery("COMMIT;");
                        return ("Statischen Posten erstellen", true);
                    }
                }
                else
                {
                    dbc.CommandNonQuery("BEGIN;");
                    id = 0;
                    var err = dbc.CommandNonQuery(string.Format(
                        "INSERT INTO StatischeRechnungsPosten (SRPID, Beschreibung, Wert, Def) VALUES(0, '{0}', {1}, {2});",
                        tbstaticname, StringToDecimal(tbstaticwert).ToString(nfi), (bool)cbstaticdefault));
                    if (err is not null)
                    {
                        dbc.CommandNonQuery("ROLLBACK;"); ErrorLogging.Log(err);
                    }
                    else
                    {
                        foreach (var u in users)
                        {
                            var error = CreateUserStatic(u.Id, id, (bool)cbstaticdefault);
                            if (!error)
                            {
                                dbc.CommandNonQuery("ROLLBACK;"); return ("Statischen Posten erstellen", false);
                            }
                        }
                        dbc.CommandNonQuery("COMMIT;");
                        return ("Statischen Posten erstellen", true);
                    }
                }
            }
            return ("Statischen Posten erstellen", false);
        }
        private (string aktion, bool erfolgreich) ChangeJub()
        {
            var tbjubwert = SearchTextBoxes("tbjubwert");
            if (tbjubwert is not null) {
                dbc.CommandNonQuery("BEGIN;");
                var err = dbc.CommandNonQuery(string.Format(
                    "UPDATE Jubilaeum SET Jahre={0} WHERE JID={1};", 
                    StringToInt(tbjubwert), id));
                if (err is not null)
                {
                    dbc.CommandNonQuery("ROLLBACK;"); ErrorLogging.Log(err);
                }
                else
                {
                    dbc.CommandNonQuery("COMMIT;"); return ("Jubiläum ändern", true);
                }
            }
            return ("Jubiläum ändern", false);
        }
        private (string aktion, bool erfolgreich) CreateJub()
        {
            var tbjubwert = SearchTextBoxes("tbjubwert");
            if(tbjubwert is not null)
            {
                var query = dbc.CommandQueryToList("SELECT JID FROM Jubilaeum;");
                if (query.error is not null) ErrorLogging.Log(query.error);
                else if (query.solution is not null && query.solution.Count > 0)
                {
                    dbc.CommandNonQuery("BEGIN;");
                    var err = dbc.CommandNonQuery(string.Format(
                        "INSERT INTO Jubilaeum (JID, Jahre) VALUES((SELECT MAX(JID) FROM Jubilaeum) + 1, '{0}');", 
                        tbjubwert));
                    if (err is not null)
                    {
                        dbc.CommandNonQuery("ROLLBACK;"); ErrorLogging.Log(err);
                    }
                    else
                    {
                        dbc.CommandNonQuery("COMMIT;"); return ("Jubiläum erstellen", true);
                    }
                }
                else
                {
                    dbc.CommandNonQuery("BEGIN;");
                    var err = dbc.CommandNonQuery(string.Format(
                        "INSERT INTO Jubilaeum (JID, Jahre) VALUES(0, {0});", 
                        tbjubwert));
                    if (err is not null)
                    {
                        dbc.CommandNonQuery("ROLLBACK;"); ErrorLogging.Log(err);
                    }
                    else
                    {
                        dbc.CommandNonQuery("COMMIT;"); return ("Jubiläum erstellen", true);
                    }
                }
            }
            return ("Jubiläum erstellen", false);
        }
        private (string aktion, bool erfolgreich) ChangeUser()
        {
            var tbusername = SearchTextBoxes("tbusername");
            var tbuservorname = SearchTextBoxes("tbuservorname");
            var tbuserstrasse = SearchTextBoxes("tbuserstrasse");
            var tbuserplz = SearchTextBoxes("tbuserplz");
            var tbuserort = SearchTextBoxes("tbuserort");
            var tbusergeburtsdatum = SearchTextBoxes("tbusergeburtsdatum");
            var tbusereintrittsdatum = SearchTextBoxes("tbusereintrittsdatum");
            if(tbusername != null && tbuservorname != null && tbuserstrasse != null && tbuserplz != null && tbuserort != null && tbusergeburtsdatum != null && tbusereintrittsdatum != null)
            {
                try
                {
                    DataBaseConnection.Date gd = new DataBaseConnection.Date(tbusergeburtsdatum);
                    DataBaseConnection.Date ed = new DataBaseConnection.Date(tbusereintrittsdatum);
                }
                catch (Exception ex)
                {
                    ErrorLogging.Log(ex.ToString());
                    MessageBox.Show("Falsche Eingabe in Datumfeld. Formate (DD.MM.YYYY oder YYYY.MM.DD)");
                    return ("Benutzer ändern", false);
                }
                dbc.CommandNonQuery("BEGIN;");
                var err = dbc.CommandNonQuery(string.Format(
                    "UPDATE Benutzer SET Name='{0}', Vorname='{1}', Strasse='{2}', PLZ={3}, Ort='{4}', Geburtsdatum='{5}', Eintrittsdatum='{6}' WHERE BID={7};",
                    tbusername, tbuservorname, tbuserstrasse, StringToInt(tbuserplz), tbuserort, StringToDate(tbusergeburtsdatum).ToString(), StringToDate(tbusereintrittsdatum).ToString(), id));
                if (err is not null) ErrorLogging.Log(err);
                else
                {
                    foreach(var s in statics)
                    {
                        var cbstatic = SearchCheckBoxes(string.Format("cbstatic{0}{1}", s.Id, s.Name.Replace(' ', '_')));
                        if (cbstatic is not null)
                        {
                            var error = ChangeUserStatic(id, s.Id, (bool)cbstatic);
                            if (!error)
                            {
                                dbc.CommandNonQuery("ROLLBACK;");
                                return ("Benutzer ändern", false);
                            }
                        }
                        else { dbc.CommandNonQuery("ROLLBACK;"); return ("Benutzer ändern", false);}
                        }

                    foreach(var v in variables)
                    {
                        var tbvariable = SearchTextBoxes(string.Format("tbvariable{0}{1}", v.Id, v.Name.Replace(' ', '_')));
                        if (tbvariable is not null)
                        {
                            var error = ChangeUserVariable(id, v.Id, StringToDecimal(tbvariable));
                            if (!error)
                            {
                                dbc.CommandNonQuery("ROLLBACK;");
                                return ("Benutzer ändern", false);
                            }
                        }
                        else
                        {
                            dbc.CommandNonQuery("ROLLBACK;"); return ("Benutzer ändern", false);
                        }
                    }
                    dbc.CommandNonQuery("COMMIT;");
                    return ("Benutzer ändern", true);
                }
            }
            return ("Benutzer ändern", false);
        }
        private (string aktion, bool erfolgreich) CreateUser()
        {
            var tbusername = SearchTextBoxes("tbusername");
            var tbuservorname = SearchTextBoxes("tbuservorname");
            var tbuserstrasse = SearchTextBoxes("tbuserstrasse");
            var tbuserplz = SearchTextBoxes("tbuserplz");
            var tbuserort = SearchTextBoxes("tbuserort");
            var tbusergeburtsdatum = SearchTextBoxes("tbusergeburtsdatum");
            var tbusereintrittsdatum = SearchTextBoxes("tbusereintrittsdatum");
            if (tbusername != null && tbuservorname != null && tbuserstrasse != null && tbuserplz != null && tbuserort != null && tbusergeburtsdatum != null && tbusereintrittsdatum != null)
            {
                try
                {
                    DataBaseConnection.Date gd = new DataBaseConnection.Date(tbusergeburtsdatum);
                    DataBaseConnection.Date ed = new DataBaseConnection.Date(tbusereintrittsdatum);
                }
                catch (Exception ex)
                {
                    ErrorLogging.Log(ex.ToString());
                    MessageBox.Show("Falsche Eingabe in Datumfeld. Formate (DD.MM.YYYY oder YYYY.MM.DD)");
                    return ("Benutzer erstellen", false);
                }
                var query = dbc.CommandQueryToList("SELECT BID FROM Benutzer;");
                if (query.error is not null) ErrorLogging.Log(query.error);
                else if (query.solution is not null && query.solution.Count > 0)
                {
                    dbc.CommandNonQuery("BEGIN;");
                    id = NextId(query.solution);
                    var err = dbc.CommandNonQuery(string.Format(
                        "INSERT INTO Benutzer (BID, Name, Vorname, Strasse, PLZ, Ort, Geburtsdatum, Eintrittsdatum) VALUES('{7}', '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');",
                        tbusername, tbuservorname, tbuserstrasse, StringToInt(tbuserplz), tbuserort, StringToDate(tbusergeburtsdatum).ToString(), StringToDate(tbusereintrittsdatum).ToString(), id));
                    if (err is not null) ErrorLogging.Log(err);
                    else
                    {
                        foreach (var s in statics)
                        {
                            var cbstatic = SearchCheckBoxes(string.Format("cbstatic{0}{1}", s.Id, s.Name.Replace(' ', '_')));
                            if (cbstatic is not null)
                            {
                                var error = CreateUserStatic(id, s.Id, (bool)cbstatic);
                                if (!error)
                                {
                                    dbc.CommandNonQuery("ROLLBACK;");
                                    return ("Benutzer erstellen", false);
                                }
                            }
                            else
                            {
                                dbc.CommandNonQuery("ROLLBACK;"); return ("Benutzer erstellen", false);
                            }
                        }

                        foreach (var v in variables)
                        {
                            var tbvariable = SearchTextBoxes(string.Format("tbvariable{0}{1}", v.Id, v.Name.Replace(' ', '_')));
                            if (tbvariable is not null)
                            {
                                var error = CreateUserVariable(id, v.Id, StringToDecimal(tbvariable));
                                if (!error)
                                {
                                    dbc.CommandNonQuery("ROLLBACK;");
                                    return ("Benutzer erstellen", false);
                                }
                            }
                            else
                            {
                                dbc.CommandNonQuery("ROLLBACK;"); return ("Benutzer erstellen", false);
                            }
                        }
                        dbc.CommandNonQuery("COMMIT;");
                        return ("Benutzer erstellen", true);
                    }
                }
                else
                {
                    dbc.CommandNonQuery("BEGIN;");
                    id = 0;
                    var err = dbc.CommandNonQuery(string.Format(
                        "INSERT INTO Benutzer (BID, Name, Vorname, Strasse, PLZ, Ort, Geburtsdatum, Eintrittsdatum) VALUES(0, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');",
                        tbusername, tbuservorname, tbuserstrasse, StringToInt(tbuserplz), tbuserort, StringToDate(tbusergeburtsdatum).ToString(), StringToDate(tbusereintrittsdatum).ToString()));
                    if (err is not null) ErrorLogging.Log(err);
                    else
                    {
                        foreach (var s in statics)
                        {
                            var cbstatic = SearchCheckBoxes(string.Format("cbstatic{0}{1}", s.Id, s.Name.Replace(' ', '_')));
                            if (cbstatic is not null)
                            {
                                var error = CreateUserStatic(id, s.Id, (bool)cbstatic);
                                if (!error)
                                {
                                    dbc.CommandNonQuery("ROLLBACK;");
                                    return ("Benutzer erstellen", false);
                                }
                            }
                            else
                            {
                                dbc.CommandNonQuery("ROLLBACK;"); return ("Benutzer erstellen", false);
                            }
                        }

                        foreach (var v in variables)
                        {
                            var tbvariable = SearchTextBoxes(string.Format("tbvariable{0}{1}", v.Id, v.Name.Replace(' ', '_')));
                            if (tbvariable is not null)
                            {
                                var error = CreateUserVariable(id, v.Id, StringToDecimal(tbvariable));
                                if (!error)
                                {
                                    dbc.CommandNonQuery("ROLLBACK;");
                                    return ("Benutzer erstellen", false);
                                }
                            }
                            else
                            {
                                dbc.CommandNonQuery("ROLLBACK;"); return ("Benutzer erstellen", false);
                            }
                        }
                        dbc.CommandNonQuery("COMMIT;");
                        return ("Benutzer erstellen", true);
                    }
                }
            }
            return ("Benutzer erstellen", false);
        }
    }
}
