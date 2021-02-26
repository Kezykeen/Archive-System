namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCheckedToEmployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "Completed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "Completed");
        }
    }
}
