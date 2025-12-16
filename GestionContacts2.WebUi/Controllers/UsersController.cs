using GestionContacts2.Data;
using GestionContacts2.WebUi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;



namespace GestionContacts2.WebUi.Controllers
{
    public class UsersController : Controller
    {
        // La création de l'objet SignInManager est pour gérer l’authentification (connexion, déconnexion, etc.)
        private ApplicationSignInManager _signInManager;
        // La création de l'objet usermanager est pour gerer toutes les operations qui ont rapport avec les utilisateurs 
        private ApplicationUserManager _userManager;
        private readonly AppDbContext _context;

        //On intialise le contexte de la base de données dans le contructeur
        public UsersController()
        {
            _context = new AppDbContext();
        }
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        //cette methode retourne la liste de tous les utilisateurs avec leurs roles mais aussi
        //elle permet de faire une recherche par nom , prenom ou email
        public ActionResult TousLesUtilisateurs(string recherche)
        {

            if (Request.HttpMethod == "GET" && Request.QueryString.AllKeys.Contains("recherche") && string.IsNullOrWhiteSpace(recherche))
            {
                ViewBag.MessageErreur = "Le champ recherche ne peut pas être vide.";
            }

            var users = string.IsNullOrEmpty(recherche)
        ? _context.Users.ToList()
        : _context.Users
            .Where(u => u.FirstName.Contains(recherche) || u.Name.Contains(recherche) || u.Email.Contains(recherche))
            .ToList();

            var userManager = UserManager;
            var model = users.Select(u => new UtilisateurAvecRolesViewModel
            {
                Utilisateur = u,
                Roles = userManager.GetRoles(u.Id)
            }).ToList();

            return View(model);

           
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Name = model.Name,
                    FirstName = model.FirstName,
                    PhoneNumber = model.PhoneNumber,
                    BirthDate = model.BirthDate,
                    RegistrationDate = DateTime.Now
                };

                try
                {
                    //Iic on cree un utilisateur 
                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        // Ajout de l'utilisateur au rôle "Utilisateur"
                        await UserManager.AddToRoleAsync(user.Id, "User");

                        return Content("Utilisateur créé avec succès !"); // Tous les utilisateurs
                    }

                    else
                    {
                        return Content("Erreur : " + string.Join(", ", result.Errors));
                    }
                }
                catch (Exception ex)
                {

                    var fullError = new StringBuilder();
                    var currentEx = ex;

                    while (currentEx != null)
                    {
                        fullError.AppendLine(currentEx.Message);
                        currentEx = currentEx.InnerException;
                    }

                    return Content("Erreur : " + fullError.ToString());
                }

               

               
            }
            return View(model);
        }



        public async Task<ActionResult> RegisterTest()
        {
            var user = new GestionContacts2.Data.ApplicationUser
            {
                UserName = "shiradibula@example.com",
                Email = "shiradibula@example.com",
                Name = "Dibula",
                FirstName = "Shira",
                PhoneNumber = "4387569133",
                BirthDate = new DateTime(2003, 8, 22),
                RegistrationDate = DateTime.Now


            };

            try
            {



                var result = await UserManager.CreateAsync(user, "Shira123#");

                if (result.Succeeded)
                {

                    // Ajout de l'utilisateur au rôle "Utilisateur"
                    await UserManager.AddToRoleAsync(user.Id, "User");
                    return Content("Utilisateur créé avec succès !");
                }
                else
                {
                    return Content("Erreur : " + string.Join(", ", result.Errors));
                }
            }
            catch (Exception ex)
            {

                var fullError = new StringBuilder();
                var currentEx = ex;

                while (currentEx != null)
                {
                    fullError.AppendLine(currentEx.Message);
                    currentEx = currentEx.InnerException;
                }

                return Content("Erreur : " + fullError.ToString());
            }
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            using (var context = new AppDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return RedirectToAction("TousLesUtilisateurs");
                }
                return View(user); // Affiche la vue de confirmation
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(string id)
        {

            using (var context = new AppDbContext())
            {
                // ici on retourne un contact au lieu d une liste des contacts et on utilise FirstOrDefault car il retournera null si il ne trouve pas la valeur 
                var user = context.Users.FirstOrDefault(u => u.Id == id);

                //on doit verifier si il est null ou pas et si il est null on retourne le controleur accueil 

                if (user != null)
                {
                    context.Users.Remove(user);
                    context.SaveChanges();
                }


                return RedirectToAction("TousLesUtilisateurs");

            }



        }
        //Dans la methode get on recupere l id de l'utilisateur a modifier et on retourne la vue avec les informations actuelles du contact

        [HttpGet]
        public ActionResult Edit(string id)
        {

            using (var context = new AppDbContext())
            {
                // Récupère le contact à modifier par son ID
                var user = context.Users.FirstOrDefault(u => u.Id == id);

                // Si le contact n'est pas trouvé, on peut rediriger vers une page d'erreur ou la liste des contacts
                if (user == null)
                {
                    return RedirectToAction("TousLesUtilisateurs");
                }

                // Retourne la vue avec le contact pour afficher les informations actuelles
                return View(user);


            }

        }
        //Le formulaire POST transmet les modifications apportées au contact et on met a jour la base de donnee

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApplicationUser userModifie)
        {
            if (!ModelState.IsValid)
            {
                // Retourne la vue avec le modèle pour afficher les messages d’erreur
                return View(userModifie);
            }

            using (var context = new AppDbContext())
            {
                // Récupère l'utilisateur existant dans la base de données
                var userExistant = context.Users.FirstOrDefault(u => u.Id ==userModifie.Id);

                if (userExistant == null)
                {
                    // Redirige si l'utilisateurs n'est pas trouvé
                    return RedirectToAction("TousLesUtilisateurs");
                }

                // Met à jour les propriétés
                userExistant.Name = userModifie.Name;
                userExistant.FirstName = userModifie.FirstName;
                userExistant.UserName = userModifie.UserName;
                userExistant.Email = userModifie.Email;
                userExistant.PhoneNumber = userModifie.PhoneNumber;
                userExistant.BirthDate = userModifie.BirthDate;
             
                // Sauvegarde les modifications
                context.SaveChanges();

                // Redirige vers la liste des utilisateurs
                return RedirectToAction("TousLesUtilisateurs");
            }
        }

        [HttpGet]
        public ActionResult FindUser(string recherche)
        {
            using (var context = new AppDbContext())
            {

                List<ApplicationUser> resultats;
                if (string.IsNullOrEmpty(recherche))
                {
                    resultats = new List<ApplicationUser>();
                }
                else
                {
                    resultats = context.Users
                   .Where(c => c.FirstName.Contains(recherche) || c.Name.Contains(recherche) || c.Email.Contains(recherche))
                   .ToList();

                }
                //Ici on verifie si la valeur de recherche correspond au nom , au prenom ou a l'email si cela correspond on retourne la valeur sous forme de liste


                return View(resultats);
            }


           
        }









    }
}