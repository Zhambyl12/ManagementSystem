using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementSystem.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Название роли")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }


    public class ExtraEditUserViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
         
        [Display(Name = "Фото")] 
        public string Photo { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Имя")] 
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Фамилия")] 
        public string LastName { get; set; }

        [Display(Name = "Отчество")] 
        public string MiddleName { get; set; }
        
        [Display(Name = "День рождения")] 
        public DateTime? BirthDate { get; set; }
         
        [Display(Name = "Отдел")]
        public string Department { get; set; }
         
        [Display(Name = "Должность")]
        public string Position { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }

    public class DetailsUserViewModel
    {  
        public ApplicationUser UserInfo { get; set; }
        public Electronics Electronics { get; set; }
        public IEnumerable<SelectListItem> RolesList { get; set; } 
    }

    public class AddElectronicToUserViewModel
    {
        public string UserId { get; set; }
        public Electronics Electronics { get; set; }
        public ApplicationUser UserInfo { get; set; }

        public IEnumerable<SelectListItem> ElectronicsList { get; set; }
    }
}