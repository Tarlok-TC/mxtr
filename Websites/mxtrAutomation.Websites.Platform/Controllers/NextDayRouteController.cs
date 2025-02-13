using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.NextDayRoute.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.DataModels;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Api.Services;
using mxtrAutomation.Corporate.Data.Enums;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using mxtrAutomation.Api.EZShred;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class NextDayRouteController : MainLayoutControllerBase
    {
        private readonly INextDayRouteViewModelAdapter _viewModelAdapter;
        private readonly IAccountService _accountService;
        private readonly IEZShredService _apiEZShredService;
        private readonly INextDayRouteLogsServices _dbNextDayRouteLogsService;
        private readonly ICRMEZShredBuildingService _dbEZShredBuildingService;
        public NextDayRouteController(INextDayRouteViewModelAdapter viewModelAdapter, IAccountService accountService, IEZShredService apiEZShredService, INextDayRouteLogsServices dbNextDayRouteLogsService, ICRMEZShredBuildingService dbEZShredBuildingService)
        {
            _viewModelAdapter = viewModelAdapter;
            _accountService = accountService;
            _apiEZShredService = apiEZShredService;
            _dbNextDayRouteLogsService = dbNextDayRouteLogsService;
            _dbEZShredBuildingService = dbEZShredBuildingService;
        }
        public NextDayRouteController()
        {
        }
        public ActionResult ViewPage(NextDayRouteWebQuery query)
        {
            NextDayRouteViewModel model = _viewModelAdapter.BuildNextDayRouteViewModel();

            mxtrUser mxtrUser = UserService.GetUserByUserObjectID(User.MxtrUserObjectID);
            mxtrAccount mxtrAccount = AccountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);
            List<EZShredAccountDataModel> EZShredMarket = new List<EZShredAccountDataModel>();


            if (mxtrUser.EZShredAccountMappings != null)
            {
                EZShredMarket = AccountService.GetEzshredByAccountObjectIds(mxtrUser.EZShredAccountMappings.Select(a => a.AccountObjectId).ToList(), mxtrUser);
                model.EZShredAccountDataModel = EZShredMarket;
            }
            return View(ViewKind.NextDayRoute, model, query);
        }
        public ActionResult GetNextDayRouteTicket(GetNextDayRouteTicketWebQuery query)
        {
            try
            {
                string nextDate = DateTime.Now.AddDays(1).ToString();
                mxtrUser mxtrUser = UserService.GetUserByUserObjectID(User.MxtrUserObjectID);
                mxtrAccount mxtrAccount = AccountService.GetAccountByAccountObjectID(query.AccountObjectId);
                _apiEZShredService.SetConnectionTokens(mxtrAccount.EZShredIP, mxtrAccount.EZShredPort);
                string responseText = _apiEZShredService.GetNextDayRoute(Convert.ToInt32(query.EzshredUserID), nextDate);

                EZShredResponse objTicketsForTheDayResponse = null;
                TicketsForTheDayInfo objTicketsForTheDayInfo = null;
                if (!string.IsNullOrEmpty(responseText))
                {
                    objTicketsForTheDayResponse = JsonConvert.DeserializeObject<EZShredResponse>(responseText);
                    objTicketsForTheDayInfo = JsonConvert.DeserializeObject<TicketsForTheDayInfo>(objTicketsForTheDayResponse.Result);
                    if (objTicketsForTheDayInfo.status == "OK")
                    {
                        NextDayRouteLogsDataModel ObjNextDayRouteLogsDataModel = new NextDayRouteLogsDataModel()
                        {
                            AccountObjectID = mxtrAccount.ObjectID,
                            MxtrAccountID = mxtrAccount.MxtrAccountID,
                            UserID = mxtrUser.ObjectID,
                            MxtrUserID = mxtrUser.MxtrUserID,
                            LocationName = query.LocationName,
                            APIResponse = responseText,
                            TicketGenerateFrom = NextDayRouteKind.Web.ToString(),
                            NextRunDate = nextDate
                        };
                        //NextDayRouteLogsDataModel NextDayRouteLogsDataModelResult = _dbNextDayRouteLogsService.AddNextDayRouteLogs(ObjNextDayRouteLogsDataModel);

                        foreach (var item in objTicketsForTheDayInfo.TicketsForTheDay)
                        {
                            item.CustomerID = _dbEZShredBuildingService.GetCustomerByBuildingId(query.AccountObjectId, item.BuildingID).ToString();

                        }
                        return Json(new { Success = true, Message = objTicketsForTheDayInfo.status, Data = objTicketsForTheDayInfo.TicketsForTheDay.ToList() });
                    }
                    else
                        return Json(new { Success = false, Message = objTicketsForTheDayInfo.status });

                }
                else
                    return Json(new { Success = false, Message = "Unknown Request" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
        public ActionResult CheckNextDayRouteTicket(CheckNextDayRouteTicketWebQuery query)
        {
            try
            {
                NextDayRouteLogsDataModel NextDayRouteLogsDataModelResult = _dbNextDayRouteLogsService.CheckNextDayRouteTicketByAccountID(query.AccountObjectId);
                if (NextDayRouteLogsDataModelResult.Id != null)
                    return Json(new { Success = true, Data = NextDayRouteLogsDataModelResult });
                else
                    return Json(new { Success = false, Data = string.Empty });

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
    }
}