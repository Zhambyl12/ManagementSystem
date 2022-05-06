using System;
using System.Net;
using System.Text;
using System.Net.Mail; 
using System.Net.Http;
using System.Configuration; 
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity; 
using Microsoft.AspNet.Identity.Owin; 
using Microsoft.AspNet.Identity.EntityFramework; 
using Newtonsoft.Json;
using ManagementSystem.Models;
using System.Diagnostics;

namespace ManagementSystem
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            MailMessage mailmessage = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            string mail = ConfigurationManager.AppSettings["mailAccount"];
            string pass = ConfigurationManager.AppSettings["mailPassword"];
            try
            {
                mailmessage.From = new MailAddress("notification@m-system.kz", "ManagmentSystem", Encoding.UTF8);
                mailmessage.To.Add(new MailAddress(message.Destination));
                mailmessage.Subject = message.Subject;
                mailmessage.IsBodyHtml = true;
                mailmessage.Body = message.Body;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(mail, pass);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mailmessage);
            }

            catch (Exception ex) { Debug.WriteLine(ex.Message); }

            finally { await Task.FromResult(0); }
        }
       
    }

    public class SmsService : IIdentityMessageService
    { 
        public Task SendAsync(IdentityMessage message)
        { 
            string json = "{\"messages\":[{\"channel\": \"whatsapp\", \"to\": \"77056828910\", \"content\": "+message.Body+ "},{\"channel\": \"sms\",\"to\": \"77056828910\",\"content\": "+message.Body+"}]}";
            try
            { 
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpClient client = new HttpClient();
                    httpClient.BaseAddress = new Uri("https://platform.clickatell.com");
                    client.DefaultRequestHeaders.Clear(); 

                    client.DefaultRequestHeaders.Add("Authorization",String.Format("4hGT3G1OS--1StSdjtfjFw==")); 
                    
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8);

                    var response = client.PostAsync("https://platform.clickatell.com/v1/message", httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var data = response.Content.ReadAsStringAsync();
                        return Task.FromResult(data.Result);
                    }
                     
                }
            }
            catch (Exception ex)
            { 
                throw new Exception(ex.ToString());
            }
            // Подключите здесь службу SMS, чтобы отправить текстовое сообщение.
            return Task.FromResult(0);
        }
    }

    // Настройка диспетчера пользователей приложения. UserManager определяется в ASP.NET Identity и используется приложением.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        } 
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Настройка логики проверки имен пользователей
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Настройка логики проверки паролей
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Настройка параметров блокировки по умолчанию
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Регистрация поставщиков двухфакторной проверки подлинности. Для получения кода проверки пользователя в данном приложении используется телефон и сообщения электронной почты
            // Здесь можно указать собственный поставщик и подключить его.
            manager.RegisterTwoFactorProvider("Код, полученный по телефону", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Ваш код безопасности: {0}"
            });
            manager.RegisterTwoFactorProvider("Код из сообщения", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Код безопасности",
                BodyFormat = "Ваш код безопасности: {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Настройка диспетчера входа для приложения.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }

}
