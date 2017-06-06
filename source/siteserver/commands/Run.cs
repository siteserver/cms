using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace siteserver.commands
{
    internal class Run
    {
        public const string CommandName = "run";

        public class CreateThread
        {
            public void Run()
            {
                ExecutionManager.ClearAllPendingCreate();

                var timeout = 1000;

                while (true)
                {
                    ServiceManager.SetServiceOnline(true);
                    if (ExecutionManager.ExecutePendingCreate())
                    {
                        Console.WriteLine("Create Pages: " + DateUtils.GetDateAndTimeString(DateTime.Now));
                        timeout = 10;
                    }
                    else if (timeout < 10000)
                    {
                        timeout += 1000;
                    }

                    Thread.Sleep(timeout);
                }
                // ReSharper disable once FunctionNeverReturns
            }
        }

        public class TaskThread
        {
            public void Run()
            {
                while (true)
                {
                    if (ExecutionManager.ExecuteTask())
                    {
                        Console.WriteLine("Execute Tasks: " + DateUtils.GetDateAndTimeString(DateTime.Now));
                    }

                    Thread.Sleep(60000);
                }
                // ReSharper disable once FunctionNeverReturns
            }
        }

        public static void Start()
        {
            // Some biolerplate to react to close window event, CTRL-C, kill, etc
            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);

            Console.WriteLine("SiteServer Service is running...");

            var thr1 = new CreateThread();
            var thr2 = new TaskThread();

            var tid1 = new Thread(thr1.Run);
            var tid2 = new Thread(thr2.Run);

            tid1.IsBackground = true;
            tid1.Priority = ThreadPriority.Highest;
            tid2.IsBackground = true;
            tid2.Priority = ThreadPriority.Lowest;

            try
            {
                tid1.Start();
                tid2.Start();
            }
            catch (ThreadStateException te)
            {
                Console.WriteLine(te.ToString());
            }

            var watcher = new FileSystemWatcher
            {
                Path = Environment.CurrentDirectory,
                IncludeSubdirectories = true,
                Filter = "*.*"
            };

            watcher.Changed += (sender, e) =>
            {
                if (PathUtils.IsSystemPath(e.FullPath)) return;

                try
                {
                    watcher.EnableRaisingEvents = false;

                    ServiceUtils.OnFileChanged(sender, e);
                }

                finally
                {
                    watcher.EnableRaisingEvents = true;
                }
            };
            watcher.EnableRaisingEvents = true;

            Thread.Sleep(10);
            while (true) { }

            //_commandRun = new CommandRun();
            //_commandRun.StartService();
        }

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        private static EventHandler _handler;

        private enum CtrlType { }

        private static bool Handler(CtrlType sig)
        {
            //do your cleanup here
            ServiceManager.SetServiceOnline(false);

            Console.WriteLine("SiteServer Service is shutting down...");

            //allow main to run off
            //_exitSystem = true;

            //_commandRun?.EndService();

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
    }
}
