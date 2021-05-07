using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RealEstateDB;
using RealEstate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Utills;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;

namespace RealEstate.Controllers
{
    public class CompanyController : BaseController
    {
        private RealEstateContext _dbContext;
        //private readonly ApplicationDbContext _context;
        private IConfiguration Configuration;

        public CompanyController(RealEstateContext context, IConfiguration _configuration)
        {
            _dbContext = context;
            Configuration = _configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult signup(UserViewModel model)
        {
            try
            {
                ModelState.Remove("Address");
                ModelState.Remove("City");
                ModelState.Remove("ZipCode");
                if (ModelState.IsValid)
                {
                    using (var DB = _dbContext)
                    {
                        TblAccount oData = new TblAccount();
                        if (!IsCompanyExist(model.EmailAddress))
                        {
                            oData.FullName = string.IsNullOrEmpty(model.FullName) ? string.Empty : model.FullName;
                            oData.PhoneNumber = string.IsNullOrEmpty(model.CellPhone) ? string.Empty : model.CellPhone;
                            oData.UserName = string.IsNullOrEmpty(model.EmailAddress) ? string.Empty : model.EmailAddress;
                            oData.Password = string.IsNullOrEmpty(model.Password) ? string.Empty : Encryption.EncryptText(model.Password);
                            oData.IsOwner = true;
                            oData.CreatedDate = DateTime.Now;
                            oData.RoleId = RoleType.Admin.GetHashCode();
                            oData.Status = true;
                            DB.TblAccounts.Add(oData);
                            DB.SaveChanges();


                            TblAccountCompany oDataCompany = new TblAccountCompany();
                            oDataCompany.AccountId = oData.AccountId;
                            oDataCompany.CreatedDate = DateTime.Now;
                            DB.TblAccountCompanies.Add(oDataCompany);
                            DB.SaveChanges();


                            SetCookie("EmailAddress", oData.UserName);
                            SetCookie("FullName", oData.FullName);
                            SetCookie("UserId", oData.AccountId.ToString());
                            SetCookie("UserLoginTypeId", RoleType.Admin.GetHashCode().ToString());
                            //SetCookie("UserLoginTypeId", UserLoginType.Company.GetHashCode().ToString());
                            return RedirectToAction("Index", "Lead", new { area = "" });
                        }
                        else
                        {
                            ShowWarningMessage("EmailAddress already taken.", true);
                            return View(model);
                        }
                    }
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }

        }

        #region SignupWithGoogle
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
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
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
                TblAccount oUser = new TblAccount();
                oUser.UserName = result.Principal.FindFirst(ClaimTypes.Email).Value;
                oUser.FullName = result.Principal.FindFirst(ClaimTypes.Name).Value;
                oUser.RoleId = RoleType.Admin.GetHashCode();
                oUser.IsOwner = true;
                oUser.Status = true;

                if (!IsCompanyExist(oUser.UserName))
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
                    //SetCookie("EmailAddress", oCompany.Email);
                    //SetCookie("FullName", oCompany.FullName);
                    //SetCookie("LoginAccountId", oCompany.CompanyId.ToString());
                    //SetCookie("UserLoginTypeId", UserLoginType.Company.GetHashCode().ToString());
                    return RedirectToAction("Index", "Lead");
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }
        }
        #endregion

        #region SignupWithMicosoft
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
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
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
                TblAccount oUser = new TblAccount();
                oUser.UserName = result.Principal.FindFirst(ClaimTypes.Email).Value;
                oUser.FullName = result.Principal.FindFirst(ClaimTypes.Name).Value;
                oUser.RoleId = RoleType.Admin.GetHashCode();
                oUser.IsOwner = true;
                oUser.Status = true;

                if (!IsCompanyExist(oUser.UserName))
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
                    //SetCookie("EmailAddress", oCompany.Email);
                    //SetCookie("FullName", oCompany.FullName);
                    //SetCookie("LoginAccountId", oCompany.CompanyId.ToString());
                    //SetCookie("UserLoginTypeId", UserLoginType.Company.GetHashCode().ToString());
                    return RedirectToAction("Index", "Lead");
                }

            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }

        }
        #endregion

        #region SetPassword
        public IActionResult SetPassword()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }

        }
        [HttpPost]
        public IActionResult SetPassword(SetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var DB = _dbContext)
                    {
                        TblAccount oUser = _dbContext.TblAccounts.Where(x => x.UserName.Equals(model.EmailAddress)).SingleOrDefault();
                        oUser.Password = Encryption.EncryptText(model.ConfirmPassword);
                        _dbContext.Entry(oUser).State = EntityState.Modified;
                        _dbContext.SaveChanges();

                        ClaimsIdentity identity = null;
                        identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, oUser.UserName), new Claim(ClaimTypes.Role, "Company") }, CookieAuthenticationDefaults.AuthenticationScheme);
                        var prinicpal = new ClaimsPrincipal(identity);
                        var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);

                        SetCookie("EmailAddress", oUser.UserName);
                        SetCookie("FullName", oUser.FullName);
                        SetCookie("LoginAccountId", oUser.AccountId.ToString());
                        SetCookie("UserLoginTypeId", RoleType.Admin.GetHashCode().ToString());
                        if (model.KeepMeSigninIn)
                            return RedirectToAction("Index", "Lead", new { area = "" });
                        else
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
                return View(model);
            }
        }
        #endregion

        #region CompanySetting
        [HttpGet]
        public IActionResult CompanySetting()
        {
            try
            {
                CompanyViewModel model = new CompanyViewModel();
                if (ModelState.IsValid)
                {
                    using (var DB = _dbContext)
                    {
                        if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                            return RedirectToAction("Login", "Account");

                        int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);

                        var oCompanyDetails = DB.TblAccountCompanies.Where(x => x.AccountId == AccountId && x.Account.RoleId == RoleType.Admin.GetHashCode()).Include(x => x.Account).FirstOrDefault();
                        if (oCompanyDetails != null)
                        {
                            model.FullName = oCompanyDetails.Account.FullName;
                            model.EmailAddress = oCompanyDetails.Account.UserName;
                            model.CellPhone = oCompanyDetails.Account.PhoneNumber;
                            model.Address = oCompanyDetails.Address;
                            model.City = oCompanyDetails.City;
                            model.State = oCompanyDetails.State;
                            model.ZipCode = oCompanyDetails.ZipCode;
                            model.Country = oCompanyDetails.Country;
                            model.UpdatedDate = DateTime.Now;
                            _dbContext.Entry(oCompanyDetails).State = EntityState.Modified;
                            _dbContext.SaveChanges();
                            return View(model);
                        }
                        else
                        {
                            ShowErrorMessage("Company Details not found", true);
                            return View(model);
                        }
                    }
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult CompanySetting(CompanyViewModel model)
        {
            try
            {
                ModelState.Remove("Password");
                if (ModelState.IsValid)
                {
                    using (var DB = _dbContext)
                    {
                        if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                            return RedirectToAction("Login", "Account");

                        int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);

                        var oCompanyDetails = DB.TblAccountCompanies.Where(x => x.AccountId == AccountId && x.Account.RoleId == RoleType.Admin.GetHashCode()).Include(x => x.Account).FirstOrDefault();
                        if (oCompanyDetails != null)
                        {
                            oCompanyDetails.Account.FullName = model.FullName;
                            oCompanyDetails.Account.UserName = model.EmailAddress;
                            oCompanyDetails.Account.PhoneNumber = model.CellPhone;
                            oCompanyDetails.Address = model.Address;
                            oCompanyDetails.City = model.City;
                            oCompanyDetails.State = model.State;
                            oCompanyDetails.ZipCode = model.ZipCode;
                            oCompanyDetails.Country = model.Country;
                            oCompanyDetails.AccountId = AccountId;
                            oCompanyDetails.AddedBy = AccountId;
                            oCompanyDetails.UpdatedDate = DateTime.Now;
                            _dbContext.Entry(oCompanyDetails).State = EntityState.Modified;
                            _dbContext.SaveChanges();
                            ShowSuccessMessage("Company Details updated successfully.", true);
                            return View("CompanySetting");
                        }
                        else
                        {
                            ShowErrorMessage("Company Details not found", true);
                            return View(model);
                            //TblAccountCompany oData = new TblAccountCompany();
                            //oData.AccountId = AccountId;
                            //oData.AddedBy = AccountId;
                            //oData.Account.PhoneNumber = model.CellPhone;
                            //oData.Address = model.Address;
                            //oData.City = model.City;
                            //oData.State = model.State;
                            //oData.ZipCode = model.ZipCode;
                            //oData.Country = model.Country;
                            ////oData.CreatedDate = DateTime.Now;
                            //oData.UpdatedDate = DateTime.Now;
                            //_dbContext.TblAccountCompanies.Add(oData);
                            //_dbContext.SaveChanges();
                            //ShowSuccessMessage("Company Details updated successfully.", true);
                            //return View("CompanySetting");
                        }
                    }
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }
        }
        #endregion

        #region Function
        public bool IsCompanyExist(string email)
        {
            try
            {
                var result = _dbContext.TblAccounts.Where(u => u.UserName.ToLower().Equals(email.ToLower()) && u.RoleId == RoleType.Admin.GetHashCode()).ToList();
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
        #endregion

    }
}
