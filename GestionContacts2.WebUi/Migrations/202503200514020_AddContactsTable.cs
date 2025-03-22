namespace GestionContacts2.WebUi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContactsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contacts",
                c => new
                {
                    ContactId = c.Int(nullable: false, identity: true),
                    Nom = c.String(nullable: false),
                    Prenom = c.String(nullable: false),
                    Email = c.String(nullable: false),
                    NumeroTel = c.String(nullable: false),
                    Adresse = c.String(),
                    Entreprise = c.String(),
                    NotesPersonnelles = c.String(),
                    DateCreation = c.DateTime(nullable: false),
                    DateModification = c.DateTime(),
                    UserId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ContactId)
               ;

        }
        
        public override void Down()
        {
            
            DropTable("dbo.Contacts");
        }
    }
}
