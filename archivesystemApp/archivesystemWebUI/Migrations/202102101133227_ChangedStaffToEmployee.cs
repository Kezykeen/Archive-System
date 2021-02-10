namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedStaffToEmployee : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Staffs", newName: "Employees");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Employees", newName: "Staffs");
        }
    }
}
