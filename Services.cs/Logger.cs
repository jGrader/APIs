namespace Services
{
    using System;
    using System.IO;

    public class Logger
    {
        private string _pathName;

        public void Log(Exception e)
        {
            var tmp = DateTime.Now.ToShortDateString();
            tmp = tmp.Replace(':', '-');
            tmp = tmp.Replace('/', '-');
            _pathName = Path.Combine(Directory.GetCurrentDirectory(), "\\App_Data\\Logs\\");
            Directory.CreateDirectory(_pathName);
            using (var s = File.AppendText(_pathName + tmp + ".log"))
            {
                s.Write(DateTime.Now.ToLocalTime() + " ");
                s.WriteLine(e.ToString());
            }
        }
    }
}
