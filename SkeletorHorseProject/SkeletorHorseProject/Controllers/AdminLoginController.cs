﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SkeletorDAL;
using SkeletorHorseProject.Helpers;
using SkeletorHorseProject.Models;

namespace SkeletorHorseProject.Controllers
{
    public class AdminLoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AdminLoginModel model)
        {

            if (ModelState.IsValid)
            {
                bool isValid = Repository.AuthenticateAdminLogin(model.Username,model.Password.SuperHash());
                if (isValid)
                {
                    FormsAuthentication.SetAuthCookie(model.Username,false);
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    return View();                    
                }
                }
            else
            {
                return View(model);
            }
        }
    }
}