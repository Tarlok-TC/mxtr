using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.ProshredHome.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Services;
using System.Globalization;
using mxtrAutomation.Websites.Platform.Models.ShawHome.ViewModels;
using mxtrAutomation.Api.Services;
using mxtrAutomation.Api.Sharpspring;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class ShawHomeController : MainLayoutControllerBase
    {
        private readonly IShawHomeViewModelAdapter _viewModelAdapter;
        private readonly IUserService _userService;
        private readonly ISharpspringService _apiSharpspringService;
        private readonly ICRMLeadService _leadService;
        private readonly IShawLeadDetailService _dbShawLeadDetailService;
        private readonly IShawListBasedLeadService _dbIShawListBasedLeadService;

        public ShawHomeController(IShawHomeViewModelAdapter viewModelAdapter, IUserService userService, ISharpspringService apiSharpspringService, ICRMLeadService leadService, IShawLeadDetailService dbShawLeadDetailService, IShawListBasedLeadService dbIShawListBasedLeadService)
        {
            _viewModelAdapter = viewModelAdapter;
            _userService = userService;
            _apiSharpspringService = apiSharpspringService;
            _leadService = leadService;
            _dbShawLeadDetailService = dbShawLeadDetailService;
            _dbIShawListBasedLeadService = dbIShawListBasedLeadService;
        }
        public ActionResult ViewPage(ShawHomeWebQuery query)
        {
            DateTime tempEnd = System.DateTime.Now;
            DateTime tempStart = tempEnd.AddDays(-30);

            DateTime startDate = query.StartDate != DateTime.MinValue ? query.StartDate : new DateTime(tempStart.Year, tempStart.Month, tempStart.Day);
            DateTime endDate = query.EndDate != DateTime.MinValue ? query.EndDate : new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day);


            //mxtrUser user = _userService.GetUserByUserObjectID(User.MxtrUserObjectID);
            //mxtrAccount mxtrAccount = AccountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);
            //_apiSharpspringService.SetConnectionTokens("21F48BF118F4B564A580C98392F04BFF", "3_E29E78FE002A55979FD656175DFD29BF");
            // SharpspringGetActiveListDataModel result = _apiSharpspringService.GetActiveLists("594999299");

            //-------List<string> accountIds = AccountService.GetFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
            //accountIds.Remove(User.MxtrAccountObjectID);

            //-----------------------
            List<string> accountObjectIDs = new List<string>();
            //accountObjectIDs = AccountService.GetFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
            //accountObjectIDs = AccountService.GetFlattenedChildAccountObjectIDsWithGroupClients(User.MxtrAccountObjectID);

            var globalDealerAccounts = AccountService.GetFlattenedChildAccountsCoordinates(User.MxtrAccountObjectID);
            TempData["globalDealerAccounts"] = globalDealerAccounts;
            accountObjectIDs = globalDealerAccounts.Select(x => x.ObjectID).ToList();

            if (!query.IsAjax)
            {
                //if (!string.IsNullOrEmpty(Convert.ToString(Session["workspacesAccountIdsCache"])))
                //{
                //    if ("none" != Convert.ToString(Session["workspacesAccountIdsCache"]))
                //        accountObjectIDs = Convert.ToString(Session["workspacesAccountIdsCache"]).Split(',').ToList<string>();
                //}
                //else if ("none" == Convert.ToString(Session["workspacesAccountIdsCache"]))
                //{

                //}
                //else
                //{
                //    accountObjectIDs.Add(User.MxtrAccountObjectID);
                //    accountObjectIDs.AddRange(accountIds);
                //}
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
                //if (!string.IsNullOrEmpty(query.AccountObjectIDs))
                //    accountObjectIDs = query.AccountObjectIDs.Split(',').ToList<string>();
                //else
                //    Session["workspacesAccountIdsCache"] = "none";
            }

            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
            //--------------------
            var dataShawListBasedLeadService = _dbIShawListBasedLeadService.GetShawListBasedData(User.MxtrAccountObjectID, startDate, endDate);
            var leadsData = _leadService.GetLeadsByAccountObjectIDsForDateRange(accountObjectIDs, null, null);
            List<long> leadIds = leadsData.Select(x => x.LeadID).ToList();
            var ShawLeadData = _leadService.GetLeadsByAccountObjectIDsForDateRange(new List<string> { User.MxtrAccountObjectID }, null, null);
            List<long> shawLeadIds = ShawLeadData.Select(x => x.LeadID).ToList();


            //ShawHomeViewModel model = _viewModelAdapter.BuildShawHomeViewModel(User.MxtrAccountObjectID, dataShawListBasedLeadService.MemberCount, startDate, endDate, accountObjectIDs, leadIds, shawLeadIds, _leadService, _dbShawLeadDetailService);
            ShawHomeViewModel model = _viewModelAdapter.BuildShawHomeViewModel(User.MxtrAccountObjectID, dataShawListBasedLeadService.MemberCount, startDate, endDate, accountObjectIDs, leadIds, shawLeadIds, _leadService, _dbShawLeadDetailService);
            model.StartDate = startDate.ToString("MM-dd-yyyy");
            model.EndDate = endDate.ToString("MM-dd-yyyy");

            if (query.IsAjax)
            {
                //if (!string.IsNullOrWhiteSpace(query.AccountObjectIDs))
                //    Session["workspacesAccountIdsCache"] = query.AccountObjectIDs;
                //else
                //    Session["workspacesAccountIdsCache"] = "none";

                return Json(new { Success = true, MemberCount = model.MemberCount, ParticipatingDealerCount = model.ParticipatingDealerCount, AverageLeadScore = model.AverageLeadScore, AveragePassToDealerDays = model.AveragePassToDealerDays, AverageCreateDateToSaleDate = model.AverageCreateDateToSaleDate, PassOffRate = model.PassOffRate, ConversionRate = model.ConversionRate, WonLeadCount = model.WonLeadCount, LeadScoreCount = model.LeadScoreCount, LeadScoreMin = model.LeadScoreMin, LeadScoreMax = model.LeadScoreMax, PassOffLeadCount = model.PassOffLeadCount, StartDate = startDate.ToString("MM-dd-yyyy"), EndDate = endDate.ToString("MM-dd-yyyy") });
            }

            return View(ViewKind.ShawHome, model, query);
        }

        public ActionResult GetDealerData(GetDealerDataWebQuery query)
        {
            try
            {
                DateTime tempEnd = System.DateTime.Now;
                DateTime tempStart = tempEnd.AddDays(-30);
                DateTime startDate = query.StartDate != DateTime.MinValue ? query.StartDate : new DateTime(tempStart.Year, tempStart.Month, tempStart.Day);
                DateTime endDate = query.EndDate != DateTime.MinValue ? query.EndDate : new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day);

                List<mxtrAccount> dealerAccounts = new List<mxtrAccount>();
                List<string> accountObjectIDs = new List<string>();
                //accountObjectIDs = AccountService.GetFlattenedChildAccountObjectIDs(User.MxtrAccountObjectID);
                //if (!string.IsNullOrEmpty(query.AccountObjectIDs))
                //if (query.IsSearchCall || !string.IsNullOrEmpty(query.AccountObjectIDs))
                //{
                //    if (!string.IsNullOrEmpty(query.AccountObjectIDs))
                //    {
                //        accountObjectIDs = query.AccountObjectIDs.Split(',').ToList<string>();
                //        accountObjectIDs.Remove(User.MxtrAccountObjectID);
                //        dealerAccounts = AccountService.GetAccountsCoordinates(accountObjectIDs);
                //        dealerAccounts = AccountService.GetAccountsCoordinates(accountObjectIDs);
                //    }
                //}
                //else
                //{
                //    dealerAccounts = AccountService.GetFlattenedChildAccountsCoordinates(User.MxtrAccountObjectID);
                //}

                if (TempData["globalDealerAccounts"] != null)
                {
                    dealerAccounts = (List<mxtrAccount>)TempData["globalDealerAccounts"];
                }

                if (dealerAccounts.Count == 0)
                {
                    dealerAccounts = AccountService.GetFlattenedChildAccountsCoordinates(User.MxtrAccountObjectID);
                }

                startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
                endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

                //dealerAccounts = dealerAccounts.Where(x => Convert.ToDateTime(x.CreateDate) >= startDate && Convert.ToDateTime(x.CreateDate) <= endDate).ToList();
                accountObjectIDs = dealerAccounts.Select(x => x.ObjectID).ToList();

                List<CRMLeadSummaryDataModel> lstLeadCount = _leadService.GetLeadCountsByAccountObjectIDs(accountObjectIDs, startDate, endDate);
                DealerData objDealerData = new DealerData();
                objDealerData.Dealer = new DealerDetails();
                objDealerData.Dealer.Coords = new List<List<double>>();
                objDealerData.Dealer.Names = new List<string>();
                objDealerData.Dealer.Cities = new List<string>();
                objDealerData.Dealer.LeadCount = new List<int>();
                var lstGroupData = AccountService.GetFlattenedChildAccounts(User.MxtrAccountObjectID).Where(x => x.AccountType == AccountKind.Group.ToString()).ToList();

                foreach (var item in dealerAccounts)
                {
                    List<double> lstCoordinates = new List<double>();
                    lstCoordinates.Add(item.Latitude);
                    lstCoordinates.Add(item.Longitude);
                    objDealerData.Dealer.Coords.Add(lstCoordinates);
                    objDealerData.Dealer.Names.Add(item.AccountName);
                    objDealerData.Dealer.Cities.Add(item.City);
                    var record = lstLeadCount.FirstOrDefault(w => w.AccountObjectID == item.ObjectID);
                    objDealerData.Dealer.LeadCount.Add(record == null ? 0 : Convert.ToInt32(record.LeadCount));

                    var data = lstGroupData.Where(x => x.ObjectID == item.ParentAccountObjectID).FirstOrDefault();
                    if (data != null && data.AccountName.ToUpper().Trim() == "SOUTHEAST")
                    {
                        objDealerData.SouthEastLeads += (record == null ? 0 : Convert.ToInt32(record.LeadCount));
                    }
                    else if (data != null && data.AccountName.ToUpper().Trim() == "NORTH CENTRAL")
                    {
                        objDealerData.NorthCentralLeads += (record == null ? 0 : Convert.ToInt32(record.LeadCount));
                    }
                    else if (data != null && data.AccountName.ToUpper().Trim() == "SOUTH CENTRAL")
                    {
                        objDealerData.SouthCentralLeads += (record == null ? 0 : Convert.ToInt32(record.LeadCount));
                    }
                    else if (data != null && data.AccountName.ToUpper().Trim() == "WEST")
                    {
                        objDealerData.WestLeads += (record == null ? 0 : Convert.ToInt32(record.LeadCount));
                    }
                    else if (data != null && data.AccountName.ToUpper().Trim() == "NORTHEAST")
                    {
                        objDealerData.NorthEastLeads += (record == null ? 0 : Convert.ToInt32(record.LeadCount));
                    }

                }

                // Get dealer count
                foreach (var item in lstGroupData)
                {
                    if (item.AccountName.ToUpper().Trim() == "SOUTHEAST")
                    {
                        objDealerData.SouthEastDealers = dealerAccounts.Where(x => x.ParentAccountObjectID == item.ObjectID).Count();
                    }
                    else if (item.AccountName.ToUpper().Trim() == "NORTH CENTRAL")
                    {
                        objDealerData.NorthCentralDealers = dealerAccounts.Where(x => x.ParentAccountObjectID == item.ObjectID).Count();
                    }
                    else if (item.AccountName.ToUpper().Trim() == "SOUTH CENTRAL")
                    {
                        objDealerData.SouthCentralDealers = dealerAccounts.Where(x => x.ParentAccountObjectID == item.ObjectID).Count();
                    }
                    else if (item.AccountName.ToUpper().Trim() == "WEST")
                    {
                        objDealerData.WestDealers = dealerAccounts.Where(x => x.ParentAccountObjectID == item.ObjectID).Count();
                    }
                    else if (item.AccountName.ToUpper().Trim() == "NORTHEAST")
                    {
                        objDealerData.NorthEastDealers = dealerAccounts.Where(x => x.ParentAccountObjectID == item.ObjectID).Count();
                    }
                }

                //Get Participating dealer data for last 90 days
                var ParticipatingDealerChartdata = AccountService.GetParticipatingdealerdata(User.MxtrAccountObjectID, endDate, 90);


                List<string> dates = new List<string>();
                List<int> dealerCount = new List<int>();

                int count = 0;
                foreach (var item in ParticipatingDealerChartdata)
                {
                    dates.Add(item.Item1.ToString("MM-dd-yyyy"));
                    count += item.Item2;
                    dealerCount.Add(count);                    
                }

                objDealerData.ParticipatingDealerChartData = new ParticipatingDealerChartData()
                {
                    CreatedDate = dates,
                    DealerCount = dealerCount
                };

                accountObjectIDs.Remove(User.MxtrAccountObjectID);
                return Json(new { Success = true, Data = objDealerData, ParticipatingDealerCount = accountObjectIDs == null ? "0" : accountObjectIDs.Count.ToString() });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Data = ex.Message });
            }
        }

        public ActionResult GetLeadsChartData(GetLeadsChartDataWebQuery query)
        {
            DateTime tempEnd = DateTime.Now;
            DateTime tempStart = tempEnd.AddDays(-30);
            DateTime startDate = query.StartDate != DateTime.MinValue ? query.StartDate : new DateTime(tempStart.Year, tempStart.Month, tempStart.Day);
            DateTime endDate = query.EndDate != DateTime.MinValue ? query.EndDate : new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day);
            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
            //------------------
            //List<mxtrAccount> dealerAccounts = new List<mxtrAccount>();
            //List<string> accountObjectIDs = new List<string>();
            ////if (!string.IsNullOrEmpty(query.AccountObjectIDs))
            //if (query.IsSearchCall || !string.IsNullOrEmpty(query.AccountObjectIDs))
            //{
            //    if (!string.IsNullOrEmpty(query.AccountObjectIDs))
            //    {
            //        accountObjectIDs = query.AccountObjectIDs.Split(',').ToList<string>();
            //        dealerAccounts = AccountService.GetAccountsCoordinates(accountObjectIDs);
            //    }
            //}
            //else
            //{
            //    //dealerAccounts = AccountService.GetFlattenedChildAccountsCoordinates(User.MxtrAccountObjectID);
            //}
            //-------------
            //dealerAccounts = dealerAccounts.Where(x => Convert.ToDateTime(x.CreateDate) >= startDate && Convert.ToDateTime(x.CreateDate) <= endDate).ToList();
            //accountObjectIDs = dealerAccounts.Select(x => x.ObjectID).ToList();

            List<Tuple<DateTime, int, int, int>> leadCount = _dbShawLeadDetailService.GetColdHotWarmLeadCount(new List<string> { User.MxtrAccountObjectID }, startDate, endDate);
            List<string> dates = new List<string>();
            List<string> cold = new List<string>();
            List<string> warm = new List<string>();
            List<string> hot = new List<string>();
            leadCount.Reverse();
            foreach (var item in leadCount)
            {
                dates.Add(item.Item1.ToString("MM-dd-yyyy"));
                cold.Add(item.Item2.ToString());
                warm.Add(item.Item3.ToString());
                hot.Add(item.Item4.ToString());
            }
            //dates.Reverse();
            //cold.Reverse();
            //warm.Reverse();
            //hot.Reverse();
            return Json(new { Success = true, Message = "", Dates = dates, Cold = cold, Warm = warm, Hot = hot });
        }

    }
}