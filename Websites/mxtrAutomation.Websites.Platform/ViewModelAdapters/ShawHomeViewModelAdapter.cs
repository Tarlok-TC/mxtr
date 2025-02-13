using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Websites.Platform.Models.ShawHome.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IShawHomeViewModelAdapter
    {
        ShawHomeViewModel BuildShawHomeViewModel(string mxtrAccountObjectID, string memberCount, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, List<long> leadIds, List<long> shawLeadIds, ICRMLeadService _leadService, IShawLeadDetailService _dbShawLeadDetailService);
    }
    public class ShawHomeViewModelAdapter : IShawHomeViewModelAdapter
    {
        public ShawHomeViewModel BuildShawHomeViewModel(string mxtrAccountObjectID, string memberCount, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, List<long> leadIds, List<long> shawLeadIds, ICRMLeadService _leadService, IShawLeadDetailService _dbShawLeadDetailService)
        {
            ShawHomeViewModel model = new ShawHomeViewModel();
            AddAttribute(startDate, endDate, accountObjectIDs, model);
            AddPageData(mxtrAccountObjectID, memberCount, accountObjectIDs, leadIds, shawLeadIds, model, _leadService, _dbShawLeadDetailService, startDate, endDate);
            return model;
        }

        private static void AddAttribute(DateTime startDate, DateTime endDate, List<string> accountObjectIDs, ShawHomeViewModel model)
        {
            model.PageTitle = "Home";
            model.ShowWorkspaceFilter = false;
            model.ShowDateFilter = true;
            model.StartDate = startDate.ToShortDateString();
            model.EndDate = endDate.ToShortDateString();
            model.CallbackFunction = "updatePageFromWorkspace";
            model.UpdateDataUrl = new ShawHomeWebQuery();
            model.CurrentAccountIDs = string.Join(",", accountObjectIDs);
        }

        private static void AddPageData(string mxtrAccountObjectID, string memberCount, List<string> accountObjectIDs, List<long> leadIds, List<long> shawLeadIds, ShawHomeViewModel model, ICRMLeadService leadService, IShawLeadDetailService _dbShawLeadDetailService, DateTime startDate, DateTime endDate)
        {
            model.MemberCount = String.IsNullOrEmpty(memberCount) ? "0" : memberCount;
            model.ParticipatingDealerCount = accountObjectIDs == null ? "0" : accountObjectIDs.Count.ToString();
            // model.AverageLeadScore = leadService.GetLeadScores(leadIds) / (Convert.ToInt32(model.MemberCount) > 0 ? Convert.ToInt32(model.MemberCount) : 1);          

            //Tuple<double, int, int, int> resultData = _dbShawLeadDetailService.GetLeadScores(leadIds);
            Tuple<double, int, int, int> resultData = _dbShawLeadDetailService.GetLeadScores(shawLeadIds, startDate, endDate);
            int members = Convert.ToInt32(model.MemberCount);
            if (members > 0)
            {
                double averageLeadScore = Math.Round((double)resultData.Item1 / (double)members, 0);
                model.AverageLeadScore = string.Format("{0:n0}", averageLeadScore);
            }
            model.LeadScoreCount = string.Format("{0:n0}", members);
            //model.LeadScoreCount = string.Format("{0:n0}", resultData.Item2);
            model.LeadScoreMin = string.Format("{0:n0}", resultData.Item3);
            model.LeadScoreMax = string.Format("{0:n0}", resultData.Item4);

            model.AveragePassToDealerDays = string.Format("{0:n0}", Math.Round(leadService.GetAveragePassToDealerDays(mxtrAccountObjectID), 0));
            model.AverageCreateDateToSaleDate = string.Format("{0:n0}", Math.Round(leadService.GetAverageCreateToSaleDate(leadIds), 0));
            //---model.PassOffLeadCount = leadService.GetCopiedLeads(mxtrAccountObjectID);
            int passOffLeadCount = leadService.GetCopiedLeads(accountObjectIDs, startDate, endDate);
            model.PassOffLeadCount = string.Format("{0:n0}", passOffLeadCount);
            int wonLeadCount = leadService.GetWonLeadCount(leadIds);
            model.WonLeadCount = string.Format("{0:n0}", wonLeadCount);
            double passoffRate = 0;
            double conversionRate = 0;
            if (members > 0)
            {
                passoffRate = ((double)passOffLeadCount / (double)(Convert.ToInt32(model.MemberCount) > 0 ? Convert.ToInt32(model.MemberCount) : 1)) * 100;
                conversionRate = ((double)wonLeadCount / (double)(Convert.ToInt32(model.MemberCount) > 0 ? Convert.ToInt32(model.MemberCount) : 1)) * 100;
            }
            //double passoffRate = ((double)model.PassOffLeadCount / (double)(Convert.ToInt32(model.MemberCount) > 0 ? Convert.ToInt32(model.MemberCount) : 1)) * 100;
            //double conversionRate = ((double)model.WonLeadCount / (double)(Convert.ToInt32(model.MemberCount) > 0 ? Convert.ToInt32(model.MemberCount) : 1)) * 100;
            model.PassOffRate = Math.Round(passoffRate, 0);// (leadService.GetCopiedLeads(mxtrAccountObjectID) / (Convert.ToInt32(model.MemberCount) > 0 ? Convert.ToInt32(model.MemberCount) : 1)) * 100;
            model.ConversionRate = Math.Round(conversionRate, 0); //(model.WonLeadCount / (Convert.ToInt32(model.MemberCount) > 0 ? Convert.ToInt32(model.MemberCount) : 1)) * 100;
            model.MemberCount = string.Format("{0:n0}", members);
        }
    }
}