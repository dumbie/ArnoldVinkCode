using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace ArnoldVinkCode
{
    public partial class AVClassConverters
    {
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

        //Serialize object to bytes
        public static byte[] SerializeObjectToBytes(object serializeObject)
        {
            try
            {
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings
                {
                    //TypeNameHandling = TypeNameHandling.Objects
                };

                string jsonString = JsonConvert.SerializeObject(serializeObject, jsonSettings);
                return Encoding.UTF8.GetBytes(jsonString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to SerializeObjectToBytes: " + ex.Message);
                return null;
            }
        }

        //Deserialize bytes to object
        public static bool DeserializeBytesToObject<T>(byte[] bytesObject, out T outObject)
        {
            try
            {
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings
                {
                    //TypeNameHandling = TypeNameHandling.Objects
                };

                string jsonString = Encoding.UTF8.GetString(bytesObject);
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