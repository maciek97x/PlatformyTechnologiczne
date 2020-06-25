using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for CreateDialog.xaml
    /// </summary>
    public partial class CreateDialog : Window
    {
        public string path { get; private set; }
        public CreateDialog(string path)
        {
            InitializeComponent();
            this.path = path;
        }

        private void Ok(object sender, RoutedEventArgs e)
        {
            bool isFile = (bool)radioButtonFile.IsChecked;
            bool isDirectory = (bool)radioButtonDirectory.IsChecked;
            string name = textBoxName.Text;

            path = System.IO.Path.Combine(path, name);

            if (isFile && File.Exists(path))
            {
                System.Windows.MessageBox.Show("File already exists!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (isDirectory && Directory.Exists(path))
            {
                System.Windows.MessageBox.Show("Directory already exists!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (isFile && !Regex.IsMatch(name, "^[a-zA-Z0-9_~-]{1,8}\\.(txt|php|html)$"))
            {
                System.Windows.MessageBox.Show("Invalid name!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                FileAttributes attributes = FileAttributes.Normal;
                if (checkBoxReadOnly.IsChecked.GetValueOrDefault())
                {
                    attributes |= FileAttributes.ReadOnly;
                }
                if (checkBoxArchive.IsChecked.GetValueOrDefault())
                {
                    attributes |= FileAttributes.Archive;
                }
                if (checkBoxHidden.IsChecked.GetValueOrDefault())
                {
                    attributes |= FileAttributes.Hidden;
                }
                if (checkBoxSystem.IsChecked.GetValueOrDefault())
                {
                    attributes |= FileAttributes.System;
                }
                if (isFile)
                {
                    File.Create(path);
                }
                else if (isDirectory)
                {
                    Directory.CreateDirectory(path);
                }
                File.SetAttributes(path, attributes);
                this.DialogResult = true;
                Close();
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
