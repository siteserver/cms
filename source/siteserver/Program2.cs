using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using SiteServer.CMS.Core;

namespace siteserver
{
    internal class Program
    {
        private static CommandRun _commandRun;

        private static void Main(string[] args)
        {
            if (!EnvironmentManager.IsSiteServerDir)
            {
                Console.Write("当前文件夹不是正确的SiteServer系统根目录");
                Console.ReadLine();
            }
            else if (!EnvironmentManager.IsInitialized)
            {
                Console.Write("SiteServer系统未安装或数据库无法正确连接");
                Console.ReadLine();
            }

            var invokedVerb = string.Empty;
            object invokedVerbInstance = null;
            if (args.Length == 0)
            {
                invokedVerb = "run";
            }
            else
            {
                var options = new Options();
                if (!Parser.Default.ParseArguments(args, options,
                    (verb, subOptions) =>
                    {
                        invokedVerb = verb;
                        invokedVerbInstance = subOptions;
                    }))
                {
                    Console.WriteLine(options.GetUsage());
                    return;
                }
            }

            if (invokedVerb == "build")
            {
                var commitSubOptions = (BuildSubOptions)invokedVerbInstance;
                var isAll = commitSubOptions != null && commitSubOptions.All;
                Console.WriteLine(isAll);
                Console.WriteLine("build site...");
            }
            else if (invokedVerb == "test")
            {
                var i = 0;
                while (true)
                {
                    Console.WriteLine("Application thread ID: {0}, I:{1}",
                        Thread.CurrentThread.ManagedThreadId, i++);
                    var t = Task.Run(() => {
                        Console.WriteLine("Task thread ID: {0}",
                           Thread.CurrentThread.ManagedThreadId);
                    });
                    t.Wait();

                    Thread.Sleep(5000);
                }
            }
            else if (invokedVerb == "run")
            {
                // Some biolerplate to react to close window event, CTRL-C, kill, etc
                _handler += Handler;
                SetConsoleCtrlHandler(_handler, true);

                Console.WriteLine("SiteServer Service is running...");
                //Console.CancelKeyPress += CurrentDomain_ProcessExit;

                _commandRun = new CommandRun();
                _commandRun.StartService();

                //var manager = new ExecutionManager();

                //while (!_exitSystem)
                //{
                //    ServiceManager.SetServiceOnline(true);
                //    manager.ExecutePendingCreate();
                //    manager.ExecuteTask();
                //    Thread.Sleep(1000);
                //}
            }
        }

        //protected static void CurrentDomain_ProcessExit(object sender, ConsoleCancelEventArgs args)
        //{
        //    Console.WriteLine("CurrentDomain_ProcessExit");
        //    Thread.Sleep(750);
        //    if (args.SpecialKey == ConsoleSpecialKey.ControlC)
        //    {
        //        ServiceManager.SetServiceOnline(false);
        //        Console.WriteLine("Shutting down...");
        //        Thread.Sleep(750);
        //    }
        //}

        //private static bool _exitSystem;

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        private static EventHandler _handler;

        private enum CtrlType{ }

        private static bool Handler(CtrlType sig)
        {
            //do your cleanup here
            ServiceManager.SetServiceOnline(false);

            Console.WriteLine("SiteServer Service is shutting down...");

            //allow main to run off
            //_exitSystem = true;

            _commandRun?.EndService();

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
    }
}
