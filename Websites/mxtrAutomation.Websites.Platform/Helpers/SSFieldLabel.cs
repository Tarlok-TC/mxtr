using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Services;
using System.Collections.Generic;
using mxtrAutomation.Api.Services;
using mxtrAutomation.Api.Sharpspring;
using System.Linq;

namespace mxtrAutomation.Websites.Platform.Helpers
{
    public class SSFieldLabel
    {
        public static void AddSSFieldInEZShred()
        {
            IEZShredFieldLabelMappingService _eZShredFieldLabelMappingService = ServiceLocator.Current.GetInstance<IEZShredFieldLabelMappingServiceInternal>();
            ISharpspringService _apiSharpspringService = ServiceLocator.Current.GetInstance<ISharpspringService>();
            IAccountService _dbAccountService = ServiceLocator.Current.GetInstance<IAccountService>();
            ICRMEZShredService _crmEZShredService = ServiceLocator.Current.GetInstance<ICRMEZShredService>();

            IEnumerable<mxtrAccount> accounts = _dbAccountService.GetAllAccountsWithEZShred();
            List<EZShredFieldLabelMappingDataModel> _eZShredFieldLabelMappingDataModel = _eZShredFieldLabelMappingService.GetAllFieldLabels();//All Field Label from EZShredFieldLabelMapping Collection

            foreach (var account in accounts)
            {
                List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel = GetAllSharpspringCustomFields(_dbAccountService, _apiSharpspringService, _eZShredFieldLabelMappingService, account.ObjectID);//Combine Data From Sharpspring and EZShredFieldLabelMapping Collection
                List<SSField> SSFields = _crmEZShredService.GetAllSSFields(account.ObjectID);//All Field Label from EZShredDATA Collection
                if (ObjSharpspringCustomFieldsDataModel.Count > SSFields.Count)
                {
                    List<SSField> ListSSField = new List<SSField>();
                    foreach (var item in ObjSharpspringCustomFieldsDataModel)
                    {
                        ListSSField.Add(new SSField { EZShredFieldName = item.EZShredFieldName, SSSystemName = item.SSSystemName, Label = item.Label, Type = item.Type, Set = item.Set });

                    }
                    EZShredDataModel objEZShredDataModel = new EZShredDataModel();
                    objEZShredDataModel.SSField = ListSSField;
                    objEZShredDataModel.AccountObjectId = account.ObjectID;
                    _crmEZShredService.AddUpdateData(objEZShredDataModel);
                }
            }
        }
        private static List<SharpspringCustomFieldsDataModel> GetAllSharpspringCustomFields(IAccountService _dbAccountService, ISharpspringService _apiSharpspringService, IEZShredFieldLabelMappingService _eZShredFieldLabelMappingService, string accountObjectID)
        {
            List<EZShredFieldLabelMappingDataModel> _eZShredFieldLabelMappingDataModel = _eZShredFieldLabelMappingService.GetAllFieldLabels();//All Field Label from EZShredFieldLabelMapping Collection
            mxtrAccount account = _dbAccountService.GetAccountByAccountObjectID(accountObjectID);
            _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);
            List<SharpspringFieldLabelDataModel> customFields = _apiSharpspringService.GetCustomFields();//All Custom fields from Sharpspring

            List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel = new List<SharpspringCustomFieldsDataModel>();
            foreach (var item in _eZShredFieldLabelMappingDataModel)
            {
                ObjSharpspringCustomFieldsDataModel.Add(
                    new SharpspringCustomFieldsDataModel
                    {
                        SSSystemName = customFields.Where(e => e.Label == item.Label && e.Type.ToLower() == item.Type.ToLower()).Select(e => e.SSSystemName).FirstOrDefault(),
                        EZShredFieldName = item.EZShredFieldName,
                        Type = item.Type,
                        Label = item.Label,
                        Set = _eZShredFieldLabelMappingDataModel.Where(e => e.Label == item.Label && e.Type == item.Type).Select(e => e.Set).FirstOrDefault()
                    });
            }
            return ObjSharpspringCustomFieldsDataModel;
        }
    }
}