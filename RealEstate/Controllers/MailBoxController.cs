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
using System.Linq;
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
                    var oMainMsgList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.IsReplay == false).OrderByDescending(x => x.CreatedDate).ToList();
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
                        MainMessageList.ToName = item.ToName;
                        MainMessageList.FromName = item.FromName;
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

        public IActionResult LoadBodyById(int Id)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                LeadEmailDetails oModel = new LeadEmailDetails();
                var oLeadEmailMessageList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId).Include(x => x.Lead).ToList();
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
                        MainMessageList.LeadEmailMessageId = item.LeadEmailMessageId;
                        MainMessageList.LeadId = item.LeadId;
                        MainMessageList.Subject = item.Subject;
                        MainMessageList.ToName = ToTitleCase(item.ToName);
                        MainMessageList.FromName = ToTitleCase(item.FromName);
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
                var oLeadEmailMessageList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.LeadEmailMessageId == Id).Include(x => x.Lead).ToList();
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

                    var oMainMsgList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.IsReplay == false && x.LeadId == LeadId).OrderByDescending(x => x.CreatedDate).ToList().Take(5);

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

        public string ToTitleCase(string title)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }
    }
}
