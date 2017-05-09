namespace InventoryManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Inventories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        UserId = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Category = c.String(nullable: false),
                        Type = c.String(nullable: false),
                        Amount = c.String(nullable: false),
                        Size = c.String(),
                        Color = c.String(),
                        InventoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Inventories", t => t.InventoryId, cascadeDelete: true)
                .Index(t => t.InventoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "InventoryId", "dbo.Inventories");
            DropIndex("dbo.Items", new[] { "InventoryId" });
            DropTable("dbo.Items");
            DropTable("dbo.Inventories");
        }
    }
}
