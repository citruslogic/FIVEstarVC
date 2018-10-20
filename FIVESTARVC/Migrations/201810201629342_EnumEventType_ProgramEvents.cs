namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnumEventType_ProgramEvents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgramType", "EventType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProgramType", "EventType");
        }
    }
}
