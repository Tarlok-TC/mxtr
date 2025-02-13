using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Corporate.Data.Enums;
using System.Text.RegularExpressions;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface ICorporateSSLastRunService
    {
        DateTime GetCorporateSSLastRun(string accountObjectId);
        void AddCorporateSSLastRunDate(string accountObjectId, DateTime lastRun);
    }
    public interface ICorporateSSLastRunServiceInternal : ICorporateSSLastRunService
    {
    }
}
