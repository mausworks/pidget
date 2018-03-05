using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Pidget.Client.DataModels;
using Xunit;

namespace Pidget.Client.Test.DataModels
{
    public class ArbitraryDataTests
    {
        [Theory, InlineData("some_data", 1)]
        public void SetValue_GetsEnumerated(string key, object value)
        {
            var data = new ArbitraryData();

            data[key] = value;

            Assert.Equal(key, data.First().Key);
            Assert.Equal(value, data.First().Value);
        }

        [Theory, InlineData("some_data", 1)]
        public void GetSetValue(string key, object value)
        {
            var data = new ArbitraryData();

            data[key] = value;

            Assert.Equal(1, data.Count);
            Assert.Equal(key, data.Keys.First());
            Assert.Equal(value, data.Values.First());
            Assert.Equal(value, data[key]);
            Assert.True(data.ContainsKey(key));
            Assert.True(data.TryGetValue(key, out object outValue));
            Assert.Equal(value, outValue);
        }

        [Fact]
        public void ImplementsDictionary()
        {
            var data = new ArbitraryData
            {
                { "foo", "bar" }
            };

            Assert.False(data.IsReadOnly);

            data.Add("bar", "baz");
            data.Add(new KeyValuePair<string, object>("baz", "foo"));

            Assert.True(data.Count == 3);

            var copy = new KeyValuePair<string, object>[data.Count];
            data.CopyTo(copy, 0);

            Assert.Equal(data.ToArray(), copy);

            data.Remove("foo");
            Assert.False(data.ContainsKey("foo"));

            data.Remove(new KeyValuePair<string, object>("bar", "baz"));
            Assert.False(data.Contains(
                new KeyValuePair<string, object>("bar", "baz")));

            data.Clear();
            Assert.Equal(0, data.Count);
        }
    }
}
