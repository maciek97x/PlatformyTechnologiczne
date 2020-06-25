using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lab1
{
    static class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];

            int[] maxPrintLength = MaxPrintLength(path, 0);
            if (Directory.Exists(path))
            {
                PrintDirectory(path, 0, maxPrintLength);
                Console.WriteLine("\nNajstarszy plik: {0}\n", (new DirectoryInfo(path)).GetOldestFile());
                CreateCollection(path);
            }
            else if (File.Exists(path))
            {
                PrintFile(path, 0, maxPrintLength);
            }
            else
            {
                Console.WriteLine("Błędna ścieżka.");
            }
        }

        private static int[] MaxPrintLength(string path, int depth)
        {
            if (Directory.Exists(path))
            {
                var directoryInfo = new DirectoryInfo(path);
                int[] maxPrintLength = new int[] { 2 * depth + directoryInfo.Name.Length, directoryInfo.GetFiles().Length.ToString().Length };
                foreach (var subDirectoryInfo in directoryInfo.GetDirectories())
                {
                    int[] subDirectoryPrintLength = MaxPrintLength(subDirectoryInfo.FullName, depth + 1);
                    maxPrintLength[0] = subDirectoryPrintLength[0] > maxPrintLength[0] ? subDirectoryPrintLength[0] : maxPrintLength[0];
                    maxPrintLength[1] = subDirectoryPrintLength[1] > maxPrintLength[1] ? subDirectoryPrintLength[1] : maxPrintLength[1];
                }
                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    int[] filePrintLength = MaxPrintLength(fileInfo.FullName, depth + 1);
                    maxPrintLength[0] = filePrintLength[0] > maxPrintLength[0] ? filePrintLength[0] : maxPrintLength[0];
                    maxPrintLength[1] = filePrintLength[1] > maxPrintLength[1] ? filePrintLength[1] : maxPrintLength[1];
                }
                return maxPrintLength;
            }
            else if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                return new int[] { 2 * depth + fileInfo.Name.Length, fileInfo.Length.ToString().Length};
            }
            return new int[] { 0, 0 };
        }

        private static void PrintDirectory(string path, int depth, int[] maxPrintLength)
        {
            var directoryInfo = new DirectoryInfo(path);
            Console.WriteLine(new String(' ', 2 * depth) + "{0} {1:d} plików {2}",
                directoryInfo.Name.PadRight(maxPrintLength[0] - 2 * depth),
                (directoryInfo.GetFiles().Length + directoryInfo.GetDirectories().Length).ToString().PadRight(maxPrintLength[1]),
                directoryInfo.GetRAHS());
            foreach (var subDirectoryInfo in directoryInfo.GetDirectories())
            {
                PrintDirectory(subDirectoryInfo.FullName, depth + 1, maxPrintLength);
            }
            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                PrintFile(fileInfo.FullName, depth + 1, maxPrintLength);
            }
        }
        private static void PrintFile(string path, int depth, int[] maxPrintLength)
        {
            var fileInfo = new FileInfo(path);
            Console.WriteLine(new String(' ', 2 * depth) + "{0} {1} bajtów {2}",
                fileInfo.Name.PadRight(maxPrintLength[0] - 2 * depth),
                fileInfo.Length.ToString().PadRight(maxPrintLength[1]),
                fileInfo.GetRAHS());
        }
        public static DateTime GetOldestFile(this DirectoryInfo directory)
        {
            var oldest = DateTime.MaxValue;

            foreach (var directoryInfo in directory.GetDirectories())
            {
                DateTime dirOldest = directoryInfo.GetOldestFile();
                if (dirOldest < oldest)
                {
                    oldest = dirOldest;
                }
            }
            foreach (var fileInfo in directory.GetFiles())
            {
                DateTime creationDate = fileInfo.CreationTime;
                if (creationDate < oldest)
                {
                    oldest = creationDate;
                }
            }
            return oldest;
        }
        public static string GetRAHS(this FileSystemInfo fileSystemInfo)
        {
            string rahs = "";

            FileAttributes fileAttributes = fileSystemInfo.Attributes;

            rahs += (fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly ? 'r': '-';
            rahs += (fileAttributes & FileAttributes.Archive) == FileAttributes.Archive ? 'a' : '-';
            rahs += (fileAttributes & FileAttributes.Hidden) == FileAttributes.Hidden ? 'h' : '-';
            rahs += (fileAttributes & FileAttributes.System) == FileAttributes.System ? 's' : '-';

            return rahs;
        }
        public static void CreateCollection(string path)
        {
            var collection = new SortedDictionary<string, int>(new MyStringComparer());
            if (Directory.Exists(path))
            {
                var directoryInfo = new DirectoryInfo(path);
                foreach (var subDirectoryInfo in directoryInfo.GetDirectories())
                {
                    collection.Add(subDirectoryInfo.Name, (subDirectoryInfo.GetFiles().Length + subDirectoryInfo.GetDirectories().Length));
                }
                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    collection.Add(fileInfo.Name, (int)fileInfo.Length);
                }
            }
            else if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                collection.Add(fileInfo.Name, (int)fileInfo.Length);
            }
            var binaryFormatter = new BinaryFormatter();
            var fileStream = new FileStream("DataFile.dat", FileMode.Create);
            try
            {
                binaryFormatter.Serialize(fileStream, collection);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Błąd serializacji: {0}", e.Message);
            }
            fileStream.Close();
            Deserialize();
        }

        public static void Deserialize()
        {
            var collection = new SortedDictionary<string, int>(new MyStringComparer());
            var fileStream = new FileStream("DataFile.dat", FileMode.Open);
            try
            {
                var binaryFormatter = new BinaryFormatter();
                collection = (SortedDictionary<string, int>)binaryFormatter.Deserialize(fileStream);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Błąd serializacji: {0}", e.Message);
            }

            foreach (var file in collection)
            {
                Console.WriteLine("{0} -> {1}", file.Key, file.Value);
            }
        }
    }
}
