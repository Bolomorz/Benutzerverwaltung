using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Benutzerverwaltung
{
    public enum Einheit { quadratmeter, liter, euro}
    public class User
    {
        public int Id;
        public string Name;
        public string Vorname;
        public string Strasse;
        public int PLZ;
        public string Ort;
        public DateTime Geburtstag;
        public DateTime Eintrittsdatum;
        List<(Static s, bool b)> statics;
        List<(Variable v, decimal w)> variables;

        public User(int id, string name, string vorname, string strasse, int plz, string ort, DateTime geburtstag, DateTime eintrittsdatum) 
        {
            this.Id = id;
            this.Name = name;
            this.Vorname = vorname;
            this.Strasse = strasse;
            this.PLZ = plz;
            this.Ort = ort;
            this.Geburtstag = geburtstag;
            this.Eintrittsdatum = eintrittsdatum;
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

        public Static(int id, string name, decimal wert)
        {
            this.Id = id;
            this.Name = name;
            this.Wert = wert;
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
        public Einheit Einheit;
        public string Formel;

        public Variable(int id, string name, Einheit einheit, string Formel)
        {
            this.Id = id;
            this.Name = name;
            this.Einheit = einheit;
            this.Formel = Formel;
        }

        public decimal CalcValue(decimal input)
        {
            switch (Einheit)
            {
                case Einheit.euro: return input;
                case Einheit.quadratmeter: return 
            }
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
