namespace FIVESTARVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActualDaysInCenter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "ActualDaysStayed", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "ActualDaysStayed");
        }
    }
}
