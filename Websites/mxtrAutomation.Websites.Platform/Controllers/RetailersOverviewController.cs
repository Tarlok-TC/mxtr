using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using Ninject;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Utils;
using mxtrAutomation.Common.Utils;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class RetailersOverviewController : MainLayoutControllerBase
    {
        private readonly IRetailersOverviewViewModelAdapter _viewModelAdapter;
        private readonly ICRMRestSearchResponseLogService _cRMRestSearchResponseLogService;
        private readonly IAccountService _accountService;
        private readonly ICRMLeadService _cRMLeadService;
        private readonly ICRMEmailJobService _cRMEmailJobService;

        public RetailersOverviewController(IRetailersOverviewViewModelAdapter viewModelAdapter, ICRMRestSearchResponseLogService cRMRestSearchResponseLogService, IAccountService accountService, ICRMLeadService cRMLeadService, ICRMEmailJobService cRMEmailJobService)
        {
            _viewModelAdapter = viewModelAdapter;
            _cRMRestSearchResponseLogService = cRMRestSearchResponseLogService;
            _accountService = accountService;
            _cRMLeadService = cRMLeadService;
            _cRMEmailJobService = cRMEmailJobService;
        }

        public ActionResult ViewPage(RetailersOverviewWebQuery query)
        {
            DateTime tempEnd = System.DateTime.Now;
            DateTime tempStart = tempEnd.AddDays(-14);

            DateTime startDate = query.StartDate != DateTime.MinValue ? query.StartDate : new DateTime(tempStart.Year, tempStart.Month, tempStart.Day);
            DateTime endDate = query.EndDate != DateTime.MinValue ? query.EndDate : new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day);

            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
            // Get data...
            List<string> accountObjectIDs = new List<string>();
            accountObjectIDs.Add(User.MxtrAccountObjectID);
            List<string> childAccountObjectIDs = _accountService.GetFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
            accountObjectIDs.AddRange(childAccountObjectIDs);

            List<mxtrAccount> accounts = _accountService.GetAccountsByAccountObjectIDs(accountObjectIDs);

            List<CRMLeadSummaryDataModel> leads = _cRMLeadService.GetLeadCountsByDate(accountObjectIDs, startDate, endDate);

            List<CRMRestSearchResponseLogModel> searchResponses =
                _cRMRestSearchResponseLogService.GetSearchResponseSummariesByDate(accountObjectIDs, startDate, endDate);

            List<CRMEmailJobDataModel> emails =
                _cRMEmailJobService.GetEmailJobsGroupedByDate(accountObjectIDs, startDate, endDate);

            // Adapt data...
            RetailersOverviewViewModel model =
                _viewModelAdapter.BuildRetailersOverviewViewModel(accounts, searchResponses, leads, emails, startDate, endDate, accountObjectIDs);

            // Handle...
            return View(ViewKind.RetailersOverview, model, query);
        }
    }
}
