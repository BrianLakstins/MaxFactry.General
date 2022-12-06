// <copyright file="MaxEnableCorsAttributeWebApi.cs" company="Lakstins Family, LLC">
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
// <change date="12/20/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="9/7/2020" author="Brian A. Lakstins" description="Add comment about OptionsVerbHandler because it's really important!">
// <change date="12/19/2020" author="Brian A. Lakstins" description="Add PUT because used to create new items in REST api">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Filters;

    /// <summary>
    /// Attribute that enables CORS for the request
    /// Make sure to add <remove name="OPTIONSVerbHandler" /> to <handlers> section in web.config or this will not work!
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class MaxEnableCorsAttributeWebApi : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the MaxEnableCorsAttribute class.
        /// </summary>
        public MaxEnableCorsAttributeWebApi()
            : base()
        {
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response == null)
            {
                actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.NoContent);
            }

            /// sent in Origin header
            /// 
            string lsOrigin = "*";
            IEnumerable<string> loValueList = new List<string>();
            if (actionExecutedContext.Request.Headers.TryGetValues("Origin", out loValueList))
            {
                foreach (string lsOriginFromServer in loValueList)
                {
                    if (!string.IsNullOrEmpty(lsOriginFromServer))
                    {
                        lsOrigin = lsOriginFromServer;
                    }
                }
            }

            actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", lsOrigin);
            actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Headers", "origin, content-type, accept, authorization");
            actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            actionExecutedContext.Response.Headers.Add("Access-Control-Max-Age", "600");
            base.OnActionExecuted(actionExecutedContext);
        }

    }
}