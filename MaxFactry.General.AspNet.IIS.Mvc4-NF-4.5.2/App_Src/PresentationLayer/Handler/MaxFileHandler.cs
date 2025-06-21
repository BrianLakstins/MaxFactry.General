// <copyright file="MaxFileHandler.cs" company="Lakstins Family, LLC">
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
// <change date="7/24/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="7/26/2014" author="Brian A. Lakstins" description="Update to set the storage key.">
// <change date="8/13/2014" author="Brian A. Lakstins" description="Updated to return handler to avoid thread abort errors.">
// <change date="10/28/2016" author="Brian A. Lakstins" description="Updated ignore client errors when sending a file.">
// <change date="4/29/2019" author="Brian A. Lakstins" description="Fix issue with errors when file does not exist.">
// <change date="5/27/2019" author="Brian A. Lakstins" description="Add some checking to make sure client still connected.">
// <change date="6/17/2025" author="Brian A. Lakstins" description="Update logging.  Make sure client is connecte before sending response">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{

    using System;
    using System.Web;
    using System.Web.Routing;
    using MaxFactry.Core;
    using MaxFactry.General.AspNet.PresentationLayer;

    public class MaxFileHandler : IRouteHandler, IHttpHandler
    {
        private string _sId = string.Empty;

        private string _sFileName = string.Empty;

        public MaxFileHandler()
        {
        }

        public MaxFileHandler(object loId, object loFileName)
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
            object loId = loRequestContext.RouteData.Values["MaxFileId"];
            object loFileName = loRequestContext.RouteData.Values["filename"];
            return new MaxFileHandler(loId, loFileName);
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
            if (!string.IsNullOrEmpty(this._sId))
            {
                MaxFileUploadViewModel loModel = new MaxFileUploadViewModel(this._sId);
                loContext.Response.Clear();
                if (null == loModel || null == loModel.Id || Guid.Empty.Equals(new Guid(loModel.Id)))
                {
                    loContext.Response.StatusCode = 404;
                }
                else
                {
                    if (null != loModel.Content)
                    {
                        try
                        {
                            System.Net.Mime.ContentDisposition loContent = new System.Net.Mime.ContentDisposition();
                            if (!string.IsNullOrEmpty(this._sFileName))
                            {
                                loContent.FileName = this._sFileName;
                            }
                            else
                            {
                                loContent.FileName = loModel.FileName;
                                if (string.IsNullOrEmpty(loContent.FileName))
                                {
                                    loContent.FileName = loModel.Name;
                                }
                            }

                            if (loContext.Response.IsClientConnected)
                            {
                                loContent.Inline = false;
                                loContext.Response.ContentType = loModel.MimeType;
                                if (loModel.IsDownload)
                                {
                                    loContext.Response.AppendHeader("Content-Disposition", loContent.ToString());
                                }                            

                                int lnBufferSize = 1024 * 50;
                                byte[] loBuffer = new byte[lnBufferSize];
                                int lnRead = loModel.Content.Read(loBuffer, 0, lnBufferSize);
                                while (lnRead > 0 && loContext.Response.IsClientConnected)
                                {
                                    loContext.Response.OutputStream.Write(loBuffer, 0, lnRead);
                                    loContext.Response.Flush();
                                    lnRead = loModel.Content.Read(loBuffer, 0, lnBufferSize);
                                }
                            }
                        }
                        catch (Exception loERead)
                        {
                            if (!(loERead is HttpException))
                            {
                                MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "ProcessRequest", MaxEnumGroup.LogError, "Error reading and sending content.", loERead));
                            }
                        }
                        finally
                        {
                            loModel.Content.Close();
                            loModel.Content.Dispose();
                        }
                    }
                    else
                    {
                        loContext.Response.StatusCode = 404;
                    }
                }
            }

            loContext.ApplicationInstance.CompleteRequest();
        }
    }
}
