using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Common.Codebase;
using mxtrAutomation.Common.Ioc;
using WebMatrix.WebData;

namespace mxtrAutomation.Websites.Platform.App_Start
{
    public class DatabaseConfig
    {
        public static void Register()
        {
            //WebSecurity.InitializeDatabaseConnection("GetInteractiveDataContext", "hcgMediaUsers", "hcgMediaUserID", "Username", false);
            ////WebSecurity.CreateUserAndAccount("brian.sloyer@hcgmedia.com", "password1", new { ClientID = 1 }, false);
            
            //IConfiguration configuration = ServiceLocator.Current.TryGetInstance<IConfiguration>();
            //if (configuration != null)
            //{
            //    //configuration.ApplyDatabaseMigrations();

            //    //WebSecurity.InitializeDatabaseConnection("GetInteractiveDataContext", "aspnet_Users", "UserID", "Username", false);
            //    //ReseedDatabase();
            //}
        }

        public static void ReseedDatabase()
        {
            //List<ISeedCollection> seedCollections =
            //    ServiceLocator.Current.GetAllInstances<ISeedCollection>().ToList();

            //seedCollections.ForEach(c => c.SystemSeed());

            //IEnvironment environment =
            //    ServiceLocator.Current.GetInstance<IEnvironment>();

            //if (environment.Environment == EnvironmentKind.Development)
            //    seedCollections.ForEach(c => c.DevSeed());
        }
    }
}