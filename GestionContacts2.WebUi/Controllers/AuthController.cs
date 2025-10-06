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
using System.Net;
using System.Net.Mail;

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

            // Récupère l'utilisateur par email
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Utilisateur introuvable.");
                return View(model);
            }

            // Utilise le UserName pour la connexion
            var result = await SignInManager.PasswordSignInAsync(
                user.UserName, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    if (await UserManager.IsInRoleAsync(user.Id, "Admin"))
                    {
                        return RedirectToAction("TousLesContacts", "Contact");
                    }
                    else
                    {
                        return RedirectToAction("MesContacts", "Contact");
                    }
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
        public ActionResult ResetPassword(string code,string email)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            // ici on crée un modèle pour passer l'email et le code à la vue au lieu de retourner application user directement
            //Car cela va generer une erreur 
            var model = new ResetPasswordViewModel
            {
                Email = email,
                Code = code
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPasswordPost(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Retourne la vue avec le modèle pour afficher les messages d’erreur
                return View("ResetPassword",model);
            }

            model.Code = HttpUtility.UrlDecode(model.Code);
            var user = UserManager.FindByEmail(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Utilisateur introuvable.");
                return View("ResetPassword",model);
            }

            // Utilise le token reçu (model.Code) et le nouveau mot de passe (model.Password)
            var result = UserManager.ResetPassword(user.Id, HttpUtility.UrlDecode(model.Code), model.Password);

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
                return View("ResetPassword",model);
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
                // Construire le lien de réinitialisation correct
                var callbackUrl = Url.Action(
                    "ResetPassword",      // action
                    "Auth",               // controller
                    new { code = HttpUtility.UrlEncode(code), email = user.Email },// paramètres
                    protocol: Request.Url.Scheme              // http ou https automatiquement
                );
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


        public ActionResult TestEmail()
        {
            try
            {
                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("kabeyadaniel27@gmail.com", "hruxovonrmxumgee"),
                    EnableSsl = true
                };
                smtp.Send("kabeyadaniel27@gmail.com", "danielkabeya243@outlook.com", "Test SMTP", "Ceci est un test.");
                return Content("Email envoyé avec succès !");
            }
            catch (Exception ex)
            {
                return Content("Erreur lors de l'envoi : " + ex.Message);
            }
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}