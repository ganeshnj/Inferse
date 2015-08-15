namespace inferse.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                        ProfileImageUrl = c.String(),
                        Location = c.String(),
                        Website = c.String(),
                        TimeZone = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Codes",
                c => new
                    {
                        CodeId = c.Int(nullable: false, identity: true),
                        Post = c.String(),
                        PostedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CodeId)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Followship",
                c => new
                    {
                        SourceUserId = c.Int(nullable: false),
                        TargetUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SourceUserId, t.TargetUserId })
                .ForeignKey("dbo.UserProfile", t => t.SourceUserId)
                .ForeignKey("dbo.UserProfile", t => t.TargetUserId)
                .Index(t => t.SourceUserId)
                .Index(t => t.TargetUserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Followship", new[] { "TargetUserId" });
            DropIndex("dbo.Followship", new[] { "SourceUserId" });
            DropIndex("dbo.Codes", new[] { "UserId" });
            DropForeignKey("dbo.Followship", "TargetUserId", "dbo.UserProfile");
            DropForeignKey("dbo.Followship", "SourceUserId", "dbo.UserProfile");
            DropForeignKey("dbo.Codes", "UserId", "dbo.UserProfile");
            DropTable("dbo.Followship");
            DropTable("dbo.Codes");
            DropTable("dbo.UserProfile");
        }
    }
}
