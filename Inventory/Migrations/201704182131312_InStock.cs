namespace InventoryManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InStock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inventories", "DefaultInstock", c => c.Int(nullable: false));
            AddColumn("dbo.Inventories", "DefaultTotal", c => c.Int(nullable: false));
            AddColumn("dbo.Items", "InStock", c => c.Int(nullable: false));
            AddColumn("dbo.Items", "Total", c => c.Int(nullable: false));
            DropColumn("dbo.Inventories", "DefaultAmount");
            DropColumn("dbo.Items", "Amount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Items", "Amount", c => c.Int(nullable: false));
            AddColumn("dbo.Inventories", "DefaultAmount", c => c.Int(nullable: false));
            DropColumn("dbo.Items", "Total");
            DropColumn("dbo.Items", "InStock");
            DropColumn("dbo.Inventories", "DefaultTotal");
            DropColumn("dbo.Inventories", "DefaultInstock");
        }
    }
}
