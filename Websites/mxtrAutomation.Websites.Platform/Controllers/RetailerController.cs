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
    public class RetailerController : MainLayoutControllerBase
    {
        private readonly IRetailerViewModelAdapter _viewModelAdapter;
        private readonly ICRMRestSearchResponseLogService _cRMRestSearchResponseLogService;
        private readonly IAccountService _accountService;
        private readonly ICRMLeadService _cRMLeadService;
        private readonly ICRMEmailJobService _cRMEmailJobService;
        private readonly IGoogleAnalyticService _googleAnalyticService;

        public RetailerController(IRetailerViewModelAdapter viewModelAdapter, ICRMRestSearchResponseLogService cRMRestSearchResponseLogService, IAccountService accountService, ICRMLeadService cRMLeadService, ICRMEmailJobService cRMEmailJobService, IGoogleAnalyticService googleAnalyticService)
        {
            _viewModelAdapter = viewModelAdapter;
            _cRMRestSearchResponseLogService = cRMRestSearchResponseLogService;
            _accountService = accountService;
            _cRMLeadService = cRMLeadService;
            _cRMEmailJobService = cRMEmailJobService;
            _googleAnalyticService = googleAnalyticService;
        }


        public ActionResult ViewPage(RetailerWebQuery query)
        {
            DateTime tempEnd = System.DateTime.Now;
            DateTime tempStart = tempEnd.AddDays(-30);

            DateTime startDate = query.StartDate != DateTime.MinValue ? query.StartDate : new DateTime(tempStart.Year, tempStart.Month, tempStart.Day);
            DateTime endDate = query.EndDate != DateTime.MinValue ? query.EndDate : new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day);
            // Get data...
            List<string> accountObjectIDs = new List<string>();
            if (!query.IsAjax)
            {
                accountObjectIDs.Add(query.ObjectID);
                //accountObjectIDs.Add(User.MxtrUserObjectID);
                //List<string> childAccountObjectIDs = _accountService.GetFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
                //accountObjectIDs.AddRange(childAccountObjectIDs);
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
                if (!string.IsNullOrEmpty(query.AccountObjectIDs))
                    accountObjectIDs = query.AccountObjectIDs.Split(',').ToList<string>();
            }

            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            //List<mxtrAccount> accounts = _accountService.GetAccountsByAccountObjectIDs(accountObjectIDs);
            List<mxtrAccount> accounts = _accountService.GetAllAccountsByAccountObjectIDs(accountObjectIDs);

            List<CRMLeadSummaryDataModel> leads = _cRMLeadService.GetLeadCountsByAccountObjectIDs(accountObjectIDs, startDate, endDate);

            List<CRMRestSearchResponseLogModel> searchResponses =
                _cRMRestSearchResponseLogService.GetSearchResponseSummariesByAccountObjectIDs(accountObjectIDs, startDate, endDate);

            List<CRMEmailJobDataModel> emails =
                _cRMEmailJobService.GetEmailJobsGroupedByAccountObjectIDsForDateRange(accountObjectIDs, startDate, endDate);

            List<CRMLeadSummaryDataModel> leadsDate = _cRMLeadService.GetLeadCountsByDate(accountObjectIDs, startDate, endDate);

            List<CRMRestSearchResponseLogModel> searchResponsesDate =
                _cRMRestSearchResponseLogService.GetSearchResponseSummariesByDate(accountObjectIDs, startDate, endDate);

            List<CRMEmailJobDataModel> emailsDate =
                _cRMEmailJobService.GetEmailJobsGroupedByDate(accountObjectIDs, startDate, endDate);

            List<CRMLeadDataModel> retailerLeads = _cRMLeadService.GetLeadsByAccountObjectIDsForDateRange(accountObjectIDs, startDate, endDate);

            IEnumerable<GASearchDataModel> gaData = _googleAnalyticService.GetGoogleAnalyticData(accountObjectIDs, startDate, endDate);

            // Adapt data...            
            RetailerViewModel model = _viewModelAdapter.BuildRetailerViewModel(accounts, searchResponses, leads, emails, searchResponsesDate, leadsDate, emailsDate, startDate, endDate, accountObjectIDs, retailerLeads, gaData);
            if(model.RetailerActivityReportViewData==null)
            {
                model.RetailerActivityReportViewData = new Models.Retailers.ViewData.RetailerActivityReportViewData();
            }
            // Handle...
            if (query.IsAjax)
            {
                model.Success = true;
                return Json(model);
            }

            return View(ViewKind.Retailer, model, query);
        }
    }
}
