using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class LeadsController : MainLayoutControllerBase
    {
        private readonly ILeadsViewModelAdapter _viewModelAdapter;
        private readonly ICRMLeadService _crmLeadService;
        private readonly ICRMCampaignService _crmCampaignService;
        private readonly IAccountService _accountService;

        public LeadsController(ILeadsViewModelAdapter viewModelAdapter, ICRMLeadService crmLeadService, ICRMCampaignService crmCampaignService, IAccountService accountService)
        {
            _viewModelAdapter = viewModelAdapter;
            _crmLeadService = crmLeadService;
            _crmCampaignService = crmCampaignService;
            _accountService = accountService;
        }

        public ActionResult ViewPage(LeadsWebQuery query)
        {
            DateTime tempEnd = System.DateTime.Now;
            DateTime tempStart = tempEnd.AddDays(-30);

            DateTime startDate = query.StartDate != DateTime.MinValue ? query.StartDate : new DateTime(tempStart.Year, tempStart.Month, tempStart.Day);
            DateTime endDate = query.EndDate != DateTime.MinValue ? query.EndDate : new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day);

            // Get data...
            List<string> accountObjectIDs = new List<string>();
            if (!query.IsAjax)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["workspacesAccountIdsCache"])))
                {
                    if ("none" != Convert.ToString(Session["workspacesAccountIdsCache"]))
                        accountObjectIDs = Convert.ToString(Session["workspacesAccountIdsCache"]).Split(',').ToList<string>();
                }
                else if ("none" == Convert.ToString(Session["workspacesAccountIdsCache"]))
                {

                }
                else
                {
                    accountObjectIDs.Add(User.MxtrAccountObjectID);
                    List<string> childAccountObjectIDs = _accountService.GetFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
                    accountObjectIDs.AddRange(childAccountObjectIDs);
                }
                if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["dateFilter"])))
                {
                    List<string> dateFilter = Convert.ToString(Session["dateFilter"]).Split(',').ToList();
                    startDate = Convert.ToDateTime(dateFilter[0]);
                    endDate = Convert.ToDateTime(dateFilter[1]);
                }
            }
            else
            {
                Session["dateFilter"] = query.StartDate.ToString() + ',' + query.EndDate.ToString();
                if (!string.IsNullOrEmpty(query.AccountObjectID))
                    accountObjectIDs = query.AccountObjectID.Split(',').ToList<string>();
                else
                    Session["workspacesAccountIdsCache"] = "none";
            }

            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);


            // Get data...

            IEnumerable<CRMLeadDataModel> leads = _crmLeadService.GetLeadsByAccountObjectIDs(accountObjectIDs, startDate, endDate);

            IEnumerable<CRMCampaignDataModel> campaigns = _crmCampaignService.GetCampaignsByAccountObjectIDs(accountObjectIDs, startDate, endDate);

            // Adapt data...
            LeadsViewModel model = _viewModelAdapter.BuildLeadsViewModel(leads, campaigns, accountObjectIDs, startDate, endDate);

            //TempData["LeadsData"] = model.Leads;
            Session["LeadsData"] = model.Leads;
            model.Leads = null;
            ViewBag.DataTableIdentifier = "LeadsData";

            TempData["AccountObjectIDs"] = accountObjectIDs;
            TempData["StartDate"] = startDate;
            TempData["EndDate"] = endDate;

            // Handle...
            if (query.IsAjax)
            {
                if (!string.IsNullOrWhiteSpace(query.AccountObjectID))
                    Session["workspacesAccountIdsCache"] = query.AccountObjectID;
                else
                    Session["workspacesAccountIdsCache"] = "none";
                model.Success = true;
                return Json(model);
            }

            return View(ViewKind.Leads, model, query);
        }

        [HttpPost]
        [Route("GetLeadTableData")]
        public ActionResult GetLeadTableData(int draw, int start, int length)
        {
            try
            {
                DateTime startDate = Convert.ToDateTime(TempData["StartDate"]);
                DateTime endDate = Convert.ToDateTime(TempData["EndDate"]);
                List<string> accountObjectIDs = (List<string>)TempData["AccountObjectIDs"];
                TempData.Keep("StartDate");
                TempData.Keep("EndDate");
                TempData.Keep("AccountObjectIDs");

                IEnumerable<CRMCampaignDataModel> campaigns = _crmCampaignService.GetCampaignsByAccountObjectIDs(accountObjectIDs, startDate, endDate);
                IQueryable<CRMLeadDataModel> memberCol = _crmLeadService.GetLeadsByAccountObjectIDsAsQueryable(accountObjectIDs, startDate, endDate);
                int totalCount = memberCol.Count();
                IEnumerable<CRMLeadDataModel> filteredMembers = memberCol;
                string search = "", SearchColumnName = "";//, OrderColumnName = "";
                int sortColumn = -1, OrderColumnName = 0;
                string sortDirection = "asc";

                search = Request.Form["search[value]"];
                SearchColumnName = Request.Form["columns[" + 0 + "][data]"];
                if (Request.Form["order[0][column]"] != null)
                {
                    sortColumn = int.Parse(Request.Form["order[0][column]"]);
                    //OrderColumnName = Request.Form["columns[" + sortColumn + "][data]"];
                }

                if (Request.Form["order[0][dir]"] != null)
                {
                    sortDirection = Request.Form["order[0][dir]"];
                }

                if (!string.IsNullOrEmpty(search))
                {
                    filteredMembers = memberCol.ToList()
                            .Where(m => (m.FirstName != null && m.FirstName.Contains(search)) ||
                               (m.LastName != null && m.LastName.Contains(search)) ||
                              (m.EmailAddress != null && m.EmailAddress.Contains(search)) ||
                               (m.CompanyName != null && m.CompanyName.Contains(search)));

                    totalCount = filteredMembers.Count();
                }

                Func<CRMLeadDataModel, string> orderingFunction = (m =>
                                      OrderColumnName == 0 ? m.FirstName :
                                      OrderColumnName == 1 ? m.LastName :
                                      OrderColumnName == 2 ? m.EmailAddress :
                                      OrderColumnName == 3 ? m.LeadParentAccount :
                                      OrderColumnName == 4 ? m.CompanyName :
                                      OrderColumnName == 5 ? campaigns.Where(c => c.CampaignID == m.CampaignID).Select(c => c.CampaignName).FirstOrDefault() ?? "Unassigned" :
                                      OrderColumnName == 6 ? m.LeadScore.ToString() :
                                      OrderColumnName == 7 ? m.Events.Count().ToString() :
                                      OrderColumnName == 8 ? GetLastTouch(m) :
                                      m.EmailAddress);

                if (sortDirection == "asc")
                    filteredMembers = filteredMembers.OrderBy(orderingFunction);
                else
                    filteredMembers = filteredMembers.OrderByDescending(orderingFunction);

                var displayedMembers = filteredMembers.Skip(start).Take(length);
                var result = displayedMembers.Select(l => new Models.Leads.ViewData.LeadViewData
                {
                    ObjectID = l.ObjectID,
                    AccountObjectID = l.AccountObjectID,
                    CreateDate = l.CreateDate,
                    LeadID = l.LeadID,
                    OwnerID = l.OwnerID,
                    CampaignID = l.CampaignID,
                    CampaignName = campaigns.Where(c => c.CampaignID == l.CampaignID).Select(c => c.CampaignName).FirstOrDefault() ?? "Unassigned",
                    LeadStatus = l.LeadStatus,
                    LeadScore = l.LeadScore,
                    IsActive = l.IsActive,
                    FirstName = l.FirstName ?? string.Empty,
                    LastName = l.LastName ?? string.Empty,
                    EmailAddress = l.EmailAddress ?? string.Empty,
                    CompanyName = l.CompanyName ?? string.Empty,
                    City = l.City ?? string.Empty,
                    State = l.State ?? string.Empty,
                    EventsCount = l.Events.Count(),
                    EventLastTouch = GetLastTouch(l),
                    LeadParentAccount = l.LeadParentAccount,
                }).OrderBy(l => l.CreateDate);

                DataTableData dataTableData = new DataTableData();
                dataTableData.draw = draw;
                dataTableData.recordsTotal = totalCount;
                dataTableData.data = result.ToList();
                dataTableData.recordsFiltered = totalCount;

                return Json(dataTableData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static string GetLastTouch(CRMLeadDataModel leadData)
        {
            var data = leadData.Events.OrderByDescending(t => t.CreateTimestamp).FirstOrDefault();
            if (data == null)
            {
                return string.Empty;
            }
            return data.CreateTimestamp.ToString("yyyy-MM-dd") ?? string.Empty;
        }

        public class DataTableData
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<Models.Leads.ViewData.LeadViewData> data { get; set; }
        }
    }
}
