using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;


namespace GestionContacts2.Data
{
    internal class ApplicationUser:IdentityUser
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public DateTime DateInscription { get; set; }
        public DateTime DateNaissance { get; set; }
        public string RoleU { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }
    }
}
