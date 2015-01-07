namespace GraderApi
{
    public enum SuperUserPermissions
    {
        CanDeleteUser = 1    
    }

    public enum AdminPermissions
    {
        CanCreateCourse = 1,
        CanUpdateCourse = 1 << 1,
        CanDeleteCourse = 1 << 2,
    }

    public enum CourseOwnerPermissions
    {
        CanCreateGradedPart = 1,
        CanUpdateGradedPart = 1 << 2,
        CanDeleteGradedPart = 1 << 3,
        CanAddEnrollment = 1 << 4,
        CanUpdateEnrollment = 1 << 5,
        CanDeleteEnrollment = 1 << 6,
    }

    public enum CoursePermissions
    {
        Nothing = 0,
        CanSeeGrades = 1,
        CanCreateTasks = 1<<1,
        CanUpdateTasks = 1 << 2,
        CanDeleteTasks = 1 << 3,
        CanCreateEntities = 1 << 4,
        CanUpdateEntities = 1 << 5,
        CanDeleteEntities = 1 << 6,
    }
}