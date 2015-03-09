using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FritOS
{
    public class ProcessBlock
    {
        public Stack<String> tempFNames;
        public Stack<String> tempLines;
        public Stack<Int32> tempLineCounts;
        public Int32 currentLine;
        public Int32 currentLineCount;
        public File processFile;

        public ProcessBlock(File f)
        {
            tempFNames = new Stack<String>();
            tempLines = new Stack<String>();
            tempLineCounts = new Stack<Int32>();
            currentLine = 0;
            currentLineCount = 0;
            processFile = f;
        }
    }
}
