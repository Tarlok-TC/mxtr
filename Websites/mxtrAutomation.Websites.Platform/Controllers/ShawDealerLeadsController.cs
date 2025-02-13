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
    public class ShawDealerLeadsController : MainLayoutControllerBase
    {
        private readonly IShawDealerLeadsViewModelAdapter _viewModelAdapter;
        private readonly ICRMLeadService _crmLeadService;
        private readonly ICRMCampaignService _crmCampaignService;
        private readonly IAccountService _accountService;

        public ShawDealerLeadsController(IShawDealerLeadsViewModelAdapter viewModelAdapter, ICRMLeadService crmLeadService, ICRMCampaignService crmCampaignService, IAccountService accountService)
        {
            _viewModelAdapter = viewModelAdapter;
            _crmLeadService = crmLeadService;
            _crmCampaignService = crmCampaignService;
            _accountService = accountService;
        }

        public ActionResult ViewPage(ShawDealerLeadsWebQuery query)
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
                    //accountObjectIDs.Add(User.MxtrAccountObjectID);
                    List<string> childAccountObjectIDs = _accountService.GetFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
                    accountObjectIDs.AddRange(childAccountObjectIDs);
                    accountObjectIDs.Remove(User.MxtrAccountObjectID);
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

            IEnumerable<CRMLeadDataModel> leads = _crmLeadService.GetLeadsCreatedDateByAccountObjectIDs(accountObjectIDs, startDate, endDate);

            // IEnumerable<CRMCampaignDataModel> campaigns = _crmCampaignService.GetCampaignsByAccountObjectIDs(accountObjectIDs, startDate, endDate);            

            // Adapt data...
            ShawDealerLeadsViewModel model = _viewModelAdapter.BuildShawDealerLeadsViewModel(leads, accountObjectIDs, startDate, endDate);


            //Session["ShawLeadsData"] = model.Leads;
            //model.Leads = null;
            //ViewBag.DataTableIdentifier = "ShawLeadsData";

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

            return View(ViewKind.ShawDealerLeads, model, query);
        }

        [HttpPost]
        [Route("GetShawDealerLeadTableData")]
        public ActionResult GetShawDealerLeadTableData(int draw, int start, int length)
        {
            try
            {
                DateTime startDate = Convert.ToDateTime(TempData["StartDate"]);
                DateTime endDate = Convert.ToDateTime(TempData["EndDate"]);
                List<string> accountObjectIDs = (List<string>)TempData["AccountObjectIDs"];
                TempData.Keep("StartDate");
                TempData.Keep("EndDate");
                TempData.Keep("AccountObjectIDs");

                if (startDate == DateTime.MinValue || endDate == DateTime.MinValue || accountObjectIDs == null)
                {
                    endDate = System.DateTime.Now;
                    startDate = endDate.AddDays(-30);

                    if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["dateFilter"])))
                    {
                        List<string> dateFilter = Convert.ToString(Session["dateFilter"]).Split(',').ToList();
                        startDate = Convert.ToDateTime(dateFilter[0]);
                        endDate = Convert.ToDateTime(dateFilter[1]);
                    }
                    startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
                    endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
                    if (!string.IsNullOrEmpty(Convert.ToString(Session["workspacesAccountIdsCache"])) && Convert.ToString(Session["workspacesAccountIdsCache"]) != "none")
                    {
                        accountObjectIDs = Convert.ToString(Session["workspacesAccountIdsCache"]).Split(',').ToList<string>();
                    }
                    else
                    {
                        accountObjectIDs = new List<string>();
                        //accountObjectIDs.Add(User.MxtrAccountObjectID);
                        List<string> childAccountObjectIDs = _accountService.GetFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
                        accountObjectIDs.AddRange(childAccountObjectIDs);
                        accountObjectIDs.Remove(User.MxtrAccountObjectID);
                    }
                }

                IEnumerable<CRMCampaignDataModel> campaigns = _crmCampaignService.GetCampaignsByAccountObjectIDs(accountObjectIDs, startDate, endDate);
                //IQueryable<CRMLeadDataModel> memberCol = _crmLeadService.GetLeadsByAccountObjectIDsAsQueryable(accountObjectIDs, startDate, endDate);
                IEnumerable<CRMLeadDataModel> memberCol = _crmLeadService.GetLeadsByAccountObjectIDs(accountObjectIDs, startDate, endDate);
                int totalCount = memberCol.Count();
                IEnumerable<CRMLeadDataModel> filteredMembers = memberCol;
                string search = "", SearchColumnName = "";
                int sortColumn = 0;
                string sortDirection = "asc";

                search = Request.Form["search[value]"];
                SearchColumnName = Request.Form["columns[" + 0 + "][data]"];
                if (Request.Form["order[0][column]"] != null)
                {
                    sortColumn = int.Parse(Request.Form["order[0][column]"]);
                }

                if (Request.Form["order[0][dir]"] != null)
                {
                    sortDirection = Request.Form["order[0][dir]"];
                }

                if (!string.IsNullOrEmpty(search))
                {
                    filteredMembers = memberCol
                           .Where(m => (m.FirstName != null && m.FirstName.Contains(search)) ||
                               (m.LastName != null && m.LastName.Contains(search)) ||
                              (m.EmailAddress != null && m.EmailAddress.Contains(search)) ||
                              (m.LeadParentAccount != null && m.LeadParentAccount.Contains(search)) ||
                              m.LeadScore.ToString().Contains(search));

                    totalCount = filteredMembers == null ? 0 : filteredMembers.Count();
                }

                //Func<CRMLeadDataModel, dynamic> orderingFunction = (m =>
                //                      sortColumn == 0 ? m.FirstName :
                //                      sortColumn == 1 ? m.LastName :
                //                      sortColumn == 2 ? m.EmailAddress :
                //                      //sortColumn == 3 ? m.LeadParentAccount :
                //                      sortColumn == 3 ? m.CompanyName :
                //                      sortColumn == 4 ? campaigns.Where(c => c.CampaignID == m.CampaignID).Select(c => c.CampaignName).FirstOrDefault() ?? "Unassigned" :
                //                      sortColumn == 5 ? m.LeadScore.ToString() :
                //                      // OrderColumnName == 7 ? m.Events.Count().ToString() :
                //                      sortColumn == 6 ? GetLastTouch(m) :
                //                      m.EmailAddress);
                //if (sortDirection == "asc")
                //    filteredMembers = filteredMembers.OrderBy(orderingFunction);
                //else
                //    filteredMembers = filteredMembers.OrderByDescending(orderingFunction);


                IEnumerable<Models.Leads.ViewData.ShawDealerLeadViewData> result = null;
                if (sortColumn == 4 || sortColumn == 7)
                {
                    //result = AssignShawLeadViewData(campaigns, filteredMembers);
                    //if (sortColumn == 4)
                    //{
                    //    result = result.OrderBy(x => x.CampaignName);
                    //}
                    //else
                    //{
                    //    result = result.OrderBy(x => x.EventLastTouch);
                    //}
                    //result = result.Skip(start).Take(length);

                    var displayedMembers = filteredMembers.Skip(start).Take(length);
                    result = AssignShawLeadViewData(campaigns, displayedMembers);
                }
                else
                {
                    // Increase performance when we don't need data from different collections for sorting.
                    filteredMembers = SortDataColumns(filteredMembers, sortColumn, sortDirection);
                    var displayedMembers = filteredMembers.Skip(start).Take(length);
                    result = AssignShawLeadViewData(campaigns, displayedMembers);
                }

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

        private static IEnumerable<Models.Leads.ViewData.ShawDealerLeadViewData> AssignShawLeadViewData(IEnumerable<CRMCampaignDataModel> campaigns, IEnumerable<CRMLeadDataModel> displayedMembers)
        {
            IEnumerable<Models.Leads.ViewData.ShawDealerLeadViewData> result;
            result = displayedMembers.Select(l => new Models.Leads.ViewData.ShawDealerLeadViewData
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
                //EventsCount = l.Events.Count(),
                EventLastTouch = GetLastTouch(l),
                LeadParentAccount = l.LeadParentAccount,
            }).OrderBy(l => l.CreateDate);
            return result;
        }

        private static IEnumerable<CRMLeadDataModel> SortDataColumns(IEnumerable<CRMLeadDataModel> filteredMembers, int sortColumn, string sortDirection)
        {
            switch (sortColumn)
            {
                case 0:
                    if (sortDirection == "asc")
                        filteredMembers = filteredMembers.OrderBy(x => x.FirstName);
                    else
                        filteredMembers = filteredMembers.OrderByDescending(x => x.FirstName);
                    break;
                case 1:
                    if (sortDirection == "asc")
                        filteredMembers = filteredMembers.OrderBy(x => x.LastName);
                    else
                        filteredMembers = filteredMembers.OrderByDescending(x => x.LastName);
                    break;
                case 2:
                    if (sortDirection == "asc")
                        filteredMembers = filteredMembers.OrderBy(x => x.EmailAddress);
                    else
                        filteredMembers = filteredMembers.OrderByDescending(x => x.EmailAddress);
                    break;
                case 3:
                    if (sortDirection == "asc")
                        filteredMembers = filteredMembers.OrderBy(x => x.LeadParentAccount);
                    else
                        filteredMembers = filteredMembers.OrderByDescending(x => x.LeadParentAccount);
                    break;
                case 5:
                    if (sortDirection == "asc")
                        filteredMembers = filteredMembers.OrderBy(x => x.LeadScore);
                    else
                        filteredMembers = filteredMembers.OrderByDescending(x => x.LeadScore);
                    break;
                default:
                    if (sortDirection == "asc")
                        filteredMembers = filteredMembers.OrderBy(x => x.CreateDate);
                    else
                        filteredMembers = filteredMembers.OrderByDescending(x => x.CreateDate);
                    break;
            }

            return filteredMembers;
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
            public List<Models.Leads.ViewData.ShawDealerLeadViewData> data { get; set; }
        }
    }
}
