using inRiver.Remoting.Extension;
using inRiver.Remoting.Log;
using System;
using System.Collections.Generic;
using System.IO;
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

            string path = @"C:\TengTools\Integration\inRiver-LitiumIntegration\Logging\" + DateTime.Now.ToShortDateString() + ".txt";
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(message);
                }
            }
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
