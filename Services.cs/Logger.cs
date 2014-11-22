using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Logger
    {
        private readonly string _pathName;

        public Logger()
        {
            _pathName = ConfigurationManager.AppSettings["pathName"];
        }
        public void Log(Exception e)
        {
            using (var s = File.AppendText(String.Format(_pathName, DateTime.Now.ToShortDateString())))
            {
                s.Write(DateTime.Now.ToLocalTime());
            }
        }
    }
}
