using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionContacts2.Data;
using GestionContacts2.WebUi.Models;

namespace GestionContacts2.WebUi.Controllers
{
    public class ContactController : Controller
    {

        private readonly AppDbContext _context;

        // Initialisation du contexte dans le constructeur
        public ContactController()
        {
            _context = new AppDbContext();
        }
        // GET: Contact
        public ActionResult TousLesContacts()
        {
            var listContacts = _context.Contacts.ToList();

            //On retourne la vue pour la liste des contacts
            return View(listContacts);
        }

        //Methode pour recuperer les informations d'un contact alors on utilise le nom du contact pour afficher les informations relatives à un contact
        public ActionResult Details(string nomContact)
        {
            var vm = new DetailsContactViewModel();

            using (var context = new AppDbContext())
            {
                // ici on retourne un contact au lieu d une liste des contacts et on utilise FirstOrDefault car il retournera null si il ne trouve pas la valeur 
                var contactEntity = context.Contacts.Where(c=>c.Nom == nomContact).FirstOrDefault();

                //on doit verifier si il est null ou pas et si il est null on retourne le controleur accueil 

                if (contactEntity == null)
                {
                    return RedirectToAction("Accueil", "Home");
                }

                vm.Nom = contactEntity.Nom;
                vm.Prenom = contactEntity.Prenom;
                vm.Email = contactEntity.Email;
                vm.NumeroTel = contactEntity.NumeroTel;
                vm.Adresse = contactEntity.Adresse;
                vm.Entreprise = contactEntity.Entreprise;
                vm.NotesPersonnelles = contactEntity.NotesPersonnelles;
                vm.DateCreation = contactEntity.DateCreation;
                vm.DateModification = contactEntity.DateModification;


            }

            return View(vm);


        }

        [HttpGet]
        public ActionResult Edit(int id) 
        {

            using (var context = new AppDbContext())
            {
                // Récupère le contact à modifier par son ID
                var contact = context.Contacts.FirstOrDefault(c => c.ContactId == id);

                // Si le contact n'est pas trouvé, on peut rediriger vers une page d'erreur ou la liste des contacts
                if (contact == null)
                {
                    return RedirectToAction("TousLesContacts");
                }

                // Retourne la vue avec le contact pour afficher les informations actuelles
                return View(contact);


            }

        }

        [HttpPost]

        public ActionResult Edit(Contact contactModifie)
        {
            if (!ModelState.IsValid)
            {
                // Retourne la vue avec le modèle pour afficher les messages d’erreur
                return View(contactModifie);
            }

            using (var context = new AppDbContext())
            {
                // Récupère le contact existant dans la base de données
                var contactExistant = context.Contacts.FirstOrDefault(c => c.ContactId == contactModifie.ContactId);

                if (contactExistant == null)
                {
                    // Redirige si le contact n'est pas trouvé
                    return RedirectToAction("TousLesContacts");
                }

                // Met à jour les propriétés
                contactExistant.Nom = contactModifie.Nom;
                contactExistant.Prenom = contactModifie.Prenom;
                contactExistant.Email = contactModifie.Email;
                contactExistant.NumeroTel = contactModifie.NumeroTel;
                contactExistant.Adresse = contactModifie.Adresse;
                contactExistant.Entreprise = contactModifie.Entreprise;
                contactExistant.NotesPersonnelles = contactModifie.NotesPersonnelles;
                contactExistant.DateModification = DateTime.Now; // Met à jour la date de modification

                // Sauvegarde les modifications
                context.SaveChanges();

                // Redirige vers la liste des contacts
                return RedirectToAction("TousLesContacts");
            }
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (var context = new AppDbContext())
            {
                var contact = context.Contacts.FirstOrDefault(c => c.ContactId == id);
                if (contact == null)
                {
                    return RedirectToAction("TousLesContacts");
                }
                return View(contact); // Affiche la vue de confirmation
            }
        }
        [HttpPost]
        public ActionResult DeleteContactConfirmed(int ContactId)
        {

            using (var context = new AppDbContext())
            {
                // ici on retourne un contact au lieu d une liste des contacts et on utilise FirstOrDefault car il retournera null si il ne trouve pas la valeur 
                var contact = context.Contacts.FirstOrDefault(c => c.ContactId == ContactId);

                //on doit verifier si il est null ou pas et si il est null on retourne le controleur accueil 

                if (contact != null)
                {
                    context.Contacts.Remove(contact);
                    context.SaveChanges();
                }


                return RedirectToAction("TousLesContacts");

            }



        }

        public ActionResult RechercherContact(string recherche)
        {
            using (var context = new AppDbContext())
            {

                List<Contact> resultats;
                if (string.IsNullOrEmpty(recherche))
                {
                    resultats = new List<Contact>();
                }
                else
                {
                    resultats = context.Contacts
                   .Where(c => c.Nom.Contains(recherche) || c.Prenom.Contains(recherche) || c.Email.Contains(recherche))
                   .ToList();

                }
                //Ici on verifie si la valeur de recherche correspond au nom , au prenom ou a l'email si cela correspond on retourne la valeur sous forme de liste


                return View(resultats);
            }




        }
    }
}