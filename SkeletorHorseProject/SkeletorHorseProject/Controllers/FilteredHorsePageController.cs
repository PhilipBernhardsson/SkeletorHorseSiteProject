﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SkeletorDAL;

namespace SkeletorHorseProject.Controllers
{
    public class FilteredHorsePageController : Controller
    {
        // GET: FilteredHorsePage
        public ActionResult Index(int navigationId)
        {
            try
            {
                var model = Repository.GetHorsesDependingOnNavigation(navigationId);
                return View(model);
            }
            catch (Exception)
            {

                return View();
            }

        }
    }
}