using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using RealEstate.Models;
using RealEstate.Utills;
using RealEstateDB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Controllers
{

    [Authorize(Roles = "Company")]
    public class AdminController : BaseController
    {
        private RealEstateContext _dbContext;
        private IConfiguration Configuration;
        private IWebHostEnvironment Environment;
        public AdminController(RealEstateContext dbContext, IConfiguration _configuration, IWebHostEnvironment _environment)
        {
            _dbContext = dbContext;
            Configuration = _configuration;
            Environment = _environment;
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                        //int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                        //var oAgentList = DB.TblAgents.Where(x => x.AccountId == AccountId).ToList().Select(s => new AgentViewModel
                        //{
                        //    AgentId = s.Id,
                        //    FullName = s.FullName,
                        //    EmailAddress = s.EmailAddress,
                        //    CellPhone = s.CellPhone,
                        //    IsActive = (bool)s.IsActive,
                        //    CreatedDate = (DateTime)s.CreatedDate
                        //});
                        //return View(oAgentList);

                        int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                        var oAgentList = from A in DB.TblAccounts // outer sequence
                                         join AC in DB.TblAccountCompanies //inner sequence 
                                         on A.AccountId equals AC.AccountId // key selector 
                                         where AC.AddedBy == AccountId && A.RoleId == RoleType.Agent.GetHashCode()
                                         select new AgentViewModel
                                         {
                                             AccountId = (int)A.AccountId,
                                             FullName = A.FullName,
                                             EmailAddress = A.UserName,
                                             CellPhone = A.PhoneNumber,
                                             IsActive = (bool)A.Status,
                                             CreatedDate = (DateTime)A.CreatedDate,
                                             //IsOwner = (bool)A.IsOwner
                                         };
                        return View(oAgentList.ToList());
                    }
                }
                else
                    return RedirectToAction("Login", "Account");
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                    //TblAgent oData = new TblAgent();
                    //oData.FullName = string.IsNullOrEmpty(model.FullName) ? string.Empty : model.FullName;
                    //oData.EmailAddress = string.IsNullOrEmpty(model.EmailAddress) ? string.Empty : model.EmailAddress;
                    //oData.CellPhone = string.IsNullOrEmpty(model.CellPhone) ? string.Empty : model.CellPhone;
                    ////oData.UserLoginTypeId = UserLoginType.Manual.GetHashCode();
                    //oData.IsActive = model.IsActive;
                    //oData.AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                    //oData.CreatedDate = DateTime.Now;

                    //_dbContext.TblAgents.Add(oData);
                    //_dbContext.SaveChanges();

                    var Password = Guid.NewGuid().ToString("N").Substring(0, 8);
                    TblAccount oData = new TblAccount();
                    oData.FullName = string.IsNullOrEmpty(model.FullName) ? string.Empty : model.FullName;
                    oData.PhoneNumber = string.IsNullOrEmpty(model.CellPhone) ? string.Empty : model.CellPhone;
                    oData.UserName = string.IsNullOrEmpty(model.EmailAddress) ? string.Empty : model.EmailAddress;
                    oData.Password = Encryption.EncryptText(Password);
                    oData.IsOwner = false;
                    oData.CreatedDate = DateTime.Now;
                    oData.RoleId = RoleType.Agent.GetHashCode();
                    oData.IsTempPassword = true;
                    oData.Status = true;
                    _dbContext.TblAccounts.Add(oData);
                    _dbContext.SaveChanges();


                    TblAccountCompany oDataCompany = new TblAccountCompany();
                    oDataCompany.AccountId = oData.AccountId;
                    oDataCompany.AddedBy = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                    oDataCompany.CreatedDate = DateTime.Now;
                    _dbContext.TblAccountCompanies.Add(oDataCompany);
                    _dbContext.SaveChanges();


                    #region SendMail
                    string LoginURL = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                    string SmtpUserName = this.Configuration.GetSection("MailSettings")["SmtpUserName"];
                    string SmtpPassword = this.Configuration.GetSection("MailSettings")["SmtpPassword"];
                    int SmtpPort = Convert.ToInt32(this.Configuration.GetSection("MailSettings")["SmtpPort"]);
                    string SmtpServer = this.Configuration.GetSection("MailSettings")["SmtpServer"];
                    string fromEmail = this.Configuration.GetSection("MailSettings")["fromEmail"];
                    bool isSSL = Convert.ToBoolean(this.Configuration.GetSection("MailSettings")["isSSL"]);
                    //var password = Encryption.DecryptText(oData.Password);
                    var body = "<p>Hi,</p>" +
                                "<p>Your Agent Email Address is:- " + oData.UserName + "</p>" +
                                "<p>Your Temporary Agent Password is:- " + Password + "</p>" +
                                "<p><a href=" + LoginURL + ">Please login with the above details and set your own password.</a></p><br/>" +
                                "Thank You.";

                    var subject = "Estajo - Agent Login Details";

                    //Utility.sendMail(oData.UserName, body, subject, fromEmail, SmtpUserName, SmtpPassword, SmtpPort, SmtpServer, isSSL);

                    var SMTPDetail = _dbContext.TblSmtps.FirstOrDefault();
                    if (SMTPDetail != null)
                    {
                        DateTime myDate1 = SMTPDetail.CreatedDate.Value;
                        DateTime myDate2 = DateTime.Now;
                        TimeSpan difference = myDate2.Subtract(myDate1);
                        if (difference.TotalHours >= 1)
                        {
                            AuthResponse response = AuthResponse.refresh(this.Configuration.GetSection("Google")["GoogleClientId"], this.Configuration.GetSection("Google")["GoogleoAuthTokenURL"], this.Configuration.GetSection("Google")["GoogleClientSecret"], SMTPDetail.RefreshToken);

                            if (response.Access_token != null)
                            {
                                SMTPDetail.AccessToken = response.Access_token;
                                SMTPDetail.CreatedDate = response.created;
                                _dbContext.SaveChanges();
                                Utility.SendMailAccessToken(SmtpUserName, SMTPDetail.AccessToken, oData.UserName, subject, body);
                            }
                        }
                        else
                        {
                            Utility.SendMailAccessToken(SmtpUserName, SMTPDetail.AccessToken, oData.UserName, subject, body);
                        }
                    }

                    #endregion

                    ShowSuccessMessage("User added Successfully. Please check your email address for login details.", true);
                    return RedirectToAction("Team", "Admin");
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                TblAccount agent = _dbContext.TblAccounts.SingleOrDefault(c => c.AccountId == id);// && c.AccountId == AccountId

                if (Flag >= 1)
                    agent.Status = true;
                else
                    agent.Status = false;
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                //int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int id = 0;
                if (Request.Form.ContainsKey("id")) { id = Convert.ToInt32(Request.Form["id"]); }
                if (id <= 0) { throw new Exception("Identity can not be blank!"); }

                var model = _dbContext.TblCustomFieldValues.Where(x => x.AccountId == id).ToList();
                if (model.Count > 0)
                {
                    foreach (var obj in model)
                    {
                        _dbContext.Entry(obj).State = EntityState.Deleted;
                    }
                    _dbContext.SaveChanges();
                }

                TblCustomField oData = _dbContext.TblCustomFields.Where(x => x.AccountId == id).FirstOrDefault();
                if (oData != null)
                {
                    _dbContext.TblCustomFields.Remove(oData);
                    _dbContext.SaveChanges();
                }


                var oTblLeads = _dbContext.TblLeads.Where(x => x.AgentId == id).ToList();
                foreach (var item in oTblLeads)
                {
                    var oCustomFieldAnswer = _dbContext.TblCustomFieldAnswers.Where(x => x.LeadId == item.LeadId).ToList();
                    foreach (var itemCustomFieldAnswer in oCustomFieldAnswer)
                    {
                        _dbContext.TblCustomFieldAnswers.Remove(itemCustomFieldAnswer);
                        _dbContext.SaveChanges();
                    }

                    var oLeadEmailMessage = _dbContext.TblLeadEmailMessages.Where(x => x.LeadId == item.LeadId).ToList();
                    foreach (var itemLeadEmailMessage in oLeadEmailMessage)
                    {

                        var oLeadEmailMessageAttachment = _dbContext.TblLeadEmailMessageAttachments.Where(x => x.LeadEmailMessageId == itemLeadEmailMessage.LeadEmailMessageId).ToList();
                        foreach (var itemLeadEmailMessageAttachment in oLeadEmailMessageAttachment)
                        {
                            _dbContext.TblLeadEmailMessageAttachments.Remove(itemLeadEmailMessageAttachment);
                            _dbContext.SaveChanges();
                        }
                        _dbContext.TblLeadEmailMessages.Remove(itemLeadEmailMessage);
                        _dbContext.SaveChanges();
                    }

                    item.AgentId = null;
                    //_dbContext.TblLeads.Remove(item);
                    _dbContext.SaveChanges();
                }

                var oLeadAppointment = _dbContext.TblLeadAppointments.Where(x => x.AgentId == id).ToList();
                foreach (var itemLeadAppointment in oLeadAppointment)
                {
                    _dbContext.TblLeadAppointments.Remove(itemLeadAppointment);
                    _dbContext.SaveChanges();
                }


                //_dbContext.TblLeads.RemoveRange(_dbContext.TblLeads.Where(x => x.AgentId == id));
                _dbContext.TblAccountIntegrations.Remove(_dbContext.TblAccountIntegrations.Where(x => x.AccountId == id).FirstOrDefault());
                _dbContext.TblAccountCompanies.Remove(_dbContext.TblAccountCompanies.Where(x => x.AccountId == id).FirstOrDefault());
                _dbContext.TblAccounts.Remove(_dbContext.TblAccounts.Where(x => x.AccountId == id).FirstOrDefault());
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }

        public IActionResult UpdateAgent()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int intAgentID = Convert.ToInt32(Request.Form["agentID"]);
                int intLeadID = Convert.ToInt32(Request.Form["leadID"]);
                TblLead lead = _dbContext.TblLeads.Where(x => x.LeadId == intLeadID && x.AccountId == AccountId).FirstOrDefault();
                lead.AgentId = intAgentID <= 0 ? Convert.ToInt32(DBNull.Value) : intAgentID;
                _dbContext.Entry(lead).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public IActionResult GetAgentDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Form["AccountId"]);
                //int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                return Json(_dbContext.TblAccounts.Where(x => x.AccountId == AccountId).Select(x => new
                {
                    success = true,
                    AccountId = x.AccountId,
                    fullName = x.FullName,
                    emailAddress = x.UserName,
                    cellPhone = x.PhoneNumber,
                    status = x.Status == true ? 1 : 0
                }).FirstOrDefault());
                //return Json(_dbContext.TblAgents.Where(x => x.Id == id && x.AccountId == AccountId).Select(x => new
                //{
                //    success = true,
                //    agentID = x.Id,
                //    fullName = x.FullName,
                //    emailAddress = x.EmailAddress,
                //    cellPhone = x.CellPhone,
                //    status = x.IsActive == true ? 1 : 0
                //}).FirstOrDefault());
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public IActionResult UpdateAgentDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                //int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);

                int AccountId = Convert.ToInt32(Request.Form["AccountId"]);
                string strfullName = Request.Form["fullName"];

                string stremailAddress = Request.Form["emailAddress"];
                string strcellPhone = Request.Form["cellPhone"];

                if (_dbContext.TblAccounts.Where(x => x.UserName.Equals(stremailAddress)).FirstOrDefault() != null) //&& x.AccountId != AccountId
                    return Json(new { success = false, message = "Email address is already in use! Try another email address!" });

                TblAccount oAgent = _dbContext.TblAccounts.Where(x => x.AccountId == AccountId && x.RoleId == RoleType.Agent.GetHashCode()).FirstOrDefault();
                if (oAgent != null)
                {
                    oAgent.FullName = strfullName;
                    oAgent.UserName = stremailAddress;
                    oAgent.PhoneNumber = strcellPhone;
                    //oAgent.UserLoginTypeId = UserLoginType.Manual.GetHashCode();
                    oAgent.UpdatedDate = DateTime.Now;
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        #endregion

        #region Function
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

        public bool IsUserAuthorize()
        {
            try
            {
                if (Request.Cookies.ContainsKey("UserLoginTypeId"))
                {
                    if (Convert.ToInt32(Request.Cookies["UserLoginTypeId"]) != RoleType.Admin.GetHashCode())
                        //if (Convert.ToInt32(Request.Cookies["UserLoginTypeId"]) != UserLoginType.Company.GetHashCode())
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
                var result = _dbContext.TblAccounts.Where(u => u.UserName.ToLower().Equals(email.ToLower()) && u.RoleId == RoleType.Admin.GetHashCode()).ToList();
                if (result.Count() > 0)
                    return true;
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
                        int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);


                        var oAdminUserList = from A in DB.TblAccounts // outer sequence
                                             join AC in DB.TblAccountCompanies //inner sequence 
                                             on A.AccountId equals AC.AccountId // key selector 
                                             where AC.AddedBy == AccountId && A.RoleId == RoleType.Admin.GetHashCode()
                                             select new UserViewModel
                                             {
                                                 AccountId = (int)A.AccountId,
                                                 FullName = A.FullName,
                                                 EmailAddress = A.UserName,
                                                 Status = A.Status == null ? false : true,
                                                 CreatedDate = (DateTime)A.CreatedDate,
                                                 IsOwner = (bool)A.IsOwner
                                             };
                        return View(oAdminUserList.ToList());
                        //var oAccount = DB.TblAccounts.Where(c => c.RoleId == (int)RoleType.Admin && c.AccountId == AccountId).ToList().Select(s => new UserViewModel
                        //{
                        //    AccountId = (int)s.AccountId,
                        //    FullName = s.FullName,
                        //    EmailAddress = s.UserName,
                        //    Status = s.Status == null ? false : true,
                        //    CreatedDate = (DateTime)s.CreatedDate,
                        //    IsOwner = (bool)s.IsOwner
                        //});

                        //var oAccountDetails = DB.TblAccountCompanies.Where(x => x.AddedBy == AccountId).Include(x => x.Account).ToList().Select(s => new UserViewModel
                        //{
                        //    AccountId = (int)s.AccountId,
                        //    FullName = s.Account.FullName,
                        //    EmailAddress = s.Account.UserName,
                        //    Status = s.Account.Status == null ? false : true,
                        //    CreatedDate = (DateTime)s.CreatedDate,
                        //    IsOwner = (bool)s.Account.IsOwner
                        //});

                        //var oAdminUserList = oAccount.Union(oAccountDetails);

                        //var oAdminUserList = DB.TblAccountCompanies.Where(x => x.Account.RoleId == (int)RoleType.Admin && x.AccountId == AccountId).ToList().Select(s => new UserViewModel
                        //{
                        //    AccountId = (int)s.AccountId,
                        //    FullName = s.Account.FullName,
                        //    EmailAddress = s.Account.UserName,
                        //    Status = s.Account.Status == null ? false : true,
                        //    CreatedDate = (DateTime)s.CreatedDate,
                        //    IsOwner = (bool)s.Account.IsOwner
                        //});

                    }
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult AddAdminUser(UserViewModel model)
        {
            try
            {
                ModelState.Remove("CellPhone");

                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                if (ModelState.IsValid)
                {
                    int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);

                    if (IsAdminExist(model.EmailAddress))
                    {
                        ShowErrorMessage("Email is already used!", true);
                        return View(model);
                    }
                    TblAccount oData = new TblAccount();
                    oData.FullName = string.IsNullOrEmpty(model.FullName) ? string.Empty : model.FullName;
                    oData.UserName = string.IsNullOrEmpty(model.EmailAddress) ? string.Empty : model.EmailAddress;
                    oData.Password = string.IsNullOrEmpty(model.Password) ? string.Empty : Encryption.EncryptText(model.Password);
                    oData.Status = model.Status;
                    oData.CreatedDate = DateTime.Now;
                    oData.RoleId = RoleType.Admin.GetHashCode();
                    oData.IsOwner = false;
                    _dbContext.TblAccounts.Add(oData);
                    _dbContext.SaveChanges();

                    TblAccountCompany oCompany = new TblAccountCompany();
                    oCompany.AccountId = oData.AccountId;
                    oCompany.AddedBy = AccountId;
                    oCompany.CreatedDate = DateTime.Now;
                    _dbContext.TblAccountCompanies.Add(oCompany);
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
                                "<p>Your Email Address is:- " + oData.UserName + "</p>" +
                                "<p>Your password is:- " + password + "</p><br/>" +
                                "Thank You.";

                    var subject = "Estajo - Admin Details";
                    //Utility.sendMail(oData.UserName, body, subject, fromEmail, SmtpUserName, SmtpPassword, SmtpPort, SmtpServer, isSSL);
                    var SMTPDetail = _dbContext.TblSmtps.FirstOrDefault();
                    if (SMTPDetail != null)
                    {
                        DateTime myDate1 = SMTPDetail.CreatedDate.Value;
                        DateTime myDate2 = DateTime.Now;
                        TimeSpan difference = myDate2.Subtract(myDate1);
                        if (difference.TotalHours >= 1)
                        {
                            AuthResponse response = AuthResponse.refresh(this.Configuration.GetSection("Google")["GoogleClientId"], this.Configuration.GetSection("Google")["GoogleoAuthTokenURL"], this.Configuration.GetSection("Google")["GoogleClientSecret"], SMTPDetail.RefreshToken);

                            if (response.Access_token != null)
                            {
                                SMTPDetail.AccessToken = response.Access_token;
                                SMTPDetail.CreatedDate = response.created;
                                _dbContext.SaveChanges();
                                Utility.SendMailAccessToken(SmtpUserName, SMTPDetail.AccessToken, oData.UserName, subject, body);
                            }
                        }
                        else
                        {
                            Utility.SendMailAccessToken(SmtpUserName, SMTPDetail.AccessToken, oData.UserName, subject, body);
                        }
                    }

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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return View(model);
            }
        }


        public IActionResult UpdateAdminDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);

                int intAdminID = Convert.ToInt32(Request.Form["adminID"]);
                string strfullName = Request.Form["fullName"];

                string stremailAddress = Request.Form["emailAddress"];

                if (_dbContext.TblAccounts.Where(x => x.UserName.Equals(stremailAddress) && x.AccountId != intAdminID && x.AccountId == AccountId && x.RoleId == RoleType.Admin.GetHashCode()).FirstOrDefault() != null)
                    return Json(new { success = false, message = "Email address is already in use! Try another email address!" });

                TblAccount user = _dbContext.TblAccounts.Where(x => x.RoleId == RoleType.Admin.GetHashCode() && x.AccountId == AccountId).FirstOrDefault();
                if (user.UserName.Equals(Request.Cookies["EmailAddress"].ToString()))
                    return Json(new { success = false, message = "You can not change your own details!" });
                user.FullName = strfullName;
                user.UserName = stremailAddress;
                user.UpdatedDate = DateTime.Now;
                _dbContext.Entry(user).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                return Json(_dbContext.TblAccounts.Where(x => x.RoleId == RoleType.Admin.GetHashCode() && x.AccountId == AccountId).Select(x => new
                {
                    success = true,
                    adminID = x.AccountId,
                    fullName = x.FullName,
                    emailAddress = x.UserName
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int id = Convert.ToInt32(Request.Form["id"]);
                int Flag = Convert.ToInt32(Request.Form["flag"]);
                TblAccount user = _dbContext.TblAccounts.FirstOrDefault(c => c.RoleId == RoleType.Admin.GetHashCode() && c.IsOwner == false && c.AccountId == AccountId);
                if (user.UserName.Equals(Request.Cookies["EmailAddress"].ToString()))
                    return Json(new { success = false, message = "You can not change your status!" });
                if (Flag >= 1)
                    user.Status = true;
                else
                    user.Status = false;
                user.UpdatedDate = DateTime.Now;
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                if (Request.Form.ContainsKey("id")) { id = Convert.ToInt32(Request.Form["id"]); }
                if (id <= 0) { throw new Exception("Identity can not be blank!"); }
                TblAccount user = _dbContext.TblAccounts.Where(c => c.RoleId == RoleType.Admin.GetHashCode() && c.IsOwner == false && c.AccountId == AccountId).FirstOrDefault();
                if (user.UserName.Equals(Request.Cookies["EmailAddress"].ToString()))
                    return Json(new { success = false, message = "You can not delete your self!" });
                _dbContext.TblAccounts.Remove(user);
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int intAgentID = Convert.ToInt32(Request.Form["agentID"]);
                var leadIDList = Request.Form["leadID"].ToString().Split(",");
                if (leadIDList.Count() > 0)
                {
                    foreach (var item in leadIDList)
                    {
                        TblLead lead = _dbContext.TblLeads.Where(x => x.LeadId == Convert.ToInt32(item) && x.AccountId == AccountId).FirstOrDefault();
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                        int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                        var oStageList = DB.TblStages.Where(x => x.AccountId == AccountId).ToList().Select(s => new StageViewModel
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                    int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                    TblStage oData = new TblStage();
                    oData.AccountId = AccountId;
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int stageId = Convert.ToInt32(Request.Form["stageId"]);
                return Json(_dbContext.TblStages.Where(x => x.AccountId == AccountId && x.StageId == stageId).Select(x => new
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int stageId = Convert.ToInt32(Request.Form["stageId"]);
                string stageName = Request.Form["stageName"];

                TblStage oStage = _dbContext.TblStages.Where(x => x.StageId == stageId && x.AccountId == AccountId).FirstOrDefault();
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int stageId = Convert.ToInt32(Request.Form["stageId"]);
                if (stageId <= 0) { throw new Exception("Identity can not be blank!"); }
                TblStage oData = _dbContext.TblStages.Where(x => x.StageId == stageId && x.AccountId == AccountId).FirstOrDefault();
                _dbContext.TblStages.Remove(oData);
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Stage deleted Sucessfully." });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }

        public IActionResult AssignStageToLead()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int intstageID = Convert.ToInt32(Request.Form["stageID"]);
                var leadIDList = Request.Form["leadID"].ToString().Split(",");
                if (leadIDList.Count() > 0)
                {
                    foreach (var item in leadIDList)
                    {
                        TblLead lead = _dbContext.TblLeads.Where(x => x.LeadId == Convert.ToInt32(item) && x.AccountId == AccountId).FirstOrDefault();
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public IActionResult UpdateStageByLeadId()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int stageId = Convert.ToInt32(Request.Form["stageId"]);
                int intLeadID = Convert.ToInt32(Request.Form["leadID"]);
                TblLead lead = _dbContext.TblLeads.Where(x => x.LeadId == intLeadID && x.AccountId == AccountId).FirstOrDefault();
                lead.StageId = stageId <= 0 ? Convert.ToInt32(DBNull.Value) : stageId;
                _dbContext.Entry(lead).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                        int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                        var oTagsList = DB.TblTags.Where(x => x.AccountId == AccountId).ToList().Select(s => new TagsViewModel
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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
                    int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                    TblTag oData = new TblTag();
                    oData.AccountId = AccountId;
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int tagId = Convert.ToInt32(Request.Form["TagId"]);
                return Json(_dbContext.TblTags.Where(x => x.AccountId == AccountId && x.TagId == tagId).Select(x => new
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
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

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int tagId = Convert.ToInt32(Request.Form["tagId"]);
                string tagName = Request.Form["TagName"];

                TblTag oStage = _dbContext.TblTags.Where(x => x.TagId == tagId && x.AccountId == AccountId).FirstOrDefault();
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public IActionResult DeleteTags()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int TagId = Convert.ToInt32(Request.Form["TagId"]);
                if (TagId <= 0) { throw new Exception("Identity can not be blank!"); }
                TblTag oData = _dbContext.TblTags.Where(x => x.TagId == TagId && x.AccountId == AccountId).FirstOrDefault();
                _dbContext.TblTags.Remove(oData);
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Tags deleted Sucessfully." });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }

        public IActionResult AssignTagToLead()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                var tageIDList = Request.Form["tagID"].ToString().Split(",");
                var leadIDList = Request.Form["leadID"].ToString().Split(",");
                if (leadIDList.Count() > 0)
                {
                    foreach (var item in leadIDList)
                    {
                        int LeadId = Convert.ToInt32(item);
                        var model = _dbContext.TblLeadTags.Where(x => x.LeadId == LeadId && x.AccountId == AccountId).ToList();
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
                            oData.AccountId = AccountId;
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }


        public IActionResult GetSelectedTagbyLeadId()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }
        #endregion

        #region AppointmentTypes
        public IActionResult AppointmentTypes()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                {
                    using (var DB = _dbContext)
                    {
                        int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                        var oAppointmentTypeList = DB.TblAppointmentTypes.Where(x => x.AccountId == AccountId).ToList().Select(s => new AppointmentTypeViewModel
                        {
                            AppointmenTypeId = s.AppointmenTypeId,
                            AppointmentTypeName = s.AppointmentTypeName,
                        });
                        return View(oAppointmentTypeList);
                    }
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }

        }

        //[HttpGet]
        public IActionResult AddAppointmentTypes()
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult AddAppointmentTypes(AppointmentTypeViewModel model)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                if (ModelState.IsValid)
                {
                    int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                    TblAppointmentType oData = new TblAppointmentType();
                    oData.AccountId = AccountId;
                    oData.AppointmentTypeName = string.IsNullOrEmpty(model.AppointmentTypeName) ? string.Empty : model.AppointmentTypeName;
                    oData.CreatedDate = DateTime.Now;

                    _dbContext.TblAppointmentTypes.Add(oData);
                    _dbContext.SaveChanges();

                    ShowSuccessMessage("Appointment Type added Successfully.", true);
                    return RedirectToAction("AppointmentTypes", "Admin");
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                ShowErrorMessage(ex.Message, true);
                return View(model);
            }
        }


        public IActionResult GetAppointmentTypeDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int AppointmentTypeID = Convert.ToInt32(Request.Form["AppointmentTypeID"]);
                return Json(_dbContext.TblAppointmentTypes.Where(x => x.AccountId == AccountId && x.AppointmenTypeId == AppointmentTypeID).Select(x => new
                {
                    success = true,
                    AppointmenTypeId = x.AppointmenTypeId,
                    AppointmentTypeName = x.AppointmentTypeName
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public IActionResult UpdateAppointmentTypeDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int AppointmentTypeId = Convert.ToInt32(Request.Form["AppointmentTypeId"]);
                string Name = Request.Form["Name"];

                TblAppointmentType oAppointmentType = _dbContext.TblAppointmentTypes.Where(x => x.AppointmenTypeId == AppointmentTypeId && x.AccountId == AccountId).FirstOrDefault();
                if (oAppointmentType != null)
                {
                    oAppointmentType.AppointmentTypeName = Name;
                    oAppointmentType.UpdatedDate = DateTime.Now;
                    _dbContext.Entry(oAppointmentType).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No Appointment Type Found." });
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }


        public IActionResult DeleteAppointmentType()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int AppointmentTypeId = Convert.ToInt32(Request.Form["AppointmentTypeId"]);
                if (AppointmentTypeId <= 0) { throw new Exception("Identity can not be blank!"); }
                TblAppointmentType oData = _dbContext.TblAppointmentTypes.Where(x => x.AppointmenTypeId == AppointmentTypeId && x.AccountId == AccountId).FirstOrDefault();
                _dbContext.TblAppointmentTypes.Remove(oData);
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Appointment Type deleted Sucessfully." });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }
        #endregion

        #region AppointmentOutcomes
        public IActionResult AppointmentOutcomes()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                {
                    using (var DB = _dbContext)
                    {
                        int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                        var oAppointmentOutcomeList = DB.TblAppointmentOutcomes.Where(x => x.AccountId == AccountId).ToList().Select(s => new AppointmentOutcomeViewModel
                        {
                            AppointmentOutcomeId = s.AppointmentOutcomeId,
                            AppointmentOutcomeName = s.AppointmentOutcomeName,
                        });
                        return View(oAppointmentOutcomeList);
                    }
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }

        }

        //[HttpGet]
        public IActionResult AddAppointmentOutcomes()
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult AddAppointmentOutcomes(AppointmentOutcomeViewModel model)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                if (ModelState.IsValid)
                {
                    int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                    TblAppointmentOutcome oData = new TblAppointmentOutcome();
                    oData.AccountId = AccountId;
                    oData.AppointmentOutcomeName = string.IsNullOrEmpty(model.AppointmentOutcomeName) ? string.Empty : model.AppointmentOutcomeName;
                    oData.CreatedDate = DateTime.Now;

                    _dbContext.TblAppointmentOutcomes.Add(oData);
                    _dbContext.SaveChanges();

                    ShowSuccessMessage("Appointment Outcome added Successfully.", true);
                    return RedirectToAction("AppointmentOutcomes", "Admin");
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                ShowErrorMessage(ex.Message, true);
                return View(model);
            }
        }


        public IActionResult GetAppointmentOutcomeDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int AppointmentOutcomeId = Convert.ToInt32(Request.Form["AppointmentOutcomeId"]);
                return Json(_dbContext.TblAppointmentOutcomes.Where(x => x.AccountId == AccountId && x.AppointmentOutcomeId == AppointmentOutcomeId).Select(x => new
                {
                    success = true,
                    AppointmentOutcomeId = x.AppointmentOutcomeId,
                    AppointmentOutcomeName = x.AppointmentOutcomeName
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public IActionResult UpdateAppointmentOutcomeDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int AppointmentOutcomeId = Convert.ToInt32(Request.Form["AppointmentOutcomeId"]);
                string Name = Request.Form["Name"];

                TblAppointmentOutcome oAppointmentOutCome = _dbContext.TblAppointmentOutcomes.Where(x => x.AppointmentOutcomeId == AppointmentOutcomeId && x.AccountId == AccountId).FirstOrDefault();
                if (oAppointmentOutCome != null)
                {
                    oAppointmentOutCome.AppointmentOutcomeName = Name;
                    oAppointmentOutCome.UpdatedDate = DateTime.Now;
                    _dbContext.Entry(oAppointmentOutCome).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No Appointment Type Found." });
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }


        public IActionResult DeleteAppointmentOutcome()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int AppointmentOutcomeId = Convert.ToInt32(Request.Form["AppointmentOutcomeId"]);
                if (AppointmentOutcomeId <= 0) { throw new Exception("Identity can not be blank!"); }
                TblAppointmentOutcome oData = _dbContext.TblAppointmentOutcomes.Where(x => x.AppointmentOutcomeId == AppointmentOutcomeId && x.AccountId == AccountId).FirstOrDefault();
                _dbContext.TblAppointmentOutcomes.Remove(oData);
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Appointment Outcome deleted Sucessfully." });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }
        #endregion

        #region CustomFields
        public IActionResult CustomFields()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                {
                    using (var DB = _dbContext)
                    {
                        int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                        var oCustomFieldList = DB.TblCustomFields.Where(x => x.AccountId == AccountId).Include(x => x.FieldType).ToList().Select(s => new CustomFieldViewModel
                        {
                            Id = s.Id,
                            FieldName = s.FieldName,
                            FieldTypeId = (int)s.FieldTypeId,
                            CustomFieldType = s.FieldType,
                            HideIfEmpty = (bool)s.HideIfEmpty
                        });
                        return View(oCustomFieldList);
                    }
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }

        }

        public IActionResult AddCustomFields()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                LoadDropDown();

                if (IsUserAuthorize())
                    return View();
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult AddCustomFields(CustomFieldViewModel model, string[] DynamicTextBox)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                LoadDropDown();

                if (ModelState.IsValid)
                {
                    int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                    TblCustomField oData = new TblCustomField();
                    oData.AccountId = AccountId;
                    oData.FieldTypeId = model.FieldTypeId;
                    oData.FieldName = string.IsNullOrEmpty(model.FieldName) ? string.Empty : model.FieldName;
                    oData.HideIfEmpty = model.HideIfEmpty;
                    oData.CreatedDate = DateTime.Now;

                    _dbContext.TblCustomFields.Add(oData);
                    _dbContext.SaveChanges();

                    if (DynamicTextBox[0] != null)
                    {
                        foreach (var item in DynamicTextBox)
                        {
                            TblCustomFieldValue oFieldValue = new TblCustomFieldValue();
                            oFieldValue.CustomFieldId = oData.Id;
                            oFieldValue.AccountId = AccountId;
                            oFieldValue.FieldValue = item.Trim();
                            oFieldValue.CreatedDate = DateTime.Now;
                            _dbContext.TblCustomFieldValues.Add(oFieldValue);
                            _dbContext.SaveChanges();
                        }
                    }
                    ShowSuccessMessage("Custom Field added Successfully.", true);
                    return RedirectToAction("CustomFields", "Admin");
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return View(model);
            }
        }

        public IActionResult GetCustomFieldDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int CustomFieldId = Convert.ToInt32(Request.Form["CustomFieldId"]);
                return Json(_dbContext.TblCustomFields.Where(x => x.AccountId == AccountId && x.Id == CustomFieldId).Include(x => x.TblCustomFieldValues.Where(y => y.CustomFieldId == CustomFieldId)).Select(x => new
                {
                    success = true,
                    Id = x.Id,
                    FieldTypeId = x.FieldTypeId,
                    FieldName = x.FieldName,
                    FieldValues = x.TblCustomFieldValues
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public IActionResult getCustomFieldType()
        {
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                //DatabaseEntities db = new DatabaseEntities();
                return Json(_dbContext.TblCustomFieldTypes.Select(x => new
                {
                    FieldTypeId = x.Id,
                    FieldType = x.FieldType
                }).ToList());
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public IActionResult UpdateCustomFieldDetails()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int CustomFieldId = Convert.ToInt32(Request.Form["CustomFieldId"]);
                int CustomFieldTypeId = Convert.ToInt32(Request.Form["CustomFieldTypeId"]);
                string Name = Request.Form["Name"];
                string DropDownOption = Request.Form["DropDownOption"];
                string[] ddlOptionvalue = null;
                if (DropDownOption != null)
                {
                    ddlOptionvalue = DropDownOption.Split(",");
                }

                TblCustomField oCustomField = _dbContext.TblCustomFields.Where(x => x.Id == CustomFieldId && x.AccountId == AccountId).FirstOrDefault();
                if (oCustomField != null)
                {
                    oCustomField.FieldName = Name;
                    oCustomField.UpdatedDate = DateTime.Now;
                    _dbContext.Entry(oCustomField).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    if (ddlOptionvalue.Length > 0)
                    {
                        var model = _dbContext.TblCustomFieldValues.Where(x => x.CustomFieldId == CustomFieldId && x.AccountId == AccountId).ToList();
                        if (model.Count > 0)
                        {
                            foreach (var obj in model)
                            {
                                _dbContext.Entry(obj).State = EntityState.Deleted;
                            }
                            _dbContext.SaveChanges();
                        }

                        foreach (var item in ddlOptionvalue)
                        {
                            TblCustomFieldValue oCustomFieldValue = new TblCustomFieldValue();
                            oCustomFieldValue.CustomFieldId = CustomFieldId;
                            oCustomFieldValue.AccountId = AccountId;
                            oCustomFieldValue.FieldValue = item.Trim();
                            oCustomFieldValue.CreatedDate = DateTime.Now;
                            _dbContext.TblCustomFieldValues.Add(oCustomFieldValue);
                            _dbContext.SaveChanges();
                        }
                    }
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No Custom Field Found." });
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public IActionResult DeleteCustomField()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int CustomFieldId = Convert.ToInt32(Request.Form["CustomFieldId"]);
                if (CustomFieldId <= 0) { throw new Exception("Identity can not be blank!"); }

                var model = _dbContext.TblCustomFieldValues.Where(x => x.CustomFieldId == CustomFieldId && x.AccountId == AccountId).ToList();
                if (model.Count > 0)
                {
                    foreach (var obj in model)
                    {
                        _dbContext.Entry(obj).State = EntityState.Deleted;
                    }
                    _dbContext.SaveChanges();
                }

                TblCustomField oData = _dbContext.TblCustomFields.Where(x => x.Id == CustomFieldId && x.AccountId == AccountId).FirstOrDefault();
                _dbContext.TblCustomFields.Remove(oData);
                _dbContext.SaveChanges();


                return Json(new { success = true, message = "Custom Field deleted Sucessfully." });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }
        #endregion


        #region EmailTemplate
        public IActionResult EmailTemplate()
        {
            try
            {
                LoadDropDown();

                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                {
                    using (var DB = _dbContext)
                    {
                        int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                        var oEmailTemplateList = DB.TblEmailTemplates.Where(x => x.AccountId == AccountId && x.IsType == EmailType.EmailTemplate.GetHashCode()).ToList().Select(s => new EmailTemplateViewModel
                        {
                            EmailTemplateID = s.EmailTemplateId,
                            EmailSubject = s.EmailSubject,
                            EmailName = s.EmailName,
                            EmailTemplateDescription = s.EmailTemplateDescription,
                            IsActive = (bool)s.IsActive
                        });
                        return View(oEmailTemplateList);
                    }
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }

        }

        public IActionResult AddEmailTemplate()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                LoadDropDown();

                if (IsUserAuthorize())
                    return View();
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult AddEmailTemplate(EmailTemplateViewModel model)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                LoadDropDown();

                if (ModelState.IsValid)
                {
                    int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                    TblEmailTemplate oData = new TblEmailTemplate();
                    oData.AccountId = AccountId;
                    oData.TemplateTypeId = model.TemplateTypeId;
                    oData.EmailName = string.IsNullOrEmpty(model.EmailName) ? string.Empty : model.EmailName;
                    oData.EmailTemplateDescription = string.IsNullOrEmpty(model.EmailTemplateDescription) ? string.Empty : model.EmailTemplateDescription;
                    oData.FromEmail = string.IsNullOrEmpty(model.FromEmail) ? string.Empty : model.FromEmail;
                    oData.EmailSubject = string.IsNullOrEmpty(model.EmailSubject) ? string.Empty : model.EmailSubject;
                    oData.Body = string.IsNullOrEmpty(model.Body) ? string.Empty : model.Body;
                    oData.IsType = EmailType.EmailTemplate.GetHashCode();
                    oData.IsActive = model.IsActive;
                    oData.CreatedDate = DateTime.Now;
                    //oData.UpdatedDate = DateTime.Now;
                    _dbContext.TblEmailTemplates.Add(oData);
                    _dbContext.SaveChanges();

                    ShowSuccessMessage("Email template added Successfully.", true);
                    return RedirectToAction("EmailTemplate", "Admin");
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                ShowErrorMessage(ex.Message, true);
                return View(model);
            }
        }

        public IActionResult EmailTemplateList()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                LoadDropDown();
                if (IsUserAuthorize())
                {
                    TemplateCategoryHTMLEmailList model = new TemplateCategoryHTMLEmailList();

                    var oCateGoryList = _dbContext.TblTemplateCategories.ToList();
                    List<TemplateCategoryHTMLDetails> Listmodel = new List<TemplateCategoryHTMLDetails>();
                    foreach (var item in oCateGoryList)
                    {
                        var TemplateCategoryHTMLDetailsModel = new TemplateCategoryHTMLDetails();
                        TemplateCategoryHTMLDetailsModel.CategotyName = item.TemplateCategoryName;

                        List<TemplateCategoryHTMLEmailViewModel> oList = new List<TemplateCategoryHTMLEmailViewModel>();
                        var oEmailTemplate = _dbContext.TblTemplateCategoryHtmlemails.Where(x => x.TemplateCategoryId == item.TemplateCategoryId).ToList();//.OrderByDescending(x=>x.Id)
                        if (oEmailTemplate.Count > 0)
                        {
                            foreach (var itemEmailTemplate in oEmailTemplate)
                            {
                                var oTemplateCategoryHTMLEmailList = new TemplateCategoryHTMLEmailViewModel();
                                oTemplateCategoryHTMLEmailList.TemplateCategoryHTMLEmailID = itemEmailTemplate.TemplateCategoryHtmlemailId;
                                oTemplateCategoryHTMLEmailList.TemplateCategoryId = itemEmailTemplate.TemplateCategoryId;
                                oTemplateCategoryHTMLEmailList.TemplateHTMLEmail = itemEmailTemplate.TemplateHtmlemail;
                                oTemplateCategoryHTMLEmailList.TemplateHTMLEmailDescription = itemEmailTemplate.TemplateHtmlemailDescription;
                                //var provider = new PhysicalFileProvider(Environment.WebRootPath);
                                //var contents = provider.GetDirectoryContents(Path.Combine("image", "email-img"));
                                oTemplateCategoryHTMLEmailList.TemplateHTMLImage = itemEmailTemplate.TemplateHtmlimage;
                                oList.Add(oTemplateCategoryHTMLEmailList);
                            }
                        }
                        TemplateCategoryHTMLDetailsModel.TemplateCategoryHTMLEmailList = oList;
                        Listmodel.Add(TemplateCategoryHTMLDetailsModel);
                    }
                    model.TemplateCategoryHTMLDetails = Listmodel;
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
                return View();
            }

        }

        public IActionResult GetTemplateCategoryHTMLEmailById()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int TemplateCategoryHTMLEmailID = Convert.ToInt32(Request.Form["TemplateCategoryHTMLEmailID"]);
                return Json(_dbContext.TblTemplateCategoryHtmlemails.Where(x => x.TemplateCategoryHtmlemailId == TemplateCategoryHTMLEmailID).Select(x => new
                {
                    success = true,
                    TemplateCategoryHtmlemailId = x.TemplateCategoryHtmlemailId,
                    TemplateCategoryId = x.TemplateCategoryId,
                    TemplateHtmlemailDescription = x.TemplateHtmlemailDescription,
                    TemplateHtmlemail = x.TemplateHtmlemail,
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }
        public IActionResult GetEmailTemplateById()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int EmailTemplateID = Convert.ToInt32(Request.Form["EmailTemplateID"]);
                return Json(_dbContext.TblEmailTemplates.Where(x => x.AccountId == AccountId && x.IsType == EmailType.EmailTemplate.GetHashCode() && x.EmailTemplateId == EmailTemplateID).Select(x => new
                {
                    success = true,
                    TemplateTypeId = x.TemplateTypeId,
                    EmailTemplateId = x.EmailTemplateId,
                    EmailName = x.EmailName,
                    EmailSubject = x.EmailSubject,
                    EmailTemplateDescription = x.EmailTemplateDescription,
                    Body = x.Body,
                    FromEmail = x.FromEmail,
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public IActionResult UpdateEmailTemplate()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int EmailTemplateID = Convert.ToInt32(Request.Form["EmailTemplateID"]);
                int TemplateTypeID = Convert.ToInt32(Request.Form["TemplateTypeID"]);
                string EmailName = Request.Form["EmailName"];
                string Description = Request.Form["Description"];
                string FromEmail = Request.Form["FromEmail"];
                string EmailSubject = Request.Form["EmailSubject"];
                string MailBody = Request.Form["MailBody"];

                TblEmailTemplate oEmailTemplate = _dbContext.TblEmailTemplates.Where(x => x.EmailTemplateId == EmailTemplateID && x.AccountId == AccountId && x.IsType == EmailType.EmailTemplate.GetHashCode()).FirstOrDefault();
                if (oEmailTemplate != null)
                {
                    oEmailTemplate.AccountId = AccountId;
                    oEmailTemplate.TemplateTypeId = TemplateTypeID;
                    oEmailTemplate.EmailName = EmailName;
                    oEmailTemplate.EmailTemplateDescription = Description;
                    oEmailTemplate.FromEmail = FromEmail;
                    oEmailTemplate.EmailSubject = EmailSubject;
                    oEmailTemplate.Body = MailBody;
                    oEmailTemplate.IsType = EmailType.EmailTemplate.GetHashCode();
                    oEmailTemplate.UpdatedDate = DateTime.Now;
                    _dbContext.Entry(oEmailTemplate).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No Email Template Found." });
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public IActionResult ActivateDeactivateEmailTemplate()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int EmailTemplateID = Convert.ToInt32(Request.Form["EmailTemplateID"]);
                int Flag = Convert.ToInt32(Request.Form["flag"]);
                TblEmailTemplate EmailTemplate = _dbContext.TblEmailTemplates.FirstOrDefault(c => c.EmailTemplateId == EmailTemplateID && c.AccountId == AccountId && c.IsType == EmailType.EmailTemplate.GetHashCode());
                if (Flag >= 1)
                    EmailTemplate.IsActive = true;
                else
                    EmailTemplate.IsActive = false;
                EmailTemplate.UpdatedDate = DateTime.Now;
                _dbContext.Entry(EmailTemplate).State = EntityState.Modified;
                _dbContext.SaveChanges();
                if (Flag >= 1)
                    return Json(new { Act = true, message = "Email template is activated" });
                else
                    return Json(new { DeAct = true, message = "Email template is de-activated" });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { Error = false });
            }
        }

        public IActionResult DeleteEmailTemplate()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int EmailTemplateID = Convert.ToInt32(Request.Form["EmailTemplateID"]);
                if (EmailTemplateID <= 0) { throw new Exception("Identity can not be blank!"); }
                TblEmailTemplate oData = _dbContext.TblEmailTemplates.Where(x => x.EmailTemplateId == EmailTemplateID && x.AccountId == AccountId && x.IsType == EmailType.EmailTemplate.GetHashCode()).FirstOrDefault();
                _dbContext.TblEmailTemplates.Remove(oData);
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Email template deleted Sucessfully." });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }


        public IActionResult UpdateTemplateCategoryHTMLEmail()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                //int EmailTemplateID = Convert.ToInt32(Request.Form["EmailTemplateID"]);
                int TemplateTypeID = Convert.ToInt32(Request.Form["TemplateTypeID"]);
                string EmailName = Request.Form["EmailName"];
                string Description = Request.Form["Description"];
                string FromEmail = Request.Form["FromEmail"];
                string EmailSubject = Request.Form["EmailSubject"];
                string MailBody = Request.Form["MailBody"];
                bool Status = Convert.ToBoolean(Request.Form["Status"]);
                TblEmailTemplate oData = new TblEmailTemplate();
                oData.AccountId = AccountId;
                oData.TemplateTypeId = TemplateTypeID;
                oData.EmailName = string.IsNullOrEmpty(EmailName) ? string.Empty : EmailName;
                oData.EmailTemplateDescription = string.IsNullOrEmpty(Description) ? string.Empty : Description;
                oData.FromEmail = string.IsNullOrEmpty(FromEmail) ? string.Empty : FromEmail;
                oData.EmailSubject = string.IsNullOrEmpty(EmailSubject) ? string.Empty : EmailSubject;
                oData.Body = string.IsNullOrEmpty(MailBody) ? string.Empty : MailBody;
                oData.IsType = EmailType.EmailTemplate.GetHashCode();
                oData.IsActive = Status;
                oData.CreatedDate = DateTime.Now;
                //oData.UpdatedDate = DateTime.Now;
                _dbContext.TblEmailTemplates.Add(oData);
                _dbContext.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }
        #endregion


        #region TextTemplate
        public IActionResult TextTemplate()
        {
            try
            {
                LoadDropDown();

                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (IsUserAuthorize())
                {
                    using (var DB = _dbContext)
                    {
                        int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                        var oEmailTemplateList = DB.TblEmailTemplates.Where(x => x.AccountId == AccountId && x.IsType == EmailType.TextTemplate.GetHashCode()).ToList().Select(s => new EmailTemplateViewModel
                        {
                            EmailTemplateID = s.EmailTemplateId,
                            EmailSubject = s.EmailSubject,
                            EmailName = s.EmailName,
                            EmailTemplateDescription = s.EmailTemplateDescription,
                            IsActive = (bool)s.IsActive
                        });
                        return View(oEmailTemplateList);
                    }
                }
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }

        }

        public IActionResult AddTextTemplate()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                LoadDropDown();

                if (IsUserAuthorize())
                    return View();
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult AddTextTemplate(EmailTemplateViewModel model)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                LoadDropDown();

                if (ModelState.IsValid)
                {
                    int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                    TblEmailTemplate oData = new TblEmailTemplate();
                    oData.AccountId = AccountId;
                    oData.TemplateTypeId = model.TemplateTypeId;
                    oData.EmailName = string.IsNullOrEmpty(model.EmailName) ? string.Empty : model.EmailName;
                    oData.EmailTemplateDescription = string.IsNullOrEmpty(model.EmailTemplateDescription) ? string.Empty : model.EmailTemplateDescription;
                    oData.FromEmail = string.IsNullOrEmpty(model.FromEmail) ? string.Empty : model.FromEmail;
                    oData.EmailSubject = string.IsNullOrEmpty(model.EmailSubject) ? string.Empty : model.EmailSubject;
                    oData.Body = string.IsNullOrEmpty(model.Body) ? string.Empty : model.Body;
                    oData.IsType = EmailType.TextTemplate.GetHashCode();
                    oData.IsActive = model.IsActive;
                    oData.CreatedDate = DateTime.Now;
                    //oData.UpdatedDate = DateTime.Now;
                    _dbContext.TblEmailTemplates.Add(oData);
                    _dbContext.SaveChanges();

                    ShowSuccessMessage("Text template added Successfully.", true);
                    return RedirectToAction("TextTemplate", "Admin");
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return View(model);
            }
        }

        public IActionResult GetTextTemplateById()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int EmailTemplateID = Convert.ToInt32(Request.Form["EmailTemplateID"]);
                return Json(_dbContext.TblEmailTemplates.Where(x => x.AccountId == AccountId && x.IsType == EmailType.TextTemplate.GetHashCode() && x.EmailTemplateId == EmailTemplateID).Select(x => new
                {
                    success = true,
                    TemplateTypeId = x.TemplateTypeId,
                    EmailTemplateId = x.EmailTemplateId,
                    EmailName = x.EmailName,
                    EmailSubject = x.EmailSubject,
                    EmailTemplateDescription = x.EmailTemplateDescription,
                    Body = x.Body,
                    FromEmail = x.FromEmail,
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public IActionResult UpdateTextTemplate()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int EmailTemplateID = Convert.ToInt32(Request.Form["EmailTemplateID"]);
                int TemplateTypeID = Convert.ToInt32(Request.Form["TemplateTypeID"]);
                string EmailName = Request.Form["EmailName"];
                string Description = Request.Form["Description"];
                string FromEmail = Request.Form["FromEmail"];
                string EmailSubject = Request.Form["EmailSubject"];
                string MailBody = Request.Form["MailBody"];

                TblEmailTemplate oEmailTemplate = _dbContext.TblEmailTemplates.Where(x => x.EmailTemplateId == EmailTemplateID && x.AccountId == AccountId && x.IsType == EmailType.TextTemplate.GetHashCode()).FirstOrDefault();
                if (oEmailTemplate != null)
                {
                    oEmailTemplate.AccountId = AccountId;
                    oEmailTemplate.TemplateTypeId = TemplateTypeID;
                    oEmailTemplate.EmailName = EmailName;
                    oEmailTemplate.EmailTemplateDescription = Description;
                    oEmailTemplate.FromEmail = FromEmail;
                    oEmailTemplate.EmailSubject = EmailSubject;
                    oEmailTemplate.Body = MailBody;
                    oEmailTemplate.IsType = EmailType.TextTemplate.GetHashCode();
                    oEmailTemplate.UpdatedDate = DateTime.Now;
                    _dbContext.Entry(oEmailTemplate).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No Text Template Found." });
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public IActionResult ActivateDeactivateTextTemplate()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int EmailTemplateID = Convert.ToInt32(Request.Form["EmailTemplateID"]);
                int Flag = Convert.ToInt32(Request.Form["flag"]);
                TblEmailTemplate EmailTemplate = _dbContext.TblEmailTemplates.FirstOrDefault(c => c.EmailTemplateId == EmailTemplateID && c.AccountId == AccountId && c.IsType == EmailType.TextTemplate.GetHashCode());
                if (Flag >= 1)
                    EmailTemplate.IsActive = true;
                else
                    EmailTemplate.IsActive = false;
                EmailTemplate.UpdatedDate = DateTime.Now;
                _dbContext.Entry(EmailTemplate).State = EntityState.Modified;
                _dbContext.SaveChanges();
                if (Flag >= 1)
                    return Json(new { Act = true, message = "Text template is activated" });
                else
                    return Json(new { DeAct = true, message = "Text template is de-activated" });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { Error = false });
            }
        }

        public IActionResult DeleteTextTemplate()
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                int EmailTemplateID = Convert.ToInt32(Request.Form["EmailTemplateID"]);
                if (EmailTemplateID <= 0) { throw new Exception("Identity can not be blank!"); }
                TblEmailTemplate oData = _dbContext.TblEmailTemplates.Where(x => x.EmailTemplateId == EmailTemplateID && x.AccountId == AccountId && x.IsType == EmailType.TextTemplate.GetHashCode()).FirstOrDefault();
                _dbContext.TblEmailTemplates.Remove(oData);
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Text template deleted Sucessfully." });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }
        #endregion
        public void LoadDropDown()
        {
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);

                Dictionary<int, string> sFieldTypeList = new Dictionary<int, string>();
                var FieldTypes = _dbContext.TblCustomFieldTypes.ToList();
                foreach (var item in FieldTypes)
                {
                    sFieldTypeList.Add(item.Id, item.FieldType);
                }
                ViewBag.FieldTypeList = sFieldTypeList;


                Dictionary<int, string> sTemplateTypeList = new Dictionary<int, string>();
                var TemplateTypes = _dbContext.TblTemplateTypes.ToList();
                foreach (var item in TemplateTypes)
                {
                    sTemplateTypeList.Add(item.TemplateTypeId, item.TypeName);
                }
                ViewBag.TemplateTypeList = sTemplateTypeList;

                Dictionary<int, string> sTemplateCategoryList = new Dictionary<int, string>();
                var TemplateCategoryList = _dbContext.TblTemplateCategories.ToList();
                foreach (var item in TemplateCategoryList)
                {
                    sTemplateCategoryList.Add(item.TemplateCategoryId, item.TemplateCategoryName);
                }
                ViewBag.TemplateCategoryList = sTemplateCategoryList;
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, Environment.WebRootPath);
            }
        }

        public IActionResult uploadnow(IFormFile upload)
        {
            string path = "";
            string wwwPath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + upload.FileName;
            //Path.Combine(Directory.GetCurrentDirectory(), this.Environment.WebRootPath, "image/uploads/", fileName);
            path = Path.Combine(this.Environment.WebRootPath, "image/uploads/", fileName); //Path.Combine(this.Environment.WebRootPath, "image/uploads/", fileName);
            var stream = new FileStream(path, FileMode.Create);
            upload.CopyToAsync(stream);
            //return Json(new { path = @"image\uploads\" + fileName });
            return new JsonResult(new { path1 = "image/uploads/" + fileName });
            //string path = "";
            //string pathWeb = "";
            //if (upload != null)
            //{
            //    string wwwPath = this.Environment.WebRootPath;
            //    string contentPath = this.Environment.ContentRootPath;

            //    path = Path.Combine(this.Environment.WebRootPath, @"image\uploads\");
            //    if (!Directory.Exists(path))
            //    {
            //        Directory.CreateDirectory(path);
            //    }

            //    string fileName = Path.GetFileName(upload.FileName);
            //    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            //    {
            //        upload.CopyTo(stream);
            //    }
            //    return fileName;
            //}
            //return null;
        }

        public IActionResult UploadImage(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            if (upload.Length <= 0) return null;

            var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();

            var path = Path.Combine(
                Environment.WebRootPath, "image/uploads/",
                fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                upload.CopyTo(stream);

            }
            var url = $"{"/image/uploads/"}{fileName}";
            return Json(new { uploaded = true, url });
        }

        public ActionResult uploadPartial()
        {
            string webRootPath = Environment.WebRootPath;
            string path = "";
            path = Path.Combine(webRootPath, "image/uploads/");
            var images = Directory.GetFiles(path).Select(x => new CkEditorImagesViewModel
            {
                Url = Url.Content("/image/uploads/" + Path.GetFileName(x))
            });
            return View(images);

        }
    }
}
