using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface IManageMenuService
    {
        CreateNotificationReturn UpdateMenuData(List<ManageMenuDataModel> menuData);
        List<ManageMenuDataModel> GetMenuData(string accountObjectID);
        CreateNotificationReturn ResetMenuData(string accountObjectID, string userObjectId);
        bool IsMenuMasterExist();
        void AddMenuMaster();
        List<ManageMenuDataModel> GetMenuMaster();
        List<ManageMenuDataModel> GetAllMenuMaster();
        CreateNotificationReturn AddUpdateMenuMaster(ManageMenuDataModel menuData);
        CreateNotificationReturn DeleteMenuMasterData(string menuMasterId, string subMenuMasterId);
    }

    public interface IManageMenuServiceInternal : IManageMenuService
    {
    }
}
