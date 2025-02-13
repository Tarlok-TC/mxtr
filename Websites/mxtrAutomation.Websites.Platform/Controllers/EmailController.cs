using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using mxtrAutomation.Websites.Platform.Models.Email.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class EmailController : MainLayoutControllerBase
    {
        private readonly IEmailViewModelAdapter _viewModelAdapter;
        private readonly IAccountService _accountService;
        private readonly ICRMEmailService _cRMEmailService;
        private readonly ICRMEmailJobService _cRMEmailJobService;

        public EmailController(IEmailViewModelAdapter viewModelAdapter, IAccountService accountService, ICRMEmailService cRMEmailService, ICRMEmailJobService cRMEmailJobService)
        {
            _viewModelAdapter = viewModelAdapter;
            _accountService = accountService;
            _cRMEmailService = cRMEmailService;
            _cRMEmailJobService = cRMEmailJobService;
        }

        public ActionResult ViewPage(EmailWebQuery query)
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
                if (!string.IsNullOrEmpty(query.AccountObjectIDs))
                    accountObjectIDs = query.AccountObjectIDs.Split(',').ToList<string>();
                else
                    Session["workspacesAccountIdsCache"] = "none";
            }

            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            IEnumerable<mxtrAccount> accounts = _accountService.GetAccountsByAccountObjectIDsAsIEnumberable(accountObjectIDs);

            
            IEnumerable<CRMEmailJobDataModel> emailJobs =
              _cRMEmailJobService.GetEmailJobsByAccountObjectIDsForDateRangeAsEnumerable(accountObjectIDs, startDate, endDate);
            
            // Adapt data...
            EmailViewModel model = _viewModelAdapter.BuildEmailViewModel(accounts, emailJobs,startDate, endDate, accountObjectIDs);
            Session["EmailsData"] = model.EmailActivityViewData;
            model.EmailActivityViewData = null;            
            
            ViewBag.DataTableIdentifier = "EmailsData";
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

            return View(ViewKind.Email, model, query);
        }
    }
}
