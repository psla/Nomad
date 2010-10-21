using System.IO;
using System.Xml.Serialization;

namespace Nomad.Utils
{
    /// <summary>
    /// Class which provides help for xml serialization
    /// </summary>
    public class XmlSerializerHelper
    {
        /// <summary>
        /// Serializes provided object and returns byte array with serialized data
        /// </summary>
        /// <typeparam name="T">Type of object to serialize</typeparam>
        /// <param name="obj">object to serialize</param>
        /// <returns>array of serialized data</returns>
        public static byte[] Serialize<T>(T obj) where T : class
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof (T));
                serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }


        /// <summary>
        /// Tries to deserialize object of type <typeparamref name="T"/> from provided array.
        /// </summary>
        /// <typeparam name="T">Type of object to be deserialized</typeparam>
        /// <param name="serialized"></param>
        /// <returns>null if deserialization succeeds, but casting fails</returns>
        /// <exception>when backing deserializer fails</exception>
        public static T Deserialize<T>(byte[] serialized) where T : class
        {
            using (var ms = new MemoryStream(serialized))
            {
                var serializer = new XmlSerializer(typeof (T));
                return serializer.Deserialize(ms) as T;
            }
        }
    }
}