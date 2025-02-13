using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Websites.Platform.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mxtrAutomation.Websites.Platform.Helpers
{
    public class MenuData
    {
        public static void CheckForMenuMasterData()
        {
            IManageMenuService _manageMenuService = ServiceLocator.Current.GetInstance<IManageMenuService>();
            if (!_manageMenuService.IsMenuMasterExist())
            {
                _manageMenuService.AddMenuMaster();
            }
        }      

        public static List<SelectListItem> SortOrders(int count)
        {
            Dictionary<string, string> dicSortOrder = new Dictionary<string, string>();
            List<SelectListItem> lstSortOrder = new List<SelectListItem>();

            for (int i = 1; i <= count; i++)
            {
                dicSortOrder.Add(i.ToString(), i.ToString());
            }

            foreach (var item in dicSortOrder)
            {
                lstSortOrder.Add(new SelectListItem
                {
                    Text = item.Key,
                    Value = item.Value,
                });
            }

            return lstSortOrder;
        }

        public static Dictionary<int, int> SortOrdersData(int count)
        {
            Dictionary<int, int> dicSortOrder = new Dictionary<int, int>();

            for (int i = 1; i <= count; i++)
            {
                dicSortOrder.Add(i, i);
            }

            return dicSortOrder;
        }
    }
}