namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeptStaffModelCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Staffs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Email = c.String(),
                        Name = c.String(),
                        StaffId = c.String(),
                        DOB = c.DateTime(),
                        MaritalStatus = c.Int(nullable: false),
                        Address = c.String(),
                        DepartmentId = c.Int(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Staffs", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.Staffs", new[] { "DepartmentId" });
            DropTable("dbo.Staffs");
            DropTable("dbo.Departments");
        }
    }
}
