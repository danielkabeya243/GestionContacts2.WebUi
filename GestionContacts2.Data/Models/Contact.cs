using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GestionContacts2.Data
{
    public class Contact
    {



        // Clé étrangère (Foreign Key) :
        // Cette propriété contient l'identifiant unique de l'utilisateur auquel ce contact est lié.
        // Le type correspond toujours au type de la clé primaire de la table référencée (ex. int, string).
        // Cette valeur est stockée dans la base de données et sert à faire le lien entre les tables.

        // Propriété de navigation :
        // Cette propriété est un objet complet de type 'User' (ou autre classe liée).
        // Elle permet d'accéder facilement aux données complètes de l'utilisateur lié dans le code C#.
        // Cette propriété n'est pas directement stockée en base, mais utilisée par Entity Framework pour gérer les relations.
        public int ContactId { get; set; }
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le nom ne doit pas dépasser 50 caractères.")]
        public string Nom { get; set; }
        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le prénom ne doit pas dépasser 50 caractères.")]
        public string Prenom { get; set; }
        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
        [StringLength(20, ErrorMessage = "Le numéro de téléphone ne doit pas dépasser 20 caractères.")]
        public string NumeroTel { get; set; }

        [StringLength(100, ErrorMessage = "L'adresse ne doit pas dépasser 100 caractères.")]
        public string Adresse { get; set; }
        [StringLength(100, ErrorMessage = "Le nom de l'entreprise ne doit pas dépasser 100 caractères.")]
        public string Entreprise { get; set; }
        [StringLength(250, ErrorMessage = "Les notes personnelles ne doivent pas dépasser 250 caractères.")]
        public string NotesPersonnelles { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateModification { get; set; }


       //[Required(ErrorMessage = "L'utilisateur associé est obligatoire.")]
        public string UserId { get; set; } // Clé étrangère vers ApplicationUser
      //public virtual ApplicationUser User { get; set; } // Relation avec ApplicationUser
    }
}
