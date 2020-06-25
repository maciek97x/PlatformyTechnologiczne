using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
    class NChooseK
    {
        private UInt32 N;
        private UInt32 K;
        public UInt64 Result { get; private set; }

        public enum Mode
        {
            Tasks,
            Delegates,
            AsyncAwait
        }

        public NChooseK(UInt32 n, UInt32 k)
        {
            if (n < 0 || k < 0)
            {
                throw new ArgumentException("N and K should be non-negative.");
            }
            if (k > n)
            {
                throw new ArgumentException("N should be greater than K.");
            }
            N = n;
            K = k;
        }

        public void Compute(Mode mode) 
        {
            switch (mode)
            {
                case Mode.Tasks:
                    ComputeTasks();
                    break;
                case Mode.Delegates:
                    ComputeDelegates();
                    break;
                case Mode.AsyncAwait:
                    ComputeAsyncAwait();
                    break;
                default:
                    Result = 0;
                    break;
            }
        }

        private void ComputeTasks()
        {
            Task<UInt64> nominatorTask = Task.Factory.StartNew(
                (obj) => ComputeNominator(),
                100 //state
            );

            Task<UInt64> denominatorTask = Task.Factory.StartNew(
                (obj) => ComputeDenominator(),
                100 //state
            );

            nominatorTask.Wait();
            denominatorTask.Wait();

            Result = nominatorTask.Result / denominatorTask.Result;
        }

        private void ComputeDelegates()
        {
            Func<UInt64> nominatorFunc = ComputeNominator;
            Func<UInt64> denominatorFunc = ComputeDenominator;

            var nominator = nominatorFunc.BeginInvoke(null, null);
            var denominator = denominatorFunc.BeginInvoke(null, null);

            while (!nominator.IsCompleted && !denominator.IsCompleted);

            Result = nominatorFunc.EndInvoke(nominator) / denominatorFunc.EndInvoke(denominator);
        }

        private async void ComputeAsyncAwait()
        {
            var result = await ComputeAsyncAwaitTask();
            Result = result;
        }

        private async Task<UInt64> ComputeAsyncAwaitTask()
        {
            Func<UInt64> nominatorFunc = ComputeNominator;
            Func<UInt64> denominatorFunc = ComputeDenominator;

            var nominator = Task.Run(nominatorFunc);
            var denominator = Task.Run(denominatorFunc);

            await Task.WhenAll(nominator, denominator);

            return nominator.Result / denominator.Result;
        }

        private UInt64 ComputeNominator()
        {
            UInt64 nominator = 1;
            for (UInt32 i = N - K; i < N; nominator *= ++i);
            return nominator;
        }
        private UInt64 ComputeDenominator()
        {
            UInt64 denominator = 1;
            for (UInt32 i = 0; i < K; denominator *= ++i);
            return denominator;
        }
    }
}
