namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class residentReferral : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Referral",
                c => new
                    {
                        ReferralID = c.Int(nullable: false, identity: true),
                        ReferralName = c.String(maxLength: 200),
                        AdditionalData = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.ReferralID);
            
            AddColumn("dbo.Person", "ReferralID", c => c.Int());
            CreateIndex("dbo.Person", "ReferralID");
            AddForeignKey("dbo.Person", "ReferralID", "dbo.Referral", "ReferralID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Person", "ReferralID", "dbo.Referral");
            DropIndex("dbo.Person", new[] { "ReferralID" });
            DropColumn("dbo.Person", "ReferralID");
            DropTable("dbo.Referral");
        }
    }
}
