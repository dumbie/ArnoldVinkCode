using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ArnoldVinkCode
{
    public partial class AVJsonFunctions
    {
        //Read Json from file (Deserialize)
        ///Example: JsonLoadFile<List<string>>("JsonFile");
        public static T JsonLoadFile<T>(string filePath) where T : class
        {
            try
            {
                string jsonFileText = File.ReadAllText(filePath);
                T targetDeserialize = JsonConvert.DeserializeObject<T>(jsonFileText);
                Debug.WriteLine("Completed reading json file: " + filePath);
                return targetDeserialize;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading json file: " + filePath + "/" + ex.Message);
                return null;
            }
        }

        //Read Json from embedded file (Deserialize)
        ///Example: JsonLoadEmbeddedFile<List<string>>("JsonFile");
        public static T JsonLoadEmbeddedFile<T>(Assembly resourceAssembly, string resourcePath) where T : class
        {
            try
            {
                string jsonFileText = string.Empty;
                using (Stream stream = AVEmbedded.EmbeddedResourceToStream(resourceAssembly, resourcePath))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonFileText = reader.ReadToEnd();
                    }
                }

                T targetDeserialize = JsonConvert.DeserializeObject<T>(jsonFileText);
                Debug.WriteLine("Completed reading json resource: " + resourcePath);
                return targetDeserialize;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading json resource: " + resourcePath + "/" + ex.Message);
                return null;
            }
        }

        //Read Json files from directory (Deserialize)
        ///Example: JsonLoadDirectory<List<string>, string>("JsonFilesDirectory");
        public static T1 JsonLoadDirectory<T1, T2>(string directoryPath) where T1 : class
        {
            try
            {
                //Check directory
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    Debug.WriteLine("Created json load directory: " + directoryPath);
                }

                //Create list collection
                dynamic targetCollection = Activator.CreateInstance(typeof(T1));

                //Add all found json files
                string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");
                foreach (string jsonFile in jsonFiles)
                {
                    string jsonFileText = File.ReadAllText(jsonFile);
                    targetCollection.Add(JsonConvert.DeserializeObject<T2>(jsonFileText));
                }

                Debug.WriteLine("Completed reading json files from: " + directoryPath);
                return targetCollection;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading json files from: " + directoryPath + "/" + ex.Message);
                return null;
            }
        }

        //Save to Json file (Serialize)
        public static void JsonSaveObject(object serializeObject, string filePath)
        {
            try
            {
                //Json settings
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;
                //jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                //jsonSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

                //Json serialize
                string serializedObject = JsonConvert.SerializeObject(serializeObject, jsonSettings);

                //Save to file
                AVFiles.StringToFile(filePath, serializedObject);
                Debug.WriteLine("Completed saving json file: " + filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving json file: " + filePath + "/" + ex.Message);
            }
        }

        //Find tokens by key name
        public static void JsonFindTokens(string keyName, JToken targetToken, ref List<JToken> foundTokens)
        {
            try
            {
                if (targetToken.Type == JTokenType.Object)
                {
                    foreach (JProperty child in targetToken.Children<JProperty>())
                    {
                        if (child.Name == keyName)
                        {
                            foundTokens.Add(child.Value);
                        }
                        JsonFindTokens(keyName, child.Value, ref foundTokens);
                    }
                }
                else if (targetToken.Type == JTokenType.Array || targetToken.Type == JTokenType.Property)
                {
                    foreach (JToken child in targetToken.Children())
                    {
                        JsonFindTokens(keyName, child, ref foundTokens);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed finding json tokens: " + ex.Message);
            }
        }
    }
}