using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Models;

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
                    ServiceManager.SetServiceOnline(DateTime.Now);
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

        public class FileSystemWatcherThread
        {
            private static FileSystemWatcher _watcher;

            public void Run()
            {
                _watcher = new FileSystemWatcher
                {
                    Path = Environment.CurrentDirectory,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.CreationTime | NotifyFilters.Size,
                    IncludeSubdirectories = true
                };

                _watcher.Created += Watcher_EventHandler;
                _watcher.Changed += Watcher_EventHandler;
                _watcher.Deleted += Watcher_EventHandler;
                _watcher.Renamed += Watcher_EventHandler;
                _watcher.EnableRaisingEvents = true;
            }

            private static void Watcher_EventHandler(object sender, FileSystemEventArgs e)
            {
                //Console.WriteLine(e.FullPath);
                if (PathUtils.IsSystemPath(e.FullPath)) return;

                try
                {
                    _watcher.EnableRaisingEvents = false;

                    foreach (var action in PluginCache.GetFileSystemChangedActions())
                    {
                        try
                        {
                            action(sender, e);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }

                finally
                {
                    _watcher.EnableRaisingEvents = true;
                }
            }
        }

        public static void Start()
        {
            // Some biolerplate to react to close window event, CTRL-C, kill, etc
            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);

            PluginManager.Load(new PluginEnvironment(EDatabaseTypeUtils.GetValue(WebConfigUtils.DatabaseType), WebConfigUtils.ConnectionString,
                WebConfigUtils.PhysicalApplicationPath, true));

            Console.WriteLine("SiteServer Service is running...");

            var thr1 = new CreateThread();
            var thr2 = new TaskThread();
            var thr3 = new FileSystemWatcherThread();

            var tid1 = new Thread(thr1.Run);
            var tid2 = new Thread(thr2.Run);
            var tid3 = new Thread(thr3.Run);

            tid1.IsBackground = true;
            tid1.Priority = ThreadPriority.Highest;
            tid2.IsBackground = true;
            tid2.Priority = ThreadPriority.Lowest;
            tid3.IsBackground = true;
            tid3.Priority = ThreadPriority.Normal;

            try
            {
                tid1.Start();
                tid2.Start();
                tid3.Start();
            }
            catch (ThreadStateException te)
            {
                Console.WriteLine(te.ToString());
            }

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
            ServiceManager.SetServiceOffline();

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
