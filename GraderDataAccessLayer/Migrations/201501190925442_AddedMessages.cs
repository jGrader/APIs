namespace GraderDataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMessages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GradeId = c.Int(nullable: false),
                        MessageId = c.Int(nullable: false),
                        Contents = c.String(nullable: false, unicode: false),
                        UserId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GradeModel", t => t.GradeId)
                .ForeignKey("dbo.MessageModel", t => t.MessageId)
                .ForeignKey("dbo.UserModel", t => t.UserId)
                .Index(t => t.GradeId)
                .Index(t => t.MessageId)
                .Index(t => t.UserId);
            
            DropColumn("dbo.GradeModel", "Comment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GradeModel", "Comment", c => c.String(nullable: false, unicode: false));
            DropForeignKey("dbo.MessageModel", "UserId", "dbo.UserModel");
            DropForeignKey("dbo.MessageModel", "MessageId", "dbo.MessageModel");
            DropForeignKey("dbo.MessageModel", "GradeId", "dbo.GradeModel");
            DropIndex("dbo.MessageModel", new[] { "UserId" });
            DropIndex("dbo.MessageModel", new[] { "MessageId" });
            DropIndex("dbo.MessageModel", new[] { "GradeId" });
            DropTable("dbo.MessageModel");
        }
    }
}
