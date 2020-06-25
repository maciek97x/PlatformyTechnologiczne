using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections;
using System.IO.Compression;
using System.Windows;

namespace Lab5
{
    class GZip
    {
        private DirectoryInfo InputDirectoryInfo;
        private ListBox ResultListBox;
        private List<Tuple<FileInfo, Label>> files;

        public GZip(DirectoryInfo inputDirectoryInfo, ListBox resultListBox)
        {
            InputDirectoryInfo = inputDirectoryInfo;
            ResultListBox = resultListBox;

            ResultListBox.Items.Clear();
            files = new List<Tuple<FileInfo, Label>>();

            foreach (var file in InputDirectoryInfo.GetFiles())
            {
                StackPanel stackPanel = new StackPanel();
                Label fileNameLabel = new Label();
                Label fileStatusLabel = new Label();

                fileNameLabel.Width = 240;
                fileNameLabel.Content = file.Name;

                fileStatusLabel.Width = 120;

                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Children.Add(fileNameLabel);
                stackPanel.Children.Add(fileStatusLabel);

                ResultListBox.Items.Add(stackPanel);

                files.Add(new Tuple<FileInfo, Label>(file, fileStatusLabel));
            }
        }

        public void Compress()
        {
            List<Task> tasks = new List<Task>();
            foreach (var file in files)
            {
                tasks.Add(Task.Factory.StartNew(() => CompressFile(file.Item1)));
            }
            foreach (var file in files)
            {
                if ((File.GetAttributes(file.Item1.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden && file.Item1.Extension != ".gz")
                {
                    file.Item2.Content = "Compressed";
                }
            }
            Task.WaitAll(tasks.ToArray());
        }

        public void Decompress()
        {
            List<Task> tasks = new List<Task>();
            foreach (var file in files)
            {
                tasks.Add(Task.Factory.StartNew(() => DecompressFile(file.Item1)));
            }
            foreach (var file in files)
            {
                if ((File.GetAttributes(file.Item1.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden && file.Item1.Extension == ".gz")
                {
                    file.Item2.Content = "Decompressed";
                }
            }
            Task.WaitAll(tasks.ToArray());
        }

        private void CompressFile(FileInfo file)
        {
            if ((File.GetAttributes(file.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden && file.Extension != ".gz")
            {
                using (FileStream inputFileStream = file.OpenRead())
                using (FileStream outputFileStream = new FileStream(file.FullName + ".gz", FileMode.Create, FileAccess.Write))
                using (GZipStream gzipStream = new GZipStream(outputFileStream, CompressionMode.Compress))
                {
                    inputFileStream.CopyTo(gzipStream);
                }
            }
        }

        private void DecompressFile(FileInfo file)
        {
            if ((File.GetAttributes(file.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden && file.Extension == ".gz")
            {
                using (FileStream inputFileStream = file.OpenRead())
                using (FileStream outputFileStream = new FileStream(Path.Combine(file.Directory.FullName, Path.GetFileNameWithoutExtension(file.Name)), FileMode.Create, FileAccess.Write))
                using (GZipStream gzipStream = new GZipStream(inputFileStream, CompressionMode.Decompress))
                {
                    gzipStream.CopyTo(outputFileStream);
                }
            }
        }
    }
}
