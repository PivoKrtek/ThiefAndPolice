using System;
using System.Collections.Generic;
using System.Text;

namespace ThiefAndPolice
{
    public class Citizen : Person
    {
        public List<Inventory> Belongings { get; set; }
        public Citizen()
        {
            Random random = new Random();
            XPosition = random.Next(0, Program.SizeOfBoardX);
            YPosition = random.Next(0, Program.SizeOfBoardY);
            int backOrForward = 0;
            int upOrDown = 0;

            while (true)
            {
                backOrForward = random.Next(0, 3);
                upOrDown = random.Next(0, 3);
                if (backOrForward != 0 || upOrDown != 0)
                {
                    break;
                }
            }

            if (backOrForward == 1)
            { XDirection = 1; }
            else if (backOrForward == 2)
            { XDirection = -1; }
            else { XDirection = 0; }

            if (upOrDown == 1)
            { YDirection = 1; }
            else if (upOrDown == 2)
            { YDirection = -1; }
            else
            { YDirection = 0; }

            Belongings = new List<Inventory>();
            Belongings.Add(new Inventory("nycklar"));
            Belongings.Add(new Inventory("pengar"));
            Belongings.Add(new Inventory("klocka"));
            Belongings.Add(new Inventory("mobiltelefon"));

        }
        public void CitizensOnThisPosition(int xPosition, int yPosition)
        {
            foreach (Person person in Program.City)
            {
                if (person.XPosition == xPosition && person.YPosition == yPosition)
                {
                    if (person is Citizen)
                    { Program.NumberOfCitizensOnThisPosition++; }
                }
            }
        }
    }
}
