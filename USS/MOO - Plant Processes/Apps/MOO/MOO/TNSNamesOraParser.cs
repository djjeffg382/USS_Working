using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MOO
{
    /// <summary>
    /// Used for parsing the tnsnames.ora file
    /// </summary>
    public class TNSNamesOraParser
    {
        /// <summary>
        /// Parses the tnsnames.ora file into entry/value list
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<TNSEntry> ParseTnsNamesFile(string filePath)
        {
            using StreamReader rdr = new(filePath);
            StringBuilder sbFile = new();
            while (!rdr.EndOfStream)
            {
                string line = rdr.ReadLine().Trim();
                if (line.Length > 0 && line[..1] != "#")
                    sbFile.AppendLine(line);
            }

            string fileText = sbFile.ToString();
            List<TNSEntry> entries = [];
            //start by removing all line breaks
            fileText = fileText.Replace("\r", "").Replace("\n","");
            //now split everything at the equals
            var split = fileText.Split('=');
            StringBuilder value = new();
            TNSEntry entry = null;
            int parenCount = 0;
            for (int i = 0; i < split.Length; i++)
            {
                if(entry == null)
                {
                    entry = new() { Entry = split[i] };
                    value.Clear();
                }                    
                else
                {
                    
                    parenCount += split[i].Count(x => x == '(');
                    parenCount -= split[i].Count(x => x == ')');
                    if(parenCount == 0)
                    {
                        //need to check the remaining string for the end parenthesis
                        int idxParen = split[i].LastIndexOf(')');
                        if(idxParen == -1)
                        {
                            //just skip over this but add it to the value, found this with an error in tnsnames example:
                            //cdxsoa.psc.uss.com=cdxsoa.psc.uss.com=(description=(address_list=(load_balance=yes)(failover=yes)(address=(protocol=tcp)(host=cdxsoa-host.psc.uss.com)(port=2041)))(connect_data=(service_name=soadev.psc.uss.com)))
                            value.Append(split[i]);
                            value.Append('=');  //we split on equals so add it back in
                        }
                        else
                        {
                            value.Append(split[i][..(idxParen +1)]);

                            entry.Value = value.ToString();
                            entries.Add(entry);
                            entry = null;
                            value.Clear();
                            //now check if we have remainder
                            if (split[i].Length > idxParen + 1)
                            {
                                string nextEntry = split[i][(idxParen + 1)..];
                                if (!string.IsNullOrEmpty(nextEntry))
                                {
                                    entry = new() { Entry = nextEntry };
                                    value.Clear();
                                }
                            }
                        }


                    }
                    else
                    {
                        value.Append(split[i]);
                        value.Append('=');  //we split on equals so add it back in
                    }
                        
                }
            }
            return entries;

        }

        /// <summary>
        /// TNS Entries from tnsnames.ora file
        /// </summary>
        public class TNSEntry
        {
            /// <summary>
            /// The TNS Entry
            /// </summary>
            public string Entry { get; set; }
            /// <summary>
            /// The Value of the TNS Entry
            /// </summary>
            public string Value { get; set; }
        }
    }
}
