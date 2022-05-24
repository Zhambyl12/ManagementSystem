namespace ManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requests", "SignerFIO", c => c.String());
            AddColumn("dbo.Requests", "ExecutorFIO", c => c.String());
            AddColumn("dbo.Requests", "UserFIO", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requests", "UserFIO");
            DropColumn("dbo.Requests", "ExecutorFIO");
            DropColumn("dbo.Requests", "SignerFIO");
        }
    }
}
