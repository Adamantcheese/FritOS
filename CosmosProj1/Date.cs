using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosProj1
{
    public class Date
    {
        private byte cDMo;
        private byte cDw;
        private byte cMo;
        private byte cCent;
        private byte cYr;

        //Initialize this object with the current date
        public Date()
        {
            cDMo = Cosmos.Hardware.RTC.DayOfTheMonth;
            cDw = Cosmos.Hardware.RTC.DayOfTheWeek;
            cMo = Cosmos.Hardware.RTC.Month;
            cCent = Cosmos.Hardware.RTC.Century;
            cYr = Cosmos.Hardware.RTC.Year;
        }

        //Wrapper for getStrDay
        public String getStrDay()
        {
            return getStrDay(cDw);
        }

        //Converts a given day byte from the RTC clock to a day string
        private String getStrDay(byte input)
        {
            input = (byte)(input % 7);
            switch (input)
            {
                case 0:
                    return "Wed";
                case 1:
                    return "Thu";
                case 2:
                    return "Fri";
                case 3:
                    return "Sat";
                case 4:
                    return "Sun";
                case 5:
                    return "Mon";
                case 6:
                    return "Tue";
                default:
                    return "";
            }
        }

        //Returns a byte corresponding to the day of the week as noted in the toDay method
        private static byte getDOW(byte cent, byte yr, byte mon, byte day)
        {
            int y = (int)(cent) * 100 + yr - 1;
            int m = (mon - 2) % 12;
            int d = day;
            int w = (int)(d + 2.6 * m - 0.2 + 5 * (y % 4) + 4 * (y % 100) + 6 * (y % 400) + 5) % 7;
            return (byte)w;
        }

        public void update()
        {
            cDMo = Cosmos.Hardware.RTC.DayOfTheMonth;
            cDw = Cosmos.Hardware.RTC.DayOfTheWeek;
            cMo = Cosmos.Hardware.RTC.Month;
            cCent = Cosmos.Hardware.RTC.Century;
            cYr = Cosmos.Hardware.RTC.Year;
        }

        public void update(byte dm, byte m, byte c, byte y)
        {
            cDw = getDOW(c, y, m, dm);
            cDMo = dm;
            cMo = m;
            cCent = c;
            cYr = y;
        }

        public String getLongDate()
        {
            return getStrDay() + ' ' + byteToString(cMo) + '/' + byteToString(cDMo) + '/' + byteToString(cCent) + byteToString(cYr);
        }

        public String getShortDate()
        {
            return byteToString(cMo) + '/' + byteToString(cDMo) + '/' + byteToString(cCent) + byteToString(cYr);
        }

        //Displays bytes as pairs of two digits 0-9.
        private String byteToString(byte s)
        {
            String r = s.ToString();
            if (s < 10)
            {
                r = '0' + r;
            }
            return r;
        }
    }
}
