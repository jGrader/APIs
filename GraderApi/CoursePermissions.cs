using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GraderApi
{
    public enum CoursePermissions
    {
        Nothing = 0,
        CanSeeGrades = 1,
        CanCreateTasks = 1<<1,
        CanGrade = 1<<2,
        CanUploadForOthers = 1<<3,
        CanGrantExtensions = 1<<4,
        CanGrantExcuses = 1<<5,
        CanSeeFullGrades = 1<<6,

    }
}