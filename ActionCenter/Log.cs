using System;
using System.IO;

namespace NX_Overmind.Actions
{
    public class Log
    {
        private static readonly object _lock = new object();
        public static void WriteLine(string s)
        {
            lock (_lock)
            {
                using (StreamWriter sw = new StreamWriter("log.txt", true))
                {
                    sw.WriteLine(s);
                }
            }
        }
    }
}
