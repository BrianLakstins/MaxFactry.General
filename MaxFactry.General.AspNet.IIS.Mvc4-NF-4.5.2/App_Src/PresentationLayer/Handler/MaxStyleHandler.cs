// <copyright file="MaxCssHandler.cs" company="Lakstins Family, LLC">
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
// <change date="7/14/2016" author="Brian A. Lakstins" description="Initial creation">
// <change date="5/27/2019" author="Brian A. Lakstins" description="Add some checking to make sure client still connected.">
// <change date="9/15/2019" author="Brian A. Lakstins" description="Handle static files.">
// <change date="6/16/2025" author="Brian A. Lakstins" description="Add using a cache">
// <change date="6/17/2025" author="Brian A. Lakstins" description="Clean up usings">
// <change date="6/20/2025" author="Brian A. Lakstins" description="Update functionality to work with just name and cache output for 10 minutes">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{

    using System;
    using System.Web;
    using System.Web.Routing;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.Core;
    using MaxFactry.General.AspNet.PresentationLayer;

    public class MaxStyleHandler : IRouteHandler, IHttpHandler
    {
        private string _sFileName = string.Empty;

        public MaxStyleHandler()
        {
        }

        public MaxStyleHandler(object loFileName)
        {
            this._sFileName = MaxConvertLibrary.ConvertToString(typeof(object), loFileName);
            if (!string.IsNullOrEmpty(this._sFileName) && !this._sFileName.EndsWith(".css", System.StringComparison.CurrentCultureIgnoreCase))
            {
                this._sFileName += ".css";
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext loRequestContext)
        {
            object loFileName = loRequestContext.RouteData.Values["filename"];
            return new MaxStyleHandler(loFileName);
        }

        // Summary:
        //     Gets a value indicating whether another request can use the System.Web.IHttpHandler
        //     instance.
        //
        // Returns:
        //     true if the System.Web.IHttpHandler instance is reusable; otherwise, false.
        public bool IsReusable { get { return false; } }

        // Summary:
        //     Enables processing of HTTP Web requests by a custom HttpHandler that implements
        //     the System.Web.IHttpHandler interface.
        //
        // Parameters:
        //   context:
        //     An System.Web.HttpContext object that provides references to the intrinsic
        //     server objects (for example, Request, Response, Session, and Server) used
        //     to service HTTP requests.
        public void ProcessRequest(HttpContext loContext)
        {
            string lsFilePath = string.Empty;
            if (null != loContext && null != loContext.Request && null != loContext.Request.AppRelativeCurrentExecutionFilePath)
            {
                lsFilePath = loContext.Request.AppRelativeCurrentExecutionFilePath;
            }

            if (!string.IsNullOrEmpty(lsFilePath))
            {
                string lsCacheKey = this.GetType() + "/" + MaxDataLibrary.GetApplicationKey() + "/" + lsFilePath;
                string lsContent = MaxCacheRepository.Get(typeof(object), lsCacheKey, typeof(string)) as string;
                if (null == lsContent)
                {
                    lsContent = string.Empty;
                    string lsFile = loContext.Server.MapPath(lsFilePath);
                    if (System.IO.File.Exists(lsFile))
                    {
                        lsContent = System.IO.File.ReadAllText(lsFile);
                    }
                    else
                    {
                        MaxStyleFileViewModel loModel = new MaxStyleFileViewModel(this._sFileName);
                        if (!string.IsNullOrEmpty(loModel.Content) && loContext.Response.IsClientConnected)
                        {
                            lsContent = loModel.Content;
                        }
                    }

                    MaxCacheRepository.Set(typeof(object), lsCacheKey, lsContent, DateTime.UtcNow.AddMinutes(10));
                }
                
                if (!string.IsNullOrEmpty(lsContent) && loContext.Response.IsClientConnected)
                {                    
                    byte[] laContent = System.Text.UTF8Encoding.UTF8.GetBytes(lsContent);
                    loContext.Response.Clear();
                    loContext.Response.ContentType = "text/css";
                    loContext.Response.OutputStream.Write(laContent, 0, laContent.Length);
                    loContext.Response.Flush();
                }
                else
                {
                    loContext.Response.StatusCode = 404;
                }
            }

            loContext.ApplicationInstance.CompleteRequest();
        }
    }
}
