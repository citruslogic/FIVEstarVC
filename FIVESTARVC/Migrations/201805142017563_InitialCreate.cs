namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Benefit",
                c => new
                    {
                        BenefitID = c.Int(nullable: false, identity: true),
                        DisabilityPercentage = c.String(),
                        DisabilityAmount = c.Decimal(storeType: "money"),
                        TotalBenefitAmount = c.Decimal(storeType: "money"),
                        SSI = c.Decimal(storeType: "money"),
                        SSDI = c.Decimal(storeType: "money"),
                        FoodStamp = c.Boolean(nullable: false),
                        OtherDescription = c.String(),
                        Other = c.Decimal(storeType: "money"),
                    })
                .PrimaryKey(t => t.BenefitID);
            
            CreateTable(
                "dbo.MilitaryCampaign",
                c => new
                    {
                        MilitaryCampaignID = c.Int(nullable: false, identity: true),
                        CampaignName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.MilitaryCampaignID);
            
            CreateTable(
                "dbo.Person",
                c => new
                    {
                        ResidentID = c.Int(nullable: false, identity: true),
                        LastName = c.String(),
                        FirstMidName = c.String(),
                        Birthdate = c.String(),
                        Gender = c.Int(nullable: false),
                        Ethnicity = c.Int(nullable: false),
                        Religion = c.Int(nullable: false),
                        StateTerritoryID = c.Int(nullable: false),
                        ServiceBranch = c.Int(),
                        MilitaryDischarge = c.Int(),
                        IsNoncombat = c.Boolean(),
                        InVetCourt = c.Boolean(),
                        RoomNumber = c.Int(),
                        Note = c.String(maxLength: 150),
                        BenefitID = c.Int(),
                    })
                .PrimaryKey(t => t.ResidentID)
                .ForeignKey("dbo.Benefit", t => t.BenefitID)
                .ForeignKey("dbo.Room", t => t.RoomNumber)
                .ForeignKey("dbo.StateTerritory", t => t.StateTerritoryID, cascadeDelete: true)
                .Index(t => t.StateTerritoryID)
                .Index(t => t.RoomNumber)
                .Index(t => t.BenefitID);
            
            CreateTable(
                "dbo.ProgramEvent",
                c => new
                    {
                        ProgramEventID = c.Int(nullable: false, identity: true),
                        ResidentID = c.Int(nullable: false),
                        ProgramTypeID = c.Int(),
                        StartDate = c.String(),
                        EndDate = c.String(),
                        Completed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ProgramEventID)
                .ForeignKey("dbo.ProgramType", t => t.ProgramTypeID)
                .ForeignKey("dbo.Person", t => t.ResidentID, cascadeDelete: true)
                .Index(t => t.ResidentID)
                .Index(t => t.ProgramTypeID);
            
            CreateTable(
                "dbo.ProgramType",
                c => new
                    {
                        ProgramTypeID = c.Int(nullable: false, identity: true),
                        ProgramDescription = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.ProgramTypeID);
            
            CreateTable(
                "dbo.Room",
                c => new
                    {
                        RoomNumber = c.Int(nullable: false),
                        IsOccupied = c.Boolean(nullable: false),
                        WingName = c.String(),
                    })
                .PrimaryKey(t => t.RoomNumber)
                .Index(t => t.RoomNumber, name: "RoomNumber");
            
            CreateTable(
                "dbo.StateTerritory",
                c => new
                    {
                        StateTerritoryID = c.Int(nullable: false, identity: true),
                        State = c.String(),
                        Region = c.String(),
                    })
                .PrimaryKey(t => t.StateTerritoryID);
            
            CreateTable(
                "dbo.CampaignAssignment",
                c => new
                    {
                        MilitaryCampaignID = c.Int(nullable: false),
                        ResidentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MilitaryCampaignID, t.ResidentID })
                .ForeignKey("dbo.MilitaryCampaign", t => t.MilitaryCampaignID, cascadeDelete: true)
                .ForeignKey("dbo.Person", t => t.ResidentID, cascadeDelete: true)
                .Index(t => t.MilitaryCampaignID)
                .Index(t => t.ResidentID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Person", "StateTerritoryID", "dbo.StateTerritory");
            DropForeignKey("dbo.CampaignAssignment", "ResidentID", "dbo.Person");
            DropForeignKey("dbo.CampaignAssignment", "MilitaryCampaignID", "dbo.MilitaryCampaign");
            DropForeignKey("dbo.Person", "RoomNumber", "dbo.Room");
            DropForeignKey("dbo.ProgramEvent", "ResidentID", "dbo.Person");
            DropForeignKey("dbo.ProgramEvent", "ProgramTypeID", "dbo.ProgramType");
            DropForeignKey("dbo.Person", "BenefitID", "dbo.Benefit");
            DropIndex("dbo.CampaignAssignment", new[] { "ResidentID" });
            DropIndex("dbo.CampaignAssignment", new[] { "MilitaryCampaignID" });
            DropIndex("dbo.Room", "RoomNumber");
            DropIndex("dbo.ProgramEvent", new[] { "ProgramTypeID" });
            DropIndex("dbo.ProgramEvent", new[] { "ResidentID" });
            DropIndex("dbo.Person", new[] { "BenefitID" });
            DropIndex("dbo.Person", new[] { "RoomNumber" });
            DropIndex("dbo.Person", new[] { "StateTerritoryID" });
            DropTable("dbo.CampaignAssignment");
            DropTable("dbo.StateTerritory");
            DropTable("dbo.Room");
            DropTable("dbo.ProgramType");
            DropTable("dbo.ProgramEvent");
            DropTable("dbo.Person");
            DropTable("dbo.MilitaryCampaign");
            DropTable("dbo.Benefit");
        }
    }
}
