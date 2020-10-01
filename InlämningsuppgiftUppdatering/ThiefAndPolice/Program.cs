using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ThiefAndPolice
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(120, 50);
            Program.Prison = new List<Thief>();
            Program.CountTotalSuspectedThiefs = 0;
            Program.CountTotalCitizensRobbed = 0;

            //set size of board yourself 
            Program.SizeOfBoardX = 100;
            Program.SizeOfBoardY = 25;
            //decide amount of persons in your city
            int numberOfPolices = 20;
            int numberOfThief = 20;
            int numberOfCitizens = 20;
            //long text or short text information
            Program.LongTextInformation = true;

            Program.City = Person.CreateListOfPersons(numberOfPolices, numberOfThief, numberOfCitizens);

            while (true)
            {
                PrintOutBoard();
                Program.City = ChangeXAndYForPeolpeInCity();
                Thread.Sleep(100);
            }
            
        }

        public static int SizeOfBoardX { get; set; }
        public static int SizeOfBoardY { get; set; }
        public static bool LongTextInformation { get; set; }
        public static List<Person> City { get; set; }
        public static List<Thief> Prison { get; set; }
        public static int CountTotalSuspectedThiefs { get; set; }
        public static int CountTotalCitizensRobbed { get; set; }
        public static int CountTotalArrestedThiefs { get; set; }
        public static int CountTotalRobAttempts { get; set; }
        public static int MeetingsPerUpdate { get; set; }
        public static int NumberOfPolicesOnThisPosition { get; set; }
        public static int NumberOfCitizensOnThisPosition { get; set; }
        public static int NumberOfRobbedCitizensOnThisPosition { get; set; }
        public static int NumberOfThiefsOnThisPosition { get; set; }
        public static int NumberOfThiefsArrestedOnThisPosition { get; set; }
        public static int NumberOfReleasedThiefs { get; set; }

        static void PrintOutBoard()
        {
            Console.Clear();
            List<string> happeningsInCity = new List<string>();
            bool pauseAfterMeeting = true;

            for (int y = 0; y < Program.SizeOfBoardY; y++)
            {
                if (y == 0)
                {
                    pauseAfterMeeting = false;
                    happeningsInCity.Clear();
                    MeetingsPerUpdate = 0;
                }
                for (int x = 0; x < Program.SizeOfBoardX; x++)
                {
                    bool someOneMet = false;
                    NumberOfCitizensOnThisPosition = 0;
                    NumberOfPolicesOnThisPosition = 0;
                    NumberOfThiefsOnThisPosition = 0;
                    NumberOfThiefsArrestedOnThisPosition = 0;
                    NumberOfRobbedCitizensOnThisPosition = 0;
                    NumberOfReleasedThiefs = 0;

                    foreach (Person person in City)
                    {
                        if (person.XPosition == x && person.YPosition == y)
                        {
                            if (person is Police)
                            {
                                List<Police> listOfPolicesOnThisPosition = ((Police)person).ListOfPolicesOnSpot(x, y);
                                Person.PoliceFindsThiefsAndCitizens(x, y, listOfPolicesOnThisPosition);
                            }
                            else if (person is Thief)
                            {
                                if (((Thief)person).InPrison == false)
                                { Person.ThiefFindsThiefsAndCitizens(x, y); }
                            }
                            else if (person is Citizen)
                            {
                                ((Citizen)person).CitizensOnThisPosition(x, y);
                            }
                            if ((NumberOfPolicesOnThisPosition + NumberOfThiefsOnThisPosition + NumberOfCitizensOnThisPosition) > 1)
                            { someOneMet = true; }

                            if (!someOneMet)
                            {
                                if (person is Police)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                                    Console.Write("P");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                if (person is Thief)
                                {
                                    if (((Thief)person).InPrison == true)
                                    {
                                        { Console.Write(" "); }
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.Write("T");
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                }
                                if (person is Citizen)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write("M");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                            }
                            
                            break;
                        }
                    }

                    if (!someOneMet)
                    { Console.Write(" "); }
                    if (someOneMet)
                    {
                        Program.MeetingsPerUpdate++;
                        happeningsInCity = SaveInformationAboutHappeningsInCity(happeningsInCity);
                        Console.Write("X");
                        pauseAfterMeeting = true;
                    }
                }
                Console.WriteLine();
            }

            if (Prison.Any<Thief>())
            {
                CheckIfPrisonersShouldBeFree();
            }

            if ((pauseAfterMeeting && happeningsInCity.Any<string>()) || NumberOfReleasedThiefs > 0)
            {
                PrintOutInformation(happeningsInCity);
                Console.WriteLine();
                PrintCounterOnRobbedAndCatched();
                PrintOutPrison();
                Thread.Sleep(2000);
            }
        }
        static List<Person> ChangeXAndYForPeolpeInCity()
        {
            foreach (Person person in Program.City)
            {
                person.SetNewYAndXPosition();
            }
            return Program.City;
        }
        static void CheckIfPrisonersShouldBeFree()
        {
            for (int i = 0; i < Prison.Count; i++)
            {
                if (Prison[i].SecondPuttedInPrison <= GetTimeSecondsNow())
                {
                    Prison[i].InPrison = false;
                    Prison.RemoveAt(i);
                    NumberOfReleasedThiefs++;
                    i--;
                }
            }
        }
        static void PrintOutPrison()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("######FÄNGELSE######");
            Console.WriteLine("----------------------------------------------------");
            foreach (Thief thief in Program.Prison)
            {
                Console.WriteLine($"* Tjuv, har suttit {(GetTimeSecondsNow() - (thief.SecondPuttedInPrison - 30))} sekunder i finkan.");
            }
            Console.WriteLine("----------------------------------------------------");
            if (NumberOfReleasedThiefs > 0)
            {
                Console.WriteLine($"{NumberOfReleasedThiefs} {(NumberOfReleasedThiefs > 1 ? "tjuvar" : "tjuv")} har släppts från fängelset!");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        static int GetTimeSecondsNow()
        {
            int secondNow = (DateTime.Now.Hour * 60 * 60) + (DateTime.Now.Minute * 60) + DateTime.Now.Second;
            return secondNow;
        }

        static List<string> SaveInformationAboutHappeningsInCity(List<string> happeningsInCity)
        {
            if (Program.LongTextInformation)
            {
                if (NumberOfPolicesOnThisPosition > 0 && NumberOfThiefsOnThisPosition > 0 && NumberOfCitizensOnThisPosition > 0)
                {
                    happeningsInCity.Add($"{NumberOfThiefsOnThisPosition} {(NumberOfThiefsOnThisPosition > 1 ? "tjuvar" : "tjuv")} försökte råna {NumberOfCitizensOnThisPosition} medborgare. {(NumberOfCitizensOnThisPosition - NumberOfRobbedCitizensOnThisPosition > 0 ? $"{(NumberOfRobbedCitizensOnThisPosition > 0 ? $"{NumberOfRobbedCitizensOnThisPosition} rånförsök lyckades (medborgaren hade något kvar att sno)" : $"{(NumberOfCitizensOnThisPosition > 1 ? "Rånförsöken" : "Rånförsöket")} misslyckades")}." : $"{(NumberOfCitizensOnThisPosition > 1 ? "Rånen lyckades" : "Rånet lyckades")}.")} Samtidigt kom { NumberOfPolicesOnThisPosition} {(NumberOfPolicesOnThisPosition > 1 ? "poliser" : "polis")} och anade ugglor i mossen. {(NumberOfThiefsArrestedOnThisPosition == 0 ? "Ingen tjuv arresterades då inget stöldgods fanns på dem." : $"{(NumberOfThiefsArrestedOnThisPosition > 1 ? $"{NumberOfThiefsArrestedOnThisPosition} tjuvar hade stöldgods på sig och arresterades." : $"{NumberOfThiefsArrestedOnThisPosition} tjuv hade stöldgods på sig och arresterades.")}")}");
                }
                else if (NumberOfPolicesOnThisPosition > 0 && NumberOfThiefsOnThisPosition > 0 && NumberOfCitizensOnThisPosition == 0)
                {
                    happeningsInCity.Add($"{NumberOfPolicesOnThisPosition} {(NumberOfPolicesOnThisPosition > 1 ? "poliser" : "polis")} träffade {NumberOfThiefsOnThisPosition} {(NumberOfThiefsOnThisPosition > 1 ? "tjuvar" : "tjuv")} och anade ugglor i mossen. {(NumberOfThiefsArrestedOnThisPosition == 0 ? "Ingen tjuv arresterades då inget stöldgods fanns på dem." : $"{(NumberOfThiefsArrestedOnThisPosition > 1 ? $"{NumberOfThiefsArrestedOnThisPosition} tjuvar hade stöldgods på sig och arresterades." : $"{NumberOfThiefsArrestedOnThisPosition} tjuv hade stöldgods på sig och arresterades.")}")}");
                }
                else if (NumberOfPolicesOnThisPosition > 0 && NumberOfThiefsOnThisPosition == 0 && NumberOfCitizensOnThisPosition > 0)
                {
                    happeningsInCity.Add($"{NumberOfPolicesOnThisPosition} {(NumberOfPolicesOnThisPosition > 1 ? "poliser" : "polis")} stannade och snackade med {NumberOfCitizensOnThisPosition} medborgare.");
                }
                else if (NumberOfPolicesOnThisPosition == 0 && NumberOfThiefsOnThisPosition > 0 && NumberOfCitizensOnThisPosition > 0)
                {
                    happeningsInCity.Add($"{NumberOfThiefsOnThisPosition} {(NumberOfThiefsOnThisPosition > 1 ? "tjuvar" : "tjuv")} försökte råna {NumberOfCitizensOnThisPosition} medborgare. {(NumberOfCitizensOnThisPosition - NumberOfRobbedCitizensOnThisPosition > 0 ? $"{(NumberOfRobbedCitizensOnThisPosition > 0 ? $"{NumberOfRobbedCitizensOnThisPosition} rånförsök lyckades (medborgaren hade något kvar att sno)." : $"{(NumberOfCitizensOnThisPosition > 1 ? "Rånförsöken" : "Rånförsöket")} misslyckades")}" : $"{(NumberOfCitizensOnThisPosition > 1 ? "Rånen lyckades" : "Rånet lyckades")}")}.");
                }
                else if (NumberOfPolicesOnThisPosition > 1 && NumberOfThiefsOnThisPosition == 0 && NumberOfCitizensOnThisPosition == 0)
                {
                    happeningsInCity.Add($"{NumberOfPolicesOnThisPosition} poliser stannade och diskuterade taktik.");
                }
                else if (NumberOfPolicesOnThisPosition == 0 && NumberOfThiefsOnThisPosition > 1 && NumberOfCitizensOnThisPosition == 0)
                {
                    happeningsInCity.Add($"{NumberOfThiefsOnThisPosition} tjuvar hade tjuvmöte.");
                }
                else if (NumberOfPolicesOnThisPosition == 0 && NumberOfThiefsOnThisPosition == 0 && NumberOfCitizensOnThisPosition > 1)
                {
                    happeningsInCity.Add($"{NumberOfCitizensOnThisPosition} medborgare sågs över en fika.");
                }
                return happeningsInCity;
            }
            else
            {
                if (NumberOfRobbedCitizensOnThisPosition > 0 && NumberOfThiefsArrestedOnThisPosition > 0)
                {
                    happeningsInCity.Add($"{NumberOfRobbedCitizensOnThisPosition} medborgare rånades och {NumberOfThiefsArrestedOnThisPosition} {(NumberOfThiefsArrestedOnThisPosition > 1 ? "tjuvar" : "tjuv")} fångades av polis och fick sitt stöldgods beslagtaget.");
                }
                if (NumberOfRobbedCitizensOnThisPosition > 0)
                {
                    happeningsInCity.Add($"{NumberOfRobbedCitizensOnThisPosition} medborgare rånades.");
                }
                if (NumberOfThiefsArrestedOnThisPosition > 0)
                {
                    happeningsInCity.Add($"{ NumberOfThiefsArrestedOnThisPosition} {(NumberOfThiefsArrestedOnThisPosition > 1 ? "tjuvar" : "tjuv")} fångades av polis och fick sitt stöldgods beslagtaget.");
                }
                return happeningsInCity;
            }
        }
        static void PrintCounterOnRobbedAndCatched()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Antal rånförsök: " + Program.CountTotalRobAttempts);
            Console.WriteLine("Antal gånger medborgare rånats: " + Program.CountTotalCitizensRobbed);
            Console.WriteLine("Antal möten mellan polis och misstänkt tjuv: " + Program.CountTotalSuspectedThiefs);
            Console.WriteLine("Antal arresterade tjuvar: " + Program.CountTotalArrestedThiefs);
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void PrintOutInformation(List<string> happeningsInCity)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("BREAKING NEWS!!!");
            Console.WriteLine($"Det skedde {Program.MeetingsPerUpdate} möten i staden CEESHARP:");
            Console.WriteLine("-----------------------------------------------------------------");

            foreach (string happening in happeningsInCity)
            {
                Console.WriteLine(happening);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

}

