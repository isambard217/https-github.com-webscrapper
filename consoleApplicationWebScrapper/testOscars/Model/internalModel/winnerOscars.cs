using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testOscars.Model.internalModel
{

    /// <summary>
    ///  This class is
    ///  useful when the oscarWinner does not have one film but more 
    ///  such as three
    /// </summary>
    public class winnerOscars
    {

        public static int ID { get; set; }

        public string Name { get; set; }

        public List<string> Films { get; set;  }


        public winnerOscars()
        {
            ID++;
            Films = new List<string>();
        }

    }
}
