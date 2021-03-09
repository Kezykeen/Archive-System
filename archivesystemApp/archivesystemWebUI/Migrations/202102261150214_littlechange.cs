namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class littlechange : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Employees", "UserIdIndex");
            CreateIndex("dbo.Employees", "UserId", name: "UserIdIndex");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Employees", "UserIdIndex");
            CreateIndex("dbo.Employees", "UserId", unique: true, name: "UserIdIndex");
        }
    }
}
