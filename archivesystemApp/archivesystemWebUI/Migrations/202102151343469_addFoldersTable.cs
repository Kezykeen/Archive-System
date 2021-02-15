namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFoldersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Folders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Folders", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.Folders", new[] { "DepartmentId" });
            DropTable("dbo.Folders");
        }
    }
}
