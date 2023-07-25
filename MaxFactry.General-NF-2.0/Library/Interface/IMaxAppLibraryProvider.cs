// <copyright file="IMaxHttpApplicationLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="7/22/2016" author="Brian A. Lakstins" description="Added initialization of encryption as public method.">
// <change date="2/4/2020" author="Brian A. Lakstins" description="Add method to handle shutdown of app.">
// <change date="2/6/2020" author="Brian A. Lakstins" description="Adding properties and methods for system information">
// <change date="5/6/2020" author="Brian A. Lakstins" description="Updating handing of application start time">
// <change date="7/25/2023" author="Brian A. Lakstins" description="Update order of methods. Add GetTempFolder.  Add GetConfig method.">
// </changelog>
#endregion

namespace MaxFactry.General
{
	using System;
    using MaxFactry.Core;

    /// <summary>
    /// Inteface for MaxApplicationLibrary
    /// </summary>
	public interface IMaxAppLibraryProvider
    {
        /// <summary>
        /// Gets the time passed in milliseconds since the application started
        /// </summary>
        /// <returns></returns>
        double GetTimeSinceApplicationStart();

        /// <summary>
        /// Gets the time the application started
        /// </summary>
        /// <returns></returns>
        DateTime GetApplicationStartDateTime();

        /// <summary>
        /// Gets an ID unique to this instance of running this application
        /// </summary>
        /// <returns></returns>
        Guid GetApplicationRunId();

        /// <summary>
        /// Get the current instance of the application temporary folder
        /// </summary>
        string GetTempFolder();

        /// <summary>
        /// Gets the configuration to use to initialize providers
        /// </summary>
        /// <returns></returns>
        MaxIndex GetConfig();

        void AddValidStorageKey(string lsStorageKey);

        bool IsValidStorageKey(string lsStorageKey);

        void ClearValidStorageKey();

        /// <summary>
        /// To be run first to set up configuration for providers.
        /// </summary>
        /// <param name="loConfig">The configuration for the default repository provider.</param>
        void SetProviderConfiguration(MaxIndex loConfig);
        
        /// <summary>
        /// To be run after config has been set.
        /// </summary>
        void RegisterProviders();

        /// <summary>
        /// To be run after providers have been configured.
        /// </summary>
        void ApplicationStartup();

        /// <summary>
        /// To be run when application stops
        /// </summary>
        void ApplicationShutdown();

        /// <summary>
        /// Gets AssemblyProduct
        /// </summary>
        string Product { get; }

        /// <summary>
        /// Gets AssemblyVersion
        /// </summary>
        string ProductVersion { get; }

        /// <summary>
        /// Gets AssemblyFileVersion
        /// </summary>
        string ProductFileVersion { get; }

        /// <summary>
        /// Gets the information about the system that the code is running on
        /// </summary>
        MaxIndex SystemInfoIndex { get; }

        /// <summary>
        /// Gets text indicating which environment is being used.
        /// </summary>
        string EnvironmentName { get; }

        /// <summary>
        /// Gets variable indicating the current process is the application startup process
        /// </summary>
        bool IsApplicationStartup { get; }
    }
}