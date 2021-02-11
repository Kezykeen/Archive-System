namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeName : DbMigration
    {
        public override void Up()
        {
            
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Employees", newName: "Staffs");
        }
    }
}
