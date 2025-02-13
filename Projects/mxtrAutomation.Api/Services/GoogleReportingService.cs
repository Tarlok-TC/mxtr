using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Configuration;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;

namespace mxtrAutomation.Api.Services
{
    public interface IGoogleReportingService
    {
        string ViewId { get; set; }
        string AccountEmail { get; set; }
        string AccountCredentialFile { get; set; }
        string TimeZoneName { get; set; }
        TimeZoneInfo GoogleAnalyticsTimeZoneInfo { get; }

        void SetConnectionTokens(string ViewId, string AccountEmail, string AccountCredentialFile, string TimeZoneName);
        AnalyticsReportingService AuthenticateServiceAccount();
        AnalyticsReportingService AuthenticateServiceAccountFromKey(string key);
        GetReportsResponse GetReportsResponse(List<DateRange> dateRange, List<Dimension> dimensions, List<Metric> metrics);
    }
    public class GoogleReportingService : IGoogleReportingServiceInternal
    {
        public string ViewId { get; set; }
        public string AccountEmail { get; set; }
        public string AccountCredentialFile { get; set; }
        public string TimeZoneName { get; set; }
        public TimeZoneInfo GoogleAnalyticsTimeZoneInfo
        {
            get
            {
                var timeZoneName = TimeZoneName ?? "Pacific Standard Time";
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.Local;
                try
                {
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                }
                catch (Exception)
                {
                    timeZoneInfo = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(t => t.DisplayName.Contains(timeZoneName) || t.StandardName.Contains(timeZoneName));
                }
                return timeZoneInfo;
            }
        }

        public void SetConnectionTokens(string reportingViewId, string googleAccountEmail, string googleAccountCredentialFile, string accountTimeZoneName)
        {
            ViewId = reportingViewId;
            AccountEmail = googleAccountEmail;
            AccountCredentialFile = googleAccountCredentialFile;
            TimeZoneName = accountTimeZoneName;
        }

        /// <summary>
        /// Authenticating to Google using a Service account
        /// Documentation: https://developers.google.com/accounts/docs/OAuth2#serviceaccount
        /// </summary>
        /// <param name="serviceAccountEmail">From Google Developer console https://console.developers.google.com</param>
        /// <param name="serviceAccountCredentialFilePath">Location of the .p12 or Json Service account key file downloaded from Google Developer console https://console.developers.google.com</param>
        /// <returns>AnalyticsService used to make requests against the Analytics API</returns>
        public AnalyticsReportingService AuthenticateServiceAccount()
        {
            var serviceAccountCredentialFilePath = AppDomain.CurrentDomain.BaseDirectory + AccountCredentialFile;
            var tokenFolderPath = ConfigurationManager.AppSettings["GAFilePath"];
            //var hasAuthTokenFile = (Directory.Exists(tokenFolderPath) == true && Directory.GetFiles(tokenFolderPath, "*").Length > 0) ? true : false; // TODO: Need to get info from database

            try
            {
                if (string.IsNullOrEmpty(serviceAccountCredentialFilePath))
                    throw new Exception("Path to the .p12 service account credentials file is required.");
                //if (!File.Exists(serviceAccountCredentialFilePath) && !hasAuthTokenFile)
                if (!File.Exists(serviceAccountCredentialFilePath) && !File.Exists(tokenFolderPath + AccountCredentialFile))
                    throw new Exception("The service account credentials .p12 file does not exist at: " + serviceAccountCredentialFilePath);
                if (string.IsNullOrEmpty(AccountEmail))
                    throw new Exception("ServiceAccountEmail is required.");

                // These are the scopes of permissions you need. It is best to request only what you need and not all of them
                string[] scopes = new string[] { AnalyticsReportingService.Scope.AnalyticsReadonly, AnalyticsReportingService.Scope.Analytics };             // View your Google Analytics data

                dynamic credential;
                // For Json file
                if (Path.GetExtension(serviceAccountCredentialFilePath).ToLower() == ".json")
                {
                    using (var stream = new FileStream(serviceAccountCredentialFilePath, FileMode.Open, FileAccess.Read))
                    {
                        credential = GoogleCredential.FromStream(stream)
                             .CreateScoped(scopes);
                    }
                }
                else if (Path.GetExtension(serviceAccountCredentialFilePath).ToLower() == ".p12")
                {   // If its a P12 file
                    var certificate = new X509Certificate2(serviceAccountCredentialFilePath, "notasecret", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
                    credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(AccountEmail)
                    {
                        Scopes = scopes
                    }.FromCertificate(certificate));

                }
                else if (File.Exists(tokenFolderPath + AccountCredentialFile))
                {
                    // Using user credentials
                    string userName = AccountCredentialFile.Split('-')[1];
                    //// here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData% 
                    //credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
                    //{
                    //    ClientId = ConfigurationManager.AppSettings["ClientId"],
                    //    ClientSecret = ConfigurationManager.AppSettings["ClientSecret"],
                    //},
                    //    scopes,
                    //    userName,
                    //    System.Threading.CancellationToken.None,
                    //    new Google.Apis.Util.Store.FileDataStore(tokenFolderPath, true)
                    //    ).Result;


                    // New code for handling token
                    string tokenData = File.ReadAllText(tokenFolderPath + AccountCredentialFile);
                    var token = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(tokenData);
                    credential = new UserCredential(new GoogleAuthorizationCodeFlow(
                        new GoogleAuthorizationCodeFlow.Initializer
                        {
                            ClientSecrets = new ClientSecrets
                            {
                                ClientId = ConfigurationManager.AppSettings["ClientId"],
                                ClientSecret = ConfigurationManager.AppSettings["ClientSecret"],
                            },
                            Scopes = new[] { AnalyticsReportingService.Scope.AnalyticsReadonly, AnalyticsReportingService.Scope.Analytics, },
                        }), userName, token);
                }
                else
                {
                    throw new Exception("Unsupported Service accounts credentials.");
                }

                // Create the  Analytics service.
                return new AnalyticsReportingService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Mxtr",
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Create service account AnalyticsService failed" + ex.Message);
                throw new Exception("CreateServiceAccountAnalyticsServiceFailed", ex);
            }
        }

        /// <summary>
        /// Authenticating to Google using a Service account
        /// Documentation: https://developers.google.com/accounts/docs/OAuth2#serviceaccount
        /// </summary>
        /// <param name="serviceAccountEmail">From Google Developer console https://console.developers.google.com</param>
        /// <param name="key">Key from with in json file   
        /// 
        /// Example:
        /// ServiceAccountAuth tmp = JsonConvert.DeserializeObject<ServiceAccountAuth>(File.ReadAllText(serviceAccountJsonPath));
        /// 
        /// </param>
        /// <returns>AnalyticsService used to make requests against the Analytics API</returns>
        public AnalyticsReportingService AuthenticateServiceAccountFromKey(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    throw new Exception("Key is required.");
                if (string.IsNullOrEmpty(AccountEmail))
                    throw new Exception("ServiceAccountEmail is required.");

                // These are the scopes of permissions you need. It is best to request only what you need and not all of them
                string[] scopes = new string[] { AnalyticsReportingService.Scope.Analytics };             // View your Google Analytics data


                var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(AccountEmail)
                {
                    Scopes = scopes
                }.FromPrivateKey(key));


                // Create the  Analytics service.
                return new AnalyticsReportingService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Analytics Authentication Sample",
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine("Create service account AnalyticsService failed" + ex.Message);
                throw new Exception("CreateServiceAccountAnalyticsServiceFailed", ex);
            }
        }

        public GetReportsResponse GetReportsResponse(List<DateRange> dateRange, List<Dimension> dimensions, List<Metric> metrics)
        {
            var analyticsreporting = AuthenticateServiceAccount();
            var reportRequest = new ReportRequest
            {
                ViewId = ViewId,
                DateRanges = dateRange,
                Dimensions = dimensions,
                Metrics = metrics
            };

            var requests = new List<ReportRequest>();
            requests.Add(reportRequest);

            var getReport = new GetReportsRequest() { ReportRequests = requests };
            return analyticsreporting.Reports.BatchGet(getReport).Execute();
        }

    }

    public interface IGoogleReportingServiceInternal : IGoogleReportingService
    {
    }
}
