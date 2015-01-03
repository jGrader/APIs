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

    public enum AdminPermissions
    {
        CanCreateCourse = 1,
        CanUpdateCourse = 1<<1,
        CanDeleteCourse = 1<<2,
        CanCreateGradedPart = 1<<3,
        CanUpdateGradedPart = 1<<4,
        CanDeleteGradedPart = 1<<5,
    }
}