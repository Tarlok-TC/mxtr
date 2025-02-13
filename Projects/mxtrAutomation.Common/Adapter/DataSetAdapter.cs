using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Common.Adapter
{
    public class DataSetAdapter<T> : IAdapter<DataSet, IEnumerable<T>>
        where T : class, new()
    {
        public IEnumerable<T> Map(DataSet input)
        {
            ICollection<IDictionary<string, object>> valueCollection =
                input.ToDictionaryCollection();

            List<PropertyInfo> properties =
                typeof(T).GetProperties().ToList();

            List<T> results = new List<T>();

            foreach (IDictionary<string, object> row in valueCollection)
            {
                T data = new T();

                IDictionary<string, object> row1 = row;

                //properties
                //    .Where(p => row1.ContainsKey(p.Name) && row1[p.Name] != null && !(row1[p.Name] is System.DBNull))
                //    .ForEach(p => p.SetValue(data, row1[p.Name], null));

                var pr = properties.Where(p => row1.ContainsKey(p.Name) && row1[p.Name] != null && !(row1[p.Name] is System.DBNull));
                pr.ForEach(p => p.SetValue(data, row1[p.Name], null));

                results.Add(data);
            }

            return results;
        }

        public IEnumerable<T> MapToIgnoreSpaceInColumnNames(DataSet input)
        {
            ICollection<IDictionary<string, object>> valueCollection =
                input.ToDictionaryCollectionIgnoreSpace();

            List<PropertyInfo> properties =
                typeof(T).GetProperties().ToList();

            List<T> results = new List<T>();

            foreach (IDictionary<string, object> row in valueCollection)
            {
                T data = new T();

                IDictionary<string, object> row1 = row;

                //properties
                //    .Where(p => row1.ContainsKey(p.Name) && row1[p.Name] != null && !(row1[p.Name] is System.DBNull))
                //    .ForEach(p => p.SetValue(data, row1[p.Name], null));

                var pr = properties.Where(p => row1.ContainsKey(p.Name) && row1[p.Name] != null && !(row1[p.Name] is System.DBNull));
                pr.ForEach(p => p.SetValue(data, row1[p.Name], null));

                results.Add(data);
            }

            return results;
        }
    }
}
