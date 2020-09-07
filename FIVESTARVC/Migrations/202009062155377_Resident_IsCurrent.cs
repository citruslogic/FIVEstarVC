namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Resident_IsCurrent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "IsCurrent", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "IsCurrent");
        }
    }
}
