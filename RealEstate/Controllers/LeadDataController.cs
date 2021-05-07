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
using System.Threading.Tasks;

namespace RealEstate.Controllers
{
    public class LeadDataController : Controller
    {
        private RealEstateContext _dbContext;
        //private readonly ApplicationDbContext _context;
        private IConfiguration Configuration;

        public LeadDataController(RealEstateContext context, IConfiguration _configuration)
        {
            _dbContext = context;
            Configuration = _configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAllLead()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            // Skiping number of Rows count
            var start = Request.Form["start"].FirstOrDefault();
            // Paging Length 10,20
            var length = Request.Form["length"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;


            int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
            var oLeadList = _dbContext.TblLeads.Where(x => x.AccountId == AccountId).Include(x => x.Agent).ToList().Select(s => new LeadViewModel
            {
                LeadId = s.LeadId,
                LeadSource = s.LeadSource,
                LeadStatus = s.LeadStatus,
                Industry = s.Industry,
                //Stage = s.Stage,
                StageId = s.StageId,
                Stage = s.StageNavigation,
                TagsName = GetTagsName(s.LeadId),
                AgentId = s.AgentId,
                Agent = s.Agent,
                OwnerImg = s.OwnerImg,
                LeadOwner = s.LeadOwner,
                Company = s.Company,
                FirstName = s.FirstName,
                LastName = s.LastName,
                FullName = s.FirstName + " " + s.LastName,
                Title = s.Title,
                EmailAddress = s.EmailAddress,
                PhoneNumber = s.PhoneNumber,
                Fax = s.Fax,
                MobileNumber = s.MobileNumber,
                Website = s.Website,
                NoOfEmp = s.NoOfEmp,
                AnnualRevenue = s.AnnualRevenue,
                Rating = s.Rating,
                EmailOptOut = s.EmailOptOut == true ? true : false,
                SkypeId = s.SkypeId,
                TwitterId = s.TwitterId,
                SecondaryEmail = s.SecondaryEmail,
                Street = s.Street,
                State = s.State,
                Country = s.Country,
                City = s.City,
                ZipCode = s.ZipCode,
                Description = s.Description,
                CreatedDate = s.CreatedDate
            }).Skip(skip).Take(pageSize);
            LoadComboBoxes();
            return new JsonResult(new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = oLeadList
            });
            //return Json(new
            //{
            //    draw = draw,
            //    recordsFiltered = recordsTotal,
            //    recordsTotal = recordsTotal,
            //    data = oLeadList
            //}, new Newtonsoft.Json.JsonSerializerSettings());
            //return Json(oLeadList);
        }


        public void LoadComboBoxes()
        {
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);

                Dictionary<string, string> sOwnerList = new Dictionary<string, string>();
                var users = _dbContext.TblAccounts.Where(u => u.RoleId == RoleType.Admin.GetHashCode() && u.Status == true).ToList();
                foreach (var item in users)
                {
                    sOwnerList.Add(item.FullName, item.FullName);
                }
                ViewBag.OwnerList = sOwnerList;


                //Dictionary<string, string> oSourcelist = new Dictionary<string, string>();

                //var leads = _dbContext.TblLeads.ToList();
                //var items = leads.Select(x => x.LeadSource).Distinct();
                //foreach (string item in items)
                //{
                //    oSourcelist.Add(item, item);
                //}
                //ViewBag.LeadSourceList = oSourcelist;


                Dictionary<int, string> oAgentlist = new Dictionary<int, string>();
                var agents = _dbContext.TblAgents.Where(u => u.IsActive == true && u.AccountId == AccountId).ToList();
                foreach (var item in agents)
                {
                    oAgentlist.Add(item.Id, item.FullName);
                }
                ViewBag.AgentList = oAgentlist;



                Dictionary<int, string> oStagelist = new Dictionary<int, string>();
                var stages = _dbContext.TblStages.Where(u => u.AccountId == AccountId).ToList();
                foreach (var item in stages)
                {
                    oStagelist.Add(item.StageId, item.StageName);
                }
                ViewBag.StageList = oStagelist;


                Dictionary<int, string> oTaglist = new Dictionary<int, string>();
                var tags = _dbContext.TblTags.Where(u => u.AccountId == AccountId).ToList();
                foreach (var item in tags)
                {
                    oTaglist.Add(item.TagId, item.TagName);
                }
                ViewBag.TagList = oTaglist;

            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
            }
        }

        public string GetTagsName(int LeadId)
        {
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                var leadTag = _dbContext.TblLeadTags.Where(x => x.LeadId == LeadId && x.AccountId == AccountId).ToList();
                var TagsName = string.Empty;
                foreach (var data in leadTag)
                {
                    var tag = _dbContext.TblTags.Where(x => x.TagId == data.TagId).Select(x => x.TagName).First();
                    TagsName += tag + ",";
                }
                TagsName = TagsName.TrimEnd(',');
                return TagsName;
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return null;
            }

        }
    }
}
