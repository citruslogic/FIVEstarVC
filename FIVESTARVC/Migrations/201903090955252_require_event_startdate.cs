namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class require_event_startdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProgramEvent", "StartDate", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProgramEvent", "StartDate", c => c.String());
        }
    }
}
