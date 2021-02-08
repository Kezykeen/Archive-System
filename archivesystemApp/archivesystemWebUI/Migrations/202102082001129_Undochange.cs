namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Undochange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "CreatedAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.Departments", "Created");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Departments", "Created", c => c.DateTime(nullable: false));
            DropColumn("dbo.Departments", "CreatedAt");
        }
    }
}
