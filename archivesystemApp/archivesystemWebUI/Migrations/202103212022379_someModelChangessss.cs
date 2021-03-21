namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class someModelChangessss : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Files", "FileMetaId", "dbo.FileMetas");
            DropIndex("dbo.Files", new[] { "FileMetaId" });
            AlterColumn("dbo.Files", "FileMetaId", c => c.Int());
            CreateIndex("dbo.Files", "FileMetaId");
            AddForeignKey("dbo.Files", "FileMetaId", "dbo.FileMetas", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Files", "FileMetaId", "dbo.FileMetas");
            DropIndex("dbo.Files", new[] { "FileMetaId" });
            AlterColumn("dbo.Files", "FileMetaId", c => c.Int(nullable: false));
            CreateIndex("dbo.Files", "FileMetaId");
            AddForeignKey("dbo.Files", "FileMetaId", "dbo.FileMetas", "Id", cascadeDelete: true);
        }
    }
}
