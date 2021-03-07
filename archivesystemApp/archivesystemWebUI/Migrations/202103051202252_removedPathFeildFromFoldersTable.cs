namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedPathFeildFromFoldersTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Folders", "Path");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Folders", "Path", c => c.String());
        }
    }
}
