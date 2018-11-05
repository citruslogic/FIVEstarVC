namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoomLastResident : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Room", "LastResident", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Room", "LastResident");
        }
    }
}
