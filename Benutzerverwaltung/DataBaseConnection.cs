using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Benutzerverwaltung
{
    public class DataBaseConnection
    {
        const string connection = @"Data Source=DB/Datenbank.sqlite3;Version=3;";
        const string file = @"DB/Datenbank.sqlite3";
        const string directory = "DB";

        public struct QueryDataTable
        {
            public string? error;
            public DataTable? solution;
        }

        public struct QueryList
        {
            public string? error;
            public List<List<object>>? solution;
        }

        public struct Date
        {
            public int day;
            public int month;
            public int year;

            public Date(string date)
            {
                //0123-56-89
                //01-34-6789
                if (date.Length != 10) throw new Exception(string.Format("cannot convert string {0} to date!", date));
                if (!ConfigWindow.IsNumeric(date[4]) && !ConfigWindow.IsNumeric(date[7])
                    && ConfigWindow.IsNumeric(date[0]) && ConfigWindow.IsNumeric(date[1]) && ConfigWindow.IsNumeric(date[2]) && ConfigWindow.IsNumeric(date[3])
                    && ConfigWindow.IsNumeric(date[5]) && ConfigWindow.IsNumeric(date[6]) && ConfigWindow.IsNumeric(date[8]) && ConfigWindow.IsNumeric(date[9]))
                {
                    string y = date[0].ToString() + date[1].ToString() + date[2].ToString() + date[3].ToString();
                    string m = date[5].ToString() + date[6].ToString();
                    string d = date[8].ToString() + date[9].ToString();
                    year = Convert.ToInt32(y);
                    month = Convert.ToInt32(m);
                    day = Convert.ToInt32(d);
                }
                else if(!ConfigWindow.IsNumeric(date[2]) && !ConfigWindow.IsNumeric(date[5])
                    && ConfigWindow.IsNumeric(date[0]) && ConfigWindow.IsNumeric(date[1]) && ConfigWindow.IsNumeric(date[3]) && ConfigWindow.IsNumeric(date[4])
                    && ConfigWindow.IsNumeric(date[6]) && ConfigWindow.IsNumeric(date[7]) && ConfigWindow.IsNumeric(date[8]) && ConfigWindow.IsNumeric(date[9]))
                {
                    string y = date[6].ToString() + date[7].ToString() + date[8].ToString() + date[9].ToString();
                    string m = date[3].ToString() + date[4].ToString();
                    string d = date[0].ToString() + date[1].ToString();
                    year = Convert.ToInt32(y);
                    month = Convert.ToInt32(m);
                    day = Convert.ToInt32(d);
                }
                else
                {
                    throw new Exception(string.Format("cannot convert string {0} to date!", date));
                }
            }

            public override string ToString()
            {
                string y = year.ToString().PadLeft(4, '0');
                string m = month.ToString().PadLeft(2, '0');
                string d = day.ToString().PadLeft(2, '0');
                return string.Format("{0}.{1}.{2}", d, m, y);
            }
        }

        public DataBaseConnection() 
        {
            CreateDatabase();
        }

        private void CreateDatabase()
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            if (!File.Exists(file)) SQLiteConnection.CreateFile(file);

            using(var con = new SQLiteConnection(connection))
            {
                con.Open();
                List<string> sqlcommands = new List<string>
                {
                    @"CREATE TABLE IF NOT EXISTS Benutzer(
                                    BID int PRIMARY KEY NOT NULL,
                                    Name varchar(255) NOT NULL,
                                    Vorname varchar(255) NOT NULL,
                                    Strasse varchar(255) NOT NULL,
                                    PLZ int NOT NULL,
                                    Ort varchar(255) NOT NULL,
                                    Geburtsdatum varchar(10) NOT NULL,
                                    Eintrittsdatum varchar(10) NOT NULL);",

                    @"CREATE TABLE IF NOT EXISTS StatischeRechnungsPosten(
                                    SRPID int PRIMARY KEY NOT NULL,
                                    Beschreibung varchar(255) NOT NULL,
                                    Wert decimal(10,2) NOT NULL,
                                    Def boolean NOT NULL);",

                    @"CREATE TABLE IF NOT EXISTS VariableRechnungsPosten(
                                    VRPID int PRIMARY KEY NOT NULL,
                                    Beschreibung varchar(255) NOT NULL,
                                    Formel varchar(255) NOT NULL,
                                    Def decimal(10,2) NOT NULL);",

                    @"CREATE TABLE IF NOT EXISTS BenutzerStatisch(
                                    BSID int PRIMARY KEY NOT NULL,
                                    BID int NOT NULL,
                                    SRPID int NOT NULL,
                                    Aktiv boolean NOT NULL,
                                    FOREIGN KEY(BID) REFERENCES Benutzer(BID),
                                    FOREIGN KEY(SRPID) REFERENCES StatischeRechnungsPosten(SRPID));",

                    @"CREATE TABLE IF NOT EXISTS BenutzerVariable(
                                    BVID int PRIMARY KEY NOT NULL,
                                    BID int NOT NULL,
                                    VRPID int NOT NULL,
                                    Wert decimal(10,2) NOT NULL,
                                    FOREIGN KEY(BID) REFERENCES Benutzer(BID),
                                    FOREIGN KEY(VRPID) REFERENCES VariableRechnungsPosten(VRPID));",

                    @"CREATE TABLE IF NOT EXISTS Jubilaeum(
                                    JID int PRIMARY KEY NOT NULL,
                                    Jahre int NOT NULL);"
                };

                using (var command = new SQLiteCommand(con))
                {
                    foreach(var sql in  sqlcommands)
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }

                con.Close();
            }
        }

        public string? CommandNonQuery(string sql)
        {
            try
            {
                using (var con = new SQLiteConnection(connection))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public QueryDataTable CommandQueryToDataTable(string sql)
        {
            try
            {
                using(var con = new SQLiteConnection(connection))
                {
                    con.Open();
                    DataTable sol = new();
                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        var adapter = new SQLiteDataAdapter(cmd);
                        adapter.Fill(sol);
                    }
                    con.Close();
                    return new QueryDataTable() { error=null, solution=sol};
                }
            }
            catch (Exception ex)
            {
                return new QueryDataTable() { error=ex.ToString(), solution=null};
            }
        }

        public QueryList CommandQueryToList(string sql)
        {
            try
            {
                using (var con = new SQLiteConnection(connection))
                {
                    con.Open();
                    List<List<object>> rows = new List<List<object>>();
                    using (var cmd = new SQLiteCommand(sql, con))
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            List<object> row = new List<object>();
                            for(int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetValue(i));
                            }
                            rows.Add(row);
                        }
                    }
                    con.Close();
                    return new QueryList() { error = null, solution = rows };
                }
            }
            catch (Exception ex)
            {
                return new QueryList() { error = ex.ToString(), solution = null };
            }
        }
    }
}
