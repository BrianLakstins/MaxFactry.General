// <copyright file="MaxStartup.cs" company="Lakstins Family, LLC">
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
// <change date="6/7/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="5/19/2020" author="Brian A. Lakstins" description="Add logging of external Http requests">
// <change date="6/5/2020" author="Brian A. Lakstins" description="Remove setting configuration for providers because it's set globally for all.">
// <change date="7/25/2023" author="Brian A. Lakstins" description="Change order of methods so they match when they are run.">
// </changelog>
#endregion

namespace MaxFactry.General
{
    using System;
    using MaxFactry.Core;

    public class MaxStartup : MaxFactry.Base.MaxStartup
    {
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static object _oInstance = null;

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public new static MaxStartup Instance
        {
            get
            {
                _oInstance = CreateInstance(typeof(MaxStartup), _oInstance);
                return _oInstance as MaxStartup;
            }
        }

        public override void SetProviderConfiguration(MaxFactry.Core.MaxIndex loConfig)
        {
        }

        public override void RegisterProviders()
        {
            //// Configure provider for MaxFactryLibrary
            MaxSettingsStructure loSettingMaxFactry = new MaxSettingsStructure(
                typeof(MaxFactry.Core.Provider.MaxFactryLibraryDefaultProvider).Name,
                typeof(MaxFactry.Core.Provider.MaxFactryLibraryDefaultProvider));
            MaxFactryLibrary.SetSetting(typeof(MaxFactry.Core.Provider.MaxFactryLibraryDefaultProvider).ToString(), loSettingMaxFactry);
            //// Set provider for MaxConfigurationLibrary
            MaxFactry.Core.MaxConfigurationLibrary.Instance.ProviderSet(typeof(MaxFactry.Core.Provider.MaxConfigurationLibraryGeneralProvider));
        }
    
        public override void ApplicationStartup()
        {
#if net4_52
            new MaxHttpEventListener();
#endif
        }
    }
}