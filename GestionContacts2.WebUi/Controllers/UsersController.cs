using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionContacts2.Data;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using GestionContacts2.WebUi.Models;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;


namespace GestionContacts2.WebUi.Controllers
{
    public class UsersController : Controller
    {
        // La création de l'objet SignInManager est pour gérer l’authentification (connexion, déconnexion, etc.)
        private ApplicationSignInManager _signInManager;
        // La création de l'objet usermanager est pour gerer l'authentification  
        private ApplicationUserManager _userManager;

        // La creation d'un contructeur vide est requise par mvc voila pourquoi je l'ai mis 
        public UsersController()
        {
        }
        // Le deuxième contructeur prend en charge deux paramètres pour permettre l'injection de dépendances 

        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        //Si _signInManager est null, on va le récupérer depuis OWIN (le système de gestion d’authentification de ASP.NET).

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        //Quand l’utilisateur veut accéder à la page de connexion, on lui affiche la vue
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //Premierement on verifie que le formulaire correspond aux normes etablies par mvc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //Ici on essaie de connecter l'utilisateur par son email , mot de passe, se souvenir de moi , shouldLockout: false : on ne verrouille pas le compte après plusieurs échecs.

            var result = await SignInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, shouldLockout: false);

           // Si la connexion est réussie, on redirige vers la liste des contacts.
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("TousLesContacts", "Contact");

                // Si la connexion échoue, on affiche un message d’erreur. 
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Tentative de connexion invalide.");
                    return View(model);
            }
        }

        //Déconnexion de l’utilisateur. On supprime le cookie de session, puis on redirige vers la page de login.
        public ActionResult Logout()
        {

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login");
        }

       // Permet de gérer la déconnexion(SignOut), et autres choses liées à l’authentification via OWIN.
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
    }
}