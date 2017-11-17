using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace Pidget.Client.DataModels
{
    [JsonDictionary]
    public class ArbitraryData : IReadOnlyDictionary<string, object>
    {
        public IEnumerable<string> Keys => _data.Keys;

        public IEnumerable<object> Values => _data.Values;

        public int Count => _data.Count;

        public object this[string key] => _data[key];

        private readonly IDictionary<string, object> _data
            = new Dictionary<string, object>();

        public ArbitraryData Set(string name, object data)
        {
            _data[name] = data;

            return this;
        }

        public object Get(string name)
        {
            _data.TryGetValue(name, out object value);

            return value;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public bool ContainsKey(string key)
            => _data.ContainsKey(key);

        public bool TryGetValue(string key, out object value)
            => _data.TryGetValue(key, out value);
    }
}
