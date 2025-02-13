using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Common.Extensions
{ 
    public static class PropertyGetterExtension
    {
        public static object GetProperty<T>(this T obj, string name) where T : class
        {
            Type t = typeof(T);
            return t.GetProperty(name).GetValue(obj, null);
        }

        public static T CastToT<T>(this object input)
        {
            return (T)input;
        }
    }
    
    public class DataTableData
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<dynamic> data { get; set; }
    }
}
