//using EAGetMail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RealEstate.Models;
using RealEstate.Utills;
using RealEstateDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealEstate.Controllers
{
    public class AccountController : BaseController
    {
        private RealEstateContext _dbContext;

        //private readonly ApplicationDbContext _context;
        private IConfiguration Configuration;
        private IWebHostEnvironment Environment;

        public AccountController(RealEstateContext context, IConfiguration _configuration, IWebHostEnvironment _environment)
        {
            _dbContext = context;
            Configuration = _configuration;
            Environment = _environment;
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
                    //TblAgent agent = _dbContext.TblAgents.Where(x => x.EmailAddress.Equals(model.EmailAddress)).SingleOrDefault();
                    TblAccount agent = _dbContext.TblAccounts.Where(x => x.UserName.Equals(model.EmailAddress) && x.RoleId == RoleType.Agent.GetHashCode()).SingleOrDefault();
                    agent.Password = Encryption.EncryptText(model.ConfirmPassword);
                    _dbContext.Entry(agent).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                    ClaimsIdentity identity = null;
                    identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, agent.UserName), new Claim(ClaimTypes.Role, "Agent") }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var prinicpal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                    SetCookie("EmailAddress", agent.UserName);
                    SetCookie("FullName", agent.FullName);
                    //SetCookie("LoginAccountId", agent.AccountId.ToString());
                    SetCookie("UserLoginTypeId", RoleType.Agent.GetHashCode().ToString());
                    if (model.KeepMeSigninIn)
                    {
                        if (agent.IsEmailConfig == false)
                            return RedirectToAction("gettingstarted", "Account", new { area = "" });
                        else
                            return RedirectToAction("Index", "Lead", new { area = "" });
                    }
                    else
                    {
                        return RedirectToAction("Login", "Account", new { area = "" });
                    }
                }
                else
                    return View(model);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
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
                        //var resultForAdmin = DB.TblUsers.Where(u => u.EmailAddress.Equals(model.EmailAddress.Trim()) &&
                        //                               u.Password.Equals(Encryption.EncryptText(model.Password)) &&
                        //                               u.IsActive == true && u.UserLoginTypeId == UserLoginType.Admin.GetHashCode()).FirstOrDefault();


                        var oUserDetails = DB.TblAccounts.Where(u => u.UserName.Equals(model.EmailAddress.Trim()) &&
                                                       u.Password.Equals(Encryption.EncryptText(model.Password)) && u.Status == true && u.RoleId == RoleType.Admin.GetHashCode()).FirstOrDefault(); // && u.LogionTypeId == UserLoginType.Company.GetHashCode()

                        ClaimsIdentity identity = null;
                        //if (resultForAdmin != null)
                        if (oUserDetails != null)
                        {
                            //if (resultForAdmin.UserLoginTypeId == UserLoginType.Admin.GetHashCode())
                            //if (oCompanyDetails.LogionTypeId == UserLoginType.Company.GetHashCode())
                            {
                                identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, oUserDetails.UserName), new Claim(ClaimTypes.Role, "Company") }, CookieAuthenticationDefaults.AuthenticationScheme);
                                var prinicpal = new ClaimsPrincipal(identity);
                                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);

                                SetCookie("EmailAddress", oUserDetails.UserName);
                                SetCookie("FullName", oUserDetails.FullName);
                                SetCookie("LoginAccountId", oUserDetails.AccountId.ToString());
                                SetCookie("UserLoginTypeId", oUserDetails.RoleId.ToString());
                                //SetCookie("CompanyId", oCompanyDetails.CompanyId.ToString());
                                return RedirectToAction("Index", "Lead", new { area = "" });

                                //identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, resultForAdmin.EmailAddress), new Claim(ClaimTypes.Role, "Admin") }, CookieAuthenticationDefaults.AuthenticationScheme);
                                //var prinicpal = new ClaimsPrincipal(identity);
                                //var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                                //SetCookie("EmailAddress", resultForAdmin.EmailAddress);
                                //SetCookie("FullName", resultForAdmin.FullName);
                                //SetCookie("UserLoginTypeId", resultForAdmin.UserLoginTypeId.ToString());
                                //SetCookie("LoginUserId", resultForAdmin.UserId.ToString());
                                //return RedirectToAction("Index", "Lead", new { area = "" });
                            }
                        }
                        else
                        {
                            //var resultForAgent = DB.TblAgents.Where(u => u.EmailAddress.Equals(model.EmailAddress.Trim()) && u.IsActive == true).FirstOrDefault();
                            //u.Password.Equals(Encryption.EncryptText(model.Password))).FirstOrDefault();

                            var resultForAgent = DB.TblAccounts.Where(u => u.UserName.Equals(model.EmailAddress.Trim()) &&
                                                      u.RoleId == RoleType.Agent.GetHashCode() && u.Status == true).FirstOrDefault(); // && u.LogionTypeId == UserLoginType.Company.GetHashCode()


                            if (resultForAgent != null)
                            {
                                if (string.IsNullOrEmpty(resultForAgent.Password))
                                {
                                    TempData["EmailAddress"] = resultForAgent.UserName;
                                    TempData.Keep();
                                    return RedirectToAction("SetPassword", "Account", new { area = "" });
                                }
                                else
                                {
                                    if (resultForAgent.Password.Equals(Encryption.EncryptText(model.Password)))
                                    {
                                        //if (resultForAgent.RoleId != RoleType.Admin.GetHashCode())
                                        {
                                            identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, resultForAgent.UserName), new Claim(ClaimTypes.Role, "Agent") }, CookieAuthenticationDefaults.AuthenticationScheme);
                                            var prinicpal = new ClaimsPrincipal(identity);
                                            var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                                            SetCookie("EmailAddress", resultForAgent.UserName);
                                            SetCookie("FullName", resultForAgent.FullName);
                                            SetCookie("LoginAccountId", resultForAgent.AccountId.ToString());
                                            SetCookie("UserLoginTypeId", resultForAgent.RoleId.ToString());
                                            //if (resultForAgent.IsEmailConfig == false)
                                            return RedirectToAction("gettingstarted", "Account", new { area = "" });
                                            //else
                                            //return RedirectToAction("Index", "Lead", new { area = "" });
                                            //return RedirectToAction("Index", "Lead", new { area = "" });
                                        }
                                    }
                                    else
                                    {
                                        ShowErrorMessage("User credentials are not matched please try again!", true);
                                    }
                                }

                            }
                            else
                            {
                                ShowErrorMessage("User credentials are not matched please try again!", true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
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
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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

                var EmailAddress = result.Principal.FindFirst(ClaimTypes.Email).Value;
                var oData = _dbContext.TblAccounts.Where(x => x.UserName.Equals(EmailAddress) && x.RoleId == RoleType.Admin.GetHashCode()).FirstOrDefault();
                if (oData == null)
                {
                    ShowWarningMessage("Your Google account couldn't be found in Estajo." + System.Environment.NewLine + "You can try another Google account or register in Estajo. ", true);
                    return RedirectToAction("Login", "Account");
                }

                TblAccount oUser = new TblAccount();
                oUser.UserName = result.Principal.FindFirst(ClaimTypes.Email).Value;
                oUser.FullName = result.Principal.FindFirst(ClaimTypes.Name).Value;
                oUser.RoleId = RoleType.Admin.GetHashCode();
                oUser.IsOwner = true;
                oUser.Status = true;

                if (!IsUserExist(oUser.UserName))
                {
                    oUser.CreatedDate = DateTime.Now;
                    _dbContext.TblAccounts.Add(oUser);
                    _dbContext.SaveChanges();

                    TblAccountCompany oDataCompany = new TblAccountCompany();
                    oDataCompany.AccountId = oUser.AccountId;
                    oDataCompany.AddedBy = oUser.AccountId;
                    oDataCompany.CreatedDate = DateTime.Now;
                    _dbContext.TblAccountCompanies.Add(oDataCompany);
                    _dbContext.SaveChanges();
                }
                else
                    oUser = _dbContext.TblAccounts.Where(x => x.UserName.Equals(oUser.UserName)).FirstOrDefault();


                if (string.IsNullOrEmpty(oUser.Password))
                {
                    TempData["EmailAddress"] = oUser.UserName;
                    TempData.Keep();
                    return RedirectToAction("SetPassword", "Company");
                }
                else
                {
                    //Here we are storing claims for authentication
                    ClaimsIdentity identity = null;
                    identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, oUser.UserName), new Claim(ClaimTypes.Role, "Company") }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var prinicpal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                    //Redirection of lead user is here. Please give appropreate direction URL to it


                    SetCookie("EmailAddress", oUser.UserName);
                    SetCookie("FullName", oUser.FullName);
                    SetCookie("LoginAccountId", oUser.AccountId.ToString());
                    SetCookie("UserLoginTypeId", RoleType.Admin.GetHashCode().ToString());
                    return RedirectToAction("Index", "Lead");
                }



                #region OldCode

                // Here the following code reperesent that user is sucessfully authenticated by google.
                //We are taking user's information and redirecting user to his environment.
                //TblAgent oAgent = new TblAgent();
                //oAgent.EmailAddress = result.Principal.FindFirst(ClaimTypes.Email).Value;
                //oAgent.FullName = result.Principal.FindFirst(ClaimTypes.Name).Value;
                //oAgent.UserLoginTypeId = UserLoginType.GoogleAccount.GetHashCode();

                //if (!IsUserExist(oAgent.EmailAddress))
                //{
                //    oAgent.IsActive = true;
                //    oAgent.CreatedDate = DateTime.Now;
                //    _dbContext.TblAgents.Add(oAgent);
                //    _dbContext.SaveChanges();
                //}
                //else
                //    oAgent = _dbContext.TblAgents.Where(x => x.EmailAddress.Equals(oAgent.EmailAddress)).SingleOrDefault();

                //if (string.IsNullOrEmpty(oAgent.Password))
                //{
                //    TempData["EmailAddress"] = oAgent.EmailAddress;
                //    TempData.Keep();
                //    return RedirectToAction("SetPassword", "Account");
                //}
                //else
                //{
                //    //Here we are storing claims for authentication
                //    ClaimsIdentity identity = null;
                //    identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, oAgent.EmailAddress), new Claim(ClaimTypes.Role, "Agent") }, CookieAuthenticationDefaults.AuthenticationScheme);
                //    var prinicpal = new ClaimsPrincipal(identity);
                //    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                //    //Redirection of lead user is here. Please give appropreate direction URL to it

                //    SetCookie("EmailAddress", oAgent.EmailAddress);
                //    SetCookie("FullName", oAgent.FullName);
                //    SetCookie("UserLoginTypeId", oAgent.UserLoginTypeId.ToString());

                //    return RedirectToAction("Index", "Lead");
                //}
                #endregion
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                        var oUser = _dbContext.TblAccounts.Where(x => x.UserName.ToLower() == model.Email.ToLower()).FirstOrDefault();
                        if (oUser != null)
                        {
                            //if (oUser.UserLoginTypeId == UserLoginType.GoogleAccount.GetHashCode())
                            //{
                            //    ShowErrorMessage("You have registered with google account. You can't change your password from here!", true);
                            //    return View(model);
                            //}

                            //if (oUser.UserLoginTypeId == UserLoginType.MicrosoftAccount.GetHashCode())
                            //{
                            //    ShowErrorMessage("You have registered with microsoft account. You can't change your password from here!", true);
                            //    return View(model);
                            //}

                            string SmtpUserName = this.Configuration.GetSection("MailSettings")["SmtpUserName"];
                            string SmtpPassword = this.Configuration.GetSection("MailSettings")["SmtpPassword"];
                            int SmtpPort = Convert.ToInt32(this.Configuration.GetSection("MailSettings")["SmtpPort"]);
                            string SmtpServer = this.Configuration.GetSection("MailSettings")["SmtpServer"];
                            string fromEmail = this.Configuration.GetSection("MailSettings")["fromEmail"];
                            bool isSSL = Convert.ToBoolean(this.Configuration.GetSection("MailSettings")["isSSL"]);
                            var password = Encryption.DecryptText(oUser.Password);
                            var body = "<p>Hi,</p>" +
                                        "<p>Your Email Address is:- " + oUser.UserName + "</p>" +
                                        "<p>Your password is:- " + password + "</p><br/>" +
                                        "Thank You.";

                            var subject = "Estajo - ForgotPassword Mail";
                            Utility.sendMail(oUser.UserName, body, subject, fromEmail, SmtpUserName, SmtpPassword, SmtpPort, SmtpServer, isSSL);

                            ModelState.Clear();
                            //ViewBag.sucessMessage = "Forgot Password Mail has been sent successfully. Please check the mail and login again.";
                            ShowSuccessMessage("Forgot Password Mail has been sent successfully. Please check the mail and login again.", true);
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            var resultForAgent = DB.TblAgents.Where(u => u.EmailAddress.Equals(model.Email.Trim()) &&
                                                                  u.IsActive == true).FirstOrDefault();
                            if (resultForAgent != null)
                            {
                                string SmtpUserName = this.Configuration.GetSection("MailSettings")["SmtpUserName"];
                                string SmtpPassword = this.Configuration.GetSection("MailSettings")["SmtpPassword"];
                                int SmtpPort = Convert.ToInt32(this.Configuration.GetSection("MailSettings")["SmtpPort"]);
                                string SmtpServer = this.Configuration.GetSection("MailSettings")["SmtpServer"];
                                string fromEmail = this.Configuration.GetSection("MailSettings")["fromEmail"];
                                bool isSSL = Convert.ToBoolean(this.Configuration.GetSection("MailSettings")["isSSL"]);
                                var password = Encryption.DecryptText(oUser.Password);
                                var body = "<p>Hi,</p>" +
                                            "<p>Your Email Address is:- " + resultForAgent.EmailAddress + "</p>" +
                                            "<p>Your password is:- " + password + "</p><br/>" +
                                            "Thank You.";

                                var subject = "Estajo Agent - ForgotPassword Mail";
                                Utility.sendMail(resultForAgent.EmailAddress, body, subject, fromEmail, SmtpUserName, SmtpPassword, SmtpPort, SmtpServer, isSSL);

                                ModelState.Clear();
                                //ViewBag.sucessMessage = "Forgot Password Mail has been sent successfully. Please check the mail and login again.";
                                ShowSuccessMessage("Forgot Password Mail has been sent successfully. Please check the mail and login again.", true);
                                return RedirectToAction("Login", "Account");
                            }

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
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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

                var EmailAddress = result.Principal.FindFirst(ClaimTypes.Email).Value;
                var oData = _dbContext.TblAccounts.Where(x => x.UserName.Equals(EmailAddress) && x.RoleId == RoleType.Admin.GetHashCode()).FirstOrDefault();
                if (oData == null)
                {
                    ShowWarningMessage("Your Microsoft account couldn't be found in Estajo." + System.Environment.NewLine + "You can try another Microsoft account or register in Estajo. ", true);
                    return RedirectToAction("Login", "Account");
                }

                TblAccount oUser = new TblAccount();
                oUser.UserName = result.Principal.FindFirst(ClaimTypes.Email).Value;
                oUser.FullName = result.Principal.FindFirst(ClaimTypes.Name).Value;
                oUser.RoleId = RoleType.Admin.GetHashCode();
                oUser.IsOwner = true;
                oUser.Status = true;

                if (!IsUserExist(oUser.UserName))
                {
                    oUser.CreatedDate = DateTime.Now;
                    _dbContext.TblAccounts.Add(oUser);
                    _dbContext.SaveChanges();

                    TblAccountCompany oDataCompany = new TblAccountCompany();
                    oDataCompany.AccountId = oUser.AccountId;
                    oDataCompany.AddedBy = oUser.AccountId;
                    oDataCompany.CreatedDate = DateTime.Now;
                    _dbContext.TblAccountCompanies.Add(oDataCompany);
                    _dbContext.SaveChanges();
                }
                else
                    oUser = _dbContext.TblAccounts.Where(x => x.UserName.Equals(oUser.UserName)).FirstOrDefault();

                if (string.IsNullOrEmpty(oUser.Password))
                {
                    TempData["EmailAddress"] = oUser.UserName;
                    TempData.Keep();
                    return RedirectToAction("SetPassword", "Company");
                }
                else
                {
                    //Here we are storing claims for authentication
                    ClaimsIdentity identity = null;
                    identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, oUser.UserName), new Claim(ClaimTypes.Role, "Company") }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var prinicpal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                    //Redirection of lead user is here. Please give appropreate direction URL to it


                    SetCookie("EmailAddress", oUser.UserName);
                    SetCookie("FullName", oUser.FullName);
                    SetCookie("LoginAccountId", oUser.AccountId.ToString());
                    SetCookie("UserLoginTypeId", RoleType.Admin.GetHashCode().ToString());
                    return RedirectToAction("Index", "Lead");
                }

                #region OldCode
                // Here the following code reperesent that user is sucessfully authenticated by google.
                //We are taking user's information and redirecting user to his environment.
                //TblAgent agent = new TblAgent();
                //agent.EmailAddress = result.Principal.FindFirst(ClaimTypes.Email).Value;
                //agent.FullName = result.Principal.FindFirst(ClaimTypes.Name).Value;
                //agent.UserLoginTypeId = UserLoginType.MicrosoftAccount.GetHashCode();
                //if (!IsUserExist(agent.EmailAddress))
                //{
                //    agent.IsActive = true;
                //    agent.CreatedDate = DateTime.Now;
                //    _dbContext.TblAgents.Add(agent);
                //    _dbContext.SaveChanges();
                //}
                //else
                //    agent = _dbContext.TblAgents.Where(x => x.EmailAddress.Equals(agent.EmailAddress)).SingleOrDefault();

                //if (string.IsNullOrEmpty(agent.Password))
                //{
                //    TempData["EmailAddress"] = agent.EmailAddress;
                //    TempData.Keep();
                //    return RedirectToAction("SetPassword", "Account");
                //}
                //else
                //{
                //    //Here we are storing claims for authentication
                //    ClaimsIdentity identity = null;
                //    identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, agent.EmailAddress), new Claim(ClaimTypes.Role, "Agent") }, CookieAuthenticationDefaults.AuthenticationScheme);
                //    var prinicpal = new ClaimsPrincipal(identity);
                //    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                //    //Redirection of lead user is here. Please give appropreate direction URL to it
                //    SetCookie("EmailAddress", agent.EmailAddress);
                //    SetCookie("FullName", agent.FullName);
                //    SetCookie("UserLoginTypeId", agent.UserLoginTypeId.ToString());

                //    return RedirectToAction("Index", "Lead", new { area = "" });
                //}
                #endregion
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }

        }

        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Delete("FullName");
                Response.Cookies.Delete("EmailAddress");
                Response.Cookies.Delete("UserLoginTypeId");
                Response.Cookies.Delete("LoginAccountId");
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return View("Login");
            }
        }

        public IActionResult gettingstarted(AccountIntegrationViewModel model)
        {
            try
            {
                //AuthResponseMicrosoft.refreshTokenMicrosoft(this.Configuration.GetSection("MicrosoftEmailPermission")["MicrosoftClientId"], this.Configuration.GetSection("MicrosoftEmailPermission")["MicrosoftClientSecret"], "M.R3_BAY.-CYU*OTLb*bB5E02FuQWE8eH!x2c9YnijegHItUdUwGKAkq!glywkGeatDSJDZD3bEGuLUomu1IVJG8sLZQKqgQ3hZeaz2yiWltW!cXxQnhcXSd8QxuMNK!pEAZPBN!xhBc7gHfLEQCgv!DqQHxYC1ySkkdOcOgFMZUdsbgP5p6QcZ3l99qdGrqrjvRm644tv8zxnYIIQ0Qzukua3ydx5ctqUaHUpoIO8tBMqCqMatVUZOShjyG1lUR87b96v3ketEZ2vN1!acZGc6*YncgdjvB4f3NYUP98K3CKvZ5pXybpZykRESyyFR!XUQ6Gn73vxUC1rh21cZzNc45krFvQqHUg$");

                if (Request.Cookies.ContainsKey("FullName") && Request.Cookies.ContainsKey("EmailAddress"))
                {
                    int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                    var AgentName = Request.Cookies["FullName"].ToString();
                    ViewBag.AgentName = AgentName;

                    TblAccountIntegration oData = _dbContext.TblAccountIntegrations.Where(x => x.AccountId == AccountId).FirstOrDefault();
                    if (oData != null)
                    {
                        model.AuthAccountType = oData.AuthAccountType.Value;
                        model.EmailAddress = oData.EmailAddress;
                    }
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }
        
        }

        public bool IsUserExist(string email)
        {
            try
            {
                var result = _dbContext.TblAccounts.Where(u => u.UserName.ToLower().Equals(email.ToLower())).ToList();
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

        public void SetCookie(string key, string value, int? expireTimeInDays = 0)
        {
            CookieOptions option = new CookieOptions();

            if (expireTimeInDays > 0)
                option.Expires = DateTime.Now.AddMinutes(expireTimeInDays.Value);
            else
                option.Expires = DateTime.Now.AddMinutes(15); //minimum 15 Minutes

            Response.Cookies.Append(key, value, option);
        }

        #region AgentConnectWithGoogle
        public IActionResult ConnectWithGoogle()
        {
            string url = AuthResponse.GetAutenticationURI(this.Configuration.GetSection("Google")["GoogleClientId"], $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/Account/authorize/", this.Configuration.GetSection("Google")["Scope"]);
            return Redirect(url);
        }

        public IActionResult authorize(string code)
        {
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);

                if (code != null)
                {
                    AuthResponse access = AuthResponse.Exchange(code, this.Configuration.GetSection("Google")["GoogleClientId"], this.Configuration.GetSection("Google")["GoogleClientSecret"], $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/Account/authorize/");
                    if (access != null)
                    {
                        //TblAccountIntegration oData = new TblAccountIntegration();
                        //oData.AccountId = AccountId;
                        //oData.AuthAccountType = AuthAccountType.GoogleAuth.GetHashCode();
                        //oData.ExpiresIn = Convert.ToInt32(access.expires_in);
                        //oData.RefreshToken = access.refresh_token;
                        //oData.CreatedDate = DateTime.Now;
                        //_dbContext.TblAccountIntegrations.Add(oData);
                        //_dbContext.SaveChanges();


                       
                        var EmailRequest = "https://www.googleapis.com/oauth2/v2/userinfo?access_token=" + access.Access_token;
                        // Create a request for the URL.
                        var webRequest = WebRequest.Create(EmailRequest);
                        // Get the response.
                        var Response = (HttpWebResponse)webRequest.GetResponse();
                        // Get the stream containing content returned by the server.
                        var DataStream = Response.GetResponseStream();
                        // Open the stream using a StreamReader for easy access.
                        var Reader = new StreamReader(DataStream);
                        // Read the content.
                        var JsonString = Reader.ReadToEnd();
                        // Cleanup the streams and the response.
                        Reader.Close();
                        DataStream.Close();
                        Response.Close();
                        RootobjectGmail result = JsonConvert.DeserializeObject<RootobjectGmail>(JsonString);
                        if (result != null)
                        {
                            TblAccountIntegration oData = _dbContext.TblAccountIntegrations.Where(x => x.AccountId == AccountId && x.AuthAccountType == AuthAccountType.GoogleAuth.GetHashCode()).FirstOrDefault();
                            if (oData != null)
                            {
                                oData.AccountId = AccountId;
                                oData.AuthAccountType = AuthAccountType.GoogleAuth.GetHashCode();
                                oData.EmailAddress = result.email;
                                oData.Name = result.name;
                                oData.ExpiresIn = Convert.ToInt32(access.expires_in);
                                oData.AccessToken = access.Access_token;
                                oData.RefreshToken = access.refresh_token;
                                oData.CreatedDate = DateTime.Now;
                                _dbContext.TblAccountIntegrations.Add(oData);
                                _dbContext.SaveChanges();
                            }
                            else
                            {
                                oData = new TblAccountIntegration();
                                oData.AccountId = AccountId;
                                oData.AuthAccountType = AuthAccountType.GoogleAuth.GetHashCode();
                                oData.EmailAddress = result.email;
                                oData.Name = result.name;
                                oData.ExpiresIn = Convert.ToInt32(access.expires_in);
                                oData.AccessToken = access.Access_token;
                                oData.RefreshToken = access.refresh_token;
                                oData.CreatedDate = DateTime.Now;
                                _dbContext.TblAccountIntegrations.Add(oData);
                                _dbContext.SaveChanges();
                            }

                            TblAccount oAccount = _dbContext.TblAccounts.Where(x => x.AccountId == AccountId && x.RoleId == RoleType.Agent.GetHashCode()).FirstOrDefault();
                            if (oAccount != null)
                            {
                                oAccount.IsEmailConfig = true;
                                _dbContext.SaveChanges();
                                //TempData["oAuth"] = "Google";
                                //TempData["IsEmailConfig"] = true;
                            }

                            //TblAccount oAccount = _dbContext.TblAccounts.Where(x => x.AccountId == AccountId && x.RoleId == RoleType.Agent.GetHashCode()).FirstOrDefault();
                            //if (oAccount != null)
                            //{
                            //    oAccount.IsEmailConfig = true;
                            //    _dbContext.SaveChanges();
                            //    //TempData["oAuth"] = "Google";
                            //    //TempData["IsEmailConfig"] = true;
                            //}
                        }
                        return RedirectToAction("gettingstarted", "Account");
                    }
                    else
                    {
                        ShowErrorMessage("Something went wrong in connecting with google. Please try again!");
                        return RedirectToAction("gettingstarted", "Account");
                    }
                }
                else
                {
                    ShowErrorMessage("You cancelled the current process Please try again!");
                    return RedirectToAction("gettingstarted", "Account");
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }

        }

        public IActionResult ConnectWithMicrosoft()
        {
            int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
            string url = AuthResponseMicrosoft.GetAutenticationURIMicrosoft(this.Configuration.GetSection("MicrosoftEmailPermission")["MicrosoftClientId"], $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/Account/authorizeMicrosoft/", this.Configuration.GetSection("MicrosoftEmailPermission")["Scope"], AccountId);
            return Redirect(url);
        }


        public IActionResult authorizeMicrosoft(string code, int state)
        {
            try
            {
                if (code != null)
                {
                    AuthResponseMicrosoft access = AuthResponseMicrosoft.ExchangeMicrosoft(code, this.Configuration.GetSection("MicrosoftEmailPermission")["MicrosoftClientId"], this.Configuration.GetSection("MicrosoftEmailPermission")["MicrosoftClientSecret"], $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/Account/authorizeMicrosoft/");

                    if (access != null)
                    {
                      
                        const string WEBSERVICE_URL = "https://graph.microsoft.com/v1.0/me";
                        try
                        {
                            var webRequest = System.Net.WebRequest.Create(WEBSERVICE_URL);
                            if (webRequest != null)
                            {
                                webRequest.Method = "GET";
                                //webRequest.Timeout = 20000;
                                webRequest.ContentType = "application/json";
                                webRequest.Headers.Add("Authorization", "Bearer " + access.access_token);
                                using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                                {
                                    using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                                    {
                                        var jsonResponse = sr.ReadToEnd();
                                        RootobjectMicrosoft result = JsonConvert.DeserializeObject<RootobjectMicrosoft>(jsonResponse);
                                        if (result.userPrincipalName != null)
                                        {
                                            TblAccountIntegration oData = _dbContext.TblAccountIntegrations.Where(x => x.AccountId == state && x.AuthAccountType == AuthAccountType.MicrosoftAuth.GetHashCode()).FirstOrDefault();
                                            if (oData != null)
                                            {
                                                oData.AccountId = state;
                                                oData.AuthAccountType = AuthAccountType.MicrosoftAuth.GetHashCode();
                                                oData.EmailAddress = result.userPrincipalName;
                                                oData.Name = result.displayName;
                                                oData.ExpiresIn = Convert.ToInt32(access.expires_in);
                                                oData.AccessToken = access.access_token;
                                                oData.RefreshToken = access.refresh_token;
                                                oData.CreatedDate = DateTime.Now;
                                                _dbContext.TblAccountIntegrations.Add(oData);
                                                _dbContext.SaveChanges();
                                            }
                                            else
                                            {
                                                oData = new TblAccountIntegration();
                                                oData.AccountId = state;
                                                oData.AuthAccountType = AuthAccountType.MicrosoftAuth.GetHashCode();
                                                oData.EmailAddress = result.userPrincipalName;
                                                oData.Name = result.displayName;
                                                oData.ExpiresIn = Convert.ToInt32(access.expires_in);
                                                oData.AccessToken = access.access_token;
                                                oData.RefreshToken = access.refresh_token;
                                                oData.CreatedDate = DateTime.Now;
                                                _dbContext.TblAccountIntegrations.Add(oData);
                                                _dbContext.SaveChanges();
                                            }


                                            TblAccount oAccount = _dbContext.TblAccounts.Where(x => x.AccountId == state && x.RoleId == RoleType.Agent.GetHashCode()).FirstOrDefault();
                                            if (oAccount != null)
                                            {
                                                oAccount.IsEmailConfig = true;
                                                _dbContext.SaveChanges();
                                                //TempData["oAuth"] = "Google";
                                                //TempData["IsEmailConfig"] = true;
                                            }

                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                            ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                        }
                        return RedirectToAction("gettingstarted", "Account");
                    }
                    else
                    {
                        ShowErrorMessage("Something went wrong in connecting with microsoft. Please try again!");
                        return RedirectToAction("gettingstarted", "Account");
                    }
                }
                else
                {
                    ShowErrorMessage("You cancelled the current process Please try again!");
                    return RedirectToAction("gettingstarted", "Account");
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }

        }


        public IActionResult DisconnectAccount(int AuthAccountType)
        {
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                if (AccountId > 0)
                {
                    TblAccount oAccount = _dbContext.TblAccounts.Where(x => x.AccountId == AccountId && x.RoleId == RoleType.Agent.GetHashCode()).FirstOrDefault();
                    if (oAccount != null)
                    {
                        oAccount.IsEmailConfig = false;
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        ShowErrorMessage("Something went wrong. Please try again!");
                    }

                    TblAccountIntegration oAccountIntegration = _dbContext.TblAccountIntegrations.Where(x => x.AccountId == AccountId && x.AuthAccountType == AuthAccountType).FirstOrDefault();
                    if (oAccountIntegration != null)
                    {
                        _dbContext.TblAccountIntegrations.Remove(oAccountIntegration);
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        ShowErrorMessage("Something went wrong. Please try again!");
                    }
                }
                return RedirectToAction("gettingstarted", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }
        }
        #endregion
    }
}
