namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgeAtRelease : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "AgeAtRelease", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "AgeAtRelease");
        }
    }
}
