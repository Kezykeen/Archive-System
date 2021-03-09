namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAccessLevelFieldToFolderTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Folders", "AccessLevelId", c => c.Int());
            CreateIndex("dbo.Folders", "AccessLevelId");
            AddForeignKey("dbo.Folders", "AccessLevelId", "dbo.AccessLevels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Folders", "AccessLevelId", "dbo.AccessLevels");
            DropIndex("dbo.Folders", new[] { "AccessLevelId" });
            DropColumn("dbo.Folders", "AccessLevelId");
        }
    }
}
