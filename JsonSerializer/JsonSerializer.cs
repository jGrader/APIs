namespace JsonSerializer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using GraderDataAccessLayer.Models;

    public static class JsonSerializer
    {
        public static JObject ToJson(this CourseModel cm)
        {
            var result = new JObject();

            result.Add("Id", cm.Id);
            result.Add("Name", cm.Name);
            result.Add("ShortName", cm.ShortName);
            result.Add("CourseNumber", cm.CourseNumber);
            result.Add("Semester", cm.Semester);
            result.Add("StartTime", cm.StartTime);
            result.Add("EndTime", cm.EndTime);
            result.Add("OwnerId", cm.OwnerId);

            return result;
        }
    }
}
