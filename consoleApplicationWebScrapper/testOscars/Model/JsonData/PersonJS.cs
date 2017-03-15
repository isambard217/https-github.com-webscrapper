using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testOscars.Model.JsonData
{
    public class PersonJS
    {

        public int ID { get; set; }

        public string name { get; set; }

        public double DOB { get; set; }

        public List<AwardJS> awards { get; set; }

        public List<Link> links { get; set; }
        //

        public PersonJS()
        { 
            
        
        }
    }
}
