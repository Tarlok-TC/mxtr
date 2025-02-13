using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class CRMEZShredCustomerService : MongoRepository<EZShredCustomerData>, ICRMEZShredCustomerServiceInternal
    {
        public bool AddUpdateCustomerData(EZShredCustomerDataModel customerData)
        {
            using (MongoRepository<EZShredCustomerData> repo = new MongoRepository<EZShredCustomerData>())
            {
                try
                {
                    EZShredCustomerData customer = repo.FirstOrDefault(a => a.AccountObjectId == customerData.AccountObjectId && a.MxtrAccountId == customerData.MxtrAccountId && a.CustomerID == customerData.CustomerID);
                    if (customer == null)
                    {
                        customer = GetCustomerDataEntity(customerData);
                        customer.CreatedOn = DateTime.UtcNow;
                        customer.AccountObjectId = customerData.AccountObjectId;
                        customer.MxtrAccountId = customerData.MxtrAccountId;
                        repo.Add(customer);
                    }
                    else
                    {
                        customer = GetCustomerDataEntity(customerData, customer.Id);
                        customer.ModifiedOn = DateTime.UtcNow;
                        customer.CreatedOn = customer.CreatedOn;
                        customer.AccountObjectId = customer.AccountObjectId;
                        customer.MxtrAccountId = customer.MxtrAccountId;
                        repo.Update(customer);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public bool AddUpdateCustomerData(List<EZShredCustomerDataModel> lstCustomerData, string accountObjectId, string mxtrAccountId)
        {
            if (lstCustomerData != null && lstCustomerData.Count > 0)
            {
                lstCustomerData.ForEach(x => { x.AccountObjectId = accountObjectId; x.MxtrAccountId = mxtrAccountId; });
                using (MongoRepository<EZShredCustomerData> repo = new MongoRepository<EZShredCustomerData>())
                {
                    try
                    {
                        List<EZShredCustomerData> entry = repo.Where(a => a.AccountObjectId == accountObjectId && a.MxtrAccountId == mxtrAccountId).ToList();
                        List<EZShredCustomerData> data = new List<EZShredCustomerData>();
                        foreach (var customer in lstCustomerData)
                        {
                            EZShredCustomerData customersData = entry.FirstOrDefault(x => x.CustomerID == customer.CustomerID && x.AccountObjectId == customer.AccountObjectId && x.MxtrAccountId == customer.MxtrAccountId);
                            if (customersData == null)
                            {
                                EZShredCustomerData customerToAdd = GetCustomerDataEntity(customer);
                                customerToAdd.CreatedOn = DateTime.UtcNow;
                                customerToAdd.AccountObjectId = accountObjectId;
                                customerToAdd.MxtrAccountId = mxtrAccountId;
                                data.Add(customerToAdd);
                            }
                            else
                            {
                                EZShredCustomerData customerToUpdate = GetCustomerDataEntity(customer, customersData.Id);
                                customerToUpdate.ModifiedOn = DateTime.UtcNow;
                                customerToUpdate.CreatedOn = customersData.CreatedOn;
                                customerToUpdate.AccountObjectId = customersData.AccountObjectId;
                                customerToUpdate.MxtrAccountId = customersData.MxtrAccountId;
                                data.Add(customerToUpdate);
                            }
                        }
                        repo.Update(data);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Insert/Update Customer from EZShredData to EZShredCustomerData table in database
        /// This is to handle old data migration only. 
        /// Do not use this function else where This is only one time utility
        /// </summary>
        /// <returns>true/false</returns>
        public bool InsertUpdateCustomer()
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                var data = repo.ToList();
                foreach (var item in data)
                {
                    if (item.Customers != null)
                    {
                        List<EZShredCustomerDataModel> lstCustomers = GetCustomerDataModel(item.Customers, item.AccountObjectId, item.MxtrAccountId);
                        if (lstCustomers != null && lstCustomers.Count > 0)
                        {
                            AddUpdateCustomerData(lstCustomers, item.AccountObjectId, item.MxtrAccountId);
                        }
                    }
                }
            }
            return true;
        }
        //Temp function delete after testing
        public int GetCustomerCountInEZShredTable()
        {
            int records = 0;
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                var data = repo.ToList();
                foreach (var item in data)
                {
                    if (item.Customers != null)
                    {
                        List<EZShredCustomerDataModel> lstCustomers = GetCustomerDataModel(item.Customers, item.AccountObjectId, item.MxtrAccountId);
                        if (lstCustomers != null && lstCustomers.Count > 0)
                        {
                            records += lstCustomers.Count;
                        }
                    }
                }
            }
            return records;
        }
        public IEnumerable<EZShredCustomerDataModel> GetAllCustomerByAccountObjectId(string accountObjectId)
        {
            using (MongoRepository<EZShredCustomerData> repo = new MongoRepository<EZShredCustomerData>())
            {
                IEnumerable<EZShredCustomerData> customers = repo.Where(a => a.AccountObjectId == accountObjectId).OrderBy(o => o.Company);
                return GetCustomerDataModel(customers);
            }
        }
        public IEnumerable<EZShredCustomerDataModelMini> GetAllCustomerMiniByAccountObjectId(string accountObjectId)
        {
            using (MongoRepository<EZShredCustomerData> repo = new MongoRepository<EZShredCustomerData>())
            {
                return repo.Where(a => a.AccountObjectId == accountObjectId).Select(x => new EZShredCustomerDataModelMini
                {
                    Company = x.Company,
                    Street = x.Street,
                    Zip = x.Zip,
                    LeadID = x.LeadID,
                    CustomerID = x.CustomerID,
                    OpportunityID = x.OpportunityID,
                    AccountObjectId = x.AccountObjectId,
                    AllowZeroInvoices = x.AllowZeroInvoices,
                    ARCustomerCode = x.ARCustomerCode,
                });//.OrderBy(o => o.Company);
                //return GetCustomerMiniDataModel(customers);
            }
        }
        public IEnumerable<EZShredCustomerDataModelMini> GetAllCustomerMiniByCustomerId(string accountObjectId, string customerId)
        {
            using (MongoRepository<EZShredCustomerData> repo = new MongoRepository<EZShredCustomerData>())
            {
                return repo.Where(a => a.CustomerID == customerId && a.AccountObjectId == accountObjectId).Select(x => new EZShredCustomerDataModelMini
                {
                    Company = x.Company,
                    Street = x.Street,
                    Zip = x.Zip,
                    LeadID = x.LeadID,
                    CustomerID = x.CustomerID,
                    OpportunityID = x.OpportunityID,
                    AccountObjectId = x.AccountObjectId,
                    AllowZeroInvoices = x.AllowZeroInvoices,
                    ARCustomerCode = x.ARCustomerCode,
                });//.OrderBy(o => o.Company);
                //return GetCustomerMiniDataModel(customers);
            }
        }
        public IEnumerable<EZShredCustomerDataModelMini> SearchCustomer(string accountObjectId, string searchCompany)
        {
            using (MongoRepository<EZShredCustomerData> repo = new MongoRepository<EZShredCustomerData>())
            {
                return repo.Where(a => a.AccountObjectId == accountObjectId &&
                 a.Company.ToUpper().Contains(searchCompany.ToLower())).Select(x => new EZShredCustomerDataModelMini
                 {
                     Company = x.Company,
                     Street = x.Street,
                     Zip = x.Zip,
                     LeadID = x.LeadID,
                     CustomerID = x.CustomerID,
                     OpportunityID = x.OpportunityID,
                     AccountObjectId = x.AccountObjectId,
                     AllowZeroInvoices = x.AllowZeroInvoices,
                     ARCustomerCode = x.ARCustomerCode,
                 }); //.OrderBy(o => o.Company);
                //return GetCustomerMiniDataModel(customers);
            }
        }
        public bool DeleteDuplicateCustomerRecords()
        {
            try
            {
                using (MongoRepository<EZShredCustomerData> repo = new MongoRepository<EZShredCustomerData>())
                {
                    //Get all records will be used for filtering
                    var records = repo.ToList();

                    //Get duplicate records
                    var duplicateCustomers = (from ssi in records
                                              group ssi by new { ssi.AccountObjectId, ssi.CustomerID } into g
                                              select new
                                              {
                                                  AccountObjectId = g.Key.AccountObjectId,
                                                  BuildingID = g.Key.CustomerID,
                                                  Count = g.Count()
                                              }).Where(w => w.Count > 1).ToList();

                    // make empty list and add records to be deleted
                    List<EZShredCustomerData> lstRecordsToDelete = new List<EZShredCustomerData>();

                    //Find the record to be deleted
                    foreach (var duplicateBuilding in duplicateCustomers)
                    {
                        //filter records order by date
                        var duplicateRecords = records.Where(w => w.AccountObjectId == duplicateBuilding.AccountObjectId
                        && w.CustomerID == duplicateBuilding.BuildingID).OrderBy(o => o.CreatedOn).ToList();
                        //add record to be deleted in the list
                        foreach (var item in duplicateRecords)
                        {
                            //only add records except first one which is older
                            if (duplicateRecords.IndexOf(item) > 0)
                            {
                                lstRecordsToDelete.Add(item);
                            }
                        }
                    }

                    //delete duplicate records
                    foreach (var item in lstRecordsToDelete)
                    {
                        repo.Delete(item.Id);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region Private Methods
        private EZShredCustomerData GetCustomerDataEntity(EZShredCustomerDataModel data, string id = null)
        {
            return new EZShredCustomerData()
            {
                Id = id,
                AccountObjectId = data.AccountObjectId,
                MxtrAccountId = data.MxtrAccountId,
                AllowZeroInvoices = data.AllowZeroInvoices,
                ARCustomerCode = data.ARCustomerCode,
                Attention = data.Attention,
                BillingContact = data.BillingContact,
                BillingContactExtension = data.BillingContactExtension,
                BillingContactPhone = data.BillingContactPhone,
                BillingContactSameAsMainContact = data.BillingContactSameAsMainContact,
                BillingCountryCode = data.BillingCountryCode,
                CertificateDestruction = data.CertificateDestruction,
                City = data.City,
                Company = data.Company,
                Contact = data.Contact,
                CreditHold = data.CreditHold,
                CustomerID = data.CustomerID,
                CustomerTypeID = data.CustomerTypeID,
                EmailAddress = data.EmailAddress,
                EmailCOD = data.EmailCOD,
                EmailInvoice = data.EmailInvoice,
                EZTimeStamp = data.EZTimestamp,
                Fax = data.Fax,
                InvoiceNote = data.InvoiceNote,
                InvoiceTypeID = data.InvoiceTypeID,
                LeadID = data.LeadID,
                Mobile = data.Mobile,
                Notes = data.Notes,
                NumberOfBoxes = data.NumberOfBoxes,
                NumberOfEmployees = data.NumberOfEmployees,
                Operation = data.Operation,
                OpportunityID = data.OpportunityID,
                PaidInFull = data.PaidInFull,
                Phone = data.Phone,
                PipelineStatus = data.PipelineStatus,
                PurchaseOrder = data.PurchaseOrder,
                PurchaseOrderExpire = data.PurchaseOrderExpire,
                ReferralSourceID = data.ReferralSourceID,
                ServicesProfessionalType = data.ServicesProfessionalType,
                State = data.State,
                Street = data.Street,
                Suite = data.Suite,
                TermID = data.TermID,
                TravelTourismType = data.TravelTourismType,
                UserID = data.UserID,
                Zip = data.Zip
            };
        }
        private IEnumerable<EZShredCustomerDataModel> GetCustomerDataModel(IEnumerable<EZShredCustomerData> data)
        {
            return data.Select(x => new EZShredCustomerDataModel()
            {
                AccountObjectId = x.AccountObjectId,
                AllowZeroInvoices = x.AllowZeroInvoices,
                ARCustomerCode = x.ARCustomerCode,
                Attention = x.Attention,
                BillingContact = x.BillingContact,
                BillingContactExtension = x.BillingContactExtension,
                BillingContactPhone = x.BillingContactPhone,
                BillingContactSameAsMainContact = x.BillingContactSameAsMainContact,
                BillingCountryCode = x.BillingCountryCode,
                CertificateDestruction = x.CertificateDestruction,
                City = x.City,
                Company = x.Company,
                Contact = x.Contact,
                CreditHold = x.CreditHold,
                CustomerID = x.CustomerID,
                CustomerTypeID = x.CustomerTypeID,
                EmailAddress = x.EmailAddress,
                EmailCOD = x.EmailCOD,
                EmailInvoice = x.EmailInvoice,
                EZTimestamp = x.EZTimeStamp,
                Fax = x.Fax,
                InvoiceNote = x.InvoiceNote,
                InvoiceTypeID = x.InvoiceTypeID,
                LeadID = x.LeadID,
                Mobile = x.Mobile,
                MxtrAccountId = x.MxtrAccountId,
                Notes = x.Notes,
                NumberOfBoxes = x.NumberOfBoxes,
                NumberOfEmployees = x.NumberOfEmployees,
                Operation = x.Operation,
                OpportunityID = x.OpportunityID,
                PaidInFull = x.PaidInFull,
                Phone = x.Phone,
                PipelineStatus = x.PipelineStatus,
                PurchaseOrder = x.PurchaseOrder,
                PurchaseOrderExpire = x.PurchaseOrderExpire,
                ReferralSourceID = x.ReferralSourceID,
                ServicesProfessionalType = x.ServicesProfessionalType,
                State = x.State,
                Street = x.Street,
                Suite = x.Suite,
                TermID = x.TermID,
                TravelTourismType = x.TravelTourismType,
                UserID = x.UserID,
                Zip = x.Zip
            });
        }
        private IEnumerable<EZShredCustomerDataModelMini> GetCustomerMiniDataModel(IEnumerable<EZShredCustomerData> data)
        {
            return data.Select(x => new EZShredCustomerDataModelMini()
            {
                Company = x.Company,
                Street = x.Street,
                Zip = x.Zip,
                LeadID = x.LeadID,
                CustomerID = x.CustomerID,
                OpportunityID = x.OpportunityID,
                AccountObjectId = x.AccountObjectId,
                AllowZeroInvoices = x.AllowZeroInvoices,
                ARCustomerCode = x.ARCustomerCode,
            });
        }
        private EZShredCustomerDataModel GetCustomerDataModel(EZShredCustomerData data)
        {
            return new EZShredCustomerDataModel()
            {
                AccountObjectId = data.AccountObjectId,
                AllowZeroInvoices = data.AllowZeroInvoices,
                ARCustomerCode = data.ARCustomerCode,
                Attention = data.Attention,
                BillingContact = data.BillingContact,
                BillingContactExtension = data.BillingContactExtension,
                BillingContactPhone = data.BillingContactPhone,
                BillingContactSameAsMainContact = data.BillingContactSameAsMainContact,
                BillingCountryCode = data.BillingCountryCode,
                CertificateDestruction = data.CertificateDestruction,
                City = data.City,
                Company = data.Company,
                Contact = data.Contact,
                CreditHold = data.CreditHold,
                CustomerID = data.CustomerID,
                CustomerTypeID = data.CustomerTypeID,
                EmailAddress = data.EmailAddress,
                EmailCOD = data.EmailCOD,
                EmailInvoice = data.EmailInvoice,
                EZTimestamp = data.EZTimeStamp,
                Fax = data.Fax,
                InvoiceNote = data.InvoiceNote,
                InvoiceTypeID = data.InvoiceTypeID,
                LeadID = data.LeadID,
                Mobile = data.Mobile,
                MxtrAccountId = data.MxtrAccountId,
                Notes = data.Notes,
                NumberOfBoxes = data.NumberOfBoxes,
                NumberOfEmployees = data.NumberOfEmployees,
                Operation = data.Operation,
                OpportunityID = data.OpportunityID,
                PaidInFull = data.PaidInFull,
                Phone = data.Phone,
                PipelineStatus = data.PipelineStatus,
                PurchaseOrder = data.PurchaseOrder,
                PurchaseOrderExpire = data.PurchaseOrderExpire,
                ReferralSourceID = data.ReferralSourceID,
                ServicesProfessionalType = data.ServicesProfessionalType,
                State = data.State,
                Street = data.Street,
                Suite = data.Suite,
                TermID = data.TermID,
                TravelTourismType = data.TravelTourismType,
                UserID = data.UserID,
                Zip = data.Zip
            };
        }
        private List<EZShredCustomerDataModel> GetCustomerDataModel(List<Customers> lstCustomer, string accountObjectId, string mxtrAccountObjectId)
        {
            return lstCustomer.Select(x => new EZShredCustomerDataModel()
            {
                AccountObjectId = accountObjectId,
                MxtrAccountId = mxtrAccountObjectId,
                AllowZeroInvoices = x.AllowZeroInvoices,
                ARCustomerCode = x.ARCustomerCode,
                Attention = x.Attention,
                BillingContact = x.BillingContact,
                BillingContactExtension = x.BillingContactExtension,
                BillingContactPhone = x.BillingContactPhone,
                BillingContactSameAsMainContact = x.BillingContactSameAsMainContact,
                BillingCountryCode = x.BillingCountryCode,
                CertificateDestruction = x.CertificateDestruction,
                City = x.City,
                Company = x.Company,
                Contact = x.Contact,
                CreditHold = x.CreditHold,
                CustomerID = x.CustomerID,
                CustomerTypeID = x.CustomerTypeID,
                DataSource = x.DataSource,
                EmailAddress = x.EmailAddress,
                EmailCOD = x.EmailCOD,
                EmailInvoice = x.EmailInvoice,
                EZTimestamp = x.ezTimestamp,
                Fax = x.Fax,
                InvoiceNote = x.InvoiceNote,
                InvoiceTypeID = x.InvoiceTypeID,
                LeadID = x.LeadID,
                Mobile = x.Mobile,
                Notes = x.Notes,
                NumberOfBoxes = x.NumberOfBoxes,
                NumberOfEmployees = x.NumberOfEmployees,
                Operation = x.operation,
                OpportunityID = x.OpportunityID,
                PaidInFull = x.PaidInFull,
                Phone = x.Phone,
                PipelineStatus = x.PipelineStatus,
                PurchaseOrder = x.PurchaseOrder,
                PurchaseOrderExpire = x.PurchaseOrderExpire,
                ReferralSourceID = x.ReferralSourceID,
                ServicesProfessionalType = x.ServicesProfessionalType,
                State = x.State,
                Street = x.Street,
                Suite = x.Suite,
                TermID = x.TermID,
                TravelTourismType = x.TravelTourismType,
                UserID = x.userID,
                Zip = x.Zip
            }).ToList();
        }
        #endregion
    }
}
