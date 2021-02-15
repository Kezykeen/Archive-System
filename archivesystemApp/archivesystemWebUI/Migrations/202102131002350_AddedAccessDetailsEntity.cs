namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAccessDetailsEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccessDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        EmployeeName = c.String(),
                        AccessLevel = c.String(),
                        AccessCode = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AccessDetails");
        }
    }
}
