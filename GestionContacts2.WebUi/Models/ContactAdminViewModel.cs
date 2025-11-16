using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GestionContacts2.WebUi.Models
{
    public class ContactAdminViewModel
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        // Informations sur l'utilisateur associé au contact

        public string Adresse { get; set; }
       
        public string Entreprise { get; set; }
       
        public string NotesPersonnelles { get; set; }
        public DateTime DateCreation { get; set; }
        public string UtilisateurEmail { get; set; }
        public string UtilisateurNom { get; set; }
    }
}