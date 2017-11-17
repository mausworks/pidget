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

            data.Set(key, value);

            Assert.Equal(key, data.First().Key);
            Assert.Equal(value, data.First().Value);
        }

        [Theory, InlineData("some_data", 1)]
        public void GetSetValue(string key, object value)
        {
            var data = new ArbitraryData();

            data.Set(key, value);

            Assert.Equal(1, data.Count);
            Assert.Equal(key, data.Keys.First());
            Assert.Equal(value, data.Values.First());
            Assert.Equal(value, data.Get(key));
            Assert.Equal(value, data[key]);
            Assert.True(data.ContainsKey(key));
            Assert.True(data.TryGetValue(key, out object outValue));
            Assert.Equal(value, outValue);
        }
    }
}
