using System.Collections.Generic;
using System.IO;
using LINQtoCSV;

namespace mxtrAutomation.Common.Utils
{
    public interface ICsvContext
    {
        IEnumerable<T> Read<T>(StreamReader streamReader, CsvFileDescription csvFileDescription)
            where T : class, new();
    }

    public class AdTrakCsvContext : CsvContext, ICsvContext
    {
        IEnumerable<T> ICsvContext.Read<T>(StreamReader streamReader, CsvFileDescription csvFileDescription)
        {
            return Read<T>(streamReader, csvFileDescription);
        }
    }
}
