namespace GraderApi
{
    public enum SuperUserPermissions
    {
        CanDeleteUser = 1,
        CanSeeAllUsers = 1 << 2,
        CanSeeAllCourseUsers = 1 << 3,
        CanSeeAllGradedParts = 1 << 4,
        CanSeeAllTasks = 1 << 5,
        CanSeeAllEntities = 1 << 6,
        CanSeeAllFiles = 1 << 7,
        CanSeeAllSubmissions = 1 << 8,
        CanDeleteSubmission = 1 << 9,
        CanSeeAllTeams = 1 << 10,
        CanSeeAllGrades = 1 << 11
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
        CanSeeEnrollment = 1 << 4,
        CanAddEnrollment = 1 << 5,
        CanUpdateEnrollment = 1 << 6,
        CanDeleteEnrollment = 1 << 7,
    }

    public enum CoursePermissions
    {
        Nothing = 0,
        CanSeeGrades = 1,
        CanSeeGradedParts = 1 << 1,
        CanSeeTasks = 1 << 2,
        CanCreateTasks = 1 << 3,
        CanUpdateTasks = 1 << 4,
        CanDeleteTasks = 1 << 5,
        CanSeeEntities = 1 << 6,
        CanCreateEntities = 1 << 7,
        CanUpdateEntities = 1 << 8,
        CanDeleteEntities = 1 << 9,
        CanSeeFiles = 1 << 10,
        CanCreateFiles = 1 << 11,
        CanUpdateFiles = 1 << 12,
        CanDeleteFiles = 1 << 13,
        CanSeeSubmissions = 1 << 14,
        CanCreateSubmissions = 1 << 15,
        CanUpdateSubmissions = 1 << 16,
        CanSeeTeams = 1 << 17,
        CanCreateTeams = 1 << 18,
        CanUpdateTeams = 1 << 19,
        CanDeleteTeams = 1 << 20,
        CanGrade = 1 << 21,
    }
}