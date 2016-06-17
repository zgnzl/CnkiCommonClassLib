using System.IO;
using System.Threading.Tasks;
using ProtoBuf;

namespace CommonFoundation.Redis.Serializer
{
    public class ProtobufSerializer : ISerializer
    {
        public byte[] Serialize(object item)
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, item);
                return ms.ToArray();
            }
        }

        public Task<byte[]> SerializeAsync(object item)
        {
            return Task.Factory.StartNew(() => Serialize(item));
        }

        public object Deserialize(byte[] serializedObject)
        {
            return Deserialize<object>(serializedObject);
        }

        public Task<object> DeserializeAsync(byte[] serializedObject)
        {
            return Task.Factory.StartNew(() => Deserialize(serializedObject));
        }

        public T Deserialize<T>(byte[] serializedObject)
        {
            using (var ms = new MemoryStream(serializedObject))
            {
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }

        public Task<T> DeserializeAsync<T>(byte[] serializedObject)
        {
            return Task.Factory.StartNew(() => Deserialize<T>(serializedObject));
        }
    }
}