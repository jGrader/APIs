namespace GraderDataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BeforeFirstRelease : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GradeModel", "UserModel_Id", c => c.Int());
            CreateIndex("dbo.GradeModel", "UserModel_Id");
            AddForeignKey("dbo.GradeModel", "UserModel_Id", "dbo.UserModel", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GradeModel", "UserModel_Id", "dbo.UserModel");
            DropIndex("dbo.GradeModel", new[] { "UserModel_Id" });
            DropColumn("dbo.GradeModel", "UserModel_Id");
        }
    }
}
