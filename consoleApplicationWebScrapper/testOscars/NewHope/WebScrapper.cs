using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using testOscars.Model;
using testOscars.Helpers;
using testOscars.Model.internalModel;

namespace testOscars.NewHope
{
    public class WebScrapper
    {

        private List<string> getWinnerList;
        List<string> categorySections;
        public static List<Film> refinedFilms;
        public static List<Person> actorNameID;
        public static List<Person> secondActorNameID;
        public Person personExample;
        public OscarData od;

        public WebScrapper() { }

        public WebScrapper(string htmlPage, int year)
        {
                   
          categorySections = GetCategorySections(htmlPage);
          getWinnerList = new List<string>();

          // Loop over each category 
          for (int i = 0; i<categorySections.Count; i++)
          {
              /// Have a method to determine which category it is 
              /// Then pass it into the GetWinners method
              int categorySectionID = DetermineTheCategory(categorySections[i]);

              if (year == 1932 && categorySectionID == 2)
              {

                  GetWinnerExceptionFour(categorySections[i], categorySectionID, year);
              }

              if (year == 1969 && categorySectionID == 3)
              {

                  GetWinnerExceptionOne(categorySections[i], categorySectionID, year);
              } else if (year == 1962 && categorySectionID == 1) {

                  GetWinnerExceptionTwo(categorySections[i], categorySectionID, year);
              } else if (year == 2008  && categorySectionID == 1) {

                  GetWinnerExceptionThree(categorySections[i], categorySectionID, year);
              } else {

                  GetWinners(categorySections[i], categorySectionID, year);
              }
                    
           } // End for-loop

         } // End function
          
    
        public static int DetermineTheCategory(string category)
        {

            int categoryItemID = 0;

            if (category.Contains("Best Director") == true || category.Contains("Best Achievement in Directing"))
            {
                categoryItemID = 1;
            }

            if (category.Contains("Best Actor in a Leading Role") == true || category.Contains("Best Performance by an Actor in a Leading Role"))
            {
               categoryItemID = 2;   
            }

            if (category.Contains("Best Actress in a Leading Role") == true || category.Contains("Best Performance by an Actress in a Leading Role"))
            {
                categoryItemID = 3;
            }

            if (category.Contains("Best Actor in a Supporting Role") == true || category.Contains("Best Performance by an Actor in a Supporting Role"))
            {
                categoryItemID = 4;
            }

            if (category.Contains("Best Actress in a Supporting Role") == true || category.Contains("Best Performance by an Actress in a Supporting Role"))
            {
                categoryItemID = 5;
            }
               
            return categoryItemID;
       }
     

  
        // Get the different sections in the Page
        public static List<string> GetCategorySections(string htmlPage)
        {

            List<string> result = new List<string>();

            // Best Actor
            int endIndex;
            int length;
            string endBlockQuote = "</blockquote>";

            string bestActorOne = "Best Performance by an Actor in a Leading Role";
            string bestActorTwo = "Best Actor in a Leading Role";

            int startIndex = htmlPage.IndexOf(bestActorOne);

            if (startIndex == -1)
            {

                startIndex = htmlPage.IndexOf(bestActorTwo);             
            }

            int ending = htmlPage.IndexOf(endBlockQuote, startIndex);

            length =  ending - startIndex;

            string LeadingActorSection =  htmlPage.Substring(startIndex, length);
            result.Add(LeadingActorSection);


            // Best Actress

            string bestActressOne = ">Best Actress in a Leading Role";
            string bestActressTwo = ">Best Performance by an Actress in a Leading Role";

            startIndex = htmlPage.IndexOf(bestActressOne);

            if (startIndex == -1)
            { 
                
                startIndex = htmlPage.IndexOf(bestActressTwo);
            }

            ending = htmlPage.IndexOf(endBlockQuote, startIndex);

            length = ending - startIndex;

            string LeadingActressSection = htmlPage.Substring(startIndex, length);

            result.Add(LeadingActressSection);

            // Best supporting Actor

            string bestSupportingActorOne = "Best Performance by an Actor in a Supporting Role";
            string bestSupportingActorTwo = "Best Actor in a Supporting Role";

            startIndex = htmlPage.IndexOf(bestSupportingActorOne);

            if (startIndex == -1)
            {

                startIndex = htmlPage.IndexOf(bestSupportingActorTwo);
            }
            
            
            if (startIndex == -1)
            {

            // Second option might not exist as well in the year laters best supporting actor did not exist
            
            } else {

                endIndex = htmlPage.IndexOf(endBlockQuote, startIndex);

                length = endIndex - startIndex;

                string SupportingActorSection = htmlPage.Substring(startIndex, length);
                result.Add(SupportingActorSection);
            }


            //Best supporting Actress 

            string bestSupportingActressOne = "Best Performance by an Actress in a Supporting Role";
            string bestSupportingActressTwo = "Best Actress in a Supporting Role";

            startIndex = htmlPage.IndexOf(bestSupportingActressOne);

            if (startIndex == -1) 
            {
            
                startIndex = htmlPage.IndexOf(bestSupportingActressTwo);

            }
            
            if (startIndex == -1)
            {

                // Second option might not exist as well in the year laters best supporting actress did not exist

            } else {


                endIndex = htmlPage.IndexOf(endBlockQuote, startIndex);

                length = endIndex - startIndex;

                string bestSupportingActressSection = htmlPage.Substring(startIndex, length);
                result.Add(bestSupportingActressSection);
            }

            // Best Director

            string BestDirectorSection;
            string bestDirectorOne = "Best Achievement in Directing";
            string bestDirectorTwo = "Best Director, Dramatic Picture";
            string bestDirectorThree = "Best Director";
            string bestDirectorFour = "Best Director, Comedy Picture";
            

            startIndex = htmlPage.IndexOf(bestDirectorOne);
            
            if (startIndex == -1)
            {

                startIndex = htmlPage.IndexOf(bestDirectorTwo);
            }

            if (startIndex == -1)
            {

                startIndex = htmlPage.IndexOf(bestDirectorThree);

                endIndex = htmlPage.IndexOf(endBlockQuote, startIndex);

                length = endIndex - startIndex;

                BestDirectorSection = htmlPage.Substring(startIndex, length);

                result.Add(BestDirectorSection);


            } else if (startIndex != -1)
            {

                endIndex = htmlPage.IndexOf(endBlockQuote, startIndex);

                endIndex = htmlPage.IndexOf(endBlockQuote, startIndex);

                length = endIndex - startIndex;

                BestDirectorSection = htmlPage.Substring(startIndex, length);

                result.Add(BestDirectorSection);
                
            
            }

                // There is a case where 1929 has two best director awards
                // so use this technique
                int directorOfComedy = htmlPage.IndexOf(bestDirectorFour);

                if (directorOfComedy != -1)
                {

                    int endDirectorOfComedy = htmlPage.IndexOf(endBlockQuote, directorOfComedy);

                    length = endDirectorOfComedy - directorOfComedy;

                    string BesterDirectorSectionTwo = htmlPage.Substring(directorOfComedy, length);

                    result.Add(BesterDirectorSectionTwo);

                }

            return result;

        }

        // 1969 
        //Best Actress in a Leading Role
        // There are two winners 
        // Katharine Hepburn && Barbra Streisand
        public void GetWinnerExceptionOne(string htmlSection, int categorySectionID, int year)
        {

            string startPoint = "<h3>WINNER</h3>";
            string endPoint = "<h3>NOMINEES</h3>";
            string firstSection = "<strong>";
            string sectionTwo = "</strong>";
            int firstStrong;

            int secondSection;
            int startIndex = htmlSection.IndexOf(startPoint);

            if (startIndex == -1)
            {
                startIndex = htmlSection.IndexOf("<h3>WINNERS</h3>");
            }

            int endIndex = htmlSection.IndexOf(endPoint, startIndex);

            if (endIndex == -1)
            {

                endIndex = htmlSection.IndexOf("<h3>NOMINEE</h3>", startIndex);
            }

            int length = endIndex - startIndex;

            // This variable contains winners actor name and film name
            string winnerSection = htmlSection.Substring(startIndex, length);

            int startStringLength = firstSection.Length;

            startIndex = winnerSection.IndexOf(firstSection);
            endIndex = winnerSection.IndexOf(sectionTwo);

            length = endIndex - startIndex;

            string fileTitleName = winnerSection.Substring(startIndex, length);

            List<string> temp = ReturnNumberOfFilms(fileTitleName);

            List<Film> filmTemp = RefineFilmsArray(temp, year);

             // After we get the name of the Film we also get the Actor name as well
            int whereToCut = winnerSection.IndexOf("<a", endIndex);
            int whereToStopCut = winnerSection.IndexOf("</a>", endIndex);

            int whereToStop = whereToStopCut - whereToCut;

            string actorName = winnerSection.Substring(whereToCut, whereToStop);

            // ACTOR array use a simple model first. Then later on add the model when it has more information to the actorNameID array
            actorNameID = returnActorNameID(actorName);


            // Grab the second Winner

            int hrTag = winnerSection.IndexOf("<hr style");
            int smallTag = winnerSection.IndexOf("</small>", hrTag);

            string secondSectionTemp = winnerSection.Substring(hrTag, smallTag - hrTag);

            int startPlace = secondSectionTemp.IndexOf("<strong>");
            int endPlace = secondSectionTemp.IndexOf("</strong", startPlace);

            string secondWinnerFilm = secondSectionTemp.Substring(startPlace, endPlace - startPlace);

            List<string> films = ReturnNumberOfFilms(secondWinnerFilm);

            List<Film> secondFilmTemp = RefineFilmsArray(films, year);
            //winnerSection.Substring();

            int test = secondSectionTemp.IndexOf("<a", endPlace);
            int testEnd = secondSectionTemp.IndexOf("</a>", test);

            string secondActorName = secondSectionTemp.Substring(test, testEnd - test);
            
            // ACTOR array use a simple model first. Then later on add the model when it has more information to the actorNameID array
            secondActorNameID = returnActorNameID(secondActorName);

            CreatePersonAndFilm(secondFilmTemp, secondActorNameID, categorySectionID);
            CreatePersonAndFilm(filmTemp, actorNameID, categorySectionID);

        
        }


        /*
          This creates a Person then creates the films
           It also then makes a request for the user's image
           and stores it in the cache
         
         */
        public void CreatePersonAndFilm(List<Film> refinedFilms, List<Person> actorNameID, int categorySectionID)
        { 
        
            foreach (var actor in actorNameID)
            {
                foreach (var film in refinedFilms)
                {
                    actor.awards.Add(new Award()
                    {

                        categoryID = categorySectionID,
                        filmID = film.FID,
                        filmWebToken = film.webIDToken
                                          
                    });

                    if (DataRepository.allFilms.Find(q => q.webIDToken.Equals(film.webIDToken)) == null)
                    {

                        DataRepository.AddFilm(film);
                      } else {

                            Debug.WriteLine("The film exists");
                    }

                };

                  actor.DOB = GetDateOfBirthForWinner(actor.nameToken);

                  List<Person> checkIfUserExists = DataRepository.GetAllPersons();

                  if (checkIfUserExists.Contains(actor) == true)
                  {
                      Debug.WriteLine("The actor exists");
                  } else {
                      DataRepository.AddPerson(actor);
                  }

                  //Recently Added 
                  OscarData od = new OscarData(actor.ID);
                  string htmlPage = OscarsAwardHelper.GetActorsPage(actor.nameToken);
                  string[] profileImageUrl = ImageHelper.GetProfileImageUrl(htmlPage, actor);
                  if (profileImageUrl != null)
                  {
                    od.RequestImage(profileImageUrl);
                  }

            }

        }
        
        
        // 1962
        // There are two winners side-by-side for the same Film
        // The film is Best Director by West Side Story 
        // Best Director
        // Winners: Robert Wise and Jerome Robbins
        public void GetWinnerExceptionTwo(string htmlSection, int categorySectionID, int year)
        {

            string startPoint = "<h3>WINNER</h3>";
            string endPoint = "<h3>NOMINEES</h3>";
            string firstSection = "<strong>";
            string sectionTwo = "</strong>";
            int firstStrong;

            int secondSection;
            int startIndex = htmlSection.IndexOf(startPoint);

            if (startIndex == -1)
            {
                startIndex = htmlSection.IndexOf("<h3>WINNERS</h3>");
            }

            int endIndex = htmlSection.IndexOf(endPoint, startIndex);

            if (endIndex == -1)
            {

                endIndex = htmlSection.IndexOf("<h3>NOMINEE</h3>", startIndex);
            }

            int length = endIndex - startIndex;

            // This variable contains winners actor name and film name
            string winnerSection = htmlSection.Substring(startIndex, length);

            int startPlace = winnerSection.IndexOf("<strong>");
            int endPlace = winnerSection.IndexOf("</strong>");

            string temp = winnerSection.Substring(startPlace, endPlace - startPlace);

            List<string> films = ReturnNumberOfFilms(temp);

            List<Film> secondFilmTemp = RefineFilmsArray(films, year);

            int strongStart = winnerSection.IndexOf("</strong>");

            int strongEnd = winnerSection.IndexOf("<small", strongStart);

            string twoActors = winnerSection.Substring(strongStart, strongEnd - strongStart);

            // Remove strong tag at the beginning

            twoActors = twoActors.Remove(0, 9);
            int size = twoActors.Length;

            twoActors = twoActors.Remove(87, 4);
        
            string[] bothActors = twoActors.Split(',');

            List<Person> firstActor = new List<Person>();
            List<Person> secondActor = new List<Person>();

            for (int i = 0; i < bothActors.Length; i++)
            {
                if (i == 0)
                {
                    bothActors[i] = bothActors[i].Remove(39, 4);

                    firstActor = returnActorNameID(bothActors[i]);
                    
                } else if(i == 1) {

                    bothActors[i] = bothActors[i].Trim();

                  secondActor = returnActorNameID(bothActors[i]);
                }

              }

            CreatePersonAndFilm(secondFilmTemp, firstActor, categorySectionID);
            CreatePersonAndFilm(secondFilmTemp, secondActor, categorySectionID);


        }


        //2008
        // Best Achievement in Directing
        // Two Directors have won an oscar there is a ty between the Winners
        // Winners: Ethan Coen and Joel Coen
        public void GetWinnerExceptionThree(string htmlSection, int categorySectionID, int year)
        {


            string startPoint = "<h3>WINNER</h3>";
            string endPoint = "<h3>NOMINEES</h3>";

            int secondSection;
            int startIndex = htmlSection.IndexOf(startPoint);

            if (startIndex == -1)
            {
                startIndex = htmlSection.IndexOf("<h3>WINNERS</h3>");
            }

            int endIndex = htmlSection.IndexOf(endPoint, startIndex);

            if (endIndex == -1)
            {

                endIndex = htmlSection.IndexOf("<h3>NOMINEE</h3>", startIndex);
            }

            int length = endIndex - startIndex;


            // This variable contains winners actor name and film name
            string winnerSection = htmlSection.Substring(startIndex, length);


            int startPlace = winnerSection.IndexOf("<strong>");
            int endPlace = winnerSection.IndexOf("</strong>");

            string temp = winnerSection.Substring(startPlace, endPlace - startPlace);

            List<string> films = ReturnNumberOfFilms(temp);

            List<Film> secondFilmTemp = RefineFilmsArray(films, year);

            int strongStart = winnerSection.IndexOf("</strong>");

            int strongEnd = winnerSection.IndexOf("</div>", strongStart);

            string twoActors = winnerSection.Substring(strongStart, strongEnd - strongStart);

            // Remove strong tag at the beginning

            twoActors = twoActors.Remove(0, 9);
            int size = twoActors.Length;

            string[] bothActors = twoActors.Split(',');

            List<Person> firstActor = new List<Person>();
            List<Person> secondActor = new List<Person>();

            for (int i = 0; i < bothActors.Length; i++)
            {

                if (i == 0)
                {
                    bothActors[i] = bothActors[i].Remove(38, 4);
                    firstActor = returnActorNameID(bothActors[i]);
                    CreatePersonAndFilm(secondFilmTemp, firstActor, categorySectionID);
                   

                } else if (i == 1) {

                    bothActors[i] = bothActors[i].Trim();
                    bothActors[i] = bothActors[i].Remove(37, 4);
                    secondActor = returnActorNameID(bothActors[i]);
                    CreatePersonAndFilm(secondFilmTemp, secondActor, categorySectionID);
                    
                }

            }

        }

        public void GetWinnerExceptionFour(string htmlSection, int categorySectionID, int year)
        {

            string startPoint = "<h3>WINNER</h3>";
            string endPoint = "<h3>NOMINEES</h3>";

            int secondSection;
            int startIndex = htmlSection.IndexOf(startPoint);

            if (startIndex == -1)
            {
                startIndex = htmlSection.IndexOf("<h3>WINNERS</h3>");
            }

            int endIndex = htmlSection.IndexOf(endPoint, startIndex);

            if (endIndex == -1)
            {

                endIndex = htmlSection.IndexOf("<h3>NOMINEE</h3>", startIndex);
            }

            int length = endIndex - startIndex;


            // This variable contains winners actor name and film name
            string winnerSection = htmlSection.Substring(startIndex, length);

            int startPlace = winnerSection.IndexOf("<strong>");
            int endPlace = winnerSection.IndexOf("</strong>");

            string firstWinnerFilm = winnerSection.Substring(startPlace, endPlace - startPlace);

            List<string> tempx = ReturnNumberOfFilms(firstWinnerFilm);

            List<Film> tempy = RefineFilmsArray(tempx, year);

            int firstWinnerActor = winnerSection.IndexOf("<a", endPlace);
            int firstActorWinnerTwo = winnerSection.IndexOf("</a>", firstWinnerActor);

            string ActorOne = winnerSection.Substring(firstWinnerActor, firstActorWinnerTwo - firstWinnerActor);
            
            List<Person> FirstActorList = returnActorNameID(ActorOne);

            int secondWinnerStartPlace = winnerSection.IndexOf("<strong>", endPlace);
            int secondWinnerEndPlace = winnerSection.IndexOf("</a>", secondWinnerStartPlace);

            string atab = "</a>";
            int atagLength = atab.Length;

            string secondWinnerFilm = winnerSection.Substring(secondWinnerStartPlace, secondWinnerEndPlace - secondWinnerStartPlace + atagLength);

            StringBuilder sb = new StringBuilder();

            sb.Append(secondWinnerFilm);
            sb.Append(":");

            sb.ToString();


            List<string> tempa = ReturnNumberOfFilms(sb.ToString());
            List<Film> tempb = RefineFilmsArray(tempa, year);


            int secondWinnerActor = winnerSection.IndexOf("<a", secondWinnerEndPlace);
            int secondActorWinnerTwo = winnerSection.IndexOf("</a>", secondWinnerActor);

            string ActorTwo = winnerSection.Substring(secondWinnerActor, secondActorWinnerTwo - secondWinnerActor);

            List<Person> SecondActorList = returnActorNameID(ActorTwo);


            CreatePersonAndFilm(tempy, FirstActorList, categorySectionID);
            CreatePersonAndFilm(tempb, SecondActorList, categorySectionID);

        
        }


        // One single function called grab winners 
        public void GetWinners(string htmlSection, int categorySectionID, int year)
        {

            string startPoint = "<h3>WINNER</h3>";
            string endPoint = "<h3>NOMINEES</h3>";

            int secondSection;
            int startIndex = htmlSection.IndexOf(startPoint);

            if ( startIndex == -1 )
            {
                startIndex = htmlSection.IndexOf("<h3>WINNERS</h3>");
            }

            int endIndex = htmlSection.IndexOf(endPoint, startIndex);

            if (endIndex == -1)
            {

                endIndex = htmlSection.IndexOf("<h3>NOMINEE</h3>", startIndex);
            }

            int length = endIndex - startIndex;

            // This variable contains winners actor name and film name
            string winnerSection = htmlSection.Substring(startIndex, length);

            int firstSection = winnerSection.IndexOf("<strong>");
            secondSection = winnerSection.IndexOf("</strong>");

            int innerLength = secondSection - firstSection;

            string filmName = winnerSection.Substring(firstSection, innerLength);

            // There could be more than one film so we need to check
            List<string> films = ReturnNumberOfFilms(filmName);

            // FILM array
            List<Film> refinedFilms = RefineFilmsArray(films, year);

            if (refinedFilms == null)
            {

                Debug.WriteLine("refinedFilms");
            }

            // After we get the name of the Film we also get the Actor name as well
            int whereToCut = winnerSection.IndexOf("<a", secondSection);
            int whereToStopCut = winnerSection.IndexOf("</a>", secondSection);

            int whereToStop = whereToStopCut - whereToCut;

            string actorName = winnerSection.Substring(whereToCut, whereToStop);

            // ACTOR array use a simple model first. 
            //Then later on add the model when it has more information to the actorNameID array
            actorNameID = returnActorNameID(actorName);

            foreach (var actor in actorNameID)
            {

                    foreach (var film in refinedFilms)
                    {

                        actor.awards.Add(new Award()
                        {

                            categoryID = categorySectionID,
                            filmID = film.FID,
                            filmWebToken = film.webIDToken
                        });

                            if (DataRepository.allFilms.Find(q => q.webIDToken.Equals(film.webIDToken)) == null)
                            {

                                DataRepository.AddFilm(film);
                            } else {

                                Debug.WriteLine("The film exists");
                            }         
                    };

                  string htmlPage = OscarsAwardHelper.GetActorsPage(actor.nameToken);

                  if (htmlPage.Contains("\"title=\"No photo available.\"") == true)
                  {

                      Debug.WriteLine("Problem");
                  }

                  actor.DOB = GetDateOfBirthForWinner(htmlPage);

                  // Create a function that gets the Actor's profile image once you have the Image 
                  // This array contains the url to the Page [0] 
                  // It also contains whether it is a profile or media image [1]
                  // Contains Actor's id as well [2]
                  string[] profileImageUrl = ImageHelper.GetProfileImageUrl(htmlPage, actor);

                  if (profileImageUrl[0].Length < 20 || profileImageUrl[0].Length < 100)
                  {

                      Debug.WriteLine("PROBLEM");
                  }

                  OscarData od = new OscarData(actor.ID);

                  // If statement is a DEBUG test
                  if (profileImageUrl[1].Equals("media") == true || profileImageUrl == null)
                  {

                      Debug.WriteLine("media items");
                  }


                  if (profileImageUrl != null)
                  {

                     od.RequestImage(profileImageUrl);
                  }
                                 
                  List<Person> checkIfUserExists = DataRepository.GetAllPersons();

                  if (checkIfUserExists.Contains(actor) == true)
                  {

                      Debug.WriteLine("The actor exists");
                  } else {

                      DataRepository.AddPerson(actor);
                  }
            }

        }

       public string GetHtmlPage(string url)
       {
     
           WebClient client = new WebClient();
           Stream datastream = client.OpenRead(url);
           StreamReader reader = new StreamReader(datastream);
           StringBuilder sb = new StringBuilder();
           while (!reader.EndOfStream)
               sb.Append(reader.ReadLine());
           string htmlPage = sb.ToString();


           return htmlPage;
       
       }
                    



        // Used to only receive ID however it now received ID's 
        // as well as html Text
        public static double GetDateOfBirthForWinner(string htmlPage)
        {

            // check whether we passed in a html page as a parameter no a nameToken
            if ( htmlPage.Length < 20 )
            {
                OscarData oscarData = new OscarData();
                string url = "http://www.imdb.com/name/" + htmlPage;
                htmlPage = oscarData.RequestFile(url);
            }


            string startElement = "<div id=\"name-born-info\"";
            string endElement = "</div>";

            // Get DateOfBirth Section
            int startIndex = 0;
            int endIndex = 0;
            int length = 0;

            startIndex = htmlPage.IndexOf(startElement);
            endIndex = htmlPage.IndexOf(endElement, startIndex);

            length = endIndex - startIndex;

            string refinedDOB = htmlPage.Substring(startIndex, length);

            startElement = "<time datetime=\"";
            endElement = "itemprop";

            startIndex = refinedDOB.IndexOf(startElement);
            endIndex = refinedDOB.IndexOf(endElement, startIndex);
            startIndex += startElement.Length;

            length = endIndex - startIndex;

            string resultDOB = refinedDOB.Substring(startIndex, length);

            resultDOB = OscarsAwardHelper.cleanUpString(resultDOB);

            string year = resultDOB.Substring(0, 4);
            string month = resultDOB.Substring(5, 1);
            string day;

            try
            {
                day = resultDOB.Substring(7, 2);
            } catch {
                day = resultDOB.Substring(7, 1);
            
            }

            var dateTime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day), 0, 0, 0, DateTimeKind.Local);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixDateTime = (dateTime.ToUniversalTime() - epoch).TotalSeconds;

            return unixDateTime;
        
        }


        /*
         * FilmModel used here 
         */
        public static List<Film> RefineFilmsArray(List<string> filmsToRefine, int year)
        {

            List<Film> result = new List<Film>();
            Film filmModel;
            string startPoint = ">";
            string endPoint = "<";
            int startIndex;
            int endIndex;
            int length;
            string filmName;
            int additionalParam;
            string webTokenId;
        

            for (int i = 0; i < filmsToRefine.Count; i++)
            {

                filmModel = new Film();

                startPoint = ">";
                endPoint = "<";

                startIndex = filmsToRefine[i].IndexOf(startPoint);
                endIndex = filmsToRefine[i].IndexOf(endPoint, startIndex);

                additionalParam = startPoint.Length;

                length = endIndex - startIndex - additionalParam;

                startIndex += 1;

                filmName = filmsToRefine[i].Substring(startIndex, length);

                // Get the filmName and add it to the Model
                filmModel.name = filmName;


                // Set the year field and add it to the Model
                filmModel.year = year;

                // Once you have the film name you can then get the film webtokenid
                startPoint = "title/";
                endPoint = "/";

                startIndex = filmsToRefine[i].IndexOf(startPoint);

                startIndex = startIndex + startPoint.Length;

                endIndex = filmsToRefine[i].IndexOf(endPoint, startIndex );

                length = endIndex - startIndex;

                webTokenId = filmsToRefine[i].Substring(startIndex, length);

                filmModel.webIDToken = webTokenId;

                // Check if Film exists in FilmRepo
                if (DataRepository.allFilms.Find(q => q.name.Equals(filmModel.name)) != null )
                {

                    Film film = DataRepository.GetFilmByName(filmModel.name); //OscarsAwardHelper.RemoveUnicodeCharacter(
                    Debug.WriteLine("There is a film match");
                    result.Add(film);

                } else {

                    result.Add(filmModel);
                }
                
            }

            return result;
        }


        /* There is only one winner. The reason we return a List is 
         * because it contains the ID to the actor's page
         */
        public static List<Person> returnActorNameID(string actorUnrefined) 
        {

            string startPoint = "<a";
            string endPoint = ">";
            int startIndex;
            int endIndex;
            int length;

            Person model = new Person();
            List<Person> result = new List<Person>();

            startIndex = actorUnrefined.IndexOf(startPoint);
            endIndex = actorUnrefined.IndexOf(endPoint);

            int additionalParam = endPoint.Length;

            length = endIndex - startIndex + additionalParam;

            string temporary = actorUnrefined.Substring(startIndex, length);

            int nameLength = actorUnrefined.Length - temporary.Length;

            string winnerName = actorUnrefined.Substring(length);

            // Extract the name ID
            //<a  href="/name/nm0417837/">Emil Jannings

            string extractName = "name/";
            string extractSlash = "/";

            int extractNameStartIndex = actorUnrefined.IndexOf(extractName);
            int extractNameLength = extractName.Length;
            extractNameStartIndex += extractNameLength;

            int extraSlashIndex = actorUnrefined.IndexOf(extractSlash, extractNameStartIndex);

            int imbetween = extraSlashIndex - extractNameStartIndex;

            string nameCode = actorUnrefined.Substring(extractNameStartIndex, imbetween);

            // Check if the Winner Name has unicode characters before we assign it to the model.Name property
            if (winnerName.Contains('&') || winnerName.Contains(';'))
            {

               winnerName = OscarsAwardHelper.RemoveUnicodeCharacter(winnerName);
            }

            // Add the winner's ID and code name  
            model.name = winnerName;
            model.nameToken = nameCode;

            Person person = DataRepository.allPersons.Find(f => f.name == model.name);

            if (person == null)
            { // No one was found add person

                result.Add(model);
                //DataRepository.allPersons.Add(model);
            
            } else {
             // A person was found in the list don't add them
               // result.Add(model);
                result.Add(DataRepository.FindByPersonByName(person.name));
               
            }
           
            return result;

        }

        public static List<string> ReturnNumberOfFilms(string filmName) 
        {

            bool isFirstTime = true;
            bool stopWhile = false;
            int startIndex = 0;
            int endIndex;
            int length;

            List<string> result = new List<string>();

            do {

                string startPoint = "<a";
                string endPoint = "</a>";

                if (isFirstTime == true)
                {

                    startIndex = filmName.IndexOf(startPoint);
                }

                endIndex = filmName.IndexOf(endPoint, (startIndex));
         

                if (endIndex != -1)
                {
                    // Add the endPoint onto the Length 
                    int additionalPlus = endPoint.Length;
                    length = endIndex - startIndex + additionalPlus;
                    string aFilmName = filmName.Substring(startIndex, length);

                    if (aFilmName.IndexOf(" ") == 0)
                    {
                       aFilmName = aFilmName.Remove(0,1);
                    }
                    
                    result.Add(aFilmName);
                    startIndex = endIndex + additionalPlus + 1;
                    isFirstTime = false;

                } else if (endIndex == -1) {

                    stopWhile = true;
                }

            } while (stopWhile == false);


            return result;
        }


        public void GetLinksForWinnersTwo(Person person)
        {

            OscarData oscarData = new OscarData();

            List<Film> films = DataRepository.GetAllFilms();

            foreach (var film in films) // Loop through all of the films
            {

                string url = "http://www.imdb.com/title/" + film.webIDToken + "/fullcredits?ref_=tt_cl_sm#cast";

                string htmlPage = oscarData.RequestFile(url);

                List<Person> castMembers = OscarsAwardHelper.ReturnCastForThatFilm(htmlPage);
                List<Person> oscarWinnerList = DataRepository.GetAllPersons();

                    // The CastMember list needs to contain the Person even though they might not have won an award
                           
                    // Check that the person is on the film's cast
                    bool containsPerson = false;
                    
                    foreach (var cast in castMembers) // we check the Cast to make sure that the person is on it
                    {
                       if (cast.nameToken.Equals(person.nameToken) && cast.name.Equals(person.name))
                       {
                           containsPerson = true;
                       }
                    }

                    if (containsPerson == true)
                    {

                        //Check whether any of the cast are oscar winners
                        foreach (var cast in castMembers)
                        {

                            foreach (var oscarWinner in oscarWinnerList)
                            {

                                // Looking for a match between the cast and the oscar winner by name and nameToken
                                // The person who you have passed in; their name should not be matched
                                if (cast.name.Equals(oscarWinner.name) && cast.nameToken.Equals(oscarWinner.nameToken) && person.name != cast.name) 
                                {

                                    Person personTemp = DataRepository.FindByPersonByName(oscarWinner.name);

                                    person.links.Add(new Link()
                                    { 
                                        filmID = film.FID,
                                        PersonID = oscarWinner.ID

                                    });

                                    /*foreach (var award in personTemp.awards) // iterate over links not awards
                                    {

                                        person.links.Add(new Link()
                                            {
                                                filmID = film.FID,
                                                PersonID = oscarWinner.ID

                                            });
                                    }*/

                                }
                            }

                        }// End foreach

                    }// End if

               } // End foreach
                
        } // End function

        



        public void GetLinksForWinner(Person person)
        {

            OscarData oscarData = new OscarData();

            List<Person> listOfPerson = new List<Person>();

            foreach (var award in person.awards)
            {

                    string url = "http://www.imdb.com/title/" + award.filmWebToken + "/fullcredits?ref_=tt_cl_sm#cast";
               
                    string htmlPage = oscarData.RequestFile(url);

                    List<Person> castMembers = OscarsAwardHelper.ReturnCastForThatFilm(htmlPage);

                    List<Person> oscarWinnerList = DataRepository.GetAllPersons();
                    int castcounter = 0;
                    int counter = 0;

                    Random rand = new Random();

                    foreach (var oscarWinner in oscarWinnerList)
                    {
                        
                        foreach (var cast in castMembers)
                        {

                            if (oscarWinner.nameToken.Equals(cast.nameToken) && oscarWinner.name.Equals(cast.name) && oscarWinner.name != person.name)
                            {

                               Person personTemp = DataRepository.FindByPersonByName(oscarWinner.name); // Could do find actor by nameID and and also webTokenID

                                for (int i = 0; i<personTemp.awards.Count; i++)
                                {
                                
                                    person.links.Add(new Link()
                                    {

                                       PersonID = personTemp.ID,
                                       filmID = personTemp.awards[i].filmID
                                                                    
                                    });
                                }

                            } else {

                                //Debug.WriteLine("There is no match" + rand.Next());
                             }

                        }// End of ForEach Loop 3
  
                    }// End of ForEach Loop 2

            }// End of ForEach loop 1
                
        }// End Method


    }// End Class
}
