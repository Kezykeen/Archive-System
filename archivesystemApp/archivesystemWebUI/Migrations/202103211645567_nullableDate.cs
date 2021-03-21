namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullableDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ApplicationReceivers", "TimeRejected", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ApplicationReceivers", "TimeRejected", c => c.DateTime(nullable: false));
        }
    }
}
