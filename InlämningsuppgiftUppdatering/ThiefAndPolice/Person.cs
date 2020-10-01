using System;
using System.Collections.Generic;
using System.Linq;

namespace ThiefAndPolice
{
    public class Person
    {
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public int XDirection { get; set; }
        public int YDirection { get; set; }
        public static List<Person> CreateListOfPersons(int numberOfPolices, int numberOfThiefs, int numberOfCitizens)
        {
            List<Person> persons = new List<Person>();
            for (int i = 0; i < numberOfPolices; i++)
            {
                persons.Add(new Police());
            }
            for (int i = 0; i < numberOfThiefs; i++)
            {
                persons.Add(new Thief());
            }
            for (int i = 0; i < numberOfCitizens; i++)
            {
                persons.Add(new Citizen());
            }
            return persons;
        }
        public void SetNewYAndXPosition()
        {
            XPosition = XPosition + XDirection;
            YPosition = YPosition + YDirection;
            if (XPosition == Program.SizeOfBoardX)
            {
                XPosition = 0;
            }
            else if (XPosition == -1)
            {
                XPosition = Program.SizeOfBoardX - 1;
            }
            if (YPosition == Program.SizeOfBoardY)
            {
                YPosition = 0;
            }
            else if (YPosition == -1)
            {
                YPosition = Program.SizeOfBoardY - 1;
            }
        }
        public static void PoliceFindsThiefsAndCitizens(int xPosition, int yPosition, List<Police> policesOnThisPosition)
        {
            foreach (Person person in Program.City)
            {
                if (person.XPosition == xPosition && person.YPosition == yPosition)
                {
                    if (person is Thief)
                    {
                        if (((Thief)person).InPrison == false)
                        {
                            Program.NumberOfThiefsOnThisPosition++;
                            Program.CountTotalSuspectedThiefs++;
                            person.StealFromCitizen(xPosition, yPosition, (Thief)person);

                            if (((Thief)person).StolenGoods.Any<Inventory>())
                            {
                                Program.CountTotalArrestedThiefs++;
                                Program.NumberOfThiefsArrestedOnThisPosition++;
                                ((Thief)person).PutThiefInPrison((Thief)person);
                                Seize(policesOnThisPosition, (Thief)person);
                            }
                        }

                    }
                    if (person is Citizen)
                    { Program.NumberOfCitizensOnThisPosition++; }
                }
            }
        }
        public static void Seize(List<Police> policesOnThisPosition, Thief currentThief)

        {
            int policeThatSeize = 0;
            if (policesOnThisPosition.Count > 1)
            {
                Random random = new Random();
                policeThatSeize = random.Next(0, policesOnThisPosition.Count);
            }
            foreach (var item in currentThief.StolenGoods)
            {
                (policesOnThisPosition.ElementAt<Police>(policeThatSeize)).Seized.Add(item);
            }
            currentThief.StolenGoods.Clear();
        }
        public void StealFromCitizen(int xPosition, int yPosition, Thief currentThief)
        {
            Random random = new Random();
            foreach (Person person in Program.City)
            {
                if (person.XPosition == xPosition && person.YPosition == yPosition)
                {
                    if (person is Citizen)
                    {
                        if (((Citizen)person).Belongings.Any())
                        {
                            int numberInList = random.Next(0, ((Citizen)person).Belongings.Count);
                            currentThief.StolenGoods.Add(((Citizen)person).Belongings.ElementAt<Inventory>(numberInList));
                            ((Citizen)person).Belongings.RemoveAt(numberInList);
                            Program.CountTotalCitizensRobbed++;
                            Program.CountTotalRobAttempts++;
                            Program.NumberOfRobbedCitizensOnThisPosition++;
                        }
                        else
                        {
                            Program.CountTotalRobAttempts++;
                        }
                    }
                }
            }
        }
        public static void ThiefFindsThiefsAndCitizens(int xPosition, int yPosition)
        {
            foreach (Person person in Program.City)
            {
                if (person.XPosition == xPosition && person.YPosition == yPosition)
                {
                    if (person is Thief)
                    {
                        if (((Thief)person).InPrison == false)
                        {
                            Program.NumberOfThiefsOnThisPosition++;

                            person.StealFromCitizen(xPosition, yPosition, (Thief)person);
                        }
                    }
                    if (person is Citizen)
                    { Program.NumberOfCitizensOnThisPosition++; }
                }
            }
        }
    }
}
