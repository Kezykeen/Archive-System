namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class someModelChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Approvals", "ApprovalDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Approvals", "InviteDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Approvals", "UpdatedAt");
            DropColumn("dbo.Approvals", "CreatedAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Approvals", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Approvals", "UpdatedAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.Approvals", "InviteDate");
            DropColumn("dbo.Approvals", "ApprovalDate");
        }
    }
}
