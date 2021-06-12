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


        public JsonResult LeadDataJson(int StartRecordNumber, int PageSize, string SortColumn, string SortDirection, string echo, string Search)
        {
            List<LeadViewModel> model = new List<LeadViewModel>();
            try
            {
                int AccountId = Convert.ToInt32(Request.Cookies["LoginAccountId"]);
                using (RealEstateContext DB = new RealEstateContext())
                {
                    var Total = 0;
                    var reviews = DB.TblLeads.Where(x => x.AccountId == AccountId).Include(x => x.Agent).ToList();
                    if (Search != null)
                    {
                        Search = Search.ToLower();
                        reviews = reviews.Where(x => (x.FirstName != null ? x.FirstName.ToLower().Contains(Search) : true) || (x.LastName != null ? x.LastName.ToLower().Contains(Search) : true) || (x.PhoneNumber != null ? x.PhoneNumber.ToLower().Contains(Search) : true) || (x.EmailAddress != null ? x.EmailAddress.Contains(Search) : true) || (x.Agent.FullName != null ? x.Agent.FullName.ToLower().Contains(Search) : true) || (x.LeadSource != null ? x.LeadSource.ToLower().Contains(Search) : true) || (x.LeadStatus != null ? x.LeadStatus.ToLower().Contains(Search) : true) || (x.Stage != null ? x.Stage.ToLower().Contains(Search) : true)).ToList();
                    }

                    Total = reviews.Count;
                    if (SortDirection == "asc")
                    {
                        switch (SortColumn)
                        {
                            case "FirstName":
                                reviews = reviews.OrderBy(x => x.FirstName).ToList();
                                break;
                            case "LastName":
                                reviews = reviews.OrderBy(x => x.LastName).ToList();
                                break;
                            case "PhoneNumber":
                                reviews = reviews.OrderBy(x => x.PhoneNumber).ToList();
                                break;
                            case "EmailAddress":
                                reviews = reviews.OrderBy(x => x.EmailAddress).ToList();
                                break;
                            case "Stage":
                                reviews = reviews.OrderBy(x => x.Stage).ToList();
                                break;
                            case "LeadStatus":
                                reviews = reviews.OrderBy(x => x.LeadStatus).ToList();
                                break;
                            case "LeadSource":
                                reviews = reviews.OrderBy(x => x.LeadSource).ToList();
                                break;
                            case "Industry":
                                reviews = reviews.OrderBy(x => x.Industry).ToList();
                                break;
                            default:
                                reviews = reviews.OrderBy(x => x.LeadId).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (SortColumn)
                        {
                            case "FirstName":
                                reviews = reviews.OrderByDescending(x => x.FirstName).ToList();
                                break;
                            case "LastName":
                                reviews = reviews.OrderByDescending(x => x.LastName).ToList();
                                break;
                            case "PhoneNumber":
                                reviews = reviews.OrderByDescending(x => x.PhoneNumber).ToList();
                                break;
                            case "EmailAddress":
                                reviews = reviews.OrderByDescending(x => x.EmailAddress).ToList();
                                break;
                            case "Stage":
                                reviews = reviews.OrderByDescending(x => x.Stage).ToList();
                                break;
                            case "LeadStatus":
                                reviews = reviews.OrderByDescending(x => x.LeadStatus).ToList();
                                break;
                            case "LeadSource":
                                reviews = reviews.OrderByDescending(x => x.LeadSource).ToList();
                                break;
                            case "Industry":
                                reviews = reviews.OrderByDescending(x => x.Industry).ToList();
                                break;
                            default:
                                reviews = reviews.OrderByDescending(x => x.LeadId).ToList();
                                break;
                        }
                    }
                    reviews = reviews.Skip(StartRecordNumber).Take(PageSize).ToList();

                    foreach (var item in reviews)
                    {
                        LeadViewModel providerReview = new LeadViewModel();
                        providerReview.LeadId = item.LeadId;
                        providerReview.LeadSource = item.LeadSource;
                        providerReview.LeadStatus = item.LeadStatus;
                        providerReview.Industry = item.Industry;
                        //providerReview.StageId = item.StageId;
                        providerReview.Stage = item.StageNavigation;
                        providerReview.TagsName = GetTagsName(item.LeadId);
                        //providerReview.AgentId = item.AgentId;
                        providerReview.Agent = item.Agent;
                        //providerReview.OwnerImg = item.OwnerImg;
                        //providerReview.LeadOwner = item.LeadOwner;
                        //providerReview.Company = item.Company;
                        providerReview.FirstName = item.FirstName;
                        providerReview.LastName = item.LastName;
                        //providerReview.Title = item.Title;
                        providerReview.EmailAddress = item.EmailAddress;
                        providerReview.PhoneNumber = item.PhoneNumber;
                        //providerReview.Fax = item.Fax;
                        //providerReview.MobileNumber = item.MobileNumber;
                        //providerReview.Website = item.Website;
                        //providerReview.NoOfEmp = item.NoOfEmp;
                        //providerReview.AnnualRevenue = item.AnnualRevenue;
                        //providerReview.Rating = item.Rating;
                        //providerReview.EmailOptOut = item.EmailOptOut == true ? true : false;
                        //providerReview.SkypeId = item.SkypeId;
                        //providerReview.TwitterId = item.TwitterId;
                        //providerReview.SecondaryEmail = item.SecondaryEmail;
                        //providerReview.Street = item.Street;
                        //providerReview.State = item.State;
                        //providerReview.Country = item.Country;
                        //providerReview.City = item.City;
                        //providerReview.ZipCode = item.ZipCode;
                        //providerReview.Description = item.Description;
                        providerReview.CreatedDate = item.CreatedDate;
                        model.Add(providerReview);
                    }

                    var result = from d in model
                                 select new[] {
                                d.LeadId.ToString(),
                                d.FirstName.ToString() + " " + d.LastName.ToString() ,
                                d.PhoneNumber == null ? "N/A" : d.PhoneNumber.ToString(),
                                d.EmailAddress.ToString(),
                                "50000",
                                d.LeadSource == null ? "N/A" : d.LeadSource.ToString(),
                                d.LeadStatus == null ? "N/A" : d.LeadStatus.ToString(),
                                d.Industry == null ? "N/A" : d.Industry.ToString(),
                                "02/01/2021",
                                "02/01/2021",
                                d.Stage == null ? "N/A" : d.Stage.StageName.ToString(),
                                d.TagsName == "" || d.TagsName == null ? "N/A" : d.TagsName.ToString(),
                                d.CreatedDate.Value.ToString(),
                                d.Agent == null ? "N/A" : d.Agent.FullName.ToString(),
                                "Action"
                    };

                    return Json(new { aaData = result, iTotalRecords = Total, iTotalDisplayRecords = Total, sEcho = echo }, new Newtonsoft.Json.JsonSerializerSettings());
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { aaData = model }, new Newtonsoft.Json.JsonSerializerSettings());
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
