using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benutzerverwaltung
{
    public static class Convert
    {
        public static DataBaseConnection.Date StringToDate(string input)
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
        public static double StringToDouble(string input)
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
                return double.Parse(newval, System.Globalization.NumberStyles.AllowDecimalPoint);
            }
            else
            {
                return 0;
            }
        }
        public static decimal StringToDecimal(string input)
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
        public static int StringToInt(string input)
        {
            string output = "";
            foreach (var c in input) if (IsNumeric(c)) output += c;
            return int.Parse(output);
        }
        public static bool IsNumeric(char input)
        {
            if (input == '0' || input == '1' || input == '2' || input == '3' || input == '4' || input == '5' || input == '6' || input == '7' || input == '8' || input == '9') return true; else return false;
        }
    }
}
