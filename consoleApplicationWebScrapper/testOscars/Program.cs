using IMDb_Scraper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using testOscars.NewHope;
using testOscars.Model;
using Newtonsoft.Json;
using testOscars.Model.JsonData;
using testOscars.Helpers;

namespace testOscars
{

    public class Program
    {

        public static void Main(string[] args)
        {

            string url = "http://www.imdb.com/event/ev0000003/";
            int year;          

            OscarData od = new OscarData();
            DataRepository dataRepo = new DataRepository();

            // Define Web-Scrapper
            WebScrapper webScrapper;

            for (year = 1929; year < 2017; year++ ) 
            {
             
                if (year == 1933)
                {
                    // If year is 1933 then don't process data as there is no record for 1933
                } else {

                    if (year == 1930)
                    {

                        for (int i = 1; i < 3; i++)
                        {

                            Debug.WriteLine("The current year is " + year.ToString() + "-" + i.ToString());

                            string file = od.RequestFile(url + year.ToString() + "-" + i.ToString());

                            //string result = od.ProcessData(file, year);
                            webScrapper = new WebScrapper(file, year);

                            // Need to also process Data here as well
                            // Go off and extract info.. haven't finished logic yet
                            //string result = od.ProcessData(file);
                        }

                    } else {

                        // Debug.WriteLine("The current year is " + year.ToString());

                        // This returns the Html contents for requested page.
                        string file = od.RequestFile(url + year.ToString());

                        // Go off and extract info.. haven't finished logic yet
                        //string result = od.ProcessData(file, year);
                        webScrapper = new WebScrapper(file, year);

                    }
                }
            }

            webScrapper = new WebScrapper();
     
            foreach (Film film in DataRepository.allFilms.OrderBy(q => q.name)) 
            {
            
                Debug.WriteLine(film.name);          
            }

             // This part loops the persons again and 
            // Updates the actors by filling in the link property
            foreach (var actor in DataRepository.allPersons)
            {

                webScrapper.GetLinksForWinnersTwo(actor);
                Debug.WriteLine("Loading");
            }

        
            // Get all People and Film once they have been updated
            List<Person> allPersons = DataRepository.GetAllPersons();
            List<Film> allFilms = DataRepository.GetAllFilms();

            IEnumerable<Person> personResults = DataRepository.allPersons.Where(q => q.links.Count != 0);

            int howManyWithLinks = personResults.Count();

            ///////////////////////////////////////////////
            // Build the JS file //
            //////////////////////////////////////////////

            // Build the PersonData array
            List<PersonJS> personjs = new List<PersonJS>(); // empty to start with

           
            foreach (var person in allPersons.OrderBy(q => q.name))
            {

                personjs.Add(
                        new PersonJS
                        {
                            ID = person.ID,
                            links = person.links,
                            name = OscarsAwardHelper.RemoveUnicodeCharacter(person.name),
                            DOB = person.DOB,
                            // awards = person.awards.Select( x => new AwardJS{categoryID = x.categoryID, filmID = x.filmID}),// The types are not equal: Person contains filmWebToken whereas PersonJS does not
                            awards = person.awards.Select(x => x.ToAwardJS()).ToList(),

                        });

            } // End foreach
            


            /*
            foreach (var person in allPersons)
            {

                foreach (var award in person.awards)
                {
                    personjs.Add(
                        new PersonJS
                        {
                            ID = person.ID,
                            links = person.links,
                            name = person.name,
                            DOB = person.DOB,
                            // awards = person.awards.Select( x => new AwardJS{categoryID = x.categoryID, filmID = x.filmID}),// The types are not equal: Person contains filmWebToken whereas PersonJS does not
                            awards = person.awards.Select(x => x.ToAwardJS()).ToList(),

                        });

                } // End foreach
            }// End foreach
            */

            // Remove any duplicates in the person array


            // Build film array
            List<FilmJS> filmjs = new List<FilmJS>();

            foreach (var film in allFilms.OrderBy(q => q.name))
            {

                film.name = OscarsAwardHelper.RemoveUnicodeCharacter(film.name);
                Debug.WriteLine(film.name);           
            }


            foreach (var film in allFilms.OrderBy(q => q.name))
            {
                filmjs.Add(new FilmJS()
                {

                     ID = film.FID,
                     name = OscarsAwardHelper.RemoveUnicodeCharacter(film.name),
                     year = film.year
                });

                Debug.WriteLine(film.name);


            }// End foreach


            
          
            // Build category array
           List<CategoryJS> categoryjs = new List<CategoryJS>();

           categoryjs.Add(new CategoryJS()
           {
               ID = 1,
               name = "Best Achievement in Directing"
           });

           categoryjs.Add(new CategoryJS()
           {
               ID = 2,
               name = "Best Performance by an Actor in a Leading Role"
           });

           categoryjs.Add(new CategoryJS()
           {
               ID = 3,
               name = "Best Performance by an Actress in a Leading Role"
           });

           categoryjs.Add(new CategoryJS()
           {
               ID = 4,
               name = "Best Performance by an Actor in a Supporting Role"
           });

           categoryjs.Add(new CategoryJS()
           {
               ID = 5,
               name = "Best Performance by an Actress in a Supporting Role"
           });


            // Build Person array
            StringBuilder personSB = new StringBuilder();
            bool firstTime = true;
            bool linkFirstTime = true;
            bool firstPerson = true;
          
            personSB.Append(" var personData = [");

            foreach (var person in personjs.OrderBy(a => a.ID))
            {

                firstTime = true;
                linkFirstTime = true;
                

                if (firstPerson == true)
                {
                    personSB.Append("{");
                }else{
                    personSB.Append(",{");
                }
                personSB.AppendFormat("ID:{0}", person.ID);
                personSB.AppendFormat(",name:\"{0}\"", person.name);
                personSB.AppendFormat(",DOB:{0}", person.DOB);
                personSB.Append(",awards:[");

                foreach (var award in person.awards)
                {
                    if (firstTime == true)
                    {
                        personSB.Append("{");
                    } else {
                        personSB.Append(",{");
                    }

                    personSB.AppendFormat("filmID:{0}", award.filmID);
                    personSB.AppendFormat(",categoryID:{0}", award.categoryID);
                    personSB.Append("}");

                    firstTime = false;
                }

                personSB.Append("],");
                personSB.Append("links:[");

                foreach(var link in person.links)
                {

                    if (linkFirstTime == true)
                    {
                        personSB.Append("{");

                    }  else {
                        personSB.Append(",{");
                    } 
                   
                    personSB.AppendFormat("personID:{0}", link.PersonID);
                    personSB.AppendFormat(",filmID:{0}", link.filmID);
                    personSB.Append("}");

                    linkFirstTime = false;

               }

                personSB.Append("]"); // close the Link array
                personSB.Append("}"); // close the person Object

                firstPerson = false;
              
            //    personSB.Append("},");
               
            }

            personSB.Append("];");

            personSB.ToString();

            // Build the Film Array
            StringBuilder filmSB = new StringBuilder();

            filmSB.Append("var filmData = [");

            bool isFirstFilm = true;

            foreach (var film in filmjs)
            {
                if (isFirstFilm == true)
                {
                    filmSB.Append("{");
                } else {
                    filmSB.Append(",{");
                }
                filmSB.AppendFormat("ID:{0}", film.ID);
                filmSB.AppendFormat(",name:\"{0}\"", film.name);
                filmSB.AppendFormat(",year:{0}", film.year);
                filmSB.Append("}");

                isFirstFilm = false;
            
            }

            filmSB.Append("];");


            // Build Category array
            StringBuilder categorySB = new StringBuilder();

            bool isCategoryfirst = true;

            categorySB.Append("var categoryData = [");

            foreach (var cat in categoryjs)
            {

                if (isCategoryfirst == true)
                {

                    categorySB.Append("{");
                } else {

                    categorySB.Append(",{");
                }

                categorySB.AppendFormat("ID:{0}", cat.ID);
                categorySB.AppendFormat(",name:\"{0}\"", cat.name);

                categorySB.Append("}");

                isCategoryfirst = false;
            }

            categorySB.Append("];");
                        
            StringBuilder textFile = categorySB.Append(filmSB.ToString());

            StringBuilder textFilePartTwo = personSB.Append(textFile.ToString());

            string resulty = textFilePartTwo.ToString();

            string json = JsonConvert.SerializeObject(resulty);

            // Write JSON File
            System.IO.File.WriteAllText("C:\\Cache\\data.js", personSB.ToString());
   
        }
    }
       
}


