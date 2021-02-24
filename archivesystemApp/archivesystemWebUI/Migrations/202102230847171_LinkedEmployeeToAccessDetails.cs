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
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessDetails", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.AccessDetails", new[] { "EmployeeId" });
        }
    }
}
