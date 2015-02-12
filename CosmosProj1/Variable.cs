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
        private Double val3 = Double.NaN;
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

        public Variable(String s, Double val)
        {
            name = s;
            val3 = val;
            valType = 3;
        }

        public void setVal(String o)
        {
            val1 = o;
        }

        public void setVal(Int32 o)
        {
            val2 = o;
        }

        public void setVal(Double o)
        {
            val3 = o;
        }

        public String getName()
        {
            return name;
        }

        public Object getVal()
        {
            switch (valType)
            {
                case 1:
                    return (Object) val1;
                case 2:
                    return (Object) val2;
                case 3:
                    return (Object) val3;
                default:
                    return null;
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
                case 3:
                    return val3.ToString();
                default:
                    return null;
            }
        }
    }
}
