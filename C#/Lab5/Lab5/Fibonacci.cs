using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Lab5
{
    class Fibonacci
    {
        private UInt32 N;
        private TextBox ResultTextBox;
        private ProgressBar Progress;
        public UInt64 Result { get; private set; }

        public Fibonacci(UInt32 n, TextBox resultTextBox=null, ProgressBar progress=null)
        {
            Progress = progress;
            ResultTextBox = resultTextBox;
            if (n < 0)
            {
                throw new ArgumentException("I should be non-negative.");
            }
            N = n;
        }

        public void Compute()
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += WorkerDoWork;
            worker.RunWorkerCompleted += WorkerRunWorkerCompleted;
            worker.ProgressChanged += WorkerProgressChanged;
            Progress.Value = 0;
            worker.RunWorkerAsync(N);
        }
        private void WorkerDoWork(object sender, DoWorkEventArgs args)
        {
            var worker = sender as BackgroundWorker;
            if (worker != null)
            {
                worker.WorkerReportsProgress = true;
                args.Result = ComputeTerm((UInt32)args.Argument, worker, args);
            }
        }

        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            Result = (UInt64)args.Result;
            if (ResultTextBox != null)
            {
                ResultTextBox.Text = Result.ToString();
            }
        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs args)
        {
            if (Progress != null)
            {
                Progress.Value = args.ProgressPercentage;
            }
        }

        private UInt64 ComputeTerm(UInt32 n, BackgroundWorker worker, DoWorkEventArgs args)
        {
            if (worker.CancellationPending)
            {
                args.Cancel = true;
                return 0;
            }
            else
            {
                UInt32 a = 1, b = 0;
                for (UInt32 i = 0; i < n; ++i)
                {
                    b = a + b;
                    a = b - a;
                    worker.ReportProgress((int)((float)(i + 1) / n * 100));
                    Thread.Sleep(20);
                }
                worker.ReportProgress(100);
                return b;
            }
        }
    }
}
