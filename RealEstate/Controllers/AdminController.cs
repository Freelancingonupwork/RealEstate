using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RealEstate.Models;
using RealEstate.Utills;
using RealEstateDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Controllers
{

    [Authorize(Roles = "Company")]
    public class AdminController : BaseController
    {
        private RealEstateContext _dbContext;
        private IConfiguration Configuration;
        public AdminController(RealEstateContext dbContext, IConfiguration _configuration)
        {
            _dbContext = dbContext;
            Configuration = _configuration;
        }

        #region Agent/Team
        [Route("Admin/")]
        public IActionResult LeadFlow()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                    return View();
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Team()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                {
                    using (var DB = _dbContext)
                    {
                        int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                        var oAgentList = DB.TblAgents.Where(x => x.CompanyId == CompanyId).ToList().Select(s => new AgentViewModel
                        {
                            AgentId = s.Id,
                            FullName = s.FullName,
                            EmailAddress = s.EmailAddress,
                            CellPhone = s.CellPhone,
                            IsActive = (bool)s.IsActive,
                            CreatedDate = (DateTime)s.CreatedDate
                        });
                        return View(oAgentList);
                    }
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }

        }

        [HttpGet]
        public IActionResult AddAgents()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                    return View();
                else
                    return RedirectToAction("Login", "Account");
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
        public IActionResult AddAgents(AgentViewModel model)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                if (ModelState.IsValid)
                {
                    if (IsUserExist(model.EmailAddress))
                    {
                        //ViewBag.MessageType = "danger";
                        //ViewBag.Message = "Email is already in used!";
                        ShowErrorMessage("Email is already used!", true);
                        return View(model);
                    }
                    TblAgent oData = new TblAgent();
                    oData.FullName = string.IsNullOrEmpty(model.FullName) ? string.Empty : model.FullName;
                    oData.EmailAddress = string.IsNullOrEmpty(model.EmailAddress) ? string.Empty : model.EmailAddress;
                    oData.CellPhone = string.IsNullOrEmpty(model.CellPhone) ? string.Empty : model.CellPhone;
                    oData.UserLoginTypeId = UserLoginType.Manual.GetHashCode();
                    oData.IsActive = model.IsActive;
                    oData.CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                    oData.CreatedDate = DateTime.Now;

                    _dbContext.TblAgents.Add(oData);
                    _dbContext.SaveChanges();

                    #region SendMail
                    string SmtpUserName = this.Configuration.GetSection("MailSettings")["SmtpUserName"];
                    string SmtpPassword = this.Configuration.GetSection("MailSettings")["SmtpPassword"];
                    int SmtpPort = Convert.ToInt32(this.Configuration.GetSection("MailSettings")["SmtpPort"]);
                    string SmtpServer = this.Configuration.GetSection("MailSettings")["SmtpServer"];
                    string fromEmail = this.Configuration.GetSection("MailSettings")["fromEmail"];
                    bool isSSL = Convert.ToBoolean(this.Configuration.GetSection("MailSettings")["isSSL"]);
                    //var password = Encryption.DecryptText(oData.Password);
                    var body = "<p>Hi,</p>" +
                                "<p>Your Agent Email Address is:- " + oData.EmailAddress + "</p><br/>" +
                                //"<p>Your password is:- " + password + "</p><br/>" +
                                "Thank You.";

                    var subject = "Estajo - Agent Details";
                    Utility.sendMail(oData.EmailAddress, body, subject, fromEmail, SmtpUserName, SmtpPassword, SmtpPort, SmtpServer, isSSL);

                    #endregion

                    ShowSuccessMessage("User added Successfully.", true);
                    return RedirectToAction("Team", "Admin");
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.log("AdminController AddAgents" + ex);
                ViewBag.MessageType = "danger";
                ViewBag.Message = "Opps! Something went wrong while inserting user! Please Try again later.";
                ShowErrorMessage("Opps! Something went wrong while inserting user! Please Try again later.", true);
                return View(model);
            }
        }

        public IActionResult ActivateDeactivateAgent()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");
            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                int Flag = Convert.ToInt32(Request.Form["flag"]);
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                TblAgent agent = _dbContext.TblAgents.SingleOrDefault(c => c.Id == id && c.CompanyId == CompanyId);

                if (Flag >= 1)
                    agent.IsActive = true;
                else
                    agent.IsActive = false;
                _dbContext.Entry(agent).State = EntityState.Modified;
                _dbContext.SaveChanges();
                if (Flag >= 1)
                    return Json(new { Act = true, message = "User is activated." });
                else
                    return Json(new { DeAct = true, message = "User is de-activated." });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { Error = false });
            }
        }

        public IActionResult DeleteAgent()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int id = 0;
                if (Request.Form.ContainsKey("id")) { id = Convert.ToInt32(Request.Form["id"]); }
                if (id <= 0) { throw new Exception("Identity can not be blank!"); }
                _dbContext.TblAgents.Remove(_dbContext.TblAgents.Where(x => x.Id == id && x.CompanyId == CompanyId).FirstOrDefault());
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }

        public IActionResult UpdateAgent()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int intAgentID = Convert.ToInt32(Request.Form["agentID"]);
                int intLeadID = Convert.ToInt32(Request.Form["leadID"]);
                TblLead lead = _dbContext.TblLeads.Where(x => x.LeadId == intLeadID && x.CompanyId == CompanyId).FirstOrDefault();
                lead.AgentId = intAgentID <= 0 ? Convert.ToInt32(DBNull.Value) : intAgentID;
                _dbContext.Entry(lead).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                //ErrorLog.log("AdminController UpdateAgent" + ex);
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public IActionResult GetAgentDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int id = Convert.ToInt32(Request.Form["agentID"]);
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                return Json(_dbContext.TblAgents.Where(x => x.Id == id && x.CompanyId == CompanyId).Select(x => new
                {
                    success = true,
                    agentID = x.Id,
                    fullName = x.FullName,
                    emailAddress = x.EmailAddress,
                    cellPhone = x.CellPhone,
                    status = x.IsActive == true ? 1 : 0
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public IActionResult UpdateAgentDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);

                int intAgentID = Convert.ToInt32(Request.Form["agentID"]);
                string strfullName = Request.Form["fullName"];

                string stremailAddress = Request.Form["emailAddress"];
                string strcellPhone = Request.Form["cellPhone"];

                if (_dbContext.TblAgents.Where(x => x.EmailAddress.Equals(stremailAddress) && x.Id != intAgentID).FirstOrDefault() != null)
                    return Json(new { success = false, message = "Email address is already in use! Try another email address!" });

                TblAgent oAgent = _dbContext.TblAgents.Where(x => x.Id == intAgentID && x.CompanyId == CompanyId).FirstOrDefault();
                if (oAgent != null)
                {
                    oAgent.FullName = strfullName;
                    oAgent.EmailAddress = stremailAddress;
                    oAgent.CellPhone = strcellPhone;
                    oAgent.UserLoginTypeId = UserLoginType.Manual.GetHashCode();
                    oAgent.CreatedDate = DateTime.Now;
                    _dbContext.Entry(oAgent).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No Agent found." });
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        #endregion

        #region Function
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

        public bool IsUserAuthorize()
        {
            try
            {
                if (Request.Cookies.ContainsKey("UserLoginTypeId"))
                {
                    //if (Convert.ToInt32(Request.Cookies["UserLoginTypeId"]) != UserLoginType.Admin.GetHashCode())
                    if (Convert.ToInt32(Request.Cookies["UserLoginTypeId"]) != UserLoginType.Company.GetHashCode())
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return false;
            }

        }

        public bool IsAdminExist(string email)
        {
            try
            {
                var result = _dbContext.TblUsers.Where(u => u.EmailAddress.ToLower().Equals(email.ToLower()) && u.UserLoginTypeId == UserLoginType.Admin.GetHashCode()).ToList();
                if (result.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                ErrorLog.log("Account Controller IsAdminExist:-" + ex);
                return false;
            }
        }
        #endregion

        #region AdminUser
        public IActionResult AdminUser()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                {
                    using (var DB = _dbContext)
                    {
                        int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                        var oAdminUserList = DB.TblUsers.Where(x => x.UserLoginTypeId == (int)UserLoginType.Admin && x.CompanyId == CompanyId).ToList().Select(s => new UserViewModel
                        {
                            UserId = s.UserId,
                            FullName = s.FullName,
                            EmailAddress = s.EmailAddress,
                            IsActive = (bool)s.IsActive,
                            CreatedDate = (DateTime)s.CreatedDate
                        });
                        return View(oAdminUserList);
                    }
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ErrorLog.log("Admin Controller AdminUser :- " + ex);
                return null;
            }
        }

        [HttpGet]
        public IActionResult AddAdminUser()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                    return View();
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ErrorLog.log("Admin AddAdminUser LeadFlow :-" + ex.Message);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult AddAdminUser(UserViewModel model)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                if (ModelState.IsValid)
                {
                    int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);

                    if (IsAdminExist(model.EmailAddress))
                    {
                        ShowErrorMessage("Email is already used!", true);
                        return View(model);
                    }
                    TblUser oData = new TblUser();
                    oData.FullName = string.IsNullOrEmpty(model.FullName) ? string.Empty : model.FullName;
                    oData.EmailAddress = string.IsNullOrEmpty(model.EmailAddress) ? string.Empty : model.EmailAddress;
                    oData.Password = string.IsNullOrEmpty(model.Password) ? string.Empty : Encryption.EncryptText(model.Password);
                    oData.IsActive = model.IsActive;
                    oData.CreatedDate = DateTime.Now;
                    oData.UserLoginTypeId = UserLoginType.Admin.GetHashCode();
                    oData.CompanyId = CompanyId;
                    _dbContext.TblUsers.Add(oData);
                    _dbContext.SaveChanges();


                    #region SendMail
                    string SmtpUserName = this.Configuration.GetSection("MailSettings")["SmtpUserName"];
                    string SmtpPassword = this.Configuration.GetSection("MailSettings")["SmtpPassword"];
                    int SmtpPort = Convert.ToInt32(this.Configuration.GetSection("MailSettings")["SmtpPort"]);
                    string SmtpServer = this.Configuration.GetSection("MailSettings")["SmtpServer"];
                    string fromEmail = this.Configuration.GetSection("MailSettings")["fromEmail"];
                    bool isSSL = Convert.ToBoolean(this.Configuration.GetSection("MailSettings")["isSSL"]);
                    var password = Encryption.DecryptText(oData.Password);
                    var body = "<p>Hi,</p>" +
                                "<p>Your Email Address is:- " + oData.EmailAddress + "</p>" +
                                "<p>Your password is:- " + password + "</p><br/>" +
                                "Thank You.";

                    var subject = "Estajo - Admin Details";
                    Utility.sendMail(oData.EmailAddress, body, subject, fromEmail, SmtpUserName, SmtpPassword, SmtpPort, SmtpServer, isSSL);

                    #endregion

                    ShowSuccessMessage("Admin User added Successfully.", true);
                    return RedirectToAction("AdminUser", "Admin");
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
                return View(model);
            }
        }


        public IActionResult UpdateAdminDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);

                int intAdminID = Convert.ToInt32(Request.Form["adminID"]);
                string strfullName = Request.Form["fullName"];

                string stremailAddress = Request.Form["emailAddress"];

                if (_dbContext.TblUsers.Where(x => x.EmailAddress.Equals(stremailAddress) && x.UserId != intAdminID && x.CompanyId == CompanyId).FirstOrDefault() != null)
                    return Json(new { success = false, message = "Email address is already in use! Try another email address!" });

                TblUser user = _dbContext.TblUsers.Where(x => x.UserId == intAdminID && x.CompanyId == CompanyId).FirstOrDefault();
                if (user.EmailAddress.Equals(Request.Cookies["EmailAddress"].ToString()))
                    return Json(new { success = false, message = "You can not change your own details!" });
                user.FullName = strfullName;
                user.EmailAddress = stremailAddress;
                _dbContext.Entry(user).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public IActionResult GetAdminDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int id = Convert.ToInt32(Request.Form["adminID"]);
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                return Json(_dbContext.TblUsers.Where(x => x.UserId == id && x.CompanyId == CompanyId).Select(x => new
                {
                    success = true,
                    adminID = x.UserId,
                    fullName = x.FullName,
                    emailAddress = x.EmailAddress
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public IActionResult ActivateDeactivateAdmin()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int id = Convert.ToInt32(Request.Form["id"]);
                int Flag = Convert.ToInt32(Request.Form["flag"]);
                TblUser user = _dbContext.TblUsers.FirstOrDefault(c => c.UserId == id && c.CompanyId == CompanyId);
                if (user.EmailAddress.Equals(Request.Cookies["EmailAddress"].ToString()))
                    return Json(new { success = false, message = "You can not change your status!" });
                if (Flag >= 1)
                    user.IsActive = true;
                else
                    user.IsActive = false;
                _dbContext.Entry(user).State = EntityState.Modified;
                _dbContext.SaveChanges();
                if (Flag >= 1)
                    return Json(new { Act = true, message = "Admin is activated" });
                else
                    return Json(new { DeAct = true, message = "Admin is de-activated" });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { Error = false });
            }
        }

        public IActionResult DeleteAdmin()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int id = 0;
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                if (Request.Form.ContainsKey("id")) { id = Convert.ToInt32(Request.Form["id"]); }
                if (id <= 0) { throw new Exception("Identity can not be blank!"); }
                TblUser user = _dbContext.TblUsers.Where(x => x.UserId == id && x.CompanyId == CompanyId).FirstOrDefault();
                if (user.EmailAddress.Equals(Request.Cookies["EmailAddress"].ToString()))
                    return Json(new { success = false, message = "You can not delete your self!" });
                _dbContext.TblUsers.Remove(user);
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }
        #endregion


        #region HubSportAPI
        //public IActionResult UserHubSportAPI()
        //{
        //    try
        //    {
        //        if (IsUserAuthorize())
        //        {
        //            using (var DB = _dbContext)
        //            {
        //                int LoginUserId = Convert.ToInt32(Request.Cookies["LoginUserId"]);
        //                var oHubSportAPIList = DB.TblHubSportApikeys.Where(x => x.UserId == LoginUserId).Include(x => x.User).ToList().Select(s => new HubSportViewModel
        //                {
        //                    Id = s.Id,
        //                    UserId = s.UserId,
        //                    HubSportAPIKey = s.HubSportApikey,
        //                    IsActive = (bool)s.IsActive,
        //                    User = s.User,
        //                    CreatedDate = (DateTime)s.CreatedDate
        //                });
        //                return View(oHubSportAPIList);
        //            }
        //        }
        //        else
        //            return RedirectToAction("Login", "Account");
        //    }
        //    catch (Exception ex)
        //    {
        //       
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
        //        ErrorLog.log(DateTime.Now + "--" + area + "--" + actionName + "--" + controllerName + "--\n" + ex);
        //        return null;
        //    }
        //}

        //[HttpGet]
        //public IActionResult AddAPIKey()
        //{
        //    try
        //    {
        //        if (IsUserAuthorize())
        //            return View();
        //        else
        //            return RedirectToAction("Login", "Account");
        //    }
        //    catch (Exception ex)
        //    {
        //        string area = this.ControllerContext.RouteData.Values["area"].ToString();
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
        //        ErrorLog.log(DateTime.Now + "--" + area + "--" + actionName + "--" + controllerName + "--\n" + ex);
        //        return RedirectToAction("Login", "Account");
        //    }
        //}

        //[HttpPost]
        //public IActionResult AddAPIKey(HubSportViewModel model)
        //{
        //    try
        //    {
        //        if (!IsUserAuthorize())
        //            return RedirectToAction("Login", "Account");

        //        if (ModelState.IsValid)
        //        {
        //            int LoginUserId = Convert.ToInt32(Request.Cookies["LoginUserId"]);
        //            TblHubSportApikey oData = new TblHubSportApikey();
        //            oData.UserId = LoginUserId;
        //            oData.HubSportApikey = string.IsNullOrEmpty(model.HubSportAPIKey) ? string.Empty : model.HubSportAPIKey;
        //            oData.IsActive = model.IsActive;
        //            oData.CreatedDate = DateTime.Now;

        //            _dbContext.TblHubSportApikeys.Add(oData);
        //            _dbContext.SaveChanges();

        //            ShowSuccessMessage("API key added Successfully.", true);
        //            return RedirectToAction("UserHubSportAPI", "Admin");
        //        }
        //        else
        //        {
        //            return View(model);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        string area = this.ControllerContext.RouteData.Values["area"].ToString();
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
        //        ErrorLog.log(DateTime.Now + "--" + area + "--" + actionName + "--" + controllerName + "--\n" + ex);
        //        ShowErrorMessage(ex.Message, true);
        //        return View(model);
        //    }
        //}

        //public ActionResult ActivateDeactivateKey()
        //{
        //    if (!IsUserAuthorize())
        //        return RedirectToAction("Login", "Account");
        //    try
        //    {
        //        int userid = Convert.ToInt32(Request.Form["userid"]);
        //        int Flag = Convert.ToInt32(Request.Form["flag"]);
        //        int id = Convert.ToInt32(Request.Form["id"]);
        //        TblHubSportApikey oData = _dbContext.TblHubSportApikeys.FirstOrDefault(c => c.UserId == userid && c.Id == id);
        //        if (Flag >= 1)
        //            oData.IsActive = true;
        //        else
        //            oData.IsActive = false;
        //        _dbContext.Entry(oData).State = EntityState.Modified;
        //        _dbContext.SaveChanges();
        //        if (Flag >= 1)
        //            return Json(new { Act = true, message = "API Key is activated" });
        //        else
        //            return Json(new { DeAct = true, message = "API Key is de-activated" });
        //    }
        //    catch (Exception ex)
        //    {
        //        string area = this.ControllerContext.RouteData.Values["area"].ToString();
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
        //        ErrorLog.log(DateTime.Now + "--" + area + "--" + actionName + "--" + controllerName + "--\n" + ex);
        //        return Json(new { Error = false });
        //    }
        //}

        //public ActionResult DeleteAPIKey()
        //{
        //    if (!IsUserAuthorize())
        //        return RedirectToAction("Login", "Account");
        //    try
        //    {
        //        int id = 0;
        //        int UserId = 0;
        //        if (Request.Form.ContainsKey("id")) { id = Convert.ToInt32(Request.Form["id"]); }
        //        if (id <= 0) { throw new Exception("Identity can not be blank!"); }
        //        UserId = Convert.ToInt32(Request.Form["UserId"]);
        //        TblHubSportApikey oData = _dbContext.TblHubSportApikeys.Where(x => x.UserId == UserId && x.Id == id).FirstOrDefault();
        //        _dbContext.TblHubSportApikeys.Remove(oData);
        //        _dbContext.SaveChanges();
        //        return Json(new { success = true, message = "API Key deleted Sucessfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        string area = this.ControllerContext.RouteData.Values["area"].ToString();
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
        //        ErrorLog.log(DateTime.Now + "--" + area + "--" + actionName + "--" + controllerName + "--\n" + ex);
        //        return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
        //    }
        //}

        //public ActionResult GetHubSportDetails()
        //{
        //    try
        //    {
        //        int id = Convert.ToInt32(Request.Form["id"]);
        //        int oLoginUserId = Convert.ToInt32(Request.Form["userId"]);
        //        return Json(_dbContext.TblHubSportApikeys.Where(x => x.UserId == oLoginUserId && x.Id == id).Include(x => x.User).Select(x => new
        //        {
        //            success = true,
        //            UserId = x.UserId,
        //            Id = x.Id,
        //            fullName = x.User.FullName,
        //            Apikey = x.HubSportApikey,
        //            IsActive = x.IsActive
        //        }).FirstOrDefault());
        //    }
        //    catch (Exception ex)
        //    {
        //        string area = this.ControllerContext.RouteData.Values["area"].ToString();
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
        //        ErrorLog.log(DateTime.Now + "--" + area + "--" + actionName + "--" + controllerName + "--\n" + ex);
        //        return Json(new { success = false, message = "Opps! Something went wrong!" });
        //    }

        //}
        #endregion


        public IActionResult AssignMultipleAgent()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int intAgentID = Convert.ToInt32(Request.Form["agentID"]);
                var leadIDList = Request.Form["leadID"].ToString().Split(",");
                if (leadIDList.Count() > 0)
                {
                    foreach (var item in leadIDList)
                    {
                        TblLead lead = _dbContext.TblLeads.Where(x => x.LeadId == Convert.ToInt32(item) && x.CompanyId == CompanyId).FirstOrDefault();
                        lead.AgentId = intAgentID <= 0 ? Convert.ToInt32(DBNull.Value) : intAgentID;
                        _dbContext.Entry(lead).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No Agent Selected!" });
                }

            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }


        #region Stage
        public IActionResult Stage()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                {
                    using (var DB = _dbContext)
                    {
                        int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                        var oStageList = DB.TblStages.Where(x => x.CompanyId == CompanyId).ToList().Select(s => new StageViewModel
                        {
                            StageId = s.StageId,
                            StageName = s.StageName,
                        });
                        return View(oStageList);
                    }
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }

        }

        //[HttpGet]
        public IActionResult AddStage()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                    return View();
                else
                    return RedirectToAction("Login", "Account");
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
        public IActionResult AddStage(StageViewModel model)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                if (ModelState.IsValid)
                {
                    int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                    TblStage oData = new TblStage();
                    oData.CompanyId = CompanyId;
                    oData.StageName = string.IsNullOrEmpty(model.StageName) ? string.Empty : model.StageName;
                    oData.CreatedDate = DateTime.Now;

                    _dbContext.TblStages.Add(oData);
                    _dbContext.SaveChanges();

                    ShowSuccessMessage("Stage added Successfully.", true);
                    return RedirectToAction("Stage", "Admin");
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
                ShowErrorMessage(ex.Message, true);
                return View(model);
            }
        }

        public IActionResult GetStageDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int stageId = Convert.ToInt32(Request.Form["stageId"]);
                return Json(_dbContext.TblStages.Where(x => x.CompanyId == CompanyId && x.StageId == stageId).Select(x => new
                {
                    success = true,
                    StageId = x.StageId,
                    StageName = x.StageName
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public IActionResult UpdateStageDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int stageId = Convert.ToInt32(Request.Form["stageId"]);
                string stageName = Request.Form["stageName"];

                TblStage oStage = _dbContext.TblStages.Where(x => x.StageId == stageId && x.CompanyId == CompanyId).FirstOrDefault();
                if (oStage != null)
                {
                    oStage.StageName = stageName;
                    oStage.UpdatedDate = DateTime.Now;
                    _dbContext.Entry(oStage).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No Stage Found." });
                }
            }
            catch (Exception ex)
            {

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }


        public IActionResult DeleteStage()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int stageId = Convert.ToInt32(Request.Form["stageId"]);
                if (stageId <= 0) { throw new Exception("Identity can not be blank!"); }
                TblStage oData = _dbContext.TblStages.Where(x => x.StageId == stageId && x.CompanyId == CompanyId).FirstOrDefault();
                _dbContext.TblStages.Remove(oData);
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Stage deleted Sucessfully." });
            }
            catch (Exception ex)
            {

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }

        public IActionResult AssignStageToLead()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int intstageID = Convert.ToInt32(Request.Form["stageID"]);
                var leadIDList = Request.Form["leadID"].ToString().Split(",");
                if (leadIDList.Count() > 0)
                {
                    foreach (var item in leadIDList)
                    {
                        TblLead lead = _dbContext.TblLeads.Where(x => x.LeadId == Convert.ToInt32(item) && x.CompanyId == CompanyId).FirstOrDefault();
                        lead.StageId = intstageID <= 0 ? Convert.ToInt32(DBNull.Value) : intstageID;
                        _dbContext.Entry(lead).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No Stage Selected!" });
                }

            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public IActionResult UpdateStageByLeadId()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int stageId = Convert.ToInt32(Request.Form["stageId"]);
                int intLeadID = Convert.ToInt32(Request.Form["leadID"]);
                TblLead lead = _dbContext.TblLeads.Where(x => x.LeadId == intLeadID && x.CompanyId == CompanyId).FirstOrDefault();
                lead.StageId = stageId <= 0 ? Convert.ToInt32(DBNull.Value) : stageId;
                _dbContext.Entry(lead).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }



        #endregion


        #region Tags
        public IActionResult Tags()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                {
                    using (var DB = _dbContext)
                    {
                        int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                        var oTagsList = DB.TblTags.Where(x => x.CompanyId == CompanyId).ToList().Select(s => new TagsViewModel
                        {
                            TagsId = s.TagId,
                            TagsName = s.TagName,
                        });
                        return View(oTagsList);
                    }
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }

        }

        //[HttpGet]
        public IActionResult AddTags()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                    return View();
                else
                    return RedirectToAction("Login", "Account");
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
        public IActionResult AddTags(TagsViewModel model)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                if (ModelState.IsValid)
                {
                    int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                    TblTag oData = new TblTag();
                    oData.CompanyId = CompanyId;
                    oData.TagName = string.IsNullOrEmpty(model.TagsName) ? string.Empty : model.TagsName;
                    oData.CreatedDate = DateTime.Now;

                    _dbContext.TblTags.Add(oData);
                    _dbContext.SaveChanges();

                    ShowSuccessMessage("Tags added Successfully.", true);
                    return RedirectToAction("Tags", "Admin");
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
                ShowErrorMessage(ex.Message, true);
                return View(model);
            }
        }

        public IActionResult GetTagsDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int tagId = Convert.ToInt32(Request.Form["TagId"]);
                return Json(_dbContext.TblTags.Where(x => x.CompanyId == CompanyId && x.TagId == tagId).Select(x => new
                {
                    success = true,
                    tagId = x.TagId,
                    tagName = x.TagName
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public IActionResult UpdateTagsDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int tagId = Convert.ToInt32(Request.Form["tagId"]);
                string tagName = Request.Form["TagName"];

                TblTag oStage = _dbContext.TblTags.Where(x => x.TagId == tagId && x.CompanyId == CompanyId).FirstOrDefault();
                if (oStage != null)
                {
                    oStage.TagName = tagName;
                    oStage.UpdatedDate = DateTime.Now;
                    _dbContext.Entry(oStage).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No Stage Found." });
                }
            }
            catch (Exception ex)
            {

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public IActionResult DeleteTags()
        {
            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int TagId = Convert.ToInt32(Request.Form["TagId"]);
                if (TagId <= 0) { throw new Exception("Identity can not be blank!"); }
                TblTag oData = _dbContext.TblTags.Where(x => x.TagId == TagId && x.CompanyId == CompanyId).FirstOrDefault();
                _dbContext.TblTags.Remove(oData);
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Tags deleted Sucessfully." });
            }
            catch (Exception ex)
            {

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }

        public IActionResult AssignTagToLead()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                var tageIDList = Request.Form["tagID"].ToString().Split(",");
                var leadIDList = Request.Form["leadID"].ToString().Split(",");
                if (leadIDList.Count() > 0)
                {
                    foreach (var item in leadIDList)
                    {
                        int LeadId = Convert.ToInt32(item);
                        var model = _dbContext.TblLeadTags.Where(x => x.LeadId == LeadId && x.CompanyId == CompanyId).ToList();
                        if (model.Count > 0)
                        {
                            foreach (var obj in model)
                            {
                                _dbContext.Entry(obj).State = EntityState.Deleted;
                            }
                            _dbContext.SaveChanges();
                        }

                        foreach (var itemTag in tageIDList)
                        {
                            //TblLeadTag leadTag = _dbContext.TblLeadTags.Where(x => x.LeadId == Convert.ToInt32(item) && x.CompanyId == CompanyId).FirstOrDefault();
                            TblLeadTag oData = new TblLeadTag();
                            oData.CompanyId = CompanyId;
                            oData.TagId = Convert.ToInt32(itemTag);
                            oData.LeadId = Convert.ToInt32(item);
                            _dbContext.TblLeadTags.Add(oData);
                            _dbContext.SaveChanges();
                        }
                    }
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No Tag Selected!" });
                }

            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }


        public IActionResult GetSelectedTagbyLeadId()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");


                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int leadID = Convert.ToInt32(Request.Form["leadID"]);
                List<int?> selectedTags = _dbContext.TblLeadTags.Where(x => x.LeadId == leadID).Select(x => x.TagId).ToList();
                if (selectedTags.Count > 0)
                {
                    return Json(new { success = true, selectedTags = selectedTags });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }
        #endregion
    }
}
