using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionContacts2.Data
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string NumeroTel { get; set; }
        public string Adresse { get; set; }
        public string Entreprise { get; set; }
        public string NotesPersonnelles { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateModification { get; set; }
        public string UserId { get; set; } // Clé étrangère vers ApplicationUser
        public DataApplicationUser User { get; set; } // Relation avec ApplicationUser
    }
}
