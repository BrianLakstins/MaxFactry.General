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
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{

    using System;
    using System.Web;
    using System.Web.Routing;
    using MaxFactry.Core;
    using MaxFactry.General.AspNet.PresentationLayer;

    public class MaxStyleHandler : IRouteHandler, IHttpHandler
    {
        private string _sId = string.Empty;

        private string _sFileName = string.Empty;

        public MaxStyleHandler()
        {

        }

        public MaxStyleHandler(object loId, object loFileName)
        {
            if (null != loId && loId is string)
            {
                this._sId = (string)loId;
            }
            else if (null != loId)
            {
                this._sId = loId.ToString();
            }

            if (null != loFileName && loFileName is string)
            {
                this._sFileName = (string)loFileName;
            }
            else if (null != loFileName)
            {
                this._sFileName = loFileName.ToString();
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext loRequestContext)
        {
            object loId = loRequestContext.HttpContext.Request.QueryString["id"];
            object loFileName = loRequestContext.RouteData.Values["filename"];
            return new MaxStyleHandler(loId, loFileName);
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
            string lsFile = loContext.Server.MapPath(loContext.Request.AppRelativeCurrentExecutionFilePath);
            if (System.IO.File.Exists(lsFile))
            {
                loContext.Response.Clear();
                loContext.Response.ContentType = "text/css";
                byte[] laContent = System.IO.File.ReadAllBytes(lsFile);
                loContext.Response.OutputStream.Write(laContent, 0, laContent.Length);
                loContext.Response.Flush();
            }
            else
            {
                MaxStyleFileViewModel loModel = new MaxStyleFileViewModel();
                if (!string.IsNullOrEmpty(this._sId))
                {
                    loModel = new MaxStyleFileViewModel(this._sId);
                }
                else
                {
                    loModel.LoadFromName(this._sFileName);
                }

                if (!string.IsNullOrEmpty(loModel.Content) && loContext.Response.IsClientConnected)
                {
                    loContext.Response.Clear();
                    loContext.Response.ContentType = "text/css";
                    byte[] laContent = System.Text.UTF8Encoding.UTF8.GetBytes(loModel.Content);
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
