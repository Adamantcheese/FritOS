using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosProj1
{
    public class Variable
    {
        private String name = "";
        private String val1 = "";
        private Int32 val2 = Int32.MinValue;
        private int valType = -1;

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
    }
}
