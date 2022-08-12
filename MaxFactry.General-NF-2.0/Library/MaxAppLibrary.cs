// <copyright file="MaxApplicationLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="6/21/2015" author="Brian A. Lakstins" description="Initial Release">
// <change date="7/22/2016" author="Brian A. Lakstins" description="Added initialization of encryption as public method.">
// <change date="2/4/2020" author="Brian A. Lakstins" description="Add method to handle shutdown of app.">
// <change date="2/6/2020" author="Brian A. Lakstins" description="Adding properties and methods for system information">
// <change date="5/6/2020" author="Brian A. Lakstins" description="Updating handing of application start time">
// <change date="6/5/2020" author="Brian A. Lakstins" description="Change order of application start to be able to pass global configuration.">
// </changelog>
#endregion

namespace MaxFactry.General
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.Base.DataLayer;

    /// <summary>
    /// Provider for conversion specifically for dates.
    /// </summary>
    public class MaxAppLibrary : MaxSingleFactory
    {
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxAppLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Gets the Provider used for most MaxFactory methods
        /// </summary>
        public static IMaxAppLibraryProvider Provider
        {
            get
            {
                if (null == Instance.BaseProvider)
                {
                    Instance.SetProvider(typeof(MaxFactry.General.Provider.MaxAppLibraryDefaultProvider));
                    if (!(Instance.BaseProvider is IMaxAppLibraryProvider))
                    {
                        throw new MaxException("Provider for MaxApplicationLibrary does not implement IMaxApplicationLibraryProvider.");
                    }
                }

                return (IMaxAppLibraryProvider)Instance.BaseProvider;
            }
        }

        /// <summary>
        /// Gets the name of the product
        /// </summary>
        public static string Product
        {
            get
            {
                return Provider.Product;
            }
        }

        /// <summary>
        /// Gets the version of the product
        /// </summary>
        public static string ProductVersion
        {
            get
            {
                return Provider.ProductVersion;
            }
        }

        /// <summary>
        /// Gets the file version of the product
        /// </summary>
        public static string ProductFileVersion
        {
            get
            {
                return Provider.ProductFileVersion;
            }
        }

        /// <summary>
        /// Gets the information about the system that the code is running on
        /// </summary>
        public static MaxIndex SystemInfoIndex
        {
            get
            {
                return Provider.SystemInfoIndex;
            }
        }

        /// <summary>
        /// Gets text indicating which environment is being used.
        /// </summary>
        public static string EnvironmentName
        {
            get
            {
                return Provider.EnvironmentName;
            }
        }

        /// <summary>
        /// Gets variable indicating the current process is the application startup process
        /// </summary>
        public static bool IsApplicationStartup
        {
            get
            {
                return Provider.IsApplicationStartup;
            }
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        protected static MaxAppLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxAppLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }

        public static void Start(Type loApplicationLibraryProviderType, MaxIndex loConfig)
        {
            MaxAppLibrary.Instance.ProviderSet(loApplicationLibraryProviderType);
            MaxAppLibrary.SetProviderConfiguration(loConfig);
            MaxAppLibrary.RegisterProviders();
            MaxAppLibrary.ApplicationStartup();
        }

        public static double GetTimeSinceApplicationStart()
        {
            return Provider.GetTimeSinceApplicationStart();
        }

        public static DateTime GetApplicationStartDateTime()
        {
            return Provider.GetApplicationStartDateTime();
        }

        public static Guid GetApplicationRunId()
        {
            return Provider.GetApplicationRunId();
        }

        public static void AddValidStorageKey(string lsStorageKey)
        {
            Provider.AddValidStorageKey(lsStorageKey);
        }

        public static bool IsValidStorageKey(string lsStorageKey)
        {
            return Provider.IsValidStorageKey(lsStorageKey);
        }

        public static void ClearValidStorageKey()
        {
            Provider.ClearValidStorageKey();
        }

        /// <summary>
        /// To be run first, before anything else in the application.
        /// </summary>
        private static void RegisterProviders()
        {
            Provider.RegisterProviders();
        }

        /// <summary>
        /// Sets the global configuration for all providers.  To be run before registering any providers.
        /// </summary>
        /// <param name="loConfig">The configuration for the default repository provider.</param>
        private static void SetProviderConfiguration(MaxIndex loConfig)
        {
            Provider.SetProviderConfiguration(loConfig);
        }

        /// <summary>
        /// To be run after providers have been configured.
        /// </summary>
        private static void ApplicationStartup()
        {
            Provider.ApplicationStartup();
        }

        /// <summary>
        /// To be run when application stops
        /// </summary>
        public static void ApplicationShutdown()
        {
            Provider.ApplicationShutdown();
        }
    }
}