namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileTOMetaRshipChges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FileMetas", "UploadedById", "dbo.Employees");
            DropIndex("dbo.FileMetas", new[] { "UploadedById" });
            AlterColumn("dbo.FileMetas", "UploadedById", c => c.String(maxLength: 128));
            CreateIndex("dbo.FileMetas", "UploadedById");
            AddForeignKey("dbo.FileMetas", "UploadedById", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileMetas", "UploadedById", "dbo.AspNetUsers");
            DropIndex("dbo.FileMetas", new[] { "UploadedById" });
            AlterColumn("dbo.FileMetas", "UploadedById", c => c.Int(nullable: false));
            CreateIndex("dbo.FileMetas", "UploadedById");
            AddForeignKey("dbo.FileMetas", "UploadedById", "dbo.Employees", "Id", cascadeDelete: true);
        }
    }
}
