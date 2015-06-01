﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using SkeletorDAL;
using SkeletorDAL.Model;
using System.IO;

namespace SkeletorHorseProject.Controllers
{
    public class HorseProfileController : Controller
    {
        // GET: HorseProfile
        public ActionResult Index(int id)
        {
            var idLength = id.ToString();
            var model = Repository.GetSpecificHorse(id);
            if (model != null)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }

        public ActionResult UploadProfilePicture(int id)
        {
            return View(id);
        }

        [HttpPost]
        public ActionResult UploadProfilePicture(HttpPostedFileBase file, int id)
        {

            string imageFileName = "";

            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    fileName = id + fileName;

                    if (fileName.EndsWith(".jpg") ||
                        fileName.EndsWith(".png") ||
                        fileName.EndsWith(".bmp") ||
                        fileName.EndsWith(".gif") ||
                        fileName.EndsWith(".jpeg"))
                    {

                        RemoveOldImage(id);

                        var path = Path.Combine(Server.MapPath("~/ProfileImages"), fileName);
                        file.SaveAs(path);
                        Repository.AddNewFile(fileName, path);
                        imageFileName = fileName;
                    }
                    else
                    {
                        ViewBag.Message = "Incorrect file type, please only upload jpg, jpeg, bmp, png or gif";
                        return RedirectToAction("UploadProfilePicture", id);
                    }


                }
                ViewBag.Message = "Upload successful";

                Repository.AddNewFilePathToHorse(imageFileName, id);


                return RedirectToAction("Index", new { id = id });
            }
            catch
            {
                ViewBag.Message = "Upload Failed";
                return RedirectToAction("UploadProfilePicture", id);
            }
        }

        private void RemoveOldImage(int id)
        {
            var path = Repository.RemoveOldProfileImage(id);

            string fullPath = Request.MapPath(path);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }


        [ChildActionOnly]
        public ActionResult HorseBlog(int id)
        {
            var facebookUrl = Repository.GetFacebookPath(id);
            var horseModel = new HorseModel()
            {
                FacebookPath = facebookUrl
            };
            return PartialView(horseModel);
        }


        [ChildActionOnly]
        public ActionResult AddBlogPost(BlogModel blog)
        {
          
            var blogpost = new BlogPostModel(){BlogID = blog.ID};           
            return PartialView("_AddBlogPost", blogpost);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddBlogPostToBlog(BlogPostModel blogpost, int blogID)
        {
            if (!ModelState.IsValid) return View("_AddBlogPost", blogpost);
            var horseIdThatRecivedTheBlogPost = Repository.AddNewBlogPost(blogpost, blogID);
            return RedirectToAction("Index", new { id = horseIdThatRecivedTheBlogPost });
        }

        [ChildActionOnly]
        public ActionResult Gallery(int id)
        {
            List<HorseProfileGalleryImagesModel> images = Repository.GetHorseProfileGalleryImages(id);
            return PartialView("_horseProfileGallery", images);
        }

        public ActionResult UploadGalleryImage(int id)
        {
            return View(id);
        }

        [HttpPost]
        public ActionResult UploadGalleryImage(HttpPostedFileBase file, int id)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = id + Path.GetFileName(file.FileName);

                    if (fileName.EndsWith(".jpg") ||
                        fileName.EndsWith(".png") ||
                        fileName.EndsWith(".bmp") ||
                        fileName.EndsWith(".gif") ||
                        fileName.EndsWith(".jpeg"))
                    {
                        var path = Path.Combine(Server.MapPath("~/HorseProfileImages"), fileName);

                        file.SaveAs(path);
                        path = "~/HorseProfileImages/" + fileName;
                        Repository.AddNewFile(fileName, path);
                    }
                    else
                    {
                        ViewBag.Message = "Incorrect file type, please only upload jpg, jpeg, bmp, png or gif";
                        return RedirectToAction("UploadGalleryImage");
                    }


                }
                ViewBag.Message = "Upload successful";




                return RedirectToAction("Index", new{id=id});
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("UploadGalleryImage");
            }
        }

        [ChildActionOnly]
        public ActionResult UploadImageButton(int id)
        {
            return View(id);
        }
    }
}
