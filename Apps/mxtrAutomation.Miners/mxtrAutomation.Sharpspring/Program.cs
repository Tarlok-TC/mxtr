using System;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Collections.Generic;
using System.Reflection;
using Ninject;
using System.IO;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Api.Sharpspring;
using mxtrAutomation.Api.Services;
using mxtrAutomation.Common.Utils;
using mxtrAutomation.Common.Services;
using mxtrAutomation.Common.Extensions;
using System.Configuration;
using mxtrAutomation.Sharpspring.Adapter;

namespace mxtrAutomation.Sharpspring
{
    public class Program
    {
        private static IAccountService _dbAccountService;
        private static IMinerRunService _dbMinerRunService;
        private static ICRMLeadService _dbCRMLeadService;
        private static ICRMCampaignService _dbCRMCampaignService;
        private static ICRMEmailJobService _dbCRMEmailJobService;
        private static ICRMDealStageService _dbCRMDealStageService;
        private static ICRMOpportunityService _dbCRMOpportunityService;
        private static ISharpspringService _apiSharpspringService;
        private static ICRMEmailService _dbCRMEmailService;
        private static IErrorLogService _dbErrorLogService;

        public static void Main(string[] args)
        {
            try
            {
                AppStart();
                PrepareServices();

                if (args.Length < 1)
                {
                    System.Console.WriteLine("Enter mode type");
                }
                else
                {
                    var options = new Options();
                    if (CommandLine.Parser.Default.ParseArguments(args, options))
                    {
                        if (options.Mode.ToLower() == "single")
                        {
                            //need account id
                        }
                        else
                        {
                            System.Console.WriteLine("Processing Sharpspring data for all accounts...");
                            ProcessSharpspringForAllAccounts();
                        }
                        Environment.Exit(-1);
                    };
                }
            }
            catch (Exception ex)
            {
                _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                {
                    LogTime = DateTime.UtcNow,
                    Description = "Miner Setup Error",
                    LogType = ErrorKind.Miner + " " + CRMKind.Sharpspring.ToString(),
                    ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                });
            }
        }

        static void ProcessSharpspringForAllAccounts()
        {
            bool doProcessEmail = Convert.ToBoolean(ConfigurationManager.AppSettings["DoProcessEmail"]);
            bool isFirstTimeRun = false;
            List<MinerRunDataCollectionSummary> summary = new List<MinerRunDataCollectionSummary>();
            List<SharpspringEmailJobDataModel> allEmailJobs = new List<SharpspringEmailJobDataModel>();
            List<SharpspringEmailDataModel> allEmails = new List<SharpspringEmailDataModel>();

            List<mxtrAccount> accounts = _dbAccountService.GetAllAccountsWithSharpspring().ToList();
            //accounts = accounts.Where(x => x.AccountName == "mXtr Automation").ToList();

            foreach (mxtrAccount account in accounts)
            {
                try
                {
                    System.Threading.Thread.Sleep(1000);
                    _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);

                    MinerRunAuditTrailDataModel lastMinerRunAuditTrailDataModel = _dbMinerRunService.GetLastMinerRunAuditTrailByAccountObjectID(account.ObjectID);

                    //DateTime entryStart = System.DateTime.Today;
                    DateTime entryStart = System.DateTime.Now;
                    DateTime checkPoint = entryStart;

                    if ((lastMinerRunAuditTrailDataModel == null) || (lastMinerRunAuditTrailDataModel.MinerRunDetails == null) || (IsFirstSharpspringMinerRun(lastMinerRunAuditTrailDataModel)))
                        isFirstTimeRun = true;

                    if (lastMinerRunAuditTrailDataModel != null)
                        entryStart = EstablishStartDateForMinerRun(lastMinerRunAuditTrailDataModel, isFirstTimeRun, entryStart);


                    if (doProcessEmail)
                    {
                        if (account.ObjectID != "59bbe6e9ed1f262a341f14de")//will not work for show floor
                        {
                            allEmailJobs = GetAllEmailJobsForAccount();
                            allEmails = _apiSharpspringService.GetAllEmails();
                        }
                    }

                    DateTimeRange runDates = DateTimeRange.CreateInclusive(entryStart, entryStart.AddDays(1));

                    // Running SS multiple time on same day
                    if (runDates.Start.Date == System.DateTime.Now.Date)
                    {
                        runDates = DateTimeRange.CreateInclusive(entryStart, checkPoint);
                    }

                    //while (runDates.Start < System.DateTime.Today)
                    while (runDates.Start <= checkPoint)
                    {
                        Console.WriteLine(string.Format("Processing data for {0} for {1}", account.AccountName, runDates.Start.ToShortDateString()));
                        summary.Clear();

                        List<MinerRunDataCollectionSummary> leadsSummary = ProcessLeads(account, runDates, isFirstTimeRun);
                        summary.AddRange(leadsSummary);

                        //Process Emails
                        if (doProcessEmail)
                        {
                            List<MinerRunDataCollectionSummary> emailJobsSummary = ProcessEmailJobs(account, runDates, isFirstTimeRun, allEmailJobs);
                            summary.AddRange(emailJobsSummary);
                            List<MinerRunDataCollectionSummary> emailsSummary = ProcessEmailsForAccount(account, runDates, isFirstTimeRun, allEmails);
                        }

                        //Process Campaigns
                        List<MinerRunDataCollectionSummary> campaignsSummary = ProcessCampaigns(account, runDates, isFirstTimeRun);
                        summary.AddRange(campaignsSummary);

                        ////Process DealStages
                        List<MinerRunDataCollectionSummary> dealStagesSummary = ProcessDealStages(account, runDates, isFirstTimeRun);
                        summary.AddRange(dealStagesSummary);

                        ////Process Opportunities
                        List<MinerRunDataCollectionSummary> opportunitiesSummary = ProcessOpportunities(account, runDates, isFirstTimeRun);
                        summary.AddRange(opportunitiesSummary);

                        //create miner run details for sharpspring
                        MinerRunMinerDetails minerRunMinerDetails = new MinerRunMinerDetails()
                        {
                            MinerName = MinerKind.Sharpspring.ToString(),
                            IsFirstMinerRunComplete = true,
                            LastMinerRunTime = System.DateTime.Now,
                            //LastStartDateForDataCollection = new DateTime(runDates.Start.Year, runDates.Start.Month, runDates.Start.Day, 0, 0, 0, DateTimeKind.Utc), //runDates.Start,
                            //LastEndDateForDataCollection = new DateTime(runDates.End.Year, runDates.End.Month, runDates.End.Day, 0, 0, 0, DateTimeKind.Utc), //runDates.End,

                            LastStartDateForDataCollection = new DateTime(runDates.Start.Year, runDates.Start.Month, runDates.Start.Day, runDates.Start.Hour, runDates.Start.Minute, runDates.Start.Second, DateTimeKind.Utc), //runDates.Start,
                            LastEndDateForDataCollection = new DateTime(runDates.End.Year, runDates.End.Month, runDates.End.Day, runDates.End.Hour, runDates.End.Minute, runDates.End.Second, DateTimeKind.Utc), //runDates.End,
                            Summary = summary
                        };

                        CreateNotificationReturn returnNotification = UpdateMinerRunAuditTrail(account, isFirstTimeRun, minerRunMinerDetails);

                        isFirstTimeRun = false;

                        if (runDates.Start.Date < System.DateTime.Now.Date)
                        {
                            if (runDates.End.Date == System.DateTime.Now.Date)
                            {
                                runDates = DateTimeRange.CreateInclusive(checkPoint, checkPoint);
                            }
                            else
                            {
                                runDates = DateTimeRange.CreateInclusive(runDates.Start.AddDays(1), runDates.End.AddDays(1));
                            }
                        }
                        else
                        {
                            // Running SS multiple time on same day
                            runDates = DateTimeRange.CreateInclusive(System.DateTime.Now, System.DateTime.Now);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                    {
                        LogTime = DateTime.UtcNow,
                        Description = "Error in processing account: " + account.AccountName,
                        LogType = ErrorKind.Miner + " " + CRMKind.Sharpspring.ToString(),
                        ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                    });
                }
            }
        }

        static void AppStart()
        {
            CreateKernel();
        }

        protected static IKernel CreateKernel()
        {
            IKernel kernel = new StandardKernel();

            Assembly.Load("mxtrAutomation.Common");
            //Assembly.Load("mxtrAutomation.Web.Common");
            Assembly.Load("mxtrAutomation.Data");
            Assembly.Load("mxtrAutomation.Corporate.Data");
            Assembly.Load("mxtrAutomation.Api");

            kernel.Load(AppDomain.CurrentDomain.GetAssemblies());

            NinjectServiceLocator locator = new NinjectServiceLocator(kernel);
            ServiceLocator.SetLocatorProvider(() => locator);

            return kernel;
        }

        protected static void PrepareServices()
        {
            _dbAccountService = ServiceLocator.Current.GetInstance<IAccountService>();
            _dbMinerRunService = ServiceLocator.Current.GetInstance<IMinerRunService>();
            _dbCRMLeadService = ServiceLocator.Current.GetInstance<ICRMLeadService>();
            _dbCRMCampaignService = ServiceLocator.Current.GetInstance<ICRMCampaignService>();
            _dbCRMEmailJobService = ServiceLocator.Current.GetInstance<ICRMEmailJobService>();
            _dbCRMDealStageService = ServiceLocator.Current.GetInstance<ICRMDealStageService>();
            _dbCRMOpportunityService = ServiceLocator.Current.GetInstance<ICRMOpportunityService>();
            _dbCRMEmailService = ServiceLocator.Current.GetInstance<ICRMEmailService>();
            _apiSharpspringService = ServiceLocator.Current.GetInstance<ISharpspringService>();
            _dbErrorLogService = ServiceLocator.Current.GetInstance<IErrorLogService>();
        }

        protected static bool IsFirstSharpspringMinerRun(MinerRunAuditTrailDataModel minerRunAuditTrailDataModel)
        {
            //no details for sharpspring, so obviously it hasn't run, thus it's the first sharpspring run
            if (!minerRunAuditTrailDataModel.MinerRunDetails.Any(x => x.MinerName == MinerKind.Sharpspring.ToString()))
                return true;

            //get all details for sharpspring miner runs
            //if any have first miner run complete = true, then it has run, thus it's not the first sharpspring run
            List<MinerRunMinerDetails> minerRunMinerDetails = minerRunAuditTrailDataModel.MinerRunDetails
                .Where(x => x.MinerName == MinerKind.Sharpspring.ToString())
                .ToList();

            if (minerRunMinerDetails.Any(x => x.IsFirstMinerRunComplete))
                return false;

            return true;
        }

        protected static DateTime EstablishStartDateForMinerRun(MinerRunAuditTrailDataModel minerRunAuditTrailDataModel, bool isFirstTimeRun, DateTime entryStart)
        {
            DateTime now = entryStart; //System.DateTime.Now;
            //DateTime startDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
            //DateTime endDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
            DateTime startDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
            DateTime endDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);

            int backFillMonths = Convert.ToInt32(ConfigurationManager.AppSettings["SharpspringFirstRunBackfillMonths"]);

            if (isFirstTimeRun || (minerRunAuditTrailDataModel.MinerRunDetails == null) || (minerRunAuditTrailDataModel.MinerRunDetails.Count == 0))
                return startDate.AddMonths(backFillMonths * -1);

            List<MinerRunMinerDetails> minerRunMinerDetails = minerRunAuditTrailDataModel.MinerRunDetails
               .Where(x => x.MinerName == MinerKind.Sharpspring.ToString())
               .ToList();

            DateTime lastEndDateForCollection = minerRunMinerDetails.Max(x => x.LastEndDateForDataCollection);

            if (lastEndDateForCollection <= endDate)
                //startDate = new DateTime(lastEndDateForCollection.Year, lastEndDateForCollection.Month, lastEndDateForCollection.Day, 0, 0, 0, DateTimeKind.Utc);
                startDate = new DateTime(lastEndDateForCollection.Year, lastEndDateForCollection.Month, lastEndDateForCollection.Day, lastEndDateForCollection.Hour, lastEndDateForCollection.Minute, lastEndDateForCollection.Second, DateTimeKind.Utc);

            return startDate;
        }

        protected static DateTimeRange EstablishDateRangeForMinerRun(MinerRunAuditTrailDataModel minerRunAuditTrailDataModel, bool isFirstTimeRun, DateTime entryStart)
        {
            DateTime now = entryStart; //System.DateTime.Now;
            //DateTime startDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
            //DateTime endDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
            DateTime startDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
            DateTime endDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);

            int backFillMonths = Convert.ToInt32(ConfigurationManager.AppSettings["SharpspringFirstRunBackfillMonths"]);

            if (isFirstTimeRun)
                return DateTimeRange.CreateInclusive(startDate.AddMonths(backFillMonths * -1), endDate);

            List<MinerRunMinerDetails> minerRunMinerDetails = minerRunAuditTrailDataModel.MinerRunDetails
               .Where(x => x.MinerName == MinerKind.Sharpspring.ToString())
               .ToList();

            DateTime lastEndDateForCollection = minerRunMinerDetails.Max(x => x.LastEndDateForDataCollection);

            if (lastEndDateForCollection <= endDate)
                //startDate = new DateTime(lastEndDateForCollection.Year, lastEndDateForCollection.Month, lastEndDateForCollection.Day, 0, 0, 0, DateTimeKind.Utc);
                startDate = new DateTime(lastEndDateForCollection.Year, lastEndDateForCollection.Month, lastEndDateForCollection.Day, lastEndDateForCollection.Hour, lastEndDateForCollection.Minute, lastEndDateForCollection.Second, DateTimeKind.Utc);

            return DateTimeRange.CreateInclusive(startDate, endDate);
        }

        protected static CreateNotificationReturn UpdateMinerRunAuditTrail(mxtrAccount account, bool isFirstTimeRun, MinerRunMinerDetails minerRunMinerDetails)
        {
            if (isFirstTimeRun)
            {
                List<MinerRunMinerDetails> listDetails = new List<MinerRunMinerDetails>();
                listDetails.Add(minerRunMinerDetails);

                MinerRunAuditTrailDataModel minerRunAuditTrailDataModel = new MinerRunAuditTrailDataModel
                {
                    AccountObjectID = account.ObjectID,
                    MxtrAccountID = account.MxtrAccountID,
                    MinerRunDetails = listDetails
                };
                return _dbMinerRunService.CreateMinerRunAuditTrail(minerRunAuditTrailDataModel);
            }
            else
            {
                return _dbMinerRunService.UpdateMinerRunDetails(account.ObjectID, minerRunMinerDetails);
            }
        }

        #region Leads

        protected static List<MinerRunDataCollectionSummary> ProcessLeads(mxtrAccount account, DateTimeRange runDates, bool isFirstTimeRun)
        {
            List<SharpspringLeadDataModel> updatedLeads = new List<SharpspringLeadDataModel>();
            List<SharpspringLeadDataModel> newLeads = new List<SharpspringLeadDataModel>();
            List<MinerRunDataCollectionSummary> summary = new List<MinerRunDataCollectionSummary>();

            List<CRMLeadEventDataModel> leadEvents = ProcessEvents(runDates);

            string newMessage = string.Empty;
            string updateMessage = string.Empty;
            string updateEventsMessage = string.Empty;

            if (isFirstTimeRun)
            {
                newLeads = _apiSharpspringService.GetAllLeads();
                //newLeads = _apiSharpspringService.GetNewLeadsForDateRange(runDates.Start, runDates.End);
            }
            else
            {
                //newLeads = _apiSharpspringService.GetNewLeadsForDateRange(runDates.Start.Date, runDates.End.Date);
                //updatedLeads = _apiSharpspringService.GetUpdatedLeadsForDateRange(runDates.Start.Date, runDates.End.Date);
                newLeads = _apiSharpspringService.GetNewLeadsForDateRange(runDates.Start.Date, runDates.End);
                updatedLeads = _apiSharpspringService.GetUpdatedLeadsForDateRange(runDates.Start.Date, runDates.End);
            }

            if (newLeads.Count() > 0)
            {
                List<CRMLeadDataModel> crmLeadDataModels = SharpspringApiToDataModelAdapter.AdaptLeads(account, newLeads, leadEvents, runDates.End);
                CreateNotificationReturn returnNotification = _dbCRMLeadService.CreateBatchCRMLeads(crmLeadDataModels);
                newMessage = returnNotification.Success.ToString();
            }

            if (updatedLeads.Count > 0)
            {
                List<CRMLeadDataModel> crmLeadDataModels = SharpspringApiToDataModelAdapter.AdaptLeads(account, updatedLeads, leadEvents, runDates.End);

                //need update data service
                CreateNotificationReturn returnNotification = _dbCRMLeadService.UpdateLeads(crmLeadDataModels, account.ObjectID, CRMKind.Sharpspring.ToString());
                updateMessage = returnNotification.Success.ToString();

            }

            if (leadEvents.Count > 0)
            {
                //get leads for events
                List<long> lst = leadEvents.Select(x => x.LeadID).Distinct().ToList();
                List<CRMLeadDataModel> leads = _dbCRMLeadService.GetLeadsByLeadIds(lst);

                List<CRMLeadDataModel> crmLeadDataModels = SharpspringApiToDataModelAdapter.AdaptLeads(account, leads, leadEvents, runDates.End);

                //need update data service
                CreateNotificationReturn returnNotification = _dbCRMLeadService.UpdateLeads(crmLeadDataModels, account.ObjectID, CRMKind.Sharpspring.ToString());
                updateEventsMessage = returnNotification.Success.ToString();
            }

            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.NewLeads.ToString(), Value = newLeads.Count().ToString(), Message = newMessage });
            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.UpdatedLeads.ToString(), Value = updatedLeads.Count().ToString(), Message = updateMessage });
            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.UpdatedLeads.ToString(), Value = updatedLeads.Count().ToString(), Message = updateEventsMessage });

            return summary;
        }

        protected static List<CRMLeadEventDataModel> ProcessEvents(DateTimeRange runDates)
        {
            List<SharpspringEventDataModel> events = new List<SharpspringEventDataModel>();
            //events = _apiSharpspringService.GetEventsByDate(runDates.Start);
            events = _apiSharpspringService.GetEventsByDate(runDates.Start.Date);

            List<CRMLeadEventDataModel> crmLeadEvents = events
                .Where(l => l.CreateTimestamp >= runDates.Start.Date && l.CreateTimestamp <= runDates.End)
                .Select(l => new CRMLeadEventDataModel()
                {
                    EventID = l.EventID,
                    LeadID = l.LeadID,
                    EventName = l.EventName,
                    WhatID = l.WhatID,
                    WhatType = l.WhatType,
                    EventData = l.EventData.Select(d => new CRMLeadEventEventData() { Name = d.Name, Value = d.Value }).ToList(),
                    CreateTimestamp = l.CreateTimestamp
                }).ToList();

            return crmLeadEvents;
        }

        #endregion

        #region Campaigns

        protected static List<MinerRunDataCollectionSummary> ProcessCampaigns(mxtrAccount account, DateTimeRange runDates, bool isFirstTimeRun)
        {
            List<SharpspringCampaignDataModel> updatedCampaigns = new List<SharpspringCampaignDataModel>();
            List<SharpspringCampaignDataModel> newCampaigns = new List<SharpspringCampaignDataModel>();
            List<MinerRunDataCollectionSummary> summary = new List<MinerRunDataCollectionSummary>();

            string newMessage = string.Empty;
            string updateMessage = string.Empty;

            if (isFirstTimeRun)
            {
                newCampaigns = _apiSharpspringService.GetAllCampaigns();
            }
            else
            {
                newCampaigns = _apiSharpspringService.GetNewCampaignsForDateRange(runDates.Start, runDates.End);
                updatedCampaigns = _apiSharpspringService.GetUpdatedCampaignsForDateRange(runDates.Start, runDates.End);
            }

            if (newCampaigns.Count() > 0)
            {
                List<CRMCampaignDataModel> crmCampaignDataModels = SharpspringApiToDataModelAdapter.AdaptCampaigns(account, newCampaigns, runDates.End);
                CreateNotificationReturn returnNotification = _dbCRMCampaignService.CreateBatchCRMCampaigns(crmCampaignDataModels);
                newMessage = returnNotification.Success.ToString();
            }

            if (updatedCampaigns.Count > 0)
            {
                List<CRMCampaignDataModel> crmCampaignDataModels = SharpspringApiToDataModelAdapter.AdaptCampaigns(account, updatedCampaigns, runDates.End);
                CreateNotificationReturn returnNotification = _dbCRMCampaignService.UpdateCampaigns(crmCampaignDataModels, account.ObjectID, CRMKind.Sharpspring.ToString());
                updateMessage = returnNotification.Success.ToString();
            }

            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.NewCampaigns.ToString(), Value = newCampaigns.Count().ToString(), Message = newMessage });
            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.UpdatedCampaigns.ToString(), Value = updatedCampaigns.Count().ToString(), Message = updateMessage });

            return summary;
        }

        #endregion

        #region Emails

        protected static List<MinerRunDataCollectionSummary> ProcessEmailsForAccount(mxtrAccount account, DateTimeRange runDates, bool isFirstTimeRun, List<SharpspringEmailDataModel> allEmails)
        {
            List<MinerRunDataCollectionSummary> summary = new List<MinerRunDataCollectionSummary>();

            string newMessage = string.Empty;

            List<CRMEmailDataModel> crmEmailDataModels = SharpspringApiToDataModelAdapter.AdaptEmails(account, allEmails, runDates.End);

            CreateNotificationReturn returnNotification = _dbCRMEmailService.UpsertEmails(crmEmailDataModels, account.ObjectID, CRMKind.Sharpspring.ToString());
            newMessage = returnNotification.Success.ToString();

            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.NewEmails.ToString(), Value = allEmails.Count().ToString(), Message = newMessage });

            return summary;


        }

        protected static List<SharpspringEmailJobDataModel> GetAllEmailJobsForAccount()
        {
            List<SharpspringEmailJobDataModel> allEmailJobs = new List<SharpspringEmailJobDataModel>();
            allEmailJobs = _apiSharpspringService.GetAllEmailJobs();
            return allEmailJobs;
        }

        protected static List<MinerRunDataCollectionSummary> ProcessEmailJobs(mxtrAccount account, DateTimeRange runDates, bool isFirstTimeRun, List<SharpspringEmailJobDataModel> allEmailJobs)
        {
            List<SharpspringEmailJobDataModel> newEmailJobs = new List<SharpspringEmailJobDataModel>();
            List<CRMEmailEventDataModel> allEmailEvents = new List<CRMEmailEventDataModel>();
            List<MinerRunDataCollectionSummary> summary = new List<MinerRunDataCollectionSummary>();

            string newMessage = string.Empty;

            newEmailJobs = allEmailJobs.Where(x => x.CreateTimestamp >= runDates.Start && x.CreateTimestamp <= runDates.End).ToList();

            if (newEmailJobs.Count() > 0)
            {
                allEmailEvents = ProcessEmailEvents(runDates, newEmailJobs);
                List<CRMEmailJobDataModel> crmEmailDataModels = SharpspringApiToDataModelAdapter.AdaptEmailJobs(account, newEmailJobs, allEmailEvents, runDates.End);
                CreateNotificationReturn returnNotification = _dbCRMEmailJobService.CreateBatchCRMEmailJobs(crmEmailDataModels);
                newMessage = returnNotification.Success.ToString();
            }

            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.NewEmailJobs.ToString(), Value = newEmailJobs.Count().ToString(), Message = newMessage });

            return summary;
        }

        protected static List<CRMEmailEventDataModel> ProcessEmailEvents(DateTimeRange runDates, List<SharpspringEmailJobDataModel> jobs)
        {
            List<CRMEmailEventDataModel> crmEmailEvents = new List<CRMEmailEventDataModel>();
            foreach (var emailJob in jobs)
            {
                List<SharpspringEmailEventDataModel> sends = _apiSharpspringService.GetEmailEvents(emailJob.EmailJobID, "sends");
                List<SharpspringEmailEventDataModel> clicks = _apiSharpspringService.GetEmailEvents(emailJob.EmailJobID, "clicks");
                List<SharpspringEmailEventDataModel> opens = _apiSharpspringService.GetEmailEvents(emailJob.EmailJobID, "opens");

                crmEmailEvents.AddRange(sends
                //.Where(l => l.CreateDate >= runDates.Start && l.CreateDate <= runDates.End)
                .Select(l => new CRMEmailEventDataModel()
                {
                    LeadID = l.LeadID,
                    EmailID = l.EmailID,
                    EmailJobID = l.EmailJobID,
                    EmailAddress = l.EmailAddress,
                    EventType = "sends",
                    CreateDate = l.CreateDate
                }).ToList());

                crmEmailEvents.AddRange(clicks
                //.Where(l => l.CreateDate >= runDates.Start && l.CreateDate <= runDates.End)
                .Select(l => new CRMEmailEventDataModel()
                {
                    LeadID = l.LeadID,
                    EmailID = l.EmailID,
                    EmailJobID = l.EmailJobID,
                    EmailAddress = l.EmailAddress,
                    EventType = "clicks",
                    CreateDate = l.CreateDate
                }).ToList());


                crmEmailEvents.AddRange(opens
                //.Where(l => l.CreateDate >= runDates.Start && l.CreateDate <= runDates.End)
                .Select(l => new CRMEmailEventDataModel()
                {
                    LeadID = l.LeadID,
                    EmailID = l.EmailID,
                    EmailJobID = l.EmailJobID,
                    EmailAddress = l.EmailAddress,
                    EventType = "opens",
                    CreateDate = l.CreateDate
                }).ToList());
            }

            return crmEmailEvents;
        }

        #endregion

        #region Deal Stages

        protected static List<MinerRunDataCollectionSummary> ProcessDealStages(mxtrAccount account, DateTimeRange runDates, bool isFirstTimeRun)
        {
            List<SharpspringDealStageDataModel> updatedDealStages = new List<SharpspringDealStageDataModel>();
            List<SharpspringDealStageDataModel> newDealStages = new List<SharpspringDealStageDataModel>();
            List<MinerRunDataCollectionSummary> summary = new List<MinerRunDataCollectionSummary>();

            string newMessage = string.Empty;
            string updateMessage = string.Empty;

            if (isFirstTimeRun)
            {
                newDealStages = _apiSharpspringService.GetAllDealStages();
            }
            else
            {
                newDealStages = _apiSharpspringService.GetNewDealStagesForDateRange(runDates.Start, runDates.End);
                updatedDealStages = _apiSharpspringService.GetUpdatedDealStagesForDateRange(runDates.Start, runDates.End);
            }

            if (newDealStages.Count() > 0)
            {
                List<CRMDealStageDataModel> crmDealStageDataModels = SharpspringApiToDataModelAdapter.AdaptDealStages(account, newDealStages, runDates.End);
                CreateNotificationReturn returnNotification = _dbCRMDealStageService.CreateBatchCRMDealStages(crmDealStageDataModels);
                newMessage = returnNotification.Success.ToString();
            }

            if (updatedDealStages.Count > 0)
            {
                List<CRMDealStageDataModel> crmDealStageDataModels = SharpspringApiToDataModelAdapter.AdaptDealStages(account, updatedDealStages, runDates.End);
                CreateNotificationReturn returnNotification = _dbCRMDealStageService.UpdateDealStages(crmDealStageDataModels, account.ObjectID, CRMKind.Sharpspring.ToString());
                updateMessage = returnNotification.Success.ToString();
            }

            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.NewDealStages.ToString(), Value = newDealStages.Count().ToString(), Message = newMessage });
            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.UpdatedDealStages.ToString(), Value = updatedDealStages.Count().ToString(), Message = updateMessage });

            return summary;
        }

        #endregion

        #region Opportunities

        protected static List<MinerRunDataCollectionSummary> ProcessOpportunities(mxtrAccount account, DateTimeRange runDates, bool isFirstTimeRun)
        {
            List<SharpspringOpportunityDataModel> updatedOpportunities = new List<SharpspringOpportunityDataModel>();
            List<SharpspringOpportunityDataModel> newOpportunities = new List<SharpspringOpportunityDataModel>();
            List<MinerRunDataCollectionSummary> summary = new List<MinerRunDataCollectionSummary>();

            string newMessage = string.Empty;
            string updateMessage = string.Empty;

            if (isFirstTimeRun)
            {
                newOpportunities = _apiSharpspringService.GetAllOpportunities();
            }
            else
            {
                newOpportunities = _apiSharpspringService.GetNewOpportunitiesForDateRange(runDates.Start, runDates.End);
                updatedOpportunities = _apiSharpspringService.GetUpdatedOpportunitiesForDateRange(runDates.Start, runDates.End);
            }

            if (newOpportunities.Count() > 0)
            {
                List<CRMOpportunityDataModel> crmOpportunitiesDataModels = SharpspringApiToDataModelAdapter.AdaptOpportunities(account, newOpportunities, runDates.End);
                CreateNotificationReturn returnNotification = _dbCRMOpportunityService.CreateBatchCRMOpportunities(crmOpportunitiesDataModels);
                newMessage = returnNotification.Success.ToString();
            }

            if (updatedOpportunities.Count > 0)
            {
                List<CRMOpportunityDataModel> crmOpportunitiesDataModels = SharpspringApiToDataModelAdapter.AdaptOpportunities(account, updatedOpportunities, runDates.End);
                CreateNotificationReturn returnNotification = _dbCRMOpportunityService.UpdateOpportunities(crmOpportunitiesDataModels, account.ObjectID, CRMKind.Sharpspring.ToString());
                updateMessage = returnNotification.Success.ToString();
            }

            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.NewOpportunities.ToString(), Value = newOpportunities.Count().ToString(), Message = newMessage });
            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.UpdateOpportunities.ToString(), Value = updatedOpportunities.Count().ToString(), Message = updateMessage });

            return summary;

        }

        #endregion
    }
}
