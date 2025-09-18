using GestionContacts2.WebUi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GestionContacts2.Data;
using System.Text;

namespace GestionContacts2.WebUi.Controllers
{
    public class AuthController : Controller
    {


        // La création de l'objet SignInManager est pour gérer l’authentification (connexion, déconnexion, etc.)
        private ApplicationSignInManager _signInManager;
        // La création de l'objet usermanager est pour gerer l'authentification  
        private ApplicationUserManager _userManager;

        //Le premier constructeur est vide car c'est une covention dans asp.net mvc
        public AuthController()
        {
        }
        // Le deuxième contructeur prend en charge deux paramètres pour permettre l'injection de dépendances 

        public AuthController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
                    var user = await UserManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        // ici on verifie si l'utilisateur est dans le rôle "Admin" si oui alors on affiche la liste des tous les contacts
                        // sinon on affiche la liste des contacts de l'utilisateur connecté

                        if (await UserManager.IsInRoleAsync(user.Id, "Admin"))
                        {
                            return RedirectToAction("TousLesContacts", "Contact");
                        }
                        else
                        {
                            return RedirectToAction("MesContacts", "Contact");
                        }

                    }

                    ModelState.AddModelError("", "Utilisateur introuvable.");
                    return View(model);

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

       [HttpGet]
        public async Task<ActionResult> RegisterTest()
        {
            var user =  new GestionContacts2.Data.ApplicationUser
            {
                UserName = "ethandibula@example.com",
                Email = "ethandibula@example.com",
                Name = "Dibula",
                FirstName = "Ethan",
                PhoneNumber = "6137569133",
                BirthDate = new DateTime(2006, 8, 12),
                RegistrationDate = DateTime.Now


            };

            try
            {

            

            var result = await UserManager.CreateAsync(user, "Ethan123#");

                if (result.Succeeded)
                {

                    // Ajout de l'utilisateur au rôle "Utilisateur"
                    await UserManager.AddToRoleAsync(user.Id, "Admin ");
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
        public async Task<ActionResult> LoginTest()
        {
            try
            {
                string email = "newuser@example.com";
                string password = "P@ssword123!";

                // Trouver l'utilisateur
                var user = await UserManager.FindByEmailAsync(email);

                if (user != null)
                {
                    // Vérifier le mot de passe
                    var result = await SignInManager.PasswordSignInAsync(
                        user.UserName, password, isPersistent: false, shouldLockout: false);

                    if (result == SignInStatus.Success)
                    {
                        return Content("Connexion réussie !");
                    }
                    else if (result == SignInStatus.Failure)
                    {
                        return Content("Échec de connexion : Mauvais identifiants.");
                    }
                    else
                    {
                        return Content("Échec de connexion : " + result.ToString());
                    }
                }
                else
                {
                    return Content("Utilisateur non trouvé.");
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

        // Permet de gérer la déconnexion(SignOut), et autres choses liées à l’authentification via OWIN.
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        // GET: Auth
        [HttpGet]
        public ActionResult ResetPassword(string email)
        {
            using (var context = new AppDbContext())
            {
                // Récupère le contact à modifier par son ID
                var user = context.Users.FirstOrDefault(u => u.Email == email);

                // Si le contact n'est pas trouvé, on peut rediriger vers une page d'erreur ou la liste des contacts
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                // Retourne la vue avec le contact pour afficher les informations actuelles
                return View(user);


            }

            
        }

        [HttpPost]
      
        public ActionResult ResetPasswordPost(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Retourne la vue avec le modèle pour afficher les messages d’erreur
                return View(model);
            }
            var user = UserManager.FindByEmail(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Utilisateur introuvable.");
                return View(model);
            }

            // Utilise le token reçu (model.Code) et le nouveau mot de passe (model.Password)
            var result = UserManager.ResetPassword(user.Id, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(model);
            }


        }

        //Cette action affiche la vue pour que l’utilisateur puisse entrer son email afin de recevoir les instructions pour réinitialiser son mot de passe.
        // L'utilisateur entre son email, et on lui envoie un email avec un lien pour réinitialiser son mot de passe.
        //Si le courriel n'existe pas dans la base de données, on ne révèle pas cette information pour des raisons de sécurité.
        //Si le courriel existe, on génère un token de réinitialisation et on envoie un email avec un lien contenant ce token.

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Ne pas révéler que l'utilisateur n'existe pas ou n'est pas confirmé
                    return View("ForgotPasswordConfirmation");
                }
                // Générer le token de réinitialisation du mot de passe
                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Auth", new { code, email = user.Email }, protocol: Request.Url.Scheme);
                // Envoyer l'email avec le lien de réinitialisation
                await UserManager.SendEmailAsync(user.Id, "Réinitialiser le mot de passe",
                   $"Veuillez réinitialiser votre mot de passe en cliquant <a href=\"{callbackUrl}\">ici</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Auth");
            }
            // Si on arrive là, c'est qu'il y a une erreur dans le modèle
            return View(model);
        }

        //Montrer une confirmation que les instructions de réinitialisation du mot de passe ont été envoyées.
        [HttpGet]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }



        public ActionResult Index()
        {
            return View();
        }
    }
}