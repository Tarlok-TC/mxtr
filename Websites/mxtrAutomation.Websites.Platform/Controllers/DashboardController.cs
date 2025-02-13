using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Web.Security;
using mxtrAutomation.Websites.Platform.Models.Dashboard.ViewModels;
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
    public class DashboardController : MainLayoutControllerBase
    {
        private readonly IDashboardViewModelAdapter _viewModelAdapter;
        private readonly IDashboardSummaryService _dashboardSummaryService;
        private readonly IAccountService _accountService;
        private readonly ICRMLeadService _cRMLeadService;

        public DashboardController(IDashboardViewModelAdapter viewModelAdapter, IDashboardSummaryService dashboardSummaryService, IAccountService accountService, ICRMLeadService cRMLeadService)
        {
            _viewModelAdapter = viewModelAdapter;
            _dashboardSummaryService = dashboardSummaryService;
            _accountService = accountService;
            _cRMLeadService = cRMLeadService;
        }

        public ActionResult ViewPage(DashboardWebQuery query)
        {
            int lookback = Convert.ToInt32(ConfigManager.AppSettings["Lookback"]);

            DateTime tempEnd = System.DateTime.Now;
            DateTime tempStart = tempEnd.AddDays(-30);
            //DateTime tempStart = tempEnd.AddDays((lookback * -1));

            DateTime startDate = query.StartDate != DateTime.MinValue ? query.StartDate : new DateTime(tempStart.Year, tempStart.Month, tempStart.Day);
            DateTime endDate = query.EndDate != DateTime.MinValue ? query.EndDate : new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day);

            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
            //List<string> accountObjectIDs = new List<string>();
            //accountObjectIDs = !query.IsAjax ? accountObjectIDs.Add(User.MxtrAccountObjectID) : query.AccountObjectIDs.Split(',').ToList<string>();

            List<string> accountObjectIDs = new List<string>();
            if (!query.IsAjax)
            {
                accountObjectIDs.Add(User.MxtrAccountObjectID);
                List<string> childAccountObjectIDs = _accountService.GetFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
                accountObjectIDs.AddRange(childAccountObjectIDs);
            }
            else
            {
                if (!string.IsNullOrEmpty(query.AccountObjectIDs))
                    accountObjectIDs = query.AccountObjectIDs.Split(',').ToList<string>();
            }

            // Get data...
            List<mxtrAccount> accounts = _accountService.GetAccountsByAccountObjectIDs(accountObjectIDs);
            List<CRMLeadDataModel> leads = _cRMLeadService.GetLeadsByAccountObjectIDsForDateRange(accountObjectIDs, startDate, endDate);

            List<DashboardSummaryDataModel> dashboardSummaries = null; // _dashboardSummaryService.GetDashboardSummaries(accountObjectIDs, startDate, endDate);

            // Adapt data...
            DashboardViewModel model = _viewModelAdapter.BuildDashboardViewModel(dashboardSummaries, leads, startDate, endDate, accountObjectIDs, accounts);

            // Handle...
            if (!query.IsAjax)
            {
                return View(ViewKind.Dashboard, model, query);
            }

            var result = new { Data = model };
            return Json(result);
        }
    }
}
