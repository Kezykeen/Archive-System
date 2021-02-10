using System.Globalization;

namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedDepartMent : DbMigration
    {
        public override void Up()
        {
            Sql($"Insert INTO Departments ( Name, UpdatedAt, CreatedAt ) VALUES ('HOD', '{DateTime.Now:G}', '{DateTime.Now:G}')");
            Sql($"Insert INTO Departments ( Name, UpdatedAt, CreatedAt ) VALUES ('HR', '{DateTime.Now:G}', '{DateTime.Now:G}')");
        }
        
        public override void Down()
        {
            Sql("DELETE FROM Departments");
        }
    }
}
