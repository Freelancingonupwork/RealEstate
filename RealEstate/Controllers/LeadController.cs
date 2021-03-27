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
                if (!Request.Cookies.ContainsKey("FullName"))
                    return RedirectToAction("Login", "Account");
                if (Convert.ToInt32(Request.Cookies["UserLoginTypeId"]) != UserLoginType.Admin.GetHashCode())
                {
                    using (var DB = _dbContext)
                    {
                        var oLeadList = DB.TblLeads.Include(x => x.Agent).ToList().Select(s => new LeadViewModel
                        {
                            LeadId = s.LeadId,
                            LeadSource = s.LeadSource,
                            LeadStatus = s.LeadStatus,
                            Industry = s.Industry,
                            Stage = s.Stage,
                            AgentId = s.AgentId,
                            Agent = s.Agent,
                            OwnerImg = s.OwnerImg,
                            LeadOwner = s.LeadOwner,
                            Company = s.Company,
                            FirstName = s.FirstName,
                            LastName = s.FirstName,
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
                else
                {
                    var oLeadList = _dbContext.TblLeads.Include(x => x.Agent).ToList().Select(s => new LeadViewModel
                    {
                        LeadId = s.LeadId,
                        LeadSource = s.LeadSource,
                        LeadStatus = s.LeadStatus,
                        Industry = s.Industry,
                        Stage = s.Stage,
                        AgentId = s.AgentId,
                        Agent = s.Agent,
                        OwnerImg = s.OwnerImg,
                        LeadOwner = s.LeadOwner,
                        Company = s.Company,
                        FirstName = s.FirstName,
                        LastName = s.FirstName,
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
                ErrorLog.log(ex);
                return View();
            }
        }

        [HttpPost]
        public IActionResult AddNewLead(LeadViewModel model)
        {
            try
            {
                LoadComboBoxes();
                if (Request.Cookies.ContainsKey("FullName"))
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

                        TblLead oData = new TblLead();
                        oData.LeadOwner = model.LeadOwner;
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
                        oData.Stage = model.Stage;
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
                ErrorLog.log(ex);
                return View();
            }
        }

        public IActionResult NewLead()
        {
            try
            {
                if (Request.Cookies.ContainsKey("FullName"))
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
                ErrorLog.log("Admin Controller LeadFlow :-" + ex.Message);
                return RedirectToAction("Login", "Account");
            }
        }

        public void LoadComboBoxes()
        {
            try
            {
                Dictionary<string, string> sOwnerList = new Dictionary<string, string>();
                var users = _dbContext.TblUsers.Where(u => u.UserLoginTypeId != UserLoginType.Admin.GetHashCode() && u.IsActive == true).ToList();
                foreach (var item in users)
                {
                    sOwnerList.Add(item.FullName, item.FullName);
                }
                ViewBag.OwnerList = sOwnerList;


                Dictionary<string, string> oSourcelist = new Dictionary<string, string>();

                var leads = _dbContext.TblLeads.ToList();
                var items = leads.Select(x => x.LeadSource).Distinct();
                foreach (string item in items)
                {
                    oSourcelist.Add(item, item);
                }
                ViewBag.LeadSourceList = oSourcelist;


                Dictionary<int, string> oAgentlist = new Dictionary<int, string>();
                var agents = _dbContext.TblAgents.Where(u => u.IsActive == true).ToList();
                foreach (var item in agents)
                {
                    oAgentlist.Add(item.Id, item.FullName);
                }
                ViewBag.AgentList = oAgentlist;

            }
            catch (Exception ex)
            {
                ErrorLog.log("Lead Controller LoadComboBoxes fun :- " + ex);
            }
        }

        public ActionResult GetAllSources()
        {
            try
            {
                return Json(_dbContext.TblLeadSources.Select(s => new
                {
                    id = s.LeadSourceId,
                    name = s.LeadSourceName
                }).ToList());
            }
            catch (Exception ex)
            {
                ErrorLog.log("Lead GetAllSources" + ex);
                return Json(new { success = false, message = "Error occur while getting record!" + ex.Message });
            }

        }

        public ActionResult DeleteSource()
        {
            try
            {
                int id = 0;
                if (Request.Form.ContainsKey("id")) { id = Convert.ToInt32(Request.Form["id"]); }
                if (id <= 0) { throw new Exception("Identity can not be blank!"); }
                _dbContext.TblLeadSources.Remove(_dbContext.TblLeadSources.Where(x => x.LeadSourceId == id).FirstOrDefault());
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorLog.log("LeadController DeleteSource" + ex);
                return Json(new { success = false, message = "Error occur while deleting record!" + ex.Message });
            }
        }
        public ActionResult AddSource()
        {
            try
            {
                string name = string.Empty;
                if (Request.Form.ContainsKey("name")) { name = Request.Form["name"]; }
                if (string.IsNullOrEmpty(name)) { throw new Exception("Source name can not be blank!"); }
                var lead = _dbContext.TblLeadSources.Where(x => x.LeadSourceName.ToLower().Equals(name.ToLower())).FirstOrDefault();
                if (lead != null)
                {
                    return Json(new { success = false, message = "Lead source with name '" + name + "' is already exist" });
                }

                TblLeadSource src = new TblLeadSource();
                src.LeadSourceName = name;
                _dbContext.TblLeadSources.Add(src);
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorLog.log("Lead AddSource" + ex);
                return Json(new { success = false, message = "Error occur while inserting record!" + ex.Message });
            }

        }
        public bool IsUserExist(string email)
        {
            try
            {
                var result = _dbContext.TblLeads.Where(u => u.EmailAddress.ToLower().Equals(email.ToLower())).ToList();
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

        public async Task<IActionResult> Upload(IFormFile postedFile)
        {
            try
            {
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

                                leadList.Add(new TblLead
                                {
                                    LeadSource = strLeadSource,
                                    LeadStatus = strLeadStatus,
                                    Industry = strIndustry,
                                    Stage = strStage,
                                    OwnerImg = strOwnerImg,
                                    LeadOwner = strLeadOwner,

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

                            leadList.Add(new TblLead
                            {
                                LeadSource = strLeadSource,
                                LeadStatus = strLeadStatus,
                                Industry = strIndustry,
                                Stage = strStage,
                                OwnerImg = strOwnerImg,
                                LeadOwner = strLeadOwner,

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
                ErrorLog.log(ex.Message.ToString());
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

        public async Task<IActionResult> GetLeadsFromHubSpot(int? count)
        {
            try
            {
                string uri = _config.GetValue<string>("APIs:HubspotAPIForAllContact");
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
                ErrorLog.log("leadController GetLeadsFromHubSpot" + ex);
                ViewBag.MessageType = "danger";
                //ViewBag.Message = "Opps! Something went wrong!";
                ShowErrorMessage("Opps! Something went wrong!", true);
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
                ErrorLog.log("Lead Controller DeleteLeads :-" + ex.Message);
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
                ErrorLog.log("LeadController DeleteSingleLead" + ex);
                return false;
            }
        }

        public ActionResult LeadDetail(int? id)
        {
            try
            {
                LoadComboBoxes();
                if (!Request.Cookies.ContainsKey("FullName"))
                    return RedirectToAction("Login", "Account");
                var lead = _dbContext.TblLeads.Where(x => x.LeadId == id).Include(x => x.Agent).FirstOrDefault();
                LeadViewModel oModel = new LeadViewModel();
                oModel.LeadId = lead.LeadId;
                oModel.LeadSource = lead.LeadSource;
                oModel.LeadStatus = lead.LeadStatus;
                oModel.Industry = lead.Industry;
                oModel.Stage = lead.Stage;
                oModel.AgentId = lead.AgentId;
                oModel.Agent = lead.Agent;
                oModel.OwnerImg = lead.OwnerImg;
                oModel.LeadOwner = lead.LeadOwner;
                oModel.Company = lead.Company;
                oModel.FirstName = lead.FirstName;
                oModel.LastName = lead.FirstName;
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
                ErrorLog.log("Lead Controller LeadDetail :-" + ex.Message);
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
