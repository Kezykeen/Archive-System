namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class someModelChangesss : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Applications", "ApplicationType_Id", "dbo.Tickets");
            DropIndex("dbo.Applications", new[] { "ApplicationType_Id" });
            RenameColumn(table: "dbo.Applications", name: "ApplicationType_Id", newName: "ApplicationTypeId");
            AlterColumn("dbo.Applications", "ApplicationTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Applications", "ApplicationTypeId");
            AddForeignKey("dbo.Applications", "ApplicationTypeId", "dbo.Tickets", "Id", cascadeDelete: true);
            DropColumn("dbo.Applications", "TicketTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Applications", "TicketTypeId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Applications", "ApplicationTypeId", "dbo.Tickets");
            DropIndex("dbo.Applications", new[] { "ApplicationTypeId" });
            AlterColumn("dbo.Applications", "ApplicationTypeId", c => c.Int());
            RenameColumn(table: "dbo.Applications", name: "ApplicationTypeId", newName: "ApplicationType_Id");
            CreateIndex("dbo.Applications", "ApplicationType_Id");
            AddForeignKey("dbo.Applications", "ApplicationType_Id", "dbo.Tickets", "Id");
        }
    }
}
