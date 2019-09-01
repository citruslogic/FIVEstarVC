namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resident_todelete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "ToDelete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "ToDelete");
        }
    }
}
