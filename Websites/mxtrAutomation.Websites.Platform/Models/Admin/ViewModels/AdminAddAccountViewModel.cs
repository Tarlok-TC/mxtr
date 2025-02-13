using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Account.ViewModels;

namespace mxtrAutomation.Websites.Platform.Models.Admin.ViewModels
{
    public class AdminAddAccountViewModel : MainLayoutViewModelBase
    {
        public AccountProfileViewModel AccountProfile { get; set; }
        public AccountAttributesViewModel AccountAttributes { get; set; }
        public AccountUsersViewModel AccountUsers { get; set; }
    }
}
