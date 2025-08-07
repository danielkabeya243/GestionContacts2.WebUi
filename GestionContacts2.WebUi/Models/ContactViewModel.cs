using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionContacts2.WebUi.Models
{

    // Un ViewModel ne doit contenir que les propriétés nécessaires à la vue qui l’utilise.
    // Cette Classe sert à afficher la liste des contacts de l’utilisateur connecté, en ne transmettant à la vue que les informations nécessaires à l’affichage.
   // Il ne s’agit pas du détail d’un contact, mais bien d’un modèle simplifié pour la liste.


    public class ContactViewModel
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
    }
}