namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTimeStampToAccessDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessDetails", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.AccessDetails", "UpdatedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccessDetails", "UpdatedAt");
            DropColumn("dbo.AccessDetails", "CreatedAt");
        }
    }
}
