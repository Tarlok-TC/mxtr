using System;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Common.Attributes;
using NUnit.Framework;
using System.Reflection;

namespace mxtrAutomation.Common.Extensions
{
    public static class EnumExtensions
    {
        public static IDictionary<T, string> ToStringValueDictionary<T>()
        {
            return
                typeof(T).GetFields()
                    .Where(f => f.IsLiteral)
                    .ToDictionary(
                        f => (T)Enum.Parse(typeof(T), f.Name),
                        f => f.GetCustomAttributes(false).OfType<StringValueAttribute>().Select(a => a.Value).SingleOrDefault());
        }

        public static IDictionary<T, string> ToStringValueDictionary<T, U>()
            where U : StringValueAttribute
        {
            return
                typeof(T).GetFields()
                    .Where(f => f.IsLiteral)
                    .ToDictionary(
                        f => (T)Enum.Parse(typeof(T), f.Name),
                        f => f.GetCustomAttributes(false).OfType<U>().Select(a => a.Value).SingleOrDefault());
        }

        public static IDictionary<T, bool> ToBoolValueDictionary<T>()
        {
            return
                typeof(T).GetFields()
                    .Where(f => f.IsLiteral)
                    .ToDictionary(
                        f => (T)Enum.Parse(typeof(T), f.Name),
                        f => f.GetCustomAttributes(false).OfType<BoolValueAttribute>().Select(a => a.Value).SingleOrDefault());
        }

        public static IDictionary<T, bool> ToBoolValueDictionary<T, U>()
            where U : BoolValueAttribute
        {
            return
                typeof(T).GetFields()
                    .Where(f => f.IsLiteral)
                    .ToDictionary(
                        f => (T)Enum.Parse(typeof(T), f.Name),
                        f => f.GetCustomAttributes(false).OfType<U>().Select(a => a.Value).SingleOrDefault());
        }

        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        public static string GetStringValue(Enum value)
        {
            return
                value.GetType()
                    .GetField(value.ToString())
                    .GetCustomAttributes(false)
                    .OfType<StringValueAttribute>()
                    .Select(s => s.Value)
                    .SingleOrDefault();
        }

        public static IEnumerable<T> ToList<T>()
        {
            return
                typeof (T).GetFields()
                    .Where(f => f.IsLiteral)
                    .Select(f => (T) Enum.Parse(typeof (T), f.Name));
        }

        public static T ToEnum<T>(this string enumString, bool ignoreCase)
        {
            return (T)Enum.Parse(typeof(T), enumString, ignoreCase);
        }

        public static T ToEnum<T>(this int enumInt)
        {
            return (T)Enum.ToObject(typeof(T), enumInt);
        }
    }

    public interface IEnumNameMap<T> : IDictionary<string, T> { }

    public class EnumNameMap<T> : Dictionary<string, T>, IEnumNameMap<T>
    {
        public EnumNameMap(IDictionary<string, T> dict) : base(dict) { }
    }


}
