using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DateTime Geburtstag;
        public DateTime Eintrittsdatum;

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

        public Variable(int id, string name)
        {
            this.Id = id;
            this.Name = name;
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

    public class UserStatic
    {
        public User u;
        public Static s;
        public bool active;

        public UserStatic(User u, Static s, bool active)
        {
            this.u = u;
            this.s = s;
            this.active = active;
        }
    }

    public class UserVariable
    {
        public User u;
        public Variable v;
        public decimal val;

        public UserVariable(User u, Variable v, decimal val)
        {
            this.u = u;
            this.v = v;
            this.val = val;
        }
    }
}
