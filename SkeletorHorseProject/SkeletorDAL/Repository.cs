﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using SkeletorDAL.Helpers;
using SkeletorDAL.Model;
using SkeletorDAL.POCO;
namespace SkeletorDAL
{
    public static class Repository
    {

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
        public static List<FooterModel> GetAllFooterLinks()
        {
            using (var context = new HorseContext())
            {
                return (from l in context.FooterLinks
                        select new FooterModel { ID = l.ID, Name = l.LinkName, Url = l.LinkURL }).ToList();
            }
        }

        public static void AddNewFooterLink(FooterModel model)
        {
            using(var context = new HorseContext())
            {
                context.FooterLinks.Add(new FooterLink() { LinkName = model.Name, LinkURL = model.Url });
                context.SaveChanges();
            }
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
            var context = new HorseContext();
            
                return (from h in context.Horses
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
                            Blog = h.Blog,
                            IsSold = h.IsSold,
                            FacebookPath = h.FacebookPath,
                            IsActive = h.IsActive,
                            IsForSale = h.IsForSale,
                            Price = h.Price
                        }).FirstOrDefault();
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
                            ImagePath = h.ImagePath
                        }).FirstOrDefault();
            }
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
                                         State = "All horses",
                                         ImagePath = h.ImagePath
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
                                     where h.IsSold == true && h.IsActive == true
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
                            Textfield1 = a.Textfield1,
                            Textfield2 = a.Textfield2
                        }).ToList().LastOrDefault();

            }
        }
        public static bool AuthenticateAdminLogin(string username, string password)
        {
            using (var context = new HorseContext())
            {
                return
                    (from a in context.Users
                     where a.Username == username && a.Password == password
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
        public static EditHorseProfileModel GetFullInformationOnSpecificHorseById(int id, EditHorseProfileModel model)
        {
            using (var context = new HorseContext())
            {
                var currentHorse = (from h in context.Horses
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
                            IsSold =  h.IsSold,
                            Rent = h.Rent,
                            Price = h.Price,
                            Medicine = h.Medicine,
                            Name = h.Name,
                            Race = h.Race,
                            Withers = h.Withers,
                            LastUpdated = DateTime.Now
                            
                        }).FirstOrDefault();
                return currentHorse;
            }
        }

        public static void AddHorse(Horse newHorse)
        {
            using (var context = new HorseContext())
            {
                context.Horses.Add(newHorse);
                context.SaveChanges();
            }
        }
        public static void UpdateHorseProfile(int horseID, EditHorseProfileModel model)
        {
            using (var context = new HorseContext())
            {

                var horse = context.Horses.Find(horseID);
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
            };

            return path;
        }
        public static void DeleteGalleryImage(int id)
        {
            using (var context = new HorseContext())
            {
                var image = context.GalleryImages.Find(id);
                context.GalleryImages.Remove(image);
                context.SaveChanges();
            }
        }

        public static void AddNewFilePathToHorse(string imageName, int horseId)
        {
            string imagePath = @"~/ProfileImages/" + imageName;

            using (var context = new HorseContext())
            {
                var horse = context.Horses.Find(horseId);
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

        public static List<HorseModel> GetLatestUpdates()
        {
            using (var context = new HorseContext())
            {
                var query =
                    (from h in context.Horses
                        select new HorseModel() {ID = h.ID, ImagePath = h.ImagePath}
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

      
    }
}

