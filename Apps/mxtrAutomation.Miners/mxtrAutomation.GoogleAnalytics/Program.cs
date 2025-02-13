using Google.Apis.AnalyticsReporting.v4.Data;
using GoogleAnalyticsApp.Adapter;
using mxtrAutomation.Api.Services;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Corporate.Data.Services;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace GoogleAnalyticsApp
{
    public class Program
    {
        private static IAccountService _dbAccountService;
        private static IMinerRunService _dbMinerRunService;
        private static IGoogleReportingService _dbGoogleReportingService;
        private static IGoogleAnalyticPageTrackingService _dbGoogleAnalyticPageTrackingService;
        private static IErrorLogService _dbErrorLogService;

        [STAThread]
        static void Main(string[] args)
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
                            System.Console.WriteLine("Processing GoogleAnalytics data for all accounts...");
                            ProcessGoogleForAllAccounts();
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
                    LogType = ErrorKind.Miner + " " + CRMKind.GoogleAnalytics.ToString(),
                    ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                });
            }
        }

        static void ProcessGoogleForAllAccounts()
        {
            bool isFirstTimeRun = false;
            var summary = new List<MinerRunDataCollectionSummary>();

            var accounts = _dbAccountService.GetAllAccountsWithGoogle().ToList();

            foreach (mxtrAccount account in accounts)
            {
                try
                {
                    _dbGoogleReportingService.SetConnectionTokens(account.GoogleAnalyticsReportingViewId, account.GoogleServiceAccountEmail, account.GoogleServiceAccountCredentialFile, account.GoogleAnalyticsTimeZoneName);

                    var lastMinerRunAuditTrailDataModel = _dbMinerRunService.GetLastMinerRunAuditTrailByAccountObjectID(account.ObjectID);

                    var entryStart = System.DateTime.Today;
                    var lastRunTrack = System.DateTime.Today;

                    if ((lastMinerRunAuditTrailDataModel == null) || (lastMinerRunAuditTrailDataModel.MinerRunDetails == null) || (IsFirstGoogleAnalyticsMinerRun(lastMinerRunAuditTrailDataModel)))
                        isFirstTimeRun = true;

                    if (lastMinerRunAuditTrailDataModel != null)
                        entryStart = lastRunTrack = EstablishStartDateForMinerRun(lastMinerRunAuditTrailDataModel, isFirstTimeRun);

                    while (entryStart < System.DateTime.Today)
                    {
                        Console.WriteLine(string.Format("Processing data for {0} for {1}", account.AccountName, entryStart.ToShortDateString()));
                        summary.Clear();

                        var pageViewsSummary = ProcessPageViews(account, entryStart, lastRunTrack, isFirstTimeRun);
                        summary.AddRange(pageViewsSummary);

                        //create miner run details for google analytics
                        MinerRunMinerDetails minerRunMinerDetails = new MinerRunMinerDetails()
                        {
                            MinerName = MinerKind.GoogleAnalytics.ToString(),
                            IsFirstMinerRunComplete = true,
                            LastMinerRunTime = System.DateTime.Now,
                            LastStartDateForDataCollection = new DateTime(entryStart.Year, entryStart.Month, entryStart.Day, 0, 0, 0, DateTimeKind.Utc),
                            LastEndDateForDataCollection = new DateTime(entryStart.Year, entryStart.Month, entryStart.Day, 0, 0, 0, DateTimeKind.Utc),
                            Summary = summary
                        };

                        CreateNotificationReturn returnNotification = UpdateMinerRunAuditTrail(account, isFirstTimeRun, minerRunMinerDetails);

                        isFirstTimeRun = false;
                        entryStart = entryStart.AddDays(1);
                    }
                }
                catch (Exception ex)
                {
                    _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                    {
                        LogTime = DateTime.UtcNow,
                        Description = "Error in processing account: " + account.AccountName,
                        LogType = ErrorKind.Miner + " " + CRMKind.GoogleAnalytics.ToString(),
                        ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                    });
                }
            }
        }

        private static List<MinerRunDataCollectionSummary> ProcessPageViews(mxtrAccount account, DateTime entryStart, DateTime lastRunTrack, bool isFirstTimeRun)
        {
            var summary = new List<MinerRunDataCollectionSummary>();

            var dateRange = new List<DateRange>() { new DateRange() { StartDate = entryStart.ToString("yyyy-MM-dd"), EndDate = entryStart.ToString("yyyy-MM-dd") } };

            //get pages' views
            var dimensions = new List<Dimension>() {
                        new Dimension { Name = "ga:pagePath" },
                        new Dimension { Name = "ga:pageTitle" },
                        new Dimension { Name = "ga:date" }
                };
            var metrics = new List<Metric>() { new Metric { Expression = "ga:pageviews" } };

            var response = _dbGoogleReportingService.GetReportsResponse(dateRange, dimensions, metrics);
            var pages = GoogleDataAdapter.AdaptGoogleReportResults<GAPageTrackingDataModel>(response.Reports.FirstOrDefault(), _dbGoogleReportingService.GoogleAnalyticsTimeZoneInfo);

            //get pages' events
            dimensions = new List<Dimension>() {
                        new Dimension{ Name = "ga:pagePath" },
                        new Dimension { Name = "ga:eventLabel" },
                        new Dimension { Name = "ga:eventCategory" },
                        new Dimension { Name = "ga:eventAction" }
                };
            metrics = new List<Metric>() { new Metric { Expression = "ga:totalEvents" } };

            response = _dbGoogleReportingService.GetReportsResponse(dateRange, dimensions, metrics);
            var events = GoogleDataAdapter.AdaptGoogleReportResults<GAEventTrackingDataModel>(response.Reports.FirstOrDefault(), _dbGoogleReportingService.GoogleAnalyticsTimeZoneInfo);

            foreach (var page in pages)
            {
                page.Events = events.Where(e => e.pagePath == page.pagePath).ToList();
            }

            if (!isFirstTimeRun && entryStart.Date == lastRunTrack.Date)
            {
                var updateMessage = _dbGoogleAnalyticPageTrackingService.UpdateGoogleAnalyticsPageTracking(pages, account.ObjectID);
                summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.UpdateGoogleAnalyticsPage.ToString(), Value = pages.Count().ToString(), Message = updateMessage.Success.ToString() });
            }
            else
            {
                if (pages.Count > 0)
                {
                    var newMessage = _dbGoogleAnalyticPageTrackingService.CreateBatchGoogleAnalyticsPageTracking(pages, account.ObjectID);
                    summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.NewGoogleAnalyticsPage.ToString(), Value = pages.Count().ToString(), Message = newMessage.Success.ToString() });
                }
            }

            return summary;
        }

        protected static bool IsFirstGoogleAnalyticsMinerRun(MinerRunAuditTrailDataModel minerRunAuditTrailDataModel)
        {
            //no details for GoogleAnalytics, so obviously it hasn't run, thus it's the first sharpspring run
            if (!minerRunAuditTrailDataModel.MinerRunDetails.Any(x => x.MinerName == MinerKind.GoogleAnalytics.ToString()))
                return true;

            //get all details for GoogleAnalytics miner runs
            //if any have first miner run complete = true, then it has run, thus it's not the first sharpspring run
            List<MinerRunMinerDetails> minerRunMinerDetails = minerRunAuditTrailDataModel.MinerRunDetails
                .Where(x => x.MinerName == MinerKind.GoogleAnalytics.ToString())
                .ToList();

            if (minerRunMinerDetails.Any(x => x.IsFirstMinerRunComplete))
                return false;

            return true;
        }

        protected static DateTime EstablishStartDateForMinerRun(MinerRunAuditTrailDataModel minerRunAuditTrailDataModel, bool isFirstTimeRun)
        {
            DateTime now = System.DateTime.Now;
            DateTime startDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
            DateTime endDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

            int backFillMonths = Convert.ToInt32(ConfigurationManager.AppSettings["GoogleAnalyticsFirstRunBackfillMonths"]);

            if (isFirstTimeRun || (minerRunAuditTrailDataModel.MinerRunDetails == null) || (minerRunAuditTrailDataModel.MinerRunDetails.Count == 0))
                return startDate.AddMonths(backFillMonths * -1);

            List<MinerRunMinerDetails> minerRunMinerDetails = minerRunAuditTrailDataModel.MinerRunDetails
               .Where(x => x.MinerName == MinerKind.GoogleAnalytics.ToString())
               .ToList();

            DateTime lastEndDateForCollection = minerRunMinerDetails.Max(x => x.LastEndDateForDataCollection);

            if (lastEndDateForCollection < endDate)
                startDate = new DateTime(lastEndDateForCollection.Year, lastEndDateForCollection.Month, lastEndDateForCollection.Day, 0, 0, 0, DateTimeKind.Utc);

            return startDate;
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

        static void AppStart()
        {
            CreateKernel();
        }

        protected static IKernel CreateKernel()
        {
            IKernel kernel = new StandardKernel();

            Assembly.Load("mxtrAutomation.Common");
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
            _dbGoogleReportingService = ServiceLocator.Current.GetInstance<IGoogleReportingService>();
            _dbGoogleAnalyticPageTrackingService = ServiceLocator.Current.GetInstance<IGoogleAnalyticPageTrackingService>();
            _dbErrorLogService = ServiceLocator.Current.GetInstance<IErrorLogService>();
        }
    }
}
