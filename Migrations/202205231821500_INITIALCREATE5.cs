namespace ManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class INITIALCREATE5 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Requests", "SignedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Requests", "SignedDate", c => c.Guid());
        }
    }
}
