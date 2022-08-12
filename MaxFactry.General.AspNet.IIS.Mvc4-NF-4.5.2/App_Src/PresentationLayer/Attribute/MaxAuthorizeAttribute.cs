// <copyright file="MaxAuthorizeAttribute.cs" company="Lakstins Family, LLC">
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
// <change date="6/27/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="4/10/2020" author="Brian A. Lakstins" description="Change to use development environment for bypass authorization">
// <change date="11/3/2020" author="Brian A. Lakstins" description="Change to not bypass authorization">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Attribute to make sure that request is authorized.
    /// </summary>
    public class MaxAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the MaxAuthorizeAttribute class.
        /// </summary>
        public MaxAuthorizeAttribute()
            : base()
        {
        }

        /// <summary>
        /// Checks to the request to see if it is authorized.
        /// </summary>
        /// <param name="httpContext">The current context.</param>
        /// <returns>True if authorized. False otherwise.</returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return base.AuthorizeCore(httpContext);
        }
    }
}