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
    public class AppDbContext: IdentityDbContext<DataApplicationUser>
    {
        // Constructeur pour appeler la connexion
        public AppDbContext()
        : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        // Méthode pour créer une instance de DbContext
        public DbSet<Contact> Contacts { get; set; }
        //Ici on cree la table asp net users
        public static AppDbContext Create()
        {
            return new AppDbContext();
        }
    }
}
