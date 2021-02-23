namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkedEmployeeToAccessDetails : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AccessDetails", "EmployeeId");
            AddForeignKey("dbo.AccessDetails", "EmployeeId", "dbo.Employees", "Id", cascadeDelete: true);
            DropColumn("dbo.AccessDetails", "EmployeeName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccessDetails", "EmployeeName", c => c.String());
            DropForeignKey("dbo.AccessDetails", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.AccessDetails", new[] { "EmployeeId" });
        }
    }
}
