namespace archivesystemWebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedRootFolder : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO [dbo].[Folders] ( [Name], [CreatedAt], [UpdatedAt]) VALUES ( N'Root', N'2021-02-19 00:00:00', N'2021-02-19 00:00:00')");
        }

        public override void Down()
        {
            Sql("DELETE FROM [dbo].[Folders] Where [Name]= 'Root'");
        }
    }
}
