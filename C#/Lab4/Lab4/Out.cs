using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    class Out
    {
        private static string logFilePath = "../../../out.txt";

        public static void Write(string line)
        {
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath);
            }
            using (var streamWriter = File.AppendText(logFilePath))
            {
                streamWriter.WriteLine(line);
            }
        }

        public static void Write(string[] lines)
        {
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath);
            }
            using (var streamWriter = File.AppendText(logFilePath))
            {
                foreach (var line in lines) {
                    streamWriter.WriteLine($"{DateTime.Now.ToString()} : {line}");
                }
            }
        }

        public static void Write(ArrayList list)
        {
            var lines = new List<string>();
            foreach (var item in list)
            {
                lines.Add(item.ToString());
            }
            Write(lines.ToArray());
        }
        
        public static void Write<T>(IEnumerable<T> ts)
        {
            var lines = new List<string>();
            foreach (var elem in ts)
            {
                lines.Add(elem.ToString());
            }
            Write(lines.ToArray());
        }

        public static void Clear()
        {
            File.WriteAllText(logFilePath, String.Empty);
        }
    }
}
