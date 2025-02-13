using System;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Common.Utils;
using mxtrAutomation.Web.Common;
using mxtrAutomation.Web.Common.Authentication;
using mxtrAutomation.Web.Common.Ioc;
using mxtrAutomation.Websites.Platform.App_Start;
using Ninject;
using Quartz;
using Quartz.Impl;
using mxtrAutomation.Websites.Platform.Helpers;
using mxtrAutomation.Corporate.Data.Enums;
using System.Threading;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Services;
using System.Net;

namespace mxtrAutomation.Websites.Platform
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : mxtrAutomationHttpApplication
    {
        protected void Application_Start()
        {
            DependencyResolver.SetResolver(new NinjectDependencyResolver(CreateKernel()));

            AreaRegistration.RegisterAllAreas();

           // WebApiConfig.Register(GlobalConfiguration.Configuration);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //Config.Bootstraper.Boot(this, "mxtrAutomation", RouteConfig.RegisterRoutes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            DatabaseConfig.Register();
           
            //-----------------

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            TimeZoneInfo targetZone = TimeZoneInfo.FindSystemTimeZoneById(ConfigManager.AppSettings["MinnerRunTimeZone"]);

            string ezShredMinerRunInterval = string.IsNullOrEmpty(Convert.ToString(ConfigManager.AppSettings["EZShredMinerRunInterval"])) ? "45" : ConfigManager.AppSettings["EZShredMinerRunInterval"];

            //---EZShred Schedular-----
            IJobDetail job = JobBuilder.Create<EZShredScheduler>().Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInMinutes(Convert.ToInt32(ezShredMinerRunInterval))
                     .InTimeZone(targetZone)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(Convert.ToInt16(0), Convert.ToInt16(0)))   // Specify Time in Hours and Mins
                  )
                .Build();
            scheduler.ScheduleJob(job, trigger);
            //-------------

            // Run scheduler multiple times in a day
            string strMinnerRunMultipleTimes = ConfigManager.AppSettings["MinnerRunMultipleTimes"];
            var multipleTimeJob = JobBuilder.Create<MinerScheduler>().StoreDurably(true).WithIdentity("MinerScheduler_Job").Build();
            scheduler.AddJob(multipleTimeJob, false);
            if (!string.IsNullOrEmpty(strMinnerRunMultipleTimes))
            {
                string[] arrayTimes = strMinnerRunMultipleTimes.Split(',');

                for (int i = 0; i < arrayTimes.Length; i++)
                {
                    try
                    {
                        int hours = Convert.ToInt16(arrayTimes[i].Split(':')[0]);
                        int minutes = Convert.ToInt16(arrayTimes[i].Split(':')[1]);
                        var triggerBuilder = TriggerBuilder.Create()
                                                       .ForJob(multipleTimeJob)
                                                       .WithIdentity("trigger" + i)
                                                       .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hours, minutes).InTimeZone(targetZone))
                                                       .Build();
                        scheduler.ScheduleJob(triggerBuilder);
                    }
                    catch (Exception ex)
                    {
                        Helper.WriteErrorLog(ex, Server.MapPath("~/SchedulerLog.txt"));
                    }
                }
            }

            //Check for Default Menu master data
            MenuData.CheckForMenuMasterData();

            // Check for EZShredFieldLabel Data
            EZShredFieldLabel.CheckForEZShredFieldLabelData();

            // Add SSField In EZShredData
            SSFieldLabel.AddSSFieldInEZShred();
        }

        protected IKernel CreateKernel()
        {
            IKernel kernel = new StandardKernel();

            // The first time the web starts up, all assemblies are loaded into the app pool.
            // However, if the web.config is modified, all assemblies are unloaded, and will
            // not appear again, even if the website is restarted, until those libraries
            // are called.  We call Assembly.Load() on the AdTrak360 assemblies here to ensure
            // that they are loaded.

            Assembly.Load("mxtrAutomation.Common");
            Assembly.Load("mxtrAutomation.Web.Common");
            Assembly.Load("mxtrAutomation.Data");
            Assembly.Load("mxtrAutomation.Corporate.Data");

            kernel.Load(AppDomain.CurrentDomain.GetAssemblies());

            NinjectServiceLocator locator = new NinjectServiceLocator(kernel);
            ServiceLocator.SetLocatorProvider(() => locator);

            return kernel;
        }


        protected void Application_Error()
        {
            HttpContext httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                var serverError = Server.GetLastError() as HttpException;
                if (serverError != null)
                {
                    if (serverError.GetHttpCode() == 404)
                    {
                        Server.ClearError();
                        httpContext.Response.Redirect(new Queries.PageNotFoundWebQuery());
                        return;
                    }
                }

                Exception filterContext = Server.GetLastError().GetBaseException();
                if (filterContext is ThreadAbortException)
                    return;

                IErrorLogService ErrorLogService = ServiceLocator.Current.GetInstance<IErrorLogService>();

                ErrorLogService.CreateErrorLog(new ErrorLogModel
                {
                    LogTime = DateTime.UtcNow,
                    Description = "Application Error",
                    LogType = ErrorKind.Website.ToString(),
                    ErrorMessage = filterContext != null ? filterContext.Message + filterContext.StackTrace + filterContext.Source : null,
                });

                // Hack for handling Google Oauth Deny click
                if (httpContext.Request.Path.Contains("AuthCallback/IndexAsync"))
                {
                    string state = httpContext.Request.QueryString["state"];
                    if (!string.IsNullOrEmpty(state))
                    {
                        string accounObjectId = state.Substring(state.LastIndexOf("/") + 1, 24);
                        string redirect = new Queries.AdminEditAccountWebQuery()
                        {
                            AccountObjectID = accounObjectId,
                        };
                        httpContext.Response.Redirect(redirect);
                    }
                }
            }
        }
    }
}