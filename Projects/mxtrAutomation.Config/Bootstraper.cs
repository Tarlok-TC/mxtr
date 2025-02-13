using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace mxtrAutomation.Config
{
    public class Bootstraper
    {
        private static string ConfigSetting(string name)
        {
            return Config.ResourceManager.GetString(String.Format("{0}_{1}", Environment.MachineName, name));
        }

        public static void Boot(System.Web.HttpApplication context, string appName)
        {
            // some component creates the dynamic types assembly, that crashes impleo
            // but if we new it up before loading all the assembiles it doesn't crash on
            // static init

            InitConfigManager(appName);
        }

        public static void InitConfigManager(string appName)
        {
            var config = new StaticDictConfigProvider();
            BootstraperConfigManager.Init(config);
            config.Set(typeof(string), "appName", appName);

            //---- Common keys ----
            config.Set(typeof(string), "MongoServerSettings", ConfigSetting("MongoServerSettings"));
            config.Set(typeof(string), "Environment", ConfigSetting("Environment"));

            //--- Website platform keys ----
            config.Set(typeof(string), "GAFilePath", ConfigSetting("GAFilePath"));
            config.Set(typeof(string), "MinnerRunTimeZone", ConfigSetting("MinnerRunTimeZone"));
            config.Set(typeof(string), "ClientId", ConfigSetting("ClientId"));
            config.Set(typeof(string), "ClientSecret", ConfigSetting("ClientSecret"));
            config.Set(typeof(string), "AmazoneS3AccessKey", ConfigSetting("AmazoneS3AccessKey"));
            config.Set(typeof(string), "AmazoneS3SecretAccessKey", ConfigSetting("AmazoneS3SecretAccessKey"));
            config.Set(typeof(string), "AmazoneS3BucketName", ConfigSetting("AmazoneS3BucketName"));
            config.Set(typeof(string), "AmazoneS3ServiceURL", ConfigSetting("AmazoneS3ServiceURL"));

            //---- Bullseye miner keys ----
            config.Set(typeof(string), "BullseyeBaseUrl", ConfigSetting("BullseyeBaseUrl"));
            config.Set(typeof(string), "BullseyeFirstRunBackfillMonths", ConfigSetting("BullseyeFirstRunBackfillMonths"));

            //---- Sharpspring miner keys
            config.Set(typeof(string), "SharpspringBaseUrl", ConfigSetting("SharpspringBaseUrl"));
            config.Set(typeof(string), "SharpspringFirstRunBackfillMonths", ConfigSetting("SharpspringFirstRunBackfillMonths"));
            config.Set(typeof(string), "DoProcessEmail", ConfigSetting("DoProcessEmail"));

            //---- GA miner keys ----
            config.Set(typeof(string), "GoogleAnalyticsFirstRunBackfillMonths", ConfigSetting("GoogleAnalyticsFirstRunBackfillMonths"));
            config.Set(typeof(string), "GAMinerFilePath", ConfigSetting("GAMinerFilePath"));
        }

        public static void Boot(System.Web.HttpApplication context, string appName, Action<RouteCollection> routes)
        {
            if (routes != null)
                routes(RouteTable.Routes);
            Boot(context, appName);
        }
    }


    public static class BootstraperConfigManager
    {
        private static IConfigProvider _instance;
        public static void Init(IConfigProvider instance)
        {
            if (_instance != null)
                throw new ArgumentException("Already initialized");
            _instance = instance;
        }

        public static string Get(string name)
        {
            return Get<string>(name);
        }

        public static T Get<T>()
        {
            return Get<T>(null);
        }

        public static T Get<T>(string name)
        {
            if (_instance == null)
                throw new ArgumentException("Not initialized");
            object tmp = _instance.Get(typeof(T), name);

            if (tmp != null && typeof(T).IsAssignableFrom(tmp.GetType()))
                return (T)tmp;
            else
                return default(T);
        }
    }
}
