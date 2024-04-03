﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace ArnoldVinkCode
{
    public partial class AVClassConverters
    {
        //Clone byte array
        public static byte[] CloneByteArray(byte[] cloneArray)
        {
            try
            {
                byte[] newArray = new byte[cloneArray.Length];
                Array.Copy(cloneArray, 0, newArray, 0, cloneArray.Length);
                return newArray;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to clone byte array: " + ex.Message);
                return null;
            }
        }

        //Shallow clone object
        public static bool CloneObjectShallow<T>(T cloneObject, out T outObject)
        {
            try
            {
                MethodInfo methodIndo = cloneObject.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
                outObject = (T)methodIndo.Invoke(cloneObject, null);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to shallow clone object: " + ex.Message);
                outObject = default(T);
                return false;
            }
        }

        //Deep clone object
        public static bool CloneObjectDeep<T>(T cloneObject, out T outObject)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, cloneObject);
                    memoryStream.Position = 0;
                    outObject = (T)formatter.Deserialize(memoryStream);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to deep clone object: " + ex.Message);
                outObject = default(T);
                return false;
            }
        }

        ////Convert bytes to hex string
        //public static byte[] GetHexStringToBytes(string value)
        //{
        //    try
        //    {
        //        return SoapHexBinary.Parse(value).Value;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Failed to GetHexStringToBytes: " + ex.Message);
        //        return null;
        //    }
        //}

        //public static string GetBytesToHexString(byte[] value)
        //{
        //    try
        //    {
        //        return new SoapHexBinary(value).ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Failed to GetBytesToHexString: " + ex.Message);
        //        return null;
        //    }
        //}

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
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to SerializeObjectToBytes: " + ex.Message);
                return null;
            }
        }

        public static bool DeserializeBytesToObject<T>(byte[] bytesObject, out T outObject)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(bytesObject))
                {
                    outObject = (T)new BinaryFormatter().Deserialize(memoryStream);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to DeserializeBytesToObject: " + ex.Message);
                outObject = default(T);
                return false;
            }
        }
    }
}