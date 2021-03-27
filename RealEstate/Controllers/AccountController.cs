using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RealEstate.Models;
using RealEstate.Utills;
using RealEstateDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealEstate.Controllers
{
    public class AccountController : BaseController
    {
        private RealEstateContext _dbContext;

        //private readonly ApplicationDbContext _context;
        private IConfiguration Configuration;

        public AccountController(RealEstateContext context, IConfiguration _configuration)
        {
            _dbContext = context;
            Configuration = _configuration;
        }
        public ActionResult AccessDenied()
        {
            return RedirectToAction("Login", "Account");
        }
        public IActionResult SetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SetPassword(SetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TblAgent agent = _dbContext.TblAgents.Where(x => x.EmailAddress.Equals(model.EmailAddress)).SingleOrDefault();
                    agent.Password = Encryption.EncryptText(model.ConfirmPassword);
                    _dbContext.Entry(agent).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                    ClaimsIdentity identity = null;
                    identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, agent.EmailAddress), new Claim(ClaimTypes.Role, "Agent") }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var prinicpal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                    SetCookie("EmailAddress", agent.EmailAddress, 1);
                    SetCookie("FullName", agent.FullName, 1);
                    SetCookie("UserLoginTypeId", agent.UserLoginTypeId.ToString(), 1);
                    if (model.KeepMeSigninIn)
                        return RedirectToAction("Index", "Lead", new { area = "" });
                    else
                        return RedirectToAction("Login", "Account", new { area = "" });
                }
                else
                    return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.log("AccountController SetPassword" + ex);
                return View(model);
            }
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var DB = _dbContext)
                    {
                        var resultForAdmin = DB.TblUsers.Where(u => u.EmailAddress.Equals(model.EmailAddress.Trim()) &&
                                                       u.Password.Equals(Encryption.EncryptText(model.Password)) &&
                                                       u.IsActive == true).FirstOrDefault();

                        ClaimsIdentity identity = null;
                        if (resultForAdmin != null)
                        {


                            if (resultForAdmin.UserLoginTypeId == UserLoginType.Admin.GetHashCode())
                            {
                                identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, resultForAdmin.EmailAddress), new Claim(ClaimTypes.Role, "Admin") }, CookieAuthenticationDefaults.AuthenticationScheme);
                                var prinicpal = new ClaimsPrincipal(identity);
                                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                                SetCookie("EmailAddress", resultForAdmin.EmailAddress, 1);
                                SetCookie("FullName", resultForAdmin.FullName, 1);
                                SetCookie("UserLoginTypeId", resultForAdmin.UserLoginTypeId.ToString(), 1);
                                return RedirectToAction("Index", "Lead", new { area = "" });
                            }
                        }
                        else
                        {
                            var resultForAgent = DB.TblAgents.Where(u => u.EmailAddress.Equals(model.EmailAddress.Trim()) &&
                                                           u.IsActive == true).FirstOrDefault();

                            if (resultForAgent != null)
                            {
                                if (string.IsNullOrEmpty(resultForAgent.Password))
                                {
                                    TempData["EmailAddress"] = resultForAgent.EmailAddress;
                                    TempData.Keep();
                                    return RedirectToAction("SetPassword", "Account", new { area = "" });
                                }
                                else
                                {
                                    if (resultForAgent.Password.Equals(Encryption.EncryptText(model.Password)))
                                    {
                                        if (resultForAgent.UserLoginTypeId != UserLoginType.Admin.GetHashCode())
                                        {
                                            identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, resultForAgent.EmailAddress), new Claim(ClaimTypes.Role, "Agent") }, CookieAuthenticationDefaults.AuthenticationScheme);
                                            var prinicpal = new ClaimsPrincipal(identity);
                                            var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                                            SetCookie("EmailAddress", resultForAgent.EmailAddress, 1);
                                            SetCookie("FullName", resultForAgent.FullName, 1);
                                            SetCookie("UserLoginTypeId", resultForAgent.UserLoginTypeId.ToString(), 1);
                                            return RedirectToAction("Index", "Lead", new { area = "" });
                                        }
                                    }
                                    else
                                    {
                                        ShowErrorMessage("User credentials are not mathced please try again!", true);
                                    }
                                }

                            }
                            else
                            {
                                ShowErrorMessage("User credentials are not mathced please try again!", true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                ErrorLog.log("Account Controller AuthenticateAccount:-" + ex);
                return View("Login");
            }
            return View(model);
        }

        public IActionResult GoogleLogin()
        {
            try
            {
                var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message.ToString(), true);
                ErrorLog.log("Account Controller GoogleLogin:-" + ex);
                return View("Login");
            }

        }

        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                // Here the following code reperesent that user is sucessfully authenticated by google
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (result.Principal.Claims.Count() <= 0)
                {
                    ShowErrorMessage("User is not authorize!", true);
                    //throw new Exception();
                    return View("Login");
                }
                // Here the following code reperesent that user is sucessfully authenticated by google.
                //We are taking user's information and redirecting user to his environment.
                TblAgent oAgent = new TblAgent();
                oAgent.EmailAddress = result.Principal.FindFirst(ClaimTypes.Email).Value;
                oAgent.FullName = result.Principal.FindFirst(ClaimTypes.Name).Value;
                oAgent.UserLoginTypeId = UserLoginType.GoogleAccount.GetHashCode();

                if (!IsUserExist(oAgent.EmailAddress))
                {
                    oAgent.IsActive = true;
                    oAgent.CreatedDate = DateTime.Now;
                    _dbContext.TblAgents.Add(oAgent);
                    _dbContext.SaveChanges();
                }
                else
                    oAgent = _dbContext.TblAgents.Where(x => x.EmailAddress.Equals(oAgent.EmailAddress)).SingleOrDefault();

                if (string.IsNullOrEmpty(oAgent.Password))
                {
                    TempData["EmailAddress"] = oAgent.EmailAddress;
                    TempData.Keep();
                    return RedirectToAction("SetPassword", "Account");
                }
                else
                {
                    //Here we are storing claims for authentication
                    ClaimsIdentity identity = null;
                    identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, oAgent.EmailAddress), new Claim(ClaimTypes.Role, "Agent") }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var prinicpal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                    //Redirection of lead user is here. Please give appropreate direction URL to it

                    SetCookie("EmailAddress", oAgent.EmailAddress, 1);
                    SetCookie("FullName", oAgent.FullName, 1);
                    SetCookie("UserLoginTypeId", oAgent.UserLoginTypeId.ToString(), 1);

                    return RedirectToAction("Index", "Lead");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message.ToString(), true);
                ErrorLog.log("Account Controller GoogleResponse:-" + ex.Message.ToString());
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var DB = _dbContext)
                    {
                        var oUser = _dbContext.TblUsers.Where(x => x.EmailAddress.ToLower() == model.Email.ToLower()).FirstOrDefault();
                        if (oUser != null)
                        {
                            if (oUser.UserLoginTypeId == UserLoginType.GoogleAccount.GetHashCode())
                            {
                                ShowErrorMessage("You have registered with google account. You can't change your password from here!", true);
                                return View(model);
                            }

                            if (oUser.UserLoginTypeId == UserLoginType.MicrosoftAccount.GetHashCode())
                            {
                                ShowErrorMessage("You have registered with microsoft account. You can't change your password from here!", true);
                                return View(model);
                            }

                            string SmtpUserName = this.Configuration.GetSection("MailSettings")["SmtpUserName"];
                            string SmtpPassword = this.Configuration.GetSection("MailSettings")["SmtpPassword"];
                            int SmtpPort = Convert.ToInt32(this.Configuration.GetSection("MailSettings")["SmtpPort"]);
                            string SmtpServer = this.Configuration.GetSection("MailSettings")["SmtpServer"];
                            string fromEmail = this.Configuration.GetSection("MailSettings")["fromEmail"];
                            bool isSSL = Convert.ToBoolean(this.Configuration.GetSection("MailSettings")["isSSL"]);
                            var password = Encryption.DecryptText(oUser.Password);
                            var body = "<p>Hi,</p>" +
                                        "Your Email Address is " + oUser.EmailAddress + "<br/>" +
                                        "Your password is " + password + "<br/>" +
                                        "Thank You.";

                            var subject = "Estajo - ForgotPassword Mail";
                            Utility.sendMail(oUser.EmailAddress, body, subject, fromEmail, SmtpUserName, SmtpPassword, SmtpPort, SmtpServer, isSSL);

                            ModelState.Clear();
                            //ViewBag.sucessMessage = "Forgot Password Mail has been sent successfully. Please check the mail and login again.";
                            ShowSuccessMessage("Forgot Password Mail has been sent successfully. Please check the mail and login again.", true);
                            return View(model);
                        }
                        else
                        {
                            ShowErrorMessage("Sorry there is no account with this email address.", true);
                            return View(model);
                        }
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "Oops! Something went ForgotPassword.";
                ErrorLog.log("Account Controller ForgotPassword:-" + ex);
                return View();
            }
        }

        public IActionResult MicrosoftLogin()
        {
            try
            {
                //here the following code is redirecting the user to microsoft login
                var properties = new AuthenticationProperties { RedirectUri = Url.Action("MicrosoftResponse") };
                return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, true);
                ErrorLog.log("Account Controller MicrosoftLogin:-" + ex);
                return View("Login");
            }

        }

        public async Task<IActionResult> MicrosoftResponse()
        {
            try
            {
                // Here the following code reperesent that user is sucessfully authenticated by microsoft
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (result.Principal.Claims.Count() <= 0)
                {
                    throw new Exception("User is not authorize!");
                }
                // Here the following code reperesent that user is sucessfully authenticated by google.
                //We are taking user's information and redirecting user to his environment.
                TblAgent agent = new TblAgent();
                agent.EmailAddress = result.Principal.FindFirst(ClaimTypes.Email).Value;
                agent.FullName = result.Principal.FindFirst(ClaimTypes.Name).Value;
                agent.UserLoginTypeId = UserLoginType.MicrosoftAccount.GetHashCode();
                if (!IsUserExist(agent.EmailAddress))
                {
                    agent.IsActive = true;
                    agent.CreatedDate = DateTime.Now;
                    _dbContext.TblAgents.Add(agent);
                    _dbContext.SaveChanges();
                }
                else
                    agent = _dbContext.TblAgents.Where(x => x.EmailAddress.Equals(agent.EmailAddress)).SingleOrDefault();

                if (string.IsNullOrEmpty(agent.Password))
                {
                    TempData["EmailAddress"] = agent.EmailAddress;
                    TempData.Keep();
                    return RedirectToAction("SetPassword", "Account");
                }
                else
                {
                    //Here we are storing claims for authentication
                    ClaimsIdentity identity = null;
                    identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, agent.EmailAddress), new Claim(ClaimTypes.Role, "Agent") }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var prinicpal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                    //Redirection of lead user is here. Please give appropreate direction URL to it
                    SetCookie("EmailAddress", agent.EmailAddress, 1);
                    SetCookie("FullName", agent.FullName, 1);
                    SetCookie("UserLoginTypeId", agent.UserLoginTypeId.ToString(), 1);

                    return RedirectToAction("Index", "Lead", new { area = "" });
                }

            }
            catch (Exception ex)
            {
                ViewBag.ErroMessage = "Error occur while authente user. -->" + ex.Message;
                ErrorLog.log("Account Controller MicrosoftResponse:-" + ex);
                return View("Login");
            }

        }

        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Delete("FullName");
                Response.Cookies.Delete("EmailAddress");
                Response.Cookies.Delete("UserLoginTypeId");
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ErrorLog.log("Account Controller Logout:-" + ex);
                return View("Login");
            }
        }

        public bool IsUserExist(string email)
        {
            try
            {
                var result = _dbContext.TblAgents.Where(u => u.EmailAddress.ToLower().Equals(email.ToLower())).ToList();
                if (result.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                ErrorLog.log("Account Controller IsUserExist:-" + ex);
                return false;
            }
        }

        public void SetCookie(string key, string value, int? expireTimeInDays)
        {
            CookieOptions option = new CookieOptions();

            if (expireTimeInDays.HasValue)
                option.Expires = DateTime.Now.AddDays(expireTimeInDays.Value);
            else
                option.Expires = DateTime.Now.AddDays(2); //minimum 2 Days

            Response.Cookies.Append(key, value, option);
        }
    }
}
