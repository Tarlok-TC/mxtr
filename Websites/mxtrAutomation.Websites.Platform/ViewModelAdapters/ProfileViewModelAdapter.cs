using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Profile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IProfileViewModelAdapter
    {
        ProfileViewModel BuildProfileViewModel(mxtrUser user);
    }
    public class ProfileViewModelAdapter : IProfileViewModelAdapter
    {
        public ProfileViewModel BuildProfileViewModel(mxtrUser user)
        {
            ProfileViewModel model = new ProfileViewModel();
            AddPageTitle(model);
            AddData(model, user);
            return model;
        }

        public void AddPageTitle(ProfileViewModel model)
        {
            model.PageTitle = "Profile";
        }

        private void AddData(ProfileViewModel model, mxtrUser user)
        {
            model.ObjectID = user.ObjectID;
            model.MxtrUserID = user.MxtrUserID;
            model.MxtrAccountID = user.MxtrAccountID;
            model.AccountObjectID = user.AccountObjectID;
            model.FullName = user.FullName;
            model.Email = user.Email;
            model.UserName = user.UserName;
            model.Phone = user.Phone;
            model.CellPhone = user.CellPhone;
            model.EZShredAccountMappings = user.EZShredAccountMappings;
            model.Role = user.Role;
            model.Permissions = user.Permissions;
            model.CreateDate = user.CreateDate;
            model.IsActive = user.IsActive;
            model.IsApproved = user.IsApproved;
            model.IsLockedOut = user.IsLockedOut;
            model.SharpspringPassword = user.SharpspringPassword;
            model.SharpspringUserName = user.SharpspringUserName;
        }

    }
}