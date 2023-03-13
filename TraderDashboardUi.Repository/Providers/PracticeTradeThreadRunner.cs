using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace TraderDashboardUi.Repository.Providers
{
    public class PracticeTradeThreadRunner : IDisposable
    {
        private Thread _thread;
        public bool _isRunning = false;
        public int _elapsedTime = 0;
        public string _instrument;
        public string _strategy;

        public PracticeTradeThreadRunner()
        {
            this._thread = new Thread(StartPracticeTradingThread);
            this._thread.Start();
        }

        public void StartPracticeTradingThread()
        {
            var stopWatch = Stopwatch.StartNew();

            while (_isRunning)
            {
                _elapsedTime = (int)stopWatch.Elapsed.TotalSeconds;
                Thread.Sleep(1000);
            }
        }

        public void Dispose()
        {
            this._isRunning = false;
            this._thread.Join();
            this._thread = null;
        }
    }
}
