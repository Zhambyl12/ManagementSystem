namespace ManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "ElectronicsId", "dbo.Electronics");
            DropIndex("dbo.AspNetUsers", new[] { "ElectronicsId" });
            AddColumn("dbo.Electronics", "UserId", c => c.Guid());
            AddColumn("dbo.Electronics", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Electronics", "ApplicationUser_Id");
            AddForeignKey("dbo.Electronics", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.AspNetUsers", "ElectronicsId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "ElectronicsId", c => c.Guid());
            DropForeignKey("dbo.Electronics", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Electronics", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Electronics", "ApplicationUser_Id");
            DropColumn("dbo.Electronics", "UserId");
            CreateIndex("dbo.AspNetUsers", "ElectronicsId");
            AddForeignKey("dbo.AspNetUsers", "ElectronicsId", "dbo.Electronics", "Id");
        }
    }
}
