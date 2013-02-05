using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataSciLib.IO
{
    public class Writer : StreamWriter
    {
        
        public Writer(Stream stream)
            : base(stream)
        {
        }

        public Writer(string filename)
            : base(filename)
        {
        }

        public void CSV(List<string[]> data)
        {
            foreach (var row in data)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var element in row)
                {
                    // Implement special handling for values that contain comma or quote
                    // Enclose in quotes and double up any double quotes
                    if (element.IndexOfAny(new char[] { '"', ',' }) != -1)
                        builder.AppendFormat("\"{0}\"", element.Replace("\"", "\"\"")).Append(",");
                    else
                        builder.Append(element).Append(',');
                }
                // Remove last comma
                builder.Remove(builder.Length - 1, 1);
                WriteLine(builder.ToString());
            }
        }

        public void CSV<Tin>(IEnumerable<Tin> data)
            where Tin : IEnumerable<string>
        {
            foreach (var row in data)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var element in row)
                {
                    // Implement special handling for values that contain comma or quote
                    // Enclose in quotes and double up any double quotes
                    if (element.IndexOfAny(new char[] { '"', ',' }) != -1)
                        builder.AppendFormat("\"{0}\"", element.Replace("\"", "\"\"")).Append(",");
                    else
                        builder.Append(element).Append(',');
                }
                // Remove last comma
                builder.Remove(builder.Length - 1, 1);
                WriteLine(builder.ToString());
            }
        }     
    }
}
