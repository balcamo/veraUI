using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VeraAPI.Models.Tools
{
    public class Scribe
    {
        //Properties
        private string logFilePath;
        private string logFileName;
        private string logFile;

        public Scribe(string fileName)
        {
            this.logFileName = fileName;
            this.logFilePath = Path.GetTempPath();
            this.logFile = Path.Combine(this.logFilePath, this.logFileName);
        }

        public Scribe(string filePath, string fileName)
        {
            this.logFilePath = filePath;
            this.logFileName = fileName;
            this.logFile = Path.Combine(this.logFilePath, this.logFileName);
        }

        //Methods
        public void WriteLogEntry(string message)
        {
            if (File.Exists(this.logFile))
            {
                using (StreamWriter writer = new StreamWriter(this.logFile, true))
                {
                    writer.WriteLine("{0} {1}", String.Format("{0:yyyyMMdd HH:mm:ss}", DateTime.Now), message);
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(this.logFile))
                {
                    writer.WriteLine("{0} {1}", String.Format("{0:yyyyMMdd HH:mm:ss}", DateTime.Now), "Begin new log file");
                    writer.WriteLine("{0} {1}", String.Format("{0:yyyyMMdd HH:mm:ss}", DateTime.Now), message);
                }
            }
        }

        public bool DeleteLogFile()
        {
            bool result = false;
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
                result = true;
            }
            return result;
        }
    }
}