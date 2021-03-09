namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileMeta : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileMetas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        FolderId = c.Int(nullable: false),
                        UploadedById = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        File_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.File_Id)
                .ForeignKey("dbo.Folders", t => t.FolderId)
                .ForeignKey("dbo.Employees", t => t.UploadedById)
                .Index(t => t.FolderId)
                .Index(t => t.UploadedById)
                .Index(t => t.File_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileMetas", "UploadedById", "dbo.Employees");
            DropForeignKey("dbo.FileMetas", "FolderId", "dbo.Folders");
            DropForeignKey("dbo.FileMetas", "File_Id", "dbo.Files");
            DropIndex("dbo.FileMetas", new[] { "File_Id" });
            DropIndex("dbo.FileMetas", new[] { "UploadedById" });
            DropIndex("dbo.FileMetas", new[] { "FolderId" });
            DropTable("dbo.FileMetas");
        }
    }
}
