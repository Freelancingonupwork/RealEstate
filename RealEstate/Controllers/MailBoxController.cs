using Microsoft.AspNetCore.Hosting;
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


                    var oMainMsgList = _dbContext.TblLeadEmailMessages.Where(x => x.AccountId == AccountId && x.IsReplay == false).OrderByDescending(x => x.CreatedDate).ToList().Select(s => new LeadEmailMessageViewModel
                    {
                        LeadId = s.LeadId,
                        LeadEmailMessageId = s.LeadEmailMessageId,
                        AccountId = s.AccountId,
                        Subject = s.Subject,
                        Body = s.Body,
                        FromName = s.FromName,
                        ToName = s.ToName,
                        IsReplay = s.IsReplay,
                        EmailMessageId = s.EmailMessageId,
                        CreatedDate = s.CreatedDate
                    });

                    return View(oMainMsgList);
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
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);

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
                        MainMessageList.ToName = item.ToName;
                        MainMessageList.FromName = item.FromName;
                        MainMessageList.LeadEmailMessageReplayList = new List<LeadEmailMessageViewModel>();
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
                            ReplayMessageList.ToName = itemReplay.ToName;
                            ReplayMessageList.FromName = itemReplay.FromName;
                            MainMessageList.LeadEmailMessageReplayList.Add(ReplayMessageList);
                        }
                        model.Add(MainMessageList);
                    }
                    return PartialView("_MailBody", model);
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
    }
}
