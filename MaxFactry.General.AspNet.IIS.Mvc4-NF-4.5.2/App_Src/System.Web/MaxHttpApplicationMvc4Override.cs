// <copyright file="MaxHttpApplication.cs" company="Lakstins Family, LLC">
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
// <change date="8/25/2014" author="Brian A. Lakstins" description="Initial creation">
// <change date="6/1/2018" author="Brian A. Lakstins" description="Redirect Anti Forgery errors on the log in page.">
// </changelog>
#endregion

namespace System.Web
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using MaxFactry.Core;

    /// <summary>
    /// Base application for Mvc4 based web applications.
    /// </summary>
    public class MaxHttpApplicationMvc4Override : System.Web.MaxHttpApplicationOverride
    {
        protected override void Application_Error_Handler(object sender, EventArgs e)
        {
            if (null != HttpContext.Current && null != HttpContext.Current.Error && HttpContext.Current.Error is System.Web.Mvc.HttpAntiForgeryException && HttpContext.Current.Error.Message.Contains("The provided anti-forgery token was meant for user \"\", but the current user is") && !string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ReturnUrl"]))
            {
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.QueryString["ReturnUrl"]);
            }
            else
            {
                base.Application_Error_Handler(sender, e);
            }
        }
    }
}