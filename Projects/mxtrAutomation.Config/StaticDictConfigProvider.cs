using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Config
{
    public class StaticDictConfigProvider : IConfigProvider
    {
        public object Get(Type t, string n)
        {
            object tmp;
            if (_dict.TryGetValue(new Key(t, n), out tmp))
                return tmp;
            else
                return null;
        }

        public void Set(Type t, string n, object config)
        {
            _dict[new Key(t, n)] = config;
        }

        private Dictionary<Key, object> _dict = new Dictionary<Key, object>();

        private class Key
        {
            public Key(Type t, string n) { T = t; N = n; }
            public Type T { get; set; }
            public string N { get; set; }
            public override bool Equals(object obj)
            {
                Key tmp = (Key)obj;
                return tmp.T == T && tmp.N == N;
            }

            public override int GetHashCode()
            {
                return (T != null ? T.GetHashCode() : 0)
                    ^ (N != null ? N.GetHashCode() : 0);
            }

            public override string ToString()
            {
                return string.Format("{0} {1}", T, N);
            }
        }
    }
}
