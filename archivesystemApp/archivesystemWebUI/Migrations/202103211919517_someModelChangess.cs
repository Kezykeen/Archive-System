namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class someModelChangess : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Approvals", "ApprovalDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Approvals", "ApprovalDate", c => c.DateTime(nullable: false));
        }
    }
}
