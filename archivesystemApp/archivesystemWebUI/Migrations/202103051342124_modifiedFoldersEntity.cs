namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiedFoldersEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Folders", "IsRestricted", c => c.Boolean(nullable: false));
            DropColumn("dbo.Folders", "IsDeletable");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Folders", "IsDeletable", c => c.Boolean(nullable: false));
            DropColumn("dbo.Folders", "IsRestricted");
        }
    }
}
