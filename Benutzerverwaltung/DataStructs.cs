﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Data;

namespace Benutzerverwaltung
{
    public class User
    {
        public int Id;
        public string Name;
        public string Vorname;
        public string Strasse;
        public int PLZ;
        public string Ort;
        public DataBaseConnection.Date Geburtstag;
        public DataBaseConnection.Date Eintrittsdatum;
        public decimal Bezahlt;
        public List<(Static s, bool b)> statics;
        public List<(Variable v, decimal w)> variables;

        public User(int id, string name, string vorname, string strasse, int plz, string ort, DataBaseConnection.Date geburtstag, DataBaseConnection.Date eintrittsdatum, decimal bezahlt) 
        {
            this.Id = id;
            this.Name = name;
            this.Vorname = vorname;
            this.Strasse = strasse;
            this.PLZ = plz;
            this.Ort = ort;
            this.Geburtstag = geburtstag;
            this.Eintrittsdatum = eintrittsdatum;
            this.Bezahlt = bezahlt;
            this.statics = new List<(Static s, bool b)>();
            this.variables = new List<(Variable v, decimal w)>();
        }

        public void AddStatic((Static s, bool b) add)
        {
            statics.Add(add);
        }

        public void AddVariable((Variable v, decimal w) add)
        {
            variables.Add(add);
        }

        public override int GetHashCode()
        {
            int hash = (Id + PLZ)/17;
            foreach(var c in Name) hash *= c.GetHashCode();
            foreach (var c in Vorname) hash *= c.GetHashCode();
            return hash;
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || typeof(User) != obj.GetType()) return false;
            User v = (User)obj;
            if (Id != v.Id) return false; else return true;
        }
        public static bool operator ==(User A, User B)
        {
            if (A.Id == B.Id) return true; else return false;
        }
        public static bool operator !=(User A, User B)
        {
            if (A.Id != B.Id) return true; else return false;
        }
    }

    public class Static
    {
        public int Id;
        public string Name;
        public decimal Wert;
        public bool Default;

        public Static(int id, string name, decimal wert, bool defaultvalue)
        {
            this.Id = id;
            this.Name = name;
            this.Wert = wert;
            this.Default = defaultvalue;
        }

        public override int GetHashCode()
        {
            int hash = Id + 17;
            foreach (var c in Name) hash *= c.GetHashCode();
            hash *= Wert.GetHashCode();
            return hash;
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || typeof(Static) != obj.GetType()) return false;
            Static v = (Static)obj;
            if (Id != v.Id) return false; else return true;
        }
        public static bool operator ==(Static A, Static B)
        {
            if (A.Id == B.Id) return true; else return false;
        }
        public static bool operator !=(Static A, Static B)
        {
            if (A.Id != B.Id) return true; else return false;
        }
    }

    public class Variable
    {
        public int Id;
        public string Name;
        public string Formel;
        public decimal Default;

        public Variable(int id, string name, string formel, decimal defaultvalue)
        {
            this.Id = id;
            this.Name = name;
            this.Formel = formel;
            this.Default = defaultvalue;
        }

        public decimal CalcValue(decimal input)
        {
            if (Formel != "W" && Formel != string.Empty)
            {
                string? result = new DataTable().Compute(string.Format(Replace(Formel), input), "").ToString();
                if (result is not null) return Decimal.Parse(result); else return input;
            }
            else
            {
                return input;
            }
        }

        private string Replace(string input)
        {
            string output = string.Empty;
            foreach(var c in input)
            {
                if (c == 'W') output += "{0}"; else if(c != ' ') output += c;
            }
            return output;
        }

        public override int GetHashCode()
        {
            int hash = Id + 17;
            foreach (var c in Name) hash *= c.GetHashCode();
            return hash;
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || typeof(Variable) != obj.GetType()) return false;
            Variable v = (Variable)obj;
            if (Id != v.Id) return false; else return true;
        }
        public static bool operator ==(Variable A, Variable B)
        {
            if (A.Id == B.Id) return true; else return false;
        }
        public static bool operator !=(Variable A, Variable B)
        {
            if (A.Id != B.Id) return true; else return false;
        }
    }

    public class Jubilaeum
    {
        public int Id;
        public int Jahre;

        public Jubilaeum(int id, int jahre)
        {
            this.Id = id;
            this.Jahre = jahre;
        }

        public override int GetHashCode()
        {
            int hash = Id + 17 * Jahre;
            return hash;
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || typeof(Jubilaeum) != obj.GetType()) return false;
            Jubilaeum v = (Jubilaeum)obj;
            if (Id != v.Id) return false; else return true;
        }
        public static bool operator ==(Jubilaeum A, Jubilaeum B)
        {
            if (A.Id == B.Id) return true; else return false;
        }
        public static bool operator !=(Jubilaeum A, Jubilaeum B)
        {
            if (A.Id != B.Id) return true; else return false;
        }
    }
}
