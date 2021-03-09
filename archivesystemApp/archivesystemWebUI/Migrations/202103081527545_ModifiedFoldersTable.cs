namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedFoldersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Folders", "DepartmentId", c => c.Int());
            AddColumn("dbo.Folders", "FacultyId", c => c.Int());
            CreateIndex("dbo.Folders", "DepartmentId");
            CreateIndex("dbo.Folders", "FacultyId");
            AddForeignKey("dbo.Folders", "DepartmentId", "dbo.Departments", "Id");
            AddForeignKey("dbo.Folders", "FacultyId", "dbo.Faculties", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Folders", "FacultyId", "dbo.Faculties");
            DropForeignKey("dbo.Folders", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.Folders", new[] { "FacultyId" });
            DropIndex("dbo.Folders", new[] { "DepartmentId" });
            DropColumn("dbo.Folders", "FacultyId");
            DropColumn("dbo.Folders", "DepartmentId");
        }
    }
}
