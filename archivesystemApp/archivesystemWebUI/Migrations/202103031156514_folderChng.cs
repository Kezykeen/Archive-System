namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class folderChng : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Employees", "UserIdIndex");
            AddColumn("dbo.Files", "ContentType", c => c.String());
            AddColumn("dbo.Files", "Content", c => c.Binary());
            AddColumn("dbo.Files", "AccessLevelId", c => c.Int());
            AddColumn("dbo.Files", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Files", "UpdatedAt", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Employees", "UserId", name: "UserIdIndex");
            CreateIndex("dbo.Files", "AccessLevelId");
            AddForeignKey("dbo.Files", "AccessLevelId", "dbo.AccessLevels", "Id");
            DropColumn("dbo.Files", "FolderId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Files", "FolderId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Files", "AccessLevelId", "dbo.AccessLevels");
            DropIndex("dbo.Files", new[] { "AccessLevelId" });
            DropIndex("dbo.Employees", "UserIdIndex");
            DropColumn("dbo.Files", "UpdatedAt");
            DropColumn("dbo.Files", "CreatedAt");
            DropColumn("dbo.Files", "AccessLevelId");
            DropColumn("dbo.Files", "Content");
            DropColumn("dbo.Files", "ContentType");
            CreateIndex("dbo.Employees", "UserId", unique: true, name: "UserIdIndex");
        }
    }
}
