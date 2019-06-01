namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_room_support : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Person", "RoomNumber", "dbo.Room");
            DropForeignKey("dbo.RoomLog", "Event_ProgramEventID", "dbo.ProgramEvent");
            DropForeignKey("dbo.RoomLog", "ResidentID", "dbo.Person");
            DropForeignKey("dbo.RoomLog", "RoomNumber", "dbo.Room");
            DropIndex("dbo.Person", new[] { "RoomNumber" });
            DropIndex("dbo.Room", "RoomNumber");
            DropIndex("dbo.RoomLog", new[] { "ResidentID" });
            DropIndex("dbo.RoomLog", new[] { "RoomNumber" });
            DropIndex("dbo.RoomLog", new[] { "Event_ProgramEventID" });
            DropColumn("dbo.Person", "RoomNumber");
            DropTable("dbo.Room");
            DropTable("dbo.RoomLog");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RoomLog",
                c => new
                    {
                        RoomLogID = c.Int(nullable: false, identity: true),
                        ResidentID = c.Int(nullable: false),
                        RoomNumber = c.Int(),
                        Comment = c.String(),
                        Event_ProgramEventID = c.Int(),
                    })
                .PrimaryKey(t => t.RoomLogID);
            
            CreateTable(
                "dbo.Room",
                c => new
                    {
                        RoomNumber = c.Int(nullable: false),
                        IsOccupied = c.Boolean(nullable: false),
                        WingName = c.String(),
                        LastResident = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.RoomNumber);
            
            AddColumn("dbo.Person", "RoomNumber", c => c.Int());
            CreateIndex("dbo.RoomLog", "Event_ProgramEventID");
            CreateIndex("dbo.RoomLog", "RoomNumber");
            CreateIndex("dbo.RoomLog", "ResidentID");
            CreateIndex("dbo.Room", "RoomNumber", name: "RoomNumber");
            CreateIndex("dbo.Person", "RoomNumber");
            AddForeignKey("dbo.RoomLog", "RoomNumber", "dbo.Room", "RoomNumber");
            AddForeignKey("dbo.RoomLog", "ResidentID", "dbo.Person", "ResidentID", cascadeDelete: true);
            AddForeignKey("dbo.RoomLog", "Event_ProgramEventID", "dbo.ProgramEvent", "ProgramEventID");
            AddForeignKey("dbo.Person", "RoomNumber", "dbo.Room", "RoomNumber");
        }
    }
}
