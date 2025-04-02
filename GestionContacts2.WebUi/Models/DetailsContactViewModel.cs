using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionContacts2.WebUi.Models
{
    public class DetailsContactViewModel
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string NumeroTel { get; set; }
        public string Adresse { get; set; }
        public string Entreprise { get; set; }
        public string NotesPersonnelles { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateModification { get; set; }

    }
}