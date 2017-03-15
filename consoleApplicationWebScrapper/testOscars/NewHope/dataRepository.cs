using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testOscars.Helpers;
using testOscars.Model;

namespace testOscars.NewHope
{
    public class DataRepository
    {

        public static List<Person> allPersons { get; set; }
        public static List<Film> allFilms { get; set; }


        public DataRepository()
        {

            allFilms = new List<Film>();
            allPersons = new List<Person>();
        }

        public static void AddPerson(Person person)
        {


            allPersons.Add(person);
        }

        public static List<Person> GetAllPersons() 
        {


            return allPersons;
        }

        public static int GetAllPersonsCount() 
        {

            return Convert.ToInt32(allPersons.Count);
        }


        public static void AddFilm(Film film)
        {

            film.name = OscarsAwardHelper.RemoveUnicodeCharacter(film.name);

            allFilms.Add(film);
        }

        public static List<Film> GetAllFilms()
        {

            return allFilms;
        }

        public static Person FindByPersonByName(string name)
        {

            Person person = allPersons.FirstOrDefault(p => p.name.Equals(name));
            //Person person = allPersons.Find(p => p.Name.Equals(name));

            return person;       
        }
        

        public static Person FindPersonById(int PersonID)
        {

            Person person  = allPersons.Find(p => Person.PSID == PersonID);
        
            return person;
        }


        public static Film GetFilmByName(string name)
        {

            Film film = allFilms.Find(f => f.name.Equals(name));

            return film;
        }

    }
}
