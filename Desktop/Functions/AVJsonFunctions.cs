using Newtonsoft.Json;
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
        public static void JsonLoadFile<T>(ref T deserializeTarget, string filePath)
        {
            try
            {
                string jsonFileText = File.ReadAllText(filePath);
                deserializeTarget = JsonConvert.DeserializeObject<T>(jsonFileText);
                Debug.WriteLine("Completed reading json file: " + filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading json file: " + filePath + "/" + ex.Message);
            }
        }

        //Read Json from embedded file (Deserialize)
        public static void JsonLoadEmbeddedFile<T>(Assembly assembly, ref T deserializeTarget, string resourcePath)
        {
            try
            {
                string jsonFileText = string.Empty;
                using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonFileText = reader.ReadToEnd();
                    }
                }

                deserializeTarget = JsonConvert.DeserializeObject<T>(jsonFileText);
                Debug.WriteLine("Completed reading json resource: " + resourcePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading json resource: " + resourcePath + "/" + ex.Message);
            }
        }

        //Read Json from multiple files (Deserialize)
        public static void JsonLoadMulti<T>(ICollection<T> targetCollection, string directoryPath, bool clearCollection)
        {
            try
            {
                //Clear loaded json collection
                if (clearCollection)
                {
                    targetCollection.Clear();
                }

                //Add all found json files
                string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");
                foreach (string jsonFile in jsonFiles)
                {
                    string jsonFileText = File.ReadAllText(jsonFile);
                    targetCollection.Add(JsonConvert.DeserializeObject<T>(jsonFileText));
                }
                Debug.WriteLine("Completed reading json files from: " + directoryPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading json files from: " + directoryPath + "/" + ex.Message);
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

                //Json serialize
                string serializedObject = JsonConvert.SerializeObject(serializeObject, jsonSettings);

                //Check directory
                string directoryName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                    Debug.WriteLine("Created json file directory: " + directoryName);
                }

                //Save to file
                File.WriteAllText(filePath, serializedObject);
                Debug.WriteLine("Completed saving json file: " + filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving json file: " + filePath + "/" + ex.Message);
            }
        }
    }
}