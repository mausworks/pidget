using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Raven.Client.Serialization.Test
{
    using static JsonConvert;

    public class JsonStreamSerializerTests
    {
        public JsonStreamSerializer Serializer
            = new JsonStreamSerializer(Sentry.ApiEncoding,
                new JsonSerializer());

        [Fact]
        public void CanSerialize()
            => Serializer.Serialize(new { foo = "bar" });

        [Theory]
        [InlineData("{ \"foo\": \"bar\" }", typeof(JObject))]
        public void CanDeserialize(string json, Type type)
            => Serializer.Deserialize(StringToStream(json), type);

        [Fact]
        public void Compare_JsonConvert_Serialize()
        {
            var obj = new
            {
                foo = "bar",
                inner = new { bar = "baz", value = Math.PI }
            };

            var result = StreamToString(Serializer.Serialize(obj));

            Assert.Equal(
                expected: SerializeObject(obj),
                actual: result);
        }

        [Theory]
        [InlineData("{\"foo\":\"bar\",\"inner\":{\"bar\":\"baz\",\"value\":3.1415926535897931}}")]
        public void Compare_JsonConvert_Deserialize(string json)
        {
            var result = Serializer.Deserialize<JObject>(
                stream: StringToStream(json));

            Assert.Equal(
                expected: SerializeObject(DeserializeObject(json)),
                actual: SerializeObject(result));
        }

        private Stream StringToStream(string value)
            => new MemoryStream(Sentry.ApiEncoding.GetBytes(value));

        private string StreamToString(Stream stream)
        {
            using (var reader = new StreamReader(stream, Sentry.ApiEncoding))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
