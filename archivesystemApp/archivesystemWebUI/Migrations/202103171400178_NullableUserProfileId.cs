namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableUserProfileId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AppUsers", "UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.AppUsers", new[] { "UserProfileId" });
            AlterColumn("dbo.AppUsers", "UserProfileId", c => c.Int());
            CreateIndex("dbo.AppUsers", "UserProfileId");
            AddForeignKey("dbo.AppUsers", "UserProfileId", "dbo.UserProfiles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AppUsers", "UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.AppUsers", new[] { "UserProfileId" });
            AlterColumn("dbo.AppUsers", "UserProfileId", c => c.Int(nullable: false));
            CreateIndex("dbo.AppUsers", "UserProfileId");
            AddForeignKey("dbo.AppUsers", "UserProfileId", "dbo.UserProfiles", "Id", cascadeDelete: true);
        }
    }
}
