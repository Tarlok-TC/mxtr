using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GoogleAnalyticsApp.Adapter
{
    class GoogleDataAdapter
    {
        public static List<T> AdaptGoogleReportResults<T>(Report report, TimeZoneInfo GoogleAnalyticsTimeZoneInfo)
        {
            var dataModelList = new List<T>();

            if (report == null) return dataModelList;
            if ((List<ReportRow>)report.Data.Rows == null) return dataModelList;


            var dataModelProperties = typeof(T).GetProperties();
            var rows = (List<ReportRow>)report.Data.Rows;

            var dimensionHeaders = (List<string>)report.ColumnHeader.Dimensions;
            foreach (ReportRow row in rows)
            {
                var dataModel = (T)Activator.CreateInstance(typeof(T));
                var dimensions = (List<string>)row.Dimensions;
                for (int i = 0; i < dimensionHeaders.Count && i < dimensions.Count; i++)
                {
                    foreach (var property in dataModelProperties)
                    {
                        if (property.Name == dimensionHeaders[i].Replace("ga:", ""))
                        {
                            setValueToProperty(property, dimensions[i], ref dataModel, GoogleAnalyticsTimeZoneInfo);
                            break;
                        }
                    }
                }

                var metricHeaders = (List<MetricHeaderEntry>)report.ColumnHeader.MetricHeader.MetricHeaderEntries;
                var metrics = (List<DateRangeValues>)row.Metrics;
                for (int j = 0; j < metrics.Count; j++)
                {
                    DateRangeValues values = metrics[j];
                    for (int k = 0; k < values.Values.Count && k < metricHeaders.Count; k++)
                    {
                        foreach (var property in dataModelProperties)
                        {
                            if (property.Name == metricHeaders[k].Name.Replace("ga:", ""))
                            {
                                setValueToProperty(property, values.Values[k], ref dataModel, GoogleAnalyticsTimeZoneInfo);
                                break;
                            }
                        }
                    }
                }

                dataModelList.Add(dataModel);
            }

            return dataModelList;
        }
        public static void setValueToProperty<T>(PropertyInfo property, string valueToSet, ref T dataModel, TimeZoneInfo GoogleAnalyticsTimeZoneInfo)
        {
            switch (property.PropertyType.ToString())
            {
                case "System.DateTime":
                    var dValue = DateTime.MinValue;
                    DateTime.TryParseExact(valueToSet, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dValue);
                    if (GoogleAnalyticsTimeZoneInfo != null)
                    {
                        //convert to UTC timezone from Google Analytics timezone
                        dValue = TimeZoneInfo.ConvertTime(dValue, GoogleAnalyticsTimeZoneInfo, TimeZoneInfo.Utc);
                    }
                    //set time to 0
                    dValue = new DateTime(dValue.Year, dValue.Month, dValue.Day, 0, 0, 0, DateTimeKind.Utc);
                    property.SetValue(dataModel, dValue, null);
                    break;
                case "System.Int32":
                    var iValue = 0;
                    int.TryParse(valueToSet, out iValue);
                    property.SetValue(dataModel, iValue, null);
                    break;
                case "System.String":
                    property.SetValue(dataModel, valueToSet, null);
                    break;
            }
        }
    }
}
