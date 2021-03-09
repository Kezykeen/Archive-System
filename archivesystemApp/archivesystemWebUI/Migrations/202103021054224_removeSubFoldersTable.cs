namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeSubFoldersTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SubFolders", "AccessLevelId", "dbo.AccessLevels");
            DropForeignKey("dbo.SubFolders", "FolderId", "dbo.Folders");
            DropIndex("dbo.SubFolders", new[] { "FolderId" });
            DropIndex("dbo.SubFolders", new[] { "AccessLevelId" });
            DropTable("dbo.SubFolders");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SubFolders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        FolderId = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.SubFolders", "AccessLevelId");
            CreateIndex("dbo.SubFolders", "FolderId");
            AddForeignKey("dbo.SubFolders", "FolderId", "dbo.Folders", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubFolders", "AccessLevelId", "dbo.AccessLevels", "Id", cascadeDelete: true);
        }
    }
}
