using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Raven.Client.Test
{
    public class SentryEventBuilderTests
    {
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

        [Theory, InlineData("transaction")]
        public void SetTransaction(string transaction)
        {
            var builder = new SentryEventBuilder()
                .SetException(new Exception())
                .SetTransaction(transaction);

            Assert.Equal(transaction, builder.Build().Culprit);
        }

        [Theory, InlineData("message")]
        public void SetMessage(string message)
        {
            var builder = new SentryEventBuilder()
                .SetException(new Exception())
                .SetMessage(message);

            Assert.Equal(message, builder.Build().Message);
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

        [Theory, InlineData("foo", "bar")]
        public void AddFingerprintData(params string[] data)
        {
            var builder = new SentryEventBuilder()
                .SetException(new Exception())
                .AddFingerprintData(data);

            Assert.Equal(data, builder.Build().Fingerprint);
        }
    }
}
