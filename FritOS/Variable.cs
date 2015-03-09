using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FritOS
{
    public class Variable
    {
        private String name = "";
        private String val1 = "";
        private Int32 val2 = Int32.MinValue;
        private Int32 valType = -1;

        public Variable(String s, String val)
        {
            name = s;
            val1 = val;
            valType = 1;
        }

        public Variable(String s, Int32 val)
        {
            name = s;
            val2 = val;
            valType = 2;
        }

        public void setVal(String o)
        {
            val1 = o;
        }

        public void setVal(Int32 o)
        {
            val2 = o;
        }

        public String getName()
        {
            return name;
        }

        public String getStrVal()
        {
            return val1;
        }

        public Int32 getIntVal()
        {
            return val2;
        }

        public Int32 getType()
        {
            return valType;
        }

        public String getTypeStr()
        {
            if (valType == 1)
            {
                return "str ";
            }
            else
            {
                return "int ";
            }
        }

        public String toString()
        {
            switch (valType)
            {
                case 1:
                    return val1;
                case 2:
                    return val2.ToString();
                default:
                    return null;
            }
        }

        private void castToStr()
        {
            if (valType == 1)
            {
                Console.WriteLine("Variable is already type string.");
            }
            else
            {
                val1 = val2.ToString();
                val2 = Int32.MinValue;
                valType = 1;
            }
        }

        private void castToInt()
        {
            if (valType == 2)
            {
                Console.WriteLine("Variable is already type integer");
            }
            else
            {
                if (Kernel.isValidInt(val1))
                {
                    val2 = Int32.Parse(val1);
                    val1 = "";
                    valType = 2;
                }
                else
                {
                    Console.WriteLine("Unable to convert current value " + val1 + " to type integer.");
                }
            }
        }

        public void cast()
        {
            if (valType == 1)
            {
                castToInt();
            }
            else
            {
                castToStr();
            }
        }
    }
}
