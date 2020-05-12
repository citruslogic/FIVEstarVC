namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadingSelectOption_NGReserve_Territory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "StateTerritoryOther", c => c.String(maxLength: 100));
            AddColumn("dbo.Person", "NGReserve", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "NGReserve");
            DropColumn("dbo.Person", "StateTerritoryOther");
        }
    }
}
