namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileTOMetaRshipChgess : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FileMetas", "UploadedById", "dbo.Employees");
            DropIndex("dbo.FileMetas", new[] { "UploadedById" });
            AddColumn("dbo.FileMetas", "UploadedBy_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.FileMetas", "UploadedBy_Id");
            AddForeignKey("dbo.FileMetas", "UploadedBy_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileMetas", "UploadedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.FileMetas", new[] { "UploadedBy_Id" });
            DropColumn("dbo.FileMetas", "UploadedBy_Id");
            CreateIndex("dbo.FileMetas", "UploadedById");
            AddForeignKey("dbo.FileMetas", "UploadedById", "dbo.Employees", "Id", cascadeDelete: true);
        }
    }
}
