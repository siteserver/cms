using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using BaiRong.Core;
using CommandLine;
using SiteServer.CMS.Core;

namespace siteserver
{
    public class CreateThread
    {
        public void Run()
        {
            while (true)
            {
                ServiceManager.SetServiceOnline(true);
                if (ExecutionManager.ExecutePendingCreate())
                {
                    Console.WriteLine("Create Pages: " + DateUtils.GetDateAndTimeString(DateTime.Now));
                }

                Thread.Sleep(5000);
            }
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
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            if (!ServiceUtils.IsSiteServerDir)
            {
                Console.WriteLine("当前文件夹不是正确的SiteServer系统根目录");
                Console.ReadLine();
                return;
            }
            if (!ServiceUtils.IsInitialized)
            {
                Console.WriteLine("SiteServer系统未安装或数据库无法正确连接");
                Console.ReadLine();
                return;
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

                Thread.Sleep(10);
                while (true) { }

                //_commandRun = new CommandRun();
                //_commandRun.StartService();
            }
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

