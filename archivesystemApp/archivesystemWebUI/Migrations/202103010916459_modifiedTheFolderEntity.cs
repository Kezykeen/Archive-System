namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiedTheFolderEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Folders", "ParentId", c => c.Int());
            CreateIndex("dbo.Folders", "ParentId");
            AddForeignKey("dbo.Folders", "ParentId", "dbo.Folders", "Id",cascadeDelete:false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Folders", "ParentId", "dbo.Folders");
            DropIndex("dbo.Folders", new[] { "ParentId" });
            DropColumn("dbo.Folders", "ParentId");
        }
    }
}
