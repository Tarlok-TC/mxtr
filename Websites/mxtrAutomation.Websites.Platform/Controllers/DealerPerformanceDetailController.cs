using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Dealer.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Dealer.ViewData;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class DealerPerformanceDetailController : MainLayoutControllerBase
    {
        private readonly IDealerPerformanceDetailViewModelAdapter _viewModelAdapter;
        //private readonly IUserService _userService;
        private readonly ICRMLeadService _leadService;
        private readonly IShawLeadDetailService _dbShawLeadDetailService;
        //private readonly IShawListBasedLeadService _dbIShawListBasedLeadService;

        public DealerPerformanceDetailController(IDealerPerformanceDetailViewModelAdapter viewModelAdapter, ICRMLeadService leadService, IShawLeadDetailService dbShawLeadDetailService)
        {
            _viewModelAdapter = viewModelAdapter;
            _leadService = leadService;
            _dbShawLeadDetailService = dbShawLeadDetailService;
        }


        public ActionResult ViewPage(DealerPerformanceDetailWebQuery query)
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
                if (!string.IsNullOrEmpty(query.ObjectID))
                    accountObjectIDs = query.ObjectID.Split(',').ToList<string>();
                //if (!string.IsNullOrEmpty(query.AccountObjectIDs))
                //    accountObjectIDs = query.AccountObjectIDs.Split(',').ToList<string>();
            }

            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
            //accountObjectIDs.Add(query.ObjectID);
            List<mxtrAccount> accounts = AccountService.GetAllAccountsByAccountObjectIDs(accountObjectIDs);
            DealerPerformanceDetailViewModel model = _viewModelAdapter.BuildDealerPerformanceDetailViewModel(accounts.FirstOrDefault().AccountName, startDate, endDate, accountObjectIDs, _leadService, _dbShawLeadDetailService);
            // Handle...
            if (query.IsAjax)
            {
                return Json(new { Success = true, DealerDetail = model.DealerDetail, DealerLeads = model.DealerLeads });
            }
            return View(ViewKind.DealerPerformanceDetail, model, query);
        }
    }
}
