// <copyright file="MaxEnableCorsAttribute.cs" company="Lakstins Family, LLC">
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
// <change date="3/17/2015" author="Brian A. Lakstins" description="Initial Release">
// <change date="11/27/2020" author="Brian A. Lakstins" description="Add more http methods by default">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Attribute that enables CORS for the request
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class MaxEnableCorsAttribute : ActionFilterAttribute, IResultFilter
    {
        /// <summary>
        /// Initializes a new instance of the MaxEnableCorsAttribute class.
        /// </summary>
        public MaxEnableCorsAttribute()
            : base()
        {
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            string lsOrigin = "*";
            List<string> loKeyList = new List<string>(filterContext.HttpContext.Request.Headers.AllKeys);
            if (loKeyList.Contains("Origin"))
            {
                lsOrigin = filterContext.HttpContext.Request.Headers["Origin"];
            }

            filterContext.HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", lsOrigin);
            filterContext.HttpContext.Response.AppendHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            filterContext.HttpContext.Response.AppendHeader("Access-Control-Allow-Headers", "origin, content-type, accept");
            filterContext.HttpContext.Response.AppendHeader("Access-Control-Allow-Credentials", "true");
            filterContext.HttpContext.Response.AppendHeader("Access-Control-Max-Age", "600");
            base.OnResultExecuting(filterContext);
        }
    }
}