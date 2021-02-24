namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAccessDetailsEntity1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessDetails", "AccessLevelId", c => c.Int(nullable: false));
            AddColumn("dbo.AccessDetails", "AccessLevelName", c => c.String());
            CreateIndex("dbo.AccessDetails", "AccessLevelId");
            AddForeignKey("dbo.AccessDetails", "AccessLevelId", "dbo.AccessLevels", "Id", cascadeDelete: true);
            DropColumn("dbo.AccessDetails", "AccessLevel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccessDetails", "AccessLevel", c => c.String());
            DropForeignKey("dbo.AccessDetails", "AccessLevelId", "dbo.AccessLevels");
            DropIndex("dbo.AccessDetails", new[] { "AccessLevelId" });
            DropColumn("dbo.AccessDetails", "AccessLevelName");
            DropColumn("dbo.AccessDetails", "AccessLevelId");
        }
    }
}
