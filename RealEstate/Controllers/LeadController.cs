using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;
using RealEstateDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Threading.Tasks;
using RealEstate.Models;
using RealEstate.Utills;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Collections.Specialized;

namespace RealEstate.Controllers
{
    public class LeadController : BaseController
    {
        private RealEstateContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        public LeadController(RealEstateContext dbContext, IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
        }

        public IActionResult Index()
        {
            try
            {
                //if (!Request.Cookies.ContainsKey("FullName"))
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");
                //if (Convert.ToInt32(Request.Cookies["UserLoginTypeId"]) != UserLoginType.Admin.GetHashCode())
                if (Convert.ToInt32(Request.Cookies["UserLoginTypeId"]) != UserLoginType.Company.GetHashCode())
                {
                    using (var DB = _dbContext)
                    {
                        var agentDetails = DB.TblAgents.Where(x => x.EmailAddress.Equals(Request.Cookies["EmailAddress"].ToString())).FirstOrDefault();
                        if (agentDetails != null)
                        {
                            var oLeadList = DB.TblLeads.Where(x => x.AgentId == agentDetails.Id).Include(x => x.Agent).Include(x => x.StageNavigation).ToList().Select(s => new LeadViewModel
                            {
                                LeadId = s.LeadId,
                                LeadSource = s.LeadSource,
                                LeadStatus = s.LeadStatus,
                                Industry = s.Industry,
                                StageId = s.StageId,
                                Stage = s.StageNavigation,
                                AgentId = s.AgentId,
                                Agent = s.Agent,
                                OwnerImg = s.OwnerImg,
                                TagsName = GetTagsName(s.LeadId),
                                LeadOwner = s.LeadOwner,
                                Company = s.Company,
                                FirstName = s.FirstName,
                                LastName = s.LastName,
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
                            });
                            LoadComboBoxes();
                            return View(oLeadList);
                        }
                        else
                        {
                            return View();
                        }
                    }
                }
                else
                {
                    int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                    var oLeadList = _dbContext.TblLeads.Where(x => x.CompanyId == CompanyId).Include(x => x.Agent).ToList().Select(s => new LeadViewModel
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
                    });
                    LoadComboBoxes();
                    return View(oLeadList);
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

        [HttpGet]
        public IActionResult EditLeadDetails(int id)
        {
            if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                return RedirectToAction("Login", "Account");
            LoadComboBoxes();
            try
            {
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                TblLead oLead = _dbContext.TblLeads.Where(x => x.LeadId == id && x.CompanyId == CompanyId).Include(x => x.Agent).Include(x => x.StageNavigation).FirstOrDefault();
                LeadViewModel oModel = new LeadViewModel();
                oModel.LeadId = oLead.LeadId;
                ViewBag.LeadSource = oLead.LeadSource;
                oModel.LeadSource = oLead.LeadSource;
                oModel.LeadStatus = oLead.LeadStatus;
                oModel.Industry = oLead.Industry;
                //oModel.Stage = oLead.Stage;
                oModel.StageId = oLead.StageId;
                oModel.Stage = oLead.StageNavigation;
                oModel.TagsName = GetTagsName(oLead.LeadId);
                oModel.AgentId = oLead.AgentId;
                oModel.Agent = oLead.Agent;
                oModel.OwnerImg = oLead.OwnerImg;
                oModel.LeadOwner = oLead.LeadOwner;
                oModel.Company = oLead.Company;
                oModel.FirstName = oLead.FirstName;
                oModel.LastName = oLead.LastName;
                oModel.Title = oLead.Title;
                oModel.EmailAddress = oLead.EmailAddress;
                oModel.PhoneNumber = oLead.PhoneNumber;
                oModel.Fax = oLead.Fax;
                oModel.MobileNumber = oLead.MobileNumber;
                oModel.Website = oLead.Website;
                oModel.NoOfEmp = oLead.NoOfEmp;
                oModel.AnnualRevenue = oLead.AnnualRevenue;
                oModel.Rating = oLead.Rating;
                oModel.EmailOptOut = oLead.EmailOptOut == true ? true : false;
                oModel.SkypeId = oLead.SkypeId;
                oModel.TwitterId = oLead.TwitterId;
                oModel.SecondaryEmail = oLead.SecondaryEmail;
                oModel.Street = oLead.Street;
                oModel.State = oLead.State;
                oModel.Country = oLead.Country;
                oModel.City = oLead.City;
                oModel.ZipCode = oLead.ZipCode;
                oModel.Description = oLead.Description;
                oModel.CreatedDate = oLead.CreatedDate;
                return View(oModel);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return View("Index");
            }
        }

        [HttpPost]
        public IActionResult EditLeadDetails(LeadViewModel model)
        {
            try
            {
                LoadComboBoxes();
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                if (Request.Cookies.ContainsKey("FullName") && Request.Cookies.ContainsKey("EmailAddress"))
                {
                    if (ModelState.IsValid)
                    {
                        var result = _dbContext.TblLeads.Where(u => u.EmailAddress.ToLower().Equals(model.EmailAddress.ToLower()) && u.LeadId != model.LeadId && u.CompanyId == CompanyId).ToList();
                        if (result.Count() > 0)
                        {
                            ShowErrorMessage("Email is already used!", true);
                            LoadComboBoxes();
                            return View(model);
                        }

                        TblLead oData = new TblLead();
                        oData.LeadId = model.LeadId;
                        oData.LeadOwner = model.LeadOwner;
                        oData.StageId = model.StageId;
                        oData.CompanyId = CompanyId;
                        oData.Company = model.Company;
                        oData.FirstName = model.FirstName;
                        oData.LastName = model.LastName;
                        oData.Title = model.Title;
                        oData.EmailAddress = model.EmailAddress;
                        oData.PhoneNumber = model.PhoneNumber;
                        oData.Fax = model.Fax;
                        oData.MobileNumber = model.MobileNumber;
                        oData.Website = model.Website;
                        oData.LeadSource = model.LeadSource;
                        oData.LeadStatus = model.LeadStatus;
                        oData.Industry = model.Industry;
                        oData.AgentId = model.AgentId;
                        //oData.Stage = model.Stage;
                        oData.NoOfEmp = model.NoOfEmp;
                        oData.AnnualRevenue = model.AnnualRevenue;

                        oData.Rating = model.Rating;
                        oData.EmailOptOut = model.EmailOptOut;
                        oData.SkypeId = model.SkypeId;
                        oData.TwitterId = model.TwitterId;
                        oData.SecondaryEmail = model.SecondaryEmail;

                        oData.Street = model.Street;
                        oData.City = model.City;
                        oData.State = model.State;
                        oData.ZipCode = model.ZipCode;
                        oData.Country = model.Country;
                        oData.Description = model.Description;
                        oData.CreatedDate = DateTime.Now;
                        _dbContext.Entry(oData).State = EntityState.Modified;
                        _dbContext.SaveChanges();


                        LoadComboBoxes();
                        ShowSuccessMessage("Lead updated Successfully.", true);
                        return RedirectToAction("Index", "Lead");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.log(ex);
                return View();
            }
        }

        public IActionResult AddNewLead()
        {
            try
            {
                if (Request.Cookies.ContainsKey("FullName"))
                {
                    LoadComboBoxes();
                    return View();
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
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult AddNewLead(LeadViewModel model)
        {
            try
            {
                LoadComboBoxes();
                if (Request.Cookies.ContainsKey("FullName") && Request.Cookies.ContainsKey("EmailAddress"))
                {
                    if (ModelState.IsValid)
                    {
                        if (IsUserExist(model.EmailAddress) || IsUserExist(model.SecondaryEmail))
                        {
                            //ViewBag.MessageType = "danger";
                            //ViewBag.Message = "Email is already in used!";
                            ShowErrorMessage("Email is already used!", true);
                            LoadComboBoxes();
                            return View(model);
                        }
                        int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                        TblLead oData = new TblLead();
                        oData.LeadOwner = model.LeadOwner;
                        oData.StageId = Convert.ToInt32(model.StageId);
                        oData.CompanyId = CompanyId;
                        oData.Company = model.Company;
                        oData.FirstName = model.FirstName;
                        oData.LastName = model.LastName;
                        oData.Title = model.Title;
                        oData.EmailAddress = model.EmailAddress;
                        oData.PhoneNumber = model.PhoneNumber;
                        oData.Fax = model.Fax;
                        oData.MobileNumber = model.MobileNumber;
                        oData.Website = model.Website;
                        oData.LeadSource = model.LeadSource;
                        oData.LeadStatus = model.LeadStatus;
                        oData.Industry = model.Industry;
                        //oData.Stage = model.Stage;
                        oData.NoOfEmp = model.NoOfEmp;
                        oData.AnnualRevenue = model.AnnualRevenue;

                        oData.Rating = model.Rating;
                        oData.EmailOptOut = model.EmailOptOut;
                        oData.SkypeId = model.SkypeId;
                        oData.TwitterId = model.TwitterId;
                        oData.SecondaryEmail = model.SecondaryEmail;

                        oData.Street = model.Street;
                        oData.City = model.City;
                        oData.State = model.State;
                        oData.ZipCode = model.ZipCode;
                        oData.Country = model.Country;
                        oData.Description = model.Description;
                        oData.CreatedDate = DateTime.Now;
                        _dbContext.TblLeads.Add(oData);
                        _dbContext.SaveChanges();

                        LoadComboBoxes();
                        //ViewBag.MessageType = "success";
                        //ViewBag.Message = "Lead added Successfully";
                        ShowSuccessMessage("Lead added Successfully.", true);
                        //return View(model);
                        return RedirectToAction("Index", "Lead");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult NewLead()
        {
            try
            {
                if (Request.Cookies.ContainsKey("FullName") && Request.Cookies.ContainsKey("EmailAddress"))
                {
                    return View();
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
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }
        }

        public void LoadComboBoxes()
        {
            try
            {
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);

                Dictionary<string, string> sOwnerList = new Dictionary<string, string>();
                var users = _dbContext.TblUsers.Where(u => u.UserLoginTypeId == UserLoginType.Admin.GetHashCode() && u.IsActive == true).ToList();
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
                var agents = _dbContext.TblAgents.Where(u => u.IsActive == true && u.CompanyId == CompanyId).ToList();
                foreach (var item in agents)
                {
                    oAgentlist.Add(item.Id, item.FullName);
                }
                ViewBag.AgentList = oAgentlist;



                Dictionary<int, string> oStagelist = new Dictionary<int, string>();
                var stages = _dbContext.TblStages.Where(u => u.CompanyId == CompanyId).ToList();
                foreach (var item in stages)
                {
                    oStagelist.Add(item.StageId, item.StageName);
                }
                ViewBag.StageList = oStagelist;


                Dictionary<int, string> oTaglist = new Dictionary<int, string>();
                var tags = _dbContext.TblTags.Where(u => u.CompanyId == CompanyId).ToList();
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

        public ActionResult GetAllSources()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                return Json(_dbContext.TblLeadSources.Where(x => x.CompanyId == CompanyId).Select(s => new
                {
                    id = s.LeadSourceId,
                    name = s.LeadSourceName
                }).ToList());
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Error occur while getting record!" + ex.Message });
            }

        }

        public ActionResult DeleteSource()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                int id = 0;
                if (Request.Form.ContainsKey("id")) { id = Convert.ToInt32(Request.Form["id"]); }
                if (id <= 0) { throw new Exception("Identity can not be blank!"); }
                _dbContext.TblLeadSources.Remove(_dbContext.TblLeadSources.Where(x => x.LeadSourceId == id && x.CompanyId == CompanyId).FirstOrDefault());
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
        public ActionResult AddSource()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                string name = string.Empty;
                if (Request.Form.ContainsKey("name")) { name = Request.Form["name"]; }
                if (string.IsNullOrEmpty(name)) { throw new Exception("Source name can not be blank!"); }
                var lead = _dbContext.TblLeadSources.Where(x => x.LeadSourceName.ToLower().Equals(name.ToLower()) && x.CompanyId == CompanyId).FirstOrDefault();
                if (lead != null)
                {
                    return Json(new { success = false, message = "Lead source with name '" + name + "' is already exist" });
                }

                TblLeadSource src = new TblLeadSource();
                src.LeadSourceName = name;
                src.CompanyId = CompanyId;
                src.CreatedDate = DateTime.Now;
                _dbContext.TblLeadSources.Add(src);
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Error occur while inserting record!" + ex.Message });
            }

        }


        public ActionResult AddStage()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                string name = string.Empty;
                if (Request.Form.ContainsKey("name")) { name = Request.Form["name"]; }
                if (string.IsNullOrEmpty(name)) { throw new Exception("Stage name can not be blank!"); }
                var lead = _dbContext.TblStages.Where(x => x.StageName.ToLower().Equals(name.ToLower()) && x.CompanyId == CompanyId).FirstOrDefault();
                if (lead != null)
                {
                    return Json(new { success = false, message = "Lead stage with name '" + name + "' is already exist" });
                }

                TblStage oStage = new TblStage();
                oStage.StageName = name;
                oStage.CompanyId = CompanyId;
                oStage.CreatedDate = DateTime.Now;
                _dbContext.TblStages.Add(oStage);
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false, message = "Error occur while inserting record!" + ex.Message });
            }

        }

        public async Task<IActionResult> Upload(IFormFile postedFile)
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                if (postedFile == null || postedFile.Length == 0)
                {
                    ShowErrorMessage("Please select file.", true);
                    return RedirectToAction("NewLead", "Lead");
                }

                //Get file
                var newfile = new FileInfo(postedFile.FileName);
                var fileExtension = newfile.Extension;

                //Check if file is an Excel File
                if (fileExtension.Contains(".xls"))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await postedFile.CopyToAsync(ms);
                        ExcelPackage.LicenseContext = LicenseContext.Commercial;
                        using (ExcelPackage package = new ExcelPackage(ms))
                        {
                            ExcelWorksheet workSheet = package.Workbook.Worksheets["Sheet1"];
                            int totalRows = workSheet.Dimension.End.Row;

                            List<TblLead> leadList = new List<TblLead>();

                            for (int i = 2; i < totalRows; i++)
                            {
                                string strLeadSource = workSheet.Cells[i, 1].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 1].Value.ToString();
                                string strLeadStatus = workSheet.Cells[i, 2].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 2].Value.ToString();
                                string strIndustry = workSheet.Cells[i, 3].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 3].Value.ToString();
                                string strStage = workSheet.Cells[i, 4].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 4].Value.ToString();
                                string strOwnerImg = workSheet.Cells[i, 5].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 5].Value.ToString();
                                string strLeadOwner = workSheet.Cells[i, 6].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 6].Value.ToString();

                                string strCompany = workSheet.Cells[i, 7].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 7].Value.ToString();
                                string strFirstName = workSheet.Cells[i, 8].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 8].Value.ToString();
                                string strLastName = workSheet.Cells[i, 9].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 9].Value.ToString();
                                string strTitle = workSheet.Cells[i, 10].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 10].Value.ToString();
                                string strEmailAddress = workSheet.Cells[i, 11].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 11].Value.ToString();

                                string strPhoneNumber = workSheet.Cells[i, 12].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 12].Value.ToString();
                                string strFax = workSheet.Cells[i, 13].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 13].Value.ToString();
                                string strMobileNumber = workSheet.Cells[i, 14].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 14].Value.ToString();
                                string strWebsite = workSheet.Cells[i, 15].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 15].Value.ToString();

                                int intNoOfEmp = int.TryParse(workSheet.Cells[i, 16].Value.ToString(), out intNoOfEmp) ? Convert.ToInt32(workSheet.Cells[i, 16].Value) : 0;

                                decimal decAnnualRevenue = decimal.TryParse(workSheet.Cells[i, 17].Value.ToString(), out decAnnualRevenue) ? Convert.ToDecimal(workSheet.Cells[i, 17].Value) : 0;
                                int intRating = int.TryParse(workSheet.Cells[i, 18].Value.ToString(), out intRating) ? Convert.ToInt32(workSheet.Cells[i, 18].Value) : 0;
                                bool blnEmailOptOut = bool.TryParse(workSheet.Cells[i, 19].Value.ToString(), out blnEmailOptOut) ? workSheet.Cells[i, 19].Value.ToString() == "True" ? true : false : false;
                                string strSkypeId = workSheet.Cells[i, 20].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 20].Value.ToString();
                                string strTwitterId = workSheet.Cells[i, 21].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 21].Value.ToString();

                                string strSecondaryEmail = workSheet.Cells[i, 22].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 22].Value.ToString();
                                string strStreet = workSheet.Cells[i, 23].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 23].Value.ToString();
                                string strState = workSheet.Cells[i, 24].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 24].Value.ToString();
                                string strCountry = workSheet.Cells[i, 25].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 25].Value.ToString();
                                string strCity = workSheet.Cells[i, 26].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 26].Value.ToString();

                                string strZipCode = workSheet.Cells[i, 27].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 27].Value.ToString();
                                string strDescription = workSheet.Cells[i, 28].Value.ToString() == "NULL" ? string.Empty : workSheet.Cells[i, 27].Value.ToString();
                                DateTime dtCreatedDate = DateTime.TryParse(workSheet.Cells[i, 29].Value.ToString(), out dtCreatedDate) ? Convert.ToDateTime(workSheet.Cells[i, 29].Value) : DateTime.Now;
                                var result = _dbContext.TblLeads.Where(u => u.EmailAddress.ToLower().Equals(strEmailAddress.ToLower())).ToList();
                                if (result.Count() > 0)
                                    continue;

                                leadList.Add(new TblLead
                                {
                                    LeadSource = strLeadSource,
                                    LeadStatus = strLeadStatus,
                                    Industry = strIndustry,
                                    Stage = strStage,
                                    OwnerImg = strOwnerImg,
                                    LeadOwner = strLeadOwner,
                                    CompanyId = CompanyId,
                                    Company = strCompany,
                                    FirstName = strFirstName,
                                    LastName = strLastName,
                                    Title = strTitle,
                                    EmailAddress = strEmailAddress,
                                    PhoneNumber = strPhoneNumber,
                                    Fax = strFax,
                                    MobileNumber = strMobileNumber,
                                    Website = strWebsite,
                                    NoOfEmp = intNoOfEmp,
                                    AnnualRevenue = decAnnualRevenue,
                                    Rating = intRating,
                                    EmailOptOut = blnEmailOptOut,
                                    SkypeId = strSkypeId,
                                    TwitterId = strTwitterId,
                                    SecondaryEmail = strSecondaryEmail,
                                    Street = strStreet,
                                    State = strState,
                                    Country = strCountry,
                                    City = strCity,
                                    ZipCode = strZipCode,
                                    Description = strDescription,
                                    CreatedDate = dtCreatedDate
                                }); ;
                            }

                            _dbContext.TblLeads.AddRange(leadList);
                            await _dbContext.SaveChangesAsync();
                            ShowSuccessMessage("Leads are successfully imported from file!", true);
                        }
                    }

                }
                else if (fileExtension.Contains(".csv"))
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string contentRootPath = _webHostEnvironment.ContentRootPath;

                    string path = "";
                    path = Path.Combine(contentRootPath, "wwwroot", "CSVFiles");

                    if (!Directory.Exists(path))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(path);
                    }
                    path = Path.Combine(path);
                    string renameFile = RandomString(6) + "." + newfile.Name.Split('.').Last();
                    var fullPath = Path.Combine(path, renameFile);
                    using (FileStream output = System.IO.File.Create(fullPath))
                        await postedFile.CopyToAsync(output);


                    using (var reader = new StreamReader(fullPath))
                    {
                        int cnt = 0;
                        List<TblLead> leadList = new List<TblLead>();
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(',');
                            cnt++;
                            if (cnt == 1) { continue; }
                            if (string.IsNullOrEmpty(line)) { break; }
                            string strLeadSource = values[0].ToString() == "NULL" ? string.Empty : values[0].ToString();
                            string strLeadStatus = values[1].ToString() == "NULL" ? string.Empty : values[1].ToString();
                            string strIndustry = values[2].ToString() == "NULL" ? string.Empty : values[2].ToString();
                            string strStage = values[3].ToString() == "NULL" ? string.Empty : values[3].ToString();
                            string strOwnerImg = values[4].ToString() == "NULL" ? string.Empty : values[4].ToString();
                            string strLeadOwner = values[5].ToString() == "NULL" ? string.Empty : values[5].ToString();

                            string strCompany = values[6].ToString() == "NULL" ? string.Empty : values[5].ToString();
                            string strFirstName = values[7].ToString() == "NULL" ? string.Empty : values[7].ToString();
                            string strLastName = values[8].ToString() == "NULL" ? string.Empty : values[8].ToString();
                            string strTitle = values[9].ToString() == "NULL" ? string.Empty : values[9].ToString();
                            string strEmailAddress = values[10].ToString() == "NULL" ? string.Empty : values[10].ToString();

                            string strPhoneNumber = values[11].ToString() == "NULL" ? string.Empty : values[11].ToString();
                            string strFax = values[12].ToString() == "NULL" ? string.Empty : values[12].ToString();
                            string strMobileNumber = values[13].ToString() == "NULL" ? string.Empty : values[13].ToString();
                            string strWebsite = values[14].ToString() == "NULL" ? string.Empty : values[14].ToString();

                            int intNoOfEmp = int.TryParse(values[15].ToString(), out intNoOfEmp) ? Convert.ToInt32(values[15].ToString()) : 0;

                            decimal decAnnualRevenue = decimal.TryParse(values[16].ToString(), out decAnnualRevenue) ? Convert.ToDecimal(values[16].ToString()) : 0;
                            int intRating = int.TryParse(values[17].ToString(), out intRating) ? Convert.ToInt32(values[17].ToString()) : 0;
                            bool blnEmailOptOut = bool.TryParse(values[18].ToString(), out blnEmailOptOut) ? values[18].ToString() == "True" ? true : false : false;
                            string strSkypeId = values[19].ToString() == "NULL" ? string.Empty : values[19].ToString();
                            string strTwitterId = values[20].ToString() == "NULL" ? string.Empty : values[20].ToString();

                            string strSecondaryEmail = values[21].ToString() == "NULL" ? string.Empty : values[21].ToString();
                            string strStreet = values[22].ToString() == "NULL" ? string.Empty : values[22].ToString();
                            string strState = values[23].ToString() == "NULL" ? string.Empty : values[23].ToString();
                            string strCountry = values[24].ToString() == "NULL" ? string.Empty : values[24].ToString();
                            string strCity = values[25].ToString() == "NULL" ? string.Empty : values[25].ToString();

                            string strZipCode = values[26].ToString() == "NULL" ? string.Empty : values[26].ToString();
                            string strDescription = values[27].ToString() == "NULL" ? string.Empty : values[27].ToString();
                            DateTime dtCreatedDate = DateTime.TryParse(values[28].ToString(), out dtCreatedDate) ? Convert.ToDateTime(values[28].ToString()) : DateTime.Now;
                            var result = _dbContext.TblLeads.Where(u => u.EmailAddress.ToLower().Equals(strEmailAddress.ToLower())).ToList();
                            if (result.Count() > 0)
                                continue;

                            leadList.Add(new TblLead
                            {
                                LeadSource = strLeadSource,
                                LeadStatus = strLeadStatus,
                                Industry = strIndustry,
                                Stage = strStage,
                                OwnerImg = strOwnerImg,
                                LeadOwner = strLeadOwner,
                                CompanyId = CompanyId,
                                Company = strCompany,
                                FirstName = strFirstName,
                                LastName = strLastName,
                                Title = strTitle,
                                EmailAddress = strEmailAddress,
                                PhoneNumber = strPhoneNumber,
                                Fax = strFax,
                                MobileNumber = strMobileNumber,
                                Website = strWebsite,
                                NoOfEmp = intNoOfEmp,
                                AnnualRevenue = decAnnualRevenue,
                                Rating = intRating,
                                EmailOptOut = blnEmailOptOut,
                                SkypeId = strSkypeId,
                                TwitterId = strTwitterId,
                                SecondaryEmail = strSecondaryEmail,
                                Street = strStreet,
                                State = strState,
                                Country = strCountry,
                                City = strCity,
                                ZipCode = strZipCode,
                                Description = strDescription,
                                CreatedDate = dtCreatedDate
                            });
                        }
                        _dbContext.TblLeads.AddRange(leadList);
                        await _dbContext.SaveChangesAsync();
                        ShowSuccessMessage("Leads are successfully imported from file!", true);
                    }
                }
                else
                {
                    ShowErrorMessage("Please select csv or xls file.", true);
                    return RedirectToAction("NewLead", "Lead");
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("NewLead");
            }
            return RedirectToAction("Index", "Lead");
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        public ActionResult HubSportRequest()
        {
            try
            {
                if (Request.Cookies.ContainsKey("FullName") && Request.Cookies.ContainsKey("EmailAddress"))
                {
                    return Redirect("https://app.hubspot.com/oauth/authorize?client_id=" + this._config.GetSection("APIs")["client_id"] + "&scope=contacts&redirect_uri=" + $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + this._config.GetSection("APIs")["redirect_uri"] + "");
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
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return View("NewLead");
            }

        }

        public async Task<IActionResult> HubSportResponse(string code)
        {
            try
            {
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);

                var postParams = new Dictionary<string, string>();

                postParams.Add("grant_type", "authorization_code");
                postParams.Add("client_id", this._config.GetSection("APIs")["client_id"]);
                postParams.Add("client_secret", this._config.GetSection("APIs")["client_secret"]);
                postParams.Add("redirect_uri", $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + this._config.GetSection("APIs")["redirect_uri"]);
                postParams.Add("code", code);

                var formUrlEncodedContent = new FormUrlEncodedContent(postParams);


                //The url to post to.
                var url = "https://api.hubapi.com/oauth/v1/token";
                var client = new HttpClient();

                //Pass in the full URL and the json string content
                var response = await client.PostAsync(url, formUrlEncodedContent);


                //It would be better to make sure this request actually made it through
                string result = await response.Content.ReadAsStringAsync();

                var Token = JsonConvert.DeserializeObject<Token>(result);

                if (status.EXPIRED_AUTH_CODE == Token.Status)
                {
                    ErrorLog.log(Token.message.ToString());
                    return RedirectToAction("HubSportRequest");
                }
                else if (status.MISMATCH_REDIRECT_URI_AUTH_CODE == Token.Status)
                {
                    ErrorLog.log(Token.message.ToString());
                    ShowErrorMessage(Token.message.ToString(), true);
                    return RedirectToAction("Index", "Lead");
                }
                else if (status.BAD_REDIRECT_URI == Token.Status)
                {
                    ErrorLog.log(Token.message.ToString());
                    ShowErrorMessage(Token.message.ToString(), true);
                    return RedirectToAction("Index", "Lead");
                }
                else
                {
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri("https://api.hubapi.com/contacts/v1/lists/all/contacts/all"),
                        Headers = {
                                { HttpRequestHeader.Authorization.ToString(), "Bearer "+Token.access_token},
                                { HttpRequestHeader.Accept.ToString(), "application/json" },
                        },
                    };

                    var responseContact = client.SendAsync(httpRequestMessage).Result;
                    var data = await responseContact.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<HubSpotEntity.Root>(data);
                    List<TblLead> leadList = new List<TblLead>();
                    if (model.contacts != null)
                    {
                        foreach (var item in model.contacts)
                        {
                            TblLead lead = new TblLead();
                            lead.FirstName = item.properties.firstname == null ? string.Empty : item.properties.firstname.value.ToString();
                            lead.LastName = item.properties.lastname == null ? string.Empty : item.properties.lastname.value.ToString();
                            //lead.CreatedDate = Convert.ToDateTime(item.addedAt.ToString());
                            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToInt64(item.addedAt) / 1000d)).ToLocalTime();
                            lead.CreatedDate = Convert.ToDateTime(dt);
                            lead.Company = item.properties.company == null ? string.Empty : item.properties.company.value.ToString();
                            lead.EmailAddress = string.IsNullOrEmpty(item.IdentityProfiles.FirstOrDefault().identities.FirstOrDefault().value.ToString()) ? string.Empty : item.IdentityProfiles.FirstOrDefault().identities.FirstOrDefault().value.ToString();
                            lead.CompanyId = CompanyId;
                            var resultLead = _dbContext.TblLeads.Where(u => u.EmailAddress.ToLower().Equals(lead.EmailAddress.ToLower()) && u.CompanyId == CompanyId).ToList();
                            if (resultLead.Count() > 0)
                                continue;
                            leadList.Add(lead);
                        }
                        _dbContext.TblLeads.AddRange(leadList);
                        _dbContext.SaveChanges();
                        //close out the client
                        client.Dispose();

                        ShowSuccessMessage(model.contacts.Count + " Leads are successfully imported from HubSpot!", true);

                    }
                    else
                    {
                        ShowSuccessMessage("0 Leads are founds from HubSpot!", true);
                    }

                    return RedirectToAction("Index", "Lead");
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return View("NewLead");
            }

        }

        public async Task<IActionResult> GetLeadsFromHubSpot(int? count)
        {
            try
            {
                string uri = _config.GetValue<string>("APIs:HubspotAPIKey");
                if (count > 0)
                {
                    uri = uri + count;
                }
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(uri),
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var data = await response.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<HubSpotEntity.Root>(data);
                    List<TblLead> leadList = new List<TblLead>();
                    foreach (var item in model.contacts)
                    {
                        TblLead lead = new TblLead();
                        lead.FirstName = item.properties.firstname == null ? string.Empty : item.properties.firstname.value.ToString();
                        lead.LastName = item.properties.lastname == null ? string.Empty : item.properties.lastname.value.ToString();
                        //lead.CreatedDate = Convert.ToDateTime(item.addedAt.ToString());
                        var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Convert.ToInt64(item.addedAt) / 1000d)).ToLocalTime();
                        lead.CreatedDate = Convert.ToDateTime(dt);
                        lead.Company = item.properties.company == null ? string.Empty : item.properties.company.value.ToString();
                        lead.EmailAddress = string.IsNullOrEmpty(item.IdentityProfiles.FirstOrDefault().identities.FirstOrDefault().value.ToString()) ? string.Empty : item.IdentityProfiles.FirstOrDefault().identities.FirstOrDefault().value.ToString();

                        var result = _dbContext.TblLeads.Where(u => u.EmailAddress.ToLower().Equals(lead.EmailAddress.ToLower())).ToList();
                        if (result.Count() > 0)
                            continue;
                        leadList.Add(lead);
                    }
                    _dbContext.TblLeads.AddRange(leadList);
                    _dbContext.SaveChanges();
                }
                ShowSuccessMessage("Leads are successfully added from HubSpot!", true);
                return RedirectToAction("Index", "Lead");
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return View("NewLead");
            }

        }

        [HttpPost]
        public ActionResult DeleteLeads()
        {
            string strIDs = string.Empty;
            if (Request.Form.ContainsKey("ids")) { strIDs = Request.Form["ids"]; }
            string[] intIDs = strIDs.Split(",");
            try
            {
                for (int i = 0; i < intIDs.Length; i++)
                {
                    int intLeadID = Convert.ToInt32(intIDs[i]);
                    if (intLeadID > 0)
                    {
                        if (!DeleteSingleLead(intLeadID))
                            return Json(new { success = true });
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public bool DeleteSingleLead(int id)
        {
            try
            {
                if (Request.Form.ContainsKey("leadID"))
                {
                    id = Convert.ToInt32(Request.Form["leadID"]);
                }
                _dbContext.TblLeads.Remove(_dbContext.TblLeads.Where(x => x.LeadId == id).FirstOrDefault());
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return false;
            }
        }

        public ActionResult LeadDetail(int? id)
        {
            try
            {
                LoadComboBoxes();
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");
                var lead = _dbContext.TblLeads.Where(x => x.LeadId == id).Include(x => x.Agent).Include(x => x.StageNavigation).FirstOrDefault();
                LeadViewModel oModel = new LeadViewModel();
                oModel.LeadId = lead.LeadId;
                oModel.LeadSource = lead.LeadSource;
                oModel.LeadStatus = lead.LeadStatus;
                oModel.Industry = lead.Industry;
                //oModel.Stage = lead.Stage;
                oModel.StageId = lead.StageId;
                oModel.Stage = lead.StageNavigation;
                oModel.TagsName = GetTagsName(lead.LeadId);
                oModel.AgentId = lead.AgentId;
                oModel.Agent = lead.Agent;
                oModel.OwnerImg = lead.OwnerImg;
                oModel.LeadOwner = lead.LeadOwner;
                oModel.Company = lead.Company;
                oModel.FirstName = lead.FirstName;
                oModel.LastName = lead.LastName;
                oModel.Title = lead.Title;
                oModel.EmailAddress = lead.EmailAddress;
                oModel.PhoneNumber = lead.PhoneNumber;
                oModel.Fax = lead.Fax;
                oModel.MobileNumber = lead.MobileNumber;
                oModel.Website = lead.Website;
                oModel.NoOfEmp = lead.NoOfEmp;
                oModel.AnnualRevenue = lead.AnnualRevenue;
                oModel.Rating = lead.Rating;
                oModel.EmailOptOut = lead.EmailOptOut == true ? true : false;
                oModel.SkypeId = lead.SkypeId;
                oModel.TwitterId = lead.TwitterId;
                oModel.SecondaryEmail = lead.SecondaryEmail;
                oModel.Street = lead.Street;
                oModel.State = lead.State;
                oModel.Country = lead.Country;
                oModel.City = lead.City;
                oModel.ZipCode = lead.ZipCode;
                oModel.Description = lead.Description;
                oModel.CreatedDate = lead.CreatedDate;
                return View(oModel);
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult SendBulkMail()
        {
            try
            {
                if (!Request.Cookies.ContainsKey("FullName") && !Request.Cookies.ContainsKey("EmailAddress"))
                    return RedirectToAction("Login", "Account");

                string subject = Request.Form["subject"];
                string MailBody = Request.Form["mailBody"];
                var checkedEmailList = Request.Form["checkedEmailList"].ToString().Split(",");
                var checkedAgentEmailList = Request.Form["checkedAgentEmailList"].ToString().Split(",");

                var emailList = new HashSet<string>(checkedEmailList);
                var fromEmailList = new HashSet<string>(checkedAgentEmailList);
                if (emailList.Count > 0)
                {
                    foreach (var item in emailList)
                    {
                        string SmtpUserName = this._config.GetSection("MailSettings")["SmtpUserName"];
                        string SmtpPassword = this._config.GetSection("MailSettings")["SmtpPassword"];
                        int SmtpPort = Convert.ToInt32(this._config.GetSection("MailSettings")["SmtpPort"]);
                        string SmtpServer = this._config.GetSection("MailSettings")["SmtpServer"];
                        string fromEmail = this._config.GetSection("MailSettings")["fromEmail"];
                        bool isSSL = Convert.ToBoolean(this._config.GetSection("MailSettings")["isSSL"]);

                        Utility.sendMail(item, MailBody, subject, fromEmail, SmtpUserName, SmtpPassword, SmtpPort, SmtpServer, isSSL);
                    }
                    return Json(new { success = true, message = "Your emails are on the way!" });
                }
                else
                {
                    return Json(new { success = false, message = "No Email Found!" });
                }
            }
            catch (Exception ex)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                ErrorLog.log(DateTime.Now + "--" + actionName + "--" + controllerName + "--\n" + ex);
            }
            return View();
        }


        #region Function
        public bool IsUserExist(string email)
        {
            try
            {
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                var result = _dbContext.TblLeads.Where(u => u.EmailAddress.ToLower().Equals(email.ToLower()) && u.CompanyId == CompanyId).ToList();
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
        public string GetTagsName(int LeadId)
        {
            try
            {
                int CompanyId = Convert.ToInt32(Request.Cookies["LoginCompanyId"]);
                var leadTag = _dbContext.TblLeadTags.Where(x => x.LeadId == LeadId && x.CompanyId == CompanyId).ToList();
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
        #endregion
    }
}
