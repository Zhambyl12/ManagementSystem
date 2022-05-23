using ManagementSystem.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ManagementSystem.Controllers
{ 
    [Authorize(Roles = "HR-manager")]
    public class UsersAdminController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Users/
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }
         

        //
        // GET: /Users/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { 
                    UserName = userViewModel.Email, 
                    Email = userViewModel.Email,
                    FirstName= userViewModel.FirstName,
                    LastName= userViewModel.LastName,
                    MiddleName= userViewModel.MiddleName,
                    Department= userViewModel.Department,
                    Position= userViewModel.Position,
                    StartedDate= DateTime.Now.Date
                };
                 
                var adminresult = await UserManager.CreateAsync(user, "Test1234!");

                await UserManager.SendEmailAsync(user.Id, "Регистрация", "Ваш пароль: Test1234!");

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();
                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }
         
        //
        // GET: /Users/AddElectronic/1
        [HttpGet]
        public async Task<ActionResult> AddElectronic(string id)
        {
            if (id == null) 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            ApplicationUser user = await UserManager.FindByIdAsync(id);
            if (user == null) 
                return HttpNotFound();
             
            return View(new AddElectronicToUserViewModel()
            {
                UserInfo = user,
                UserId = user.Id, 
                ElectronicsList = Misc.GetAllElectronics().Select(x => new SelectListItem()
                {
                    Selected = false,
                    Text = x.Title+" ("+x.SerialNumber+")",
                    Value = x.Id.ToString()
                })
            });
        }

        //
        // POST: /Users/AddElectronic/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddElectronic([Bind(Include = "UserId")] AddElectronicToUserViewModel addElectronic, string SelectedElectronics)
        {
            addElectronic.ElectronicsList  = Misc.GetAllElectronics().Select(x => new SelectListItem()
            {
                Selected = false,
                Text = x.Title + " (" + x.SerialNumber + ")",
                Value = x.Id.ToString()
            });
            addElectronic.UserInfo = Misc.GetUser(addElectronic.UserId);
            var electronic = Misc.GetElectronics(SelectedElectronics); 
            if (electronic == null)
            {
                ModelState.AddModelError("", "Пожалуйста выберите электронику");
                return View(addElectronic); 
            }
            
            if (ModelState.IsValid)
            {  
                try
                {
                    electronic.ApplicationUser = addElectronic.UserInfo;
                    electronic.UserId = new Guid(addElectronic.UserId); 
                    db.Entry(electronic).State = EntityState.Modified;
                    db.SaveChanges(); 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(addElectronic);
                }
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Что-то произошло не так.");
            return View(addElectronic);
        }


        public ActionResult Add(Guid? id)
        {
            ViewBag.Users = db.Users.Select(x => new SelectListItem()
            {
                Selected = false,
                Text = x.LastName + " " + x.FirstName + " " + x.MiddleName,
                Value = x.Id.ToString()
            }).ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Electronics electronics = db.Electronics.Find(id);
            if (electronics == null)
            {
                return HttpNotFound();
            }
            return View(electronics);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "Id,SerialNumber,Title,CPU,RAM,DiskSize,DiskType,Video,OS,UserId")] Electronics electronics)
        {  
            if (ModelState.IsValid)
            {
                db.Entry(electronics).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(electronics);
        }
        //
        // GET: /Users/Details/1
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null) 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var user = await UserManager.FindByIdAsync(id);
            if (user == null) 
                return HttpNotFound();
            
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            var electronics = Misc.GetUserElectronics(user.Id);
            return View(new DetailsUserViewModel()
            {
                UserInfo = user,
                Electronics = electronics,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // GET: /Users/Details/5
        [HttpGet]
        public async Task<ActionResult> Details2(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
            return View(user);
        }

        //
        // GET: /Users/Edit/1
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            return View(new ExtraEditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                LastName = user.LastName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                BirthDate = user.BirthDate,
                Department = user.Department,
                Position = user.Position,
                Photo = user.Photo,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,FirstName,LastName,MiddleName,Department,Position,Id,BirthDate")] ExtraEditUserViewModel editUser,HttpPostedFileBase Photo, params string[] selectedRole)
        {
            string lastimg = "";
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.LastName = editUser.LastName;
                user.FirstName = editUser.FirstName;
                user.MiddleName = editUser.MiddleName;
                user.Department = editUser.Department;
                user.Position = editUser.Position;
                lastimg = user.Photo;
                if (editUser.BirthDate != null)
                    user.BirthDate = editUser.BirthDate;
                if(Photo!=null && Photo.ContentLength > 0)
                {
                    user.Photo = Misc.SaveFileToUser(Photo, user.Id, "ProfilePhoto");
                    if (string.IsNullOrEmpty(lastimg))
                        Misc.DeleteFile(lastimg);
                }
                user.Position = editUser.Position;
                var userRoles = await UserManager.GetRolesAsync(user.Id);
                selectedRole = selectedRole ?? new string[] { };
                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                 
                return RedirectToAction("Index");
            }
            var roles = await UserManager.GetRolesAsync(editUser.Id);
            ModelState.AddModelError("", "Что то пошло не так."); 
            return View(new ExtraEditUserViewModel()
            {
                Id = editUser.Id,
                Email = editUser.Email,
                LastName = editUser.LastName,
                FirstName = editUser.FirstName,
                MiddleName = editUser.MiddleName,
                BirthDate = editUser.BirthDate,
                Department = editUser.Department,
                Position = editUser.Position,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected =roles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // GET: /Users/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}