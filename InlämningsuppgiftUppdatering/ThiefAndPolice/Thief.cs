using System;
using System.Collections.Generic;

namespace ThiefAndPolice
{
    public class Thief : Person
    {
        public List<Inventory> StolenGoods { get; set; }
        public bool InPrison { get; set; }
        public int SecondPuttedInPrison { get; set; }

        public Thief()
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

            StolenGoods = new List<Inventory>();
            InPrison = false;
            SecondPuttedInPrison = 0;
        }

        public void SetStartTimeForPrison()
        {
            SecondPuttedInPrison = (DateTime.Now.Hour * 60 * 60) + (DateTime.Now.Minute * 60) + DateTime.Now.Second + 30;
        }
        public void PutThiefInPrison(Thief thief)
        {
            Program.Prison.Add(thief);
            InPrison = true;
            SetStartTimeForPrison();
        }

    }
}
