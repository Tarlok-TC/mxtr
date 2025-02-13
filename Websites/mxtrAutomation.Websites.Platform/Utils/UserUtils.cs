using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Account.ViewData;

namespace mxtrAutomation.Websites.Platform.Utils
{
    public interface IUserUtils
    {
        mxtrUser ConvertToMxtrUserDataModel(AccountUserViewData userData);
    }

    public class UserUtils : IUserUtils
    {
        public mxtrUser ConvertToMxtrUserDataModel(AccountUserViewData userData)
        {
            return
                    new mxtrUser
                    {
                        ObjectID = userData.ObjectID,
                        MxtrUserID = userData.MxtrUserID != Guid.Empty ? userData.MxtrUserID.ToString() : Guid.NewGuid().ToString(),
                        MxtrAccountID = userData.MxtrAccountID.ToString(),
                        AccountObjectID = userData.AccountObjectID,
                        FullName = userData.FullName,
                        Email = !string.IsNullOrEmpty(userData.Email) ? userData.Email : userData.UserName,
                        UserName = userData.UserName,
                        Password = userData.Password,
                        Phone = userData.Phone,
                        CellPhone = userData.CellPhone,
                        EZShredAccountMappings = userData.EZShredAccountMappings,
                        Role = userData.Role,
                        Permissions = userData.Permissions != null ? userData.Permissions.Split(',').ToList<string>() : new List<string>(),
                        SharpspringPassword = userData.SharpspringPassword,
                        SharpspringUserName = userData.SharpspringUserName,
                    };
        }
    }
}