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

        public static JObject ToJson(this AdminModel am)
        {
            var result = new JObject();

            result.Add("Id", am.Id);
            result.Add("UserId", am.UserId);
            result.Add("IsSuperUser", am.IsSuperUser);

            return result;
        }

        public static JObject ToJson(this CourseUserModel cum)
        {
            var result = new JObject();

            result.Add("Id", cum.Id);
            result.Add("CourseId", cum.CourseId);
            result.Add("UserId", cum.UserId);
            result.Add("Permissions", cum.Permissions);

            return result;
        }

        public static JObject ToJson(this EntityModel em)
        {
            var result = new JObject();

            result.Add("Id", em.Id);
            result.Add("Points", em.Points);
            result.Add("BonusPoints", em.BonusPoints);
            result.Add("TaskId", em.TaskId);
            result.Add("OpenTime", em.OpenTime);
            result.Add("CloseTime", em.CloseTime);
            result.Add("Name", em.Name);

            return result;
        }

        public static JObject ToJson(this ExcuseModel em)
        {
            var result = new JObject();

            result.Add("Id", em.Id);
            result.Add("EntityId", em.EntityId);
            result.Add("UserId", em.UserId);

            return result;
        }

        public static JObject ToJson(this ExtensionModel em)
        {
            var result = new JObject();

            result.Add("Id", em.Id);
            result.Add("EntityId", em.EntityId);
            result.Add("UserId", em.UserId);

            return result;
        }

        public static JObject ToJson(this GradeComponentModel gcm)
        {
            var result = new JObject();

            result.Add("Id", gcm.Id);
            result.Add("CourseId", gcm.CourseId);
            result.Add("Name", gcm.Name);
            result.Add("Percentage", gcm.Percentage);

            return result;
        }

        public static JObject ToJson(this GradeModel gm)
        {
            var result = new JObject();

            result.Add("Id", gm.Id);
            result.Add("UserId", gm.UserId);
            result.Add("GraderId", gm.GraderId);
            result.Add("EntityId", gm.EntityId);
            result.Add("Grade", gm.Grade);
            result.Add("BonusGrade", gm.BonusGrade);
            result.Add("Comment", gm.Comment);

            return result;
        }

        public static JObject ToJson(this SshKeyModel skm)
        {
            var result = new JObject();

            result.Add("Id", skm.Id);
            result.Add("Key", skm.Key);
            result.Add("UserId", skm.UserId);

            return result;
        }

        public static JObject ToJson(this SubmissionModel sm)
        {
            var result = new JObject();

            result.Add("Id", sm.Id);
            result.Add("UserId", sm.UserId);
            result.Add("EntityId", sm.EntityId);
            result.Add("FileName", sm.FileName);
            result.Add("FilePath", sm.FilePath);
            result.Add("TimeStamp", sm.TimeStamp);

            return result;
        }

        public static JObject ToJson(this TaskModel tm)
        {
            var result = new JObject();

            result.Add("Id", tm.Id);
            result.Add("Name", tm.Name);
            result.Add("GradeComponentId", tm.GradeComponentId);
            result.Add("CourseId", tm.CourseId);

            return result;
        }

        public static JObject ToJson(this TeamMemberModel tmm)
        {
            var result = new JObject();

            result.Add("Id", tmm.Id);
            result.Add("TeamId", tmm.TeamId);
            result.Add("UserId", tmm.UserId);

            return result;
        }

        public static JObject ToJson(this TeamModel tm)
        {
            var result = new JObject();

            result.Add("Id", tm.Id);
            result.Add("CourseId", tm.CourseId);

            return result;
        }

        public static JObject ToJson(this UserModel um)
        {
            var result = new JObject();

            result.Add("Id", um.Id);
            result.Add("Name", um.Name);
            result.Add("SureName", um.SureName);
            result.Add("Email", um.Email);
            result.Add("UserName", um.UserName);
            result.Add("PasswordHash", um.PasswordHash);
            result.Add("GraduationYear", um.GraduationYear);

            return result;
        }
    }
}
