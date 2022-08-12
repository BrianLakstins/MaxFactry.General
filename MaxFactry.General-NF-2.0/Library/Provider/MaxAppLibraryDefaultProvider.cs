﻿// <copyright file="MaxApplicationLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// <change date="6/21/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="7/4/2016" author="Brian A. Lakstins" description="Updated to access provider configuration using base provider methods.">
// <change date="7/18/2019" author="Brian A. Lakstins" description="Move lazy loading of environment and log level to core.">
// <change date="2/4/2020" author="Brian A. Lakstins" description="Add creating temp folder and cleaning it up at shutdown.">
// <change date="2/6/2020" author="Brian A. Lakstins" description="Adding properties and methods for system information">
// <change date="5/6/2020" author="Brian A. Lakstins" description="Updating handing of application start time">
// <change date="6/5/2020" author="Brian A. Lakstins" description="Updated initialization so does not use global config. Only uses passed config.  Trying to use global config can cause objects to get created before they can be configured.  Move Reset of MaxFactryLibrary to SetProviderConfiguration.">
// </changelog>
#endregion

namespace MaxFactry.General.Provider
{
	using System;
    using System.Diagnostics;
    using MaxFactry.Core;

    /// <summary>
    /// Default provider for MaxApplicationLibrary
    /// </summary>
	public class MaxAppLibraryDefaultProvider : MaxProvider, IMaxAppLibraryProvider
	{
        /// <summary>
        /// Object used to lock threads
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Stores information from the assembly about the product
        /// </summary>
        private string _sProduct = null;

        /// <summary>
        /// Stores information from the assembly about the product version
        /// </summary>
        private string _sProductVersion = null;

        /// <summary>
        /// Stores information from the assembly about the product file version
        /// </summary>
        private string _sProductFileVersion = null;

        /// <summary>
        /// Stores valid storage keys for the application
        /// </summary>
        private MaxIndex _oValidStorageKeyIndex = new MaxIndex();

        /// <summary>
        /// Stores the system information
        /// </summary>
        private MaxIndex _oSystemInfoIndex = null;

        private long _nStartTicks = long.MinValue;

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="lsName">Name of the provider.</param>
        /// <param name="loConfig">Configuration information.</param>
        public override void Initialize(string lsName, MaxIndex loConfig)
        {
            this.Name = lsName;
            if (null == this.Name || this.Name.Length == 0)
            {
                this.Name = "Default";
            }

            if (null != loConfig)
            {
                string lsConfigName = loConfig["MaxProviderName"] as string;
                if (null != lsConfigName)
                {
                    this.Name = lsConfigName;
                }
            }
        }

        /// <summary>
        /// Gets AssemblyProduct
        /// </summary>
        public virtual string Product
        {
            get
            {
                if (null == this._sProduct)
                {
                    lock (_oLock)
                    {
                        if (null == this._sProduct)
                        {
                            this._sProduct = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyProduct").ToString();
                        }
                    }
                }

                return this._sProduct;
            }
        }

        /// <summary>
        /// Gets AssemblyVersion
        /// </summary>
        public virtual string ProductVersion
        {
            get
            {
                if (null == this._sProductVersion)
                {
                    lock (_oLock)
                    {
                        if (null == this._sProductVersion)
                        {
                            this._sProductVersion = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyVersion").ToString();
                        }
                    }
                }

                return this._sProductVersion;
            }
        }

        /// <summary>
        /// Gets AssemblyFileVersion
        /// </summary>
        public virtual string ProductFileVersion
        {
            get
            {
                if (null == this._sProductFileVersion)
                {
                    lock (_oLock)
                    {
                        if (null == this._sProductFileVersion)
                        {
                            this._sProductFileVersion = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyFileVersion").ToString();
                        }
                    }
                }

                return this._sProductFileVersion;
            }
        }

        /// <summary>
        /// Gets the information about the system that the code is running on
        /// </summary>
        public virtual MaxIndex SystemInfoIndex
        {
            get
            {
                if (null == this._oSystemInfoIndex)
                {
                    lock (_oLock)
                    {
                        if (null == this._oSystemInfoIndex)
                        {
                            this._oSystemInfoIndex = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxSystem") as MaxIndex;
                        }
                    }
                }

                return this._oSystemInfoIndex;
            }
        }

        /// <summary>
        /// Gets text indicating which environment is being used.
        /// </summary>
        public virtual string EnvironmentName
        {
            get
            {
                if (MaxFactryLibrary.Environment == MaxEnumGroup.EnvironmentProduction)
                {
                    return "Production";
                }
                else if (MaxFactryLibrary.Environment == MaxEnumGroup.EnvironmentQA)
                {
                    return "QA";
                }
                else if (MaxFactryLibrary.Environment == MaxEnumGroup.EnvironmentTesting)
                {
                    return "Testing";
                }
                else if (MaxFactryLibrary.Environment == MaxEnumGroup.EnvironmentDevelopment)
                {
                    return "Development";
                }

                return "Unknown";
            }
        }

        /// <summary>
        /// Gets variable indicating the current process is the application startup process
        /// </summary>
        public virtual bool IsApplicationStartup
        {
            get
            {
                object loObject = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, "__MaxIsApplicationStartup");
                if (null != loObject && loObject is bool)
                {
                    return (bool)loObject;
                }

                return false;
            }
        }

        public virtual double GetTimeSinceApplicationStart()
        {
            if (this._nStartTicks == long.MinValue)
            {
                this._nStartTicks = this.GetApplicationStartDateTime().Ticks;
            }

            double lnR = new TimeSpan(DateTime.UtcNow.Ticks - this._nStartTicks).TotalMilliseconds;
            return lnR;
        }

        public virtual DateTime GetApplicationStartDateTime()
        {
            DateTime ldR = DateTime.UtcNow;
            object loObject = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "__MaxApplicationStartDateTime");
            if (loObject is DateTime)
            {
                ldR = (DateTime)loObject;
            }
            else
            {
                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeApplication, "__MaxApplicationStartDateTime", ldR);
            }

            return ldR;
        }

        public virtual Guid GetApplicationRunId()
        {
            Guid loR = Guid.Empty;
            object loObject = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "__MaxApplicationRunId");
            if (null == loObject)
            {
                loObject = Guid.NewGuid();
                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeApplication, "__MaxApplicationRunId", loObject);
            }

            loR = ((Guid)loObject);
            return loR;
        }

        public virtual void AddValidStorageKey(string lsStorageKey)
        {
            if (!this._oValidStorageKeyIndex.Contains(lsStorageKey.ToLower()))
            {
                this._oValidStorageKeyIndex.Add(lsStorageKey.ToLower(), true);
            }
        }

        public virtual bool IsValidStorageKey(string lsStorageKey)
        {
            return this._oValidStorageKeyIndex.Contains(lsStorageKey.ToLower());
        }

        public virtual void ClearValidStorageKey()
        {
            this._oValidStorageKeyIndex = new MaxIndex();
        }

        /// <summary>
        /// To be run first, before anything else in the application.
        /// </summary>
        public virtual void RegisterProviders()
        {
            //// Configure provider for MaxFactryLibrary
            MaxSettingsStructure loSettingMaxFactry = new MaxSettingsStructure(
                "MaxFactryLibraryProvider",
                typeof(MaxFactry.Core.Provider.MaxFactryLibraryDefaultProvider));
            MaxFactryLibrary.SetSetting(typeof(MaxFactry.Core.Provider.MaxFactryLibraryDefaultProvider).ToString(), loSettingMaxFactry);

            //// Register providers for base and general
            MaxFactry.Base.MaxStartup.Instance.RegisterProviders();
            MaxFactry.General.MaxStartup.Instance.RegisterProviders();
        }

        /// <summary>
        /// To be run after providers have been registered
        /// </summary>
        /// <param name="loConfig">The configuration for the default repository provider.</param>
        public virtual void SetProviderConfiguration(MaxIndex loConfig)
        {
            //// Reset the configuration before applying the new one
            MaxFactryLibrary.Reset();
            MaxFactry.Base.MaxStartup.Instance.SetProviderConfiguration(loConfig);
            MaxFactry.General.MaxStartup.Instance.SetProviderConfiguration(loConfig);
        }

        /// <summary>
        /// To be run after providers have been configured.
        /// </summary>
        public virtual void ApplicationStartup()
        {
            //// Set a variable to indicate that this is the loading process of the application;
            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "__MaxIsApplicationStartup", true);

            //// Set application objects for diagnostics
            DateTime ldStart = this.GetApplicationStartDateTime();

            MaxFactryLibrary.SetValue("CurrentAssemblyName",  MaxFactry.Core.MaxFactryLibrary.GetAssembly(this.GetType()).ManifestModule.Name);

            string lsTempFolder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            if (!System.IO.Directory.Exists(lsTempFolder))
            {
                System.IO.Directory.CreateDirectory(lsTempFolder);
            }

            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeApplication, "__TempFolder", lsTempFolder); 

            MaxFactry.Base.MaxStartup.Instance.ApplicationStartup();
            MaxFactry.General.MaxStartup.Instance.ApplicationStartup();
        }

        public virtual void ApplicationShutdown()
        {
            string lsTempFolder = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "__TempFolder") as string;
            if (null != lsTempFolder && lsTempFolder.Length > 0)
            {
                if (System.IO.Directory.Exists(lsTempFolder))
                {
                    System.IO.Directory.Delete(lsTempFolder, true);
                }
            }
        }
    }
}