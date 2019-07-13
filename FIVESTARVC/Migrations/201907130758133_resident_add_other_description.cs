namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resident_add_other_description : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "OptionalReferralDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "OptionalReferralDescription");
        }
    }
}
