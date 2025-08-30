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

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ActionResult TousLesUtilisateurs()
        {
            var listUtilisateurs = _context.Users.ToList();

            //On retourne la vue pour la liste des contacts
            return View(listUtilisateurs);
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
                    UserName = model.Email,
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








    }
}