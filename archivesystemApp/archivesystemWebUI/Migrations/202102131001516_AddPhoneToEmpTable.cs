namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPhoneToEmpTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "Phone", c => c.String());
            AddColumn("dbo.Employees", "Appointed", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Employees", "DOB", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "DOB", c => c.DateTime());
            DropColumn("dbo.Employees", "Appointed");
            DropColumn("dbo.Employees", "Phone");
        }
    }
}
