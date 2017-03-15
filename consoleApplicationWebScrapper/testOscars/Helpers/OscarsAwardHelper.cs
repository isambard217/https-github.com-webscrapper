using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using testOscars.Model;
using testOscars.Model.internalModel;
using testOscars.NewHope;
using System.Globalization;

namespace testOscars.Helpers
{
    public class OscarsAwardHelper
    {


        public static string GetActorsPage(string nameTokenID) 
        {

            OscarData oscarData = new OscarData();

            string url = "http://www.imdb.com/name/" + nameTokenID;

            string htmlPage = oscarData.RequestFile(url);

            Debug.WriteLine(htmlPage.Length);

            if (htmlPage.Length < 100000)
            {
                Debug.WriteLine("If statement");
            }
            return htmlPage;
        }


      //  public static List<string> ReturnCastForThatFilm(string htmlPage)
        public static List<Person> ReturnCastForThatFilm(string htmlPage)
        {
            
            // Get the Director section on the cast page.
            int startDirectorSection = htmlPage.IndexOf("<table class=\"simpleTable simpleCreditsTable\">");
            int endDirectorSection = htmlPage.IndexOf("</a>", startDirectorSection);

            string directionSectionPartOne = htmlPage.Substring(startDirectorSection, endDirectorSection - startDirectorSection);

            int directorTag = directionSectionPartOne.IndexOf("<a href");

            string directorName = directionSectionPartOne.Substring(directorTag);

            Person director = RefineDirectorName(directorName);

            List<string> castMembers = new List<string>();

            string startPoint = "<table class=\"cast_list\">";

            string endPoint = "</table>";

            int startIndex = htmlPage.IndexOf(startPoint);

            int endIndex = htmlPage.IndexOf(endPoint, startIndex);

            int length = endIndex - startIndex;

            string result = htmlPage.Substring(startIndex, length);

            List<string> castMembersUnrefined = CastList(result, "odd");

           // List<string> getTheNameOfTheCast = RedefinedCastlist(castMembersUnrefined);// used CastMember for the List Type instead of String

            List<Person> getTheNameOfTheCast = RedefinedCastlist(castMembersUnrefined);

            getTheNameOfTheCast.Add(director);

          //  List<Person> personList = ReturnLinkedCastMembers(getTheNameOfTheCast);

            return getTheNameOfTheCast; 

           // return personList;
        
        }


        public static Person RefineDirectorName(string directorTag) 
        {

            Person person = new Person();

            // Get the name Token 
            string startString = "name";

            int startIndex = directorTag.IndexOf(startString);
            int endIndex = directorTag.IndexOf("/?");

            int length = startString.Length;

            startIndex += length;

            string personNameTag = directorTag.Substring(startIndex, endIndex - startIndex);

            personNameTag = OscarsAwardHelper.cleanUpString(personNameTag);

            person.nameToken = personNameTag;
            
            // Get the name Person name

            string closingTag = ">";

            int startTag = directorTag.IndexOf("<");
            int endTag = directorTag.IndexOf(closingTag);

            int additionalParam = closingTag.Length;

            int directorTagLength = directorTag.Length;

            int difference = directorTagLength - (endTag - startTag);

            string personName = directorTag.Substring(endTag, difference);

            person.name = personName;

            person.name = OscarsAwardHelper.cleanUpString(person.name);
            return person;
        }


        public static List<Person> ReturnLinkedCastMembers(List<Person> castMembers)
        {

            List<Person> personList = new List<Person>();

            // int length = Convert.ToInt32(param.Count);
            foreach (var cast in castMembers)
            {
                personList.Add(new Person()
                {

                    name = cast.name,
                });


            }

            return personList;

        }

        //public static List<Person>ReturnLinkedCastMembers(List<string> castMembers) 
        //{

        //    List<Person> personList = new List<Person>();

        //    // int length = Convert.ToInt32(param.Count);
        //    foreach (var cast in castMembers)
        //    {
        //        personList.Add(new Person()
        //        {

        //            Name = cast,
        //        });
               
            
        //    }

        //    return personList;
        
        //}


        public static List<Film> Hack(string winnerSection)
        {
            List<Film> result = new List<Film>();

            int firstSection = winnerSection.IndexOf("<strong>");
            int secondSection = winnerSection.IndexOf("</strong>");

            int firstLength = secondSection - firstSection;

            int thirdSection = winnerSection.IndexOf("<strong>", secondSection);
            int fourthSection = winnerSection.IndexOf("</strong>", thirdSection);


            return new List<Film>();
        }



        public static List<Person> RedefinedCastlist(List<string> castMembersUndefined)
        {

            List<Person> CastMembers = new List<Person>();

            List<string> result = new List<string>();

            List<string> secondResult = new List<string>();

            int length = 0;

            bool ExitDoWhile = false;

            // Error occurred on tenth loop 
          //  do {

                for (int i = 0; i < castMembersUndefined.Count; i++)
                {

                    int firstElement = castMembersUndefined[i].IndexOf("itemprop=\"name\">");

                    if (castMembersUndefined[i].Contains("Rest of cast listed alphabetically:") == true && firstElement == -1)
                    {
                        continue;

                    } else {


                        int nameWebTokenStartID = castMembersUndefined[i].IndexOf("<a href=\"/name/"); // recently added
                        string nameWebTokenStart = "<a href=\"/name";

                        nameWebTokenStartID += nameWebTokenStart.Length; //recently added

                        if (firstElement == -1 || nameWebTokenStartID == -1) // recently added second OR parameter
                        {

                            ExitDoWhile = true;
                            break;

                        }
                        else
                        {

                            int secondElement = castMembersUndefined[i].IndexOf("</span></a>", firstElement); // Added firstElement as an additional parameter

                            int nameWebTokenEndID = castMembersUndefined[i].IndexOf("?ref_", nameWebTokenStartID); // recently added

                            int nameTokenLength = nameWebTokenEndID - nameWebTokenStartID; // recently added

                            length = secondElement - firstElement;

                            string nameIDTemp = castMembersUndefined[i].Substring(nameWebTokenStartID, nameTokenLength); // recently added

                            nameIDTemp = cleanUpString(nameIDTemp);

                            string temp = castMembersUndefined[i].Substring(firstElement, length);

                            //// Remove second for-loop so you can add more information 

                            int startElement = temp.IndexOf("itemprop");

                            int endElement = temp.IndexOf(">");

                            int lengthOfString = temp.Length;

                            length = endElement - startElement;

                            int whereToStart = lengthOfString - length;

                            string tempResult = temp.Substring(length, whereToStart);

                            tempResult = cleanUpString(tempResult);

                            secondResult.Add(tempResult);


                            CastMembers.Add(new Person()
                            {
                                name = tempResult,
                                nameToken = nameIDTemp
                            });

                            //result.Add(temp);              
                        }
                    }
                }

            //} while (ExitDoWhile == false);


            return CastMembers;

        }

        public static string RemoveUnicodeCharacter(string input)
        {


            string entity = HttpUtility.HtmlDecode(input).ToString();

            return entity;        
            
        }

        

        //public static List<string> RedefinedCastlist(List<string> castMembersUndefined)
        //{

        //    List<string> result = new List<string>();

        //    List<string> secondResult = new List<string>();

        //    bool ExitDoWhile = false;

        //    for (int i = 0; i < castMembersUndefined.Count; i++) {

        //            int firstElement = castMembersUndefined[i].IndexOf("itemprop=\"name\">");

        //            int nameWebTokenStartID = castMembersUndefined[i].IndexOf("<a href=\"/name/"); // recently added
        //            string nameWebTokenStart = "<a href=\"/name";

        //            nameWebTokenStartID += nameWebTokenStart.Length;

        //            if (firstElement == -1 || nameWebTokenStartID == -1 ) // recently added second OR parameter
        //            {

        //                ExitDoWhile = true;

        //            } else {


        //                int secondElement = castMembersUndefined[i].IndexOf("</span></a>", firstElement); // Added firstElement as an additional parameter

        //                int nameWebTokenEndID = castMembersUndefined[i].IndexOf("?ref_", nameWebTokenStartID); // recently added

        //                int nameTokenLength = nameWebTokenEndID - nameWebTokenStartID; // recently added

        //                int length = secondElement - firstElement;

        //                string nameIDTemp = castMembersUndefined[i].Substring(nameWebTokenStartID, nameTokenLength); // recently added

        //                string temp = castMembersUndefined[i].Substring(firstElement, length);

        //                result.Add(temp);
        //            }

        //   } while (ExitDoWhile == false);


        //    // Clean up the string a bit more 
        //    for(int i = 0; i< result.Count; i++)
        //    {

        //        int startElement = result[i].IndexOf("itemprop");

        //        int endElement = result[i].IndexOf(">");

        //        int lengthOfString = result[i].Length;

        //        int length = endElement - startElement;

        //        int whereToStart = lengthOfString - length;

        //        string tempResult = result[i].Substring(length, whereToStart);

        //        tempResult = cleanUpString(tempResult);
            
        //        secondResult.Add(tempResult);

        //    }

        //    return secondResult;
        
        //}


        // What we did is to set the startindex equal to the Value of the endIndex inorder not to
        // get the same cast member from the arraylist
        public static List<string> CastList (string s, string oddOrEven)
        {
            
           
            List<string> result = new List<string>();

            bool isFirstTime = true;

            int startIndex = 0;

            int endIndex = 0;

            bool stopWhile = false;

            int length;

            do {


                string startPoint = "<tr class=\"" + oddOrEven + "\">";

                string endPoint = "</tr>";

                if (isFirstTime == true)
                {
                    startIndex = s.IndexOf(startPoint);
                }

                //if (startIndex != -1)
               
                endIndex = s.IndexOf(endPoint, startIndex);

                // Move the check until after the int has been set 
                if (endIndex != -1)
                {

                    length = endIndex - startIndex;

                    string castMember = s.Substring(startIndex, length);

                    result.Add(castMember);

                    startIndex = endIndex + 1;

                    isFirstTime = false;

                } else if(endIndex == -1)
                {
                    stopWhile = true;
                }

            } while (stopWhile == false); // while(startIndex != -1);

        
            return result;    
        
        }
       
        public static string cleanUpString (string s)
        {

            s = s.Replace('"', ' ').Trim();
            s = s.Replace('>', ' ').Trim();
            s = s.Replace('!', ' ').Trim();
            s = s.Replace('^', ' ').Trim();
            s = s.Replace('-', ' ').Trim();
            s = s.Replace('/', ' ').Trim();

            return s;

        }

    }
}
