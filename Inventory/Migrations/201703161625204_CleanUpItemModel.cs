namespace Inventory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CleanUpItemModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Items", "Amount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Items", "Amount", c => c.String(nullable: false));
        }
    }
}
