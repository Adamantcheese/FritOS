using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosProj1
{
    public class File
    {
        private String filename;
        private List<String> lines;
        private readonly Date creationTime;

        public File(String fname)
        {
            filename = fname;
            creationTime = new Date();
            lines = new List<String>();
        }

        public void writeLine(String l)
        {
            lines.Add(l);
        }

        public int getByteSize()
        {
            int byteCount = 0;
            String[] temp = new String[lines.Count];
            lines.CopyTo(temp);
            for(int i = 0; i < lines.Count; i++)
            {
                byteCount += lines[i].Length * 2;
            }
            byteCount += filename.Length * 2;
            byteCount += 5;
            //to get rid of some dumb offset that i don't get
            return byteCount - 66;
        }

        public int getLineCount()
        {
            return lines.Count;
        }

        public String getFileName()
        {
            return filename;
        }

        public String getCreationDate()
        {
            return creationTime.getShortDate();
        }

        public String getExtension()
        {
            return filename.Substring(filename.LastIndexOf('.') + 1);
        }

        public String readLine(int i)
        {
            if (i < 0 || i > lines.Count)
            {
                return "No line exists at that index!";
            }
            String[] temp = new String[lines.Count];
            lines.CopyTo(temp);
            return temp[i];
        }
    }
}
