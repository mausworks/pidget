using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace Pidget.Client.DataModels
{
    /// <summary>
    /// A dictionary-like name/object data structure.
    /// </summary>
    public class ArbitraryData : IDictionary<string, object>
    {
        public ICollection<string> Keys => _data.Keys;

        public ICollection<object> Values => _data.Values;

        public int Count => _data.Count;

        public bool IsReadOnly => _data.IsReadOnly;

        private readonly IDictionary<string, object> _data
            = new Dictionary<string, object>(StringComparer.Ordinal);


        public object this[string key]
        {
            get => Get(key);
            set => _data[key] = value;
        }

        private object Get(string key)
        {
            TryGetValue(key, out object value);

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

        public void Add(string key, object value)
            => _data.Add(key, value);

        public bool Remove(string key)
            => _data.Remove(key);

        public void Add(KeyValuePair<string, object> item)
            => _data.Add(item);

        public void Clear()
            => _data.Clear();

        public bool Contains(KeyValuePair<string, object> item)
            => _data.Contains(item);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            => _data.CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<string, object> item)
            => _data.Remove(item);
    }
}
