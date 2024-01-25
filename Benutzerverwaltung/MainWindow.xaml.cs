using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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

namespace Benutzerverwaltung
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum View { Benutzer, VariablePosten, StatischePosten, Jubilaeen, GesamtAktuell, JubilaeenAktuell}
        public enum Mode { Delete, Administrate, CreateNew}
        public enum JubilaeumMode { Birthday, Join}

        private DataBaseConnection dbc;
        private List<User> users;
        private List<Static> statics;
        private List<Variable> variables;
        private List<Jubilaeum> jubilaeen;
        private string? searchtext;

        private View view;

        public static Brush CreateForeground = Brushes.Blue;
        public static Brush CreateBackground = Brushes.Azure;
        public static Brush DeleteForeground = Brushes.Red;
        public static Brush DeleteBackground = Brushes.MistyRose;
        public static Brush AdminForeground = Brushes.Green;
        public static Brush AdminBackground = Brushes.YellowGreen;

        public MainWindow()
        {
            InitializeComponent();
            dbc = new DataBaseConnection();
            users = new List<User>();
            statics = new List<Static>();
            variables = new List<Variable>();
            jubilaeen = new List<Jubilaeum>();
            view = View.GesamtAktuell;
            searchtext = null;
            TBSearch.Text = string.Empty;
            TBView.Text = "Aktuelle Ansicht: Gesamt";
            LoadData();
            LoadControls();
        }

        public void Reload()
        {
            LoadData();
            LoadControls();
        }

        private void LoadData()
        {
            users.Clear();
            statics.Clear();
            variables.Clear();
            jubilaeen.Clear();
            var query = dbc.CommandQueryToList("SELECT BID, Name, Vorname, Strasse, PLZ, Ort, Geburtsdatum, Eintrittsdatum FROM Benutzer;");
            if(query.error is not null) Console.Error.WriteLine(query.error.ToString());
            else if(query.solution is not null)
            {
                foreach(var user in query.solution)
                {
                    DateTime date1;
                    DateTime.TryParse((string)user[6], out date1);
                    DateTime date2;
                    DateTime.TryParse((string)user[7], out date2);
                    users.Add(new User((int)user[0], (string)user[1], (string)user[2], (string)user[3], (int)user[4], (string)user[5], date1, date2));
                }
            }
            query = dbc.CommandQueryToList("SELECT * FROM StatischeRechnungsPosten;");
            if (query.error is not null) Console.Error.WriteLine(query.error.ToString());
            else if(query.solution is not null)
            {
                foreach(var s in query.solution)
                {
                    statics.Add(new Static((int)s[0], (string)s[1], (decimal)s[2]));
                }
            }
            query = dbc.CommandQueryToList("SELECT * FROM VariableRechnungsPosten;");
            if(query.error is not null) Console.Error.WriteLine(query.error.ToString());
            else if(query.solution is not null)
            {
                foreach(var v in query.solution)
                {
                    variables.Add(new Variable((int)v[0], (string)v[1], (string)v[2]));
                }
            }
            foreach(var us in users)
            {
                foreach(var st in statics)
                {
                    query = dbc.CommandQueryToList(string.Format("SELECT Aktiv FROM BenutzerStatisch WHERE BID={0} AND SRPID={1};", us.Id, st.Id));
                    if(query.error is not null) Console.Error.WriteLine(query.error.ToString());
                    else if(query.solution is not null)
                    {
                        if(query.solution.Count() > 0 && query.solution[0].Count > 0)
                        {
                            us.AddStatic((st, (bool)query.solution[0][0]));
                        }
                        else
                        {
                            us.AddStatic((st, false));
                        }
                    }
                }

                foreach (var v in variables)
                {
                    query = dbc.CommandQueryToList(string.Format("SELECT Wert FROM BenutzerVariable WHERE BID={0} AND VRPID={1};", us.Id, v.Id));
                    if (query.error is not null) Console.Error.WriteLine(query.error.ToString());
                    else if (query.solution is not null)
                    {
                        if (query.solution.Count() > 0 && query.solution[0].Count > 0)
                        {
                            us.AddVariable((v, (decimal)query.solution[0][0]));
                        }
                        else
                        {
                            us.AddVariable((v, (decimal)0.0));
                        }
                    }
                }
            }
            query = dbc.CommandQueryToList("SELECT * FROM Jubilaeum;");
            if (query.error is not null) Console.Error.WriteLine(query.error.ToString());
            else if(query.solution is not null)
            {
                foreach(var j in query.solution)
                {
                    jubilaeen.Add(new Jubilaeum((int)j[0], (int)j[1]));
                }
            }
        }

        private void LoadControls()
        {
            DataGrid.ShowGridLines = true;
            DataGrid.ColumnDefinitions.Clear();
            DataGrid.RowDefinitions.Clear();
            DataGrid.Children.Clear();

            int rows;
            if(searchtext is null)
            {
                switch (view)
                {
                    case View.Benutzer:
                        rows = users.Count + 2;
                        var sortedU = SortUsers(users);
                        WriteUsers(sortedU, rows);
                        break;
                    case View.StatischePosten:
                        rows = statics.Count + 2;
                        var sortedS = SortStatics(statics);
                        WriteStatics(sortedS, rows);
                        break;
                    case View.VariablePosten: 
                        rows = variables.Count + 2;
                        var sortedV = SortVariables(variables);
                        WriteVariables(sortedV, rows);
                        break;
                    case View.Jubilaeen:
                        rows = jubilaeen.Count + 2;
                        var sortedJ = SortJubs(jubilaeen);
                        WriteJubs(sortedJ, rows);
                        break;
                    case View.JubilaeenAktuell:
                        var sortedUJA = SortUsers(users);
                        var sortedJJA = SortJubs(jubilaeen);
                        var currentJ = CalculateJubs(sortedUJA, sortedJJA);
                        rows = currentJ.Count + 1;
                        WriteCurrentJubs(currentJ, rows);
                        break;
                    case View.GesamtAktuell:
                        var sortedUGA = SortUsers(users);
                        WriteCurrentUsers(sortedUGA, statics, variables);
                        break;
                }
            }
            else
            {
                switch (view)
                {
                    case View.Benutzer:
                        var queryU = SearchUser(searchtext);
                        rows = queryU.Count + 2;
                        var sortedU = SortUsers(queryU);
                        WriteUsers(sortedU, rows);
                        break;
                    case View.StatischePosten:
                        var queryS = SearchStatics(searchtext);
                        rows = queryS.Count + 2;
                        var sortedS = SortStatics(queryS);
                        WriteStatics(sortedS, rows);
                        break;
                    case View.VariablePosten:
                        var queryV = SearchVariables(searchtext);
                        rows = queryV.Count + 2;
                        var sortedV = SortVariables(queryV);
                        WriteVariables(sortedV, rows);
                        break;
                    case View.Jubilaeen:
                        var queryJ = SearchJubs(searchtext);
                        rows = queryJ.Count + 2;
                        var sortedJ = SortJubs(queryJ);
                        WriteJubs(sortedJ, rows);
                        break;
                    case View.JubilaeenAktuell:
                        var queryUJA = SearchUser(searchtext);
                        var sortedUJA = SortUsers(queryUJA);
                        var sortedJJA = SortJubs(jubilaeen);
                        var currentJ = CalculateJubs(sortedUJA, sortedJJA);
                        rows = currentJ.Count + 1;
                        WriteCurrentJubs(currentJ, rows);
                        break;
                    case View.GesamtAktuell:
                        var queryUGA = SearchUser(searchtext);
                        var sortedUGA = SortUsers(queryUGA);
                        WriteCurrentUsers(sortedUGA, statics, variables);
                        break;
                }
            }
        }

        private void WriteStatics(List<Static> s, int rows)
        {
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            for(int i = 0; i < rows; i++) DataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });

            CreateTextBlock("Nummer", 0, 0);
            CreateTextBlock("Name", 1, 0);
            CreateTextBlock("Wert", 2, 0);
            CreateTextBlock("Löschen", 3, 0);
            CreateTextBlock("Ändern", 4, 0);
            int row = 1;
            foreach(var st in s)
            {
                CreateTextBlock(row.ToString(), 0, row);
                CreateTextBlock(st.Name, 1, row);
                CreateTextBlock(st.Wert.ToString(), 2, row);
                CreateButton("X", 3, row, st.Id, View.StatischePosten, Mode.Delete, null);
                CreateButton("...", 4, row, st.Id, View.StatischePosten, Mode.Administrate, null);
                row++;
            }
            CreateButton("Neuer Posten", 1, row, 0, View.StatischePosten, Mode.CreateNew, null);
        }
        private void WriteUsers(List<User> u, int rows)
        {
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            for (int i = 0; i < rows; i++) DataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });

            CreateTextBlock("Nummer", 0, 0); CreateTextBlock("Name", 1, 0); CreateTextBlock("Vorname", 2, 0); CreateTextBlock("Strasse", 3, 0);
            CreateTextBlock("PLZ", 4, 0); CreateTextBlock("Ort", 5, 0); CreateTextBlock("Geburtsdatum", 6, 0); CreateTextBlock("Eintrittsdatum", 7, 0);
            CreateTextBlock("Löschen", 8, 0); CreateTextBlock("Ändern", 9, 0);
            int row = 1;
            foreach(var user in u)
            {
                CreateTextBlock(row.ToString(), 0, row);
                CreateTextBlock(user.Name, 1, row); CreateTextBlock(user.Vorname, 2, row); CreateTextBlock(user.Strasse, 3, row); CreateTextBlock(user.PLZ.ToString(), 4, row);
                CreateTextBlock(user.Ort, 5, row); CreateTextBlock(user.Geburtstag.Date.ToString(), 6, row); CreateTextBlock(user.Eintrittsdatum.Date.ToString(), 7, row);
                CreateButton("X", 8, row, user.Id, View.Benutzer, Mode.Delete, user);
                CreateButton("...", 9, row, user.Id, View.Benutzer, Mode.Administrate, user);
                row++;
            }
            CreateButton("Neuer Benutzer", 1, row, 0, View.Benutzer, Mode.CreateNew, null);
        }
        private void WriteVariables(List<Variable> v, int rows)
        {
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            for (int i = 0; i < rows; i++) DataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });

            CreateTextBlock("Nummer", 0, 0);
            CreateTextBlock("Name", 1, 0);
            CreateTextBlock("Formel", 2, 0);
            CreateTextBlock("Löschen", 3, 0);
            CreateTextBlock("Ändern", 4, 0);
            int row = 1;
            foreach(var variable in v)
            {
                CreateTextBlock(row.ToString(), 0, row);
                CreateTextBlock(variable.Name, 1, row);
                CreateTextBlock(variable.Formel, 2, row);
                CreateButton("X", 3, row, variable.Id, View.VariablePosten, Mode.Delete, null);
                CreateButton("...", 4, row, variable.Id, View.VariablePosten, Mode.Administrate, null);
                row++;
            }
            CreateButton("Neuer Posten", 1, row, 0, View.VariablePosten, Mode.CreateNew, null);
        }
        private void WriteJubs(List<Jubilaeum> j, int rows)
        {
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            for (int i = 0; i < rows; i++) DataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });

            CreateTextBlock("Nummer", 0, 0);
            CreateTextBlock("Jahre", 1, 0);
            CreateTextBlock("Löschen", 2, 0);
            CreateTextBlock("Ändern", 3, 0);
            int row = 1;
            foreach(var jub in j)
            {
                CreateTextBlock(row.ToString(), 0, row);
                CreateTextBlock(jub.Jahre.ToString(), 1, row);
                CreateButton("X", 2, row, jub.Id, View.Jubilaeen, Mode.Delete, null);
                CreateButton("...", 3, row, jub.Id, View.Jubilaeen, Mode.Administrate, null);
                row++;
            }
            CreateButton("Neues Jubiläum", 1, row, 0, View.Jubilaeen, Mode.CreateNew, null);
        }
        private void WriteCurrentJubs(List<(User u, Jubilaeum j, JubilaeumMode jm)> current, int rows)
        {
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });

            for (int i = 0; i < rows; i++) DataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });

            CreateTextBlock("Nummer", 0, 0); CreateTextBlock("Name", 1, 0); CreateTextBlock("Vorname", 2, 0); CreateTextBlock("Jahre", 3, 0); CreateTextBlock("Datum", 4, 0);
            CreateTextBlock("Art", 5, 0);
            int row = 1;
            foreach(var c in current)
            {
                CreateTextBlock(row.ToString(), 0, row); CreateTextBlock(c.u.Name, 1, row); CreateTextBlock(c.u.Vorname, 2, row); CreateTextBlock(c.j.Jahre.ToString(), 3, row);
                if(c.jm == JubilaeumMode.Birthday)
                {
                    CreateTextBlock(c.u.Geburtstag.Date.ToString(), 4, row);
                    CreateTextBlock("Geburtstag", 5, row);
                }
                else
                {
                    CreateTextBlock(c.u.Eintrittsdatum.Date.ToString(), 4, row);
                    CreateTextBlock("Eintrittsdatum", 5, row);
                }
                row++;
            }
        }
        private void WriteCurrentUsers(List<User> lu, List<Static> ls, List<Variable> lv)
        {
            int rows = lu.Count + 2;
            int cols = 2 + ls.Count + lv.Count + 2;
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            for(int i = 0; i < cols; i++) DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            for (int i = 0; i < rows; i++) DataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            CreateTextBlock("Nummer", 0, 0); CreateTextBlock("Name", 1, 0); CreateTextBlock("Vorname", 2, 0);
            decimal[] sums = new decimal[cols+1];
            int col = 3;
            foreach (var s in ls) { CreateTextBlock(s.Name, col, 0); col++; sums[col] = 0; }
            foreach (var v in lv) { CreateTextBlock(v.Name, col, 0); col++; sums[col] = 0; }
            CreateTextBlock("Gesamt", col, 0); sums[col] = 0;
            CreateTextBlock("Bearbeiten", col + 1, 0);
            int row = 1;
            foreach(var u in lu)
            {
                CreateTextBlock(row.ToString(), 0, row); CreateTextBlock(u.Name, 1, row); CreateTextBlock(u.Vorname, 2, row);
                decimal sum = 0;
                col = 3;
                foreach(var s in ls)
                {
                    foreach(var us in u.statics)
                    {
                        if(s == us.s)
                        {
                            if (us.b)
                            {
                                CreateTextBlock(s.Wert.ToString(), col, row);
                                sums[col] += s.Wert;
                                sum += s.Wert;
                            }
                            else
                            {
                                CreateTextBlock("-", col, row);
                            }
                        }
                    }
                    col++;
                }
                foreach(var v in lv)
                {
                    foreach(var uv in u.variables)
                    {
                        if(v == uv.v)
                        {
                            var calc = uv.v.CalcValue(uv.w);
                            CreateTextBlock(calc.ToString(), col, row);
                            sums[col] += calc;
                            sum += calc;
                        }
                    }
                    col++;
                }
                CreateTextBlock(sum.ToString(), col, row);
                CreateButton("...", col + 1, row, u.Id, View.GesamtAktuell, Mode.Administrate, u);
                sums[col] += sum;
                row++;
            }
            CreateTextBlock("Summe", 2, row);
            for(col = 3; col < sums.Length; col++)
            {
                CreateTextBlock(sums[col].ToString(), col, row);
            }        
        }

        private void CreateTextBlock(string text, int column, int row)
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
            DataGrid.Children.Add(bd);
        }
        private void CreateButton(string text, int column, int row, int id, View view, Mode mode, User? user)
        {
            (int id, View view, Mode mode, User? user) vt = new(id, view, mode, user);
            Button bt = new Button()
            {
                Margin = new Thickness(5),
                Content = text,
                Tag = vt,
                Background = (mode == Mode.Delete) ? DeleteBackground : (mode == Mode.CreateNew) ? CreateBackground : AdminBackground,
                Foreground = (mode == Mode.Delete) ? DeleteForeground : (mode == Mode.CreateNew) ? CreateForeground : AdminForeground
            };
            bt.Click += ClickButton;
            Border bd = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = (mode == Mode.Delete) ? DeleteForeground : (mode == Mode.CreateNew) ? CreateForeground : AdminForeground,
                Background = (mode == Mode.Delete) ? DeleteBackground : (mode == Mode.CreateNew) ? CreateBackground : AdminBackground,
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = bt
            };
            Grid.SetColumn(bd, column);
            Grid.SetRow(bd, row);
            DataGrid.Children.Add(bd);
        }

        private List<Static> SortStatics(List<Static> unsorted) 
        {
            List<Static> sorted = unsorted.OrderBy(s => s.Name).ToList();
            return sorted;
        }
        private List<Variable> SortVariables(List<Variable> unsorted)
        {
            List<Variable> sorted = unsorted.OrderBy(s => s.Name).ToList();
            return sorted;
        }
        private List<User> SortUsers(List<User> unsorted)
        {
            List<User> sorted = unsorted.OrderBy(s => s.Name).ToList();
            return sorted;
        }
        private List<Jubilaeum> SortJubs(List<Jubilaeum> unsorted)
        {
            List<Jubilaeum> sorted = unsorted.OrderBy(s => s.Jahre).ToList();
            return sorted;
        }

        private List<Static> SearchStatics(string query)
        {
            List<Static> search = new List<Static>();
            foreach(var s in statics)
            {
                if(s.Name.Contains(query)) search.Add(s);
            }
            return search;
        }
        private List<Variable> SearchVariables(string query)
        {
            List<Variable> search = new List<Variable>();
            foreach (var v in variables)
            {
                if (v.Name.Contains(query)) search.Add(v);
            }
            return search;
        }
        private List<User> SearchUser(string query)
        {
            List<User> search = new List<User>();
            foreach (var u in users)
            {
                if (u.Name.Contains(query) || u.Vorname.Contains(query) || u.Ort.Contains(query) || u.Strasse.Contains(query) || u.PLZ.ToString().Contains(query)) search.Add(u);
            }
            return search;
        }
        private List<Jubilaeum> SearchJubs(string query)
        {
            List<Jubilaeum> search = new List<Jubilaeum>();
            foreach (var s in jubilaeen)
            {
                if (s.Jahre.ToString().Contains(query)) search.Add(s);
            }
            return search;
        }

        private List<(User u, Jubilaeum j, JubilaeumMode jm)> CalculateJubs(List<User> lu, List<Jubilaeum> lj)
        {
            List<(User u, Jubilaeum j, JubilaeumMode jm)> currentJubs = new List<(User u, Jubilaeum j, JubilaeumMode jm)>();
            DateTime today = DateTime.Now;
            int thisyear = today.Year;
            foreach(var user in lu)
            {
                int birthyear = user.Geburtstag.Year;
                int joinyear = user.Eintrittsdatum.Year;
                foreach (var jub in lj)
                {
                    if(thisyear - birthyear == jub.Jahre) currentJubs.Add((user, jub, JubilaeumMode.Birthday));
                    if(thisyear - joinyear == jub.Jahre) currentJubs.Add((user, jub, JubilaeumMode.Join));
                }
            }
            return currentJubs;
        }

        private void ClickVWB(object sender, RoutedEventArgs e)
        {
            if (view != View.Benutzer)
            {
                view = View.Benutzer;
                searchtext = null;
                TBSearch.Text = string.Empty;
                TBView.Text = "Aktuelle Ansicht: Benutzer Verwalten";
                LoadControls();
            }
        }
        private void ClickVWVP(object sender, RoutedEventArgs e)
        {
            if (view != View.VariablePosten)
            {
                view = View.VariablePosten;
                searchtext = null;
                TBSearch.Text = string.Empty;
                TBView.Text = "Aktuelle Ansicht: Variable Posten Verwalten";
                LoadControls();
            }
        }
        private void ClickVWSP(object sender, RoutedEventArgs e)
        {
            if (view != View.StatischePosten)
            {
                view = View.StatischePosten;
                searchtext = null;
                TBSearch.Text = string.Empty;
                TBView.Text = "Aktuelle Ansicht: Statische Posten Verwalten";
                LoadControls();
            }
        }
        private void ClickVWJ(object sender, RoutedEventArgs e)
        {
            if (view != View.Jubilaeen)
            {
                view = View.Jubilaeen;
                searchtext = null;
                TBSearch.Text = string.Empty;
                TBView.Text = "Aktuelle Ansicht: Jubiläen Verwalten";
                LoadControls();
            }
        }
        private void ClickJub(object sender, RoutedEventArgs e)
        {
            if(view != View.JubilaeenAktuell)
            {
                view = View.JubilaeenAktuell;
                searchtext = null;
                TBSearch.Text = string.Empty;
                TBView.Text = "Aktuelle Ansicht: Aktuelle Jubiläen";
                LoadControls();
            }
        }
        private void ClickGesamt(object sender, RoutedEventArgs e)
        {
            if(view != View.GesamtAktuell)
            {
                view = View.GesamtAktuell;
                searchtext = null;
                TBSearch.Text = string.Empty;
                TBView.Text = "Aktuelle Ansicht: Gesamt";
                LoadControls();
            }
        }

        private void ClickButton(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            var btinfo = ((int id, View view, Mode mode, User? user))bt.Tag;
            switch (btinfo.mode)
            {
                case Mode.Delete:
                    Delete(btinfo.id, btinfo.view);
                    break;
                default:
                    Admin(btinfo.id, btinfo.view, btinfo.mode, btinfo.user);
                    break;
            }
        }

        private void Delete(int id, View view)
        {
            DataBaseConnection.QueryList query = new DataBaseConnection.QueryList();
            switch (view)
            {
                case View.Benutzer:
                    query = dbc.CommandQueryToList(string.Format("SELECT * FROM Benutzer WHERE BID={0}", id));
                    if (query.error is not null) Console.Error.WriteLine(query.error);
                    else if (query.solution is not null && query.solution.Count > 0 && query.solution[0].Count > 0) {
                        string text = string.Format("Möchten Sie folgenden Nutzer unwiderruflich löschen?\n{0} {1} {2} {3} {4}", query.solution[0][0], query.solution[0][1], query.solution[0][2], query.solution[0][3], query.solution[0][4]);
                        if (MessageBoxResult.Yes == MessageBox.Show(text, "Löschen", MessageBoxButton.YesNoCancel))
                        {
                            var err = dbc.CommandNonQuery(string.Format("DELETE FROM Benutzer WHERE BID={0}", id));
                            if(err is not null) Console.Error.WriteLine(err);
                            err = dbc.CommandNonQuery(string.Format("DELETE FROM BenutzerStatisch WHERE BID={0}", id));
                            if (err is not null) Console.Error.WriteLine(err);
                            err = dbc.CommandNonQuery(string.Format("DELETE FROM BenutzerVariable WHERE BID={0}", id));
                            if (err is not null) Console.Error.WriteLine(err);
                            Reload();
                        }
                    }
                    break;
                case View.StatischePosten:
                    query = dbc.CommandQueryToList(string.Format("SELECT * FROM StatischeRechnungsPosten WHERE SRPID={0}", id));
                    if (query.error is not null) Console.Error.WriteLine(query.error);
                    else if (query.solution is not null && query.solution.Count > 0 && query.solution[0].Count > 0)
                    {
                        string text = string.Format("Möchten Sie folgenden Statischen Rechnungsposten unwiderruflich löschen?\n{0} {1} {2}", query.solution[0][0], query.solution[0][1], query.solution[0][2]);
                        if (MessageBoxResult.Yes == MessageBox.Show(text, "Löschen", MessageBoxButton.YesNoCancel))
                        {
                            var err = dbc.CommandNonQuery(string.Format("DELETE FROM StatischeRechnungsPosten WHERE SRPID={0}", id));
                            if (err is not null) Console.Error.WriteLine(err);
                            err = dbc.CommandNonQuery(string.Format("DELETE FROM BenutzerStatisch WHERE SRPID={0}", id));
                            if (err is not null) Console.Error.WriteLine(err);
                            Reload();
                        }
                    }
                    break;
                case View.VariablePosten:
                    query = dbc.CommandQueryToList(string.Format("SELECT * FROM VariableRechnungsPosten WHERE VRPID={0}", id));
                    if (query.error is not null) Console.Error.WriteLine(query.error);
                    else if (query.solution is not null && query.solution.Count > 0 && query.solution[0].Count > 0)
                    {
                        string text = string.Format("Möchten Sie folgenden Variablen Rechnungsposten unwiderruflich löschen?\n{0} {1} {2}", query.solution[0][0], query.solution[0][1], query.solution[0][2]);
                        if (MessageBoxResult.Yes == MessageBox.Show(text, "Löschen", MessageBoxButton.YesNoCancel))
                        {
                            var err = dbc.CommandNonQuery(string.Format("DELETE FROM VariableRechnungsPosten WHERE VRPID={0}", id));
                            if (err is not null) Console.Error.WriteLine(err);
                            err = dbc.CommandNonQuery(string.Format("DELETE FROM BenutzerVariable WHERE VRPID={0}", id));
                            if (err is not null) Console.Error.WriteLine(err);
                            Reload();
                        }
                    }
                    break;
                case View.Jubilaeen:
                    query = dbc.CommandQueryToList(string.Format("SELECT * FROM Jubilaeum WHERE JID={0}", id));
                    if (query.error is not null) Console.Error.WriteLine(query.error);
                    else if (query.solution is not null && query.solution.Count > 0 && query.solution[0].Count > 0)
                    {
                        string text = string.Format("Möchten Sie folgenden Variablen Rechnungsposten unwiderruflich löschen?\n{0} {1}", query.solution[0][0], query.solution[0][1]);
                        if (MessageBoxResult.Yes == MessageBox.Show(text, "Löschen", MessageBoxButton.YesNoCancel))
                        {
                            var err = dbc.CommandNonQuery(string.Format("DELETE FROM Jubilaeum WHERE JID={0}", id));
                            if (err is not null) Console.Error.WriteLine(err);
                            Reload();
                        }
                    }
                    break;
            }
        }

        private void Admin(int id, View view, Mode mode, User? user)
        {
            ConfigWindow child = new ConfigWindow(this, id, view, mode, dbc, statics, variables, user);
            child.Show();
        }

        private void SearchTextChanged(object sender, KeyEventArgs e)
        {
            searchtext = TBSearch.Text;
            LoadControls();
        }
    }
}
