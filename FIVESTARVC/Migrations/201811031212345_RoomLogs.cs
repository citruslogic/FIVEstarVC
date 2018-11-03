namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoomLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoomLog",
                c => new
                    {
                        RoomLogID = c.Int(nullable: false, identity: true),
                        ResidentID = c.Int(nullable: false),
                        RoomNumber = c.Int(),
                        Comment = c.String(),
                        AdmittedEvent_ProgramEventID = c.Int(),
                        ReleasedEvent_ProgramEventID = c.Int(),
                    })
                .PrimaryKey(t => t.RoomLogID)
                .ForeignKey("dbo.ProgramEvent", t => t.AdmittedEvent_ProgramEventID)
                .ForeignKey("dbo.ProgramEvent", t => t.ReleasedEvent_ProgramEventID)
                .ForeignKey("dbo.Person", t => t.ResidentID, cascadeDelete: true)
                .ForeignKey("dbo.Room", t => t.RoomNumber)
                .Index(t => t.ResidentID)
                .Index(t => t.RoomNumber)
                .Index(t => t.AdmittedEvent_ProgramEventID)
                .Index(t => t.ReleasedEvent_ProgramEventID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoomLog", "RoomNumber", "dbo.Room");
            DropForeignKey("dbo.RoomLog", "ResidentID", "dbo.Person");
            DropForeignKey("dbo.RoomLog", "ReleasedEvent_ProgramEventID", "dbo.ProgramEvent");
            DropForeignKey("dbo.RoomLog", "AdmittedEvent_ProgramEventID", "dbo.ProgramEvent");
            DropIndex("dbo.RoomLog", new[] { "ReleasedEvent_ProgramEventID" });
            DropIndex("dbo.RoomLog", new[] { "AdmittedEvent_ProgramEventID" });
            DropIndex("dbo.RoomLog", new[] { "RoomNumber" });
            DropIndex("dbo.RoomLog", new[] { "ResidentID" });
            DropTable("dbo.RoomLog");
        }
    }
}
