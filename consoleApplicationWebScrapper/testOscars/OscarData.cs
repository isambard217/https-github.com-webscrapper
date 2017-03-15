using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using testOscars.Helpers;
using testOscars.Model;
using testOscars.NewHope;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace testOscars
{
    public class OscarData
    {

        //Actor id
        public int actorId { get; set; }


        // This property is used when we want to save the profileImageFile
        // The boolean will indicate whether we are saving a film page or a profile image
        public bool profileImageFile { get; set; }

        public OscarData()
        { 
        
        }

        public OscarData(int Id)
        {

            this.actorId = Id;  
        }


        public bool ImageExists(string id) 
        {
            if (File.Exists("C:\\cacheNoProfilePicture\\" + id + ".png"))
            {

                return true;
            } else {

                return false;
            }
        }


        public bool FileExists(string url)
        {
            // If we want  then we want the File
            if (profileImageFile == false)
            {

                if (File.Exists("C:\\Cache\\" + url + ".htm"))
                {
                    return true;
                } else {

                    return false;
                }

            // If we want the profileImage
            } else {

                if (File.Exists("C:\\CacheImages\\" + url + ".png"))
                {
                    return true;
                } else {
                    return false;
                }
            }

        }

        public string GetFile(string url)
        {

            if (profileImageFile == false)
            {
                return File.ReadAllText("C:\\Cache\\" + url + ".htm");

            } else {

                return File.ReadAllText("C:\\CacheImages\\" + url + ".png");
            }
        }


     
        public byte[] GetImage(string[] url)
        {        

           if (url[1].Equals("media")) 
           {

               return File.ReadAllBytes("C:\\cacheNoProfilePicture\\" + url[2] + ".png");
           }

           return File.ReadAllBytes("C:\\CacheImages\\" + url[2] + ".png");
        }



        public void RequestImage(string[] url)
        {

                // string[0] url is the url path to the image
                // string[1] url states whether it is a profile image or media library image
                // string[2] url contains the Actor's id
                WebClient client = new WebClient();

                if (url != null)
                {            
                    // Media Images
                    if ((url[1]).Equals("media"))
                    {

                        if (ImageExists(url[2]) == true)
                        {

                            // Code to get data Locally
                            byte[] imageBytes = GetImage(url);
                            MemoryStream ms = new MemoryStream(imageBytes);
                            Image image = Image.FromStream(ms);

                            ImageHelper.Antialising(image, url[2].ToString());

                        } else {

                            // Get Image from the Internet
                            byte[] imageBytes = client.DownloadData(url[0]);
                            MemoryStream ms = new MemoryStream(imageBytes);
                            // Convert byte array into an Image and save it onto the File System
                            Image image = Image.FromStream(ms);

                            ImageHelper.Antialising(image, url[2].ToString());

                            SaveImage(image, url[1]);
                        }


                    // ProfileImages
                    } else if ((url[1]).Equals("profile")) 
                    {

                        if (ImageExists(url[2]) == true)
                        {
                            // Code to get data Locally
                            // Needs to determine whether it is a profile Image or a Cache Image
                            byte[] imageBytes = GetImage(url);
                            MemoryStream ms = new MemoryStream(imageBytes);
                            Image image = Image.FromStream(ms);
                        
                        } else {

                            byte[] imageBytes = client.DownloadData(url[0]);
                            MemoryStream ms = new MemoryStream(imageBytes);
                            // Convert byte array into an Image and save it onto the File System
                            Image image = Image.FromStream(ms);

                            // Before you save the Image you need to scale it down first 
                            // Image reimage;
                            if (url[1].Equals("profile"))
                            {
                                image = ImageHelper.ScaleImage(image, url[1]);
                            }

                            SaveImage(image, url[1]);
                        }
                    }
                }
        }

/////////////////////////////////////////////////////////////////
                    // Old request Image method
                   /* // If the Image exists locally
                    if (ImageExists(url[2]) == true)
                    {
                        // Code to get data Locally
                        // Needs to determine whether it is a profile Image or a Cache Image
                        byte[] imageBytes = GetImage(url);
                        MemoryStream ms = new MemoryStream(imageBytes);
                        Image image = Image.FromStream(ms);

                    // If Image does not exists make a request for it 
                    } else if (ImageExists(url[2]) == false) {

                        byte[] imageBytes = client.DownloadData(url[0]);
                        MemoryStream ms = new MemoryStream(imageBytes);
                        // Convert byte array into an Image and save it onto the File System
                        Image image = Image.FromStream(ms);

                        // Before you save the Image you need to scale it down first 
                        // Image reimage;
                        if (url[1].Equals("profile"))
                        {
                            image = ImageHelper.ScaleImage(image, url[1]);
                        }

                        SaveImage(image, url[1]);
                    */
          

       // } // End method
        
        
        public string RequestFile(string url, bool imageFile = false) 
        {
 
            bool noProfilepic = false;

            string htmlPage;
            this.profileImageFile = imageFile;
            WebClient client = new WebClient();

            // Make a file name
            // http---www-imdb-com-title-tt0019071-fullcredits?ref_=tt_cl_sm#cast
            string FileNameUrl = url.Replace('/', '-').Replace(':', '-').Replace('.', '-').Replace('?', '-').Replace('#', '-').Replace('_', '-').Replace('=', '-').Replace('\\', '-').Replace('@', '-').Replace('\"', '-');

                if (FileExists(FileNameUrl))
                {  // true file exists locally

                    string file = GetFile(FileNameUrl);

                    return file;

                } else {

                    Stream datastream = client.OpenRead(url);
                    StreamReader reader = new StreamReader(datastream);
                    StringBuilder sb = new StringBuilder();
                    while (!reader.EndOfStream)
                        sb.Append(reader.ReadLine());
                    htmlPage = sb.ToString();

                    // Save the file for next time
                    SaveFile(FileNameUrl, htmlPage);

               }

            return htmlPage;

        }  

        

        public void SaveFile(string FileNameUrl, string htmlPage) 
        {
            if (profileImageFile == false)
            {
                File.WriteAllText(@"C:\\Cache\\" + FileNameUrl + ".htm", htmlPage);
            
            }
        }


        public void SaveImage(Image image, string url)
        {
            if (url.Equals("profile") == true)
            {

                image.Save("C:\\CacheImages\\" + actorId + ".png", ImageFormat.Png);

            } else if (url.Equals("media"))
            { 
            
                image.Save("C:\\cacheNoProfilePicture\\" + actorId + ".png", ImageFormat.Png);
            }
            
        }


    }
}
