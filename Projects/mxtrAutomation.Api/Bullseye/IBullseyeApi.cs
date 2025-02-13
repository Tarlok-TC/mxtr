using System;
using System.Collections.Generic;

namespace mxtrAutomation.Api.Bullseye
{
    public interface IBullseyeApi
    {
        int ClientId { get; set; }
        string AdminApiKey { get; set; }
        string SearchApiKey { get; set; }

        List<RestSearchResponseLogDataModel> GetSearchResponseLog(DateTime startDate, DateTime endDate, out int TotalResults);
        List<BullseyeAccountDataModel> SearchBullseyeAccount(BullseyeAccountSearchModel search, out int TotalResults);
        BullseyeAccountDataModel GetBullseyeLocation(BullseyeAccountSearchModel search);
    }
}
