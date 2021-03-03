namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDeleteableFlagToFoldersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Folders", "IsDeletable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Folders", "IsDeletable");
        }
    }
}
