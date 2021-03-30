using inRiver.Remoting.Extension;
using inRiver.Remoting.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration.Logging
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
