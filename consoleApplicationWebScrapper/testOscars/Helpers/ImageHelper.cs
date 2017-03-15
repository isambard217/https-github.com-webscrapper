using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using testOscars.Model;

namespace testOscars
{

    public class ImageHelper
    {

 
        public static string GetHtmlPage(string url)
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

        public static string GetImageMediaGallery(string url)
        {
          
            WebClient client = new WebClient();
            Random r = new Random();
            //Random IP Address
            client.Headers["X-Forwarded-For"] = r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255);
            //Random User-Agent
            client.Headers["User-Agent"] = "Mozilla/" + r.Next(3, 5) + ".0 (Windows NT " + r.Next(3, 5) + "." + r.Next(0, 2) + "; rv:2.0.1) Gecko/20100101 Firefox/" + r.Next(3, 5) + "." + r.Next(0, 5) + "." + r.Next(0, 5);
            Stream datastream = client.OpenRead(url);
            StreamReader reader = new StreamReader(datastream);
            StringBuilder sb = new StringBuilder();
            while (!reader.EndOfStream)
                sb.Append(reader.ReadLine());
            return sb.ToString();
           

            /*
            WebClient client = new WebClient();

            client.Headers.Set("Content-Type", "text/html");

            byte[] imageBytes = client.DownloadData(url);
            MemoryStream ms = new MemoryStream(imageBytes);
            // Convert byte array into an Image and save it onto the File System
            Image image = Image.FromStream(ms);

            return image;
            */
           
        }


        // This function returns the URL to an Actor's picture. 
        // If the Actor/Actress does not have a Profile image then we get one from the media gallery
        // This method needs to return the URL and whether the Image is a profileImage or not
        public static string[] GetProfileImageUrl(string htmlPage, Person actor)
        {

            string[] resultArray = new string[3];
            string isProfileImage = ""; // Can have one of two values media or profile

            string profileImageUrl;
            string startSection;
            string endSection;
            int startIndex;
            int endIndex;
            string profilePictureUrl;

            WebClient client = new WebClient();

            startSection = "<td rowspan=\"2\" id=\"img_primary\">";
            endSection = "</td>";

            startIndex = htmlPage.IndexOf(startSection);
            endIndex = htmlPage.IndexOf(endSection, startIndex);

            // Sometimes the profile page does not exist
            string profileImage = htmlPage.Substring(startIndex, endIndex - startIndex);

            startIndex = profileImage.IndexOf("src");
            endIndex = profileImage.IndexOf("itemprop");


            //test start
            if (startIndex == -1)
            {
               int test = profileImage.IndexOf("<div class=\"no-pic-wrapper\">");

               Debug.WriteLine(test);
            }
            
            // test end

            // If profile picture does not exist grab another picture
            if (startIndex == -1 && endIndex == -1)
            {
                isProfileImage = "media";

                string url = "http://www.imdb.com/name/" + actor.nameToken + "/mediaviewer/rm34487808";

                string result = GetImageMediaGallery(url);

                string GetUrlForBigImage = GetLargeImage(result);

                profileImageUrl = GetUrlForBigImage;

            } else {

                // If profile picture exists get it 
                isProfileImage = "profile";
                
                profilePictureUrl = profileImage.Substring(startIndex, endIndex - startIndex);

                StringBuilder sb = new StringBuilder();

                int urlLength = profilePictureUrl.Length;

                startIndex = profilePictureUrl.IndexOf("h");

                profileImageUrl = profilePictureUrl.Substring(startIndex);

            }

            resultArray[0] = profileImageUrl;
            resultArray[1] = isProfileImage;
            resultArray[2] = actor.ID.ToString();

            return resultArray;

        } // End Method


       
        public static string GetLargeImage(string htmlPage)
        { 
        
            string startElement = "<body";
            string endElement = "</body>";

            int startIndex = htmlPage.IndexOf(startElement);
            int endIndex = htmlPage.IndexOf(endElement);

            int length = endIndex - startIndex;

            /*Contains the mark-up between the Body Tag */
            string result = htmlPage.Substring(startIndex, length);

            startIndex = result.IndexOf("<script");
            endIndex = result.IndexOf("\"w\"");

            length = endIndex - startIndex;

            string scriptTag = result.Substring(startIndex, length);

            startIndex = scriptTag.IndexOf("\"src\""); //,"src"

            endIndex = scriptTag.IndexOf("\\.jpg\"", startIndex);

            string narrowTag = scriptTag.Substring(startIndex);


            startIndex = narrowTag.IndexOf("https");

            endIndex = narrowTag.IndexOf(",");

            result = narrowTag.Substring(startIndex, endIndex - startIndex);

            return result;
        
        } // End method

        public static Image Antialising(Image image, String s)
        {

            Bitmap result;
            Image modifiedImage;

            // Main example template size
            int mainWidth = 107;
            int mainHeight = 158;

            // Image to Refactor
            int bigWidth = image.Width;
            int bigHeight = image.Height;

            int bothSides = bigWidth/ 2;

            Bitmap imageReturn = new Bitmap(image);

            Rectangle r = new Rectangle();

            Bitmap nb = new Bitmap(107, 158);
            Graphics g = Graphics.FromImage(nb);
            g.DrawImage(imageReturn, -bothSides, 0);

            nb.Save("C:\\cacheNoProfilePicture\\" + s + ".png", ImageFormat.Png);

            return nb;


        }





        public static Image ScaleImage(Image image, string mediaProfile)
        {

            int width = 107;
            int height = 158;

            Bitmap modifiedImage;

            OscarData os = new OscarData();

         /*   if (mediaProfile.Equals("media"))
            {

               /* int mediaHeight = image.Height;
                int mediaWidth = image.Width;

                modifiedImage = new Bitmap(image, new Size(107, 158));
                */
        //    } else {


                // Width and Height of original Image
                width = image.Height;
                height = image.Width;

                Bitmap imageReturn = new Bitmap(image);
                //Graphics g = Graphics.FromImage(imageReturn);
                //g.DrawImage(image, 0, 0, 158, 107);

                modifiedImage = new Bitmap(image, new Size(107, 158));
           

            return modifiedImage;
        }


    } // End class

}
