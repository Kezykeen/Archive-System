namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubFoldersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SubFolders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        FolderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Folders", t => t.FolderId, cascadeDelete: true)
                .Index(t => t.FolderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubFolders", "FolderId", "dbo.Folders");
            DropIndex("dbo.SubFolders", new[] { "FolderId" });
            DropTable("dbo.SubFolders");
        }
    }
}
