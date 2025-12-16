using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionContacts2.Data;
using GestionContacts2.WebUi.Models;
using Microsoft.AspNet.Identity;

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
        //ici on fait une jointure entre la table contacts et users pour recuperer les informations de l'utilisateur qui a cree le contact
        public ActionResult TousLesContacts(string recherche)
        {


            if (Request.HttpMethod == "GET" && Request.QueryString.AllKeys.Contains("recherche") && string.IsNullOrWhiteSpace(recherche))
            {
                ViewBag.MessageErreur = "Le champ recherche ne peut pas être vide.";
            }

            // On filtre d'abord les contacts selon la recherche
            var contactsFiltres = string.IsNullOrEmpty(recherche)
                ? _context.Contacts.ToList()
                : _context.Contacts
                    .Where(u => u.Prenom.Contains(recherche) || u.Nom.Contains(recherche) || u.Email.Contains(recherche))
                    .ToList();

            // On fait la jointure uniquement sur les contacts filtrés
            var model = contactsFiltres
                .Join(_context.Users, c => c.UserId, u => u.Id, (c, u) => new ContactAdminViewModel
                {
                    Id = c.ContactId,
                    Nom = c.Nom,
                    Prenom = c.Prenom,
                    Email = c.Email,
                    Telephone = c.NumeroTel,
                    Adresse = c.Adresse,
                    Entreprise = c.Entreprise,
                    NotesPersonnelles = c.NotesPersonnelles,
                    UtilisateurEmail = u.Email,
                    UtilisateurNom = u.Name
                }).ToList();

            return View(model);
        }

        [Authorize]
        //Cette methode retourne la liste des contacts de l'utilisateur connecté elle retourne dans un view model 
        //contactsViewModel qui contient les informations des contacts de l'utilisateur connecté
        public ActionResult MesContacts()
        {
            var userId = User.Identity.GetUserId();
            var contacts = _context.Contacts
                .Where(c => c.UserId == userId)
                .Select(c => new ContactViewModel
                {
                    Id = c.ContactId,
                    Nom = c.Nom,
                    Prenom = c.Prenom,
                    Email = c.Email,
                    Telephone = c.NumeroTel
                    // Ajoute d'autres propriétés si besoin
                })
                .ToList();

            return View("MesContacts", contacts);
        }

        //Methode pour recuperer les informations d'un contact alors on utilise l'id du contact pour afficher les informations relatives à un contact
        //Et on utilise l'id pour l'unicité comme ça on est sure que l'on va retourner un seul contact  alors qu'avec le nom du contact
        /// <summary>
        /// on peut avoir plusieurs contacts avec le même nom
        /// et c'est aussi par convention que l'on utilise le id car il est plus performant que la recherche par nom qui est lente 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var vm = new DetailsContactViewModel();

            using (var context = new AppDbContext())
            {
                // ici on retourne un contact au lieu d une liste des contacts et on utilise FirstOrDefault car il retournera null si il ne trouve pas la valeur 
                 var contactEntity = context.Contacts.FirstOrDefault(c => c.ContactId == id);

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
                    return RedirectToAction("MesContacts");
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
                    return RedirectToAction("MesContacts");
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
                contactExistant.UserId = contactModifie.UserId;

                // Sauvegarde les modifications
                context.SaveChanges();

                // Redirige vers la liste des contacts
                return RedirectToAction("MesContacts");
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

        //La methode AjouterContact() sans parametre est appelee lorsque l'utilisateur accède pour la premiere fois à la page de formulaire pour ajouter un contact elle affiche un formulaire vide
        [Authorize]
        public ActionResult AjouterContact()
        {
            return View();
        }


        //Par contre cette methode est appelée lorsque l'utilisateur soumet le formulaire. Elle utilise l'objet Contact que l’utilisateur a rempli, et cette version de AjouterContact
        //reçoit les données du formulaire pour effectuer les validations et enregistrer les données en base si elles sont valides.

        [HttpPost]
        [Authorize]
        public ActionResult AjouterContact([Bind(Exclude = "User")] Contact nouveauContact)
        {
            // Récupère l'ID de l'utilisateur connecté
            string userID = User.Identity.GetUserId();
            // Associe l'ID de l'utilisateur connecté au nouveau contact
            nouveauContact.UserId = userID;

            //ici on verifie que toutes les règles de validation ont été respectées
            //si une règle est violée modelstate.isvalid renverra false

            ModelState.Remove("UserId");

            if (!ModelState.IsValid)
            {
                return View(nouveauContact);
            }

            

            using (var context = new AppDbContext())
            {
                // Vérifie si un contact avec le même email existe déjà
                bool contactExists = context.Contacts.Any(c => c.Email == nouveauContact.Email);

                if (contactExists)
                {
                    ModelState.AddModelError("Email", "Un contact avec cet email existe déjà.");
                    return View(nouveauContact);
                }

                // Vérifie que l'utilisateur associé existe
                bool userExists = context.Users.Any(u => u.Id == userID);
                if (!userExists)
                {
                    ModelState.AddModelError("UserId", "L'utilisateur associé est introuvable.");
                    return View(nouveauContact);
                }
                // Ajoute la date de création
                nouveauContact.DateCreation = DateTime.Now;

                
                try
                {
                    // Ajoute le contact à la base de données
                    context.Contacts.Add(nouveauContact);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", "Erreur lors de l'ajout : " + ex.Message);
                    return View(nouveauContact);
                }
               


                TempData["SuccessMessage"] = "Le contact a été ajouté avec succès !";
                // Redirige vers la liste des contacts ou une page de confirmation
                return RedirectToAction("MesContacts");

            }





        }

        public ActionResult RechercherContact(string recherche)
        {
            using (var context = new AppDbContext())
            {

                if (Request.HttpMethod == "GET" && Request.QueryString.AllKeys.Contains("recherche") && string.IsNullOrWhiteSpace(recherche))
                {
                    ViewBag.MessageErreur = "Le champ recherche ne peut pas être vide.";
                }


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