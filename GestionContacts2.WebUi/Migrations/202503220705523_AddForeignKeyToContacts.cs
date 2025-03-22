namespace GestionContacts2.WebUi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeyToContacts : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.Contacts", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contacts", "UserId", "dbo.AspNetUsers");
        }
    }
}
