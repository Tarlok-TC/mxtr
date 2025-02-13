using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Login.ViewModels;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface ILoginViewModelAdapter
    {
        LoginViewModel BuildLoginViewModel(mxtrAccount account);
    }

    public class LoginViewModelAdapter : ILoginViewModelAdapter
    {
        public LoginViewModel BuildLoginViewModel(mxtrAccount account)
        {
            LoginViewModel model = new LoginViewModel();        

            AddPageTitle(model);
            AddBrandingLogo(model, account);
            return model;
        }

        public void AddPageTitle(LoginViewModel model)
        {
            model.PageTitle = "Login";
        }

        public void AddBrandingLogo(LoginViewModel model, mxtrAccount account)
        {
            model.BrandingLogoURL = account.BrandingLogoURL;
        }
    }
}
