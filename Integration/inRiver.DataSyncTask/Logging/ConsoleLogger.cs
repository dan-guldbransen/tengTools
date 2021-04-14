using inRiver.Remoting.Extension;
using inRiver.Remoting.Log;
using System;

namespace inRiver.DataSyncTask
{
    public class ConsoleLogger : IExtensionLog
    {
        public void Log(LogLevel level, string message)
        {
            Console.WriteLine($"{level} - {message}");
        }

        public void Log(LogLevel level, string message, Exception ex)
        {
            Console.WriteLine($"{level} - {message}");
            Console.WriteLine();
            Console.WriteLine(ex);
            Console.WriteLine();
        }
    }
}
