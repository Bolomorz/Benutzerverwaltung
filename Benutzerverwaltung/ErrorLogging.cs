using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benutzerverwaltung
{
    static internal class ErrorLogging
    {
        public static void Log(string message)
        {
            DateTime now = DateTime.Now;
            List<string> lines = new List<string>() { string.Format("{0}:\n{1}", now, message), "----------", "", "----------"};
            File.AppendAllLines("errorlog.txt", lines);
        }

        public static void LogSql(string sql)
        {
            DateTime now = DateTime.Now;
            List<string> lines = new List<string>() { string.Format("{0}:\n{1}", now, sql), "----------", "", "----------" };
            File.AppendAllLines("sqllog.txt", lines);
        }
    }
}
