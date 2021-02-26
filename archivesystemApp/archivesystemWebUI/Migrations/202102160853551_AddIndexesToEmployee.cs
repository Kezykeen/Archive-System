namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexesToEmployee : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "UserId", c => c.String(maxLength: 50));
            AlterColumn("dbo.Employees", "Email", c => c.String(maxLength: 50));
            AlterColumn("dbo.Employees", "Name", c => c.String(maxLength: 50));
            AlterColumn("dbo.Employees", "Phone", c => c.String(maxLength: 50));
            AlterColumn("dbo.Employees", "StaffId", c => c.String(maxLength: 50));
            CreateIndex("dbo.Employees", "UserId", unique: true, name: "UserIdIndex");
            CreateIndex("dbo.Employees", "Email", unique: true, name: "EmailIndex");
            CreateIndex("dbo.Employees", "Name", unique: true, name: "NameIndex");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Employees", "NameIndex");
            DropIndex("dbo.Employees", "EmailIndex");
            DropIndex("dbo.Employees", "UserIdIndex");
            AlterColumn("dbo.Employees", "StaffId", c => c.String());
            AlterColumn("dbo.Employees", "Phone", c => c.String());
            AlterColumn("dbo.Employees", "Name", c => c.String());
            AlterColumn("dbo.Employees", "Email", c => c.String());
            AlterColumn("dbo.Employees", "UserId", c => c.String());
        }
    }
}
