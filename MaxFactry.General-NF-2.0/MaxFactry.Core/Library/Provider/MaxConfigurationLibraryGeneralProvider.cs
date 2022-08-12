// <copyright file="MaxConfigurationLibraryGeneralProvider.cs" company="Lakstins Family, LLC">
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
// <change date="1/19/2017" author="Brian A. Lakstins" description="Initial creation">
// <change date="4/20/2017" author="Brian A. Lakstins" description="Add to MaxSystem index information">
// <change date="5/25/2017" author="Brian A. Lakstins" description="Update key atching to not require ToLowerCase method as often to improve memory usage.">
// <change date="7/26/2018" author="Brian A. Lakstins" description="Add more to System information.">
// <change date="2/19/2019" author="Brian A. Lakstins" description="Updated to only generate system information once.">
// <change date="6/19/2019" author="Brian A. Lakstins" description="Added Persistent configuration.">
// <change date="2/7/2020" author="Brian A. Lakstins" description="Speed up accessing system information by caching query results.">
// <change date="5/19/2020" author="Brian A. Lakstins" description="Cache results for Profile configuration to help speed up accessing profile information.">
// <change date="5/21/2020" author="Brian A. Lakstins" description="Using a provider to indicate what provider to use ends up creating the provider as the provider. Fixed by using typeof(object) in a provider.">
// <change date="6/16/2020" author="Brian A. Lakstins" description="Fix getting value for profile when not in cache.">
// </changelog>
#endregion

namespace MaxFactry.Core.Provider
{
    using System;
    using System.IO;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using MaxFactry.Core;
    using MaxFactry.Core.Provider;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.General.BusinessLayer;

    /// <summary>
    /// MaxFactory Provider for use in web applications.
    /// TODO: Review this and move code to lower level if possible.
    /// </summary>
    public class MaxConfigurationLibraryGeneralProvider : MaxConfigurationLibraryDefaultProvider
    {
        /// <summary>
        /// Static object used to lock thread access
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Application configuration index.
        /// </summary>
        private static MaxIndex _oConfig = null;

        /// <summary>
        /// Caches Management object information for futher queries
        /// </summary>
        private MaxIndex _oManagementObjectIndex = new MaxIndex();

        /// <summary>
        /// Gets a value in a scope.
        /// </summary>
        /// <param name="loScope">The scope to use.</param>
        /// <param name="lsKey">The key to use to look up the value.</param>
        /// <returns>The value if it is found.  Null if not found.</returns>
        public override object GetValue(MaxEnumGroup loScope, string lsKey)
        {
            object loValue = null;
            if (lsKey.Equals("MaxSystem"))
            {
                MaxIndex loIndex = new MaxIndex();
                if (loScope == MaxEnumGroup.ScopeProfile)
                {
                    Guid loId = this.GetCurrentProfileId();
                    loIndex.Add("MaxComputerName", MaxProfileIndexEntity.Create().GetValue(loId, "MaxComputerName") as string);
                    loIndex.Add("MaxBIOSSerialNumber", MaxProfileIndexEntity.Create().GetValue(loId, "MaxBIOSSerialNumber") as string);

                    loIndex.Add("MaxBIOSManufacturer", MaxProfileIndexEntity.Create().GetValue(loId, "MaxBIOSManufacturer") as string);
                    loIndex.Add("MaxBIOSSMBIOSBIOSVersion", MaxProfileIndexEntity.Create().GetValue(loId, "MaxBIOSSMBIOSBIOSVersion") as string);
                    loIndex.Add("MaxBIOSIdentificationCode", MaxProfileIndexEntity.Create().GetValue(loId, "MaxBIOSIdentificationCode") as string);
                    loIndex.Add("MaxBIOSReleaseDate", MaxProfileIndexEntity.Create().GetValue(loId, "MaxBIOSReleaseDate") as string);
                    loIndex.Add("MaxBIOSVersion", MaxProfileIndexEntity.Create().GetValue(loId, "MaxBIOSVersion") as string);


                    loIndex.Add("MaxProcessorUniqueId", MaxProfileIndexEntity.Create().GetValue(loId, "MaxProcessorUniqueId") as string);
                    loIndex.Add("MaxProcessorId", MaxProfileIndexEntity.Create().GetValue(loId, "MaxProcessorId") as string);
                    loIndex.Add("MaxProcessorName", MaxProfileIndexEntity.Create().GetValue(loId, "MaxProcessorName") as string);
                    loIndex.Add("MaxProcessorManufacturer", MaxProfileIndexEntity.Create().GetValue(loId, "MaxProcessorManufacturer") as string);
                    loIndex.Add("MaxProcessorClockSpeed", MaxProfileIndexEntity.Create().GetValue(loId, "MaxProcessorClockSpeed") as string);

                    loIndex.Add("MaxBaseBoardProduct", MaxProfileIndexEntity.Create().GetValue(loId, "MaxBaseBoardProduct") as string);
                    loIndex.Add("MaxBaseBoardManufacturer", MaxProfileIndexEntity.Create().GetValue(loId, "MaxBaseBoardManufacturer") as string);
                    loIndex.Add("MaxBaseBoardModel", MaxProfileIndexEntity.Create().GetValue(loId, "MaxBaseBoardModel") as string);
                    loIndex.Add("MaxBaseBoardName", MaxProfileIndexEntity.Create().GetValue(loId, "MaxBaseBoardName") as string);
                    loIndex.Add("MaxBaseBoardSerialNumber", MaxProfileIndexEntity.Create().GetValue(loId, "MaxBaseBoardSerialNumber") as string);

                    loIndex.Add("MaxDiskDrive", MaxProfileIndexEntity.Create().GetValue(loId, "MaxDiskDrive") as string);
                    loIndex.Add("MaxVideoController", MaxProfileIndexEntity.Create().GetValue(loId, "MaxVideoController") as string);
                    loIndex.Add("MaxPhysicalMedia", MaxProfileIndexEntity.Create().GetValue(loId, "MaxPhysicalMedia") as string);
                    loIndex.Add("MaxOperatingSystem", MaxProfileIndexEntity.Create().GetValue(loId, "MaxOperatingSystem") as string);
                    loIndex.Add("MaxNetworkAddress", MaxProfileIndexEntity.Create().GetValue(loId, "MaxNetworkAddress") as string);
                    loIndex.Add("MaxNetworkAdapterMAC", MaxProfileIndexEntity.Create().GetValue(loId, "MaxNetworkAdapterMAC") as string);
                    loIndex.Add("MaxSystemVersion", MaxProfileIndexEntity.Create().GetValue(loId, "MaxSystemVersion") as string);
                }
                else if (loScope == MaxEnumGroup.ScopeApplication)
                {
                    MaxIndex loIndexCache = MaxFactry.Base.DataLayer.MaxCacheRepository.Get(typeof(object), "MaxSystemApplication", typeof(MaxIndex)) as MaxIndex;
                    if (null == loIndexCache)
                    {
                        lock (_oLock)
                        {
                            loIndexCache = MaxFactry.Base.DataLayer.MaxCacheRepository.Get(typeof(object), "MaxSystemApplication", typeof(MaxIndex)) as MaxIndex;
                            if (null == loIndexCache)
                            {
                                //// https://www.codeproject.com/Articles/28678/Generating-Unique-Key-Finger-Print-for-a-Computer
                                loIndex.Add("MaxComputerName", Environment.MachineName);
                                loIndex.Add("MaxBIOSSerialNumber", this.GetSystemValue("BIOS", "SerialNumber", false));
                                loIndex.Add("MaxBIOSManufacturer", this.GetSystemValue("BIOS", "Manufacturer", false));
                                loIndex.Add("MaxBIOSSMBIOSBIOSVersion", this.GetSystemValue("BIOS", "SMBIOSBIOSVersion", false));
                                loIndex.Add("MaxBIOSIdentificationCode", this.GetSystemValue("BIOS", "IdentificationCode", false));
                                loIndex.Add("MaxBIOSReleaseDate", this.GetSystemValue("BIOS", "ReleaseDate", false));
                                loIndex.Add("MaxBIOSVersion", this.GetSystemValue("BIOS", "Version", false));

                                loIndex.Add("MaxProcessorUniqueId", this.GetSystemValue("Processor", "UniqueId", false));
                                loIndex.Add("MaxProcessorId", this.GetSystemValue("Processor", "ProcessorId", false));
                                loIndex.Add("MaxProcessorName", this.GetSystemValue("Processor", "Name", false));
                                loIndex.Add("MaxProcessorManufacturer", this.GetSystemValue("Processor", "Manufacturer", false));
                                loIndex.Add("MaxProcessorClockSpeed", this.GetSystemValue("Processor", "MaxClockSpeed", false));

                                loIndex.Add("MaxBaseBoardProduct", this.GetSystemValue("BaseBoard", "Product", false));
                                loIndex.Add("MaxBaseBoardManufacturer", this.GetSystemValue("BaseBoard", "Manufacturer", false));
                                loIndex.Add("MaxBaseBoardModel", this.GetSystemValue("BaseBoard", "Model", false));
                                loIndex.Add("MaxBaseBoardName", this.GetSystemValue("BaseBoard", "Name", false));
                                loIndex.Add("MaxBaseBoardSerialNumber", this.GetSystemValue("BaseBoard", "SerialNumber", false));

                                loIndex.Add("MaxDiskDrive", this.GetSystemValue("DiskDrive", "SerialNumber", true));
                                loIndex.Add("MaxVideoController", this.GetSystemValue("VideoController", "Caption", true));
                                loIndex.Add("MaxPhysicalMedia", this.GetSystemValue("PhysicalMedia", "SerialNumber", true));
                                loIndex.Add("MaxOperatingSystem", this.GetSystemValue("OperatingSystem", "SerialNumber", false));
                                loIndex.Add("MaxNetworkAddress", this.GetSystemValue("NetworkInterface", "PhysicalAddress", true));
                                loIndex.Add("MaxNetworkAdapterMAC", this.GetSystemValue("NetworkAdapterConfiguration", "MACAddress", true));
                                loIndex.Add("MaxSystemVersion", "20180726");
                                MaxFactry.Base.DataLayer.MaxCacheRepository.Set(typeof(object), "MaxSystemApplication", loIndex);
                            }
                        }
                    }

                    if (null != loIndexCache)
                    {
                        loIndex = loIndexCache;
                    }
                }

                loValue = loIndex;
            }
            else if (loScope == MaxEnumGroup.ScopeProfile)
            {
                Guid loId = this.GetCurrentProfileId();
                if (lsKey.ToLower().Equals("id"))
                {
                    loValue = loId;
                }
                else
                {
                    try
                    {
                        //// Make this use cache to access value faster than looking in database
                        string lsCacheDataKey = this.GetType().ToString() + "ScopeProfile" + loId.ToString() + lsKey;
                        loValue = MaxCacheRepository.Get(typeof(object), lsCacheDataKey, typeof(string)) as string;
                        if (null == loValue)
                        {
                            loValue = MaxProfileIndexEntity.Create().GetValue(loId, lsKey);
                            if (loValue is string)
                            {
                                MaxCacheRepository.Set(typeof(object), lsCacheDataKey, loValue);
                            }
                            else if (null == loValue)
                            {
                                MaxCacheRepository.Set(typeof(object), lsCacheDataKey, string.Empty);
                            }
                        }
                    }
                    catch (Exception loE)
                    {
                        //// Can't get the value of the key for the profile, so just fail gracefully
                        MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error accessing Profile Scope entity", loE));
                        loValue = null;
                    }

                    if (loValue is string && string.Empty == (string)loValue)
                    {
                        loValue = null;
                    }
                }
            }
            else if (loScope == MaxEnumGroup.ScopePersistent)
            {
                if (null == _oConfig)
                {
                    lock (_oLock)
                    {
                        if (null == _oConfig)
                        {
                            //// Use an application configuration file.
                            string lsDataFolder = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxDataDirectory") as string;
                            if (!string.IsNullOrEmpty(lsDataFolder))
                            {
                                string lsConfigFile = System.IO.Path.Combine(lsDataFolder, "Config.json");
                                if (File.Exists(lsConfigFile))
                                {
                                    string lsConfigFileContent = File.ReadAllText(lsConfigFile);
                                    try
                                    {
                                        MaxIndex loFileConfig = MaxConvertLibrary.DeserializeObject(lsConfigFileContent, typeof(MaxIndex)) as MaxIndex;
                                        _oConfig = loFileConfig;
                                    }
                                    catch (Exception loE)
                                    {
                                        throw new MaxException("Configuration file is corrupt", loE);
                                    }
                                }
                            }

                            if (null == _oConfig)
                            {
                                _oConfig = new MaxIndex();
                            }
                        }
                    }
                }

                loValue = _oConfig[lsKey];
            }
            else
            {
                loValue = base.GetValue(loScope, lsKey);
            }


            return loValue;
        }

        /// <summary>
        /// Sets a value based on the scope.
        /// </summary>
        /// <param name="loScope">The scope for the value.</param>
        /// <param name="lsKey">The key to use to retrieve the value.</param>
        /// <param name="loValue">The value being set.</param>
        public override void SetValue(MaxEnumGroup loScope, string lsKey, object loValue)
        {
            if (loScope == MaxEnumGroup.ScopePersistent)
            {
                object loCurrent = this.GetValue(loScope, lsKey);
                if (loCurrent != loValue)
                {
                    lock (_oLock)
                    {
                        _oConfig.Add(lsKey, loValue);
                        string lsDataFolder = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxDataDirectory") as string;
                        if (!string.IsNullOrEmpty(lsDataFolder))
                        {
                            string lsConfigFile = System.IO.Path.Combine(lsDataFolder, "Config.json");
                            FileInfo loFile = new FileInfo(lsConfigFile);
                            if (!loFile.Directory.Exists)
                            {
                                Directory.CreateDirectory(loFile.DirectoryName);
                            }

                            File.WriteAllText(lsConfigFile, MaxConvertLibrary.SerializeObjectToString(_oConfig));
                        }
                    }
                }
            }
            else if (loScope == MaxEnumGroup.ScopeProfile)
            {
                string lsValue = string.Empty;
                if (null != loValue)
                {
                    lsValue = loValue.ToString();
                }

                Guid loId = this.GetCurrentProfileId();
                if (!lsKey.ToLower().Equals("Id".ToLower()))
                {
                    MaxProfileIndexEntity.Create().SaveValue(loId, lsKey, lsValue);
                    //// Make this use cache to access value faster than looking in database
                    string lsCacheDataKey = this.GetType().ToString() + "ScopeProfile" + loId.ToString() + lsKey;
                    MaxCacheRepository.Set(typeof(object), lsCacheDataKey, lsValue);
                }
            }
            else
            {
                base.SetValue(loScope, lsKey, loValue);
            }
        }

        /// <summary>
        /// Gets the current Profile Id
        /// </summary>
        /// <returns>Id of the Profile</returns>
        protected override Guid GetCurrentProfileId()
        {
            Guid loR = Guid.Empty;
            //// Check the process for the current profile Id
            object loValue = this.GetValue(MaxEnumGroup.ScopeProcess, "MaxProfileIdCurrent");
            if (null != loValue)
            {
                loR = MaxConvertLibrary.ConvertToGuid(typeof(object), loValue);
            }

            if (Guid.Empty.Equals(loR))
            {
                //// Check the current session for the profile Id
                loValue = this.GetValue(MaxEnumGroup.ScopeSession, "MaxProfileIdCurrent");
                if (null != loValue)
                {
                    loR = MaxConvertLibrary.ConvertToGuid(typeof(object), loValue);
                }
            }

            if (Guid.Empty.Equals(loR))
            {
                //// Check the application 
                object loProfileId = this.GetValue(MaxEnumGroup.ScopePersistent, "MaxProfileId");
                if (null != loProfileId)
                {
                    loR = MaxConvertLibrary.ConvertToGuid(typeof(object), loProfileId);
                }

                if (Guid.Empty.Equals(loR))
                {
                    loR = Guid.NewGuid();
                    this.SetValue(MaxEnumGroup.ScopePersistent, "MaxProfileId", loR);
                }
            }

            this.SetValue(MaxEnumGroup.ScopeProcess, "MaxProfileIdCurrent", loR);
            return loR;
        }

        protected virtual string GetSystemValue(string lsTable, string lsField, bool lbAll)
        {
            return this.GetSystemValueConditional(lsTable, lsField, lbAll);
        }

#if net2 || netstandard1_2

        protected virtual string GetSystemValueConditional(string lsTable, string lsField, bool lbAll)
        {
            string lsR = string.Empty;
            MaxIndex loList = new MaxIndex();
            if (lsTable == "NetworkInterface" && lsField == "PhysicalAddress")
            {
                NetworkInterface[] loNetList = NetworkInterface.GetAllNetworkInterfaces();
                string lsNetworkId = string.Empty;
                foreach (NetworkInterface loInterface in loNetList)
                {
                    if (loInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                        loInterface.NetworkInterfaceType == NetworkInterfaceType.Unknown)
                    {
                        loList.Add(loInterface.GetPhysicalAddress().ToString(), true);
                    }
                }
            }
            else
            {
                if (!this._oManagementObjectIndex.Contains(lsTable + "|" + lsField))
                {
                    System.Management.ManagementObjectSearcher loSearcher = new System.Management.ManagementObjectSearcher("Select * from Win32_" + lsTable);
                    System.Management.ManagementObjectCollection loSearchResultList = loSearcher.Get();
                    foreach (System.Management.ManagementObject loObject in loSearchResultList)
                    {
                        foreach (System.Management.PropertyData loProperty in loObject.Properties)
                        {
                            if (null != loProperty.Value)
                            {
                                MaxIndex loPropertyIndex = new MaxIndex();
                                if (this._oManagementObjectIndex.Contains(lsTable + "|" + loProperty.Name))
                                {
                                    loPropertyIndex = this._oManagementObjectIndex[lsTable + "|" + loProperty.Name] as MaxIndex;
                                }

                                loPropertyIndex.Add(loProperty.Value, true);
                                this._oManagementObjectIndex.Add(lsTable + "|" + loProperty.Name, loPropertyIndex);
                            }
                        }
                    }
                }

                if (this._oManagementObjectIndex.Contains(lsTable + "|" + lsField))
                {
                    loList = this._oManagementObjectIndex[lsTable + "|" + lsField] as MaxIndex;
                }
            }

            string[] laList = loList.GetSortedKeyList();
            if (lbAll)
            {
                lsR = string.Empty;
                foreach (string lsKey in laList)
                {
                    lsR += lsKey + "|";
                }
            }
            else if (laList.Length > 0)
            {
                lsR = laList[0];
            }

            return lsR;
        }
#else
        protected virtual string GetSystemValueConditional(string lsTable, string lsField, bool lbAll)
        {
            string lsR = string.Empty;

            return lsR;
        }
#endif
    }
}