using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class RetailersController : MainLayoutControllerBase
    {
        private readonly IRetailersViewModelAdapter _viewModelAdapter;
        private readonly ICRMRestSearchResponseLogService _cRMRestSearchResponseLogService;
        private readonly IAccountService _accountService;
        private readonly ICRMLeadService _cRMLeadService;
        private readonly ICRMEmailJobService _cRMEmailJobService;
        private readonly IGoogleAnalyticService _googleAnalyticService;

        public RetailersController(IRetailersViewModelAdapter viewModelAdapter, ICRMRestSearchResponseLogService cRMRestSearchResponseLogService,
            IAccountService accountService, ICRMLeadService cRMLeadService, ICRMEmailJobService cRMEmailJobService, IGoogleAnalyticService googleAnalyticService)
        {
            _viewModelAdapter = viewModelAdapter;
            _cRMRestSearchResponseLogService = cRMRestSearchResponseLogService;
            _accountService = accountService;
            _cRMLeadService = cRMLeadService;
            _cRMEmailJobService = cRMEmailJobService;
            _googleAnalyticService = googleAnalyticService;
        }

        public ActionResult ViewPage(RetailersWebQuery query)
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
                    List<string> childAccountObjectIDs = _accountService.GetAllFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
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
                Session["dateFilter"] = startDate.ToString() + ',' + endDate.ToString();
                if (!string.IsNullOrEmpty(query.AccountObjectIDs))
                    accountObjectIDs = query.AccountObjectIDs.Split(',').ToList<string>();
                else
                    Session["workspacesAccountIdsCache"] = "none";

                // Hack for Active/Inactive accounts
                List<string> childAccountObjectIDs = _accountService.GetAllFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
                List<string> activeAccountObjectIDs = _accountService.GetFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
                activeAccountObjectIDs.RemoveAll(x => accountObjectIDs.Select(y => y).Contains(x));
                childAccountObjectIDs.RemoveAll(x => activeAccountObjectIDs.Select(y => y).Contains(x));
                accountObjectIDs = childAccountObjectIDs;
            }

            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            IEnumerable<mxtrAccount> accounts = _accountService.GetAccountsByAccountObjectIDsAsIEnumberable(accountObjectIDs);

            #region Lead
            IEnumerable<CRMLeadDataModel> leadsFiltered = _cRMLeadService.GetLeadsByAccountObjectIDs(accountObjectIDs, startDate, endDate);

            List<CRMLeadSummaryDataModel> leads = _cRMLeadService.GetLeadCountsByAccountObjectIDs(leadsFiltered);

            List<CRMLeadSummaryDataModel> leadsDate = _cRMLeadService.GetLeadCountsByDate(leadsFiltered);

            #endregion

            #region Search Response
            IEnumerable<CRMRestSearchResponseLogModel> searchResponsesFiltered = _cRMRestSearchResponseLogService.GetRestSearchResponseLogByAccountObjectIDs(accountObjectIDs, startDate, endDate);

            List<CRMRestSearchResponseLogModel> searchResponses =
                _cRMRestSearchResponseLogService.GetSearchResponseSummariesByAccountObjectIDs(searchResponsesFiltered);

            List<CRMRestSearchResponseLogModel> searchResponsesDate =
            _cRMRestSearchResponseLogService.GetSearchResponseSummariesByDate(searchResponsesFiltered);
            #endregion

            #region Email
            IEnumerable<CRMEmailJobDataModel> emailsFiltered =
                _cRMEmailJobService.GetEmailJobs(accountObjectIDs, startDate, endDate);

            List<CRMEmailJobDataModel> emails =
                _cRMEmailJobService.GetEmailJobsGroupedByAccountObjectIDsForDateRange(emailsFiltered);

            List<CRMEmailJobDataModel> emailsDate =
               _cRMEmailJobService.GetEmailJobsGroupedByDate(emailsFiltered);
            #endregion

            #region Google Analytics
            IEnumerable<GASearchDataModel> gaData = _googleAnalyticService.GetGoogleAnalyticData(accountObjectIDs, startDate, endDate);
            #endregion

            // Adapt data...
            RetailersViewModel model = _viewModelAdapter.BuildRetailersViewModel(accounts, searchResponses, leads, emails, searchResponsesDate, leadsDate, emailsDate, startDate, endDate, accountObjectIDs, gaData);
            model.RetailerActivityReportViewData = model.RetailerActivityReportViewData.Where(x => !String.IsNullOrEmpty(x.AccountName)).ToList();
            Session["BuildRetailers"] = model.RetailerActivityReportViewData;
            model.RetailerActivityReportViewData = null;
            ViewBag.DataTableIdentifier = "BuildRetailers";
            // Handle...
            if (query.IsAjax)
            {
                if (!string.IsNullOrWhiteSpace(query.AccountObjectIDs))
                    Session["workspacesAccountIdsCache"] = query.AccountObjectIDs;
                else
                    Session["workspacesAccountIdsCache"] = "none";
                model.Success = true;
                return Json(model);
            }

            return View(ViewKind.Retailers, model, query);
        }

    }
}
