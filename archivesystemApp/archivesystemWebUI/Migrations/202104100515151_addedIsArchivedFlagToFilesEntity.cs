namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedIsArchivedFlagToFilesEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "IsArchived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Files", "IsArchived");
        }
    }
}
