﻿using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization.Formatters.Binary;

namespace ArnoldVinkCode
{
    public partial class AVClassConverters
    {
        //Clone existing class object
        public static T CloneClassObject<T>(T classObject) where T : class
        {
            try
            {
                MethodInfo methodIndo = classObject.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
                return (T)methodIndo?.Invoke(classObject, null);
            }
            catch { }
            return null;
        }

        //Convert bytes to hex string
        public static byte[] GetHexStringToBytes(string value)
        {
            try
            {
                SoapHexBinary shb = SoapHexBinary.Parse(value);
                return shb.Value;
            }
            catch
            {
                Debug.WriteLine("Failed to GetHexStringToBytes.");
                return null;
            }
        }

        public static string GetBytesToHexString(byte[] value)
        {
            try
            {
                SoapHexBinary shb = new SoapHexBinary(value);
                return shb.ToString();
            }
            catch
            {
                Debug.WriteLine("Failed to GetBytesToHexString.");
                return null;
            }
        }

        //Serialize and deserialize class
        public static byte[] SerializeObjectToBytes(object serializeObject)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(memoryStream, serializeObject);
                    return memoryStream.ToArray();
                }
            }
            catch
            {
                Debug.WriteLine("Failed to SerializeObjectToBytes.");
                return null;
            }
        }

        public static T DeserializeBytesToClass<T>(byte[] bytesObject) where T : class
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(bytesObject))
                {
                    return (T)new BinaryFormatter().Deserialize(memoryStream);
                }
            }
            catch
            {
                Debug.WriteLine("Failed to DeserializeBytesToObject.");
                return null;
            }
        }
    }
}