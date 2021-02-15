namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedAccessLevelEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessLevels", "LevelDescription", c => c.String());
            AlterColumn("dbo.AccessLevels", "Level", c => c.String());
            DropColumn("dbo.AccessLevels", "LevelName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccessLevels", "LevelName", c => c.String());
            AlterColumn("dbo.AccessLevels", "Level", c => c.Int(nullable: false));
            DropColumn("dbo.AccessLevels", "LevelDescription");
        }
    }
}
