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
        // La création de l'objet usermanager est pour gerer toutes les operations qui ont rapport avec les utilisateurs 
        private ApplicationUserManager _userManager;


        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
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
                var user = new Models.ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    FirstName = model.FirstName,
                    PhoneNumber = model.PhoneNumber,
                    BirthDate = model.BirthDate,
                    RegistrationDate = DateTime.Now
                };

                //Iic on cree un utilisateur 
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Ajout de l'utilisateur au rôle "Utilisateur"
                    await UserManager.AddToRoleAsync(user.Id, "Utilisateur");

                    return RedirectToAction("Index"); // Tous les utilisateurs
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return View(model);
        }









    }
}