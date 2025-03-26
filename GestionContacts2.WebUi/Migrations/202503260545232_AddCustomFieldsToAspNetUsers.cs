
namespace GestionContacts2.WebUi.Migrations
{
   
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomFieldsToAspNetUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Name", c => c.String()); // Anciennement "Nom"
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String()); // Anciennement "Prenom"
            AddColumn("dbo.AspNetUsers", "RegistrationDate", c => c.DateTime(nullable: false, defaultValue: DateTime.Now)); // Anciennement "DateInscription"
            AddColumn("dbo.AspNetUsers", "BirthDate", c => c.DateTime(nullable: false, defaultValue: DateTime.Now)); // Anciennement "DateNaissance"
            AddColumn("dbo.AspNetUsers", "Role", c => c.String()); // Anciennement "RoleU"
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Name");
            DropColumn("dbo.AspNetUsers", "FirstName");
            DropColumn("dbo.AspNetUsers", "RegistrationDate");
            DropColumn("dbo.AspNetUsers", "BirthDate");
            DropColumn("dbo.AspNetUsers", "Role");

        }
    }
}
