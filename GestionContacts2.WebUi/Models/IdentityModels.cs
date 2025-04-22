using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using GestionContacts2.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GestionContacts2.WebUi.Models
{
    // =======================================
    // CLASSE ApplicationUser
    // =======================================
    //
    // Cette classe ApplicationUser hérite de IdentityUser pour ajouter des propriétés personnalisées.
    // Par défaut, Identity crée seulement Id, UserName, Email, etc. dans la table AspNetUsers.
    // Toutes les nouvelles propriétés (Nom, Prenom, DateNaissance, etc.) doivent être ajoutées ici
    // et une migration doit être créée pour les ajouter physiquement dans la base SQL.
    // Cette classe étend IdentityUser pour ajouter
    // des propriétés personnalisées à l'utilisateur.
    //
    // Important :
    //
    // - Seules les propriétés déclarées ici seront mappées
    //   automatiquement avec la table AspNetUsers.
    //
    // - Les propriétés doivent exister dans la base SQL,
    //   sinon des erreurs de synchronisation peuvent apparaître.
    //
    // - IdentityUser contient déjà plusieurs champs de base
    //   comme Email, UserName, PasswordHash, etc.
    //
    // - Si une propriété existe dans la classe mais PAS dans la base,
    //   utiliser l'attribut [NotMapped] pour l'ignorer.
    //
    // - Si une colonne SQL existe mais n'est pas déclarée ici,
    //   elle sera ignorée sauf si elle est obligatoire (NOT NULL).
    //
    // Rappel :
    // Ajouter uniquement les champs nécessaires pour l'application,
    // pas besoin de tout recréer.
    //
    // Exemple : Name, FirstName, BirthDate, etc.
    //
    // =======================================

    // Vous pouvez ajouter des données de profil pour l'utilisateur en ajoutant d'autres propriétés à votre classe ApplicationUser. Pour en savoir plus, consultez https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }       // 👈 pour Name
        public string FirstName { get; set; }   // 👈 pour FirstName
        public DateTime? BirthDate { get; set; } // 👈 si tu as DateOfBirth aussi
        public DateTime RegistrationDate { get; set; }


        //Lien avec la table contacts
        public virtual ICollection<Contact> Contacts { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Notez que l'authenticationType doit correspondre à celui défini dans CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Ajouter des revendications utilisateur personnalisées ici
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}