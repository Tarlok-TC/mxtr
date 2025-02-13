using System.Collections.Generic;
using System.Data;
using System.Linq;
using mxtrAutomation.Common.Adapter;

namespace mxtrAutomation.Common.Extensions
{
    public static class DataSetExtensions
    {
        public static ICollection<IDictionary<string, object>> ToDictionaryCollection(this DataSet dataSet)
        {
            return
                dataSet.Tables[0].Rows.Cast<DataRow>()
                    .Select(row =>
                            row.Table.Columns.Cast<DataColumn>()
                                .Select((c, idx) => new { Name = c.ColumnName, Value = row.ItemArray[idx] })
                                .ToDictionary(x => x.Name, x => x.Value))
                    .Cast<IDictionary<string, object>>()
                    .ToList();
        }

        public static ICollection<IDictionary<string, object>> ToDictionaryCollectionIgnoreSpace(this DataSet dataSet)
        {
            return
                dataSet.Tables[0].Rows.Cast<DataRow>()
                    .Select(row =>
                            row.Table.Columns.Cast<DataColumn>()
                                .Select((c, idx) => new { Name = c.ColumnName.Replace(" ", string.Empty), Value = row.ItemArray[idx] })
                                .ToDictionary(x => x.Name, x => x.Value))
                    .Cast<IDictionary<string, object>>()
                    .ToList();
        }

        public static IEnumerable<T> MapTo<T>(this DataSet dataSet)
            where T : class, new()
        {
            return new DataSetAdapter<T>().Map(dataSet);
        }

        public static IEnumerable<T> MapToIgnoreSpace<T>(this DataSet dataSet)
            where T : class, new()
        {
            return new DataSetAdapter<T>().MapToIgnoreSpaceInColumnNames(dataSet);
        }
    }
}
