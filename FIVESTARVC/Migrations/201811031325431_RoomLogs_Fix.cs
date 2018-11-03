namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoomLogs_Fix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RoomLog", "ReleasedEvent_ProgramEventID", "dbo.ProgramEvent");
            DropIndex("dbo.RoomLog", new[] { "ReleasedEvent_ProgramEventID" });
            RenameColumn(table: "dbo.RoomLog", name: "AdmittedEvent_ProgramEventID", newName: "Event_ProgramEventID");
            RenameIndex(table: "dbo.RoomLog", name: "IX_AdmittedEvent_ProgramEventID", newName: "IX_Event_ProgramEventID");
            DropColumn("dbo.RoomLog", "ReleasedEvent_ProgramEventID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RoomLog", "ReleasedEvent_ProgramEventID", c => c.Int());
            RenameIndex(table: "dbo.RoomLog", name: "IX_Event_ProgramEventID", newName: "IX_AdmittedEvent_ProgramEventID");
            RenameColumn(table: "dbo.RoomLog", name: "Event_ProgramEventID", newName: "AdmittedEvent_ProgramEventID");
            CreateIndex("dbo.RoomLog", "ReleasedEvent_ProgramEventID");
            AddForeignKey("dbo.RoomLog", "ReleasedEvent_ProgramEventID", "dbo.ProgramEvent", "ProgramEventID");
        }
    }
}
