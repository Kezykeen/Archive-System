namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnsToSubFoldersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubFolders", "AccessLevelId", c => c.Int(nullable: false));
            CreateIndex("dbo.SubFolders", "AccessLevelId");
            AddForeignKey("dbo.SubFolders", "AccessLevelId", "dbo.AccessLevels", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubFolders", "AccessLevelId", "dbo.AccessLevels");
            DropIndex("dbo.SubFolders", new[] { "AccessLevelId" });
            DropColumn("dbo.SubFolders", "AccessLevelId");
        }
    }
}
