using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testOscars.Model
{
    public class Person
    {

        public int ID { get; set; }
        public static int PSID = 1;
        public string name { get; set; }
        public string nameToken { get; set; }
        public double DOB { get; set; }
        public List<Award> awards { get; set;  }
        public List<Link> links { get; set; }
        public bool noProfilePicture { get; set; }

        public Person()
        {
            noProfilePicture = false;
            awards = new List<Award>();
            links = new List<Link>();
            ID = PSID;
            PSID++;
        }

        
    }
}
