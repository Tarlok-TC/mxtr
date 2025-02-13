using mxtrAutomation.Api.Services;
using mxtrAutomation.Api.Sharpspring;
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
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.ShawKPI.Sharpspring
{
    public class Program
    {
        private static IAccountService _dbAccountService;
        private static ISharpspringService _apiSharpspringService;
        private static IShawLeadDetailService _shawLeadDetailService;
        private static IShawListBasedLeadService _shawListBasedLeadService;
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
                            System.Console.WriteLine("Processing Shaw data...");
                            ProcessShawData();
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
                    LogType = ErrorKind.Miner + " " + CRMKind.ShawKPIMiner.ToString(),
                    ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                });
            }
        }

        static void ProcessShawData()
        {
            //string accountObjectId = ConfigurationManager.AppSettings["ShawAccountObjectID"];
            var accountsWithShawFunnelListId = _dbAccountService.GetAllAccountWithShawFunnelListId();
            foreach (var accountShawFunnelListId in accountsWithShawFunnelListId)
            {
                //List<mxtrAccount> dealerAccounts = _dbAccountService.GetFlattenedChildAccounts(accountObjectId);
                List<mxtrAccount> dealerAccounts = _dbAccountService.GetFlattenedChildAccountsCoordinates(accountShawFunnelListId.ObjectID);
                _apiSharpspringService.SetConnectionTokens(accountShawFunnelListId.SharpspringSecretKey, accountShawFunnelListId.SharpspringAccountID);
                SharpspringGetActiveListDataModel result = _apiSharpspringService.GetActiveLists(Convert.ToString(accountShawFunnelListId.SharpSpringShawFunnelListID));
                _shawListBasedLeadService.AddUpdateShawListBasedData(AdaptShawList(result, accountShawFunnelListId.ObjectID, accountShawFunnelListId.MxtrAccountID, accountShawFunnelListId.SharpSpringShawFunnelListID));

                foreach (mxtrAccount account in dealerAccounts)
                {
                    try
                    {
                        _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);
                        DateTime today = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0, DateTimeKind.Utc);
                        int backFillDays = String.IsNullOrEmpty(ConfigurationManager.AppSettings["ShawDataFirstRunBackfillDays"]) ? 1 : Convert.ToInt32(ConfigurationManager.AppSettings["ShawDataFirstRunBackfillDays"]);
                        DateTime startDate = today.AddDays(-backFillDays);
                        DateTime? lastRun = _shawLeadDetailService.GetLastRunDate(account.ObjectID, CRMKind.ShawKPIMiner);
                        if (lastRun != null)
                        {
                            startDate = Convert.ToDateTime(lastRun).Date;
                        }

                        while (startDate <= today)
                        {
                            Console.WriteLine(string.Format("Processing data for {0} for {1}", account.AccountName, startDate.ToShortDateString()));
                            DateTime endDate = startDate.AddDays(1);
                            List<Api.Sharpspring.SharpspringLeadDataModel> newLeads = _apiSharpspringService.GetNewLeadsForDateRange(startDate, endDate);
                            List<Api.Sharpspring.SharpspringLeadDataModel> updatedLeads = _apiSharpspringService.GetUpdatedLeadsForDateRange(startDate, endDate);
                            newLeads.RemoveAll(x => updatedLeads.Select(y => y.LeadID).Contains(x.LeadID));
                            newLeads.AddRange(updatedLeads);
                            if (newLeads.Count > 0)
                            {
                                _shawLeadDetailService.AddLeadAnalyticData(AdaptLeads(newLeads, account.ObjectID, account.MxtrAccountID, startDate), startDate);
                            }
                            else
                            {
                                // Update log entry in ShawListBasedLeadAnalytics collection
                                _shawLeadDetailService.AddLeadAnalyticData(new LeadAnalyticDataModel()
                                {
                                    AccountObjectID = account.ObjectID,
                                    LeadId = 0,
                                    LeadScore = 0,
                                    MxtrAccountID = account.MxtrAccountID,
                                    CRMKind = CRMKind.ShawKPIMiner.ToString(),
                                    CreatedOnMXTR = DateTime.UtcNow,
                                    CreatedDate = startDate,
                                }, startDate);
                            }
                            startDate = startDate.AddDays(1);
                        }
                    }
                    catch (Exception ex)
                    {

                        _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                        {
                            LogTime = DateTime.UtcNow,
                            Description = "Error in processing account: " + account.AccountName,
                            LogType = ErrorKind.Miner + " " + CRMKind.ShawKPIMiner.ToString(),
                            ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                        });
                    }
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
            _apiSharpspringService = ServiceLocator.Current.GetInstance<ISharpspringService>();
            _shawLeadDetailService = ServiceLocator.Current.GetInstance<IShawLeadDetailService>();
            _shawListBasedLeadService = ServiceLocator.Current.GetInstance<IShawListBasedLeadService>();
            _dbErrorLogService = ServiceLocator.Current.GetInstance<IErrorLogService>();
        }

        private static List<LeadAnalyticDataModel> AdaptLeads(List<Api.Sharpspring.SharpspringLeadDataModel> leads, string accountObjectID, string mxtrAccountID, DateTime createdDate)
        {
            return leads
                    .Select(data => new LeadAnalyticDataModel()
                    {
                        AccountObjectID = accountObjectID,
                        LeadId = data.LeadID,
                        //LeadScore = data.LeadScore,
                        LeadScore = data.LeadScore > 0 ? data.LeadScore : data.LeadScoreWeighted,
                        MxtrAccountID = mxtrAccountID,
                        CRMKind = CRMKind.ShawKPIMiner.ToString(),
                        CreatedOnMXTR = DateTime.UtcNow,
                        CreatedDate = createdDate,
                    }).ToList();
        }

        private static ShawListBasedDataModel AdaptShawList(SharpspringGetActiveListDataModel result, string accountObjectID, string mxtrAccountID, long sharpSpringListID)
        {
            return new ShawListBasedDataModel()
            {
                AccountObjectID = accountObjectID,
                MxtrAccountID = mxtrAccountID,
                SharpSpringListID = sharpSpringListID,
                Name = result.Name,
                MemberCount = result.MemberCount,
                RemovedCount = String.IsNullOrEmpty(result.RemovedCount) ? 0 : Convert.ToInt32(result.RemovedCount),
                Description = result.Description,
                CreatedOnMXTR = DateTime.UtcNow,
                CreateTimestamp = Convert.ToDateTime(result.CreateTimestamp),
                CRMKind = CRMKind.ShawKPIMiner.ToString(),
            };
        }
    }
}
