using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewData;
using mxtrAutomation.Corporate.Data.DataModels;
using System;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface ILeadViewModelAdapter
    {
        LeadViewModel BuildLeadViewModel(CRMLeadDataModel lead, mxtrAccount account, IEnumerable<mxtrAccount> client, mxtrAccount parentMxtrAccount, List<CRMLeadEventDataModel> leadEvents, mxtrAccount parentAccount, Dictionary<string, Tuple<string, bool, long>> dicClonedAccount);
    }

    public class LeadViewModelAdapter : ILeadViewModelAdapter
    {
        public LeadViewModel BuildLeadViewModel(CRMLeadDataModel lead, mxtrAccount account, IEnumerable<mxtrAccount> client, mxtrAccount parentMxtrAccount, List<CRMLeadEventDataModel> leadEvents, mxtrAccount parentAccount, Dictionary<string, Tuple<string, bool, long>> dicClonedAccount)
        {
            LeadViewModel model = new LeadViewModel();

            AddPageTitle(model);
            AddLeadData(model, lead, leadEvents);
            AddLeadAccount(model, account);
            AddClient(model, client);
            AddParent(model, parentAccount);
            AddSharpspringData(model, parentMxtrAccount);
            AddClonedAccount(model, dicClonedAccount);
            return model;
        }

        private void AddSharpspringData(LeadViewModel model, mxtrAccount parentMxtrAccount)
        {
            model.Lead.SharpspringAccountID = parentMxtrAccount.SharpspringAccountID;
            model.Lead.SharpspringSecretKey = parentMxtrAccount.SharpspringSecretKey;
        }

        private void AddClient(LeadViewModel model, IEnumerable<mxtrAccount> client)
        {
            model.Lead.Clients = client.Where(w => !string.IsNullOrEmpty(w.AccountName)).Select(x => new SelectListItem
            {
                Value = x.ObjectID,
                Text = x.AccountName + " " + x.ZipCode + " " + x.City + " " + x.State,
            }).OrderBy(o => o.Text);
        }

        private void AddParent(LeadViewModel model, mxtrAccount parentAccount)
        {
            List<mxtrAccount> lstParent = new List<mxtrAccount>();
            lstParent.Add(parentAccount);

            model.Lead.Parent = lstParent.Where(w => !string.IsNullOrEmpty(w.AccountName)).Select(x => new SelectListItem
            {
                Value = x.ObjectID,
                Text = x.AccountName + " " + x.ZipCode + " " + x.City + " " + x.State,
            });
        }

        private void AddClonedAccount(LeadViewModel model, Dictionary<string, Tuple<string, bool, long>> dicClonedAccount)
        {
            model.Lead.ClonedAccount = dicClonedAccount;
        }

        private void AddLeadAccount(LeadViewModel model, mxtrAccount account)
        {
            model.Lead.LeadParentAccount = account.AccountName;
        }

        public void AddPageTitle(LeadViewModel model)
        {
            model.PageTitle = "Lead";
            model.MainPageHeader = "Lead";
            model.SubPageHeader = "View lead.";
            model.ShowWorkspaceFilter = false;
            model.ShowDateFilter = false;
        }

        public void AddLeadData(LeadViewModel model, CRMLeadDataModel lead, List<CRMLeadEventDataModel> leadEvents)
        {
            model.Lead = new LeadViewData
            {
                ObjectID = lead.ObjectID,
                AccountObjectID = lead.AccountObjectID,
                CreateDate = lead.CreateDate,
                LeadID = lead.LeadID,
                OwnerID = lead.OwnerID,
                CampaignID = lead.CampaignID,
                LeadStatus = lead.LeadStatus,
                LeadScore = lead.LeadScore,
                IsActive = lead.IsActive,
                FirstName = lead.FirstName ?? string.Empty,
                LastName = lead.LastName ?? string.Empty,
                EmailAddress = lead.EmailAddress ?? string.Empty,
                CompanyName = lead.CompanyName ?? string.Empty,
                City = lead.City ?? string.Empty,
                State = lead.State ?? string.Empty,
                EventsCount = lead.Events.Count(),
                EventsViewData = lead.Events.Select(e => new EventViewData
                {
                    EventType = e.WhatType,
                    EventDescription = e.EventName,
                    CreateTimestamp = e.CreateTimestamp,
                    EventID = e.EventID,
                    LeadID = e.LeadID,
                    WhatID = e.WhatID,
                    LeadAccountName = e.LeadAccountName,
                    CopiedToParent = e.CopiedToParent,
                    EventData = e.EventData.Select(ed => new EventEventData
                    {
                        Name = ed.Name,
                        Value = ed.Value
                    }).ToList(),
                }).OrderByDescending(e => e.CreateTimestamp).ToList()
            };

            if (leadEvents.Count > 0)
            {
                List<EventViewData> lstClonedLeadEvents = leadEvents.Select(e => new EventViewData
                {
                    EventType = e.WhatType,
                    EventDescription = e.EventName,
                    CreateTimestamp = e.CreateTimestamp,
                    EventID = e.EventID,
                    LeadID = e.LeadID,
                    WhatID = e.WhatID,
                    LeadAccountName = e.LeadAccountName,
                    CopiedToParent = e.CopiedToParent,
                    EventData = e.EventData.Select(ed => new EventEventData
                    {
                        Name = ed.Name,
                        Value = ed.Value
                    }).ToList(),
                }).OrderByDescending(e => e.CreateTimestamp).ToList();
                //add only if new event
                foreach (var item in lstClonedLeadEvents)
                {
                    if (model.Lead.EventsViewData.Where(w => w.EventID == item.EventID).Count() == 0)
                    {
                        item.IsCopied = true;
                        model.Lead.EventsViewData.Add(item);
                    }
                }

                model.Lead.EventsViewData = model.Lead.EventsViewData.OrderByDescending(o => o.CreateTimestamp).ToList();
            }
        }

    }
}
