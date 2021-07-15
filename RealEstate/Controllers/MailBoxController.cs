using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RealEstate.Models;
using RealEstate.Utills;
using RealEstateDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RealEstate.Controllers
{
    public class MailBoxController : BaseController
    {

        private RealEstateContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        public MailBoxController(RealEstateContext dbContext, IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
        }


        public IActionResult Index()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                using (var DB = _dbContext)
                {
                    int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);


                    //var oMainMsgList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.IsReplay == false).OrderByDescending(x => x.CreatedDate).ToList().Select(s => new LeadEmailMessageViewModel
                    //{
                    //    LeadId = s.LeadId,
                    //    LeadEmailMessageId = s.LeadEmailMessageId,
                    //    AccountId = s.AccountId,
                    //    Subject = s.Subject,
                    //    Body = s.Body,
                    //    FromName = s.FromName,
                    //    ToName = s.ToName,
                    //    IsReplay = s.IsReplay,
                    //    EmailMessageId = s.EmailMessageId,
                    //    CreatedDate = s.CreatedDate
                    //});


                    LeadEmailDetails oModel = new LeadEmailDetails();
                    var oMainMsgList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.IsReplay == false && x.IsType == MessageType.EmailMessage.GetHashCode()).OrderByDescending(x => x.CreatedDate).ToList();
                    List<LeadEmailMessageViewModel> model = new List<LeadEmailMessageViewModel>();
                    foreach (var item in oMainMsgList)
                    {
                        var MainMessageList = new LeadEmailMessageViewModel();
                        MainMessageList.AccountId = (int)item.AccountId;
                        MainMessageList.Body = item.Body;
                        MainMessageList.CreatedDate = (DateTime)item.CreatedDate;
                        MainMessageList.EmailMessageId = item.EmailMessageId;
                        MainMessageList.IsReplay = item.IsReplay;
                        MainMessageList.IsRead = item.IsRead;
                        MainMessageList.LeadEmailMessageId = item.LeadEmailMessageId;
                        MainMessageList.LeadId = item.LeadId;
                        MainMessageList.Subject = item.Subject;
                        MainMessageList.ToName = ToTitleCase(item.ToName);
                        MainMessageList.FromName = ToTitleCase(item.FromName);
                        MainMessageList.imgcontain = FetchImgsFromSource(item.Body);
                        MainMessageList.isAttachmentcontain = FetchAttachmentFromSource(item.LeadEmailMessageId);
                        model.Add(MainMessageList);
                    }
                    oModel.LeadEmailMessageViewModel = model;
                    return View(oModel);
                }

            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, _webHostEnvironment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }

        }

        public IActionResult GetAllTextMessage()
        {
            try
            {
                if (Request.Cookies.ContainsKey("FullName") && Request.Cookies.ContainsKey("EmailAddress"))
                {
                    int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                    LeadEmailDetails oModel = new LeadEmailDetails();
                    var oMainMsgList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.IsReplay == false && x.IsType == MessageType.TextMessage.GetHashCode()).OrderByDescending(x => x.CreatedDate).ToList();
                    if (oMainMsgList.Count >= 1)
                    {
                        List<LeadEmailMessageViewModel> model = new List<LeadEmailMessageViewModel>();
                        foreach (var item in oMainMsgList)
                        {
                            var MainMessageList = new LeadEmailMessageViewModel();
                            MainMessageList.AccountId = (int)item.AccountId;
                            MainMessageList.Body = item.Body;
                            MainMessageList.CreatedDate = (DateTime)item.CreatedDate;
                            MainMessageList.EmailMessageId = item.EmailMessageId;
                            MainMessageList.IsReplay = item.IsReplay;
                            MainMessageList.IsRead = item.IsRead;
                            MainMessageList.LeadEmailMessageId = item.LeadEmailMessageId;
                            MainMessageList.LeadId = item.LeadId;
                            MainMessageList.Subject = item.Subject;
                            MainMessageList.ToName = ToTitleCase(item.ToName);
                            MainMessageList.FromName = ToTitleCase(item.FromName);
                            MainMessageList.imgcontain = FetchImgsFromSource(item.Body);
                            MainMessageList.isAttachmentcontain = FetchAttachmentFromSource(item.LeadEmailMessageId);
                            model.Add(MainMessageList);
                        }
                        //oModel.LeadEmailMessageViewModel = model;
                        return Json(new { success = true, data = model });
                    }
                    else
                    {
                        return Json(new { success = true, message = "No Text Message Found." });
                    }
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
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, _webHostEnvironment.WebRootPath);
                return Json(new { success = false, message = "Opps! Something went wrong!" });
            }
        }

        public IActionResult LoadBodyById(int Id)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                if (Id > 0)
                {
                    int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                    string FromEmail = string.Empty;
                    var oAccountIntegrations = _dbContext.TblAccountIntegrations.Where(x => x.AccountId == AccountId && x.AuthAccountType != AuthAccountType.HubSportAuth.GetHashCode()).FirstOrDefault();
                    if (oAccountIntegrations != null)
                    {
                        FromEmail = oAccountIntegrations.EmailAddress;
                    }
                    else
                    {
                        FromEmail = "N/A";
                    }

                    var oLeadEmailMessage = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.LeadEmailMessageId == Id && x.IsType == MessageType.EmailMessage.GetHashCode()).FirstOrDefault();
                    if (oLeadEmailMessage != null)
                    {
                        oLeadEmailMessage.IsRead = true;
                        _dbContext.SaveChanges();
                    }

                    LeadEmailDetails oModel = new LeadEmailDetails();
                    var oLeadEmailMessageList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.IsType == MessageType.EmailMessage.GetHashCode()).Include(x => x.Lead).ToList();
                    if (oLeadEmailMessageList.Count > 0)
                    {
                        var oMainMsgList = oLeadEmailMessageList.Where(x => x.IsReplay == false && x.LeadEmailMessageId == Id).ToList();
                        var oReplayMsgList = oLeadEmailMessageList.Where(x => x.IsReplay == true && x.EmailMessageId == Id).ToList();

                        List<LeadEmailMessageViewModel> model = new List<LeadEmailMessageViewModel>();
                        foreach (var item in oMainMsgList)
                        {
                            var MainMessageList = new LeadEmailMessageViewModel();
                            MainMessageList.AccountId = (int)item.AccountId;
                            MainMessageList.Body = item.Body;
                            MainMessageList.CreatedDate = (DateTime)item.CreatedDate;
                            MainMessageList.EmailMessageId = item.EmailMessageId;
                            MainMessageList.IsReplay = item.IsReplay;
                            MainMessageList.IsRead = item.IsRead;
                            MainMessageList.LeadEmailMessageId = item.LeadEmailMessageId;
                            MainMessageList.LeadId = item.LeadId;
                            MainMessageList.Subject = item.Subject;
                            MainMessageList.ToName = ToTitleCase(item.ToName);
                            MainMessageList.FromName = ToTitleCase(item.FromName);
                            MainMessageList.ToEmail = ToTitleCase(item.Lead.EmailAddress);
                            MainMessageList.FromEmail = ToTitleCase(FromEmail);
                            MainMessageList.LeadEmailMessageReplayList = new List<LeadEmailMessageViewModel>();
                            MainMessageList.LeadEmailMessageattachement = new List<LeadEmailMessageAttachement>();
                            foreach (var itemReplay in oReplayMsgList.Where(x => x.EmailMessageId == item.LeadEmailMessageId))
                            {
                                var ReplayMessageList = new LeadEmailMessageViewModel();
                                ReplayMessageList.AccountId = (int)itemReplay.AccountId;
                                ReplayMessageList.Body = itemReplay.Body;
                                ReplayMessageList.CreatedDate = (DateTime)itemReplay.CreatedDate;
                                ReplayMessageList.EmailMessageId = itemReplay.EmailMessageId;
                                ReplayMessageList.IsReplay = itemReplay.IsReplay;
                                ReplayMessageList.LeadEmailMessageId = itemReplay.LeadEmailMessageId;
                                ReplayMessageList.LeadId = itemReplay.LeadId;
                                ReplayMessageList.Subject = itemReplay.Subject;
                                ReplayMessageList.ToName = ToTitleCase(itemReplay.ToName);
                                ReplayMessageList.FromName = ToTitleCase(itemReplay.FromName);
                                ReplayMessageList.ToEmail = ToTitleCase(FromEmail);
                                ReplayMessageList.FromEmail = ToTitleCase(itemReplay.Lead.EmailAddress);
                                ReplayMessageList.LeadEmailMessageReplayAttachement = new List<LeadEmailMessageReplayAttachement>();
                                foreach (var itemAttachment in _dbContext.TblLeadEmailMessageAttachments.Where(x => x.LeadEmailMessageId == itemReplay.LeadEmailMessageId).ToList())
                                {
                                    var MessageAttachmentList = new LeadEmailMessageReplayAttachement();
                                    MessageAttachmentList.LeadEmailMessageId = (int)itemAttachment.LeadEmailMessageId;
                                    MessageAttachmentList.LeadId = (int)itemReplay.LeadId;
                                    MessageAttachmentList.FileName = itemAttachment.Attachement;
                                    ReplayMessageList.LeadEmailMessageReplayAttachement.Add(MessageAttachmentList);
                                }
                                MainMessageList.LeadEmailMessageReplayList.Add(ReplayMessageList);
                            }
                            foreach (var itemAttachment in _dbContext.TblLeadEmailMessageAttachments.Where(x => x.LeadEmailMessageId == item.LeadEmailMessageId).ToList())
                            {
                                var MessageAttachmentList = new LeadEmailMessageAttachement();
                                MessageAttachmentList.LeadEmailMessageId = (int)itemAttachment.LeadEmailMessageId;
                                MessageAttachmentList.LeadId = (int)item.LeadId;
                                MessageAttachmentList.FileName = itemAttachment.Attachement;
                                MainMessageList.LeadEmailMessageattachement.Add(MessageAttachmentList);
                            }
                            model.Add(MainMessageList);
                        }
                        oModel.LeadEmailMessageViewModel = model;
                        return PartialView("_MailBody", oModel);
                    }
                    else
                    {
                        ShowWarningMessage("No Mail Found", true);
                        return View("Index", "MailBox");
                    }
                }
                else
                {
                    ShowWarningMessage("No Mail Details Found", true);
                    return View("Index", "MailBox");
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, _webHostEnvironment.WebRootPath);
                return RedirectToAction("Login", "Account");
            }

        }

        public IActionResult LoadLeadDetailsById(int Id, int LeadId)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                LeadEmailDetails oModel = new LeadEmailDetails();
                var oLeadEmailMessageList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.LeadEmailMessageId == Id && x.IsType == MessageType.EmailMessage.GetHashCode()).Include(x => x.Lead).ToList();
                if (oLeadEmailMessageList.Count > 0)
                {
                    var LeadDetails = oLeadEmailMessageList.Select(x => x.Lead).FirstOrDefault();
                    if (LeadDetails != null)
                    {
                        if (LeadDetails.AgentId > 0 && LeadDetails.AgentId != null)
                        {
                            var AgentName = _dbContext.TblAccounts.Where(x => x.AccountId == LeadDetails.AgentId).FirstOrDefault().FullName;
                            if (AgentName != null)
                            {
                                oModel.AgentName = ToTitleCase(AgentName);
                            }
                            else
                            {
                                oModel.AgentName = "N/A";
                            }
                        }
                        else
                        {
                            oModel.AgentName = "N/A";
                        }

                        if (LeadDetails.StageId > 0 && LeadDetails.StageId != null)
                        {
                            var stageName = _dbContext.TblStages.Where(x => x.StageId == LeadDetails.StageId).FirstOrDefault().StageName;
                            if (stageName != null)
                            {
                                oModel.StageName = stageName;
                            }
                            else
                            {
                                oModel.StageName = "N/A";
                            }
                        }
                        else
                        {
                            oModel.StageName = "N/A";
                        }

                        oModel.LeadName = ToTitleCase(LeadDetails.FirstName + " " + LeadDetails.LastName);
                    }


                    string PhoneNumber = oLeadEmailMessageList.Select(x => x.Lead).FirstOrDefault().PhoneNumber;
                    string EmailAddress = oLeadEmailMessageList.Select(x => x.Lead).FirstOrDefault().EmailAddress;

                    oModel.EmailAddress = EmailAddress == null ? "N/A" : EmailAddress;
                    oModel.Phonenumber = PhoneNumber == null ? "N/A" : PhoneNumber;

                    var oMainMsgList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.IsReplay == false && x.LeadId == LeadId && x.IsType == MessageType.EmailMessage.GetHashCode()).OrderByDescending(x => x.CreatedDate).ToList().Take(5);

                    List<LeadEmailMessageViewModel> model = new List<LeadEmailMessageViewModel>();
                    foreach (var item in oMainMsgList)
                    {
                        var MainMessageList = new LeadEmailMessageViewModel();
                        MainMessageList.AccountId = (int)item.AccountId;
                        MainMessageList.Body = item.Body;
                        MainMessageList.CreatedDate = (DateTime)item.CreatedDate;
                        MainMessageList.EmailMessageId = item.EmailMessageId;
                        MainMessageList.IsReplay = item.IsReplay;
                        MainMessageList.LeadEmailMessageId = item.LeadEmailMessageId;
                        MainMessageList.LeadId = item.LeadId;
                        MainMessageList.Subject = item.Subject;
                        MainMessageList.ToName = ToTitleCase(item.ToName);
                        MainMessageList.FromName = ToTitleCase(item.FromName);
                        model.Add(MainMessageList);
                    }
                    oModel.LeadEmailMessageViewModel = model;
                    return PartialView("_MailBodyRight", oModel);
                }
                else
                {
                    ShowWarningMessage("No Mail Found", true);
                    return View("Index", "MailBox");
                }

            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, _webHostEnvironment.WebRootPath);
                return null;
                //return RedirectToAction("Login", "Account");
            }

        }

        //public IActionResult LoadLeadDetailsById()
        //{
        //    try
        //    {
        //        if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
        //            return RedirectToAction("Login", "Account");

        //        int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
        //        LeadEmailDetails oModel = new LeadEmailDetails();
        //        var oLeadEmailMessageList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId).Include(x => x.Lead).ToList();
        //        if (oLeadEmailMessageList.Count > 0)
        //        {
        //            var LeadDetails = oLeadEmailMessageList.Select(x => x.Lead).FirstOrDefault();
        //            if (LeadDetails != null)
        //            {
        //                if (LeadDetails.AgentId > 0 && LeadDetails.AgentId != null)
        //                {
        //                    var AgentName = _dbContext.TblAccounts.Where(x => x.AccountId == LeadDetails.AgentId).FirstOrDefault().FullName;
        //                    if (AgentName != null)
        //                    {
        //                        oModel.AgentName = ToTitleCase(AgentName);
        //                    }
        //                    else
        //                    {
        //                        oModel.AgentName = "N/A";
        //                    }
        //                }
        //                else
        //                {
        //                    oModel.AgentName = "N/A";
        //                }

        //                if (LeadDetails.StageId > 0 && LeadDetails.StageId != null)
        //                {
        //                    var stageName = _dbContext.TblStages.Where(x => x.StageId == LeadDetails.StageId).FirstOrDefault().StageName;
        //                    if (stageName != null)
        //                    {
        //                        oModel.StageName = stageName;
        //                    }
        //                    else
        //                    {
        //                        oModel.StageName = "N/A";
        //                    }
        //                }
        //                else
        //                {
        //                    oModel.StageName = "N/A";
        //                }

        //                oModel.LeadName = ToTitleCase(LeadDetails.FirstName + " " + LeadDetails.LastName);
        //            }


        //            string PhoneNumber = oLeadEmailMessageList.Select(x => x.Lead).FirstOrDefault().PhoneNumber;
        //            string EmailAddress = oLeadEmailMessageList.Select(x => x.Lead).FirstOrDefault().EmailAddress;

        //            oModel.EmailAddress = EmailAddress == null ? "N/A" : EmailAddress;
        //            oModel.Phonenumber = PhoneNumber == null ? "N/A" : PhoneNumber;

        //            var oMainMsgList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.IsReplay == false).OrderByDescending(x => x.CreatedDate).ToList().Take(5);

        //            List<LeadEmailMessageViewModel> model = new List<LeadEmailMessageViewModel>();
        //            foreach (var item in oMainMsgList)
        //            {
        //                var MainMessageList = new LeadEmailMessageViewModel();
        //                MainMessageList.AccountId = (int)item.AccountId;
        //                MainMessageList.Body = item.Body;
        //                MainMessageList.CreatedDate = (DateTime)item.CreatedDate;
        //                MainMessageList.EmailMessageId = item.EmailMessageId;
        //                MainMessageList.IsReplay = item.IsReplay;
        //                MainMessageList.LeadEmailMessageId = item.LeadEmailMessageId;
        //                MainMessageList.LeadId = item.LeadId;
        //                MainMessageList.Subject = item.Subject;
        //                MainMessageList.ToName = ToTitleCase(item.ToName);
        //                MainMessageList.FromName = ToTitleCase(item.FromName);
        //                model.Add(MainMessageList);
        //            }
        //            oModel.LeadEmailMessageViewModel = model;
        //            return PartialView("_MailBodyRight", oModel);
        //        }
        //        else
        //        {
        //            ShowWarningMessage("No Mail Found", true);
        //            return View("Index", "MailBox");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
        //        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
        //        ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, _webHostEnvironment.WebRootPath);
        //        return null;
        //        //return RedirectToAction("Login", "Account");
        //    }

        //}

        public FileResult DownloadMailAttachmentFile(int LeadId, string fileName)
        {
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                //string fileName = Request.Form["fileName"].ToString();
                //int LeadId = Convert.ToInt32(Request.Form["LeadId"]);
                //Build the File Path.
                string path = Path.Combine(this._webHostEnvironment.WebRootPath, @"MailAttachment\" + AccountId + @"\" + LeadId + @"\") + fileName;

                //Read the File data into Byte Array.
                byte[] bytes = System.IO.File.ReadAllBytes(path);

                //Send the File to Download.
                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, _webHostEnvironment.WebRootPath);
                return null;
            }

        }

        public string ToTitleCase(string title)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }

        [HttpPost]
        public ActionResult DeleteMailByIds()
        {

            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            string strIDs = string.Empty;
            int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
            if (Request.Form.ContainsKey("ids")) { strIDs = Request.Form["ids"]; }
            string[] intIDs = strIDs.Split(",");
            try
            {
                if (intIDs.Length >= 1)
                {
                    foreach (var item in intIDs)
                    {
                        int mailId = Convert.ToInt32(item);
                        var oMailList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.IsType == MessageType.EmailMessage.GetHashCode() && (x.LeadEmailMessageId == mailId || x.EmailMessageId == mailId)).ToList();
                        foreach (var itemLeadEmailMessageId in oMailList)
                        {
                            _dbContext.Entry(itemLeadEmailMessageId).State = EntityState.Deleted;
                        }
                        _dbContext.SaveChanges();
                    }
                    return Json(new { success = true, message = "Mail deleted sucessfully." });
                }
                else
                {
                    return Json(new { success = false, message = "No mail found for delete." });
                }
                //for (int i = 0; i < intIDs.Length; i++)
                //{
                //    int intLeadID = Convert.ToInt32(intIDs[i]);
                //    if (intLeadID > 0)
                //    {
                //        if (!DeleteSingleLead(intLeadID))
                //            return Json(new { success = true });
                //    }
                //}
                //return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, _webHostEnvironment.WebRootPath);
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult DeleteMailById()
        {

            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            string strID = string.Empty;
            int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
            if (Request.Form.ContainsKey("LeadEmailMessageId")) { strID = Request.Form["LeadEmailMessageId"]; }
            int intID = Convert.ToInt32(strID);
            try
            {
                var oMailList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.IsType == MessageType.EmailMessage.GetHashCode() && (x.LeadEmailMessageId == intID || x.EmailMessageId == intID)).ToList();
                foreach (var itemLeadEmailMessageId in oMailList)
                {
                    _dbContext.Entry(itemLeadEmailMessageId).State = EntityState.Deleted;
                }
                _dbContext.SaveChanges();
                return Json(new { success = true, message = "Mail deleted sucessfully." });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, _webHostEnvironment.WebRootPath);
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult MailMarkasread()
        {

            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            string strIDs = string.Empty;
            int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
            if (Request.Form.ContainsKey("ids")) { strIDs = Request.Form["ids"]; }
            string[] intIDs = strIDs.Split(",");
            try
            {
                if (intIDs.Length >= 1)
                {
                    foreach (var item in intIDs)
                    {
                        int mailId = Convert.ToInt32(item);
                        var oMailList = _dbContext.TblLeadEmailMessages.Where(x => x.LeadEmailMessageId == mailId && x.AccountId == AccountId && x.IsType == MessageType.EmailMessage.GetHashCode()).ToList();
                        foreach (var itemLeadEmailMessageId in oMailList)
                        {
                            itemLeadEmailMessageId.IsRead = true;
                        }
                        _dbContext.SaveChanges();
                    }
                    return Json(new { success = true, message = "Conversation marked as read." });
                }
                else
                {
                    return Json(new { success = false, message = "No mail found for read." });
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, _webHostEnvironment.WebRootPath);
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult MailMarkasunread()
        {

            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");

            string strIDs = string.Empty;
            int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
            if (Request.Form.ContainsKey("ids")) { strIDs = Request.Form["ids"]; }
            string[] intIDs = strIDs.Split(",");
            try
            {
                if (intIDs.Length >= 1)
                {
                    foreach (var item in intIDs)
                    {
                        int mailId = Convert.ToInt32(item);
                        var oMailList = _dbContext.TblLeadEmailMessages.Where(x => x.LeadEmailMessageId == mailId && x.AccountId == AccountId && x.IsType == MessageType.EmailMessage.GetHashCode()).ToList();
                        foreach (var itemLeadEmailMessageId in oMailList)
                        {
                            itemLeadEmailMessageId.IsRead = false;
                        }
                        _dbContext.SaveChanges();
                    }
                    return Json(new { success = true, message = "Conversation marked as unread." });
                }
                else
                {
                    return Json(new { success = false, message = "No mail found for read." });
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.logError(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex, _webHostEnvironment.WebRootPath);
                return Json(new { success = false });
            }
        }


        public static List<string> FetchImgsFromSource(string htmlSource)
        {
            List<string> listOfImgdata = new List<string>();
            string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            MatchCollection matchesImgSrc = Regex.Matches(htmlSource, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in matchesImgSrc)
            {
                string href = m.Groups[1].Value;
                listOfImgdata.Add(href);
            }
            return listOfImgdata;
        }

        public List<string> FetchAttachmentFromSource(int LeadEmailMessageId)
        {
            List<string> listOfImgdata = new List<string>();

            var matchesAttachment = _dbContext.TblLeadEmailMessageAttachments.Where(x => x.LeadEmailMessageId == LeadEmailMessageId).ToList();
            foreach (var m in matchesAttachment)
            {
                listOfImgdata.Add(m.Attachement);
            }
            return listOfImgdata;
        }
    }
}
