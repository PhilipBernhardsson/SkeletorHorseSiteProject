﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SkeletorDAL;
using SkeletorDAL.Model;

namespace SkeletorHorseProject.Controllers
{
    public class EditHorseProfileController : Controller
    {
        // GET: EditHorseProfile
        public ActionResult EditHorseProfile(int id)
        {
	        var model = new EditHorseProfileModel();
	        var currentHorse = Repository.GetFullInformationOnSpecificHorseById(id);
	        model.ID = id;
	        model.Name = currentHorse.Name;
	        model.Birthday = currentHorse.Birthday;
	        model.Race = currentHorse.Race;
	        model.Withers = currentHorse.Withers;
	        model.Awards = currentHorse.Awards;
	        model.Description = currentHorse.Description;
	        model.Medicine = currentHorse.Medicine;
	        model.FamilyTree = currentHorse.FamilyTree;
	        model.IsForSale = currentHorse.IsForSale;
	        model.Price = currentHorse.Price;
	        model.ImagePath = currentHorse.ImagePath;
            return View(model);
        }
		[HttpPost]
	    public ActionResult EditHorseProfile(int id, EditHorseProfileModel model)
	    {
		    var currentHorse = Repository.GetFullInformationOnSpecificHorseById(id);
		    if (ModelState.IsValid)
		    {
			    currentHorse.ID = id;
				currentHorse.Name = model.Name;
				currentHorse.Birthday = model.Birthday;
				currentHorse.Race = model.Race;
				currentHorse.Withers = model.Withers;
				currentHorse.Awards = model.Awards;
				currentHorse.Description = model.Description;
				currentHorse.Medicine = model.Medicine;
				currentHorse.FamilyTree = model.FamilyTree;
				currentHorse.IsForSale = model.IsForSale;
				currentHorse.Price = model.Price;
				currentHorse.ImagePath = model.ImagePath;
				Repository.UpdateHorseProfile(currentHorse);
			    return RedirectToAction("Index", "HorseProfile", new { id = id});
		    }
		    return View(model);
	    }
    }
}