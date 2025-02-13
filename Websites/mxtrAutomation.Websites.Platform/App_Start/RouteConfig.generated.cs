using System.Web.Routing;
using mxtrAutomation.Web.Common.Extensions;
using mxtrAutomation.Websites.Platform.Controllers;
using mxtrAutomation.Websites.Platform.Queries;

namespace mxtrAutomation.Websites.Platform.App_Start
{
    public partial class RouteConfig
    {
        static partial void RegisterViewRoutes(RouteCollection routes)
        {
            routes.MapWebQueryRoute<LoginWebQuery, LoginController>(c => c.ViewPage);
            routes.MapWebQueryRoute<AdminAddAccountWebQuery, AdminAddAccountController>(c => c.ViewPage);
            routes.MapWebQueryRoute<AdminAccountUserManagementWebQuery, AdminAccountUserManagementController>(c => c.ViewPage);
            routes.MapWebQueryRoute<AdminEditAccountWebQuery, AdminEditAccountController>(c => c.ViewPage);
            routes.MapWebQueryRoute<DashboardWebQuery, DashboardController>(c => c.ViewPage);
            routes.MapWebQueryRoute<LeadsWebQuery, LeadsController>(c => c.ViewPage);
            routes.MapWebQueryRoute<LeadWebQuery, LeadController>(c => c.ViewPage);
            routes.MapWebQueryRoute<ActivityWebQuery, ActivityController>(c => c.ViewPage);
            routes.MapWebQueryRoute<CampaingWebQuery, CampaingController>(c => c.ViewPage);
            routes.MapWebQueryRoute<RetailersWebQuery, RetailersController>(c => c.ViewPage);
            routes.MapWebQueryRoute<ContactsWebQuery, ContactsController>(c => c.ViewPage);
            routes.MapWebQueryRoute<ContactWebQuery, ContactController>(c => c.ViewPage);
            routes.MapWebQueryRoute<EmailWebQuery, EmailController>(c => c.ViewPage);
            routes.MapWebQueryRoute<IndexWebQuery, IndexController>(c => c.ViewPage);
            routes.MapWebQueryRoute<CreativeWebQuery, CreativeController>(c => c.ViewPage);
            routes.MapWebQueryRoute<DMAPerformanceWebQuery, DMAPerformanceController>(c => c.ViewPage);
            routes.MapWebQueryRoute<DetailedWebQuery, DetailedController>(c => c.ViewPage);
            routes.MapWebQueryRoute<RegionPerformanceWebQuery, RegionPerformanceController>(c => c.ViewPage);
            routes.MapWebQueryRoute<RetailersOverviewWebQuery, RetailersOverviewController>(c => c.ViewPage);
            routes.MapWebQueryRoute<RetailerWebQuery, RetailerController>(c => c.ViewPage);
            routes.MapWebQueryRoute<ProfileWebQuery, ProfileController>(c => c.ViewPage);
            routes.MapWebQueryRoute<ProfileEditSubmitWebQuery, ProfileController>(c => c.EditProfileSubmit);
            routes.MapWebQueryRoute<LeadCopyWebQuery, LeadController>(c => c.CopyLead);
            routes.MapWebQueryRoute<LeadDeleteWebQuery, LeadController>(c => c.DeleteLead);
            routes.MapWebQueryRoute<ManageMinerWebQuery, ManageMinerController>(c => c.ViewPage);
            routes.MapWebQueryRoute<ManageMinerDeleteSharpspringLogWebQuery, ManageMinerController>(c => c.DeleteAuditTraillog);
            routes.MapWebQueryRoute<AdminDeleteAccountUserWebQuery, AdminAddAccountController>(c => c.DeleteUser);
            routes.MapWebQueryRoute<CustomizeMenuWebQuery, CustomizeMenuController>(c => c.ViewPage);
            routes.MapWebQueryRoute<CustomizeMenuSubmitWebQuery, CustomizeMenuController>(c => c.UpdateMenuData);
            routes.MapWebQueryRoute<ResetMenuWebQuery, CustomizeMenuController>(c => c.ResetMenuData);
            routes.MapWebQueryRoute<WhiteLabelingWebQuery, WhiteLabelingController>(c => c.ViewPage);
            routes.MapWebQueryRoute<ManageMenuWebQuery, ManageMenuController>(c => c.ViewPage);
            routes.MapWebQueryRoute<WhiteLabelingManageDomainWebQuery, WhiteLabelingController>(c => c.AddUpdateDomain);
            routes.MapWebQueryRoute<WhiteLabelingManageHomePageWebQuery, WhiteLabelingController>(c => c.AddUpdateHomePageUrl);
            routes.MapWebQueryRoute<AssignDomainWebQuery, ManageMinerController>(c => c.SetDomainName);
            routes.MapWebQueryRoute<AddEditMenuWebQuery, ManageMenuController>(c => c.AddEditMenuData);
            routes.MapWebQueryRoute<DeleteMenuWebQuery, ManageMenuController>(c => c.DeleteMenuData);
            routes.MapWebQueryRoute<SharpspringWebQuery, SharpspringController>(c => c.ViewPage);
            routes.MapWebQueryRoute<CustomerWebQuery, CustomerController>(c => c.ViewPage);
            routes.MapWebQueryRoute<KlipfolioWebQuery, KlipfolioController>(c => c.ViewPage);
            routes.MapWebQueryRoute<ProshredHomeWebQuery, ProshredHomeController>(c => c.ViewPage);
            routes.MapWebQueryRoute<PageNotFoundWebQuery, IndexController>(c => c.PageNotFound);
            routes.MapWebQueryRoute<InsertUpdateCustomerWebQuery, ManageMinerController>(c => c.InsertUpdateCustomers);
            routes.MapWebQueryRoute<InsertUpdateBuildingWebQuery, ManageMinerController>(c => c.InsertUpdateBuildings);
            routes.MapWebQueryRoute<AddUpdateCustomerDataFromJsonWebQuery, ManageMinerController>(c => c.AddUpdateCustomerDataFromJson);
            routes.MapWebQueryRoute<AddUpdateBuildingDataFromJsonWebQuery, ManageMinerController>(c => c.AddUpdateBuildingDataFromJson);
            routes.MapWebQueryRoute<DeleteDuplicateCustomerWebQuery, ManageMinerController>(c => c.DeleteDuplicateCustomer);
            routes.MapWebQueryRoute<DeleteDuplicateBuildingWebQuery, ManageMinerController>(c => c.DeleteDuplicateBuilding);
            routes.MapWebQueryRoute<HandleOldBuildingDataWebQuery, ManageMinerController>(c => c.HandleOldBuildingData);
            routes.MapWebQueryRoute<SetOpportunityPipeLineWebQuery, ManageMinerController>(c => c.SetOpportunityPipeLines);
            routes.MapWebQueryRoute<ShawHomeWebQuery, ShawHomeController>(c => c.ViewPage);
            routes.MapWebQueryRoute<AssignCoordinatesToAccountWebQuery, ManageMinerController>(c => c.AssignCoordinatesToAccount);
            routes.MapWebQueryRoute<GetDealerDataWebQuery, ShawHomeController>(c => c.GetDealerData);
            routes.MapStaticViewWebQueryRoute<LeadAnalyticalWebQuery, ManageMinerController>(c => c.AddLeadAnalyticalData);
            routes.MapStaticViewWebQueryRoute<GetLeadsChartDataWebQuery, ShawHomeController>(c => c.GetLeadsChartData);
            routes.MapWebQueryRoute<DealerPerformanceWebQuery, DealerPerformanceController>(c => c.ViewPage);
            routes.MapWebQueryRoute<DealerPerformanceDetailWebQuery, DealerPerformanceDetailController>(c => c.ViewPage);
            routes.MapWebQueryRoute<ShawLeadsWebQuery, ShawLeadsController>(c => c.ViewPage);
            routes.MapStaticViewWebQueryRoute<SubscribeLeadUpdatesWebQuery, ManageMinerController>(c => c.SetSubscribeLeadUpdates);
            routes.MapStaticViewWebQueryRoute<SSCreateDateFixWebQuery, ManageMinerController>(c => c.SSCreateDateFix);
            routes.MapWebQueryRoute<ShawDealerLeadsWebQuery, ShawDealerLeadsController>(c => c.ViewPage);
            routes.MapWebQueryRoute<NextDayRouteWebQuery, NextDayRouteController>(c => c.ViewPage);
            routes.MapStaticViewWebQueryRoute<GetNextDayRouteTicketWebQuery, NextDayRouteController>(c => c.GetNextDayRouteTicket);
            routes.MapStaticViewWebQueryRoute<CheckNextDayRouteTicketWebQuery, NextDayRouteController>(c => c.CheckNextDayRouteTicket);
        }
    }
}
