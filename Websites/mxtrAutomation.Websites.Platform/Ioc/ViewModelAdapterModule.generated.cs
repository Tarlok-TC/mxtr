using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Ioc
{
    public partial class ViewModelAdapterModule
    {
        partial void LoadViews()
        {
            Bind<ISiteTopNavViewModelAdapter>().To<SiteTopNavViewModelAdapter>().InSingletonScope();
            Bind<ILoginViewModelAdapter>().To<LoginViewModelAdapter>().InSingletonScope();
            Bind<IAdminAddAccountViewModelAdapter>().To<AdminAddAccountViewModelAdapter>().InSingletonScope();
            Bind<IAdminAccountUserManagementViewModelAdapter>().To<AdminAccountUserManagementViewModelAdapter>().InSingletonScope();
            Bind<IAdminEditAccountViewModelAdapter>().To<AdminEditAccountViewModelAdapter>().InSingletonScope();
            Bind<IAccountProfileViewModelAdapter>().To<AccountProfileViewModelAdapter>().InSingletonScope();
            Bind<IAccountAttributesViewModelAdapter>().To<AccountAttributesViewModelAdapter>().InSingletonScope();
            Bind<IAccountUsersViewModelAdapter>().To<AccountUsersViewModelAdapter>().InSingletonScope();
            Bind<IDashboardViewModelAdapter>().To<DashboardViewModelAdapter>().InSingletonScope();
            Bind<IWorkspaceFilterViewModelAdapter>().To<WorkspaceFilterViewModelAdapter>().InSingletonScope();
            Bind<ILeadsViewModelAdapter>().To<LeadsViewModelAdapter>().InSingletonScope();
            Bind<ILeadViewModelAdapter>().To<LeadViewModelAdapter>().InSingletonScope();
            Bind<IActivityViewModelAdapter>().To<ActivityViewModelAdapter>().InSingletonScope();
            Bind<ICampaingViewModelAdapter>().To<CampaingViewModelAdapter>().InSingletonScope();
            Bind<IRetailersViewModelAdapter>().To<RetailersViewModelAdapter>().InSingletonScope();
            Bind<IContactsViewModelAdapter>().To<ContactsViewModelAdapter>().InSingletonScope();
            Bind<IContactViewModelAdapter>().To<ContactViewModelAdapter>().InSingletonScope();
            Bind<IEmailViewModelAdapter>().To<EmailViewModelAdapter>().InSingletonScope();
            Bind<IIndexViewModelAdapter>().To<IndexViewModelAdapter>().InSingletonScope();
            Bind<ICreativeViewModelAdapter>().To<CreativeViewModelAdapter>().InSingletonScope();
            Bind<IDMAPerformanceViewModelAdapter>().To<DMAPerformanceViewModelAdapter>().InSingletonScope();
            Bind<IDetailedViewModelAdapter>().To<DetailedViewModelAdapter>().InSingletonScope();
            Bind<IRegionPerformanceViewModelAdapter>().To<RegionPerformanceViewModelAdapter>().InSingletonScope();
            Bind<IRetailersOverviewViewModelAdapter>().To<RetailersOverviewViewModelAdapter>().InSingletonScope();
            Bind<IRetailerViewModelAdapter>().To<RetailerViewModelAdapter>().InSingletonScope();
            Bind<IProfileViewModelAdapter>().To<ProfileViewModelAdapter>().InSingletonScope();
            Bind<Controllers.IManageMinerViewModelAdapter>().To<Controllers.ManageMinerViewModelAdapter>().InSingletonScope();
            Bind<IManageMenuViewModelAdapter>().To<ManageMenuViewModelAdapter>().InSingletonScope();
            Bind<IWhiteLabelingViewModelAdapter>().To<WhiteLabelingViewModelAdapter>().InSingletonScope();
            Bind<ICustomerViewModelAdapter>().To<CustomerViewModelAdapter>().InSingletonScope();
            Bind<IKlipfolioViewModelAdapter>().To<KlipfolioViewModelAdapter>().InSingletonScope();
            Bind<ISharpspringViewModelAdapter>().To<SharpspringViewModelAdapter>().InSingletonScope();
            Bind<IProshredHomeViewModelAdapter>().To<ProshredHomeViewModelAdapter>().InSingletonScope();
            Bind<IShawHomeViewModelAdapter>().To<ShawHomeViewModelAdapter>().InSingletonScope();
            Bind<IDealerPerformanceViewModelAdapter>().To<DealerPerformanceViewModelAdapter>().InSingletonScope();
            Bind<IDealerPerformanceDetailViewModelAdapter>().To<DealerPerformanceDetailViewModelAdapter>().InSingletonScope();
            Bind<IShawLeadsViewModelAdapter>().To<ShawLeadsViewModelAdapter>().InSingletonScope();
            Bind<IShawDealerLeadsViewModelAdapter>().To<ShawDealerLeadsViewModelAdapter>().InSingletonScope();
            Bind<INextDayRouteViewModelAdapter>().To<NextDayRouteViewModelAdapter>().InSingletonScope();

        }
    }
}
