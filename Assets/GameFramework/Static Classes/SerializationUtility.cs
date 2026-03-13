using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GF
{
    public static class SerializationUtility
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
        public static T ByteArrayToObject<T>(byte[] byteArray)
        {
            if (byteArray == null)
                throw new ArgumentNullException(nameof(byteArray));

            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}   
