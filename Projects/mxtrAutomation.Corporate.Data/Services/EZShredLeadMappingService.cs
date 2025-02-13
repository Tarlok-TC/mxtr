using mxtrAutomation.Api.EZShred;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Data.Repository;
using System.Collections.Generic;
using System.Linq;
using System;
using mxtrAutomation.Common.Enums;
using Newtonsoft.Json;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class EZShredLeadMappingService : MongoRepository<EZShredLeadMapping>, IEZShredLeadMappingInternal
    {
        public List<EZShredLeadMappingDataModel> GetCreateUpdateCustomerData(string accountObjectID)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                var result = repo.Where(w =>
                 ((w.EZShredActionType == EZShredActionTypeKind.Create.ToString() || w.EZShredActionType == EZShredActionTypeKind.Update.ToString()) && w.EZShredStatus == EZShredStatusKind.Failed.ToString() && w.AccountObjectID == accountObjectID) ||
                 ((w.Building1.EZShredActionType == EZShredActionTypeKind.Create.ToString() || w.Building1.EZShredActionType == EZShredActionTypeKind.Update.ToString()) && w.Building1.EZShredStatus == EZShredStatusKind.Failed.ToString() && w.AccountObjectID == accountObjectID) ||
                 ((w.Building2.EZShredActionType == EZShredActionTypeKind.Create.ToString() || w.Building2.EZShredActionType == EZShredActionTypeKind.Update.ToString()) && w.Building2.EZShredStatus == EZShredStatusKind.Failed.ToString() && w.AccountObjectID == accountObjectID) ||
                 ((w.Building3.EZShredActionType == EZShredActionTypeKind.Create.ToString() || w.Building3.EZShredActionType == EZShredActionTypeKind.Update.ToString()) && w.Building3.EZShredStatus == EZShredStatusKind.Failed.ToString() && w.AccountObjectID == accountObjectID) ||
                 ((w.Building4.EZShredActionType == EZShredActionTypeKind.Create.ToString() || w.Building4.EZShredActionType == EZShredActionTypeKind.Update.ToString()) && w.Building4.EZShredStatus == EZShredStatusKind.Failed.ToString() && w.AccountObjectID == accountObjectID) ||
                 ((w.Building5.EZShredActionType == EZShredActionTypeKind.Create.ToString() || w.Building5.EZShredActionType == EZShredActionTypeKind.Update.ToString()) && w.Building5.EZShredStatus == EZShredStatusKind.Failed.ToString() && w.AccountObjectID == accountObjectID)
                 && w.AccountObjectID == accountObjectID
               ).ToList();

                return result.Select(x => new EZShredLeadMappingDataModel
                {
                    Id = x.Id,
                    AccountObjectID = x.AccountObjectID,
                    CustomerID = x.CustomerID,
                    EZShredApiRequest = x.EZShredApiRequest,
                    EZShredBuildingApiRequest = x.EZShredBuildingApiRequest,
                    LeadID = x.LeadID,
                    OpportunityID = x.OpportunityID,
                    MxtrAccountID = x.MxtrAccountID,
                    MxtrUserID = x.MxtrUserID,
                    UserID = x.UserID,
                    EZShredStatus = x.EZShredStatus,
                    EZShredActionType = x.EZShredActionType,
                    Building1 = new LeadBuildingDataModel()
                    {
                        BuildingCompanyName = x.Building1 == null ? "" : x.Building1.BuildingCompanyName,
                        BuildingID = x.Building1 == null ? 0 : x.Building1.BuildingID,
                        EZShredActionType = x.Building1 == null ? EZShredActionTypeKind.NoAction.ToString() : x.Building1.EZShredActionType,
                        EZShredStatus = x.Building1 == null ? EZShredStatusKind.Complete.ToString() : x.Building1.EZShredStatus,
                        EZShredBuildingApiRequest = x.Building1 == null ? "" : x.Building1.EZShredBuildingApiRequest,
                        OpportunityID = x.Building1 == null ? 0 : x.Building1.OpportunityID,
                        Street = x.Building1 == null ? "" : x.Building1.Street,
                        ZIP = x.Building1 == null ? "" : x.Building1.ZIP,
                        City = x.Building1 == null ? "" : x.Building1.City,
                    },
                    Building2 = new LeadBuildingDataModel
                    {
                        BuildingCompanyName = x.Building2 == null ? "" : x.Building2.BuildingCompanyName,
                        BuildingID = x.Building2 == null ? 0 : x.Building2.BuildingID,
                        EZShredActionType = x.Building2 == null ? EZShredActionTypeKind.NoAction.ToString() : x.Building2.EZShredActionType,
                        EZShredStatus = x.Building2 == null ? EZShredStatusKind.Complete.ToString() : x.Building2.EZShredStatus,
                        EZShredBuildingApiRequest = x.Building2 == null ? "" : x.Building2.EZShredBuildingApiRequest,
                        OpportunityID = x.Building2 == null ? 0 : x.Building2.OpportunityID,
                        Street = x.Building2 == null ? "" : x.Building2.Street,
                        ZIP = x.Building2 == null ? "" : x.Building2.ZIP,
                        City = x.Building2 == null ? "" : x.Building2.City,
                    },
                    Building3 = new LeadBuildingDataModel
                    {
                        BuildingCompanyName = x.Building3 == null ? "" : x.Building3.BuildingCompanyName,
                        BuildingID = x.Building3 == null ? 0 : x.Building3.BuildingID,
                        EZShredActionType = x.Building3 == null ? EZShredActionTypeKind.NoAction.ToString() : x.Building3.EZShredActionType,
                        EZShredStatus = x.Building3 == null ? EZShredStatusKind.Complete.ToString() : x.Building3.EZShredStatus,
                        EZShredBuildingApiRequest = x.Building3 == null ? "" : x.Building3.EZShredBuildingApiRequest,
                        OpportunityID = x.Building3 == null ? 0 : x.Building3.OpportunityID,
                        Street = x.Building3 == null ? "" : x.Building3.Street,
                        ZIP = x.Building3 == null ? "" : x.Building3.ZIP,
                        City = x.Building3 == null ? "" : x.Building3.City,
                    },
                    Building4 = new LeadBuildingDataModel
                    {
                        BuildingCompanyName = x.Building4 == null ? "" : x.Building4.BuildingCompanyName,
                        BuildingID = x.Building4 == null ? 0 : x.Building4.BuildingID,
                        EZShredActionType = x.Building4 == null ? EZShredActionTypeKind.NoAction.ToString() : x.Building4.EZShredActionType,
                        EZShredStatus = x.Building4 == null ? EZShredStatusKind.Complete.ToString() : x.Building4.EZShredStatus,
                        EZShredBuildingApiRequest = x.Building4 == null ? "" : x.Building4.EZShredBuildingApiRequest,
                        OpportunityID = x.Building4 == null ? 0 : x.Building4.OpportunityID,
                        Street = x.Building4 == null ? "" : x.Building4.Street,
                        ZIP = x.Building4 == null ? "" : x.Building4.ZIP,
                        City = x.Building4 == null ? "" : x.Building4.City,
                    },
                    Building5 = new LeadBuildingDataModel
                    {
                        BuildingCompanyName = x.Building5 == null ? "" : x.Building5.BuildingCompanyName,
                        BuildingID = x.Building5 == null ? 0 : x.Building5.BuildingID,
                        EZShredActionType = x.Building5 == null ? EZShredActionTypeKind.NoAction.ToString() : x.Building5.EZShredActionType,
                        EZShredStatus = x.Building5 == null ? EZShredStatusKind.Complete.ToString() : x.Building5.EZShredStatus,
                        EZShredBuildingApiRequest = x.Building5 == null ? "" : x.Building5.EZShredBuildingApiRequest,
                        OpportunityID = x.Building5 == null ? 0 : x.Building5.OpportunityID,
                        Street = x.Building5 == null ? "" : x.Building5.Street,
                        ZIP = x.Building5 == null ? "" : x.Building5.ZIP,
                        City = x.Building5 == null ? "" : x.Building5.City,
                    },
                }).ToList();

                //return repo.Where(w =>
                //(w.EZShredActionType == EZShredActionTypeKind.Create.ToString() || w.EZShredActionType == EZShredActionTypeKind.Update.ToString())
                //&& w.AccountObjectID == accountObjectID
                //&& w.EZShredStatus == EZShredStatusKind.Failed.ToString()
                //)
                //    .Select(x => new EZShredLeadMappingDataModel
                //    {
                //        Id = x.Id,
                //        AccountObjectID = x.AccountObjectID,
                //        CustomerID = x.CustomerID,
                //        EZShredApiRequest = x.EZShredApiRequest,
                //        EZShredBuildingApiRequest = x.EZShredBuildingApiRequest,
                //        LeadID = x.LeadID,
                //        OpportunityID = x.OpportunityID,
                //        MxtrAccountID = x.MxtrAccountID,
                //        MxtrUserID = x.MxtrUserID,
                //        UserID = x.UserID,
                //        EZShredStatus = x.EZShredStatus,
                //        EZShredActionType = x.EZShredActionType,
                //    }).ToList();
            }
        }
        public EZShredLeadMappingDataModel GetEZShredLeadDataByAccountId(string accountObjectID)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                EZShredLeadMapping data = repo.FirstOrDefault(w => w.AccountObjectID == accountObjectID);
                return AdaptData(data);
            }
        }
        public List<EZShredLeadMappingDataModel> GetEZShredLeadDataByCustomerId(string accountObjectId, int customerId)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                return repo.Where(w =>
                w.CustomerID == customerId
                && w.AccountObjectID == accountObjectId
                && (w.EZShredStatus == EZShredStatusKind.Complete.ToString())
                )
                    .Select(x => new EZShredLeadMappingDataModel
                    {
                        Id = x.Id,
                        AccountObjectID = x.AccountObjectID,
                        CustomerID = x.CustomerID,
                        EZShredApiRequest = x.EZShredApiRequest,
                        EZShredBuildingApiRequest = x.EZShredBuildingApiRequest,
                        LeadID = x.LeadID,
                        OpportunityID = x.OpportunityID,
                        MxtrAccountID = x.MxtrAccountID,
                        MxtrUserID = x.MxtrUserID,
                        UserID = x.UserID,
                        EZShredStatus = x.EZShredStatus,
                        EZShredActionType = x.EZShredActionType,
                        Company = x.Company,
                        Street = x.Street,
                        ZIP = x.ZIP,
                    }).ToList();
            }
        }
        public List<EZShredLeadMappingDataModel> GetCustomerDataFromEZShredLeadMapping(string accountObjectID)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                return repo.Where(w =>
                w.AccountObjectID == accountObjectID
                && (w.EZShredStatus == EZShredStatusKind.Complete.ToString() || w.EZShredStatus == EZShredStatusKind.Failed.ToString())
                )
                    .Select(x => new EZShredLeadMappingDataModel
                    {
                        Id = x.Id,
                        AccountObjectID = x.AccountObjectID,
                        CustomerID = x.CustomerID,
                        EZShredApiRequest = x.EZShredApiRequest,
                        EZShredBuildingApiRequest = x.EZShredBuildingApiRequest,
                        LeadID = x.LeadID,
                        OpportunityID = x.OpportunityID,
                        MxtrAccountID = x.MxtrAccountID,
                        MxtrUserID = x.MxtrUserID,
                        UserID = x.UserID,
                        EZShredStatus = x.EZShredStatus,
                        EZShredActionType = x.EZShredActionType,
                        Company = x.Company,
                        Street = x.Street,
                        ZIP = x.ZIP,
                    }).ToList();
            }
        }
        public List<EZShredLeadMappingDataModel> GetCustomerDataFromEZShredLeadMapping(string accountObjectID, string searchText)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                return repo.Where(w =>
                w.AccountObjectID == accountObjectID
                && w.Company.ToUpper().Contains(searchText)
                && (w.EZShredStatus == EZShredStatusKind.Complete.ToString() || w.EZShredStatus == EZShredStatusKind.Failed.ToString())
                )
                    .Select(x => new EZShredLeadMappingDataModel
                    {
                        Id = x.Id,
                        AccountObjectID = x.AccountObjectID,
                        CustomerID = x.CustomerID,
                        EZShredApiRequest = x.EZShredApiRequest,
                        EZShredBuildingApiRequest = x.EZShredBuildingApiRequest,
                        LeadID = x.LeadID,
                        OpportunityID = x.OpportunityID,
                        MxtrAccountID = x.MxtrAccountID,
                        MxtrUserID = x.MxtrUserID,
                        UserID = x.UserID,
                        EZShredStatus = x.EZShredStatus,
                        EZShredActionType = x.EZShredActionType,
                        Company = x.Company,
                        Street = x.Street,
                        ZIP = x.ZIP,
                        Building1 = new LeadBuildingDataModel()
                        {
                            BuildingCompanyName = x.Building1 == null ? "" : x.Building1.BuildingCompanyName,
                            BuildingID = x.Building1 == null ? 0 : x.Building1.BuildingID,
                            OpportunityID = x.Building1 == null ? 0 : x.Building1.OpportunityID,
                            Street = x.Building1 == null ? "" : x.Building1.Street,
                            ZIP = x.Building1 == null ? "" : x.Building1.ZIP,
                            City = x.Building1 == null ? "" : x.Building1.City,
                        },
                        Building2 = new LeadBuildingDataModel
                        {
                            BuildingCompanyName = x.Building2 == null ? "" : x.Building2.BuildingCompanyName,
                            BuildingID = x.Building2 == null ? 0 : x.Building2.BuildingID,
                            OpportunityID = x.Building2 == null ? 0 : x.Building2.OpportunityID,
                            Street = x.Building2 == null ? "" : x.Building2.Street,
                            ZIP = x.Building2 == null ? "" : x.Building2.ZIP,
                            City = x.Building2 == null ? "" : x.Building2.City,
                        },
                        Building3 = new LeadBuildingDataModel
                        {
                            BuildingCompanyName = x.Building3 == null ? "" : x.Building3.BuildingCompanyName,
                            BuildingID = x.Building3 == null ? 0 : x.Building3.BuildingID,
                            OpportunityID = x.Building3 == null ? 0 : x.Building3.OpportunityID,
                            Street = x.Building3 == null ? "" : x.Building3.Street,
                            ZIP = x.Building3 == null ? "" : x.Building3.ZIP,
                            City = x.Building3 == null ? "" : x.Building3.City,
                        },
                        Building4 = new LeadBuildingDataModel
                        {
                            BuildingCompanyName = x.Building4 == null ? "" : x.Building4.BuildingCompanyName,
                            BuildingID = x.Building4 == null ? 0 : x.Building4.BuildingID,
                            OpportunityID = x.Building4 == null ? 0 : x.Building4.OpportunityID,
                            Street = x.Building4 == null ? "" : x.Building4.Street,
                            ZIP = x.Building4 == null ? "" : x.Building4.ZIP,
                            City = x.Building4 == null ? "" : x.Building4.City,
                        },
                        Building5 = new LeadBuildingDataModel
                        {
                            BuildingCompanyName = x.Building5 == null ? "" : x.Building5.BuildingCompanyName,
                            BuildingID = x.Building5 == null ? 0 : x.Building5.BuildingID,
                            OpportunityID = x.Building5 == null ? 0 : x.Building5.OpportunityID,
                            Street = x.Building5 == null ? "" : x.Building5.Street,
                            ZIP = x.Building5 == null ? "" : x.Building5.ZIP,
                            City = x.Building5 == null ? "" : x.Building5.City,
                        },
                    }).ToList();
            }
        }
        public EZShredLeadMappingDataModel GetOpportunityIdByLeadId(string accountObjectID, long leadId)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                EZShredLeadMapping data = repo.Where(
                    w => w.AccountObjectID == accountObjectID && w.LeadID == leadId).FirstOrDefault();
                return AdaptData(data);
            }
        }

        public EZShredLeadMappingDataModel AddEZShredLeadMappingData(EZShredLeadMappingDataModel data)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                EZShredLeadMapping entity = Getdata(data);
                repo.Add(entity);
                return AdaptData(entity);
            }
        }

        public bool UpdateEZShredLeadMappingData(EZShredLeadMappingDataModel data)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                try
                {
                    var entity = repo.FirstOrDefault(x => x.Id == data.Id);
                    if (entity != null)
                    {
                        //entity.AccountObjectID = data.AccountObjectID;
                        // entity.MxtrAccountID = data.MxtrAccountID;
                        //entity.UserID = data.UserID;
                        // entity.MxtrUserID = data.MxtrUserID;
                        entity.LeadID = data.LeadID;
                        entity.OpportunityID = data.OpportunityID;
                        if (data.CustomerID != null && data.CustomerID != 0)
                        {
                            entity.CustomerID = data.CustomerID;
                        }
                        entity.Company = data.Company;
                        entity.Street = data.Street;
                        entity.ZIP = data.ZIP;
                        entity.EZShredApiRequest = data.EZShredApiRequest;
                        entity.EZShredBuildingApiRequest = data.EZShredBuildingApiRequest;
                        entity.EZShredActionType = data.EZShredActionType;
                        entity.EZShredStatus = data.EZShredStatus;
                        //entity.CreateDate = DateTime.Now;
                        entity.LastUpdatedDate = DateTime.Now;
                        repo.Update(entity);
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool UpdateEZShredLeadMappingDataWithLeadID(EZShredLeadMappingDataModel data, string buildingSet)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                try
                {
                    var entity = repo.FirstOrDefault(x => x.LeadID == data.LeadID);
                    if (entity != null)
                    {
                        entity.OpportunityID = data.OpportunityID;
                        if (data.CustomerID != null && data.CustomerID != 0)
                        {
                            entity.CustomerID = data.CustomerID;
                        }
                        entity.Company = data.Company;
                        entity.Street = data.Street;
                        entity.ZIP = data.ZIP;
                        entity.EZShredApiRequest = data.EZShredApiRequest;
                        entity.EZShredBuildingApiRequest = data.EZShredBuildingApiRequest;
                        entity.EZShredActionType = data.EZShredActionType;
                        entity.EZShredStatus = data.EZShredStatus;
                        entity.LastUpdatedDate = DateTime.Now;

                        if (buildingSet == LeadBuildingSet.Building1.ToString())
                        {
                            entity.Building1 = SetBuildingData(data.Building1, data.EZShredBuildingApiRequest, data.EZShredActionType, data.EZShredStatus, data.OpportunityID);
                        }
                        else if (buildingSet == LeadBuildingSet.Building2.ToString())
                        {
                            entity.Building2 = SetBuildingData(data.Building2, data.EZShredBuildingApiRequest, data.EZShredActionType, data.EZShredStatus, data.OpportunityID);
                        }
                        else if (buildingSet == LeadBuildingSet.Building3.ToString())
                        {
                            entity.Building3 = SetBuildingData(data.Building3, data.EZShredBuildingApiRequest, data.EZShredActionType, data.EZShredStatus, data.OpportunityID);
                        }
                        else if (buildingSet == LeadBuildingSet.Building4.ToString())
                        {
                            entity.Building4 = SetBuildingData(data.Building4, data.EZShredBuildingApiRequest, data.EZShredActionType, data.EZShredStatus, data.OpportunityID);
                        }
                        else if (buildingSet == LeadBuildingSet.Building5.ToString())
                        {
                            entity.Building5 = SetBuildingData(data.Building5, data.EZShredBuildingApiRequest, data.EZShredActionType, data.EZShredStatus, data.OpportunityID);
                        }

                        repo.Update(entity);
                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private LeadBuilding SetBuildingData(LeadBuildingDataModel building, string ezShredBuildingApiRequest, string ezShredActionType, string EZShredStatus, long opportunityID)
        {
            return new LeadBuilding
            {
                BuildingCompanyName = building == null ? String.Empty : building.BuildingCompanyName,
                BuildingID = building == null ? 0 : building.BuildingID,
                //OpportunityID = building == null ? 0 : building.OpportunityID,
                OpportunityID = opportunityID,
                City = building == null ? String.Empty : building.City,
                Country = building == null ? String.Empty : building.Country,
                Street = building == null ? String.Empty : building.Street,
                State = building == null ? String.Empty : building.State,
                ZIP = building == null ? String.Empty : building.ZIP,
                // ToDo (Need to check)
                EZShredBuildingApiRequest = ezShredBuildingApiRequest,
                EZShredActionType = ezShredActionType,
                EZShredStatus = EZShredStatus,
            };
        }

        public bool AddLeadBuilding(EZShredLeadMappingDataModel data, LeadBuildingDataModel LeadBuilding, string BuildingSet)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                try
                {
                    var entity = repo.FirstOrDefault(x => x.LeadID == data.LeadID);
                    if (entity != null)
                    {
                        LeadBuilding objLeadBuilding = new LeadBuilding();

                        if (LeadBuilding.BuildingID == 0)
                            objLeadBuilding.BuildingID = null;
                        else
                            objLeadBuilding.BuildingID = LeadBuilding.BuildingID;

                        objLeadBuilding.BuildingCompanyName = LeadBuilding.BuildingCompanyName;
                        objLeadBuilding.OpportunityID = LeadBuilding.OpportunityID;
                        objLeadBuilding.EZShredBuildingApiRequest = LeadBuilding.EZShredBuildingApiRequest;
                        objLeadBuilding.EZShredStatus = LeadBuilding.EZShredStatus;
                        objLeadBuilding.EZShredActionType = LeadBuilding.EZShredActionType;
                        objLeadBuilding.CreateDate = DateTime.Now;
                        objLeadBuilding.Street = LeadBuilding.Street;
                        objLeadBuilding.City = LeadBuilding.City;
                        objLeadBuilding.State = LeadBuilding.State;
                        objLeadBuilding.ZIP = LeadBuilding.ZIP;
                        objLeadBuilding.Country = LeadBuilding.Country;

                        if (BuildingSet == LeadBuildingSet.Building1.ToString())
                            entity.Building1 = objLeadBuilding;
                        else if (BuildingSet == LeadBuildingSet.Building2.ToString())
                            entity.Building2 = objLeadBuilding;
                        else if (BuildingSet == LeadBuildingSet.Building3.ToString())
                            entity.Building3 = objLeadBuilding;
                        else if (BuildingSet == LeadBuildingSet.Building4.ToString())
                            entity.Building4 = objLeadBuilding;
                        else
                            entity.Building5 = objLeadBuilding;

                        entity.BuildingStage = BuildingSet;
                        repo.Update(entity);
                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public bool UpdateCustomerId(AddUpdateCustomerResult data, string Id)
        {
            try
            {
                using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
                {
                    var entity = repo.FirstOrDefault(x => x.Id == Id);
                    if (entity != null)
                    {
                        entity.CustomerID = Convert.ToInt32(data.CustomerID);
                        repo.Update(entity);
                    }
                }

                return true;
            }
            catch (System.Exception)
            {

                return false;
            }
        }
        public bool UpdateBuildingId(string leadMappingId, int buildingId, LeadBuildingSet whichBuilding)
        {
            if (buildingId <= 0)
            {
                return false;
            }
            try
            {
                using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
                {
                    var entity = repo.FirstOrDefault(x => x.Id == leadMappingId);
                    if (entity != null)
                    {
                        switch (whichBuilding)
                        {
                            case LeadBuildingSet.Building1:
                                entity.Building1.BuildingID = buildingId;
                                break;
                            case LeadBuildingSet.Building2:
                                entity.Building2.BuildingID = buildingId;
                                break;
                            case LeadBuildingSet.Building3:
                                entity.Building3.BuildingID = buildingId;
                                break;
                            case LeadBuildingSet.Building4:
                                entity.Building4.BuildingID = buildingId;
                                break;
                            case LeadBuildingSet.Building5:
                                entity.Building5.BuildingID = buildingId;
                                break;
                            //case LeadBuildingSet.Complete:
                            //    break;
                            default:
                                break;
                        }
                        repo.Update(entity);
                        return true;
                    }
                }
                return false;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        public bool UpdateOpportunityId(long OpportunityId, string Id)
        {
            try
            {
                using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
                {
                    var entity = repo.FirstOrDefault(x => x.Id == Id);
                    if (entity != null)
                    {
                        entity.OpportunityID = OpportunityId;
                        repo.Update(entity);
                    }
                }
                return true;
            }
            catch (System.Exception)
            {

                return false;
            }
        }
        public bool UpdateRequestStatus(string Id, EZShredStatusKind status)
        {
            try
            {
                using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
                {
                    var entity = repo.FirstOrDefault(x => x.Id == Id);
                    if (entity != null)
                    {
                        entity.EZShredStatus = status.ToString();
                        repo.Update(entity);
                        return true;
                    }
                }
                return false;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        public bool UpdateRequestStatus(string Id, EZShredStatusKind status, LeadBuildingSet whichBuilding)
        {
            try
            {
                using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
                {
                    var entity = repo.FirstOrDefault(x => x.Id == Id);
                    if (entity != null)
                    {
                        switch (whichBuilding)
                        {
                            case LeadBuildingSet.Building1:
                                entity.Building1.EZShredStatus = status.ToString();
                                break;
                            case LeadBuildingSet.Building2:
                                entity.Building2.EZShredStatus = status.ToString();
                                break;
                            case LeadBuildingSet.Building3:
                                entity.Building3.EZShredStatus = status.ToString();
                                break;
                            case LeadBuildingSet.Building4:
                                entity.Building4.EZShredStatus = status.ToString();
                                break;
                            case LeadBuildingSet.Building5:
                                entity.Building5.EZShredStatus = status.ToString();
                                break;
                            //case LeadBuildingSet.Complete:
                            //    break;
                            default:
                                break;
                        }
                        repo.Update(entity);
                        return true;
                    }
                }
                return false;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public EZShredLeadMappingDataModel GetEZShredLeadDataByLeadID(string accountObjectID, long leadId)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                EZShredLeadMapping data = repo.Where(
                    w => w.AccountObjectID == accountObjectID && w.LeadID == leadId).FirstOrDefault();
                return AdaptData(data);
            }
        }
        private EZShredLeadMapping Getdata(EZShredLeadMappingDataModel data)
        {
            return new EZShredLeadMapping()
            {
                Id = data.Id,
                AccountObjectID = data.AccountObjectID,
                MxtrAccountID = data.MxtrAccountID,
                UserID = data.UserID,
                MxtrUserID = data.MxtrUserID,
                LeadID = data.LeadID,
                OpportunityID = data.OpportunityID,
                CustomerID = data.CustomerID,
                Company = data.Company,
                Street = data.Street,
                ZIP = data.ZIP,
                EZShredApiRequest = data.EZShredApiRequest,
                EZShredBuildingApiRequest = data.EZShredBuildingApiRequest,
                EZShredActionType = data.EZShredActionType,
                EZShredStatus = data.EZShredStatus,
                CreateDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now,
            };
        }
        private EZShredLeadMappingDataModel AdaptData(EZShredLeadMapping data)
        {
            if (data == null)
            {
                return new EZShredLeadMappingDataModel();
            }
            return new EZShredLeadMappingDataModel()
            {
                Id = data.Id,
                AccountObjectID = data.AccountObjectID,
                MxtrAccountID = data.MxtrAccountID,
                UserID = data.UserID,
                MxtrUserID = data.MxtrUserID,
                LeadID = data.LeadID,
                OpportunityID = data.OpportunityID,
                CustomerID = data.CustomerID,
                Company = data.Company,
                Street = data.Street,
                ZIP = data.ZIP,
                EZShredApiRequest = data.EZShredApiRequest,
                EZShredBuildingApiRequest = data.EZShredBuildingApiRequest,
                EZShredActionType = data.EZShredActionType,
                EZShredStatus = data.EZShredStatus,
                Building1 = BindLeadDataModel(data.Building1),
                Building2 = BindLeadDataModel(data.Building2),
                Building3 = BindLeadDataModel(data.Building3),
                Building4 = BindLeadDataModel(data.Building4),
                Building5 = BindLeadDataModel(data.Building5),
                BuildingStage = data.BuildingStage,
            };
        }
        private LeadBuildingDataModel BindLeadDataModel(LeadBuilding ObjLeadBuilding)
        {
            if (ObjLeadBuilding != null)
            {
                return new LeadBuildingDataModel()
                {
                    BuildingID = ObjLeadBuilding.BuildingID,
                    BuildingCompanyName = ObjLeadBuilding.BuildingCompanyName,
                    OpportunityID = ObjLeadBuilding.OpportunityID,
                    EZShredBuildingApiRequest = ObjLeadBuilding.EZShredBuildingApiRequest,
                    EZShredActionType = ObjLeadBuilding.EZShredActionType,
                    EZShredStatus = ObjLeadBuilding.EZShredStatus,
                    CreateDate = ObjLeadBuilding.CreateDate,
                    LastUpdatedDate = ObjLeadBuilding.LastUpdatedDate,
                    Street = ObjLeadBuilding.Street,
                    City = ObjLeadBuilding.City,
                    Country = ObjLeadBuilding.Country,
                    State = ObjLeadBuilding.State,
                    ZIP = ObjLeadBuilding.ZIP,
                };
            }
            else
            {
                return new LeadBuildingDataModel();
            }
        }
        public List<EZShredLeadMappingDataModel> GetBuildingsDataFromEZShredLeadMapping(string accountObjectID, string searchText)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                var result = repo.Where(w =>
                  w.AccountObjectID == accountObjectID
                  && (w.Building1.BuildingCompanyName.ToUpper().Contains(searchText)
                  || w.Building2.BuildingCompanyName.ToUpper().Contains(searchText)
                  || w.Building3.BuildingCompanyName.ToUpper().Contains(searchText)
                  || w.Building4.BuildingCompanyName.ToUpper().Contains(searchText)
                  || w.Building5.BuildingCompanyName.ToUpper().Contains(searchText)
                  )
                  && (w.EZShredStatus == EZShredStatusKind.Complete.ToString() || w.EZShredStatus == EZShredStatusKind.Failed.ToString())
                ).ToList();

                var buildings = result.Select(x => new EZShredLeadMappingDataModel
                {
                    Id = x.Id,
                    AccountObjectID = x.AccountObjectID,
                    CustomerID = x.CustomerID,
                    EZShredApiRequest = x.EZShredApiRequest,
                    EZShredBuildingApiRequest = x.EZShredBuildingApiRequest,
                    LeadID = x.LeadID,
                    OpportunityID = x.OpportunityID,
                    MxtrAccountID = x.MxtrAccountID,
                    MxtrUserID = x.MxtrUserID,
                    UserID = x.UserID,
                    EZShredStatus = x.EZShredStatus,
                    EZShredActionType = x.EZShredActionType,
                    Company = x.Company,
                    Building1 = new LeadBuildingDataModel
                    {
                        BuildingCompanyName = x.Building1 == null ? "" : x.Building1.BuildingCompanyName,
                        BuildingID = x.Building1 == null ? 0 : x.Building1.BuildingID,
                        OpportunityID = x.Building1 == null ? 0 : x.Building1.OpportunityID,
                        Street = x.Building1 == null ? "" : x.Building1.Street,
                        ZIP = x.Building1 == null ? "" : x.Building1.ZIP,
                        City = x.Building1 == null ? "" : x.Building1.City,

                    },
                    Building2 = new LeadBuildingDataModel
                    {
                        BuildingCompanyName = x.Building2 == null ? "" : x.Building2.BuildingCompanyName,
                        BuildingID = x.Building2 == null ? 0 : x.Building2.BuildingID,
                        OpportunityID = x.Building2 == null ? 0 : x.Building2.OpportunityID,
                        Street = x.Building2 == null ? "" : x.Building2.Street,
                        ZIP = x.Building2 == null ? "" : x.Building2.ZIP,
                        City = x.Building2 == null ? "" : x.Building2.City,
                    },
                    Building3 = new LeadBuildingDataModel
                    {
                        BuildingCompanyName = x.Building3 == null ? "" : x.Building3.BuildingCompanyName,
                        BuildingID = x.Building3 == null ? 0 : x.Building3.BuildingID,
                        OpportunityID = x.Building3 == null ? 0 : x.Building3.OpportunityID,
                        Street = x.Building3 == null ? "" : x.Building3.Street,
                        ZIP = x.Building3 == null ? "" : x.Building3.ZIP,
                        City = x.Building3 == null ? "" : x.Building3.City,
                    },
                    Building4 = new LeadBuildingDataModel
                    {
                        BuildingCompanyName = x.Building4 == null ? "" : x.Building4.BuildingCompanyName,
                        BuildingID = x.Building4 == null ? 0 : x.Building4.BuildingID,
                        OpportunityID = x.Building4 == null ? 0 : x.Building4.OpportunityID,
                        Street = x.Building4 == null ? "" : x.Building4.Street,
                        ZIP = x.Building4 == null ? "" : x.Building4.ZIP,
                        City = x.Building4 == null ? "" : x.Building4.City,
                    },
                    Building5 = new LeadBuildingDataModel
                    {
                        BuildingCompanyName = x.Building5 == null ? "" : x.Building5.BuildingCompanyName,
                        BuildingID = x.Building5 == null ? 0 : x.Building5.BuildingID,
                        OpportunityID = x.Building5 == null ? 0 : x.Building5.OpportunityID,
                        Street = x.Building5 == null ? "" : x.Building5.Street,
                        ZIP = x.Building5 == null ? "" : x.Building5.ZIP,
                        City = x.Building5 == null ? "" : x.Building5.City,
                    },
                    Street = x.Street,
                    ZIP = x.ZIP,
                }).ToList();

                return buildings;

                //    return repo.Where(w =>
                //    w.AccountObjectID == accountObjectID
                //    && (w.Building1.BuildingCompanyName.ToUpper().Contains(searchText)
                //    || w.Building2.BuildingCompanyName.ToUpper().Contains(searchText)
                //    || w.Building3.BuildingCompanyName.ToUpper().Contains(searchText)
                //    || w.Building4.BuildingCompanyName.ToUpper().Contains(searchText)
                //    || w.Building5.BuildingCompanyName.ToUpper().Contains(searchText)
                //    )
                //    && (w.EZShredStatus == EZShredStatusKind.Complete.ToString() || w.EZShredStatus == EZShredStatusKind.Failed.ToString())
                //    )
                //        .Select(x => new EZShredLeadMappingDataModel
                //        {
                //            Id = x.Id,
                //            AccountObjectID = x.AccountObjectID,
                //            CustomerID = x.CustomerID,
                //            EZShredApiRequest = x.EZShredApiRequest,
                //            EZShredBuildingApiRequest = x.EZShredBuildingApiRequest,
                //            LeadID = x.LeadID,
                //            OpportunityID = x.OpportunityID,
                //            MxtrAccountID = x.MxtrAccountID,
                //            MxtrUserID = x.MxtrUserID,
                //            UserID = x.UserID,
                //            EZShredStatus = x.EZShredStatus,
                //            EZShredActionType = x.EZShredActionType,
                //            Company = x.Company,
                //            Building1 = new LeadBuildingDataModel
                //            {
                //                BuildingCompanyName = x.Building1.BuildingCompanyName,
                //                BuildingID = x.Building1.BuildingID,
                //            },
                //            Building2 = new LeadBuildingDataModel
                //            {
                //                BuildingCompanyName = x.Building2.BuildingCompanyName,
                //                BuildingID = x.Building2.BuildingID,
                //            },
                //            Building3 = new LeadBuildingDataModel
                //            {
                //                BuildingCompanyName = x.Building3.BuildingCompanyName,
                //                BuildingID = x.Building3.BuildingID,
                //            },
                //            Building4 = new LeadBuildingDataModel
                //            {
                //                BuildingCompanyName = x.Building4.BuildingCompanyName,
                //                BuildingID = x.Building4.BuildingID,
                //            },
                //            Building5 = new LeadBuildingDataModel
                //            {
                //                BuildingCompanyName = x.Building5.BuildingCompanyName,
                //                BuildingID = x.Building5.BuildingID,
                //            },
                //            Street = x.Street,
                //            ZIP = x.ZIP,
                //        }).ToList();
            }
        }
        public bool HandleOldBuildingData()
        {
            try
            {
                using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
                {
                    List<EZShredLeadMapping> lstEZShredData = repo.ToList();
                    List<EZShredLeadMapping> lstEZShredModifiedData = new List<EZShredLeadMapping>();
                    foreach (var data in lstEZShredData)
                    {
                        if (data.Building1 == null)
                        {
                            data.Building1 = new LeadBuilding();
                        }
                        data.Building1.OpportunityID = data.OpportunityID;
                        data.Building1.EZShredBuildingApiRequest = data.EZShredBuildingApiRequest;
                        data.Building1.EZShredActionType = data.EZShredActionType;
                        data.Building1.EZShredStatus = data.EZShredStatus;
                        //parse request to get data
                        try
                        {
                            BuildingDataModel buildingData = new BuildingDataModel();
                            buildingData = JsonConvert.DeserializeObject<BuildingDataModel>(data.EZShredBuildingApiRequest);
                            if (buildingData.BuildingData != null)
                            {
                                data.Building1.BuildingCompanyName = buildingData.BuildingData[0].CompanyName;
                                data.Building1.BuildingID = Convert.ToInt32(buildingData.BuildingData[0].BuildingID);
                                data.Building1.ZIP = buildingData.BuildingData[0].Zip;
                                data.Building1.Street = buildingData.BuildingData[0].Street;
                                data.Building1.State = buildingData.BuildingData[0].State;
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        data.Building1.CreateDate = data.CreateDate;
                        data.Building1.LastUpdatedDate = data.LastUpdatedDate;
                        lstEZShredModifiedData.Add(data);
                    }
                    repo.Update(lstEZShredModifiedData);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int GetBuildingCountByOpportunity(string accountObjectId, long opportunityId)
        {
            int buildingCountFromEZShredleadMapping = 0;
            int buildingCountFromEZShredBuildingData = 0;
            List<int> lstBuildingId = new List<int>();

            //get building count from EZShred lead mapping      
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                var data = repo.FirstOrDefault(a => a.AccountObjectID == accountObjectId &&
                (a.Building1.OpportunityID == opportunityId && a.AccountObjectID == accountObjectId) ||
                (a.Building2.OpportunityID == opportunityId && a.AccountObjectID == accountObjectId) ||
                (a.Building3.OpportunityID == opportunityId && a.AccountObjectID == accountObjectId) ||
                (a.Building4.OpportunityID == opportunityId && a.AccountObjectID == accountObjectId) ||
                (a.Building5.OpportunityID == opportunityId && a.AccountObjectID == accountObjectId));

                if (data != null)
                {
                    //add building Id to list
                    if (data.Building1 != null && data.Building1.OpportunityID > 0)
                    {
                        lstBuildingId.Add(Convert.ToInt32(data.Building1.OpportunityID));
                    }
                    if (data.Building2 != null && data.Building2.OpportunityID > 0)
                    {
                        lstBuildingId.Add(Convert.ToInt32(data.Building2.OpportunityID));
                    }
                    if (data.Building3 != null && data.Building3.OpportunityID > 0)
                    {
                        lstBuildingId.Add(Convert.ToInt32(data.Building3.OpportunityID));
                    }
                    if (data.Building4 != null && data.Building4.OpportunityID > 0)
                    {
                        lstBuildingId.Add(Convert.ToInt32(data.Building4.OpportunityID));
                    }
                    if (data.Building5 != null && data.Building5.OpportunityID > 0)
                    {
                        lstBuildingId.Add(Convert.ToInt32(data.Building5.OpportunityID));
                    }

                    if (data.BuildingStage == LeadBuildingSet.Building1.ToString())
                    {
                        buildingCountFromEZShredleadMapping = 1;
                    }
                    else if (data.BuildingStage == LeadBuildingSet.Building2.ToString())
                    {
                        buildingCountFromEZShredleadMapping = 2;
                    }
                    else if (data.BuildingStage == LeadBuildingSet.Building3.ToString())
                    {
                        buildingCountFromEZShredleadMapping = 3;
                    }
                    else if (data.BuildingStage == LeadBuildingSet.Building4.ToString())
                    {
                        buildingCountFromEZShredleadMapping = 4;
                    }
                    else if (data.BuildingStage == LeadBuildingSet.Building5.ToString())
                    {
                        buildingCountFromEZShredleadMapping = 5;
                    }

                    //get building count from EZShredBuildingData table                    
                    if (data.CustomerID > 0)
                    {
                        using (MongoRepository<EZShredBuildingData> repoEZShredBuildingData = new MongoRepository<EZShredBuildingData>())
                        {
                            List<EZShredBuildingData> buildings = repoEZShredBuildingData.Where(a => a.AccountObjectId == accountObjectId && a.CustomerID != null && a.CustomerID == Convert.ToString(data.CustomerID)).ToList();
                            buildings = buildings.Where(x => !lstBuildingId.Contains(Convert.ToInt32(x.OpportunityID))).ToList();
                            if (buildings != null && buildings.Count > 0)
                            {
                                buildingCountFromEZShredBuildingData = buildings.Count;
                            }
                        }
                    }
                }
            }
            return buildingCountFromEZShredleadMapping + buildingCountFromEZShredBuildingData;
        }

        public List<EZShredLeadMappingDataModel> GetAllEZShredLeadDataByAccountId(string accountObjectID)
        {
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                return repo.Where(w =>
                w.AccountObjectID == accountObjectID)
                    .Select(x => new EZShredLeadMappingDataModel
                    {
                        Id = x.Id,
                        AccountObjectID = x.AccountObjectID,
                        CustomerID = x.CustomerID,
                        EZShredApiRequest = x.EZShredApiRequest,
                        EZShredBuildingApiRequest = x.EZShredBuildingApiRequest,
                        LeadID = x.LeadID,
                        OpportunityID = x.OpportunityID,
                        MxtrAccountID = x.MxtrAccountID,
                        MxtrUserID = x.MxtrUserID,
                        UserID = x.UserID,
                        EZShredStatus = x.EZShredStatus,
                        EZShredActionType = x.EZShredActionType,
                        Company = x.Company,
                        Street = x.Street,
                        ZIP = x.ZIP,
                    }).ToList();
            }
        }
    }
}
