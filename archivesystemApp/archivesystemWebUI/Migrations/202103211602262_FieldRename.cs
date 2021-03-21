namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FieldRename : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Applications", name: "AttachFileId", newName: "AttachmentId");
            RenameIndex(table: "dbo.Applications", name: "IX_AttachFileId", newName: "IX_AttachmentId");
            AddColumn("dbo.Applications", "TicketTypeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Applications", "TicketTypeId");
            RenameIndex(table: "dbo.Applications", name: "IX_AttachmentId", newName: "IX_AttachFileId");
            RenameColumn(table: "dbo.Applications", name: "AttachmentId", newName: "AttachFileId");
        }
    }
}
