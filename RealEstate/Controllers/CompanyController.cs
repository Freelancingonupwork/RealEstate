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
        public IActionResult signup(CompanyViewModel model)
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
                        TblCompany oData = new TblCompany();
                        if (!IsCompanyExist(model.EmailAddress))
                        {
                            oData.FullName = string.IsNullOrEmpty(model.FullName) ? string.Empty : model.FullName;
                            oData.CellPhone = string.IsNullOrEmpty(model.CellPhone) ? string.Empty : model.CellPhone;
                            oData.Email = string.IsNullOrEmpty(model.EmailAddress) ? string.Empty : model.EmailAddress;
                            oData.Password = string.IsNullOrEmpty(model.Password) ? string.Empty : Encryption.EncryptText(model.Password);
                            oData.LogionTypeId = UserLoginType.Company.GetHashCode();
                            oData.CreatedDate = DateTime.Now;
                            oData.IsCompany = true;
                            DB.TblCompanies.Add(oData);
                            DB.SaveChanges();

                            SetCookie("EmailAddress", oData.Email);
                            SetCookie("FullName", oData.FullName);
                            SetCookie("LoginCompanyId", oData.CompanyId.ToString());
                            SetCookie("UserLoginTypeId", UserLoginType.Company.GetHashCode().ToString());
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
                TblCompany oCompany = new TblCompany();
                oCompany.Email = result.Principal.FindFirst(ClaimTypes.Email).Value;
                oCompany.FullName = result.Principal.FindFirst(ClaimTypes.Name).Value;
                oCompany.LogionTypeId = UserLoginType.GoogleAccount.GetHashCode();
                oCompany.IsCompany = true;

                if (!IsCompanyExist(oCompany.Email))
                {
                    oCompany.CreatedDate = DateTime.Now;
                    _dbContext.TblCompanies.Add(oCompany);
                    _dbContext.SaveChanges();
                }
                else
                    oCompany = _dbContext.TblCompanies.Where(x => x.Email.Equals(oCompany.Email)).FirstOrDefault();

                if (string.IsNullOrEmpty(oCompany.Password))
                {
                    TempData["EmailAddress"] = oCompany.Email;
                    TempData.Keep();
                    return RedirectToAction("SetPassword", "Company");
                }
                else
                {
                    //Here we are storing claims for authentication
                    ClaimsIdentity identity = null;
                    identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, oCompany.Email), new Claim(ClaimTypes.Role, "Company") }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var prinicpal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                    //Redirection of lead user is here. Please give appropreate direction URL to it

                    SetCookie("EmailAddress", oCompany.Email);
                    SetCookie("FullName", oCompany.FullName);
                    SetCookie("LoginCompanyId", oCompany.CompanyId.ToString());
                    SetCookie("UserLoginTypeId", UserLoginType.Company.GetHashCode().ToString());
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
                TblCompany oCompany = new TblCompany();
                oCompany.Email = result.Principal.FindFirst(ClaimTypes.Email).Value;
                oCompany.FullName = result.Principal.FindFirst(ClaimTypes.Name).Value;
                oCompany.LogionTypeId = UserLoginType.MicrosoftAccount.GetHashCode();
                oCompany.IsCompany = true;

                if (!IsCompanyExist(oCompany.Email))
                {
                    oCompany.CreatedDate = DateTime.Now;
                    _dbContext.TblCompanies.Add(oCompany);
                    _dbContext.SaveChanges();
                }
                else
                    oCompany = _dbContext.TblCompanies.Where(x => x.Email.Equals(oCompany.Email)).FirstOrDefault();

                if (string.IsNullOrEmpty(oCompany.Password))
                {
                    TempData["EmailAddress"] = oCompany.Email;
                    TempData.Keep();
                    return RedirectToAction("SetPassword", "Company");
                }
                else
                {
                    //Here we are storing claims for authentication
                    ClaimsIdentity identity = null;
                    identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, oCompany.Email), new Claim(ClaimTypes.Role, "Company") }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var prinicpal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);
                    //Redirection of lead user is here. Please give appropreate direction URL to it
                    SetCookie("EmailAddress", oCompany.Email);
                    SetCookie("FullName", oCompany.FullName);
                    SetCookie("LoginCompanyId", oCompany.CompanyId.ToString());
                    SetCookie("UserLoginTypeId", UserLoginType.Company.GetHashCode().ToString());
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
                        TblCompany oCompany = _dbContext.TblCompanies.Where(x => x.Email.Equals(model.EmailAddress) && x.IsCompany == true).SingleOrDefault();
                        oCompany.Password = Encryption.EncryptText(model.ConfirmPassword);
                        _dbContext.Entry(oCompany).State = EntityState.Modified;
                        _dbContext.SaveChanges();

                        ClaimsIdentity identity = null;
                        identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, oCompany.Email), new Claim(ClaimTypes.Role, "Company") }, CookieAuthenticationDefaults.AuthenticationScheme);
                        var prinicpal = new ClaimsPrincipal(identity);
                        var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicpal);

                        SetCookie("EmailAddress", oCompany.Email);
                        SetCookie("FullName", oCompany.FullName);
                        SetCookie("LoginCompanyId", oCompany.CompanyId.ToString());
                        SetCookie("UserLoginTypeId", UserLoginType.Company.GetHashCode().ToString());
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

                        int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);

                        var oCompanyDetails = DB.TblCompanies.Where(x => x.CompanyId == CompanyId && x.IsCompany == true).FirstOrDefault();
                        if (oCompanyDetails != null)
                        {
                            model.FullName = oCompanyDetails.FullName;
                            model.EmailAddress = oCompanyDetails.Email;
                            model.CellPhone = oCompanyDetails.CellPhone;
                            model.Address = oCompanyDetails.Address;
                            model.City = oCompanyDetails.City;
                            model.State = oCompanyDetails.State;
                            model.ZipCode = oCompanyDetails.ZipCode;
                            model.Country = oCompanyDetails.Country;
                            model.UpdatedDate = DateTime.Now;
                            //_dbContext.Entry(oCompanyDetails).State = EntityState.Modified;
                            //_dbContext.SaveChanges();
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
                        int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);

                        var oCompanyDetails = DB.TblCompanies.Where(x => x.CompanyId == CompanyId && x.IsCompany == true).FirstOrDefault();
                        if (oCompanyDetails != null)
                        {
                            oCompanyDetails.FullName = model.FullName;
                            oCompanyDetails.Email = model.EmailAddress;
                            oCompanyDetails.CellPhone = model.CellPhone;
                            oCompanyDetails.Address = model.Address;
                            oCompanyDetails.City = model.City;
                            oCompanyDetails.State = model.State;
                            oCompanyDetails.ZipCode = model.ZipCode;
                            oCompanyDetails.Country = model.Country;
                            oCompanyDetails.UpdatedDate = DateTime.Now;
                            _dbContext.Entry(oCompanyDetails).State = EntityState.Modified;
                            _dbContext.SaveChanges();
                            ShowSuccessMessage("Company Details updated successfully.", true);
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
        #endregion

        #region Function
        public bool IsCompanyExist(string email)
        {
            try
            {
                var result = _dbContext.TblCompanies.Where(u => u.Email.ToLower().Equals(email.ToLower()) && u.IsCompany == true).ToList();
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
