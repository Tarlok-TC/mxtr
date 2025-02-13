using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.Index.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Common.Extensions;
using System;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewData;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewData;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using System.Globalization;
using mxtrAutomation.Websites.Platform.Models.ManageMenu.ViewModels;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class IndexController : MainLayoutControllerBase
    {
        private readonly IIndexViewModelAdapter _viewModelAdapter;
        private readonly IAccountService _accountService;
        private readonly ICRMLeadService _cRMLeadService;

        public IndexController(IIndexViewModelAdapter viewModelAdapter, IAccountService accountService, ICRMLeadService cRMLeadService)
        {
            _viewModelAdapter = viewModelAdapter;
            _accountService = accountService;
            _cRMLeadService = cRMLeadService;
        }
        public ActionResult ViewPage(IndexWebQuery query)
        {

            //string subDomain = Request.Url.AbsoluteUri.ToLower();
            //if (subDomain.Contains("proshred"))
            //{
            //    return Redirect(new ProshredHomeWebQuery { });
            //}
            // Get data...

            // set startdate & enddate set when no date filter apply
            DateTime tempEnd = System.DateTime.Now;
            DateTime tempStart = tempEnd.AddDays(-30);

            // get startdate & enddate set when date filter apply
            DateTime startDate = query.StartDate != DateTime.MinValue ? query.StartDate : new DateTime(tempStart.Year, tempStart.Month, tempStart.Day);
            DateTime endDate = query.EndDate != DateTime.MinValue ? query.EndDate : new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day);

            // Get accountobjectids by root parent accountid i.e logged in...
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
                    // Get logged in user accountObjectId
                    accountObjectIDs.Add(User.MxtrAccountObjectID);
                    // Get all child account objectids by  logged in user accountObjectId
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
                    // Set Session none when no selection from workspace filter
                    Session["workspacesAccountIdsCache"] = "none";
            }

            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            // get account data as per accountobjectids
            List<mxtrAccount> accounts = _accountService.GetAccountsByAccountObjectIDs(accountObjectIDs);
            List<GroupLeadsDataModel> groupLead = new List<GroupLeadsDataModel>();
            // get grouplead as per accounts group by stateName
            groupLead = _cRMLeadService.GetGroupLeads(accountObjectIDs, accounts, startDate, endDate);
            // Adapt data...
            IndexViewModel model = _viewModelAdapter.BuildIndexViewModel(accounts, startDate, endDate, accountObjectIDs, groupLead);
            // get total no of retailers by accountobjectids from account table
            model.TotalRetailers = _accountService.GetTotalRetailers(accountObjectIDs);
            // get total no of TotalLeads by accountobjectids from CRMLead table
            model.TotalLeads = _cRMLeadService.GetTotalLeads(accountObjectIDs, startDate, endDate);
            // calculate averagelead= totalleads/totalretailers
            if (model.TotalRetailers != 0 && model.TotalLeads != 0)
            {
                model.AverageLead = Math.Round(Convert.ToDecimal(model.TotalLeads) / Convert.ToDecimal(model.TotalRetailers), 2);
            }
            // Handle...
            if (query.IsAjax)
            {
                if (!string.IsNullOrWhiteSpace(query.AccountObjectIDs))
                    Session["workspacesAccountIdsCache"] = query.AccountObjectIDs;
                else
                    // Set Session none when no selection from workspace filter
                    Session["workspacesAccountIdsCache"] = "none";
                model.Success = true;
                return Json(model);
            }
            return View(ViewKind.Index, model, query);
        }


        /// <summary>
        /// Set SetworkspacesAccountIdsCache session when select data on map view 
        /// </summary>
        /// <param name="accountObjectId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SetworkspacesAccountIdsCache")]
        public ActionResult SetworkspacesAccountIdsCache(string accountObjectId)
        {
            Session["workspacesAccountIdsCache"] = accountObjectId;
            return Json(new
            {
                Success = true
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("GetTableData")]
        public ActionResult GetTableData(int draw, int start, int length)
        {
            dynamic tableData = (dynamic)Session[Request.Form["DataTableIdentifier"]];

            string statusfilter = Request.Form["statusfilter"];
            if (!String.IsNullOrEmpty(statusfilter) && Convert.ToInt16(statusfilter) > 0)
            {
                if (statusfilter == "2")
                {
                    // Show inactive records
                    tableData = ((System.Collections.IEnumerable)tableData).Cast<dynamic>()
                            .Where(p => p.IsActive == false);
                }
                else
                {
                    // Show active records
                    tableData = ((System.Collections.IEnumerable)tableData).Cast<dynamic>()
                           .Where(p => p.IsActive);
                }
            }

            List<dynamic> lstRecords = new List<dynamic>();

            if (tableData != null)
            {
                foreach (var item in tableData)
                {
                    lstRecords.Add(item);
                }
            }

            var myRegex = new System.Text.RegularExpressions.Regex(@"columns\[\d+\]\[data\]");
            var matches = Request.Form.AllKeys.Where(m => myRegex.IsMatch(m)).ToList();

            var ColumnNames = new List<string>();

            foreach (var column in matches)
            {
                ColumnNames.Add(Request.Form[column]);
            }

            string search = Request.Form["search[value]"];
            string SearchColumnName = "", OrderColumnName = "";
            SearchColumnName = Request.Form["columns[" + 0 + "][data]"];
            int sortColumn = -1;

            string sortDirection = "asc";
            if (length == -1)
            {
                length = lstRecords.Count;
            }

            // note: we only sort one column at a time
            if (Request.Form["order[0][column]"] != null)
            {
                sortColumn = int.Parse(Request.Form["order[0][column]"]);
                OrderColumnName = Request.Form["columns[" + sortColumn + "][data]"];
            }

            if (Request.Form["order[0][dir]"] != null)
            {
                sortDirection = Request.Form["order[0][dir]"];
            }

            DataTableData dataTableData = new DataTableData();
            dataTableData.draw = draw;
            dataTableData.recordsTotal = lstRecords.Count;

            int SearchInteger = -1;
            if (search == "0")
            {
                SearchInteger = 0;
            }
            else
            {
                int.TryParse(search, out SearchInteger);
                if (SearchInteger == 0)
                    SearchInteger = -1;
            }

            int recordsFiltered = 0;

            var MainQuery = lstRecords.AsEnumerable();

            MainQuery = MainQuery.Where(m =>
            {
                var IsMatch = false;
                foreach (var ColumnName in ColumnNames)
                {
                    if (m.GetType().GetProperty(ColumnName).GetValue(m, null) != null && m.GetType().GetProperty(ColumnName).GetValue(m, null).ToString().ToLower().Contains((search.ToLower())))
                    {
                        IsMatch = true;
                        break;
                    }
                }
                return IsMatch;
            }).ToList();

            if (search != "")
                recordsFiltered = MainQuery.Count();
            else
                recordsFiltered = dataTableData.recordsTotal;



            var IsColumnDataIntegerType = false;
            var IsColumnDataDoubleType = false;
            var IsColumnDataDateTimeType = false;
            double OurCather = -1D;
            DateTime tempDate = DateTime.MinValue;

            if (lstRecords.Count > 0)
            {
                IsColumnDataIntegerType = int.TryParse(lstRecords[0].GetType().GetProperty(OrderColumnName).GetValue(lstRecords[0], null).ToString().ToLower(), out SearchInteger);
                IsColumnDataDoubleType = double.TryParse(lstRecords[0].GetType().GetProperty(OrderColumnName).GetValue(lstRecords[0], null).ToString().ToLower(), out OurCather);
                if (IsColumnDataDoubleType)
                {
                    IsColumnDataIntegerType = false;
                }

                IsColumnDataDateTimeType = DateTime.TryParse(lstRecords[0].GetType().GetProperty(OrderColumnName).GetValue(lstRecords[0], null).ToString().ToLower(), out tempDate);
                if (IsColumnDataDateTimeType)
                {
                    IsColumnDataIntegerType = false;
                    IsColumnDataDoubleType = false;
                }
            }

            Func<dynamic, dynamic> ComparePredicate = delegate (dynamic m)
            {
                try
                {
                    if (IsColumnDataDateTimeType)
                    {
                        return DateTime.Parse(m.GetType().GetProperty(OrderColumnName).GetValue(m, null).ToString().ToLower());
                    }
                    else if (IsColumnDataDoubleType)
                    {
                        return double.Parse(m.GetType().GetProperty(OrderColumnName).GetValue(m, null).ToString().ToLower());
                    }
                    else if (IsColumnDataIntegerType)
                    {
                        return int.Parse(m.GetType().GetProperty(OrderColumnName).GetValue(m, null).ToString().ToLower());
                    }
                    else
                    {
                        return m.GetType().GetProperty(OrderColumnName).GetValue(m, null).ToString().ToLower();
                    }

                }
                catch (System.Exception)
                {
                    if (IsColumnDataDateTimeType)
                    {
                        return DateTime.MinValue;
                    }
                    else if (IsColumnDataDoubleType)
                    {
                        return -1.0D;
                    }
                    else if (IsColumnDataIntegerType)
                    {
                        return -1;
                    }
                    else
                    {
                        return "";
                    }
                }

            };

            List<dynamic> OrderedData = null;

            if (sortDirection == "asc")
            {
                OrderedData = MainQuery.OrderBy(m => ComparePredicate(m)).ToList();
            }
            else
            {
                OrderedData = MainQuery.OrderByDescending(m => ComparePredicate(m)).ToList();
            }

            OrderedData = OrderedData.Skip(start).Take(length).ToList();

            dataTableData.data = OrderedData;

            dataTableData.recordsFiltered = recordsFiltered;
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PageNotFound(PageNotFoundWebQuery query)
        {
            PageNotFoundViewModel model = new PageNotFoundViewModel();
            return View(ViewKind.PageNotFound, model, query);
        }
    }
}
