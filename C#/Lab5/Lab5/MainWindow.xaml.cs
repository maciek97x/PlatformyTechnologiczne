using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO;

namespace Lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GZip gzip;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NChooseKTasksButton_Click(object sender, RoutedEventArgs e)
        {
            NChooseKCompute(NChooseK.Mode.Tasks, NChooseKTasksTextBox);
        }

        private void NChooseKDelegatesButton_Click(object sender, RoutedEventArgs e)
        {
            NChooseKCompute(NChooseK.Mode.Delegates, NChooseKDelegatesTextBox);
        }

        private void NChooseKAsyncAwaitButton_Click(object sender, RoutedEventArgs e)
        {
            NChooseKCompute(NChooseK.Mode.AsyncAwait, NChooseKAsyncAwaitTextBox);
        }

        private void NChooseKCompute(NChooseK.Mode mode, TextBox outputTextBox)
        {
            UInt32 n = UInt32.Parse(NChooseKInputNTextBox.Text);
            UInt32 k = UInt32.Parse(NChooseKInputKTextBox.Text);

            NChooseKErrorText.Content = String.Empty;
            try
            {
                NChooseK nchoosek = new NChooseK(n, k);

                nchoosek.Compute(mode);

                outputTextBox.Text = nchoosek.Result.ToString();
            }
            catch (ArgumentException exception)
            {
                NChooseKErrorText.Content = exception.Message;
            }
        }

        private void FibonacciComputeButton_Click(object sender, RoutedEventArgs e)
        {
            UInt32 n = UInt32.Parse(FibonacciNTextBox.Text);
            Fibonacci fibonacci = new Fibonacci(n, FibonacciResultTextBox, FibonacciProgressBar);
            fibonacci.Compute();
            FibonacciResultTextBox.Text = fibonacci.Result.ToString();
        }

        private void GZipChooseDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var directoryInfo = new DirectoryInfo(dialog.SelectedPath);
                gzip = new GZip(directoryInfo, GZipListBox);
            }
        }

        private void GZipCompressButton_Click(object sender, RoutedEventArgs e)
        {
            if (gzip != null)
            {
                gzip.Compress();
            }
        }

        private void GZipDecompressButton_Click(object sender, RoutedEventArgs e)
        {
            if (gzip != null)
            {
                gzip.Decompress();
            }
        }

        private void DNSButton_Click(object sender, RoutedEventArgs e)
        {
            var resolved = DNSResolve.ResolveDomains();

            foreach (StackPanel panel in DNSListBox.Items)
            {
                string hostName = ((Label)panel.Children[0]).Content.ToString();
                ((Label)panel.Children[1]).Content = resolved[hostName];
            }
        }

        private void DNSListBox_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var hostName in DNSResolve.HostNames) {
                StackPanel stackPanel = new StackPanel();
                Label hostNameLabel = new Label();
                Label hostAddressLabel = new Label();

                hostNameLabel.Width = 240;
                hostNameLabel.Content = hostName;

                hostAddressLabel.Width = 120;

                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Children.Add(hostNameLabel);
                stackPanel.Children.Add(hostAddressLabel);

                DNSListBox.Items.Add(stackPanel);
            }
        }

        private void NChooseKInputValidation(object sender, TextChangedEventArgs e)
        {
            UInt32 n;
            UInt32 k;

            bool valid = UInt32.TryParse(NChooseKInputNTextBox.Text, out n) & UInt32.TryParse(NChooseKInputKTextBox.Text, out k);

            valid = valid & (k <= n);

            NChooseKTasksButton.IsEnabled = valid;
            NChooseKDelegatesButton.IsEnabled = valid;
            NChooseKAsyncAwaitButton.IsEnabled = valid;
        }

        private void FibonacciInputValidation(object sender, TextChangedEventArgs e)
        {
            UInt32 i;

            bool valid = UInt32.TryParse(FibonacciNTextBox.Text, out i);

            FibonacciComputeButton.IsEnabled = valid;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DisableSpaceTextBox(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}
