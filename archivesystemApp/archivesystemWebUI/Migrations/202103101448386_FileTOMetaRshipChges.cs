namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileTOMetaRshipChges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FileMetas", "File_Id", "dbo.Files");
            DropForeignKey("dbo.FileMetas", "FolderId", "dbo.Folders");
            DropIndex("dbo.FileMetas", new[] { "FolderId" });
            DropIndex("dbo.FileMetas", new[] { "File_Id" });
            AddColumn("dbo.Files", "FileMetaId", c => c.Int(nullable: false));
            CreateIndex("dbo.Files", "FileMetaId");
            AddForeignKey("dbo.Files", "FileMetaId", "dbo.FileMetas", "Id", cascadeDelete: true);
            DropColumn("dbo.FileMetas", "FolderId");
            DropColumn("dbo.FileMetas", "File_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FileMetas", "File_Id", c => c.Int());
            AddColumn("dbo.FileMetas", "FolderId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Files", "FileMetaId", "dbo.FileMetas");
            DropIndex("dbo.Files", new[] { "FileMetaId" });
            DropColumn("dbo.Files", "FileMetaId");
            CreateIndex("dbo.FileMetas", "File_Id");
            CreateIndex("dbo.FileMetas", "FolderId");
            AddForeignKey("dbo.FileMetas", "FolderId", "dbo.Folders", "Id", cascadeDelete: true);
            AddForeignKey("dbo.FileMetas", "File_Id", "dbo.Files", "Id");
        }
    }
}
