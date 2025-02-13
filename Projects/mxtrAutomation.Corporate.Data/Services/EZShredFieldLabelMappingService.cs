using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class EZShredFieldLabelMappingService : MongoRepository<EZShredFieldLabelMapping>, IEZShredFieldLabelMappingServiceInternal
    {
        public CreateNotificationReturn AddFieldLabel(EZShredFieldLabelMappingDataModel fields)
        {
            try
            {
                EZShredFieldLabelMapping entity = new EZShredFieldLabelMapping
                {
                    EZShredFieldName = fields.EZShredFieldName,
                    Label = fields.Label,
                    Type = fields.Type,
                    Set = fields.Set

                };
                using (MongoRepository<EZShredFieldLabelMapping> repo = new MongoRepository<EZShredFieldLabelMapping>())
                {
                    repo.Add(entity);
                    return new CreateNotificationReturn { Success = true, ObjectID = entity.Id };
                }
            }
            catch (Exception ex)
            {

                return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
            }
        }
        public CreateNotificationReturn AddFieldLabel(List<EZShredFieldLabelMappingDataModel> fields)
        {
            try
            {
                foreach (var field in fields)
                {
                    EZShredFieldLabelMapping entity = new EZShredFieldLabelMapping
                    {
                        EZShredFieldName = field.EZShredFieldName,
                        Label = field.Label,
                        Type = field.Type,
                        Set = field.Set

                    };
                    using (MongoRepository<EZShredFieldLabelMapping> repo = new MongoRepository<EZShredFieldLabelMapping>())
                    {
                        if (repo.FirstOrDefault(x => x.Label == entity.Label && x.EZShredFieldName == entity.EZShredFieldName && x.Type==entity.Type) == null)
                        {
                            repo.Add(entity);
                        }
                    }
                }
                return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
            }
            catch (Exception ex)
            {
                return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
            }
        }

        public List<EZShredFieldLabelMappingDataModel> GetAllFieldLabels()
        {
            using (MongoRepository<EZShredFieldLabelMapping> repo = new MongoRepository<EZShredFieldLabelMapping>())
            {
                return repo
                     .Select(a => new EZShredFieldLabelMappingDataModel
                     {
                         EZShredFieldName = a.EZShredFieldName,
                         Label = a.Label,
                         Type=a.Type,
                         Set=a.Set
                     }).ToList();
            }
        }
        public int IsEZShredFieldLabelExist()
        {
            using (MongoRepository<EZShredFieldLabelMapping> repo = new MongoRepository<EZShredFieldLabelMapping>())
            {
                if (repo.Count() > 0)
                {
                    return Convert.ToInt32(repo.Count());
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
