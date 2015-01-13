namespace Grader.JsonSerializer
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using GraderDataAccessLayer.Models;

    public static class JsonSerializer
    {
        public static JObject ToJson(this CourseModel cm)
        {
            var result = new JObject
            {
                {"Id", cm.Id},
                {"Name", cm.Name},
                {"ShortName", cm.ShortName},
                {"CourseNumber", cm.CourseNumber},
                {"Semester", cm.Semester},
                {"Year", cm.Year},
                {"StartDate", cm.StartDate},
                {"EndDate", cm.EndDate},
                {"OwnerId", cm.OwnerId}
            };

            return result;
        }

        public static JObject ToJson(this AdminModel am)
        {
            var result = new JObject
            {
                {"Id", am.Id}, 
                {"UserId", am.UserId}, 
                {"IsSuperUser", am.IsSuperUser}
            };

            return result;
        }

        public static JObject ToJson(this CourseUserModel cum)
        {
            var result = new JObject
            {
                {"Id", cum.Id},
                {"CourseId", cum.CourseId},
                {"UserId", cum.UserId},
                {"Permissions", cum.Permissions},
                {"ExtensionLimit", cum.ExtensionLimit},
                {"ExcuseLimit", cum.ExcuseLimit}
            };

            return result;
        }

        public static JObject ToJson(this EntityModel em)
        {
            var result = new JObject
            {
                {"Id", em.Id},
                {"Points", em.Points},
                {"BonusPoints", em.BonusPoints},
                {"TaskId", em.TaskId},
                {"OpenTime", em.OpenTime},
                {"CloseTime", em.CloseTime},
                {"Name", em.Name}
            };

            return result;
        }

        public static JObject ToJson(this ExcuseModel em)
        {
            var result = new JObject
            {
                {"Id", em.Id}, 
                {"EntityId", em.EntityId}, 
                {"UserId", em.UserId}
            };

            return result;
        }

        public static JObject ToJson(this ExtensionModel em)
        {
            var result = new JObject
            {
                {"Id", em.Id}, 
                {"EntityId", em.EntityId}, 
                {"UserId", em.UserId}
            };

            return result;
        }

        public static JObject ToJson(this GradeComponentModel gcm)
        {
            var result = new JObject
            {
                {"Id", gcm.Id},
                {"CourseId", gcm.CourseId},
                {"Name", gcm.Name},
                {"Percentage", gcm.Percentage}
            };

            return result;
        }

        public static JObject ToJson(this GradeModel gm)
        {
            var result = new JObject
            {
                {"Id", gm.Id},
                {"UserId", gm.UserId},
                {"GraderId", gm.GraderId},
                {"EntityId", gm.EntityId},
                {"Grade", gm.Grade},
                {"BonusGrade", gm.BonusGrade},
                {"Comment", gm.Comment}
            };

            return result;
        }

        public static JObject ToJson(this SshKeyModel skm)
        {
            var result = new JObject
            {
                {"Id", skm.Id}, 
                {"Key", skm.Key}, 
                {"UserId", skm.UserId}
            };

            return result;
        }

        public static JObject ToJson(this SubmissionModel sm)
        {
            var result = new JObject
            {
                {"Id", sm.Id},
                {"UserId", sm.UserId},
                {"FileId", sm.FileId},
                {"FilePath", sm.FilePath},
                {"TimeStamp", sm.TimeStamp}
            };

            return result;
        }

        public static JObject ToJson(this TaskModel tm)
        {
            var result = new JObject
            {
                {"Id", tm.Id},
                {"Name", tm.Name},
                {"GradeComponentId", tm.GradeComponentId},
                {"CourseId", tm.CourseId}
            };

            return result;
        }

        public static JObject ToJson(this TeamModel tm)
        {
            var result = new JObject
            {
                {"Id", tm.Id},
                {"EntityId", tm.EntityId},
                {"TeamMembers", tm.TeamMembers.ToJson()}
            };

            return result;
        }

        public static JObject ToJson(this UserModel um)
        {
            var result = new JObject
            {
                {"Id", um.Id},
                {"Name", um.Name},
                {"Surname", um.Surname},
                {"Email", um.Email},
                {"UserName", um.UserName},
                {"PasswordHash", um.PasswordHash},
                {"GraduationYear", um.GraduationYear}
            };

            return result;
        }

        public static JObject ToJson(this FileModel fm)
        {
            var result = new JObject
            {
                {"Id", fm.Id},
                {"FileName", fm.FileName},
                {"Extension", fm.Extension},
                {"EntityId", fm.EntityId}
            };

            return result;
        }

        public static JArray ToJson(this IEnumerable<FileModel> fm)
        {
            var query = (from f in fm select f.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<CourseModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<AdminModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<CourseUserModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<EntityModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<ExcuseModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<ExtensionModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<GradeComponentModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<GradeModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<SshKeyModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<SubmissionModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<TaskModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<TeamModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }

        public static JArray ToJson(this IEnumerable<UserModel> cm)
        {
            var query = (from c in cm select c.ToJson());
            var result = new JArray(query);
            return result;
        }
    }
}
