namespace ACS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeAgregaPartner : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Partners",
                c => new
                    {
                        PartnerID = c.Int(nullable: false, identity: true),
                        LastName = c.String(),
                        Name = c.String(),
                        DocumentNumber = c.String(),
                        DocumentTypeID = c.Int(nullable: false),
                        Phone = c.String(),
                        Address = c.String(),
                        EMail = c.String(),
                    })
                .PrimaryKey(t => t.PartnerID)
                .ForeignKey("dbo.DocumentTypes", t => t.DocumentTypeID, cascadeDelete: true)
                .Index(t => t.DocumentTypeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Partners", "DocumentTypeID", "dbo.DocumentTypes");
            DropIndex("dbo.Partners", new[] { "DocumentTypeID" });
            DropTable("dbo.Partners");
        }
    }
}
