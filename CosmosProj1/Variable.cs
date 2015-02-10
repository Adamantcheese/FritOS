using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosProj1
{
    public class Variable
    {
        private Object value;
        private String name;

        public Variable(String s, String val)
        {
            name = s;
            value = val;
        }

        public Variable(String s, Int32 val)
        {
            name = s;
            value = val;
        }

        public Variable(String s, Double val)
        {
            name = s;
            value = val;
        }

        public void setVal(String o)
        {
            value = o;
        }

        public void setVal(Int32 o)
        {
            value = o;
        }

        public void setVal(Double o)
        {
            value = o;
        }

        public Object getVal()
        {
            return value;
        }

        public String getName()
        {
            return name;
        }
    }
}
