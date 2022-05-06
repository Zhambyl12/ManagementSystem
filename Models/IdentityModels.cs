﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ManagementSystem.Models
{  
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }
        [Display(Name = "Дата рождения")]
        public DateTime? BirthDate { get; set; }
        [Display(Name = "Дата принятия")]
        public DateTime StartedDate { get; set; }
        [Display(Name = "Дата увольнения")]
        public DateTime? RevokedDate { get; set; }
        [Display(Name = "Табельный номер")]
        public string TabNumber { get; set; }
        [Display(Name = "Должность")]
        public string Position { get; set; }
        [Display(Name = "Департамент")]
        public string Department { get; set; }
        [Display(Name = "Статус работника")]
        public string Status { get; set; }
        [Display(Name = "Фото")]
        public string Photo { get; set; } 
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            
            return userIdentity;
        }

        public string GetUserFullname()
        {
            return this.LastName + " " + this.FirstName + " " + this.MiddleName;
        }
    } 
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false) {} 
        public static ApplicationDbContext Create() =>  new ApplicationDbContext();

        public DbSet<Electronics> Electronics { get; set; }

    }

    public class Electronics
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Серийный номер")]
        public string SerialNumber { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Процессор")]
        public string CPU { get; set; }
        [Required]
        [Display(Name = "ОЗУ")]
        public int RAM { get; set; }
        [Required]
        [Display(Name = "ПЗУ")]
        public int DiskSize { get; set; }
        [Required]
        [Display(Name = "Тип диска")]
        public int DiskType { get; set; }
        [Required]
        [Display(Name = "Видеоадаптер")]
        public string Video { get; set; }
        [Required]
        [Display(Name = "ОС")]
        public string OS { get; set; }

        public Guid? UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
     
}