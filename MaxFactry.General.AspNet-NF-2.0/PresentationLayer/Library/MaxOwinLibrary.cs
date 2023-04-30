// <copyright file="MaxOwinLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="8/31/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="9/9/2016" author="Brian A. Lakstins" description="Moving cookie name for storage key to library from provider.">
// <change date="10/1/2018" author="Brian A. Lakstins" description="Add current URL to arguments to get storage key. Add Redirect URL support.">
// <change date="4/28/2020" author="Brian A. Lakstins" description="Remove code that depends on App configuration.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.PresentationLayer
{
    using MaxFactry.Core;
    using System;

    /// <summary>
    /// Library used to wrap methods that interact with the HttpRequestBase.
    /// </summary>
    public class MaxOwinLibrary : MaxSingleFactory
    {
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxOwinLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Gets the Provider used for most MaxFactory methods
        /// </summary>
        public static IMaxOwinLibraryProvider Provider
        {
            get
            {
                if (null == Instance.BaseProvider)
                {
                    Instance.SetProvider(typeof(MaxFactry.General.AspNet.PresentationLayer.Provider.MaxOwinLibraryDefaultProvider));
                    if (!(Instance.BaseProvider is IMaxOwinLibraryProvider))
                    {
                        throw new MaxException("Provider for MaxRequestLibrary does not implement IMaxRequestLibraryProvider.");
                    }
                }

                return (IMaxOwinLibraryProvider)Instance.BaseProvider;
            }
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxOwinLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxOwinLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }


        /// <summary>
        /// Checks to see if the connection is secure.
        /// </summary>
        /// <returns>true if secure.  False otherwise.</returns>
        public static bool IsSecureConnection()
        {
            return Provider.IsSecureConnection();
        }

        /// <summary>
        /// Gets a secure URL based on the current request url.
        /// </summary>
        /// <returns>Url to a secure connection.</returns>
        public static Uri GetSecureUrl()
        {
            return Provider.GetSecureUrl();
        }

        /// <summary>
        /// Gets a secure URL based on the passed in Url and the current request.
        /// </summary>
        /// <param name="loUrl">The URL to use to get the secure Url.</param>
        /// <returns>Url to a secure connection.</returns>
        public static Uri GetSecureUrl(Uri loUrl)
        {
            return Provider.GetSecureUrl(loUrl);
        }

        /// <summary>
        /// Gets the login URL based on the current request.
        /// </summary>
        /// <returns>Url to a login page.</returns>
        public static Uri GetLoginUrl()
        {
            return Provider.GetLoginUrl();
        }

        /// <summary>
        /// Gets the login URL based on the current request that returns to ReturnUrl
        /// </summary>
        /// <param name="lsReturnUrl">The url to return to after login.</param>
        /// <returns>Url to a login page.</returns>
        public static Uri GetLoginUrl(string lsReturnUrl)
        {
            return Provider.GetLoginUrl(lsReturnUrl);
        }

        /// <summary>
        /// Checks to see if the current request is authenticated.
        /// </summary>
        /// <param name="loRequest">The current request.</param>
        /// <returns>True if authenticated.  False if not.</returns>
        public static bool IsAuthenticated()
        {
            return Provider.IsAuthenticated();
        }

        public static string GetStorageKeyForRequest(Uri loUrl)
        {
            return Provider.GetStorageKeyForRequest(loUrl);
        }

        public static void SetStorageKeyForClient(Uri loUrl, bool lbOverride, string lsStorageKey)
        {
            Provider.SetStorageKeyForClient(loUrl, lbOverride, lsStorageKey);
        }

        public static string GetTitle()
        {
            return Provider.GetTitle();
        }

        public static void ApplicationBeginRequest()
        {
            Provider.ApplicationBeginRequest();
        }

        public static void SetStorageKeyForRequest(Uri loUrl, string lsStorageKey)
        {
            Provider.SetStorageKeyForRequest(loUrl, lsStorageKey);
        }

        public static void SetTimeZoneIdForRequest()
        {
            Provider.SetTimeZoneIdForRequest();
        }

        /// <summary>
        /// Gets the milliseconds since the request began.
        /// </summary>
        /// <returns></returns>
        public static long GetTimeSinceBeginRequest()
        {
            return Provider.GetTimeSinceBeginRequest();
        }

        public static bool IsRecaptchaVerified(string lsSecret, string lsResponse, string lsRemoteIP)
        {
            return Provider.IsRecaptchaVerified(lsSecret, lsResponse, lsRemoteIP);
        }

        public static bool IshCaptchaVerified(string lsSecret, string lsResponse, string lsRemoteIP)
        {
            return Provider.IshCaptchaVerified(lsSecret, lsResponse, lsRemoteIP);
        }

        public static string GetRedirectUrl(Uri loUrl)
        {
            return Provider.GetRedirectUrl(loUrl);
        }
    }
}