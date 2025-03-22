using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionContacts2.Data
{
    // heritage qui permet d’utiliser les tables d’ASP.NET Identity(AspNetUsers, AspNetRoles, etc.).
    internal class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        // Constructeur pour appeler la connexion
        public ApplicationDbContext()
        : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        // Méthode pour créer une instance de DbContext
        public DbSet<Contact> Contacts { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
