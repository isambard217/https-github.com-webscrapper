using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testOscars.Model.JsonData;

namespace testOscars.Model
{
    public class Award
    {

        public int filmID { get; set; }

        public int categoryID { get; set; }

        public string filmWebToken { get; set; }


        public AwardJS ToAwardJS()
        {

            AwardJS awardjs = new AwardJS() { filmID = this.filmID, categoryID = this.categoryID };

            return awardjs;
        }


        public AwardJS ToAwardJS(Award award)
        {

            AwardJS awardjs = new AwardJS() { filmID = award.filmID, categoryID = award.categoryID };

            return awardjs;
        }


    }
}
