using mxtrAutomation.Api.Bullseye;
using mxtrAutomation.Api.Sharpspring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Services
{
    public interface IBullseyeService
    {
        void SetConnectionTokens(int clientId, string adminApiKey, string searchApiKey);

        List<RestSearchResponseLogDataModel> GetSearchResponseLog(DateTime startDate, DateTime endDate, out int TotalResults);
        List<BullseyeAccountDataModel> SearchBullseyeAccount(BullseyeAccountSearchModel search, out int TotalResults);
        BullseyeAccountDataModel GetBullseyeLocation(BullseyeAccountSearchModel search);
    }

    public class BullseyeService : IBullseyeService
    {
        private readonly IBullseyeApi _bullseyeApi;

        public BullseyeService(IBullseyeApi argBullseyeApi)
        {
            _bullseyeApi = argBullseyeApi;
        }

        public void SetConnectionTokens(int clientId, string adminApiKey, string searchApiKey)
        {
            _bullseyeApi.ClientId = clientId;
            _bullseyeApi.AdminApiKey = adminApiKey;
            _bullseyeApi.SearchApiKey = searchApiKey;
        }

        public List<RestSearchResponseLogDataModel> GetSearchResponseLog(DateTime startDate, DateTime endDate, out int TotalResults) {
            return _bullseyeApi.GetSearchResponseLog(startDate, endDate, out TotalResults);
        }

        public List<BullseyeAccountDataModel> SearchBullseyeAccount(BullseyeAccountSearchModel search, out int TotalResults) {
            return _bullseyeApi.SearchBullseyeAccount(search, out TotalResults);
        }

        public BullseyeAccountDataModel GetBullseyeLocation(BullseyeAccountSearchModel search)
        {
            return _bullseyeApi.GetBullseyeLocation(search);
        }

    }
}
