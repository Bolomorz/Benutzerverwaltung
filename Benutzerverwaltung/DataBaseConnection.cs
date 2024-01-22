using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benutzerverwaltung
{
    public class DataBaseConnection
    {
        const string connection = @"Data Source=DB/Datenbank.sqlite3;Version=3;";

        public DataBaseConnection() 
        {
            CreateDatabase();
        }

        private void CreateDatabase()
        {
            if (!Directory.Exists(@"DB")) Directory.CreateDirectory(@"DB");
            if (!File.Exists(@"DB/Datenbank.sqlite3")) SQLiteConnection.CreateFile(@"DB/Datenbank.sqlite3");

            using(var con = new SQLiteConnection(connection))
            {
                con.Open();
                string sqlcommand = @"CREATE TABLE IF NOT EXISTS ....";

                using (var command = new SQLiteCommand(con))
                {
                    command.CommandText = sqlcommand;
                    command.ExecuteNonQuery();
                }

                con.Close();
            }
        }
    }
}
