using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Paginate.XamarinAndroid.Sample.Model;

namespace Paginate.XamarinAndroid.Sample.Provider
{
    public class DataProvider
    {
        private DataProvider()
        {
        }

        private static Random RANDOM = new Random();

        public static List<Person> getRandomData(int size)
        {
            List<Person> persons = new List<Person>();
            for (int i = 0; i < size; i++)
            {
                persons.Add(createRandomPerson());
            }
            return persons;
        }

        public static Person createRandomPerson()
        {
            return new Person(getRandomName(), getRandomSurname(), (RANDOM.Next(35) + 20));
        }

        private static String getRandomName()
        {
            return names[RANDOM.Next(names.Length)];
        }

        private static String getRandomSurname()
        {
            return surnames[RANDOM.Next(surnames.Length)];
        }

        private static String[] names = new String[]{
            "Nickie",
            "Renita",
            "Celesta",
            "Kyra",
            "Debra",
            "Celsa",
            "Nan",
            "Tawanna",
            "Brittney",
            "Lizzette",
            "Myrtle",
            "Booker",
            "Pok",
            "Willodean",
            "Mardell",
            "Todd",
            "Lore",
            "Tandra",
            "Isidro",
            "Deandre",
            "Burton",
            "Cheryl",
            "Chong",
            "Willetta",
            "Karissa",
            "Dominque",
            "Zandra",
            "Tamala",
            "Jennifer",
            "Elisha",
            "Renate",
            "Shauna",
            "Latrisha",
            "Alayna",
            "Ana",
            "Ayana",
            "Arica",
            "Hal",
            "Marcelino",
            "Madeleine",
            "Hilton",
            "Erich",
            "Georgetta",
            "Maryrose",
            "Angelena",
            "Liana",
            "Santos",
            "Phil",
            "Annette",
            "Halina",
            "Elsy",
            "Brock",
            "Tyrone",
            "Myles",
            "Lang",
            "Micah",
            "Lee",
            "Lorraine",
            "Talia",
            "Eulalia",
            "Lavera",
            "Arlen",
            "Lena",
            "Julissa",
            "Mackenzie",
            "Lucy",
            "Juliette",
            "Tarra",
            "Clemente",
            "Ileana"
    };

    private static String[] surnames = new String[]{
            "Slaymaker",
            "Heyer",
            "Oyer",
            "Fellman",
            "Moodie",
            "Shoaf",
            "Kurland",
            "Pollman",
            "Sheridan",
            "Whiting",
            "Walson",
            "Utt",
            "Roser",
            "Schoenberg",
            "Motsinger",
            "Corley",
            "Addis",
            "Ivers",
            "Llanos",
            "Braddy",
            "Clute",
            "Heroux",
            "Ezzell",
            "Pellett",
            "Shanks",
            "Manno",
            "Boehmer",
            "Dinapoli",
            "Fannin",
            "Phair",
            "Sampsel",
            "Sorg",
            "Canter",
            "Rutter",
            "Byler",
            "Hansen",
            "Mcgilvray",
            "Schley",
            "Cardinale",
            "Kennard",
            "Palmer",
            "Simoneaux",
            "Blomberg",
            "Simental",
            "Lazaro",
            "Carte",
            "Barrow",
            "Greenburg",
            "Maginnis",
            "Blume",
            "Wrench",
            "Coachman",
            "Arzate",
            "Soderquist",
            "Uhl",
            "Leggett",
            "Square",
            "Earnest",
            "Mctaggart",
            "Marrone",
            "Lanser",
            "Ricklefs",
            "Lukasiewicz",
            "Ines",
            "Holdren",
            "Brissette",
            "Dundas",
            "Ostrom",
            "Blake",
            "Roberds"
    };
    }
}