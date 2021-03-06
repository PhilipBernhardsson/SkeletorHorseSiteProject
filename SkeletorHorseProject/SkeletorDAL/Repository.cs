﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml.Schema;
using SkeletorDAL.Helpers;
using SkeletorDAL.Model;
using SkeletorDAL.POCO;
using SkeletorHorseProject.Controllers;
using SkeletorHorseProject.Models;

namespace SkeletorDAL
{
    public static class Repository
    {
        public static FamilyTreeModel GetFamilyTree(int id)
        {
            using (var context = new HorseContext())
            {
                var horse = (from c in context.Horses
                             where c.ID == id
                             select new FamilyTreeModel()
                             { 
                                 Parents = new ParentModel()
                                 {
                                     FatherName = c.Tree.FatherName,
                                     FatherDescription = c.Tree.FatherDescription,
                                     MotherName = c.Tree.MotherName,
                                     MotherDescription = c.Tree.MotherDescription
                                 },
                                 horseid = id,
                                 HorseName = c.Name
                             }).FirstOrDefault();
                var listOfChildren = (from c in context.Horses
                                      where c.ID == id
                                      select c.Tree.Children).FirstOrDefault();
                horse.Children = listOfChildren.Select(childModel => new ChildModel() { Name = childModel.Name, Description = childModel.Description, horseid = id, HorseName = horse.HorseName }).ToList();

                return horse;
            }
        }

        public static List<ImageModel> GetAllSlideShowImages()
        {
            using (var context = new HorseContext())
            {
                return (from i in context.SlideShowImages
                        where i.Active == true && i.ImagePath.StartsWith(@"~/SlideImages/")
                        select (new ImageModel
                        {
                            ID = i.ID,
                            Active = i.Active,
                            ImagePath = i.ImagePath,
                            FileName = i.FileName
                        })).ToList();
            }
        }

        public static void AddNewSlideShowFile(string fileName, string path)
        {
            using (var context = new HorseContext())
            {
                context.SlideShowImages.Add(new SlideShowImage()
                {
                    FileName = fileName,
                    ImagePath = path,
                    Active = true
                }
                    );
                context.SaveChanges();
            }
        }

        public static void DeleteSlideShowImage(int id)
        {
            using (var context = new HorseContext())
            {
                var image = context.SlideShowImages.Find(id);
                context.SlideShowImages.Remove(image);
                context.SaveChanges();
            }
        }

        public static string GetAdminEmail()
        {
            using (var context = new HorseContext())
            {
                return
                    (from u in context.Users
                     where u.AdminLevel == 1
                     select u.Email).FirstOrDefault();
            }
        }

        public static HorseProfileModel GetHorseProfileById(int id)
        {
            using (var context = new HorseContext())
            {
                var currentHorse = (from h in context.Horses
                                    where h.ID == id
                                    select new HorseProfileModel
                                    {
                                        Awards = h.Awards,
                                        Birthday = h.Birthday,
                                        Breeding = h.Breeding,
                                        Description = h.Description,
                                        FacebookPath = h.FacebookPath,
                                        FamilyTree = h.FamilyTree,
                                        Gender = h.Gender,
                                        IsActive = !h.IsActive,
                                        IsForSale = h.IsForSale,
                                        IsSold = h.IsSold,
                                        Rent = h.Rent,
                                        Price = h.Price,
                                        Medicine = h.Medicine,
                                        Name = h.Name,
                                        Race = h.Race,
                                        Withers = h.Withers,
                                        ImagePath = h.ImagePath

                                    }).FirstOrDefault();

                return currentHorse;
            }
        }

        public static List<FooterModel> GetAllFooterLinks()
        {
            using (var context = new HorseContext())
            {
                return (from l in context.FooterLinks
                        select new FooterModel { ID = l.ID, Name = l.LinkName, Url = l.LinkURL, Column = l.Column }).ToList();
            }
        }

        public static void AddNewFooterLink(FooterModel model)
        {
            using (var context = new HorseContext())
            {
                context.FooterLinks.Add(new FooterLink()
                {
                    LinkName = model.Name,
                    LinkURL = model.Url,
                    Column = model.Column
                });
                context.SaveChanges();
            }
        }

        public static List<HorseVideoModel> GetHorseVideosByID(int id)
        {
            List<HorseVideoModel> listToReturn = new List<HorseVideoModel>();

            using (var context = new HorseContext())
            {
                List<YoutubeVideoURL> videoList = (from v in context.Horses
                                                   where v.ID == id
                                                   select v.YoutubeVideoURLs).FirstOrDefault();

                foreach (var video in videoList)
                {
                    listToReturn.Add(new HorseVideoModel()
                    {
                        ID = video.ID,
                        HorseID = id,
                        VideoName = video.VideoName,
                        VideoURL = video.VideoURL
                    });
                }
            }

            return listToReturn;
        }

        public static void DeleteFooterLink(int id)
        {
            using (var context = new HorseContext())
            {
                var footerToDelete = (from l in context.FooterLinks
                                      where l.ID == id
                                      select l).FirstOrDefault();

                context.FooterLinks.Remove(footerToDelete);
                context.SaveChanges();
            }
        }

        public static void RegisterAdmin(RegisterAdminModel model)
        {
            using (var context = new HorseContext())
            {
                var newAdmin = new User()
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password.SuperHash(),
                    AdminLevel = model.AdminLevel,
                    IsActive = true,
                };

                context.Users.Add(newAdmin);
                context.SaveChanges();
            }
        }

        public static List<Horse> GetAllHorses()
        {
            using (var context = new HorseContext())
            {
                return (from h in context.Horses
                        select h).ToList();
            }
        }

        public static HorseProfileModel GetSpecificHorse(int id)
        {

            using (var context = new HorseContext())
            {
                var admins =
                    (from h in context.Horses
                     where h.ID == id
                     select h.AssignedEditors).FirstOrDefault();

                var adminIds = new List<int>();
                foreach (var admin in admins)
                {
                    adminIds.Add(admin.ID);
                }
                var horse = (from h in context.Horses
                             where h.ID == id
                             select new HorseProfileModel()
                             {
                                 ID = h.ID,
                                 Name = h.Name,
                                 Race = h.Race,
                                 Withers = h.Withers,
                                 Birthday = h.Birthday,
                                 Description = h.Description,
                                 Medicine = h.Medicine,
                                 FamilyTree = h.FamilyTree,
                                 Awards = h.Awards,
                                 ImagePath = h.ImagePath,
                                 Blog = new BlogModel()
                                 {
                                     ID = h.Blog.ID,
                                     HorseId = h.ID,
                                     Created = h.Blog.Created,
                                     Description = h.Blog.Description,
                                 },
                                 IsSold = h.IsSold,
                                 FacebookPath = h.FacebookPath,
                                 IsActive = h.IsActive,
                                 IsForSale = h.IsForSale,
                                 Price = h.Price,
                                 Gender = h.Gender,
                                 IsForRent = h.Rent,
                                 Breeding = h.Breeding

                             }).FirstOrDefault();

                horse.AdminId = adminIds;
                List<Post> posts = (from h in context.Horses
                                    where h.ID == horse.ID
                                    select h.Blog.Posts).FirstOrDefault();

                posts = posts.OrderBy(o=>o.Created).ToList();

                posts.Reverse();

                List<BlogPostModel> blogposts = posts.Select(x => new BlogPostModel()
                {
                    BlogID = x.Blog.ID,
                    ID = x.ID,
                    IsActive = x.IsActive,
                    Created = x.Created,
                    Content = x.Content,
                    Title = x.Title,
                    
                    
                }).ToList();

                horse.Blog.Posts = blogposts;
                return horse;
            }

        }

        public static void RemoveAdminByID(int id)
        {
            using(var context = new HorseContext())
            {
                var user = context.Users.Find(id);
                user.IsActive = false;
                context.SaveChanges();
            }
        }

        public static List<AdminModel> GetAllAdmins()
        {
            using (var context = new HorseContext())
            {
                return (from a in context.Users
                        where a.IsActive == true
                        select new AdminModel { Username = a.Username, AdminLevel = a.AdminLevel, ID = a.ID }).ToList();
            }
        }

        public static HorseModel GetSpecificHorseById(int id)
        {
            using (var context = new HorseContext())
            {
                return (from h in context.Horses
                        where h.ID == id
                        select new HorseModel()
                        {
                            ID = h.ID,
                            Name = h.Name,
                            Race = h.Race,
                            Withers = h.Withers,
                            Birthday = h.Birthday,
                            Description = h.Description,
                            Medicine = h.Medicine,
                            FamilyTree = h.FamilyTree,
                            Awards = h.Awards,
                            ImagePath = h.ImagePath,
                            Rent = h.Rent
                        }).FirstOrDefault();
            }
        }

        public static void AddNewYoutubeVideoToHorse(HorseVideoModel model)
        {
            using (var context = new HorseContext())
            {
                var horse = (from h in context.Horses
                             where h.ID == model.HorseID
                             select h).FirstOrDefault();
                horse.LastUpdated = DateTime.Now;

                horse.YoutubeVideoURLs.Add(new YoutubeVideoURL()
                {
                    VideoName = model.VideoName,
                    VideoURL = model.VideoURL
                });

                context.SaveChanges();
            }
        }

        public static int DeleteYoutubeVideoFromHorse(int id)
        {
            int horseID;

            using (var context = new HorseContext())
            {
                var videoToRemove = (from y in context.YoutubeVideoURLs
                                     where y.ID == id
                                     select y).FirstOrDefault();

                horseID = videoToRemove.Horse.ID;

                context.YoutubeVideoURLs.Remove(videoToRemove);
                context.SaveChanges();

            }

            return horseID;
        }

        public static List<HorseModel> GetHorsesDependingOnNavigation(int navigationId)
        {

            using (var context = new HorseContext())
            {
                List<HorseModel> horseList = new List<HorseModel>();
                switch (navigationId)
                {
                    case 1:
                        horseList = (from h in context.Horses
                                     where h.IsActive == true
                                     select new HorseModel()
                                     {
                                         Awards = h.Awards,
                                         Birthday = h.Birthday,
                                         ID = h.ID,
                                         IsActive = h.IsActive,
                                         IsForSale = h.IsForSale,
                                         Name = h.Name,
                                         Price = h.Price,
                                         Race = h.Race,
                                         Withers = h.Withers,
                                         IsSold = h.IsSold,
                                         Breeding = h.Breeding,
                                         State = "All horses",
                                         ImagePath = h.ImagePath,
										 Gender = h.Gender,
                                         Rent = h.Rent
                                     }).ToList();
                        break;

                    case 2:
                        horseList = (from h in context.Horses
                                     where h.IsForSale == true && h.IsActive == true
                                     select new HorseModel()
                                     {
                                         Awards = h.Awards,
                                         Birthday = h.Birthday,
                                         ID = h.ID,
                                         IsActive = h.IsActive,
                                         IsForSale = h.IsForSale,
                                         Breeding = h.Breeding,
                                         Name = h.Name,
                                         Price = h.Price,
                                         Race = h.Race,
                                         Withers = h.Withers,
                                         IsSold = h.IsSold,
                                         State = "Horses for sale",
                                         ImagePath = h.ImagePath,
										 Gender = h.Gender
                                     }).ToList();
                        break;
                    case 3:
                        horseList = (from h in context.Horses
                                     where h.IsSold == true && h.IsActive == true
                                     select new HorseModel()
                                     {
                                         Awards = h.Awards,
                                         Birthday = h.Birthday,
                                         ID = h.ID,
                                         IsActive = h.IsActive,
                                         Breeding = h.Breeding,
                                         IsForSale = h.IsForSale,
                                         Name = h.Name,
                                         Price = h.Price,
                                         Race = h.Race,
                                         Withers = h.Withers,
                                         IsSold = h.IsSold,
                                         State = "Sold horses",
                                         ImagePath = h.ImagePath,
										 Gender = h.Gender
                                     }).ToList();
                        break;
                }
                return horseList;
            }

        }

        public static AboutReadOnlyModel AboutReadOnly()
        {
            using (var contex = new HorseContext())
            {

                return (from a in contex.Abouts
                        select new AboutReadOnlyModel()
                        {
                            Header1 = a.Header1,
                            Header2 = a.Header2,
                            Header3 = a.Header3,
                            Textfield1 = a.Textfield1,
                            Textfield2 = a.Textfield2,
                            Textfield3 = a.Textfield3
                        }).ToList().LastOrDefault();

            }
        }

        public static int GetAdminId(string username)
        {
            using (var context = new HorseContext())
            {
                return
                    (from u in context.Users
                     where u.Username == username
                     select u.ID).FirstOrDefault();
            }

        }

        public static bool AuthenticateAdminLogin(string username, string password)
        {
            using (var context = new HorseContext())
            {
                return
                    (from a in context.Users
                     where a.Username == username && a.Password == password && a.IsActive
                     select a).Any();
            }
        }

        public static List<HorseSaleModel> GetHorsesForSale()
        {
            using (var context = new HorseContext())
            {
                return
                    (from h in context.Horses
                     where h.IsForSale
                     select new HorseSaleModel
                     {
                         ID = h.ID,
                         Birthday = h.Birthday,
                         Name = h.Name,
                         Prices = h.Price,
                         Race = h.Race,
                         Withers = h.Withers,
                         IsForSale = h.IsForSale
                     }).ToList();
            }
        }

        public static EditAdminModel GetAdminInformationForEditModel(int id)
        {
            using (var context = new HorseContext())
            {
                return (from u in context.Users
                    where u.ID == id
                    select new EditAdminModel
                    {
                        Email = u.Email
                    }).FirstOrDefault();
            }
        }

        public static void UpdateAdminProfile(int adminId, EditAdminModel model)
        {
            using (var context = new HorseContext())
            {

                var admin = context.Users.Find(adminId);
                admin.Email = model.Email;
                admin.Password = model.Password.SuperHash();


                context.Entry(admin).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static EditHorseProfileModel GetFullInformationOnSpecificHorseById(int id)
        {
            using (var context = new HorseContext())
            {

                return (from h in context.Horses
                        where h.ID == id
                        select new EditHorseProfileModel
                        {
                            Awards = h.Awards,
                            Birthday = h.Birthday,
                            Breeding = h.Breeding,
                            Description = h.Description,
                            FacebookPath = h.FacebookPath,
                            FamilyTree = h.FamilyTree,
                            Gender = h.Gender,
                            IsActive = !h.IsActive,
                            IsForSale = h.IsForSale,
                            IsSold = h.IsSold,
                            Medicine = h.Medicine,
                            Rent = h.Rent,
                            Name = h.Name,
                            Price = h.Price,
                            Race = h.Race,
                            Withers = h.Withers
                        }).FirstOrDefault();


            }
        }

		

        public static void UpdateHorseProfile(int horseID, EditHorseProfileModel model)
        {
            using (var context = new HorseContext())
            {

                var horse = context.Horses.Find(horseID);
                horse.LastUpdated = DateTime.Now;
                horse.Name = model.Name;
                horse.Birthday = model.Birthday;
                horse.Race = model.Race;
                horse.Awards = model.Awards;
                horse.Description = model.Description;
                horse.Medicine = model.Medicine;
                horse.FamilyTree = model.FamilyTree;
                horse.IsForSale = model.IsForSale;
                horse.Price = model.Price;
                horse.IsActive = !model.IsActive;
                horse.FacebookPath = model.FacebookPath;
                horse.IsSold = model.IsSold;
                horse.Gender = model.Gender;
                horse.Breeding = model.Breeding;
                horse.Rent = model.Rent;


                context.Entry(horse).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static List<ImageModel> GetAllGalleryImages()
        {
            using (var context = new HorseContext())
            {
                return (from i in context.GalleryImages
                        where i.Active == true && i.ImagePath.StartsWith(@"~/Images/")
                        select (new ImageModel
                        {
                            ID = i.ID,
                            Active = i.Active,
                            ImagePath = i.ImagePath,
                            FileName = i.FileName
                        })).ToList();
            }
        }

        public static void AddNewFile(string fileName, string path)
        {
            using (var context = new HorseContext())
            {
                context.GalleryImages.Add(new GalleryImage()
                {
                    FileName = fileName,
                    ImagePath = path,
                    Active = true
                }
                    );
                context.SaveChanges();
            }
        }

        public static string RemoveOldProfileImage(int id)
        {
            string path = "";

            using (var context = new HorseContext())
            {
                GalleryImage image = (from h in context.GalleryImages
                                      where h.FileName.StartsWith(id + "")
                                      select h).FirstOrDefault();

                path = image.ImagePath;
                context.GalleryImages.Remove(image);
                context.SaveChanges();
            }
            ;

            return path;
        }

        public static List<HorseProfileGalleryImagesModel> GetHorseProfileGalleryImages(int id)
        {
            using (var context = new HorseContext())
            {
                return (from i in context.GalleryImages
                        where i.Active == true && i.ImagePath.StartsWith(@"~/HorseProfileImages/" + id)
                        select (new HorseProfileGalleryImagesModel()
                        {
                            ID = i.ID,
                            Active = i.Active,
                            ImagePath = i.ImagePath,
                            FileName = i.FileName,
                            HorseID = id
                        })).ToList();
            }
        }

        public static int DeleteGalleryImage(int id)
        {
            using (var context = new HorseContext())
            {
                int horseid = 0;
                var image = context.GalleryImages.Find(id);
                horseid = image.FileName[0];
                context.GalleryImages.Remove(image);
                context.SaveChanges();
                return horseid;
            }
        }

        public static void AddNewFilePathToHorse(string imageName, int horseId)
        {
            string imagePath = @"~/ProfileImages/" + imageName;

            using (var context = new HorseContext())
            {
                var horse = context.Horses.Find(horseId);
                horse.LastUpdated = DateTime.Now;
                horse.ImagePath = imagePath;
                context.SaveChanges();
            }
        }

        public static string GetFacebookPath(int id)
        {
            using (var context = new HorseContext())
            {
                var query =
                    (from h in context.Horses
                     where h.ID == id
                     select h.FacebookPath).First();

                return query;
            }
        }

        public static int AddNewBlogPost(BlogPostModel blogpost, int blogId)
        {
            var horseId = 0;
            using (var context = new HorseContext())
            {
                var blog = context.Blogs.Find(blogId);
                var newBlogpost = new Post(blogpost.Title, blogpost.Created, blogpost.Content)
                {
                    Blog = blog,
                    ID = blogpost.ID,
                    Created = DateTime.Now,
                    IsActive = true
                };

                var horse = (from h in context.Horses
                             where blog.ID == blogId
                             select h).FirstOrDefault();

                horse.LastUpdated = DateTime.Now;

                blog.Posts.Add(newBlogpost);
                context.SaveChanges();
                horseId = (from h in context.Horses
                           where h.Blog.ID == blogId
                           select h.ID).FirstOrDefault();
            }
            return horseId;
        }


        public static List<HorseModel> GetLatestUpdates()
        {
            using (var context = new HorseContext())
            {
                var query =
                    (from h in context.Horses
                     select new HorseModel() { ID = h.ID, ImagePath = h.ImagePath }
                        ).ToList();

                if (query.Count > 5)
                {
                    var HM = new List<HorseModel>();

                    HM.Add(query[0]);
                    HM.Add(query[1]);
                    HM.Add(query[2]);
                    HM.Add(query[3]);
                    HM.Add(query[4]);
                }

                return query;

            }
        }
        public static EditAboutModel GetLatestAboutInformation()
        {
            using (var context = new HorseContext())
            {
                var query =
                    (from a in context.Abouts
                     orderby a.ID descending
                     select
                         new EditAboutModel()
                         {
                             ID = a.ID,
                             Header1 = a.Header1,
                             Header2 = a.Header2,
                             Header3 = a.Header3,
                             Textfield1 = a.Textfield1,
                             Textfield2 = a.Textfield2,
                             Textfield3 = a.Textfield3
                         }).FirstOrDefault();

                return query;
            }
        }

        public static About GetLatestAbout()
        {
            using (var context = new HorseContext())
            {
                var query =
                    (from a in context.Abouts
                     orderby a.ID descending
                     select a).FirstOrDefault();

                return query;
            }
        }

        public static void UpdateAbouts(About about)
        {
            using (var context = new HorseContext())
            {
                context.Entry(about).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static About SetAboutValues(EditAboutModel model, About about)
        {
            about.ID = model.ID;
            about.Header1 = model.Header1;
            about.Header2 = model.Header2;
            about.Header3 = model.Header3;
            about.Textfield1 = model.Textfield1;
            about.Textfield2 = model.Textfield2;
            about.Textfield3 = model.Textfield3;
            return about;
        }

        public static EditPuffModel GetPuffs()
        {
            using (var context = new HorseContext())
            {
                var query =
                    (from p in context.Puffs
                     orderby p.ID descending
                     select
                         new EditPuffModel()
                         {
                             ID = p.ID,
                             Header1 = p.Header1,
                             Header2 = p.Header2,
                             Header3 = p.Header3,
                             Textfield1 = p.Textfield1,
                             Textfield2 = p.Textfield2,
                             Textfield3 = p.Textfield3,
                             Link1 = p.Link1,
                             Link2 = p.Link2,
                             Link3 = p.Link3
                         }).FirstOrDefault();

                return query;
            }
        }


        public static List<EditorModel> GetEditorForSpecificHorse(int id)
        {
            using (var context = new HorseContext())
            {
                var users =
                    (from h in context.Horses
                     where h.ID == id
                     select h.AssignedEditors).FirstOrDefault();
                List<EditorModel> model = new List<EditorModel>();
                var horseName =
                    (from h in context.Horses
                     where h.ID == id
                     select h.Name).FirstOrDefault();

                foreach (var user in users)
                {
                    model.Add(new EditorModel()
                    {
                        EditorId = user.ID,
                        EditorName = user.Username,
                        HorseId = id,
                        HorseName = horseName
                    });
                }
                return model;
            }
        }

        public static void AddEditor(EditorModel editor, int horseId)
        {
            using (var context = new HorseContext())
            {
                var EditorId = editor.EditorId;


                var horse =
                    (from h in context.Horses
                     where h.ID == horseId
                     select h).FirstOrDefault();
                var editorExists =
                    (from e in context.Users
                     where e.Username == editor.EditorName
                     select e).Any();

                if (editorExists == false)
                {
                    return;
                }

                var Editor =
                    (from e in context.Users
                     where e.Username == editor.EditorName
                     select e).FirstOrDefault();

                if (horse.AssignedEditors == null)
                {
                    horse.AssignedEditors = new List<User>();
                }
                if (Editor.AssignedHorses == null)
                {
                    Editor.AssignedHorses = new List<Horse>();
                }
                if (horse != null)
                    horse.AssignedEditors.Add(Editor);
                context.SaveChanges();
            }
        }

        public static void UpdatePuffs(EditPuffModel model)
        {
            using (var context = new HorseContext())
            {

                var puff = context.Puffs.First();
                puff.Header1 = model.Header1;
                puff.Header2 = model.Header2;
                puff.Header3 = model.Header3;
                puff.Textfield1 = model.Textfield1;
                puff.Textfield2 = model.Textfield2;
                puff.Textfield3 = model.Textfield3;
                puff.Link1 = model.Link1;
                puff.Link2 = model.Link2;
                puff.Link3 = model.Link3;

                context.Entry(puff).State = EntityState.Modified;
                context.SaveChanges();
            }
        }


        public static void RemoveEditorFromHorse(int horseId, int editorid)
        {
            using (var context = new HorseContext())
            {
                var horse =
                    (from h in context.Horses
                     where h.ID == horseId
                     select h).FirstOrDefault();
                var Editor =
                    (from e in context.Users
                     where e.ID == editorid
                     select e).FirstOrDefault();

                horse.AssignedEditors.Remove(Editor);
                context.SaveChanges();

            }
        }
        public static List<ChildModel> GetChildrenInFamilyTree(int id)
        {
            using (var context = new HorseContext())
            {
                var listWithChildren = new List<ChildModel>();
                var horse = (from c in context.Horses
                             where c.ID == id
                             select c).FirstOrDefault();
                if (horse.Tree == null)
                {
                    horse.Tree = new FamilyTree() { Children = new List<Child>() };
                    horse.Tree.Children.Add(new Child());
                    listWithChildren.Add(new ChildModel() { horseid = id, HorseName = horse.Name });
                    return listWithChildren;
                }
                else
                {
                    var list = (from c in context.Horses
                                where c.ID == id
                                select c.Tree.Children).FirstOrDefault();


                    foreach (var child in list)
                    {
                        listWithChildren.Add(new ChildModel() { Description = child.Description, Name = child.Name, horseid = id, HorseName = context.Horses.Find(id).Name });

                    } return listWithChildren;
                }


            }
        }
        public static ParentModel GetParentsInFamilyTree(int id)
        {
            using (var context = new HorseContext())
            {
                return (from c in context.Horses
                        where c.ID == id
                        select new ParentModel()
                        {
                            MotherName = c.Tree.MotherName,
                            MotherDescription = c.Tree.MotherDescription,
                            FatherName = c.Tree.FatherName,
                            FatherDescription = c.Tree.FatherDescription,
                            horseid = id,
                            HorseName = c.Name
                        }).FirstOrDefault();
            }
        }
        public static void EditHorseFamilyTree(ParentModel model)
        {
            using (var context = new HorseContext())
            {
                var horse = context.Horses.Find(model.horseid);
                if (horse.Tree == null)
                {
                    horse.Tree = new FamilyTree { Children = new List<Child>() };
                }
                horse.Tree.FatherName = model.FatherName;
                horse.Tree.FatherDescription = model.FatherDescription;
                horse.Tree.MotherName = model.MotherName;
                horse.Tree.MotherDescription = model.MotherDescription;
                context.SaveChanges();
            }
        }


        public static void AddNewChild(ChildModel childModel)
        {
            using (var context = new HorseContext())
            {
                var horse = context.Horses.Find(childModel.horseid);
                if (horse.Tree == null)
                {
                    horse.Tree = new FamilyTree { Children = new List<Child>() };
                }
                horse.Tree.Children.Add(new Child() { Name = childModel.Name, Description = childModel.Description });
                horse.LastUpdated = DateTime.Now;
                context.SaveChanges();
            }

        }

        public static List<HorseModel> GetSearchResult(string term, int navigationId)
        {
            using (var context = new HorseContext())
            {
                List<HorseModel> horseList = new List<HorseModel>();
                switch (navigationId)
                {
                    case 1:
                        horseList = (from h in context.Horses
                                     where h.Name.Contains(term)&& h.IsActive == true
                                     select new HorseModel()
                                     {
                                         Awards = h.Awards,
                                         Birthday = h.Birthday,
                                         ID = h.ID,
                                         IsActive = h.IsActive,
                                         IsForSale = h.IsForSale,
                                         Name = h.Name,
                                         Price = h.Price,
                                         Race = h.Race,
                                         Withers = h.Withers,
                                         IsSold = h.IsSold,
                                         State = "All horses",
                                         ImagePath = h.ImagePath
                                     }).ToList();
                        break;


                    case 2:
                        horseList = (from h in context.Horses
                                     where h.Name.Contains(term)&&h.IsForSale == true && h.IsActive == true
                                     select new HorseModel()
                                     {
                                         Awards = h.Awards,
                                         Birthday = h.Birthday,
                                         ID = h.ID,
                                         IsActive = h.IsActive,
                                         IsForSale = h.IsForSale,
                                         Name = h.Name,
                                         Price = h.Price,
                                         Race = h.Race,
                                         Withers = h.Withers,
                                         IsSold = h.IsSold,
                                         State = "Horses for sale",
                                         ImagePath = h.ImagePath
                                     }).ToList();
                        break;
                    case 3:
                        horseList = (from h in context.Horses
                                     where h.Name.Contains(term) && h.IsSold == true && h.IsActive == true
                                     select new HorseModel()
                                     {
                                         Awards = h.Awards,
                                         Birthday = h.Birthday,
                                         ID = h.ID,
                                         IsActive = h.IsActive,
                                         IsForSale = h.IsForSale,
                                         Name = h.Name,
                                         Price = h.Price,
                                         Race = h.Race,
                                         Withers = h.Withers,
                                         IsSold = h.IsSold,
                                         State = "Sold horses",
                                         ImagePath = h.ImagePath
                                     }).ToList();
                        break;


                } return horseList;
            }
        }

		public static void AddHorse(CreateHorseModel model)
		{
			using (var context = new HorseContext())
			{
				Horse newHorse = new Horse
				{
					Name = model.Name,
					Birthday = model.Birthday,
					Race = model.Race,
					Withers = model.Withers,
					Awards = model.Awards,
					IsActive = true,
					IsForSale = model.IsForSale,
					Price = model.Price,
					Description = model.Description,
					Medicine = model.Medicine,
					FamilyTree = model.FamilyTree,
					Gender = model.Gender,
					Breeding = model.Breeding,
                    ImagePath = "~/ProfileImages/DefaultHead.jpg",
					Blog = new Blog
					{
						Created = DateTime.Now,
						Description = "",
						Posts = new List<Post>()
					}
				};

				context.Horses.Add(newHorse);
				context.Blogs.Add(newHorse.Blog);
				context.SaveChanges();
			}
		}

        public static void DeleteSpecificPost(int postid, int blogid)
        {
            using (var context = new HorseContext())
            {
                var blog = context.Blogs.Find(blogid);
                var post = context.Posts.Find(postid);
                post.IsActive = false;
                context.SaveChanges();
            }
        }
    }
}






