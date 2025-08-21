namespace GestionContacts2.WebUi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MiseAJourApplicationUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUserClaims", "DataApplicationUser_Id", "dbo.DataApplicationUsers");
            DropForeignKey("dbo.AspNetUserLogins", "DataApplicationUser_Id", "dbo.DataApplicationUsers");
            DropForeignKey("dbo.AspNetUserRoles", "DataApplicationUser_Id", "dbo.DataApplicationUsers");
            DropForeignKey("dbo.Contacts", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUserRoles", new[] { "DataApplicationUser_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "DataApplicationUser_Id" });
            DropIndex("dbo.Contacts", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "DataApplicationUser_Id" });
            AlterColumn("dbo.AspNetUsers", "BirthDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.AspNetUserRoles", "DataApplicationUser_Id");
            DropColumn("dbo.AspNetUserClaims", "DataApplicationUser_Id");
            DropColumn("dbo.Contacts", "ApplicationUser_Id");
            DropColumn("dbo.AspNetUserLogins", "DataApplicationUser_Id");
           // DropTable("dbo.DataApplicationUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DataApplicationUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Nom = c.String(),
                        Prenom = c.String(),
                        DateInscription = c.DateTime(nullable: false),
                        DateNaissance = c.DateTime(nullable: false),
                        RoleU = c.String(),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUserLogins", "DataApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Contacts", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUserClaims", "DataApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUserRoles", "DataApplicationUser_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.AspNetUsers", "BirthDate", c => c.DateTime());
            CreateIndex("dbo.AspNetUserLogins", "DataApplicationUser_Id");
            CreateIndex("dbo.Contacts", "ApplicationUser_Id");
            CreateIndex("dbo.AspNetUserClaims", "DataApplicationUser_Id");
            CreateIndex("dbo.AspNetUserRoles", "DataApplicationUser_Id");
            AddForeignKey("dbo.Contacts", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUserRoles", "DataApplicationUser_Id", "dbo.DataApplicationUsers", "Id");
            AddForeignKey("dbo.AspNetUserLogins", "DataApplicationUser_Id", "dbo.DataApplicationUsers", "Id");
            AddForeignKey("dbo.AspNetUserClaims", "DataApplicationUser_Id", "dbo.DataApplicationUsers", "Id");
        }
    }
}
