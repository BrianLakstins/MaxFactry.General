// <copyright file="MaxRequireHttpsAttribute.cs" company="Lakstins Family, LLC">
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
// <change date="6/2/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="12/18/2014" author="Brian A. Lakstins" description="Try to fix error when accessing logon page.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using MaxFactry.General.AspNet.PresentationLayer;

    /// <summary>
    /// Attribute that forces the controller method to be called with SSL.
    /// </summary>
    public class MaxRequireHttpsAttribute : RequireHttpsAttribute
    {
        /// <summary>
        /// Initializes a new instance of the MaxRequireHttpsAttribute class.
        /// </summary>
        public MaxRequireHttpsAttribute()
            : base()
        {
        }

        /// <summary>
        /// Handles non-https requests and forces them to be SSL if they are not secure.
        /// </summary>
        /// <param name="loFilterContext">The current context.</param>
        protected override void HandleNonHttpsRequest(AuthorizationContext loFilterContext)
        {
            if (!MaxOwinLibrary.IsSecureConnection())
            {
                loFilterContext.Result = new RedirectResult(MaxOwinLibrary.GetSecureUrl().ToString());

                //loFilterContext.HttpContext.Response.Redirect(MaxRequestLibrary.GetSecureUrl(loFilterContext.HttpContext.Request).ToString());
                //loFilterContext.HttpContext.Response.End();
            }
        }
    }
}