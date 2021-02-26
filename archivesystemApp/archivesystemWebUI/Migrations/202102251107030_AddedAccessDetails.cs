namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAccessDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccessDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        AccessLevelId = c.Int(nullable: false),
                        AccessCode = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccessLevels", t => t.AccessLevelId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.AccessLevelId);
            
        }
        
        public override void Down()
        {
           
            
            DropForeignKey("dbo.AccessDetails", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.AccessDetails", "AccessLevelId", "dbo.AccessLevels");
            DropIndex("dbo.AccessDetails", new[] { "AccessLevelId" });
            DropIndex("dbo.AccessDetails", new[] { "EmployeeId" });
            DropTable("dbo.AccessDetails");
        }
    }
}
