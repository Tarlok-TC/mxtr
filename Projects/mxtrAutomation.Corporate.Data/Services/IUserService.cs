using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface IUserService
    {
        CreateNotificationReturn CreateUser(mxtrUser userData);
        mxtrUser GetUserByUserObjectID(string userObjectID);
        IEnumerable<mxtrUser> GetUsersByAccountObjectID(string accountObjectID);
        mxtrUserCookie Login(string username, string password);
        bool IsCurrentPasswordVerified(string username, string password);
        bool UpdateUserPassword(string userObjectID, string newPassword);
        CreateNotificationReturn DeleteUser(string userObjectID);
        bool IsUserExist(string userName);
        CreateNotificationReturn UpdateUser(mxtrUser userData);
        CreateNotificationReturn UpdateUserKlipfolioSSOToken(mxtrUser userData);
    }

    public interface IUserServiceInternal : IUserService
    {
    }
}
