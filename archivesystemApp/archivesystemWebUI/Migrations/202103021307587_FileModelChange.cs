namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileModelChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "FolderId", c => c.Int(nullable: false));
            AddColumn("dbo.Folders", "FileId", c => c.Int());
            CreateIndex("dbo.Folders", "FileId");
            AddForeignKey("dbo.Folders", "FileId", "dbo.Files", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Folders", "FileId", "dbo.Files");
            DropIndex("dbo.Folders", new[] { "FileId" });
            DropColumn("dbo.Folders", "FileId");
            DropColumn("dbo.Files", "FolderId");
        }
    }
}
