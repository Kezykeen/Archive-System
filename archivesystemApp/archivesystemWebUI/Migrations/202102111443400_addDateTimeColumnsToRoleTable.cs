namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDateTimeColumnsToRoleTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "CreatedAt", c => c.DateTime());
            AddColumn("dbo.AspNetRoles", "UpDatedAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetRoles", "UpDatedAt");
            DropColumn("dbo.AspNetRoles", "CreatedAt");
        }
    }
}
