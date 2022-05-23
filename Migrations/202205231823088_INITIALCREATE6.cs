namespace ManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class INITIALCREATE6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requests", "SignedDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Requests", "SignedDate");
        }
    }
}
