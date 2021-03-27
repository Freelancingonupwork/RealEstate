using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models;
using RealEstate.Utills;
using RealEstateDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private RealEstateContext _dbContext;
        public AdminController(RealEstateContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Agent/Team
        [Route("Admin/")]
        public IActionResult LeadFlow()
        {
            try
            {
                if (IsUserAuthorize())
                    return View();
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ErrorLog.log("Admin Controller LeadFlow :-" + ex.Message);
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Team()
        {
            try
            {
                if (IsUserAuthorize())
                {
                    using (var DB = _dbContext)
                    {
                        var oAgentList = DB.TblAgents.ToList().Select(s => new AgentViewModel
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
                ErrorLog.log("Admin Controller Team :-" + ex.Message);
                return RedirectToAction("Login", "Account");
            }

        }

        [HttpGet]
        public IActionResult AddAgents()
        {
            try
            {
                if (IsUserAuthorize())
                    return View();
                else
                    return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ErrorLog.log("Admin Controller AddAgents :-" + ex.Message);
                return RedirectToAction("Login", "Account");
            }

        }

        [HttpPost]
        public IActionResult AddAgents(AgentViewModel model)
        {
            try
            {
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
                    oData.IsActive = model.IsActive;
                    oData.CreatedDate = DateTime.Now;

                    _dbContext.TblAgents.Add(oData);
                    _dbContext.SaveChanges();

                    ViewBag.MessageType = "success";
                    ViewBag.Message = "User added Successfully.";
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

        public IActionResult ActivateDeactivateUser()
        {
            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                int Flag = Convert.ToInt32(Request.Form["flag"]);
                TblAgent agent = _dbContext.TblAgents.SingleOrDefault(c => c.Id == id);

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
                ErrorLog.log("AdminController ActivateDeactivateUser" + ex);
                return Json(new { Error = false });
            }
        }

        public IActionResult DeleteAgent()
        {
            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int id = 0;
                if (Request.Form.ContainsKey("id")) { id = Convert.ToInt32(Request.Form["id"]); }
                if (id <= 0) { throw new Exception("Identity can not be blank!"); }
                _dbContext.TblAgents.Remove(_dbContext.TblAgents.Where(x => x.Id == id).FirstOrDefault());
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorLog.log("AdminController DeleteAgent" + ex);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }

        public IActionResult UpdateAgent()
        {
            try
            {
                int intAgentID = Convert.ToInt32(Request.Form["agentID"]);
                int intLeadID = Convert.ToInt32(Request.Form["leadID"]);
                TblLead lead = _dbContext.TblLeads.Where(x => x.LeadId == intLeadID).FirstOrDefault();
                lead.AgentId = intAgentID <= 0 ? Convert.ToInt32(DBNull.Value) : intAgentID;
                _dbContext.Entry(lead).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorLog.log("AdminController UpdateAgent" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public IActionResult GetAgentDetails()
        {
            try
            {
                int id = Convert.ToInt32(Request.Form["agentID"]);
                return Json(_dbContext.TblAgents.Where(x => x.Id == id).Select(x => new
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
                ErrorLog.log("AdminController GetAgentDetails" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public IActionResult UpdateAgentDetails()
        {
            try
            {
                int intAgentID = Convert.ToInt32(Request.Form["agentID"]);
                string strfullName = Request.Form["fullName"];

                string stremailAddress = Request.Form["emailAddress"];
                string strcellPhone = Request.Form["cellPhone"];

                if (_dbContext.TblAgents.Where(x => x.EmailAddress.Equals(stremailAddress) && x.Id != intAgentID).FirstOrDefault() != null)
                    return Json(new { success = false, message = "Email address is already in use! Try another email address!" });

                TblAgent oAgent = _dbContext.TblAgents.Where(x => x.Id == intAgentID).FirstOrDefault();
                oAgent.FullName = strfullName;
                oAgent.EmailAddress = stremailAddress;
                oAgent.CellPhone = strcellPhone;
                oAgent.CreatedDate = DateTime.Now;
                _dbContext.Entry(oAgent).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorLog.log("AdminController UpdateAgentDetails" + ex);
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
                    if (Convert.ToInt32(Request.Cookies["UserLoginTypeId"]) != UserLoginType.Admin.GetHashCode())
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ErrorLog.log("Admin Controller IsUserAuthorize :-" + ex.Message);
                return false;
            }

        }

        public bool IsAdminExist(string email)
        {
            try
            {
                var result = _dbContext.TblUsers.Where(u => u.EmailAddress.ToLower().Equals(email.ToLower())).ToList();
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
                if (IsUserAuthorize())
                {
                    using (var DB = _dbContext)
                    {
                        var oAdminUserList = DB.TblUsers.Where(x => x.UserLoginTypeId == (int)UserLoginType.Admin).ToList().Select(s => new UserViewModel
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
                if (!IsUserAuthorize())
                    return RedirectToAction("Login", "Account");

                if (ModelState.IsValid)
                {
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

                    _dbContext.TblUsers.Add(oData);
                    _dbContext.SaveChanges();

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
                ErrorLog.log("AdminController AddAdminUser" + ex);
                ShowErrorMessage(ex.Message, true);
                return View(model);
            }
        }


        public ActionResult UpdateAdminDetails()
        {
            try
            {
                int intAdminID = Convert.ToInt32(Request.Form["adminID"]);
                string strfullName = Request.Form["fullName"];

                string stremailAddress = Request.Form["emailAddress"];

                if (_dbContext.TblUsers.Where(x => x.EmailAddress.Equals(stremailAddress) && x.UserId != intAdminID).FirstOrDefault() != null)
                    return Json(new { success = false, message = "Email address is already in use! Try another email address!" });

                TblUser user = _dbContext.TblUsers.Where(x => x.UserId == intAdminID).FirstOrDefault();
                user.FullName = strfullName;
                user.EmailAddress = stremailAddress;
                _dbContext.Entry(user).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorLog.log("AdminController UpdateAdminDetails" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public ActionResult GetAdminDetails()
        {
            try
            {
                int id = Convert.ToInt32(Request.Form["adminID"]);
                return Json(_dbContext.TblUsers.Where(x => x.UserId == id).Select(x => new
                {
                    success = true,
                    adminID = x.UserId,
                    fullName = x.FullName,
                    emailAddress = x.EmailAddress
                }).FirstOrDefault());
            }
            catch (Exception ex)
            {
                ErrorLog.log("AdminController GetAgentDetails" + ex);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }

        }

        public ActionResult ActivateDeactivateAdmin()
        {
            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                int Flag = Convert.ToInt32(Request.Form["flag"]);
                TblUser user = _dbContext.TblUsers.FirstOrDefault(c => c.UserId == id);
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
                ErrorLog.log("AdminController ActivateDeactivateAdmin" + ex);
                return Json(new { Error = false });
            }
        }

        public ActionResult DeleteAdmin()
        {
            if (!IsUserAuthorize())
                return RedirectToAction("Login", "Account");
            try
            {
                int id = 0;
                if (Request.Form.ContainsKey("id")) { id = Convert.ToInt32(Request.Form["id"]); }
                if (id <= 0) { throw new Exception("Identity can not be blank!"); }
                TblUser user = _dbContext.TblUsers.Where(x => x.UserId == id).FirstOrDefault();
                if (user.EmailAddress.Equals(Request.Cookies["EmailAddress"].ToString()))
                    return Json(new { success = false, message = "You can not delete your self!" });
                _dbContext.TblUsers.Remove(user);
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorLog.log("AdminController DeleteAdmin" + ex);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }
        #endregion
    }
}
