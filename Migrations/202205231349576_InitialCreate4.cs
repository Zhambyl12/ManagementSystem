namespace ManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StartedDate = c.DateTime(nullable: false),
                        FinishedDate = c.DateTime(),
                        UserId = c.Guid(nullable: false),
                        ExecutorId = c.Guid(nullable: false),
                        SignerId = c.Guid(),
                        SignedDate = c.Guid(),
                        DocName = c.String(),
                        DocUrl = c.String(),
                        ProcessType = c.String(nullable: false),
                        Status = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Requests");
        }
    }
}
