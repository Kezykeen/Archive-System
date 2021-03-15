namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FolderFileRHip : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Employees", "UserIdIndex");
            RenameColumn(table: "dbo.Files", name: "Folder_Id", newName: "FolderId");
            RenameIndex(table: "dbo.Files", name: "IX_Folder_Id", newName: "IX_FolderId");
            CreateTable(
                "dbo.FileMetas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        UploadedById = c.String(maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UploadedById)
                .Index(t => t.UploadedById);
            
            AddColumn("dbo.Files", "ContentType", c => c.String());
            AddColumn("dbo.Files", "Content", c => c.Binary());
            AddColumn("dbo.Files", "FileMetaId", c => c.Int(nullable: false));
            AddColumn("dbo.Files", "AccessLevelId", c => c.Int());
            AddColumn("dbo.Files", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Files", "UpdatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Folders", "DepartmentId", c => c.Int());
            AddColumn("dbo.Folders", "FacultyId", c => c.Int());
            AddColumn("dbo.Folders", "IsRestricted", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Folders", "Name", c => c.String());
            CreateIndex("dbo.Employees", "UserId", name: "UserIdIndex");
            CreateIndex("dbo.Files", "FileMetaId");
            CreateIndex("dbo.Files", "AccessLevelId");
            CreateIndex("dbo.Folders", "DepartmentId");
            CreateIndex("dbo.Folders", "FacultyId");
            AddForeignKey("dbo.Files", "AccessLevelId", "dbo.AccessLevels", "Id");
            AddForeignKey("dbo.Files", "FileMetaId", "dbo.FileMetas", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Folders", "DepartmentId", "dbo.Departments", "Id");
            AddForeignKey("dbo.Folders", "FacultyId", "dbo.Faculties", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Folders", "FacultyId", "dbo.Faculties");
            DropForeignKey("dbo.Folders", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Files", "FileMetaId", "dbo.FileMetas");
            DropForeignKey("dbo.Files", "AccessLevelId", "dbo.AccessLevels");
            DropForeignKey("dbo.FileMetas", "UploadedById", "dbo.AspNetUsers");
            DropIndex("dbo.Folders", new[] { "FacultyId" });
            DropIndex("dbo.Folders", new[] { "DepartmentId" });
            DropIndex("dbo.Files", new[] { "AccessLevelId" });
            DropIndex("dbo.Files", new[] { "FileMetaId" });
            DropIndex("dbo.FileMetas", new[] { "UploadedById" });
            DropIndex("dbo.Employees", "UserIdIndex");
            AlterColumn("dbo.Folders", "Name", c => c.String(nullable: false));
            DropColumn("dbo.Folders", "IsRestricted");
            DropColumn("dbo.Folders", "FacultyId");
            DropColumn("dbo.Folders", "DepartmentId");
            DropColumn("dbo.Files", "UpdatedAt");
            DropColumn("dbo.Files", "CreatedAt");
            DropColumn("dbo.Files", "AccessLevelId");
            DropColumn("dbo.Files", "FileMetaId");
            DropColumn("dbo.Files", "Content");
            DropColumn("dbo.Files", "ContentType");
            DropTable("dbo.FileMetas");
            RenameIndex(table: "dbo.Files", name: "IX_FolderId", newName: "IX_Folder_Id");
            RenameColumn(table: "dbo.Files", name: "FolderId", newName: "Folder_Id");
            CreateIndex("dbo.Employees", "UserId", unique: true, name: "UserIdIndex");
        }
    }
}
