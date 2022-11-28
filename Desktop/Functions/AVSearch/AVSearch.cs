using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ArnoldVinkCode
{
    public partial class AVSearch
    {
        //Search for files in folders
        public static string[] Search_Files(string[] searchFiles, SearchSource[] searchSources, bool searchContains)
        {
            IEnumerable<string> foundFiles = new List<string>();
            try
            {
                foreach (string searchFile in searchFiles)
                {
                    try
                    {
                        //Validate the search term
                        if (string.IsNullOrWhiteSpace(searchFile)) { continue; }

                        //Adjust the search term
                        string loadFileLower = searchFile.ToLower();
                        loadFileLower = AVFunctions.StringRemoveStart(loadFileLower, " ");
                        loadFileLower = AVFunctions.StringRemoveEnd(loadFileLower, " ");
                        if (!loadFileLower.Contains("/") && !loadFileLower.Contains("\\"))
                        {
                            loadFileLower = string.Join(string.Empty, loadFileLower.Split(Path.GetInvalidFileNameChars()));
                        }
                        Debug.WriteLine("Searching for file: " + loadFileLower);

                        //Search for files in source paths
                        foreach (SearchSource searchSource in searchSources)
                        {
                            try
                            {
                                if (Directory.Exists(searchSource.SearchPath))
                                {
                                    IEnumerable<string> possibleFiles = new List<string>();
                                    foreach (string searchPattern in searchSource.SearchPatterns)
                                    {
                                        possibleFiles = possibleFiles.Concat(Directory.GetFiles(searchSource.SearchPath, searchPattern, searchSource.SearchOption));
                                    }

                                    if (searchContains)
                                    {
                                        foundFiles = foundFiles.Concat(possibleFiles.Where(x => Path.GetFileNameWithoutExtension(x).ToLower().Contains(loadFileLower)));
                                    }
                                    else
                                    {
                                        foundFiles = foundFiles.Concat(possibleFiles.Where(x => Path.GetFileNameWithoutExtension(x).ToLower() == loadFileLower));
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed searching file: " + searchFile + " / " + ex.Message);
                    }
                }
            }
            catch { }
            return foundFiles.ToArray();
        }
    }
}