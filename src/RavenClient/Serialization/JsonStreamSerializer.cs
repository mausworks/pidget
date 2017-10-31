using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text;

namespace Pidget.Client.Serialization
{
    public class JsonStreamSerializer
    {
        public Encoding Encoding { get; }

        private readonly JsonSerializer _serializer;

        public JsonStreamSerializer(Encoding encoding,
            JsonSerializer jsonSerializer)
        {
            Encoding = encoding;
            _serializer = jsonSerializer;
        }

        public Stream Serialize(object item)
        {
            var stream = new MemoryStream(256);

            using (var writer = CreateWriter(stream))
            {
                _serializer.Serialize(writer, item);
            }

            stream.Position = 0;

            return stream;
        }

        public TResult Deserialize<TResult>(Stream stream)
            => (TResult)Deserialize(stream, typeof(TResult));

        public object Deserialize(Stream stream, Type type)
        {
            using (var streamReader = new StreamReader(stream, Encoding))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    return _serializer.Deserialize(jsonReader, type);
                }
            }
        }

        private StreamWriter CreateWriter(Stream stream)
            => new StreamWriter(
                stream: stream,
                encoding: Encoding,
                bufferSize: 64,
                leaveOpen: true);
    }
}
