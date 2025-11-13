using GestionContacts2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionContacts2.WebUi.Models
{
    public class UtilisateurAvecRolesViewModel
    {
        public ApplicationUser Utilisateur { get; set; }
        public IList<string> Roles { get; set; }
    }
}