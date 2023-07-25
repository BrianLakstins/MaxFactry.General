// <copyright file="MaxApplicationLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// <change date="7/25/2023" author="Brian A. Lakstins" description="Move code to MaxStartup">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.Provider
{
	using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web.Mvc;
    using System.Web.Routing;
    using MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer;
    using MaxFactry.Core;

    /// <summary>
    /// Default provider for MaxApplicationLibrary
    /// </summary>
    public class MaxAppLibraryDefaultProvider : MaxFactry.General.AspNet.IIS.Provider.MaxAppLibraryDefaultProvider, IMaxAppLibraryProvider
	{
        public override void SetProviderConfiguration(MaxIndex loConfig)
        {
            base.SetProviderConfiguration(loConfig);
            MaxFactry.General.AspNet.IIS.Mvc4.MaxStartup.Instance.SetProviderConfiguration(loConfig);
        }

        public override void RegisterProviders()
        {
            base.RegisterProviders();
            MaxFactry.General.AspNet.IIS.Mvc4.MaxStartup.Instance.RegisterProviders();
        }

        public override void ApplicationStartup()
        {
            base.ApplicationStartup();
            MaxFactry.General.AspNet.IIS.Mvc4.MaxStartup.Instance.ApplicationStartup();
        }
    }
}