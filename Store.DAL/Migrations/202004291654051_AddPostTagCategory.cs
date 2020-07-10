namespace Store.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPostTagCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 500),
                        UrlSlug = c.String(nullable: false, maxLength: 500),
                        Description = c.String(),
                        BoolArticle = c.Boolean(nullable: false),
                        Level = c.Int(nullable: false),
                        FullUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 500),
                        ShortDescription = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Meta = c.String(nullable: false, maxLength: 1000),
                        UrlSlug = c.String(nullable: false, maxLength: 1000),
                        Published = c.Boolean(nullable: false),
                        PostedOn = c.DateTime(nullable: false),
                        Modified = c.DateTime(),
                        Topic = c.String(nullable: false),
                        Subtopic = c.String(),
                        Theme = c.String(),
                        Subtheme = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 500),
                        UrlSlug = c.String(nullable: false, maxLength: 500),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TagPost",
                c => new
                    {
                        TagId = c.Int(nullable: false),
                        PostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TagId, t.PostId })
                .ForeignKey("dbo.Tags", t => t.TagId, cascadeDelete: true)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.TagId)
                .Index(t => t.PostId);
            
            CreateTable(
                "dbo.CategoryPost",
                c => new
                    {
                        CategoryId = c.Int(nullable: false),
                        PostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CategoryId, t.PostId })
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.PostId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CategoryPost", "PostId", "dbo.Posts");
            DropForeignKey("dbo.CategoryPost", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.TagPost", "PostId", "dbo.Posts");
            DropForeignKey("dbo.TagPost", "TagId", "dbo.Tags");
            DropIndex("dbo.CategoryPost", new[] { "PostId" });
            DropIndex("dbo.CategoryPost", new[] { "CategoryId" });
            DropIndex("dbo.TagPost", new[] { "PostId" });
            DropIndex("dbo.TagPost", new[] { "TagId" });
            DropTable("dbo.CategoryPost");
            DropTable("dbo.TagPost");
            DropTable("dbo.Tags");
            DropTable("dbo.Posts");
            DropTable("dbo.Categories");
        }
    }
}
