namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chngesToEmp : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Employees", "StaffId", unique: true, name: "StaffIdIndex");
            DropColumn("dbo.Employees", "MaritalStatus");
            DropColumn("dbo.Employees", "Address");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "Address", c => c.String());
            AddColumn("dbo.Employees", "MaritalStatus", c => c.Int(nullable: false));
            DropIndex("dbo.Employees", "StaffIdIndex");
        }
    }
}
