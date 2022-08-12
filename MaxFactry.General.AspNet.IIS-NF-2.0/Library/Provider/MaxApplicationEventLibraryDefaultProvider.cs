// <copyright file="MaxHttpApplicationLibraryLibraryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="5/29/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="6/23/2014" author="Brian A. Lakstins" description="Move default encryption provider from Security module into common web library.">
// <change date="6/27/2014" author="Brian A. Lakstins" description="Updates for StorageKey.">
// <change date="7/21/2014" author="Brian A. Lakstins" description="Update to no longer depend on core AppId functionality.">
// <change date="7/26/2014" author="Brian A. Lakstins" description="Add Windows Phone 8 fix.  Add default Bootstrap Navigation.">
// <change date="8/13/2014" author="Brian A. Lakstins" description="Added debug logging for all events in pipeline.">
// <change date="8/21/2014" author="Brian A. Lakstins" description="Add default file repository configuration.">
// <change date="9/23/2014" author="Brian A. Lakstins" description="Update to show error when not on Production site.">
// <change date="9/30/2014" author="Brian A. Lakstins" description="Move setting cookies to end of process after cached page is created.">
// <change date="10/1/2014" author="Brian A. Lakstins" description="Add error email notifications.">
// <change date="2/3/2015" author="Brian A. Lakstins" description="Updated email sending to use new email functionality.">
// <change date="6/10/2015" author="Brian A. Lakstins" description="Updated to no longer get storage key from process or session.  The library gets from process, and the session is not url specific.">
// <change date="10/1/2018" author="Brian A. Lakstins" description="Add current URL to arguments to get storage key. Add Redirect URL support.">
// <change date="4/28/2020" author="Brian A. Lakstins" description="Fix exception when trying to access current Request and it does not exist">
// <change date="5/22/2020" author="Brian A. Lakstins" description="Remove pipeline handling of storagekey">
// <change date="5/22/2020" author="Brian A. Lakstins" description="Update logging">
// <change date="5/31/2020" author="Brian A. Lakstins" description="Update Url processing in case it's not valid">
// </changelog>
#endregion

namespace MaxFactry.App.Base.AspNet.IIS.Provider
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Mail;
    using System.Reflection;
    using System.Web;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.App.Base;
    using MaxFactry.General.AspNet.PresentationLayer;
    using System.Security.Policy;

    /// <summary>
    /// Default provider for MaxApplicationEventLibrary
    /// </summary>
    public class MaxApplicationEventLibraryDefaultProvider : MaxProvider, IMaxApplicationEventLibraryProvider
    {

        /// <summary>
        /// 1. The BeginRequest event signals the creation of any given new request. 
        /// This event is always raised and is always the first event to occur during the processing of a request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationBeginRequest(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "1.Application_BeginRequest start", "MaxHttpApplicationLibraryProvider");
            MaxOwinLibrary.ApplicationBeginRequest();
        }

        /// <summary>
        /// 2. The AuthenticateRequest event signals that the configured authentication mechanism has authenticated the current request. 
        /// Subscribing to the AuthenticateRequest event ensures that the request will be authenticated before processing the attached 
        /// module or event handle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "2.Application_AuthenticateRequest [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 3. The PostAuthenticateRequest event is raised after the AuthenticateRequest event has occurred.
        /// All the information available is accessible in the HttpContext’s User property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationPostAuthenticateRequest(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "3.Application_PostAuthenticateRequest [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 4. The AuthorizeRequest event signals that ASP.NET has authorized the current request. 
        /// You can subscribe to the AuthorizeRequest event to perform custom authorization.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationAuthorizeRequest(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "4.Application_AuthorizeRequest [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 5. Occurs when the user for the current request has been authorized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationPostAuthorizeRequest(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "5.Application_PostAuthorizeRequest [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 6. Occurs when ASP.NET finishes an authorization event to let the caching modules serve requests from the cache, 
        /// bypassing execution of the event handler and calling any EndRequest handlers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationResolveRequestCache(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "6.Application_ResolveRequestCache [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 7. Reaching this event means the request can’t be served from the cache, and thus a HTTP handler is created here.
        /// A Page class gets created if an aspx page is requested.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationPostResolveRequestCache(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "7.Application_PostResolveRequestCache [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 8. The MapRequestHandler event is used by the ASP.NET infrastructure to determine the request handler 
        /// for the current request based on the file-name extension of the requested resource.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationMapRequestHandler(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "8.Application_MapRequestHandler [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 9. Occurs when ASP.NET has mapped the current request to the appropriate HTTP handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationPostMapRequestHandler(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "9.Application_PostMapRequestHandler [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 10. Occurs when ASP.NET acquires the current state (for example, session state) that is associated with the current request. 
        /// A valid session ID must exist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationAcquireRequestState(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "10.Application_AcquireRequestState [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 11. Occurs when the state information (for example, session state or application state) that is associated with the current request has been obtained.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationPostAcquireRequestState(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "11.Application_PostAcquireRequestState", "MaxHttpApplicationLibraryProvider");
            if (null != HttpContext.Current && null != HttpContext.Current.Request)
            {
                string lsUrl = MaxOwinLibrary.GetRedirectUrl(HttpContext.Current.Request.Url);
                if (!string.IsNullOrEmpty(lsUrl))
                {
#if net4_52
                    HttpContext.Current.Response.RedirectPermanent(lsUrl, true);
#else
                    HttpContext.Current.Response.Redirect(lsUrl, true);
#endif
                }
            }

            MaxOwinLibrary.SetTimeZoneIdForRequest();
        }

        /// <summary>
        /// 12. Occurs just before ASP.NET starts executing an event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationPreRequestHandlerExecute(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "12.Application_PreRequestHandlerExecute [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 13. ExecuteRequestHandler occurs when handler generates output. 
        /// This is the only event not exposed by the HTTPApplication class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationExecuteRequestHandler(object sender, EventArgs e)
        {
            //// This does not actually run!!!
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "13.Application_ExecuteRequestHandler [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 14. Occurs when the ASP.NET event handler has finished generating the output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationPostRequestHandlerExecute(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "14.Application_PostRequestHandlerExecute [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 15. Occurs after ASP.NET finishes executing all request event handlers. This event signal ASP.NET state modules to save the current request state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationReleaseRequestState(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "15.Application_ReleaseRequestState [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 16. Occurs when ASP.NET has completed executing all request event handlers and the request state data has been persisted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationPostReleaseRequestState(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "16.Application_PostReleaseRequestState [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 17. Occurs when ASP.NET finishes executing an event handler in order to let caching modules store responses that will be reused to serve identical requests from the cache.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationUpdateRequestCache(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "17.Application_UpdateRequestCache [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 18. When thePostUpdateRequestCache is raised, ASP.NET has completed processing code and the content of the cache is finalized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationPostUpdateRequestCache(object sender, EventArgs e)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "18.Application_PostUpdateRequestCache [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + HttpContext.Current.Request.Url.ToString() + "]", "MaxHttpApplicationLibraryProvider");
            //// When cookies are sent the OutputCache does not get added to the cache.
            //// This is so that cookies are not stored as part of the cache and sent to the wrong clients.
            //// Cookies sent with this event handler are not part of the cache.
            //// This event handler does not run when a cached page is served
            MaxOwinLibrary.SetStorageKeyForClient(HttpContext.Current.Request.Url, false, string.Empty);
        }

        /// <summary>
        /// 19. Occurs just before ASP.NET performs any logging for the current request. The LogRequest event is raised even if an error occurs. You can provide an event handler for the LogRequest event to provide custom logging for the request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationLogRequest(object sender, EventArgs e)
        {
            string lsUrl = "ApplicationLogRequest";
            try
            {
                lsUrl = HttpContext.Current.Request.RawUrl;
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure("ApplicationLogRequest", MaxEnumGroup.LogWarning, "Error Getting RawURL in ApplicationLogRequest {loE.ToString()}", loE.ToString()));
            }

            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "19.Application_LogRequest [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + lsUrl + "]", "MaxHttpApplicationLibraryProvider");
        }

        /// <summary>
        /// 20. Occurs when request has been logged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationPostLogRequest(object sender, EventArgs e)
        {
            string lsUrl = "ApplicationPostLogRequest";
            try
            {
                lsUrl = HttpContext.Current.Request.Url.ToString();
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure("ApplicationPostLogRequest", MaxEnumGroup.LogWarning, "Error Getting HttpContext.Current.Request.Url in ApplicationPostLogRequest {loE.ToString()}", loE.ToString()));
            }

            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "20.Application_PostLogRequest [" + MaxOwinLibrary.GetTimeSinceBeginRequest().ToString() + "] milliseconds [" + lsUrl + "]", "MaxHttpApplicationLibraryProvider");
            long lnDuration = MaxOwinLibrary.GetTimeSinceBeginRequest();
            if (lnDuration > 3000)
            {
                MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure("PageEnd", MaxFactry.Core.MaxEnumGroup.LogWarning, "Page Process End {lnDuration} {lsUrl}", lnDuration, lsUrl));
            }
            else
            {
                MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure("PageEnd", MaxFactry.Core.MaxEnumGroup.LogDebug, "Page Process End {lnDuration} {lsUrl}", lnDuration, lsUrl));
            }
        }
    }
}