using System;
using Quartz;
using System.Diagnostics;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Common.Utils;

namespace mxtrAutomation.Websites.Platform.Helpers
{
    public class MinerScheduler : SchedulerHelper, IJob
    {
        //private static ICRMMinerRunScheduleLogService _dbICRMMinerRunScheduleLogService;

        public void Execute(IJobExecutionContext context)
        {
            PrepareServices();
            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();
            bool isRunSuccessfully = false;
            string error = string.Empty;

            // Please don't change order

            // BullEye
            if (!String.IsNullOrEmpty(ConfigManager.AppSettings["BullseyeRunMiner"]) && Convert.ToBoolean(ConfigManager.AppSettings["BullseyeRunMiner"]))
            {
                try
                {
                    AddSchedulerLog(DateTime.Now, new DateTime(), false, "Default Entry", CRMKind.Bullseye, CRMKind.Bullseye.ToString() + " Started");
                    startTime = DateTime.Now;
                    RunMiner(CRMKind.Bullseye);
                    endTime = DateTime.Now;
                    isRunSuccessfully = true;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    endTime = DateTime.Now;
                    isRunSuccessfully = false;
                }
                AddSchedulerLog(startTime, endTime, isRunSuccessfully, error, CRMKind.Bullseye, CRMKind.Bullseye.ToString() + " Ended");
            }

            //SharpSpring
            if (!String.IsNullOrEmpty(ConfigManager.AppSettings["SharpspringRunMiner"]) && Convert.ToBoolean(ConfigManager.AppSettings["SharpspringRunMiner"]))
            {
                try
                {
                    AddSchedulerLog(DateTime.Now, new DateTime(), false, "Default Entry", CRMKind.Sharpspring, CRMKind.Sharpspring.ToString() + " Started");
                    startTime = DateTime.Now;
                    RunMiner(CRMKind.Sharpspring, 120);
                    endTime = DateTime.Now;
                    isRunSuccessfully = true;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    endTime = DateTime.Now;
                    isRunSuccessfully = false;
                }
                AddSchedulerLog(startTime, endTime, isRunSuccessfully, error, CRMKind.Sharpspring, CRMKind.Sharpspring.ToString() + " Ended");
            }

            //google Analytics
            if (!String.IsNullOrEmpty(ConfigManager.AppSettings["GoogleAnalyticsRunMiner"]) && Convert.ToBoolean(ConfigManager.AppSettings["GoogleAnalyticsRunMiner"]))
            {
                try
                {
                    AddSchedulerLog(DateTime.Now, new DateTime(), false, "Default Entry", CRMKind.GoogleAnalytics, CRMKind.GoogleAnalytics.ToString() + " Started");
                    startTime = DateTime.Now;
                    RunMiner(CRMKind.GoogleAnalytics);
                    endTime = DateTime.Now;
                    isRunSuccessfully = true;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    endTime = DateTime.Now;
                    isRunSuccessfully = false;
                }
                AddSchedulerLog(startTime, endTime, isRunSuccessfully, error, CRMKind.GoogleAnalytics, CRMKind.GoogleAnalytics.ToString() + "Ended");
            }

            //Shaw KPI SharpSpring
            if (!String.IsNullOrEmpty(ConfigManager.AppSettings["ShawKPISharpspringRunMiner"]) && Convert.ToBoolean(ConfigManager.AppSettings["ShawKPISharpspringRunMiner"]))
            {
                try
                {
                    AddSchedulerLog(DateTime.Now, new DateTime(), false, "Default Entry", CRMKind.ShawKPIMiner, CRMKind.ShawKPIMiner.ToString() + " Started");
                    startTime = DateTime.Now;
                    RunMiner(CRMKind.ShawKPIMiner);
                    endTime = DateTime.Now;
                    isRunSuccessfully = true;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    endTime = DateTime.Now;
                    isRunSuccessfully = false;
                }
                AddSchedulerLog(startTime, endTime, isRunSuccessfully, error, CRMKind.ShawKPIMiner, CRMKind.ShawKPIMiner.ToString() + " Ended");
            }

        }

        //private static void AddSchedulerLog(DateTime startTime, DateTime endTime, bool isRunSuccessfully, string error, CRMKind minerType, string description)
        //{
        //    _dbICRMMinerRunScheduleLogService.AddMinerRunLog(new Corporate.Data.DataModels.MinerRunScheduleLogDataModel()
        //    {
        //        EndTime = endTime,
        //        IsRunSuccessfully = isRunSuccessfully,
        //        Error = error,
        //        MinerType = minerType.ToString(),
        //        StartTime = startTime,
        //        Desciption = description,
        //    });
        //}

        //private void PrepareServices()
        //{
        //    _dbICRMMinerRunScheduleLogService = ServiceLocator.Current.GetInstance<ICRMMinerRunScheduleLogService>();
        //}


        /// <summary>
        /// Launch the legacy application with some options set.
        /// </summary>
        //static void LaunchCommandLineApp()
        //{
        //    // For the example
        //    const string ex1 = "C:\\";
        //    const string ex2 = "C:\\Dir";

        //    // Use ProcessStartInfo class
        //    ProcessStartInfo startInfo = new ProcessStartInfo();
        //    startInfo.CreateNoWindow = false;
        //    startInfo.UseShellExecute = false;
        //    startInfo.FileName ="dcm2jpg.exe";
        //    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //    //startInfo.Arguments = "test"; //"-f j -o \"" + ex1 + "\" -z 1.0 -s y " + ex2;

        //    try
        //    {
        //        // Start the process with the info we specified.
        //        // Call WaitForExit and then the using statement will close.
        //        using (Process exeProcess = Process.Start(startInfo))
        //        {
        //            exeProcess.WaitForExit();
        //        }
        //    }
        //    catch
        //    {
        //        // Log error.
        //    }
        //}

        //private void RunMiner(CRMKind folder)
        //{
        //    string folderPath = GetDirectory(folder);
        //    string fileName = string.Empty;
        //    string args = string.Empty;
        //    switch (folder)
        //    {
        //        case CRMKind.GoogleAnalytics:
        //            fileName = folderPath + "\\GoogleAnalyticsApp.exe";
        //            args = "GA";
        //            break;
        //        case CRMKind.Bullseye:
        //            fileName = folderPath + "\\mxtrAutomation.Bullseye.exe";
        //            args = "BE";
        //            break;
        //        case CRMKind.Sharpspring:
        //            fileName = folderPath + "\\mxtrAutomation.Sharpspring.exe";
        //            args = "SS";
        //            break;
        //        case CRMKind.EZShred:
        //            fileName = folderPath + "\\mxtrAutomation.EZShred.exe";
        //            args = "EZ";
        //            break;
        //        default:
        //            break;
        //    }
        //    if (System.IO.File.Exists(fileName))
        //    {
        //        var process = Process.Start(fileName, args);
        //        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //        process.WaitForExit(1000 * 60 * 30);// max wait 30 minute
        //    }
        //    else
        //    {
        //        throw new Exception("File " + fileName + " not found");
        //    }
        //}

        //private static string GetDirectory(CRMKind folder)
        //{
        //    string folderPath = AppDomain.CurrentDomain.BaseDirectory + "\\MinerExecutableFiles";
        //    switch (folder)
        //    {
        //        case CRMKind.GoogleAnalytics:
        //            folderPath += "\\GoogleAnalytics";
        //            break;
        //        case CRMKind.Bullseye:
        //            folderPath += "\\BullEye";
        //            break;
        //        case CRMKind.Sharpspring:
        //            folderPath += "\\SharpSpring";
        //            break;
        //        case CRMKind.EZShred:
        //            folderPath += "\\EZShred";
        //            break;
        //        default:
        //            break;
        //    }

        //    if (!System.IO.Directory.Exists(folderPath))
        //    {
        //        System.IO.Directory.CreateDirectory(folderPath);
        //    }
        //    return folderPath;
        //}
    }

    public class EZShredScheduler : SchedulerHelper, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (!String.IsNullOrEmpty(ConfigManager.AppSettings["EZShredRunMiner"]) && Convert.ToBoolean(ConfigManager.AppSettings["EZShredRunMiner"]))
            {

                bool isRunning = false;
                Process[] pname = Process.GetProcessesByName("mxtrAutomation.EZShred");
                if (pname.Length == 0)
                {
                    isRunning = false;
                }
                else
                {
                    isRunning = true;
                }

                if (!isRunning)
                {

                    PrepareServices();
                    DateTime startTime = new DateTime();
                    DateTime endTime = new DateTime();
                    bool isRunSuccessfully = false;
                    string error = string.Empty;
                    string ezShredMinerStopAfter = string.IsNullOrEmpty(Convert.ToString(ConfigManager.AppSettings["EZShredMinerStopAfter"])) ? "180" : ConfigManager.AppSettings["EZShredMinerStopAfter"];
                    //EZShred
                    try
                    {
                        AddSchedulerLog(DateTime.Now, new DateTime(), false, "Default Entry", CRMKind.EZShred, CRMKind.EZShred.ToString() + " Started");
                        startTime = DateTime.Now;
                        RunMiner(CRMKind.EZShred, Convert.ToInt32(ezShredMinerStopAfter));
                        endTime = DateTime.Now;
                        isRunSuccessfully = true;
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                        endTime = DateTime.Now;
                        isRunSuccessfully = false;
                    }
                    AddSchedulerLog(startTime, endTime, isRunSuccessfully, error, CRMKind.EZShred, CRMKind.EZShred.ToString() + "Ended");
                }
            }
        }
    }

    public abstract class SchedulerHelper
    {
        private static ICRMMinerRunScheduleLogService _dbICRMMinerRunScheduleLogService;

        public static void AddSchedulerLog(DateTime startTime, DateTime endTime, bool isRunSuccessfully, string error, CRMKind minerType, string description)
        {
            _dbICRMMinerRunScheduleLogService.AddMinerRunLog(new Corporate.Data.DataModels.MinerRunScheduleLogDataModel()
            {
                EndTime = endTime,
                IsRunSuccessfully = isRunSuccessfully,
                Error = error,
                MinerType = minerType.ToString(),
                StartTime = startTime,
                Desciption = description,
            });
        }

        public static void PrepareServices()
        {
            _dbICRMMinerRunScheduleLogService = ServiceLocator.Current.GetInstance<ICRMMinerRunScheduleLogService>();
        }

        /// <summary>
        /// Function to run miner exe
        /// </summary>
        /// <param name="folder">Physical path from here to get exe file</param>
        /// <param name="waitForExitDuration">max time duration for which exe will run. After that it will forcefully closed</param>
        public static void RunMiner(CRMKind folder, int waitForExitDuration = 0)
        {
            string folderPath = GetDirectory(folder);
            string fileName = string.Empty;
            string args = string.Empty;
            switch (folder)
            {
                case CRMKind.GoogleAnalytics:
                    fileName = folderPath + "\\GoogleAnalyticsApp.exe";
                    args = "GA";
                    break;
                case CRMKind.Bullseye:
                    fileName = folderPath + "\\mxtrAutomation.Bullseye.exe";
                    args = "BE";
                    break;
                case CRMKind.Sharpspring:
                    fileName = folderPath + "\\mxtrAutomation.Sharpspring.exe";
                    args = "SS";
                    break;
                case CRMKind.EZShred:
                    fileName = folderPath + "\\mxtrAutomation.EZShred.exe";
                    args = "EZ";
                    break;
                case CRMKind.ShawKPIMiner:
                    fileName = folderPath + "\\mxtrAutomation.ShawKPI.Sharpspring.exe";
                    args = "SKSS";
                    break;
                default:
                    break;
            }
            if (System.IO.File.Exists(fileName))
            {
                var process = Process.Start(fileName, args);
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                int exitDuration = waitForExitDuration == 0 ? 30 : waitForExitDuration;
                if (folder == CRMKind.EZShred)
                {
                    if (!process.WaitForExit(1000 * 60 * exitDuration))
                    {
                        process.Kill();
                    }
                }
                else
                {
                    process.WaitForExit(1000 * 60 * exitDuration);// max wait 30 minute
                }

            }
            else
            {
                throw new Exception("File " + fileName + " not found");
            }
        }

        public static string GetDirectory(CRMKind folder)
        {
            string folderPath = AppDomain.CurrentDomain.BaseDirectory + "\\MinerExecutableFiles";
            switch (folder)
            {
                case CRMKind.GoogleAnalytics:
                    folderPath += "\\GoogleAnalytics";
                    break;
                case CRMKind.Bullseye:
                    folderPath += "\\BullEye";
                    break;
                case CRMKind.Sharpspring:
                    folderPath += "\\SharpSpring";
                    break;
                case CRMKind.EZShred:
                    folderPath += "\\EZShred";
                    break;
                case CRMKind.ShawKPIMiner:
                    folderPath += "\\ShawKPISharpSpring";
                    break;
                default:
                    break;
            }

            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            return folderPath;
        }
    }
}