namespace ManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "ElectronicsId", "dbo.Electronics");
            DropIndex("dbo.AspNetUsers", new[] { "ElectronicsId" });
            AlterColumn("dbo.AspNetUsers", "ElectronicsId", c => c.Guid());
            CreateIndex("dbo.AspNetUsers", "ElectronicsId");
            AddForeignKey("dbo.AspNetUsers", "ElectronicsId", "dbo.Electronics", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "ElectronicsId", "dbo.Electronics");
            DropIndex("dbo.AspNetUsers", new[] { "ElectronicsId" });
            AlterColumn("dbo.AspNetUsers", "ElectronicsId", c => c.Guid(nullable: false));
            CreateIndex("dbo.AspNetUsers", "ElectronicsId");
            AddForeignKey("dbo.AspNetUsers", "ElectronicsId", "dbo.Electronics", "Id", cascadeDelete: true);
        }
    }
}
