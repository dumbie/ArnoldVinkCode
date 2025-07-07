using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVClassConverters
    {
        //Get dynamic variable type
        public static Type GetDynamicType(dynamic dynamicVariable)
        {
            try
            {
                return ((object)dynamicVariable).GetType();
            }
            catch { return null; }
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

        //Marshal object to bytes
        public static byte[] MarshalObjectToBytes(object targetObject)
        {
            IntPtr intPtr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(targetObject);
                intPtr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(targetObject, intPtr, true);
                byte[] byteArray = new byte[size];
                Marshal.Copy(intPtr, byteArray, 0, size);
                return byteArray;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to MarshalObjectToBytes: " + ex.Message);
                return null;
            }
            finally
            {
                SafeCloseMarshal(ref intPtr);
            }
        }

        //Marshal bytes to object
        public static bool MarshalBytesToObject<T>(byte[] targetBytes, out T outObject)
        {
            IntPtr intPtr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(default(T));
                intPtr = Marshal.AllocHGlobal(size);
                Marshal.Copy(targetBytes, 0, intPtr, size);
                outObject = (T)Marshal.PtrToStructure(intPtr, typeof(T));
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to MarshalBytesToObject: " + ex.Message);
                outObject = default(T);
                return false;
            }
            finally
            {
                SafeCloseMarshal(ref intPtr);
            }
        }

        //Serialize object to bytes
        public static byte[] SerializeObjectToBytes(object targetObject)
        {
            try
            {
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings
                {
                    //TypeNameHandling = TypeNameHandling.Objects
                };

                string jsonString = JsonConvert.SerializeObject(targetObject, jsonSettings);
                return Encoding.UTF8.GetBytes(jsonString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to SerializeObjectToBytes: " + ex.Message);
                return null;
            }
        }

        //Deserialize bytes to object
        public static bool DeserializeBytesToObject<T>(byte[] targetBytes, out T outObject)
        {
            try
            {
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings
                {
                    //TypeNameHandling = TypeNameHandling.Objects
                };

                string jsonString = Encoding.UTF8.GetString(targetBytes);
                outObject = JsonConvert.DeserializeObject<T>(jsonString, jsonSettings);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to DeserializeBytesToObject: " + ex.Message);
                outObject = default(T);
                return false;
            }
        }

        //Convert object to type
        public static T ConvertObjectToType<T>(object obj)
        {
            try
            {
                //Check for json object
                if (obj is JObject || obj is JArray)
                {
                    JsonSerializerSettings jsonSettings = new JsonSerializerSettings
                    {
                        //TypeNameHandling = TypeNameHandling.Objects
                    };

                    return JsonConvert.DeserializeObject<T>(obj.ToString(), jsonSettings);
                }
                else
                {
                    return (T)obj;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to ConvertObjectToType: " + ex.Message);
                return default(T);
            }
        }
    }
}