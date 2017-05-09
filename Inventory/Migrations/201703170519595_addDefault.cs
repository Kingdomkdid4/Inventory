namespace InventoryManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDefault : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inventories", "DefaultName", c => c.String());
            AddColumn("dbo.Inventories", "DefaultCategory", c => c.String());
            AddColumn("dbo.Inventories", "DefaultType", c => c.String());
            AddColumn("dbo.Inventories", "DefaultAmount", c => c.Int(nullable: false));
            AddColumn("dbo.Inventories", "DefaultSize", c => c.String());
            AddColumn("dbo.Inventories", "DefaultColor", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Inventories", "DefaultColor");
            DropColumn("dbo.Inventories", "DefaultSize");
            DropColumn("dbo.Inventories", "DefaultAmount");
            DropColumn("dbo.Inventories", "DefaultType");
            DropColumn("dbo.Inventories", "DefaultCategory");
            DropColumn("dbo.Inventories", "DefaultName");
        }
    }
}
