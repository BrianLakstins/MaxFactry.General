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
// <change date="5/29/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="6/23/2014" author="Brian A. Lakstins" description="Move default encryption provider from Security module into common web library.">
// <change date="6/27/2014" author="Brian A. Lakstins" description="Updates for StorageKey.">
// <change date="7/21/2014" author="Brian A. Lakstins" description="Update to no longer depend on core AppId functionality.">
// <change date="7/26/2014" author="Brian A. Lakstins" description="Add Windows Phone 8 fix.  Add default Bootstrap Navigation.">
// <change date="8/13/2014" author="Brian A. Lakstins" description="Added debug logging for all events in pipeline.">
// <change date="8/21/2014" author="Brian A. Lakstins" description="Add default file repository configuration.">
// <change date="12/22/2014" author="Brian A. Lakstins" description="Making storage key cookies server name specific.">
// <change date="6/10/2015" author="Brian A. Lakstins" description="Updated to store storage key in process when it is retrieved.">
// </changelog>
#endregion

namespace MaxFactry.App.Base.AspNet.IIS
{
	using System;
    using System.Collections;
    using System.Configuration;
    using System.Diagnostics;
    using System.Reflection;
    using System.Web;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;

    /// <summary>
    /// Library for handling application events
    /// </summary>
    public class MaxApplicationEventLibrary : MaxSingleFactory
	{
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxApplicationEventLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Gets the Provider used for most MaxFactory methods
        /// </summary>
        public static IMaxApplicationEventLibraryProvider Provider
        {
            get
            {
                if (null == Instance.BaseProvider)
                {
                    Instance.SetProvider(typeof(MaxFactry.App.Base.AspNet.IIS.Provider.MaxApplicationEventLibraryDefaultProvider));
                    if (!(Instance.BaseProvider is IMaxApplicationEventLibraryProvider))
                    {
                        throw new MaxException("Provider for MaxApplicationEventLibrary does not implement IMaxApplicationEventLibraryProvider.");
                    }
                }

                return (IMaxApplicationEventLibraryProvider)Instance.BaseProvider;
            }
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxApplicationEventLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxApplicationEventLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }

        /// <summary>
        /// 1. The BeginRequest event signals the creation of any given new request. 
        /// This event is always raised and is always the first event to occur during the processing of a request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationBeginRequest(object sender, EventArgs e)
        {
            Provider.ApplicationBeginRequest(sender, e);
        }

        /// <summary>
        /// 2. The AuthenticateRequest event signals that the configured authentication mechanism has authenticated the current request. 
        /// Subscribing to the AuthenticateRequest event ensures that the request will be authenticated before processing the attached 
        /// module or event handle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            Provider.ApplicationAuthenticateRequest(sender, e);
        }

        /// <summary>
        /// 3. The PostAuthenticateRequest event is raised after the AuthenticateRequest event has occurred.
        /// All the information available is accessible in the HttpContext’s User property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationPostAuthenticateRequest(object sender, EventArgs e)
        {
            Provider.ApplicationPostAuthenticateRequest(sender, e);
        }

        /// <summary>
        /// 4. The AuthorizeRequest event signals that ASP.NET has authorized the current request. 
        /// You can subscribe to the AuthorizeRequest event to perform custom authorization.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationAuthorizeRequest(object sender, EventArgs e)
        {
            Provider.ApplicationAuthorizeRequest(sender, e);
        }

        /// <summary>
        /// 5. Occurs when the user for the current request has been authorized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationPostAuthorizeRequest(object sender, EventArgs e)
        {
            Provider.ApplicationPostAuthorizeRequest(sender, e);
        }

        /// <summary>
        /// 6. Occurs when ASP.NET finishes an authorization event to let the caching modules serve requests from the cache, 
        /// bypassing execution of the event handler and calling any EndRequest handlers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationResolveRequestCache(object sender, EventArgs e)
        {
            Provider.ApplicationResolveRequestCache(sender, e);
        }

        /// <summary>
        /// 7. Reaching this event means the request can’t be served from the cache, and thus a HTTP handler is created here.
        /// A Page class gets created if an aspx page is requested.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationPostResolveRequestCache(object sender, EventArgs e)
        {
            Provider.ApplicationPostResolveRequestCache(sender, e);
        }

        /// <summary>
        /// 8. The MapRequestHandler event is used by the ASP.NET infrastructure to determine the request handler 
        /// for the current request based on the file-name extension of the requested resource.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationMapRequestHandler(object sender, EventArgs e)
        {
            Provider.ApplicationMapRequestHandler(sender, e);
        }

        /// <summary>
        /// 9. Occurs when ASP.NET has mapped the current request to the appropriate HTTP handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationPostMapRequestHandler(object sender, EventArgs e)
        {
            Provider.ApplicationPostMapRequestHandler(sender, e);
        }

        /// <summary>
        /// 10. Occurs when ASP.NET acquires the current state (for example, session state) that is associated with the current request. 
        /// A valid session ID must exist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationAcquireRequestState(object sender, EventArgs e)
        {
            Provider.ApplicationAcquireRequestState(sender, e);
        }

        /// <summary>
        /// 11. Occurs when the state information (for example, session state or application state) that is associated with the current request has been obtained.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationPostAcquireRequestState(object sender, EventArgs e)
        {
            Provider.ApplicationPostAcquireRequestState(sender, e);
        }

        /// <summary>
        /// 12. Occurs just before ASP.NET starts executing an event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationPreRequestHandlerExecute(object sender, EventArgs e)
        {
            Provider.ApplicationPreRequestHandlerExecute(sender, e);
        }

        /// <summary>
        /// 14. Occurs when the ASP.NET event handler has finished generating the output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationPostRequestHandlerExecute(object sender, EventArgs e)
        {
            Provider.ApplicationPostRequestHandlerExecute(sender, e);
        }

        /// <summary>
        /// 15. Occurs after ASP.NET finishes executing all request event handlers. This event signal ASP.NET state modules to save the current request state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationReleaseRequestState(object sender, EventArgs e)
        {
            Provider.ApplicationReleaseRequestState(sender, e);
        }

        /// <summary>
        /// 16. Occurs when ASP.NET has completed executing all request event handlers and the request state data has been persisted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationPostReleaseRequestState(object sender, EventArgs e)
        {
            Provider.ApplicationPostReleaseRequestState(sender, e);
        }

        /// <summary>
        /// 17. Occurs when ASP.NET finishes executing an event handler in order to let caching modules store responses that will be reused to serve identical requests from the cache.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationUpdateRequestCache(object sender, EventArgs e)
        {
            Provider.ApplicationUpdateRequestCache(sender, e);
        }

        /// <summary>
        /// 18. When thePostUpdateRequestCache is raised, ASP.NET has completed processing code and the content of the cache is finalized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationPostUpdateRequestCache(object sender, EventArgs e)
        {
            Provider.ApplicationPostUpdateRequestCache(sender, e);
        }

        /// <summary>
        /// 19. Occurs just before ASP.NET performs any logging for the current request. The LogRequest event is raised even if an error occurs. You can provide an event handler for the LogRequest event to provide custom logging for the request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationLogRequest(object sender, EventArgs e)
        {
            Provider.ApplicationLogRequest(sender, e);
        }

        /// <summary>
        /// 20. Occurs when request has been logged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationPostLogRequest(object sender, EventArgs e)
        {
            Provider.ApplicationPostLogRequest(sender, e);
            //// Last thing that is run with the request.  Clear any logs that are associated with the current managed thread.
            MaxLogLibrary.ClearRecent();
        }
    }
}