using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using mxtrAutomation.Common.Adapter;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Corporate.Data;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Data;
using mxtrAutomation.Api.Services;
using mxtrAutomation.Api.Sharpspring;
using System.Configuration;
using mxtrAutomation.Api.EZShred;

namespace mxtrAutomation.Corporate.Data.Ioc
{
    public class DataServicesModule : ServiceModuleBase
    {
        public class ObjectTrackingDisabledAttribute : Attribute { }

        public override void Load()
        {
            SetUpBinding<IAccountService, IAccountServiceInternal, AccountService>();
            SetUpBinding<IUserService, IUserServiceInternal, UserService>();
            SetUpBinding<IMinerRunService, IMinerRunServiceInternal, MinerRunService>();
            SetUpBinding<ICRMLeadService, ICRMLeadServiceInternal, CRMLeadService>();
            SetUpBinding<ICRMDealStageService, ICRMDealStageServiceInternal, CRMDealStageService>();
            SetUpBinding<ICRMCampaignService, ICRMCampaignServiceInternal, CRMCampaignService>();
            SetUpBinding<ICRMEmailJobService, ICRMEmailJobServiceInternal, CRMEmailJobService>();
            SetUpBinding<ICRMOpportunityService, ICRMOpportunityServiceInternal, CRMOpportunityService>();
            SetUpBinding<IDashboardSummaryService, IDashboardSummaryServiceInternal, DashboardSummaryService>();
            SetUpBinding<ICRMEmailService, ICRMEmailServiceInternal, CRMEmailService>();


            SetUpBinding<ICRMRestSearchResponseLogService, ICRMRestSearchResponseLogServiceInternal, CRMRestSearchResponseLogService>();

            SetUpBinding<IGoogleAnalyticPageTrackingService, IGoogleAnalyticPageTrackingServiceInternal, GoogleAnalyticPageTrackingService>();
            SetUpBinding<IGoogleReportingService, IGoogleReportingServiceInternal, GoogleReportingService>();
            SetUpBinding<IGoogleAnalyticService, IGoogleAnalyticServiceInternal, GoogleAnalyticService>();
            SetUpBinding<ICRMMinerRunScheduleLogService, ICRMMinerRunScheduleLogServiceInternal, CRMMinerRunScheduleLogService>();
            SetUpBinding<ICRMEZShredService, ICRMEZShredServiceInternal, CRMEZShredService>();
            SetUpBinding<IEZShredFieldLabelMappingService, IEZShredFieldLabelMappingServiceInternal, EZShredFieldLabelMappingService>();
            Bind<ISharpspringApi>().To<SharpspringApi>()
               .WithConstructorArgument("argBaseUrl", ConfigurationManager.AppSettings["SharpspringBaseUrl"]);
            Bind<ISharpspringService>().To<SharpspringService>().InSingletonScope();

            SetUpBinding<IErrorLogService, IErrorLogServiceInternal, ErrorLogService>();
            SetUpBinding<IManageMenuService, IManageMenuServiceInternal, ManageMenuService>();
            SetUpBinding<IEZShredLeadMappingService, IEZShredLeadMappingInternal, EZShredLeadMappingService>();

            Bind<IEZShredApi>().To<EZShredApi>()
                .WithConstructorArgument("argBaseUrl", "");
            Bind<IEZShredService>().To<EZShredService>().InSingletonScope();

            SetUpBinding<IEZShredMinerLogService, IEZShredMinerLogServiceInternal, EZShredMinerLogService>();
            SetUpBinding<ICRMEZShredCustomerService, ICRMEZShredCustomerServiceInternal, CRMEZShredCustomerService>();
            SetUpBinding<ICRMEZShredBuildingService, ICRMEZShredBuildingServiceInternal, CRMEZShredBuildingService>();
            SetUpBinding<ICorporateSSLastRunService, ICorporateSSLastRunServiceInternal, CorporateSSLastRunService>();
            SetUpBinding<IShawLeadDetailService, IShawLeadDetailServiceInternal, ShawLeadDetailService>();
            SetUpBinding<IShawListBasedLeadService, IShawListBasedLeadServiceInternal, ShawListBasedLeadService>();
            SetUpBinding<ISubscribeLeadUpdatesLogServices, ISubscribeLeadUpdatesLogServicesInternal, SubscribeLeadUpdatesLogServices>();
            SetUpBinding<INextDayRouteLogsServices, INextDayRouteLogsServicesInternal, NextDayRouteLogsServices>();
        }

    }
}
