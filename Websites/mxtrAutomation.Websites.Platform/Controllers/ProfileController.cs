using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.Models.Profile.ViewModels;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class ProfileController : MainLayoutControllerBase
    {
        private readonly IProfileViewModelAdapter _viewModelAdapter;
        public ProfileController(IProfileViewModelAdapter viewModelAdapter)
        {
            _viewModelAdapter = viewModelAdapter;
        }
        public ActionResult ViewPage(ProfileWebQuery query)
        {
            mxtrUser mxtrUser = UserService.GetUserByUserObjectID(User.MxtrUserObjectID);
            ProfileViewModel model = _viewModelAdapter.BuildProfileViewModel(mxtrUser); 
            return View(ViewKind.Profile, model, query);
        }

        public ActionResult EditProfileSubmit(ProfileEditSubmitWebQuery query)
        {
            bool success = false;
            string message = string.Empty;
            bool IsCurrentPasswordVerified = UserService.IsCurrentPasswordVerified(User.UserName, query.Password);
            if (IsCurrentPasswordVerified)
            {
                //update password
                if (UserService.UpdateUserPassword(User.MxtrUserObjectID, query.NewPassword))
                {
                    success = true;
                    message = "Password updated successfully";
                }
                else
                {
                    success = false;
                    message = "Please try again";
                }
            }
            else
            {
                message = "Current password do not match";
            } 
            return Json(new { Success = success, Message = message});
        }
    }
}
