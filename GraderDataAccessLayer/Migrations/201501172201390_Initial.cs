namespace GraderDataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        IsSuperUser = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserModel", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, unicode: false),
                        Surname = c.String(nullable: false, unicode: false),
                        Email = c.String(nullable: false, unicode: false),
                        UserName = c.String(nullable: false, unicode: false),
                        PasswordHash = c.String(unicode: false),
                        GraduationYear = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TeamModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EntityModel", t => t.EntityId)
                .Index(t => t.EntityId);
            
            CreateTable(
                "dbo.EntityModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, unicode: false),
                        Points = c.Int(nullable: false),
                        BonusPoints = c.Int(nullable: false),
                        OpenTime = c.DateTime(nullable: false, precision: 0),
                        CloseTime = c.DateTime(nullable: false, precision: 0),
                        TaskId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TaskModel", t => t.TaskId)
                .Index(t => t.TaskId);
            
            CreateTable(
                "dbo.FileModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(nullable: false, unicode: false),
                        Extension = c.String(nullable: false, unicode: false),
                        EntityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EntityModel", t => t.EntityId)
                .Index(t => t.EntityId);
            
            CreateTable(
                "dbo.TaskModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, unicode: false),
                        CourseId = c.Int(nullable: false),
                        GradeComponentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GradeComponentModel", t => t.GradeComponentId)
                .ForeignKey("dbo.CourseModel", t => t.CourseId)
                .Index(t => t.CourseId)
                .Index(t => t.GradeComponentId);
            
            CreateTable(
                "dbo.CourseModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, unicode: false),
                        CourseNumber = c.String(nullable: false, unicode: false),
                        ShortName = c.String(unicode: false),
                        Semester = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false, precision: 0),
                        EndDate = c.DateTime(nullable: false, precision: 0),
                        ExtensionLimit = c.Int(nullable: false),
                        ExcuseLimit = c.Int(nullable: false),
                        OwnerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserModel", t => t.OwnerId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.GradeComponentModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, unicode: false),
                        Percentage = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseModel", t => t.CourseId)
                .Index(t => t.CourseId);
            
            CreateTable(
                "dbo.CourseUserModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Permissions = c.Int(nullable: false),
                        ExtensionNumber = c.Int(nullable: false),
                        ExcuseNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseModel", t => t.CourseId)
                .ForeignKey("dbo.UserModel", t => t.UserId)
                .Index(t => t.CourseId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ExcuseModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        EntityId = c.Int(nullable: false),
                        IsGranted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EntityModel", t => t.EntityId)
                .ForeignKey("dbo.UserModel", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.EntityId);
            
            CreateTable(
                "dbo.ExtensionModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NewDeadline = c.DateTime(nullable: false, precision: 0),
                        UserId = c.Int(nullable: false),
                        EntityId = c.Int(nullable: false),
                        IsGranted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EntityModel", t => t.EntityId)
                .ForeignKey("dbo.UserModel", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.EntityId);
            
            CreateTable(
                "dbo.GradeModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Grade = c.Int(nullable: false),
                        BonusGrade = c.Int(nullable: false),
                        Comment = c.String(nullable: false, unicode: false),
                        TimeStamp = c.DateTime(nullable: false, precision: 0),
                        UserId = c.Int(nullable: false),
                        GraderId = c.Int(nullable: false),
                        EntityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EntityModel", t => t.EntityId)
                .ForeignKey("dbo.UserModel", t => t.GraderId)
                .ForeignKey("dbo.UserModel", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.GraderId)
                .Index(t => t.EntityId);
            
            CreateTable(
                "dbo.SessionIdModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SessionId = c.Guid(nullable: false),
                        ExpirationTime = c.DateTime(nullable: false, precision: 0),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserModel", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.SshKeyModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false, unicode: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserModel", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.SubmissionModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FilePath = c.String(nullable: false, unicode: false),
                        TimeStamp = c.DateTime(nullable: false, precision: 0),
                        UserId = c.Int(nullable: false),
                        FileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FileModel", t => t.FileId)
                .ForeignKey("dbo.UserModel", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.FileId);
            
            CreateTable(
                "dbo.UserTeams",
                c => new
                    {
                        TeamId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TeamId, t.UserId })
                .ForeignKey("dbo.TeamModel", t => t.TeamId, cascadeDelete: true)
                .ForeignKey("dbo.UserModel", t => t.UserId, cascadeDelete: true)
                .Index(t => t.TeamId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubmissionModel", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.SubmissionModel", "FileId", "dbo.FileModel");
            DropForeignKey("dbo.SshKeyModel", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.SessionIdModel", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.GradeModel", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.GradeModel", "GraderId", "dbo.UserModel");
            DropForeignKey("dbo.GradeModel", "EntityId", "dbo.EntityModel");
            DropForeignKey("dbo.ExtensionModel", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.ExtensionModel", "EntityId", "dbo.EntityModel");
            DropForeignKey("dbo.ExcuseModel", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.ExcuseModel", "EntityId", "dbo.EntityModel");
            DropForeignKey("dbo.CourseUserModel", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.CourseUserModel", "CourseId", "dbo.CourseModel");
            DropForeignKey("dbo.AdminModel", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.UserTeams", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.UserTeams", "TeamId", "dbo.TeamModel");
            DropForeignKey("dbo.TeamModel", "EntityId", "dbo.EntityModel");
            DropForeignKey("dbo.EntityModel", "TaskId", "dbo.TaskModel");
            DropForeignKey("dbo.TaskModel", "CourseId", "dbo.CourseModel");
            DropForeignKey("dbo.CourseModel", "OwnerId", "dbo.UserModel");
            DropForeignKey("dbo.TaskModel", "GradeComponentId", "dbo.GradeComponentModel");
            DropForeignKey("dbo.GradeComponentModel", "CourseId", "dbo.CourseModel");
            DropForeignKey("dbo.FileModel", "EntityId", "dbo.EntityModel");
            DropIndex("dbo.UserTeams", new[] { "UserId" });
            DropIndex("dbo.UserTeams", new[] { "TeamId" });
            DropIndex("dbo.SubmissionModel", new[] { "FileId" });
            DropIndex("dbo.SubmissionModel", new[] { "UserId" });
            DropIndex("dbo.SshKeyModel", new[] { "UserId" });
            DropIndex("dbo.SessionIdModel", new[] { "UserId" });
            DropIndex("dbo.GradeModel", new[] { "EntityId" });
            DropIndex("dbo.GradeModel", new[] { "GraderId" });
            DropIndex("dbo.GradeModel", new[] { "UserId" });
            DropIndex("dbo.ExtensionModel", new[] { "EntityId" });
            DropIndex("dbo.ExtensionModel", new[] { "UserId" });
            DropIndex("dbo.ExcuseModel", new[] { "EntityId" });
            DropIndex("dbo.ExcuseModel", new[] { "UserId" });
            DropIndex("dbo.CourseUserModel", new[] { "UserId" });
            DropIndex("dbo.CourseUserModel", new[] { "CourseId" });
            DropIndex("dbo.GradeComponentModel", new[] { "CourseId" });
            DropIndex("dbo.CourseModel", new[] { "OwnerId" });
            DropIndex("dbo.TaskModel", new[] { "GradeComponentId" });
            DropIndex("dbo.TaskModel", new[] { "CourseId" });
            DropIndex("dbo.FileModel", new[] { "EntityId" });
            DropIndex("dbo.EntityModel", new[] { "TaskId" });
            DropIndex("dbo.TeamModel", new[] { "EntityId" });
            DropIndex("dbo.AdminModel", new[] { "UserId" });
            DropTable("dbo.UserTeams");
            DropTable("dbo.SubmissionModel");
            DropTable("dbo.SshKeyModel");
            DropTable("dbo.SessionIdModel");
            DropTable("dbo.GradeModel");
            DropTable("dbo.ExtensionModel");
            DropTable("dbo.ExcuseModel");
            DropTable("dbo.CourseUserModel");
            DropTable("dbo.GradeComponentModel");
            DropTable("dbo.CourseModel");
            DropTable("dbo.TaskModel");
            DropTable("dbo.FileModel");
            DropTable("dbo.EntityModel");
            DropTable("dbo.TeamModel");
            DropTable("dbo.UserModel");
            DropTable("dbo.AdminModel");
        }
    }
}
