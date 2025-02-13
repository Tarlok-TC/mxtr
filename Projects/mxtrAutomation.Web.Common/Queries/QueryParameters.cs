using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace mxtrAutomation.Web.Common.Queries
{
    public class QueryParameters : ICollection<IUrlParameter>
    {
        private readonly Dictionary<string, IUrlParameter> _parameters;

        private static string GetKey(IUrlParameter parameter)
        {
            return GetKey(parameter.PropertyName);
        }

        private static string GetKey(string parameterName)
        {
            return "[" + parameterName + "]";
        }

        public QueryParameters()
        {
            _parameters = new Dictionary<string, IUrlParameter>();
        }

        public IUrlParameter this[string name]
        {
            get
            {
                if (Contains(name))
                    return name.StartsWith("[") ? _parameters[name] : _parameters[GetKey(name)];
                return null;
            }
        }

        public void Add(IUrlParameter item)
        {
            _parameters.Add(GetKey(item), item);
        }

        public UrlParameterBase<T> Add<T>(UrlParameterBase<T> item)
        {
            _parameters.Add(GetKey(item), item);
            return item;
        }

        public QueryStringParameter<T> Add<T>(string propertyName)
        {
            return Add<T>(propertyName, false);
        }

        public QueryStringParameter<T> Add<T>(string propertyName, bool isRequired)
        {
            QueryStringParameter<T> queryStringParameter = new QueryStringParameter<T>(propertyName, isRequired);
            Add(queryStringParameter);
            return queryStringParameter;
        }

        public QueryStringParameter<T> Add<T>(string propertyName, bool isRequired, Func<T,string> serializer, Func<string,T> deserializer)
        {
            QueryStringParameter<T> queryStringParameter = new QueryStringParameter<T>(propertyName, isRequired, serializer, deserializer);
            Add(queryStringParameter);
            return queryStringParameter;
        }

        public void Clear()
        {
            _parameters.Clear();
        }

        public bool Contains(string key)
        {
            if (key.StartsWith("["))
                return _parameters.ContainsKey(key);
            else
                return _parameters.ContainsKey(GetKey(key));
        }

        public bool Contains(IUrlParameter item)
        {
            return _parameters.ContainsKey(GetKey(item));
        }

        public void CopyTo(IUrlParameter[] array, int arrayIndex)
        {
            _parameters.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(IUrlParameter item)
        {
            return _parameters.Remove(GetKey(item));
        }

        public int Count
        {
            get { return _parameters.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        IEnumerator<IUrlParameter> IEnumerable<IUrlParameter>.GetEnumerator()
        {
            return _parameters.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<IUrlParameter>)this).GetEnumerator();
        }

        public IEnumerator<IUrlParameter> GetQueryStringParameterEnumerator()
        {
            return _parameters.Values.Where(value => !value.IsUrlPathParameter).GetEnumerator();
        }

        public IEnumerator<IUrlParameter> GetUrlParameterEnumerator()
        {
            return _parameters.Values.Where(value => value.IsUrlPathParameter).GetEnumerator();
        }
    }
}
