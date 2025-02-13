using mxtrAutomation.Api.EZShred;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class CRMEZShredService : MongoRepository<EZShredData>, ICRMEZShredServiceInternal
    {
        public bool AddUpdateData(EZShredDataModel data)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                //bool recordExists = false;
                var entry = repo.Where(a => a.AccountObjectId == data.AccountObjectId).FirstOrDefault();
                if (entry == null)
                {
                    entry = AddData(data);
                    repo.Add(entry);
                }
                else
                {
                    //recordExists = true;
                    //repo.Delete(entry);
                    entry = UpdateData(entry, data);
                    repo.Update(entry);
                }


                try
                {
                    //if (recordExists)
                    //{
                    //    repo.Update(entry);
                    //}
                    //else
                    //{
                    //    repo.Add(entry);
                    //}
                    return true;
                    //return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    return false;
                    //return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }
        public IOrderedEnumerable<EZShredDataModel> GetEZShredDataByAccountObjectIDs(List<string> accountObjectIDs)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                return repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectId))
                    .Select(c => new EZShredDataModel
                    {
                        AccountObjectId = c.AccountObjectId,
                        MxtrAccountId = c.MxtrAccountId,
                        LastModifiedDate = c.LastModifiedDate,
                        BuildingTypes = c.BuildingTypes,
                        CustomerTypes = c.CustomerTypes,
                        Routes = c.Routes,
                        Salesman = c.Salesman,
                        SalesTaxRegions = c.SalesTaxRegions,
                        ServiceItems = c.ServiceItems,
                        ServiceTypes = c.ServiceTypes,
                        Frequencys = c.Frequencys,
                        ReferralSources = c.ReferralSources,
                        InvoiceTypes = c.InvoiceTypes,
                        TermTypes = c.TermTypes
                    }).ToList().OrderBy(o => o.LastModifiedDate);
            }
        }
        public IOrderedEnumerable<EZShredDataModel> GetEZShredDataByAccountObjectIDs_DateRange(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                return repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectId) && (c.LastModifiedDate >= startDate && c.LastModifiedDate <= endDate))
                    .Select(c => new EZShredDataModel
                    {
                        AccountObjectId = c.AccountObjectId,
                        MxtrAccountId = c.MxtrAccountId,
                        LastModifiedDate = c.LastModifiedDate,
                        BuildingTypes = c.BuildingTypes,
                        CustomerTypes = c.CustomerTypes,
                        Routes = c.Routes,
                        Salesman = c.Salesman,
                        SalesTaxRegions = c.SalesTaxRegions,
                        ServiceItems = c.ServiceItems,
                        ServiceTypes = c.ServiceTypes,
                        Building = GetBuildingData(c.AccountObjectId, c.MxtrAccountId),
                        Frequencys = c.Frequencys,
                        ReferralSources = c.ReferralSources,
                        InvoiceTypes = c.InvoiceTypes,
                        TermTypes = c.TermTypes
                    }).ToList().OrderBy(o => o.LastModifiedDate);
            }
        }
        private List<EZShredBuildingDataModel> GetBuildingData(string accountObjectId, string mxtrAccountId)
        {
            using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
            {
                var result = repo.Where(c => c.AccountObjectId == accountObjectId && c.MxtrAccountId == mxtrAccountId);
                if (result != null)
                {
                    return result.Select(x => new EZShredBuildingDataModel
                    {
                        AccountObjectId = accountObjectId,
                        MxtrAccountId = mxtrAccountId,
                        CompanyName = x.CompanyName,
                        BuildingID = x.BuildingID,
                        CustomerID = x.CustomerID,
                        Street = x.Street,
                        City = x.City,
                        State = x.State,
                        Zip = x.Zip,
                        ScheduleFrequency = x.ScheduleFrequency,
                        ServiceTypeID = x.ServiceTypeID,
                        Notes = x.Notes,
                        Directions = x.Directions,
                        RoutineInstructions = x.RoutineInstructions,
                        SiteContact1 = x.SiteContact1,
                        SiteContact2 = x.SiteContact2,
                        Phone1 = x.Phone1,
                        Phone2 = x.Phone2,
                        SalesTaxRegionID = x.SalesTaxRegionID,
                        Suite = x.Suite,
                        SalesmanID = x.SalesmanID,
                        BuildingTypeID = x.BuildingTypeID,
                        UserID = x.UserID,
                        CompanyCountryCode = x.CompanyCountryCode,
                        EndDate = x.EndDate,
                        EZTimestamp = x.EZTimestamp,
                        LastServiceDate = x.LastServiceDate,
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        NextServiceDate = x.NextServiceDate,
                        Operation = x.Operation,
                        RouteID = x.RouteID,
                        ScheduleDescription = x.ScheduleDescription,
                        ScheduleDOWfri = x.ScheduleDOWfri,
                        ScheduleDOWmon = x.ScheduleDOWmon,
                        ScheduleDOWsat = x.ScheduleDOWsat,
                        ScheduleDOWsun = x.ScheduleDOWsun,
                        ScheduleDOWthu = x.ScheduleDOWthu,
                        ScheduleDOWtue = x.ScheduleDOWtue,
                        ScheduleDOWwed = x.ScheduleDOWwed,
                        ScheduleWeek1 = x.ScheduleWeek1,
                        ScheduleWeek2 = x.ScheduleWeek2,
                        ScheduleWeek3 = x.ScheduleWeek3,
                        ScheduleWeek4 = x.ScheduleWeek4,
                        ScheduleWeek5 = x.ScheduleWeek5,
                        StartDate = x.StartDate,
                        Stop = x.Stop,
                        TimeTaken = x.TimeTaken,
                        OpportunityID = x.OpportunityID,
                    }).ToList();
                }
            }
            return new List<EZShredBuildingDataModel>();
        }
        //public List<Customers> GetAllCustomers(string AccountObjectId)
        //{
        //    using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
        //    {
        //        // 591449f0ffec0929842a81e4 Proshred AccountObjectID--591449f0ffec0929842a81e4
        //        var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
        //        if (entry.Customers != null)
        //            return entry.Customers.OrderByDescending(c => c.CustomerID).ToList();
        //        else
        //            return new List<Customers>();
        //    }
        //}
        public EZShredDataModel GetCustomerAndBuildingInformations(string AccountObjectId, string CustomerId, string BuidlingId)
        {
            //using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            //{
            //    // 591449f0ffec0929842a81e4 Proshred AccountObjectID
            //    var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
            //    EZShredDataModel _ezSharedData = new EZShredDataModel();
            //    if (entry != null)
            //    {
            //        _ezSharedData.Customer = entry.Customers.Where(c => c.CustomerID == CustomerId).ToList();
            //        _ezSharedData.Building = entry.Buildings.Where(b => b.CustomerID == CustomerId).ToList();
            //    }
            //    return _ezSharedData;
            //}

            EZShredDataModel _ezSharedData = new EZShredDataModel();
            using (MongoRepository<EZShredCustomerData> repo = new MongoRepository<EZShredCustomerData>())
            {
                //var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId) && a.CustomerID == CustomerId).ToList();
                var entry = repo.Where(a => a.AccountObjectId == AccountObjectId && a.CustomerID == CustomerId).ToList();
                if (entry != null)
                {
                    _ezSharedData.Customer = entry.Select(x => new EZShredCustomerDataModel
                    {
                        AccountObjectId = x.AccountObjectId,
                        MxtrAccountId = x.MxtrAccountId,
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
                    }).ToList();
                }
            }

            using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
            {
                var entry = repo.Where(a => a.AccountObjectId == AccountObjectId && a.CustomerID == CustomerId && a.BuildingID == BuidlingId).ToList();
                if (entry != null)
                {
                    _ezSharedData.Building = entry.Select(x => new EZShredBuildingDataModel
                    {
                        CompanyName = x.CompanyName,
                        BuildingID = x.BuildingID,
                        CustomerID = x.CustomerID,
                        Street = x.Street,
                        City = x.City,
                        State = x.State,
                        Zip = x.Zip,
                        ScheduleFrequency = x.ScheduleFrequency,
                        ServiceTypeID = x.ServiceTypeID,
                        Notes = x.Notes,
                        Directions = x.Directions,
                        RoutineInstructions = x.RoutineInstructions,
                        SiteContact1 = x.SiteContact1,
                        SiteContact2 = x.SiteContact2,
                        Phone1 = x.Phone1,
                        Phone2 = x.Phone2,
                        SalesTaxRegionID = x.SalesTaxRegionID,
                        Suite = x.Suite,
                        SalesmanID = x.SalesmanID,
                        BuildingTypeID = x.BuildingTypeID,
                        UserID = x.UserID,
                        AccountObjectId = x.AccountObjectId,
                        CompanyCountryCode = x.CompanyCountryCode,
                        EndDate = x.EndDate,
                        EZTimestamp = x.EZTimestamp,
                        LastServiceDate = x.LastServiceDate,
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        MxtrAccountId = x.MxtrAccountId,
                        NextServiceDate = x.NextServiceDate,
                        Operation = x.Operation,
                        RouteID = x.RouteID,
                        ScheduleDescription = x.ScheduleDescription,
                        ScheduleDOWfri = x.ScheduleDOWfri,
                        ScheduleDOWmon = x.ScheduleDOWmon,
                        ScheduleDOWsat = x.ScheduleDOWsat,
                        ScheduleDOWsun = x.ScheduleDOWsun,
                        ScheduleDOWthu = x.ScheduleDOWthu,
                        ScheduleDOWtue = x.ScheduleDOWtue,
                        ScheduleDOWwed = x.ScheduleDOWwed,
                        ScheduleWeek1 = x.ScheduleWeek1,
                        ScheduleWeek2 = x.ScheduleWeek2,
                        ScheduleWeek3 = x.ScheduleWeek3,
                        ScheduleWeek4 = x.ScheduleWeek4,
                        ScheduleWeek5 = x.ScheduleWeek5,
                        StartDate = x.StartDate,
                        Stop = x.Stop,
                        TimeTaken = x.TimeTaken,
                        OpportunityID = x.OpportunityID,
                    }).ToList();
                }
            }
            return _ezSharedData;
        }
        //public int GetBuildingIdAgainstCustomerId(string AccountObjectId, string CustomerId)
        //{
        //    int buildingId = 0;
        //    using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
        //    {
        //        var entry = repo.FirstOrDefault(a => a.AccountObjectId == AccountObjectId);
        //        EZShredDataModel _ezSharedData = new EZShredDataModel();
        //        if (entry != null)
        //        {
        //            Buildings building = entry.Buildings.LastOrDefault(b => b.CustomerID == CustomerId);
        //            if (building != null && building.BuildingID != null)
        //            {
        //                buildingId = Convert.ToInt32(building.BuildingID);
        //            }
        //        }
        //        return buildingId;
        //    }
        //}
        public List<ServiceItems> GetAllServiceItems(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                //var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
                var entry = repo.FirstOrDefault(a => a.AccountObjectId.Contains(AccountObjectId));
                if (entry != null)
                    return entry.ServiceItems;
                else
                    return new List<ServiceItems>();
            }
        }
        public List<ServiceTypes> GetAllServiceTypes(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                //var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
                var entry = repo.FirstOrDefault(a => a.AccountObjectId.Contains(AccountObjectId));
                if (entry != null)
                    return entry.ServiceTypes;
                else
                    return new List<ServiceTypes>();
            }
        }
        public List<Salesmans> GetAllSalesmans(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                //var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
                var entry = repo.FirstOrDefault(a => a.AccountObjectId.Contains(AccountObjectId));
                if (entry != null)
                    return entry.Salesman;
                else
                    return new List<Salesmans>();
            }
        }
        private List<SalesTaxRegions> GetAllSalesTaxRegions(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                //var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
                var entry = repo.FirstOrDefault(a => a.AccountObjectId.Contains(AccountObjectId));
                if (entry != null)
                    return entry.SalesTaxRegions;
                else
                    return new List<SalesTaxRegions>();
            }
        }
        public List<Frequencys> GetAllFrequencys(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                //var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
                var entry = repo.FirstOrDefault(a => a.AccountObjectId.Contains(AccountObjectId));
                if (entry != null)
                    return entry.Frequencys;
                else
                    return new List<Frequencys>();
            }
        }
        public List<BuildingTypes> GetAllBuildingTypes(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                //var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
                var entry = repo.FirstOrDefault(a => a.AccountObjectId.Contains(AccountObjectId));
                if (entry != null)
                    return entry.BuildingTypes;
                else
                    return new List<BuildingTypes>();
            }
        }
        public List<InvoiceTypes> GetAllInvoiceTypes(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                //var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
                var entry = repo.FirstOrDefault(a => a.AccountObjectId.Contains(AccountObjectId));
                if (entry != null)
                    return entry.InvoiceTypes;
                else
                    return new List<InvoiceTypes>();
            }
        }
        public List<TermTypes> GetAllTermTypes(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                //var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
                var entry = repo.FirstOrDefault(a => a.AccountObjectId.Contains(AccountObjectId));
                if (entry != null)
                    return entry.TermTypes;
                else
                    return new List<TermTypes>();
            }
        }
        public List<ReferralSources> GetAllReferralSources(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                //var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
                var entry = repo.FirstOrDefault(a => a.AccountObjectId.Contains(AccountObjectId));
                if (entry != null)
                    return entry.ReferralSources;
                else
                    return new List<ReferralSources>();
            }
        }
        public List<CustomerTypes> GetAllCustomerTypes(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                //var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
                var entry = repo.FirstOrDefault(a => a.AccountObjectId.Contains(AccountObjectId));
                if (entry != null)
                    return entry.CustomerTypes;
                else
                    return new List<CustomerTypes>();
            }
        }
        public EZShredDataModel GetAllTypes(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                EZShredDataModel _ezSharedData = new EZShredDataModel()
                {
                    ServiceItems = GetAllServiceItems(AccountObjectId),
                    Frequencys = GetAllFrequencys(AccountObjectId),
                    BuildingTypes = GetAllBuildingTypes(AccountObjectId),
                    InvoiceTypes = GetAllInvoiceTypes(AccountObjectId),
                    TermTypes = GetAllTermTypes(AccountObjectId),
                    ReferralSources = GetAllReferralSources(AccountObjectId),
                    CustomerTypes = GetAllCustomerTypes(AccountObjectId),
                    ServiceTypes = GetAllServiceTypes(AccountObjectId),
                    Salesman = GetAllSalesmans(AccountObjectId)
                };
                return _ezSharedData;
            }
        }
        public List<SSField> GetAllSSFields(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
                if (entry.SSField != null)
                    return entry.SSField.OrderByDescending(c => c.EZShredFieldName).ToList();
                else
                    return new List<SSField>();
            }
        }
        public int IsSSFieldExist(string AccountObjectId)
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                var entry = repo.Where(a => a.AccountObjectId.Contains(AccountObjectId)).FirstOrDefault();
                if (entry.SSField != null)
                {
                    return Convert.ToInt32(entry.SSField.Count());
                }
                else
                {
                    return 0;
                }
            }
        }
        //public bool UpdateCustomerAndBuilding(EZShredDataModel data)
        //{
        //    using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
        //    {
        //        bool recordExists = false;
        //        var entry = repo.Where(a => a.AccountObjectId == data.AccountObjectId).FirstOrDefault();
        //        EZShredData _ezShredData = new EZShredData();
        //        if (entry != null)
        //        {
        //            if (data.Customer != null)
        //            {
        //                entry.Customer.Where(x => x.CustomerID == data.Customers[0].CustomerID.ToString()).ToList().ForEach(c =>
        //                {
        //                    c.ARCustomerCode = data.Customers[0].ARCustomerCode;
        //                    c.Company = data.Customers[0].Company;
        //                    c.Attention = data.Customers[0].Attention;
        //                    c.Street = data.Customers[0].Street;
        //                    c.City = data.Customers[0].City;
        //                    c.State = data.Customers[0].State;
        //                    c.Zip = data.Customers[0].Zip;
        //                    c.Contact = data.Customers[0].Contact;
        //                    c.Phone = data.Customers[0].Phone;
        //                    c.Fax = data.Customers[0].Fax;
        //                    c.Notes = data.Customers[0].Notes;
        //                    c.CustomerTypeID = data.Customers[0].CustomerTypeID;
        //                    c.InvoiceTypeID = data.Customers[0].InvoiceTypeID;
        //                    c.EmailAddress = data.Customers[0].EmailAddress;
        //                    c.EmailInvoice = data.Customers[0].EmailInvoice;
        //                    c.ReferralSourceID = data.Customers[0].ReferralSourceID;
        //                    c.TermID = data.Customers[0].TermID;
        //                    c.EmailCOD = data.Customers[0].EmailCOD;
        //                    c.NumberOfEmployees = data.Customers[0].NumberOfEmployees;
        //                    c.Mobile = data.Customers[0].Mobile;
        //                    c.PipelineStatus = data.Customers[0].PipelineStatus;
        //                    c.Suite = data.Customers[0].Suite;
        //                    c.userID = data.Customers[0].userID;
        //                    c.LeadID = data.Customers[0].LeadID;
        //                    c.OpportunityID = data.Customers[0].OpportunityID;
        //                });
        //                recordExists = true;
        //            }

        //            if (!String.IsNullOrEmpty(data.Building))
        //            {
        //                entry.Buildings.Where(x => x.CustomerID == data.Customers[0].CustomerID.ToString() && x.BuildingID == data.Buildings[0].BuildingID.ToString()).ToList().ForEach(c =>
        //                {
        //                    c.CompanyName = data.Buildings[0].CompanyName;
        //                    c.BuildingID = data.Buildings[0].BuildingID;
        //                    c.Street = data.Buildings[0].Street;
        //                    c.City = data.Buildings[0].City;
        //                    c.State = data.Buildings[0].State;
        //                    c.Zip = data.Buildings[0].Zip;
        //                    c.ScheduleFrequency = data.Buildings[0].ScheduleFrequency;
        //                    c.ServiceTypeID = data.Buildings[0].ServiceTypeID;
        //                    c.Notes = data.Buildings[0].Notes;
        //                    c.Directions = data.Buildings[0].Directions;
        //                    c.RoutineInstructions = data.Buildings[0].RoutineInstructions;
        //                    c.SiteContact1 = data.Buildings[0].SiteContact1;
        //                    c.Phone1 = data.Buildings[0].Phone1;
        //                    c.Phone2 = data.Buildings[0].Phone2;
        //                    c.Suite = data.Buildings[0].Suite;
        //                    c.SalesmanID = data.Buildings[0].SalesmanID;
        //                    c.BuildingTypeID = data.Buildings[0].BuildingTypeID;
        //                    c.userID = data.Buildings[0].userID;
        //                });
        //                recordExists = true;
        //            }
        //        }
        //        try
        //        {
        //            if (recordExists)
        //            {
        //                repo.Update(entry);
        //            }
        //            else
        //            {
        //                repo.Add(entry);
        //            }
        //            return true;
        //            //return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
        //        }
        //        catch (Exception ex)
        //        {
        //            string msg = ex.Message;
        //            return false;
        //            //return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
        //        }
        //    }
        //}
        //public bool AddCustomerAndBuilding(EZShredDataModel data)
        //{
        //    using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
        //    {
        //        // bool recordExists = false;
        //        var entry = repo.FirstOrDefault(a => a.AccountObjectId == data.AccountObjectId);
        //        // EZShredData _ezShredData = new EZShredData();
        //        if (entry != null)
        //        {
        //            entry.Customers.Add(new Customers
        //            {
        //                ARCustomerCode = data.Customers[0].ARCustomerCode,
        //                CustomerID = data.Customers[0].CustomerID,
        //                Company = data.Customers[0].Company,
        //                Attention = data.Customers[0].Attention,
        //                Street = data.Customers[0].Street,
        //                City = data.Customers[0].City,
        //                State = data.Customers[0].State,
        //                Zip = data.Customers[0].Zip,
        //                Contact = data.Customers[0].Contact,
        //                Phone = data.Customers[0].Phone,
        //                Fax = data.Customers[0].Fax,
        //                Notes = data.Customers[0].Notes,
        //                CustomerTypeID = data.Customers[0].CustomerTypeID,
        //                InvoiceTypeID = data.Customers[0].InvoiceTypeID,
        //                EmailAddress = data.Customers[0].EmailAddress,
        //                EmailInvoice = data.Customers[0].EmailInvoice,
        //                ReferralSourceID = data.Customers[0].ReferralSourceID,
        //                TermID = data.Customers[0].TermID,
        //                CreditHold = data.Customers[0].CreditHold,
        //                EmailCOD = data.Customers[0].EmailCOD,
        //                PipelineStatus = data.Customers[0].PipelineStatus,
        //                NumberOfEmployees = data.Customers[0].NumberOfEmployees,
        //                Mobile = data.Customers[0].Mobile,
        //                Suite = data.Customers[0].Suite,
        //                userID = data.Customers[0].userID,
        //                LeadID = data.Customers[0].LeadID,
        //                OpportunityID = data.Customers[0].OpportunityID,
        //            });
        //            entry.Buildings.Add(new Buildings
        //            {
        //                CompanyName = data.Buildings[0].CompanyName,
        //                BuildingID = data.Buildings[0].BuildingID,
        //                CustomerID = data.Buildings[0].CustomerID,
        //                Street = data.Buildings[0].Street,
        //                City = data.Buildings[0].City,
        //                State = data.Buildings[0].State,
        //                Zip = data.Buildings[0].Zip,
        //                ScheduleFrequency = data.Buildings[0].ScheduleFrequency,
        //                ServiceTypeID = data.Buildings[0].ServiceTypeID,
        //                Notes = data.Buildings[0].Notes,
        //                Directions = data.Buildings[0].Directions,
        //                RoutineInstructions = data.Buildings[0].RoutineInstructions,
        //                SiteContact1 = data.Buildings[0].SiteContact1,
        //                SiteContact2 = data.Buildings[0].SiteContact2,
        //                Phone1 = data.Buildings[0].Phone1,
        //                Phone2 = data.Buildings[0].Phone2,
        //                SalesTaxRegionID = data.Buildings[0].SalesTaxRegionID,// For tax status lable
        //                Suite = data.Buildings[0].Suite,
        //                SalesmanID = data.Buildings[0].SalesmanID,
        //                BuildingTypeID = data.Buildings[0].BuildingTypeID,
        //                userID = data.Buildings[0].userID,
        //            });
        //        }
        //        try
        //        {
        //            repo.Update(entry);
        //            return true;
        //            //return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
        //        }
        //        catch (Exception ex)
        //        {
        //            string msg = ex.Message;
        //            return false;
        //            //return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
        //        }
        //    }
        //}
        #region Private Methods
        //private EZShredData UpdateData(EZShredData entry, EZShredDataModel data)
        //{
        //    if (HasData(data.AccountObjectId))
        //    {
        //        entry.AccountObjectId = data.AccountObjectId;
        //    }
        //    if (data.BuildingTypes != null)
        //    {
        //        entry.BuildingTypes = data.BuildingTypes;
        //    }
        //    if (data.CustomerTypes != null)
        //    {
        //        entry.CustomerTypes = data.CustomerTypes;
        //    }
        //    if (data.Routes != null)
        //    {
        //        entry.Routes = data.Routes;
        //    }
        //    if (data.Salesman != null)
        //    {
        //        entry.Salesman = data.Salesman;
        //    }
        //    if (data.SalesTaxRegions != null)
        //    {
        //        entry.SalesTaxRegions = data.SalesTaxRegions;
        //    }
        //    if (data.ServiceItems != null)
        //    {
        //        entry.ServiceItems = data.ServiceItems;
        //    }
        //    if (data.ServiceTypes != null)
        //    {
        //        entry.ServiceTypes = data.ServiceTypes;
        //    }
        //    if (data.Customers != null)
        //    {
        //        entry.Customers = data.Customers;
        //    }
        //    if (data.Buildings != null)
        //    {
        //        entry.Buildings = data.Buildings;
        //    }
        //    if (data.Frequencys != null)
        //    {
        //        entry.Frequencys = data.Frequencys;
        //    }
        //    if (data.ReferralSources != null)
        //    {
        //        entry.ReferralSources = data.ReferralSources;
        //    }
        //    if (data.TermTypes != null)
        //    {
        //        entry.TermTypes = data.TermTypes;
        //    }
        //    if (data.InvoiceTypes != null)
        //    {
        //        entry.InvoiceTypes = data.InvoiceTypes;
        //    }
        //    if (data.SSField != null)
        //    {
        //        entry.SSField = data.SSField;
        //    }

        //    entry.LastModifiedDate = DateTime.Now;
        //    return entry;
        //}
        private static EZShredData AddData(EZShredDataModel data)
        {
            return new EZShredData()
            {
                AccountObjectId = data.AccountObjectId,
                MxtrAccountId = data.MxtrAccountId,
                CRMKind = Enums.CRMKind.EZShred,
                BuildingTypes = data.BuildingTypes,
                CustomerTypes = data.CustomerTypes,
                Routes = data.Routes,
                Salesman = data.Salesman,
                SalesTaxRegions = data.SalesTaxRegions,
                ServiceItems = data.ServiceItems,
                ServiceTypes = data.ServiceTypes,
                //Customers = data.Customers,
                //Buildings = data.Buildings,
                Frequencys = data.Frequencys,
                ReferralSources = data.ReferralSources,
                InvoiceTypes = data.InvoiceTypes,
                SSField = data.SSField,
                TermTypes = data.TermTypes,
                LastModifiedDate = DateTime.Now,
            };
        }
        private bool HasData(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return false;
            }
            return true;
        }

        private EZShredData UpdateData(EZShredData entry, EZShredDataModel data)
        {
            if (data.BuildingTypes != null && data.BuildingTypes.Count > 0)
            {
                if (entry.BuildingTypes == null)
                    entry.BuildingTypes = data.BuildingTypes;
                else
                {
                    foreach (var buildingType in data.BuildingTypes)
                    {
                        var buildingTypeData = entry.BuildingTypes.FirstOrDefault(x => x.BuildingTypeID == buildingType.BuildingTypeID);
                        if (buildingTypeData == null)
                        {
                            entry.BuildingTypes.Add(buildingType);
                        }
                        else
                        {
                            buildingTypeData.BuildingType = buildingType.BuildingType;
                        }
                    }
                }
            }

            if (data.CustomerTypes != null && data.CustomerTypes.Count > 0)
            {
                if (entry.CustomerTypes == null)
                    entry.CustomerTypes = data.CustomerTypes;
                else
                {
                    foreach (var customerType in data.CustomerTypes)
                    {
                        var customerTypeData = entry.CustomerTypes.FirstOrDefault(x => x.CustomerTypeID == customerType.CustomerTypeID);
                        if (customerTypeData == null)
                        {
                            entry.CustomerTypes.Add(customerType);
                        }
                        else
                        {
                            customerTypeData.CustomerType = customerType.CustomerType;
                        }
                    }
                }
            }

            if (data.Routes != null && data.Routes.Count > 0)
            {
                if (entry.Routes == null)
                    entry.Routes = data.Routes;
                else
                {
                    foreach (var routes in data.Routes)
                    {
                        var routeData = entry.Routes.FirstOrDefault(x => x.RouteID == routes.RouteID);
                        if (routeData == null)
                        {
                            entry.Routes.Add(routes);
                        }
                        else
                        {
                            routeData.Route = routes.Route;
                            routeData.order = routes.order;
                        }
                    }
                }
            }

            if (data.Salesman != null && data.Salesman.Count > 0)
            {
                if (entry.Salesman == null)
                    entry.Salesman = data.Salesman;
                else
                {
                    foreach (var salesman in data.Salesman)
                    {
                        var salesmanData = entry.Salesman.FirstOrDefault(x => x.SalesmanID == salesman.SalesmanID);
                        if (salesmanData == null)
                        {
                            entry.Salesman.Add(salesman);
                        }
                        else
                        {
                            salesmanData.Salesman = salesman.Salesman;
                            salesmanData.order = salesman.order;
                        }
                    }
                }
            }

            if (data.SalesTaxRegions != null && data.SalesTaxRegions.Count > 0)
            {
                if (entry.SalesTaxRegions == null)
                    entry.SalesTaxRegions = data.SalesTaxRegions;
                else
                {
                    foreach (var salesTaxRegions in data.SalesTaxRegions)
                    {
                        var salesTaxRegionsData = entry.SalesTaxRegions.FirstOrDefault(x => x.SalesTaxRegionID == salesTaxRegions.SalesTaxRegionID);
                        if (salesTaxRegionsData == null)
                        {
                            entry.SalesTaxRegions.Add(salesTaxRegions);
                        }
                        else
                        {
                            salesTaxRegionsData.Region = salesTaxRegions.Region;
                            salesTaxRegionsData.State = salesTaxRegions.State;
                        }
                    }
                }
            }

            if (data.ServiceItems != null && data.ServiceItems.Count > 0)
            {
                if (entry.ServiceItems == null)
                    entry.ServiceItems = data.ServiceItems;
                else
                {
                    foreach (var serviceItems in data.ServiceItems)
                    {
                        var serviceItemsData = entry.ServiceItems.FirstOrDefault(x => x.ServiceItemsID == serviceItems.ServiceItemsID);
                        if (serviceItemsData == null)
                        {
                            entry.ServiceItems.Add(serviceItems);
                        }
                        else
                        {
                            serviceItemsData.ServiceItem = serviceItems.ServiceItem;
                        }
                    }
                }
            }

            if (data.ServiceTypes != null && data.ServiceTypes.Count > 0)
            {
                if (entry.ServiceTypes == null)
                    entry.ServiceTypes = data.ServiceTypes;
                else
                {
                    foreach (var serviceTypes in data.ServiceTypes)
                    {
                        var serviceTypesData = entry.ServiceTypes.FirstOrDefault(x => x.ServiceTypeID == serviceTypes.ServiceTypeID);
                        if (serviceTypesData == null)
                        {
                            entry.ServiceTypes.Add(serviceTypes);
                        }
                        else
                        {
                            serviceTypesData.ServiceType = serviceTypes.ServiceType;
                        }
                    }
                }
            }

            //if (data.Customers != null && data.Customers.Count > 0)
            //{
            //    if (entry.Customers == null)
            //        entry.Customers = data.Customers;
            //    else
            //    {
            //        foreach (var customers in data.Customers)
            //        {
            //            var customersData = entry.Customers.FirstOrDefault(x => x.CustomerID == customers.CustomerID);
            //            if (customersData == null)
            //            {
            //                entry.Customers.Add(customers);
            //            }
            //            else
            //            {
            //                customersData.Contact = customers.Contact;
            //                customersData.Company = customers.Company;
            //                customersData.EmailAddress = customers.EmailAddress;
            //                customersData.ARCustomerCode = customers.ARCustomerCode;
            //                customersData.NumberOfEmployees = customers.NumberOfEmployees;
            //                customersData.Attention = customers.Attention;
            //                customersData.ReferralSourceID = customers.ReferralSourceID;
            //                customersData.Notes = customers.Notes;
            //                customersData.EmailInvoice = customers.EmailInvoice;
            //                customersData.Zip = customers.Zip;
            //                customersData.Phone = customers.Phone;
            //                customersData.Fax = customers.Fax;
            //                customersData.Street = customers.Street;
            //                customersData.City = customers.City;
            //                customersData.State = customers.State;
            //                customersData.Mobile = customers.Mobile;
            //                customersData.PipelineStatus = customers.PipelineStatus;
            //                customersData.Suite = customers.Suite;
            //                customersData.EmailCOD = customers.EmailCOD;
            //                customersData.CustomerTypeID = customers.CustomerTypeID;
            //                customersData.InvoiceTypeID = customers.InvoiceTypeID;
            //                customersData.TermID = customers.TermID;
            //                customersData.DataSource = customers.DataSource;
            //            }
            //        }
            //    }
            //}

            //if (data.Buildings != null && data.Buildings.Count > 0)
            //{
            //    if (entry.Buildings == null)
            //        entry.Buildings = data.Buildings;
            //    else
            //    {
            //        foreach (var buildings in data.Buildings)
            //        {
            //            var buildingsData = entry.Buildings.FirstOrDefault(x => x.BuildingID == buildings.BuildingID);
            //            if (buildingsData == null)
            //            {
            //                entry.Buildings.Add(buildings);
            //            }
            //            else
            //            {
            //                buildingsData.CompanyName = buildings.CompanyName;
            //                buildingsData.Street = buildings.Street;
            //                buildingsData.City = buildings.City;
            //                buildingsData.State = buildings.State;
            //                buildingsData.Zip = buildings.Zip;
            //                buildingsData.SalesmanID = buildings.SalesmanID;
            //                buildingsData.Directions = buildings.Directions;
            //                buildingsData.RoutineInstructions = buildings.RoutineInstructions;
            //                buildingsData.ScheduleFrequency = buildings.ScheduleFrequency;
            //                buildingsData.ServiceTypeID = buildings.ServiceTypeID;
            //                buildingsData.RouteID = buildings.RouteID;
            //                buildingsData.BuildingTypeID = buildings.BuildingTypeID;
            //                buildingsData.Suite = buildings.Suite;
            //                buildingsData.SiteContact1 = buildings.SiteContact1;
            //                buildingsData.SiteContact2 = buildings.SiteContact2;
            //                buildingsData.Phone1 = buildings.Phone1;
            //                buildingsData.Phone2 = buildings.Phone2;
            //            }
            //        }
            //    }
            //}

            if (data.Frequencys != null && data.Frequencys.Count > 0)
            {
                if (entry.Frequencys == null)
                    entry.Frequencys = data.Frequencys;
                else
                {
                    foreach (var frequencys in data.Frequencys)
                    {
                        var frequencysData = entry.Frequencys.FirstOrDefault(x => x.ScheduleFrequency == frequencys.ScheduleFrequency);
                        if (frequencysData == null)
                        {
                            entry.Frequencys.Add(frequencys);
                        }
                        else
                        {
                            frequencysData.Frequency = frequencys.Frequency;
                        }
                    }
                }
            }

            if (data.ReferralSources != null && data.ReferralSources.Count > 0)
            {
                if (entry.ReferralSources == null)
                    entry.ReferralSources = data.ReferralSources;
                else
                {
                    foreach (var referralSources in data.ReferralSources)
                    {
                        var referralSourcesData = entry.ReferralSources.FirstOrDefault(x => x.ReferralSourceID == referralSources.ReferralSourceID);
                        if (referralSourcesData == null)
                        {
                            entry.ReferralSources.Add(referralSources);
                        }
                        else
                        {
                            referralSourcesData.ReferralSource = referralSources.ReferralSource;
                        }
                    }
                }
            }

            if (data.TermTypes != null && data.TermTypes.Count > 0)
            {
                if (entry.TermTypes == null)
                    entry.TermTypes = data.TermTypes;
                else
                {
                    foreach (var termTypes in data.TermTypes)
                    {
                        var termTypesData = entry.TermTypes.FirstOrDefault(x => x.TermID == termTypes.TermID);
                        if (termTypesData == null)
                        {
                            entry.TermTypes.Add(termTypes);
                        }
                        else
                        {
                            termTypesData.Terms = termTypes.Terms;
                        }
                    }
                }
            }

            if (data.InvoiceTypes != null && data.InvoiceTypes.Count > 0)
            {
                if (entry.InvoiceTypes == null)
                    entry.InvoiceTypes = data.InvoiceTypes;
                else
                {
                    foreach (var invoiceTypes in data.InvoiceTypes)
                    {
                        var invoiceTypesData = entry.InvoiceTypes.FirstOrDefault(x => x.InvoiceTypeID == invoiceTypes.InvoiceTypeID);
                        if (invoiceTypesData == null)
                        {
                            entry.InvoiceTypes.Add(invoiceTypes);
                        }
                        else
                        {
                            invoiceTypesData.InvoiceType = invoiceTypes.InvoiceType;
                        }
                    }
                }
            }

            if (data.SSField != null && data.SSField.Count > 0)
            {
                if (entry.SSField == null)
                    entry.SSField = data.SSField;
                else
                {
                    foreach (var ssField in data.SSField)
                    {
                        var ssFieldData = entry.SSField.FirstOrDefault(x => x.SSSystemName == ssField.SSSystemName);
                        if (ssFieldData == null)
                        {
                            entry.SSField.Add(ssField);
                        }
                        else
                        {
                            ssFieldData.Label = ssField.Label;
                            ssFieldData.EZShredFieldName = ssField.EZShredFieldName;
                            ssFieldData.SSSystemName = ssField.SSSystemName;
                            ssFieldData.Type = ssField.Type;
                            ssFieldData.Set = ssField.Set;
                        }
                    }
                }
            }

            entry.LastModifiedDate = DateTime.Now;
            return entry;
        }
        #endregion
    }
}
