using System;
using System.Threading;
using SiteServer.CMS.Core;

namespace siteserver
{
    internal class CommandRun
    {
        private readonly Thread _pendingCreateThread;
        private readonly Thread _taskThread;
        private bool _pendingCreateStarted;
        private bool _taskStarted;

        public CommandRun()
        {
            var stExecutePendingCreate = new ThreadStart(ExecutePendingCreate);
            var stExecuteTask = new ThreadStart(ExecuteTask);
            _pendingCreateThread = new Thread(stExecutePendingCreate);
            _taskThread = new Thread(stExecuteTask);

            _pendingCreateStarted = true;
            _taskStarted = true;
        }

        public void StartService()
        {
            _pendingCreateThread.Start();
            _taskThread.Start();
        }

        public void EndService()
        {
            _pendingCreateStarted = false;
            _taskStarted = false;
        }

        private void ExecutePendingCreate()
        {
            while (_pendingCreateStarted)
            {
                ServiceManager.SetServiceOnline(true);
                ExecutionManager.ExecutePendingCreate();

                if (_pendingCreateStarted)
                {
                    Thread.Sleep(new TimeSpan(0, 1, 0));
                }
            }
        }

        private void ExecuteTask()
        {
            while (_taskStarted)
            {
                ExecutionManager.ExecuteTask();

                if (_taskStarted)
                {
                    Thread.Sleep(new TimeSpan(0, 1, 0));
                }
            }
        }
    }
}
