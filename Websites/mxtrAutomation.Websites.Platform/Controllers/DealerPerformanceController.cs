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

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class DealerPerformanceController : MainLayoutControllerBase
    {
        private readonly IDealerPerformanceViewModelAdapter _viewModelAdapter;
        private readonly IUserService _userService;
        private readonly ICRMLeadService _leadService;
        private readonly IShawLeadDetailService _dbShawLeadDetailService;
        private readonly IShawListBasedLeadService _dbIShawListBasedLeadService;

        public DealerPerformanceController(IDealerPerformanceViewModelAdapter viewModelAdapter, IUserService userService, ICRMLeadService leadService, IShawLeadDetailService dbShawLeadDetailService, IShawListBasedLeadService dbIShawListBasedLeadService)
        {
            _viewModelAdapter = viewModelAdapter;
            _userService = userService;
            _leadService = leadService;
            _dbShawLeadDetailService = dbShawLeadDetailService;
            _dbIShawListBasedLeadService = dbIShawListBasedLeadService;
        }


        public ActionResult ViewPage(DealerPerformanceWebQuery query)
        {

            DateTime tempEnd = System.DateTime.Now;
            DateTime tempStart = tempEnd.AddDays(-30);

            DateTime startDate = query.StartDate != DateTime.MinValue ? query.StartDate : new DateTime(tempStart.Year, tempStart.Month, tempStart.Day);
            DateTime endDate = query.EndDate != DateTime.MinValue ? query.EndDate : new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day);
           // List<mxtrAccount> accounts = AccountService.GetFlattenedChildAccounts(User.MxtrAccountObjectID);
            List<mxtrAccount> accounts = AccountService.GetFlattenedChildAccountsCoordinates(User.MxtrAccountObjectID);
            List<string> accountIds = accounts.Select(x => x.ObjectID).ToList(); //AccountService.GetFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
            accountIds.Remove(User.MxtrAccountObjectID);

            //-----------------------
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
                    accountObjectIDs.AddRange(accountIds);
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
            }

            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
            //--------------------
            accounts = accounts.Where(x => accountObjectIDs.Contains(x.ObjectID)).ToList();
            DealerPerformanceViewModel model = _viewModelAdapter.BuildDealerPerformanceViewModel(startDate, endDate, accounts, accountObjectIDs, _leadService, _dbShawLeadDetailService);
            Session["DealerPerformanceData"] = model.DealerData;
            model.DealerData = null;
            ViewBag.DataTableIdentifier = "DealerPerformanceData";

            // Handle...
            if (query.IsAjax)
            {
                if (!string.IsNullOrWhiteSpace(query.AccountObjectIDs))
                    Session["workspacesAccountIdsCache"] = query.AccountObjectIDs;
                else
                    Session["workspacesAccountIdsCache"] = "none";

                model.Success = true;
                return Json(model);
                //return Json(new { Success = model.Success, LeadsCountInDealerFunnel = model.LeadsCountInDealerFunnel, AverageLeadTimeDealer = model.AverageLeadTimeDealer, ConversionRateDealer = model.ConversionRateDealer });
            }

            return View(ViewKind.DealerPerformance, model, query);
        }
    }
}
