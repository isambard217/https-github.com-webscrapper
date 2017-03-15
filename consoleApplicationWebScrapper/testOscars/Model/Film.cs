using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testOscars.Model
{
    public class Film
    {

        public static int ID = 1;

        public int FID { get; set; }

        public string name { get; set; }

        public string webIDToken { get; set; }

        public int year { get; set; }

        public Film()
        {

            FID = ID;
            ID++;
        }

        public int getIDValue()
        {

           return FID;
        }

    }

 
}
