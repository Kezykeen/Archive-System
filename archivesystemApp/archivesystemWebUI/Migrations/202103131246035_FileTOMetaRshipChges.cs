namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileTOMetaRshipChges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "FolderId", c => c.Int());
            CreateIndex("dbo.Files", "FolderId");
            AddForeignKey("dbo.Files", "FolderId", "dbo.Folders", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Files", "FolderId", "dbo.Folders");
            DropIndex("dbo.Files", new[] { "FolderId" });
            DropColumn("dbo.Files", "FolderId");
        }
    }
}
