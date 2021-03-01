namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFilesCollectionToFoldersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "Folder_Id", c => c.Int());
            CreateIndex("dbo.Files", "Folder_Id");
            AddForeignKey("dbo.Files", "Folder_Id", "dbo.Folders", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Files", "Folder_Id", "dbo.Folders");
            DropIndex("dbo.Files", new[] { "Folder_Id" });
            DropColumn("dbo.Files", "Folder_Id");
        }
    }
}
