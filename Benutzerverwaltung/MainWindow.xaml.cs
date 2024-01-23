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

        private DataBaseConnection dbc;
        private List<User> users;
        private List<Static> statics;
        private List<Variable> variables;
        private List<int> jubilaeen;
        private string? searchtext;

        private View view;
        public MainWindow()
        {
            dbc = new DataBaseConnection();
            users = new List<User>();
            statics = new List<Static>();
            variables = new List<Variable>();
            jubilaeen = new List<int>();
            view = View.GesamtAktuell;
            searchtext = null;
            TBSearch.Text = searchtext;
            TBView.Text = "Aktuelle Ansicht: Gesamt";
            InitializeComponent();
            LoadData();
            LoadControls();
        }

        private void LoadData()
        {
            users.Clear();
            statics.Clear();
            variables.Clear();
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
                    Enum.TryParse(typeof(Einheit), (string)v[2], out object? result);
                    if (result == null || typeof(Einheit) != result.GetType()) result = Einheit.euro;
                    variables.Add(new Variable((int)v[0], (string)v[1], (Einheit)result));
                }
            }
            foreach(var us in users)
            {
                foreach(var st in statics)
                {
                    query = dbc.CommandQueryToList(string.Format("SELECT Aktiv FROM BenutzerStatisch WHERE BID={0} AND SRPID={1};", u.Id, s.Id));
                    if(query.error is not null) Console.Error.WriteLine(query.error.ToString());
                    else if(query.solution is not null)
                    {
                        if(query.solution.Count() > 0)
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
                    query = dbc.CommandQueryToList(string.Format("SELECT Wert FROM BenutzerVariable WHERE BID={0} AND VRPID={1};", u.Id, v.Id));
                    if (query.error is not null) Console.Error.WriteLine(query.error.ToString());
                    else if (query.solution is not null)
                    {
                        if (query.solution.Count() > 0)
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
            query = dbc.CommandQueryToList("SELECT Jahre FROM Jubilaeum;");
            if (query.error is not null) Console.Error.WriteLine(query.error.ToString());
            else if(query.solution is not null)
            {
                foreach(var j in query.solution)
                {
                    jubilaeen.Add((int)j[0]);
                }
            }
        }

        private void LoadControls()
        {
            DataGrid.ShowGridLines = true;
            DataGrid.ColumnDefinitions.Clear();
            DataGrid.RowDefinitions.Clear();

            int cols, rows;
            if(searchtext is null)
            {
                switch (view)
                {
                    case View.Benutzer:
                        break;
                    case View.StatischePosten:
                        cols = 3;
                        rows = statics.Count + 2;
                        break;
                    case View.VariablePosten: 
                        break;
                    case View.Jubilaeen:
                        break;
                    case View.JubilaeenAktuell:
                        break;
                    case View.GesamtAktuell:
                        break;
                }
            }
            else
            {
                switch (view)
                {
                    case View.Benutzer:
                        break;
                    case View.StatischePosten:
                        break;
                    case View.VariablePosten:
                        break;
                    case View.Jubilaeen:
                        break;
                    case View.JubilaeenAktuell:
                        break;
                    case View.GesamtAktuell:
                        break;
                }
            }
        }

        private void WriteStatics(List<Static> s)
        {

        }

        private List<Static> SortStatics(List<Static> unsorted, int left, int right) 
        {
            var i = left;
            var j = right;
            var pivot = unsorted[left];

            while(i <= j)
            {
                while (unsorted[i].Name < pivot.Name)
            }
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
        private List<int> SearchJubs(string query)
        {
            List<int> search = new List<int>();
            foreach (var s in jubilaeen)
            {
                if (s.ToString().Contains(query)) search.Add(s);
            }
            return search;
        }

        private void ClickVWB(object sender, RoutedEventArgs e)
        {
            if (view != View.Benutzer)
            {
                view = View.Benutzer;
                searchtext = null;
                TBSearch.Text = searchtext;
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
                TBSearch.Text = searchtext;
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
                TBSearch.Text = searchtext;
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
                TBSearch.Text = searchtext;
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
                TBSearch.Text = searchtext;
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
                TBSearch.Text = searchtext;
                TBView.Text = "Aktuelle Ansicht: Gesamt";
                LoadControls();
            }
        }

        private void SearchTextChanged(object sender, KeyEventArgs e)
        {
            searchtext = TBSearch.Text;
            LoadControls();
        }
    }
}
