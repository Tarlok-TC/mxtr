using GoogleMaps.LocationServices;
using mxtrAutomation.Api.Bullseye;
using mxtrAutomation.Api.Services;
using mxtrAutomation.Bullseye.Adapter;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Common.Utils;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Corporate.Data.Services;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace mxtrAutomation.Bullseye
{
    public class Program
    {
        private static IAccountService _dbAccountService;
        private static IMinerRunService _dbMinerRunService;
        private static IBullseyeService _apiBullseyeService;
        private static IUserService _dbUserService;
        private static IErrorLogService _dbErrorLogService;
        private static ICRMRestSearchResponseLogService _dbCRMRestSearchResponseLogService;

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
                            System.Console.WriteLine("Processing Bullseye data for all accounts...");
                            ProcessBullseyeForAllAccounts();
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
                    LogType = ErrorKind.Miner + " " + CRMKind.Bullseye.ToString(),
                    ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                });
            }
        }

        static void ProcessBullseyeForAllAccounts()
        {
            bool isFirstTimeRun = false;
            List<MinerRunDataCollectionSummary> summary = new List<MinerRunDataCollectionSummary>();

            //var account = _dbAccountService.GetAccountByAccountName("mXtr Automation");
            List<mxtrAccount> accounts = _dbAccountService.GetAllAccountsWithBullseye().ToList();

            foreach (mxtrAccount account in accounts)
            {
                try
                {
                    _apiBullseyeService.SetConnectionTokens(account.BullseyeClientId, account.BullseyeAdminApiKey, account.BullseyeSearchApiKey);

                    MinerRunAuditTrailDataModel lastMinerRunAuditTrailDataModel = _dbMinerRunService.GetLastMinerRunAuditTrailByAccountObjectID(account.ObjectID);

                    DateTime entryStart = System.DateTime.Today;

                    if ((lastMinerRunAuditTrailDataModel == null) || (lastMinerRunAuditTrailDataModel.MinerRunDetails == null) || (IsFirstBullseyeMinerRun(lastMinerRunAuditTrailDataModel)))
                        isFirstTimeRun = true;

                    if (lastMinerRunAuditTrailDataModel != null)
                        entryStart = EstablishStartDateForMinerRun(lastMinerRunAuditTrailDataModel, isFirstTimeRun);

                    DateRange runDates = DateTimeRange.CreateInclusive(entryStart, entryStart.AddDays(1));
                    while (runDates.Start < System.DateTime.Today)
                    {
                        Console.WriteLine(string.Format("Processing data for {0} for {1}", account.AccountName, runDates.Start.ToShortDateString()));
                        summary.Clear();

                        List<MinerRunDataCollectionSummary> searchResponseLog = ProcessResponseLog(account, runDates);
                        summary.AddRange(searchResponseLog);

                        //create miner run details for bullseye
                        MinerRunMinerDetails minerRunMinerDetails = new MinerRunMinerDetails()
                        {
                            MinerName = MinerKind.Bullseye.ToString(),
                            IsFirstMinerRunComplete = true,
                            LastMinerRunTime = System.DateTime.Now,
                            LastStartDateForDataCollection = new DateTime(runDates.Start.Year, runDates.Start.Month, runDates.Start.Day, 0, 0, 0, DateTimeKind.Utc),
                            LastEndDateForDataCollection = new DateTime(runDates.End.Year, runDates.End.Month, runDates.End.Day, 0, 0, 0, DateTimeKind.Utc),
                            Summary = summary
                        };

                        CreateNotificationReturn returnNotification = UpdateMinerRunAuditTrail(account, isFirstTimeRun, minerRunMinerDetails);

                        isFirstTimeRun = false;
                        runDates = DateTimeRange.CreateInclusive(runDates.Start.AddDays(1), runDates.End.AddDays(1));
                    }
                }
                catch (Exception ex)
                {
                    _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                    {
                        LogTime = DateTime.UtcNow,
                        Description = "Error in processing account: " + account.AccountName,
                        LogType = ErrorKind.Miner + " " + CRMKind.Bullseye.ToString(),
                        ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                    });
                }
            }
        }

        protected static bool IsFirstBullseyeMinerRun(MinerRunAuditTrailDataModel minerRunAuditTrailDataModel)
        {
            //no details for bullseye, so obviously it hasn't run, thus it's the first bullseye run
            if (!minerRunAuditTrailDataModel.MinerRunDetails.Any(x => x.MinerName == MinerKind.Bullseye.ToString()))
                return true;

            //get all details for bullseye miner runs
            //if any have first miner run complete = true, then it has run, thus it's not the first bullseye run
            List<MinerRunMinerDetails> minerRunMinerDetails = minerRunAuditTrailDataModel.MinerRunDetails
                .Where(x => x.MinerName == MinerKind.Bullseye.ToString())
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

            int backFillMonths = Convert.ToInt32(ConfigurationManager.AppSettings["BullseyeFirstRunBackfillMonths"]);

            if (isFirstTimeRun || (minerRunAuditTrailDataModel.MinerRunDetails == null) || (minerRunAuditTrailDataModel.MinerRunDetails.Count == 0))
                return startDate.AddMonths(backFillMonths * -1);

            List<MinerRunMinerDetails> minerRunMinerDetails = minerRunAuditTrailDataModel.MinerRunDetails
               .Where(x => x.MinerName == MinerKind.Bullseye.ToString())
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

        protected static List<MinerRunDataCollectionSummary> ProcessResponseLog(mxtrAccount runAccount, DateRange runDates)
        {
            int logsTotal = 0;
            var responseLog = _apiBullseyeService.GetSearchResponseLog(runDates.Start, runDates.End, out logsTotal);

            var summary = new List<MinerRunDataCollectionSummary>();

            string message = string.Empty;
            if (responseLog.Count() > 0)
            {
                List<CRMRestSearchResponseLogModel> crmLogsDataModel = BullseyeApiToDataModelAdapter.AdaptRestSearchResponseLog(responseLog, runDates.End);

                foreach (CRMRestSearchResponseLogModel log in crmLogsDataModel)
                {
                    var account = RetrieveOrCreateAccount(log, runAccount, ref summary);
                    if (account != null)
                    {
                        log.AccountObjectID = account.ObjectID;
                        log.MxtrAccountID = account.MxtrAccountID;
                    }

                }
                var returnNotification = _dbCRMRestSearchResponseLogService.CreateBatchCRMRestSearchResponseLogs(crmLogsDataModel);
                message = returnNotification.Success.ToString();
            }
            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.RestSearchResponseLog.ToString(), Value = logsTotal.ToString(), Message = message });

            return summary;
        }

        protected static mxtrAccount RetrieveOrCreateAccount(CRMRestSearchResponseLogModel logAccount, mxtrAccount parentAccount, ref List<MinerRunDataCollectionSummary> summary)
        {
            BullseyeAccountDataModel bullseyeAccount = _apiBullseyeService.GetBullseyeLocation(new BullseyeAccountSearchModel() { LocationID = logAccount.LocationID });

            //mxtrAccount account = _dbAccountService.GetAccountByBullseyeLocationID(logAccount.LocationID);
            mxtrAccount account = _dbAccountService.GetAccountByBullseyeThirdPartyId(bullseyeAccount.ThirdPartyId);
            if (account != null)
            {
                if (bullseyeAccount.Id == null)
                {
                    return account;
                }
                else
                {
                    //update account data
                    var mxtrAccountToUpdate = BullseyeApiToDataModelAdapter.AdaptAccountForUpdate(bullseyeAccount);
                    var updatedAccount = _dbAccountService.UpdateAccountFromBullsEye(mxtrAccountToUpdate);
                    return updatedAccount;
                }
            }

            if (bullseyeAccount.Id == null)
                return null;

            var mxtrAccount = BullseyeApiToDataModelAdapter.AdaptAccount(bullseyeAccount, parentAccount);
            try
            {
                var address = account.StreetAddress + ", " + account.Country + " " + account.ZipCode;
                var locationService = new GoogleLocationService();
                var point = locationService.GetLatLongFromAddress(address);
                if (point != null)
                {
                    mxtrAccount.Latitude = point.Latitude;
                    mxtrAccount.Longitude = point.Longitude;
                }
            }
            catch (Exception ex)
            {
            }
            var message = _dbAccountService.CreateAccount(mxtrAccount);

            if (message.Success)
            {
                //mxtrAccount = _dbAccountService.GetAccountByBullseyeLocationID(logAccount.LocationID);
                mxtrAccount = _dbAccountService.GetAccountByBullseyeThirdPartyId(bullseyeAccount.ThirdPartyId);
                CreateUserForAccount(bullseyeAccount, mxtrAccount, ref summary);
                summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.NewAccount.ToString(), Value = "1", Message = message.Success.ToString() });
            }

            return mxtrAccount;
        }

        protected static void CreateUserForAccount(BullseyeAccountDataModel bullseyeAccount, mxtrAccount mxtrAccount, ref List<MinerRunDataCollectionSummary> summary)
        {
            var userName = Regex.Replace(bullseyeAccount.Name, "[^0-9a-zA-Z]+", "").ToLower();
            var message = _dbUserService.CreateUser(new mxtrUser
            {
                MxtrUserID = Guid.NewGuid().ToString(),
                MxtrAccountID = mxtrAccount.MxtrAccountID.ToString(),
                AccountObjectID = mxtrAccount.ObjectID,
                FullName = string.Format("{0} Admin", userName),
                Email = string.IsNullOrEmpty(bullseyeAccount.EmailAddress) ? string.Format("{0}@mxtrautomation.com", userName) : bullseyeAccount.EmailAddress,
                UserName = string.Format("{0}@mxtrautomation.com", userName),
                Password = "password",
                Phone = string.IsNullOrEmpty(bullseyeAccount.PhoneNumber) ? "888-888-8888" : bullseyeAccount.PhoneNumber,
                CellPhone = bullseyeAccount.MobileNumber,
                Role = "Admin",
                Permissions = new List<string>()
                    {
                        PermissionKind.ManageAccountUsers.ToString(),
                        PermissionKind.ViewHierarchy.ToString(),
                        PermissionKind.CreateDashboard.ToString(),
                        PermissionKind.ViewDashboard.ToString(),
                        PermissionKind.ViewAnalytics.ToString(),
                        PermissionKind.ViewSales.ToString()
                    },
                CreateDate = DateTime.Now.ToString(),
                IsActive = true,
                IsApproved = true,
                IsLockedOut = false,
                FailedLoginAttempts = 0
            });

            summary.Add(new MinerRunDataCollectionSummary { Name = MinerSummaryKind.NewMxtrUser.ToString(), Value = "1", Message = message.Success.ToString() });
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
            _dbCRMRestSearchResponseLogService = ServiceLocator.Current.GetInstance<ICRMRestSearchResponseLogService>();
            _dbUserService = ServiceLocator.Current.GetInstance<IUserService>();
            _apiBullseyeService = ServiceLocator.Current.GetInstance<IBullseyeService>();
            _dbErrorLogService = ServiceLocator.Current.GetInstance<IErrorLogService>();
        }
    }
}
