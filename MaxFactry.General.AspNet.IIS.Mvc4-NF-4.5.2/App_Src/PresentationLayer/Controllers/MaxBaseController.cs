﻿// <copyright file="MaxController.cs" company="Lakstins Family, LLC">
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
// <change date="6/19/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="6/23/2014" author="Brian A. Lakstins" description="Added common handling for ViewModels based on MaxIdEntity.">
// <change date="6/24/2014" author="Brian A. Lakstins" description="Refactor and document.">
// <change date="6/28/2014" author="Brian A. Lakstins" description="Change to use BaseId.">
// <change date="2/15/2021" author="Brian A. Lakstins" description="Remove unused methods">
// <change date="6/17/2025" author="Brian A. Lakstins" description="Update logging.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;
    using MaxFactry.Core;
    using MaxFactry.General.AspNet.PresentationLayer;

    /// <summary>
    /// Base class for any controller.
    /// </summary>
    public abstract class MaxBaseController : Controller
    {
        public MaxBaseController()
        {
            MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "MaxBaseController", MaxEnumGroup.LogInfo, "Created"));
        }

        /// <summary>
        /// Gets a value indicating whether the current connection is secure.
        /// </summary>
        public bool IsSecureConnection
        {
            get
            {
                return MaxOwinLibrary.IsSecureConnection();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current connection is secure and the user is authenticated.
        /// </summary>
        public bool IsSecureLogin
        {
            get
            {
                if (MaxOwinLibrary.IsSecureConnection() &&
                    this.Request.IsAuthenticated)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
