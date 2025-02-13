using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class CRMEZShredBuildingService : MongoRepository<EZShredBuildingData>, ICRMEZShredBuildingServiceInternal
    {
        public bool AddUpdateBuildingData(EZShredBuildingDataModel buildingData)
        {
            using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
            {
                try
                {
                    //EZShredBuildingData building = repo.FirstOrDefault(a => a.AccountObjectId == buildingData.AccountObjectId && a.MxtrAccountId == buildingData.MxtrAccountId && a.BuildingID == buildingData.BuildingID && a.OpportunityID == buildingData.OpportunityID);
                    EZShredBuildingData building = repo.FirstOrDefault(a => a.AccountObjectId == buildingData.AccountObjectId && a.MxtrAccountId == buildingData.MxtrAccountId && a.OpportunityID == buildingData.OpportunityID);
                    if (building == null)
                    {
                        building = GetBuildingDataEntity(buildingData);
                        building.CreatedOn = DateTime.UtcNow;
                        building.AccountObjectId = buildingData.AccountObjectId;
                        building.MxtrAccountId = buildingData.MxtrAccountId;
                        repo.Add(building);
                    }
                    else
                    {
                        building = GetBuildingDataEntity(buildingData, building.Id);
                        building.CreatedOn = building.CreatedOn;
                        building.ModifiedOn = DateTime.UtcNow;
                        building.AccountObjectId = building.AccountObjectId;
                        building.MxtrAccountId = building.MxtrAccountId;
                        repo.Update(building);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public bool AddUpdateBuildingData(List<EZShredBuildingDataModel> lstBuildingData, string accountObjectId, string mxtrAccountId)
        {
            if (lstBuildingData != null && lstBuildingData.Count > 0)
            {
                lstBuildingData.ForEach(x => { x.AccountObjectId = accountObjectId; x.MxtrAccountId = mxtrAccountId; });
                using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
                {
                    try
                    {
                        List<EZShredBuildingData> entry = repo.Where(a => a.AccountObjectId == accountObjectId && a.MxtrAccountId == mxtrAccountId).ToList();
                        List<EZShredBuildingData> data = new List<EZShredBuildingData>();
                        foreach (var building in lstBuildingData)
                        {
                            EZShredBuildingData buildingData = entry.FirstOrDefault(x => x.BuildingID == building.BuildingID && x.AccountObjectId == building.AccountObjectId && x.OpportunityID == building.OpportunityID);
                            if (buildingData == null)
                            {
                                EZShredBuildingData buildingToAdd = GetBuildingDataEntity(building);
                                buildingToAdd.CreatedOn = DateTime.UtcNow;
                                buildingToAdd.AccountObjectId = accountObjectId;
                                buildingToAdd.MxtrAccountId = mxtrAccountId;
                                data.Add(buildingToAdd);
                            }
                            else
                            {
                                EZShredBuildingData buildingToUpdate = GetBuildingDataEntity(building, buildingData.Id);
                                buildingToUpdate.CreatedOn = buildingData.CreatedOn;
                                buildingToUpdate.ModifiedOn = DateTime.UtcNow;
                                buildingToUpdate.AccountObjectId = buildingData.AccountObjectId;
                                buildingToUpdate.MxtrAccountId = buildingData.MxtrAccountId;
                                data.Add(buildingToUpdate);
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
        /// Insert/Update building from EZShredData to EZShredBuildingData table in database
        /// This is to handle old data migration only. 
        /// Do not use this function else where This is only one time utility
        /// </summary>
        /// <returns>true/false</returns>
        public bool InsertUpdateBuilding()
        {
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                var data = repo.ToList();
                foreach (var item in data)
                {
                    if (item.Buildings != null)
                    {
                        List<EZShredBuildingDataModel> lstBuilding = GetBuildingDataModel(item.Buildings, item.AccountObjectId, item.MxtrAccountId);
                        if (lstBuilding != null && lstBuilding.Count > 0)
                        {
                            AddUpdateBuildingData(lstBuilding, item.AccountObjectId, item.MxtrAccountId);
                        }
                    }
                }
            }
            return true;
        }
        //Temp function delete after testing
        public int GetBuildingCountInEZShredTable()
        {
            int records = 0;
            using (MongoRepository<EZShredData> repo = new MongoRepository<EZShredData>())
            {
                var data = repo.ToList();
                foreach (var item in data)
                {
                    if (item.Buildings != null)
                    {
                        List<EZShredBuildingDataModel> lstBuilding = GetBuildingDataModel(item.Buildings, item.AccountObjectId, item.MxtrAccountId);
                        if (lstBuilding != null && lstBuilding.Count > 0)
                        {
                            records += lstBuilding.Count;
                        }
                    }
                }
            }
            return records;
        }
        public IEnumerable<EZShredBuildingDataModel> GetAllBuildingByAccountObjectId(string accountObjectId)
        {
            using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
            {
                IEnumerable<EZShredBuildingData> buildings = repo.Where(a => a.AccountObjectId == accountObjectId).OrderBy(o => o.CompanyName);
                return GetBuildingDataModel(buildings);
            }
        }
        public int GetBuildingIdAgainstCustomerId(string accountObjectId, string customerId)
        {
            int buildingId = 0;
            using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
            {
                var building = repo.LastOrDefault(a => a.AccountObjectId == accountObjectId && a.CustomerID == customerId);
                if (building != null && building.BuildingID != null)
                {
                    buildingId = Convert.ToInt32(building.BuildingID);
                }
                return buildingId;
            }
        }
        public int GetBuildingCountAgaistCustomerId(string accountObjectId, int customerId)
        {
            int buildingCountFromEZShredBuildingData = 0;
            int buildingCountFromEZShredleadMapping = 0;
            List<int> lstBuildingId = new List<int>();

            //get building count from EZShred lead mapping           
            using (MongoRepository<EZShredLeadMapping> repo = new MongoRepository<EZShredLeadMapping>())
            {
                if (customerId > 0)
                {
                    var data = repo.FirstOrDefault(a => a.AccountObjectID == accountObjectId && a.CustomerID != null && a.CustomerID == customerId);
                    if (data != null)
                    {
                        //add building Id to list
                        if (data.Building1 != null && data.Building1.BuildingID > 0)
                        {
                            lstBuildingId.Add(Convert.ToInt32(data.Building1.BuildingID));
                        }
                        if (data.Building2 != null && data.Building2.BuildingID > 0)
                        {
                            lstBuildingId.Add(Convert.ToInt32(data.Building2.BuildingID));
                        }
                        if (data.Building3 != null && data.Building3.BuildingID > 0)
                        {
                            lstBuildingId.Add(Convert.ToInt32(data.Building3.BuildingID));
                        }
                        if (data.Building4 != null && data.Building4.BuildingID > 0)
                        {
                            lstBuildingId.Add(Convert.ToInt32(data.Building4.BuildingID));
                        }
                        if (data.Building5 != null && data.Building5.BuildingID > 0)
                        {
                            lstBuildingId.Add(Convert.ToInt32(data.Building5.BuildingID));
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
                    }
                }
            }
            //get building count from EZShredBuildingData table
            using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
            {
                List<EZShredBuildingData> buildings = repo.Where(a => a.AccountObjectId == accountObjectId && a.CustomerID != null && a.CustomerID == Convert.ToString(customerId)).ToList();
                buildings = buildings.Where(x => !lstBuildingId.Contains(Convert.ToInt32(x.BuildingID))).ToList();
                if (buildings != null && buildings.Count > 0)
                {
                    buildingCountFromEZShredBuildingData = buildings.Count;
                }
            }
            return buildingCountFromEZShredBuildingData + buildingCountFromEZShredleadMapping;
        }
        public int GetCustomerByBuildingId(string accountObjectId, string buildingId)
        {
            using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
            {
                var building = repo.FirstOrDefault(a => a.AccountObjectId == accountObjectId && a.BuildingID == buildingId);
                if (building == null)
                {
                    return 0;
                }
                return Convert.ToInt32(building.CustomerID);
            }
        }
        public IEnumerable<EZShredBuildingDataModelMini> GetBuildingsAgainstCustomerId(string accountObjectId, string customerId)
        {
            using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
            {
                var buildings = repo.Where(a => a.AccountObjectId == accountObjectId && a.CustomerID == customerId);
                if (buildings == null)
                {
                    return new List<EZShredBuildingDataModelMini>();
                }
                return buildings.Select(x => new EZShredBuildingDataModelMini
                {
                    BuildingID = x.BuildingID,
                    CompanyName = x.CompanyName
                });
            }
        }

        public List<CustomerSearchResult> GetBuildingsByCustomerId(string accountObjectId, string customerId)
        {
            using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
            {
                var buildings = repo.Where(a => a.AccountObjectId == accountObjectId && a.CustomerID == customerId);
                if (buildings == null)
                {
                    return new List<CustomerSearchResult>();
                }
                return buildings.Select(x => new CustomerSearchResult
                {
                    BuildingID = x.BuildingID,
                    BuildingName = x.CompanyName,
                    Street = x.Street,
                    Zip = x.Zip,
                    OpportunityID = x.OpportunityID,
                }).ToList();
            }
        }

        public IEnumerable<EZShredBuildingDataModel> SearchBuilding(string accountObjectId, string searchText)
        {
            //EZShred API return Bilding name as CompanyName so we have taken companyName
            using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
            {
                var buildings = repo.Where(a => a.AccountObjectId == accountObjectId
                && a.CompanyName.ToLower().Contains(searchText.ToLower()
                ));
                if (buildings == null)
                {
                    return new List<EZShredBuildingDataModel>();
                }
                return GetBuildingDataModel(buildings);
            }
        }

        public bool DeleteDuplicateBuildingRecords()
        {
            try
            {
                using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
                {
                    //Get all records will be used for filtering
                    var records = repo.ToList();

                    //Get duplicate records
                    var duplicateBuildings = (from ssi in records
                                              group ssi by new { ssi.AccountObjectId, ssi.BuildingID } into g
                                              select new
                                              {
                                                  AccountObjectId = g.Key.AccountObjectId,
                                                  BuildingID = g.Key.BuildingID,
                                                  Count = g.Count()
                                              }).Where(w => w.Count > 1).ToList();

                    // make list and add records to be deleted
                    List<EZShredBuildingData> lstRecordsToDelete = new List<EZShredBuildingData>();

                    //Find the record to be deleted
                    foreach (var duplicateBuilding in duplicateBuildings)
                    {
                        //filter records order by date
                        var duplicateRecords = records.Where(w => w.AccountObjectId == duplicateBuilding.AccountObjectId
                        && w.BuildingID == duplicateBuilding.BuildingID).OrderBy(o => o.CreatedOn).ToList();
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

        public bool UpdateOpportunityIDByBuildingId(string accountObjectId, string buildingId, long opportunityId)
        {
            try
            {
                using (MongoRepository<EZShredBuildingData> repo = new MongoRepository<EZShredBuildingData>())
                {
                    var building = repo.FirstOrDefault(a => a.AccountObjectId == accountObjectId && a.BuildingID == buildingId);
                    if (building != null && building.OpportunityID == 0)
                    {
                        building.OpportunityID = opportunityId;
                        repo.Update(building);
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Private Methods
        private EZShredBuildingData GetBuildingDataEntity(EZShredBuildingDataModel data, string id = null)
        {
            return new EZShredBuildingData()
            {
                Id = id,
                CompanyName = data.CompanyName,
                BuildingID = data.BuildingID,
                CustomerID = data.CustomerID,
                Street = data.Street,
                City = data.City,
                State = data.State,
                Zip = data.Zip,
                ScheduleFrequency = data.ScheduleFrequency,
                ServiceTypeID = data.ServiceTypeID,
                Notes = data.Notes,
                Directions = data.Directions,
                RoutineInstructions = data.RoutineInstructions,
                SiteContact1 = data.SiteContact1,
                SiteContact2 = data.SiteContact2,
                Phone1 = data.Phone1,
                Phone2 = data.Phone2,
                SalesTaxRegionID = data.SalesTaxRegionID,
                Suite = data.Suite,
                SalesmanID = data.SalesmanID,
                BuildingTypeID = data.BuildingTypeID,
                UserID = data.UserID,
                AccountObjectId = data.AccountObjectId,
                CompanyCountryCode = data.CompanyCountryCode,
                EndDate = data.EndDate,
                EZTimestamp = data.EZTimestamp,
                LastServiceDate = data.LastServiceDate,
                Latitude = data.Latitude,
                Longitude = data.Longitude,
                MxtrAccountId = data.MxtrAccountId,
                NextServiceDate = data.NextServiceDate,
                Operation = data.Operation,
                RouteID = data.RouteID,
                ScheduleDescription = data.ScheduleDescription,
                ScheduleDOWfri = data.ScheduleDOWfri,
                ScheduleDOWmon = data.ScheduleDOWmon,
                ScheduleDOWsat = data.ScheduleDOWsat,
                ScheduleDOWsun = data.ScheduleDOWsun,
                ScheduleDOWthu = data.ScheduleDOWthu,
                ScheduleDOWtue = data.ScheduleDOWtue,
                ScheduleDOWwed = data.ScheduleDOWwed,
                ScheduleWeek1 = data.ScheduleWeek1,
                ScheduleWeek2 = data.ScheduleWeek2,
                ScheduleWeek3 = data.ScheduleWeek3,
                ScheduleWeek4 = data.ScheduleWeek4,
                ScheduleWeek5 = data.ScheduleWeek5,
                StartDate = data.StartDate,
                Stop = data.Stop,
                TimeTaken = data.TimeTaken,
                OpportunityID = data.OpportunityID,
                TaxExempt = data.TaxExempt,
            };
        }
        private IEnumerable<EZShredBuildingDataModel> GetBuildingDataModel(IEnumerable<EZShredBuildingData> data)
        {
            return data.Select(x => new EZShredBuildingDataModel()
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
                TaxExempt = x.TaxExempt,
            });
        }
        private EZShredBuildingDataModel GetBuildingDataModel(EZShredBuildingData data)
        {
            return new EZShredBuildingDataModel()
            {
                CompanyName = data.CompanyName,
                BuildingID = data.BuildingID,
                CustomerID = data.CustomerID,
                Street = data.Street,
                City = data.City,
                State = data.State,
                Zip = data.Zip,
                ScheduleFrequency = data.ScheduleFrequency,
                ServiceTypeID = data.ServiceTypeID,
                Notes = data.Notes,
                Directions = data.Directions,
                RoutineInstructions = data.RoutineInstructions,
                SiteContact1 = data.SiteContact1,
                SiteContact2 = data.SiteContact2,
                Phone1 = data.Phone1,
                Phone2 = data.Phone2,
                SalesTaxRegionID = data.SalesTaxRegionID,
                Suite = data.Suite,
                SalesmanID = data.SalesmanID,
                BuildingTypeID = data.BuildingTypeID,
                UserID = data.UserID,
                AccountObjectId = data.AccountObjectId,
                CompanyCountryCode = data.CompanyCountryCode,
                EndDate = data.EndDate,
                EZTimestamp = data.EZTimestamp,
                LastServiceDate = data.LastServiceDate,
                Latitude = data.Latitude,
                Longitude = data.Longitude,
                MxtrAccountId = data.MxtrAccountId,
                NextServiceDate = data.NextServiceDate,
                Operation = data.Operation,
                RouteID = data.RouteID,
                ScheduleDescription = data.ScheduleDescription,
                ScheduleDOWfri = data.ScheduleDOWfri,
                ScheduleDOWmon = data.ScheduleDOWmon,
                ScheduleDOWsat = data.ScheduleDOWsat,
                ScheduleDOWsun = data.ScheduleDOWsun,
                ScheduleDOWthu = data.ScheduleDOWthu,
                ScheduleDOWtue = data.ScheduleDOWtue,
                ScheduleDOWwed = data.ScheduleDOWwed,
                ScheduleWeek1 = data.ScheduleWeek1,
                ScheduleWeek2 = data.ScheduleWeek2,
                ScheduleWeek3 = data.ScheduleWeek3,
                ScheduleWeek4 = data.ScheduleWeek4,
                ScheduleWeek5 = data.ScheduleWeek5,
                StartDate = data.StartDate,
                Stop = data.Stop,
                TimeTaken = data.TimeTaken,
                OpportunityID = data.OpportunityID,
                TaxExempt = data.TaxExempt,
            };
        }

        private List<EZShredBuildingDataModel> GetBuildingDataModel(List<Buildings> lstBuildingData, string accountObjectId, string mxtrAccountObjectId)
        {
            return lstBuildingData.Select(x => new EZShredBuildingDataModel()
            {
                AccountObjectId = accountObjectId,
                MxtrAccountId = mxtrAccountObjectId,
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
                UserID = x.userID,
                CompanyCountryCode = x.CompanyCountryCode,
                EndDate = x.EndDate,
                EZTimestamp = x.ezTimestamp,
                LastServiceDate = x.LastServiceDate,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                NextServiceDate = x.NextServiceDate,
                Operation = x.operation,
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
                TaxExempt = x.TaxExempt,
            }).ToList();
        }
        #endregion
    }
}
