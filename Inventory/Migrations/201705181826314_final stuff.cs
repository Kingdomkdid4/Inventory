namespace InventoryManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finalstuff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "Description", c => c.String());
            AddColumn("dbo.Items", "Image", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "Image");
            DropColumn("dbo.Items", "Description");
        }
    }
}
