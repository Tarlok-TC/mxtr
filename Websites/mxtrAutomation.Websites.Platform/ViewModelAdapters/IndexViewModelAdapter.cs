using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Index.ViewData;
using mxtrAutomation.Websites.Platform.Models.Index.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewData;
using mxtrAutomation.Websites.Platform.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.IO;
using Newtonsoft.Json.Linq;
using mxtrAutomation.Websites.Platform.Helpers;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IIndexViewModelAdapter
    {
        IndexViewModel BuildIndexViewModel();
        IndexViewModel BuildIndexViewModel(List<mxtrAccount> accounts, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, List<GroupLeadsDataModel> groupLeadsDataModel);
    }

    public class IndexViewModelAdapter : IIndexViewModelAdapter
    {
        public IndexViewModel BuildIndexViewModel()
        {
            IndexViewModel model = new IndexViewModel();

            AddPageTitle(model);

            return model;
        }

        public void AddPageTitle(IndexViewModel model)
        {
            model.PageTitle = "Index";
        }

        public IndexViewModel BuildIndexViewModel(List<mxtrAccount> accounts, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, List<GroupLeadsDataModel> groupLeadsDataModel)
        {
            IndexViewModel model = new IndexViewModel();

            AddPageTitle(model);
            AddAttributes(model, startDate, endDate, accountObjectIDs);
            AddData(model, groupLeadsDataModel, accounts);

            return model;
        }

        private void AddAttributes(IndexViewModel model, DateTime startDate, DateTime endDate, List<string> accountObjectIDs)
        {
            model.StartDate = startDate.ToShortDateString();
            model.EndDate = endDate.ToShortDateString();
            model.CallbackFunction = "updatePageFromWorkspace";
            model.ShowWorkspaceFilter = true;
            model.ShowDateFilter = true;
            model.UpdateDataUrl = new IndexWebQuery();
            model.CurrentAccountIDs = string.Join(",", accountObjectIDs);
        }

        private void AddData(IndexViewModel model, List<GroupLeadsDataModel> groupLeadsDataModel, List<mxtrAccount> accounts)
        {
            model.IndexActivityAccountViewData =
                accounts
                .Where(w => w.AccountType != "Group" && w.IsActive)
                //.Where(w => w.AccountType != "Group")
                .Select(r => new IndexActivityAccountViewData
                {
                    AccountName = r.AccountName,
                    AccountObjectID = r.ObjectID
                }).ToList();

            model.GroupLeadsViewData = groupLeadsDataModel.Select(x => new GroupLeadsViewData()
            {
                Region = x.Region,
                State = "US-" + x.State.ToUpper(),
                TotalLeads = x.TotalLeads
            }).ToList();

            RegionHelper region = new RegionHelper();
            region = GetRegion();
            foreach (var item in region.States)
            {
                GroupLeadsViewData groupLeadsViewData = new GroupLeadsViewData();
                groupLeadsViewData = model.GroupLeadsViewData.Where(x => x.State == item.Abbreviation).Select(x => x).FirstOrDefault();
                if (groupLeadsViewData != null)
                {
                    groupLeadsViewData.AccountObjectID = AccountObjectIdByState(accounts, item.Abbreviation);
                    groupLeadsViewData.StateName = item.Name;
                }
                else
                {
                    model.GroupLeadsViewData.Add(new GroupLeadsViewData()
                    {
                        State = item.Abbreviation,
                        AccountObjectID = AccountObjectIdByState(accounts, item.Abbreviation),
                        StateName = item.Name
                    });
                }
            }
            model.GroupLeadsViewData.ForEach(x =>
            {
                if (region.Region.East.Contains(x.State))
                    x.Region = Helper.GetEnumDescription(RegionEnum.East);
                else if (region.Region.West.Contains(x.State))
                    x.Region = Helper.GetEnumDescription(RegionEnum.West);
                else if (region.Region.North.Contains(x.State))
                    x.Region = Helper.GetEnumDescription(RegionEnum.North);
                else if (region.Region.South.Contains(x.State))
                    x.Region = Helper.GetEnumDescription(RegionEnum.South);

                x.StateName = region.States.Where(y => y.Abbreviation == x.State).Select(z => z.Name).FirstOrDefault();
            });

        }

        private RegionHelper GetRegion()
        {
            RegionHelper region = JsonConvert.DeserializeObject<RegionHelper>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Scripts\\Region.json"));
            return region;
        }

        private string AccountObjectIdByState(List<mxtrAccount> accounts, string state)
        {
            var data = accounts.Where(x => x.State == state.Split('-')[1]).Select(x => x.ObjectID).ToArray();
            return string.Join(",", data);
        }
    }

}