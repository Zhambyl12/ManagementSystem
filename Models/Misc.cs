using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementSystem.Models
{
    public class Misc
    { 
        public static string MachineName;
        private static ApplicationDbContext db;
        public static ApplicationUser CurrentUser;
        private static Random random; 

        static Misc()
        {
            db = new ApplicationDbContext();
            CurrentUser = db.Users.Where(p => p.Email.Equals(HttpContext.Current.User.Identity.Name)).FirstOrDefault();
            MachineName = HttpContext.Current.Server.MachineName;
            random = new Random(); 
        }
        public static int GetUsersCount() { db = new ApplicationDbContext(); return db.Users.Count(); }
        public static int GetRolesCount() { db = new ApplicationDbContext(); return db.Roles.Count(); }
        public static List<ApplicationUser> GetUsers()
        {
            return db.Users.ToList();
        }
        public static List<Electronics> GetAllElectronics()
        {
            return db.Electronics.ToList();
        } 
        public static Electronics GetElectronics(string id)
        { 
            return db.Electronics.Where(p=>p.Id.ToString().Equals(id)).FirstOrDefault(); 
        } 
        public static Electronics GetUserElectronics(string userid)
        { 
            return db.Electronics.Where(p=>p.UserId.ToString().Equals(userid)).FirstOrDefault(); 
        } 
        public static ApplicationUser GetUser(string userid)
        {
            db = new ApplicationDbContext();
            return db.Users.Find(userid);
        }

        public static string SaveFileToUser(HttpPostedFileBase file, string userid, string typefile)
        {
            string result = "~/DISTR/Users/"+ userid+"/";
            string path1 = HttpContext.Current.Server.MapPath("~/DISTR/Users/" + userid + "/"); 
            string filename = typefile + "-" + RandomString(8) + Path.GetExtension(file.FileName);
            string path2 = Path.Combine(path1, filename);
            try
            {
                if (Directory.Exists(path1))
                {
                    file.SaveAs(path2); 
                }
                else
                {
                    Directory.CreateDirectory(path1);
                    file.SaveAs(path2); 
                }
                return result + filename;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteFile(string filepath)
        { 
            try
            {
                var path = HttpContext.Current.Server.MapPath(filepath);
                if (Directory.Exists(path))
                {
                    File.Delete(path);
                } 
                return;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEF0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}