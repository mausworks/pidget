using System;
using System.Collections.Generic;
using System.Linq;
using Pidget.Client.DataModels;
using Xunit;

namespace Pidget.Client.Test
{
    public class SentryEventBuilderTests
    {
        [Fact]
        public void ThrowsForNoExceptionOrMessage()
        {
            var builder = new SentryEventBuilder();

            Assert.Throws<InvalidOperationException>(()
                => builder.Build());
        }

        [Fact]
        public void SetException()
        {
            var exception = new Exception();

            var builder = new SentryEventBuilder()
                .SetException(exception);

            Assert.Equal(
                exception.GetType().FullName,
                builder.Build().Exception.Type);
        }

        [Theory, InlineData("message")]
        public void SetMessage(string message)
        {
            var builder = new SentryEventBuilder()
                .SetMessage(message);

            Assert.Equal(message, builder.Build().Message);
        }

        [Theory, InlineData("transaction")]
        public void SetTransaction(string transaction)
        {
            var builder = new SentryEventBuilder()
                .SetException(new Exception())
                .SetTransaction(transaction);

            Assert.Equal(transaction, builder.Build().Culprit);
        }

        [Theory, InlineData(ErrorLevel.Fatal)]
        public void SetErrorLevel(ErrorLevel level)
        {
            var builder = new SentryEventBuilder()
                .SetException(new Exception())
                .SetErrorLevel(level);

            Assert.Equal(level, builder.Build().Level);
        }

        [Theory, InlineData("name", "value")]
        public void AddTag(string name, string value)
        {
            var builder = new SentryEventBuilder()
                .SetException(new Exception())
                .AddTag(name, value);

            var kvp = builder.Build().Tags.First();

            Assert.Equal(name, kvp.Key);
            Assert.Equal(value, kvp.Value);
        }

        [Theory, InlineData("name", "value")]
        public void AddTags(string name, string value)
        {
            var builder = new SentryEventBuilder()
                .SetException(new Exception())
                .AddTags(new Dictionary<string, string> { { name, value } });

            var kvp = builder.Build().Tags.First();

            Assert.Equal(name, kvp.Key);
            Assert.Equal(value, kvp.Value);
        }

        [Theory, InlineData("extra_data", "foo")]
        public void AddExtraData(string name, object value)
        {
            var builder = new SentryEventBuilder()
                .SetException(new Exception())
                .AddExtraData(name, value);

            var extraData = builder.Build().Extra;

            Assert.Equal(name, extraData.First().Key);
            Assert.Equal(value, extraData.First().Value);
        }

        [Theory, InlineData("extra_data", "foo", "bar", "baz")]
        public void AddEnumerableExtraData(params object[] data)
        {
            var builder = new SentryEventBuilder()
                .SetException(new Exception())
                .AddExtraData(ToNamedPairs<object>(data));

            var extraData = builder.Build().Extra;

            var idx = 0;

            for (var c = 1; idx < extraData.Count; c += 2)
            {
                var pair = extraData.ElementAt(idx);

                Assert.Equal(data[c - 1], pair.Key);
                Assert.Equal(data[c], pair.Value);

                idx++;
            }
        }

        [Fact]
        public void SetRequestData()
        {
            var expectedData = new RequestData();

            var builder = new SentryEventBuilder()
                .SetException(new Exception())
                .SetRequestData(expectedData);

            var requestData = builder.Build().Request;

            Assert.Same(expectedData, requestData);
        }

        [Fact]
        public void SetUserData()
        {
            var expectedData = new UserData();

            var builder = new SentryEventBuilder()
                .SetException(new Exception())
                .SetUserData(expectedData);

            var userData = builder.Build().User;

            Assert.Same(expectedData, userData);
        }

        [Theory, InlineData("foo", "bar")]
        public void AddFingerprintData(params string[] data)
        {
            var builder = new SentryEventBuilder()
                .SetException(new Exception())
                .AddFingerprintData(data);

            Assert.Equal(data, builder.Build().Fingerprint);
        }

        private IEnumerable<KeyValuePair<string, TValue>> ToNamedPairs<TValue>(params object[] extraData)
        {
            if (extraData.Length % 2 != 0)
            {
                throw new NotSupportedException(
                    "Cannot created named pairs.");
            }

            for (var i = 0; i < extraData.Length; i += 2)
            {
                var name = extraData[i].ToString();
                var value = (TValue)extraData[i + 1];

                yield return new KeyValuePair<string, TValue>(name, value);
            }
        }
    }
}
