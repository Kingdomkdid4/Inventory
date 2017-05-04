namespace Inventory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sharing : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sharings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InventoryId = c.Int(nullable: false),
                        Email = c.String(nullable: false),
                        Permission = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Inventories", t => t.InventoryId, cascadeDelete: true)
                .Index(t => t.InventoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sharings", "InventoryId", "dbo.Inventories");
            DropIndex("dbo.Sharings", new[] { "InventoryId" });
            DropTable("dbo.Sharings");
        }
    }
}
