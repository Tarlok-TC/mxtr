using System.Collections.Generic;
using mxtrAutomation.Corporate.Data.DataModels;
using System;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface IAccountService
    {
        CreateNotificationReturn CreateAccount(mxtrAccount account);
        CreateNotificationReturn UpdateAccount(mxtrAccount accountData);
        mxtrAccount UpdateAccountFromBullsEye(mxtrAccount accountData);
        CreateNotificationReturn CreateAccountAttributes(mxtrAccount account);
        CreateNotificationReturn UpdateAccountAttributes(mxtrAccount accountData);
        CreateNotificationReturn updateApplicationLogo(string accountObjectId, string ApplicationLogoURL);
        CreateNotificationReturn updateBrandingLogo(string accountObjectId, string BrandingLogoURL);
        CreateNotificationReturn updateFavIcon(string accountObjectId, string FavIconURL);
        mxtrAccount GetAccountByAccountObjectID(string accountObjectID);
        mxtrAccount GetBrandingLogoURL(string subDomainName);
        IEnumerable<mxtrAccount> GetAccountsByParentAccountObjectID(string accountObjectID);
        IEnumerable<mxtrAccount> GetAccountHeirarchy(string accountObjectID);
        IEnumerable<mxtrAccount> GetAllAccounts();
        List<string> GetFlattenedChildAccountObjectIDs(string accountObjectID);
        List<string> GetAllFlattenedChildAccountObjectIDs(string accountObjectID);
        List<mxtrAccount> GetAccountsByAccountObjectIDs(List<string> accountObjectIDs);
        List<mxtrAccount> GetAllAccountsByAccountObjectIDs(List<string> accountObjectIDs);
        IEnumerable<mxtrAccount> GetAccountsByAccountObjectIDsAsIEnumberable(List<string> accountObjectIDs);
        mxtrAccount GetAccountByAccountName(string accountName);
        mxtrAccount GetAccountMiniByAccountObjectId(string accountName);
        mxtrAccount GetAccountByDealerId(string dealerId, string parentAccountObjectId);
        IEnumerable<mxtrAccount> GetAllAccountsWithBullseye();
        IEnumerable<mxtrAccount> GetAllAccountsWithSharpspring();
        List<mxtrAccount> GetFlattenedChildAccounts(string accountObjectID);
        List<mxtrAccount> GetFlattenedAccountsForMoving(string loggedInAccountObjectID, string editingAccountObjectID);
        mxtrAccount GetAccountByBullseyeLocationID(int bullseyeLocationID);
        mxtrAccount GetAccountByBullseyeThirdPartyId(string thirdPartyId);
        IEnumerable<mxtrAccount> GetAllAccountsWithGoogle();
        mxtrAccount GetAccountByMxtrAccountId(string mxtrAccountId);
        int GetTotalRetailers(List<string> accountObjectIDs);
        bool IsAccountActive(string mxtrAccountId);
        IEnumerable<mxtrAccount> GetFlattenedChildAccounts_Client(string accountObjectID);
        List<EZShredAccountDataModel> GetFlattenedChildClientAccounts(string accountObjectID);
        List<mxtrAccount> GetFlattenedChildAccountsCoordinates(string accountObjectID);
        CreateNotificationReturn AddUpdateDomainName(string accountObjectId, string domainName);
        CreateNotificationReturn AddUpdateDomainName(Dictionary<string, string> dicAccounts);
        IEnumerable<mxtrAccount> GetAllAccountsWithOrganization();
        IEnumerable<mxtrAccount> GetAllAccountsWithEZShred();
        IEnumerable<mxtrAccount> GetAllAccountWithShawFunnelListId();
        List<EZShredAccountDataModel> GetEzshredByAccountObjectIds(List<string> accountObjectIDs, mxtrUser mxtrUser);
        CreateNotificationReturn AddUpdateHomePageUrl(string accountObjectId, string homePageUrl);
        string GetHomePageUrl(string accountObjectId);
        bool SetOpportunityPipeLine();
        CreateNotificationReturn AssignCoordinates(string accountObjectId, double latitude, double longitude);
        List<mxtrAccount> GetAccountsCoordinates(List<string> accountObjectID);
        List<string> GetFlattenedChildAccountObjectIDsWithGroupClients(string accountObjectID);
        List<Tuple<DateTime, int>> GetParticipatingdealerdata(string accountObjectID, DateTime endDate, int numberOfDays);
    }

    public interface IAccountServiceInternal : IAccountService
    {
    }
}
