// <copyright file="MaxVirtualPathProviderOverride.cs" company="Lakstins Family, LLC">
// Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// </copyright>

#region License
// <license>
// This software is provided 'as-is', without any express or implied warranty. In no 
// event will the author be held liable for any damages arising from the use of this 
// software.
//  
// Permission is granted to anyone to use this software for any purpose, including 
// commercial applications, and to alter it and redistribute it freely, subject to the 
// following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not claim that 
// you wrote the original software. If you use this software in a product, an 
// acknowledgment (see the following) in the product documentation is required.
// 
// Portions Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// 
// 2. Altered source versions must be plainly marked as such, and must not be 
// misrepresented as being the original software.
// 
// 3. This notice may not be removed or altered from any source distribution.
// </license>
#endregion

#region Change Log
// <changelog>
// <change date="4/18/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="4/21/2014" author="Brian A. Lakstins" description="Get cache dependency working.">
// <change date="8/13/2014" author="Brian A. Lakstins" description="Added logging.">
// <change date="4/24/2016" author="Brian A. Lakstins" description="Update for no longer returning null from GetCurrent">
// <change date="5/18/2016" author="Brian A. Lakstins" description="Fix issue with name being case sensitive in database">
// <change date="5/16/2018" author="Brian A. Lakstins" description="Added support for StorageKey specific files">
// <change date="5/6/2019" author="Brian A. Lakstins" description="Fix issue caching content for different storagekey">
// <change date="6/26/2019" author="Brian A. Lakstins" description="Fix issue with view file specific for storagekey that also has a general view file.  Storage Key view file is higher priority.">
// <change date="3/9/2020" author="Brian A. Lakstins" description="Update logging of DLL registration process failing.">
// <change date="5/22/2020" author="Brian A. Lakstins" description="Fix reference to StorageKey.">
// <change date="5/24/2020" author="Brian A. Lakstins" description="Update exception logging for a common exception type">
// <change date="2/5/2021" author="Brian A. Lakstins" description="Add more extensions that can be handled in the Views folder for VueJS apps.  Same extensions need to be handled in web.config in 'Process Static Files, or try as folder name' rewrite rule">
// <change date="2/7/2021" author="Brian A. Lakstins" description="Fix issue loading extensions from assemblys">
// <change date="3/3/2021" author="Brian A. Lakstins" description="Add ttf files.  Handle files from file system like they all might be binary.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class.">
// <change date="6/4/2025" author="Brian A. Lakstins" description="Updates for changes to dependent classes.">
// <change date="6/11/2025" author="Brian A. Lakstins" description="Update for ApplicationKey">
// <change date="6/17/2025" author="Brian A. Lakstins" description="Update logging">
// <change date="6/21/2025" author="Brian A. Lakstins" description="Integrate version information">
// </changelog>
#endregion

namespace System.Web.Hosting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Web.Caching;
    using MaxFactry.General.AspNet.BusinessLayer;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer.Library;

    /// <summary>
    /// MaxFactry implementation of the VirtualPathProvider
    /// </summary>
	public class MaxVirtualPathProviderOverride : VirtualPathProvider
    {
        /// <summary>
        /// File does not exist
        /// </summary>
        public const int FileTypeBlank = 1;

        /// <summary>
        /// File information is contained in an entity
        /// </summary>
        public const int FileTypeEntity = 10;

        /// <summary>
        /// File information is contained in a file
        /// </summary>
        public const int FileTypeFile = 11;

        /// <summary>
        /// File information is contained in memory
        /// </summary>
        public const int FileTypeVirtual = 12;

        /// <summary>
        /// File information is embedded in a library
        /// </summary>
        public const int FileTypeEmbedded = 13;

        /// <summary>
        /// File information is contained in a file in the storage key folder under views
        /// </summary>
        public const int FileTypeFileStorageKey = 14;

        /// <summary>
        /// View extensions to handle
        /// </summary>
        public static readonly List<string> ViewExtensionList = new List<string>(new string[] { "cshtml", "txt", "htm", "html", "js", "css", "png", "json", "ttf" });

        /// <summary>
        /// Stores index of virtual text file content based on virtual path managed programmatically.
        /// </summary>
        private static Dictionary<string, string> _oVirtualTextFileIndex = new Dictionary<string, string>();

        /// <summary>
        /// Stores index of embedded text file content based on virtual path.
        /// </summary>
        private static Dictionary<string, string> _oEmbeddedTextIndex = new Dictionary<string, string>();

        /// <summary>
        /// Stores index of file type results so files that don't exists don't have to be checked for every time.
        /// </summary>
        private static Dictionary<string, int> _oFileTypeIndex = new Dictionary<string, int>();

        /// <summary>
        /// Stores index of file content so files don't need to be read from the file system each time they are used.
        /// </summary>
        private static Dictionary<string, string> _oFileContentIndex = new Dictionary<string, string>();

        /// <summary>
        /// Stores index of file hashs so files can be cached.
        /// </summary>
        private static Dictionary<string, string> _oFileHashIndex = new Dictionary<string, string>();

        /// <summary>
        /// Stores indicator of file existance based on virtual path.
        /// </summary>
        private static Dictionary<string, bool> _oFileExistsIndex = new Dictionary<string, bool>();

        /// <summary>
        /// Object to lock thread access to static dictionaries
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Register this path provider
        /// </summary>
        public static void Register()
        {
            Assembly[] loAssemblyList = AppDomain.CurrentDomain.GetAssemblies();
            string lsCurrentAssembly = string.Empty;
            if (null != MaxFactryLibrary.GetValue("CurrentAssemblyName") && MaxFactryLibrary.GetValue("CurrentAssemblyName") is string)
            {
                lsCurrentAssembly = (string)MaxFactryLibrary.GetValue("CurrentAssemblyName");
            }

            foreach (Assembly loAssembly in loAssemblyList)
            {
                if (!loAssembly.ManifestModule.Name.Equals(lsCurrentAssembly, StringComparison.InvariantCultureIgnoreCase) &&
                    !loAssembly.FullName.StartsWith("DynamicProxyGenAssembly2"))
                {
                    try
                    {
                        string[] laFileName = loAssembly.GetManifestResourceNames();
                        foreach (string lsName in laFileName)
                        {
                            string lsExtension = lsName.ToLower().Substring(lsName.LastIndexOf('.') + 1);
                            if (lsExtension == "cshtml")
                            {
                                int lnStart = 0;
                                while (loAssembly.FullName[lnStart] == lsName[lnStart] ||
                                    (loAssembly.FullName[lnStart] == '-' && lsName[lnStart] == '_'))
                                {
                                    lnStart++;
                                }

                                string lsVirtualPath = lsName.Substring(lnStart).ToLowerInvariant();
                                lsVirtualPath = lsVirtualPath.Replace("." + lsExtension, string.Empty).Replace('.', '/') + "." + lsExtension;
                                string lsKey = GetKey(lsVirtualPath);
                                if (IsView(lsKey) && IsHandled(lsKey))
                                {
                                    if (!_oEmbeddedTextIndex.ContainsKey(lsKey))
                                    {
                                        lock (_oLock)
                                        {
                                            if (!_oEmbeddedTextIndex.ContainsKey(lsKey))
                                            {
                                                StreamReader loReader = new StreamReader(loAssembly.GetManifestResourceStream(lsName));
                                                string lsText = loReader.ReadToEnd();
                                                _oEmbeddedTextIndex.Add(lsKey, lsText);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception loE)
                    {
                        if (loE is NotSupportedException)
                        {
                            MaxLogLibrary.Log(new MaxLogEntryStructure("VirtualPathProvider", MaxEnumGroup.LogWarning, "Registering Assembly {loAssembly} caused NotSupportedException.", loAssembly));
                        }
                        else
                        {
                            MaxLogLibrary.Log(new MaxLogEntryStructure("VirtualPathProvider", MaxEnumGroup.LogError, "Register process for Assembly {loAssembly} failed.", loE, loAssembly));
                        }
                    }
                }
            }

            HostingEnvironment.RegisterVirtualPathProvider(new MaxVirtualPathProviderOverride());
        }

        /// <summary>
        /// Gets the children of the current folder.  Used by MaxVirtualDirectoryOverride.
        /// </summary>
        /// <param name="lsPath">Path to a folder.</param>
        /// <returns>List of children of the folder.</returns>
        public static string[] GetChildren(string lsPath)
        {
            List<string> loR = new List<string>();
            string lsFolder = VirtualPathUtility.ToAbsolute(lsPath);
            foreach (string lsFile in Directory.GetFiles(lsFolder))
            {
                loR.Add("F" + lsFile);
            }

            foreach (string lsDirectory in Directory.GetDirectories(lsFolder))
            {
                loR.Add("D" + lsDirectory);
            }

            return loR.ToArray();
        }

        /// <summary>
        /// Opens a stream to the virtual file.  Used by MaxVirtualFileOverride.
        /// </summary>
        /// <param name="lsVirtualPath">The path to the primary virtual resource.</param>
        /// <returns>Stream of content of the virtual file.</returns>
        public static Stream Open(string lsVirtualPath)
        {
            MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure(typeof(MaxVirtualPathProviderOverride), "Open", MaxFactry.Core.MaxEnumGroup.LogInfo, "Start {Path}", lsVirtualPath));
            int lnFileType = GetFileType(lsVirtualPath);
            Stream loR = null;
            if (lnFileType > FileTypeBlank)
            {
                string lsFileNameKey = GetKey(lsVirtualPath);
                string lsText = null;
                if (lnFileType == FileTypeEntity)
                {
                    bool lbIsFound = false;
                    MaxVirtualTextFileEntity loEntity = MaxVirtualTextFileEntity.Create().GetCurrent(lsFileNameKey) as MaxVirtualTextFileEntity;
                    if (null != loEntity)
                    {
                        lbIsFound = true;
                        lsText = loEntity.Content;
                        lsFileNameKey += loEntity.Version.ToString();
                    }
                }
                else if (lnFileType == FileTypeFile)
                {
                    loR = File.OpenRead(HttpContext.Current.Server.MapPath(lsVirtualPath));
                }
                else if (lnFileType == FileTypeFileStorageKey)
                {
                    string lsStorageKey = GetStorageKey();
                    string lsFileStorageKey = lsStorageKey + lsFileNameKey;
                    loR = File.OpenRead(HttpContext.Current.Server.MapPath(lsVirtualPath.ToLower().Replace("/views/", "/views/" + lsStorageKey + "/")));
                }
                else if (lnFileType == FileTypeVirtual)
                {
                    lsText = _oVirtualTextFileIndex[lsFileNameKey];
                }
                else if (lnFileType == FileTypeEmbedded)
                {
                    lsText = _oEmbeddedTextIndex[lsFileNameKey];
                }

                if (!string.IsNullOrEmpty(lsText))
                {
                    byte[] laText = System.Text.UTF8Encoding.UTF8.GetBytes(lsText);
                    loR = new System.IO.MemoryStream(laText);
                    string lsHash = MaxEncryptionLibrary.GetHash(typeof(object), MaxEncryptionLibrary.MD5Hash, laText);
                    if (!_oFileHashIndex.ContainsKey(lsFileNameKey))
                    {
                        lock (_oLock)
                        {
                            if (!_oFileHashIndex.ContainsKey(lsFileNameKey))
                            {
                                _oFileHashIndex.Add(lsFileNameKey, lsHash);
                            }
                        }
                    }
                    else if (_oFileHashIndex[lsFileNameKey] != lsHash)
                    {
                        _oFileHashIndex[lsFileNameKey] = lsHash;
                    }
                }
            }

            return loR;
        }

        /// <summary>
        /// Gets all handled virtual paths and their current content
        /// </summary>
        /// <returns>List of all virtual paths handled by the provider.</returns>
        public static string[] GetVirtualPathList()
        {
            List<string> loList = new List<string>();
            foreach (string lsKey in _oEmbeddedTextIndex.Keys)
            {
                loList.Add(lsKey);
            }

            AddFileList(loList, "/views/");
            return loList.ToArray();
        }

        /// <summary>
        /// Adds files in the file system to the virtual path list.
        /// </summary>
        /// <param name="loList">List of virtual paths.</param>
        /// <param name="lsPath">Current file system path to add.</param>
        private static void AddFileList(List<string> loList, string lsPath)
        {
            string lsBaseAbsolute = HttpContext.Current.Server.MapPath("~/");
            string lsBaseDirectory = HttpContext.Current.Server.MapPath(lsPath);
            foreach (string lsFile in Directory.GetFiles(lsBaseDirectory))
            {
                string lsVirtualPath = lsFile.Substring(lsBaseAbsolute.Length - 1);
                if (IsView(lsVirtualPath))
                {
                    FileInfo loFile = new FileInfo(lsFile);
                    string lsKey = (lsPath + loFile.Name).ToLowerInvariant();
                    if (!loList.Contains(lsKey))
                    {
                        loList.Add(lsKey);
                    }
                }
            }

            foreach (string lsDirectory in Directory.GetDirectories(lsBaseDirectory))
            {
                DirectoryInfo loDirectory = new DirectoryInfo(lsDirectory);
                AddFileList(loList, lsPath + loDirectory.Name + "/");
            }
        }


        /// <summary>
        /// Gets a value that indicates whether a file exists in the virtual file system.
        /// </summary>
        /// <param name="lsVirtualPath">The path to the virtual file.</param>
        /// <returns>true if the file exists in the virtual file system; otherwise, false.</returns>
        public override bool FileExists(string lsVirtualPath)
        {
            MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "FileExists", MaxFactry.Core.MaxEnumGroup.LogDebug, "Start for {Path}", lsVirtualPath));
            string lsFileExistsKey = GetStorageKey() + GetKey(lsVirtualPath);
            bool lbR = true;
            if (_oFileExistsIndex.ContainsKey(lsFileExistsKey))
            {
                lbR = _oFileExistsIndex[lsFileExistsKey];
            }

            if (lbR)
            {
                int lnFileType = GetFileType(lsVirtualPath);
                if (lnFileType > 0)
                {
                    if (lnFileType > FileTypeBlank)
                    {
                        MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "FileExists", MaxFactry.Core.MaxEnumGroup.LogInfo, "File {File} is Type {Type}", lsVirtualPath, lnFileType));
                    }
                    else
                    {
                        lbR = false;
                    }
                }
                else
                {
                    lbR = Previous.FileExists(lsVirtualPath);
                }

                if (!lbR)
                {
                    if (!_oFileExistsIndex.ContainsKey(lsFileExistsKey))
                    {
                        lock (_oLock)
                        {
                            if (!_oFileExistsIndex.ContainsKey(lsFileExistsKey))
                            {
                                _oFileExistsIndex.Add(lsFileExistsKey, lbR);
                            }
                        }
                    }
                }
            }

            MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "FileExists", MaxFactry.Core.MaxEnumGroup.LogDebug, "End for {Path}", lsVirtualPath));
            return lbR;
        }

        /// <summary>
        /// Gets a virtual file from the virtual file system.
        /// </summary>
        /// <param name="lsVirtualPath">The path to the virtual file.</param>
        /// <returns>A descendent of the System.Web.Hosting.VirtualFile class that represents a file in the virtual file system.</returns>
        public override VirtualFile GetFile(string lsVirtualPath)
        {
            System.Diagnostics.Stopwatch loWatch = System.Diagnostics.Stopwatch.StartNew();
            MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "GetFile", MaxFactry.Core.MaxEnumGroup.LogDebug, "Start for [{Path}]", lsVirtualPath));
            int lnFileType = GetFileType(lsVirtualPath);
            VirtualFile loR = null;
            if (lnFileType > 0)
            {
                if (lnFileType > FileTypeBlank)
                {
                    loR = new MaxVirtualFileOverride(lsVirtualPath);
                }
            }
            else
            {
                loR = Previous.GetFile(lsVirtualPath);
            }

            if (null != loR)
            {
                if (loWatch.ElapsedMilliseconds > 100)
                {
                    MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "GetFile", MaxFactry.Core.MaxEnumGroup.LogInfo, "Path [{Path}] took {Time}ms end [{Name}]", lsVirtualPath, loWatch.ElapsedMilliseconds, loR.Name));
                }
            }

            return loR;
        }

        /// <summary>
        /// Gets a value that indicates whether a directory exists in the virtual file system.
        /// </summary>
        /// <param name="lsVirtualDir">The path to the virtual directory.</param>
        /// <returns>true if the directory exists in the virtual file system; otherwise, false.</returns>
        public override bool DirectoryExists(string lsVirtualDir)
        {
            return Previous.DirectoryExists(lsVirtualDir);
        }

        /// <summary>
        /// Gets a virtual directory from the virtual file system.
        /// </summary>
        /// <param name="lsVirtualDir">The path to the virtual directory.</param>
        /// <returns>A descendent of the System.Web.Hosting.VirtualDirectory class that represents a directory in the virtual file system.</returns>
        public override VirtualDirectory GetDirectory(string lsVirtualDir)
        {
            return Previous.GetDirectory(lsVirtualDir);
        }

        /// <summary>
        /// Creates a cache dependency based on the specified virtual paths.
        /// </summary>
        /// <param name="lsVirtualPath">The path to the primary virtual resource.</param>
        /// <param name="laVirtualPathDependencies">An array of paths to other resources required by the primary virtual resource.</param>
        /// <param name="ldUtcStart">The UTC time at which the virtual resources were read.</param>
        /// <returns>A System.Web.Caching.CacheDependency object for the specified virtual resources.</returns>
        public override CacheDependency GetCacheDependency(string lsVirtualPath, IEnumerable laVirtualPathDependencies, DateTime ldUtcStart)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "GetCacheDependency([" + lsVirtualPath + "]) start", "MaxVirtualPathProvider");
            int lnFileType = GetFileType(lsVirtualPath);
            CacheDependency loR = null;
            if (lnFileType > 0)
            {
                if (lnFileType == FileTypeFile)
                {
                    loR = new CacheDependency(HttpContext.Current.Server.MapPath(lsVirtualPath));
                }
                else if (lnFileType == FileTypeFileStorageKey)
                {
                    string lsStorageKey = GetStorageKey();
                    loR = new CacheDependency(HttpContext.Current.Server.MapPath(lsVirtualPath.ToLower().Replace("/views/", "/views/" + lsStorageKey + "/")));
                }
            }
            else
            {
                loR = Previous.GetCacheDependency(lsVirtualPath, laVirtualPathDependencies, ldUtcStart);
            }

            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "GetCacheDependency([" + lsVirtualPath + "]) end", "MaxVirtualPathProvider");
            return loR;
        }

        public override string GetFileHash(string lsVirtualPath, IEnumerable loVirtualPathDependencyList)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "GetFileHash([" + lsVirtualPath + "]) start", "MaxVirtualPathProvider");
            int lnFileType = GetFileType(lsVirtualPath);
            string lsR = null;
            if (lnFileType > 0)
            {
                if (lnFileType > FileTypeBlank)
                {
                    string lsKey = GetKey(lsVirtualPath);
                    if (lnFileType == FileTypeEntity)
                    {
                        MaxEntityList loList = MaxVirtualTextFileEntity.Create().LoadAllActiveCache();
                        bool lbIsFound = false;
                        for (int lnE = 0; lnE < loList.Count && !lbIsFound; lnE++)
                        {
                            MaxVirtualTextFileEntity loEntity = (MaxVirtualTextFileEntity)loList[lnE];
                            if (loEntity.Name.Equals(lsVirtualPath, StringComparison.InvariantCultureIgnoreCase))
                            {
                                lbIsFound = true;
                                lsKey += loEntity.Version.ToString();
                            }
                        }
                    }
                    else if (lnFileType == FileTypeFileStorageKey)
                    {
                        string lsStorageKey = GetStorageKey();
                        lsKey = lsStorageKey + lsKey;
                    }

                    if (null != HttpContext.Current && null != HttpContext.Current.Request && null != HttpContext.Current.Request.QueryString)
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["nocache"]))
                        {
                            lsKey += Guid.NewGuid().ToString();
                        }
                    }

                    if (_oFileHashIndex.ContainsKey(lsKey))
                    {
                        lsR = _oFileHashIndex[lsKey];
                        MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "GetFileHash([" + lsVirtualPath + "]) end virtual [" + lsR + "]", "MaxVirtualPathProvider");
                    }
                }
            }

            return lsR;
        }

        /// <summary>
        /// Returns a cache key to use for the specified virtual path.
        /// </summary>
        /// <param name="lsVirtualPath">The path to the virtual resource.</param>
        /// <returns>A cache key for the specified virtual resource.</returns>
        public override string GetCacheKey(string lsVirtualPath)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "GetCacheKey([" + lsVirtualPath + "]) start", "MaxVirtualPathProvider");
            int lnFileType = GetFileType(lsVirtualPath);
            string lsR = null;
            if (lnFileType > 0)
            {
                lsR = this.GetType() + "/" + MaxDataLibrary.GetApplicationKey() + "/";
                if (lnFileType > FileTypeBlank)
                {
                    string lsKey = GetKey(lsVirtualPath);
                    if (lnFileType == FileTypeEntity)
                    {
                        MaxEntityList loList = MaxVirtualTextFileEntity.Create().LoadAllActiveCache();
                        bool lbIsFound = false;
                        for (int lnE = 0; lnE < loList.Count && !lbIsFound; lnE++)
                        {
                            MaxVirtualTextFileEntity loEntity = (MaxVirtualTextFileEntity)loList[lnE];
                            if (loEntity.Name.Equals(lsVirtualPath, StringComparison.InvariantCultureIgnoreCase))
                            {
                                lbIsFound = true;
                                lsKey += loEntity.Version.ToString();
                            }
                        }
                    }
                    else if (lnFileType == FileTypeFileStorageKey)
                    {
                        string lsStorageKey = GetStorageKey();
                        lsKey = lsStorageKey + lsKey;
                    }

                    if (null != HttpContext.Current && null != HttpContext.Current.Request && null != HttpContext.Current.Request.QueryString)
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["nocache"]))
                        {
                            lsKey += Guid.NewGuid().ToString();
                        }
                    }

                    if (!string.IsNullOrEmpty(lsKey))
                    {
                        lsR += MaxEncryptionLibrary.GetHash(typeof(object), MaxEncryptionLibrary.MD5Hash, System.Text.UTF8Encoding.UTF8.GetBytes(lsKey));
                    }
                }
                else
                {
                    lsR += lsVirtualPath;
                }

                lsR = MaxEncryptionLibrary.GetHash(typeof(object), "MD5", lsR);
                MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "GetCacheKey([" + lsVirtualPath + "]) end virtual [" + lsR + "]", "MaxVirtualPathProvider");
            }
            else
            {
                lsR = Previous.GetCacheKey(lsVirtualPath);
                MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "GetCacheKey([" + lsVirtualPath + "]) end previous [" + lsR + "]", "MaxVirtualPathProvider");
            }

            return lsR;
        }

        /// <summary>
        /// Registers programmitic content as a text file for entire web site 
        /// </summary>
        /// <param name="lsVirtualPath">The path to the virtual resource.</param>
        /// <param name="lsContent">The content of the resource</param>
        public static void RegisterVirtualTextFile(string lsVirtualPath, string lsContent)
        {
            string lsFileNameKey = GetKey(lsVirtualPath);
            if (_oVirtualTextFileIndex.ContainsKey(lsFileNameKey))
            {
                _oVirtualTextFileIndex[lsFileNameKey] = lsContent;
            }
            else
            {
                _oVirtualTextFileIndex.Add(lsFileNameKey, lsContent);
            }
        }

        /// <summary>
        /// Gets the storage key
        /// </summary>
        /// <returns>Current storage key for this thread.</returns>
        private static string GetStorageKey()
        {
            string lsR = MaxDataLibrary.GetApplicationKey().ToLowerInvariant();
            return lsR;
        }

        /// <summary>
        /// Gets they key used to store and look up the virtual path
        /// </summary>
        /// <param name="lsVirtualPath">The path to the virtual resource.</param>
        /// <returns>key for the resource</returns>
        private static string GetKey(string lsVirtualPath)
        {
            return VirtualPathUtility.ToAbsolute(lsVirtualPath).ToLowerInvariant();
        }

        /// <summary>
        /// Checks to see if MaxVirtualPathProvider handles this file.
        /// </summary>
        /// <param name="lsKey">Key used to describe the file</param>
        /// <returns>True if handled.</returns>
        private static bool IsHandled(string lsKey)
        {
            if (lsKey.Contains(".mobile."))
            {
                return false;
            }

            foreach (string lsExtension in ViewExtensionList)
            {
                if (lsKey.EndsWith(lsExtension))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see if MaxVirtualPathProvider handles this file.
        /// </summary>
        /// <param name="lsKey">Key used to describe the file</param>
        /// <returns>True if handled.</returns>
        private static bool IsView(string lsKey)
        {
            if (lsKey.Contains("/views/"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the type of file that matches the virtual path.
        /// </summary>
        /// <param name="lsVirtualPath">The path to the virtual resource.</param>
        /// <returns>integer indicating file type</returns>
        private static int GetFileType(string lsVirtualPath)
        {
            int lnR = 0;
            string lsFileNameKey = GetKey(lsVirtualPath);
            string lsStorageKey = GetStorageKey();
            string lsFileTypeKey = lsStorageKey + lsFileNameKey;
            if (IsView(lsFileNameKey))
            {
                lnR = FileTypeBlank;
                if (IsHandled(lsFileNameKey))
                {
                    if (!_oFileTypeIndex.ContainsKey(lsFileTypeKey))
                    {
                        lock (_oLock)
                        {
                            if (!_oFileTypeIndex.ContainsKey(lsFileTypeKey))
                            {
                                if (File.Exists(HttpContext.Current.Server.MapPath(lsVirtualPath.ToLower().Replace("/views/", "/views/" + lsStorageKey + "/"))))
                                {
                                    // File existing in file system for current storage key (best match)  
                                    lnR = FileTypeFileStorageKey;
                                }
                                else if (File.Exists(HttpContext.Current.Server.MapPath(lsVirtualPath)))
                                {
                                    // File existing in file system general
                                    lnR = FileTypeFile;
                                }
                                else if (_oVirtualTextFileIndex.ContainsKey(lsFileNameKey))
                                {
                                    // File added programatically via RegisterVirtualTextFile method
                                    lnR = FileTypeVirtual;
                                }
                                else if (_oEmbeddedTextIndex.ContainsKey(lsFileNameKey))
                                {
                                    // File existing embedded in a referenced library (worst match)
                                    lnR = FileTypeEmbedded;
                                }

                                _oFileTypeIndex.Add(lsFileTypeKey, lnR);
                            }
                        }
                    }

                    lnR = _oFileTypeIndex[lsFileTypeKey];

                    // Try storage key specific locations, which are all better matches.
                    if (null != MaxVirtualTextFileEntity.Create().GetCurrent(lsFileNameKey))
                    {
                        // Entity based content 
                        lnR = FileTypeEntity;
                    }
                }
            }

            return lnR;
        }
    }
}