using System;
using System.Threading;
using System.Threading.Tasks;

namespace siteserver.commands
{
    internal class Test
    {
        public const string CommandName = "test";

        public static void Start()
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
    }
}
