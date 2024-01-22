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
        private List<UserStatic> userStatics;
        private List<UserVariable> userVariables;
        private List<int> jubilaeen;

        private View view;
        public MainWindow()
        {
            dbc = new DataBaseConnection();
            users = new List<User>();
            statics = new List<Static>();
            variables = new List<Variable>();
            userStatics = new List<UserStatic>();
            userVariables = new List<UserVariable>();
            jubilaeen = new List<int>();
            view = View.GesamtAktuell;
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            users.Clear();
            statics.Clear();
            variables.Clear();
            userStatics.Clear();
            userVariables.Clear();
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
                    variables.Add(new Variable((int)v[0], (string)v[1]));
                }
            }
            foreach(var u in users)
            {
                foreach(var s in statics)
                {
                    query = dbc.CommandQueryToList(string.Format("SELECT Aktiv FROM BenutzerStatisch WHERE BID={0} AND SRPID={1};", u.Id, s.Id));
                    if(query.error is not null) Console.Error.WriteLine(query.error.ToString());
                    else if(query.solution is not null)
                    {
                        if(query.solution.Count() > 0)
                        {
                            userStatics.Add(new UserStatic(u, s, (bool)query.solution[0][0]));
                        }
                        else
                        {
                            userStatics.Add(new UserStatic(u, s, false));
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
                            userVariables.Add(new UserVariable(u, v, (decimal)query.solution[0][0]));
                        }
                        else
                        {
                            userVariables.Add(new UserVariable(u, v, (decimal)0.0));
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

        }

        private void ClickVWB(object sender, RoutedEventArgs e)
        {
            if (view != View.Benutzer)
            {
                view = View.Benutzer;
                LoadControls();
            }
        }
        private void ClickVWVP(object sender, RoutedEventArgs e)
        {
            if (view != View.VariablePosten)
            {
                view = View.VariablePosten;
                LoadControls();
            }
        }
        private void ClickVWSP(object sender, RoutedEventArgs e)
        {
            if (view != View.StatischePosten)
            {
                view = View.StatischePosten;
                LoadControls();
            }
        }
        private void ClickVWJ(object sender, RoutedEventArgs e)
        {
            if (view != View.Jubilaeen)
            {
                view = View.Jubilaeen;
                LoadControls();
            }
        }
        private void ClickJub(object sender, RoutedEventArgs e)
        {
            if(view != View.JubilaeenAktuell)
            {
                view = View.JubilaeenAktuell;
                LoadControls();
            }
        }
        private void ClickGesamt(object sender, RoutedEventArgs e)
        {
            if(view != View.GesamtAktuell)
            {
                view = View.GesamtAktuell;
                LoadControls();
            }
        }

        private void SearchTextChanged(object sender, KeyEventArgs e)
        {

        }
    }
}
