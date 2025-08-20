namespace GestionContacts2.WebUi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjoutProprieté : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Contacts", "UserId", "dbo.DataApplicationUsers");
            DropIndex("dbo.Contacts", new[] { "UserId" });
            AlterColumn("dbo.Contacts", "Nom", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Contacts", "Prenom", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Contacts", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Contacts", "NumeroTel", c => c.String(maxLength: 20));
            AlterColumn("dbo.Contacts", "Adresse", c => c.String(maxLength: 100));
            AlterColumn("dbo.Contacts", "Entreprise", c => c.String(maxLength: 100));
            AlterColumn("dbo.Contacts", "NotesPersonnelles", c => c.String(maxLength: 250));
            AlterColumn("dbo.Contacts", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Contacts", "UserId");
            AddForeignKey("dbo.Contacts", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contacts", "UserId", "dbo.DataApplicationUsers");
            DropIndex("dbo.Contacts", new[] { "UserId" });
            AlterColumn("dbo.Contacts", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Contacts", "NotesPersonnelles", c => c.String());
            AlterColumn("dbo.Contacts", "Entreprise", c => c.String());
            AlterColumn("dbo.Contacts", "Adresse", c => c.String());
            AlterColumn("dbo.Contacts", "NumeroTel", c => c.String());
            AlterColumn("dbo.Contacts", "Email", c => c.String());
            AlterColumn("dbo.Contacts", "Prenom", c => c.String());
            AlterColumn("dbo.Contacts", "Nom", c => c.String());
            CreateIndex("dbo.Contacts", "UserId");
            AddForeignKey("dbo.Contacts", "UserId", "dbo.DataApplicationUsers", "Id");
        }
    }
}
