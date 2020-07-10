namespace Store.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVisits : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "NumberVisits", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "NumberVisits");
        }
    }
}
