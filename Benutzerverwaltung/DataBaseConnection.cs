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
                                    BID int PRIMARY KEY AUTO_INCREMENT,
                                    Name varchar(255) NOT NULL,
                                    Vorname varchar(255) NOT NULL,
                                    Strasse varchar(255) NOT NULL,
                                    PLZ int NOT NULL,
                                    Ort varchar(255) NOT NULL,
                                    Geburtsdatum date NOT NULL,
                                    Eintrittsdsatum date NOT NULL);COMMIT;",

                    @"CREATE TABLE IF NOT EXISTS StatischeRechnungsPosten(
                                    SRPID int PRIMARY KEY AUTO_INCREMENT,
                                    Beschreibung varchar(255) NOT NULL,
                                    Wert decimal(10,2) NOT NULL);COMMIT;",

                    @"CREATE TABLE IF NOT EXISTS VariableRechnungsPosten(
                                    VRPID int PRIMARY KEY AUTO_INCREMENT,
                                    Beschreibung varchar(255) NOT NULL);COMMIT;",

                    @"CREATE TABLE IF NOT EXISTS BenutzerStatisch(
                                    BSID int PRIMARY KEY AUTO_INCREMENT,
                                    BID int NOT NULL,
                                    SRPID int NOT NULL,
                                    Aktiv boolean NOT NULL,
                                    FOREIGN KEY(BID) REFERENCES Benutzer(BID),
                                    FOREIGN KEY(SRPID) REFERENCES StatischeRechnungsPosten(SRPID));COMMIT;",

                    @"CREATE TABLE IF NOT EXISTS BenutzerVariable(
                                    BVID int PRIMARY KEY AUTO_INCREMENT,
                                    BID int NOT NULL,
                                    VRPID int NOT NULL,
                                    Wert decimal(10,2) NOT NULL,
                                    FOREIGN KEY(BID) REFERENCES Benutzer(BID),
                                    FOREIGN KEY(VRPID) REFERENCES VariableRechnungsPosten(VRPID));COMMIT;",

                    @"CREATE TABLE IF NOT EXISTS Jubilaeum(
                                    JID int PRIMARY KEY AUTO_INCREMENT,
                                    Jahre int NOT NULL);COMMIT;"
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
