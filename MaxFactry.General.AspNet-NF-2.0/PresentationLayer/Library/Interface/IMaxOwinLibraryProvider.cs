// <copyright file="IMaxOwinLibraryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="10/1/2018" author="Brian A. Lakstins" description="Add current URL to arguments to get storage key. Add Redirect URL support.">
// <change date="4/28/2020" author="Brian A. Lakstins" description="Remove code that depends on App configuration.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.PresentationLayer
{
    using System;
    using MaxFactry.Core;

    /// <summary>
    /// Library used to wrap methods that interact with the HttpRequestBase.
    /// </summary>
    public interface IMaxOwinLibraryProvider
    {
        /// <summary>
        /// Checks to see if the connection is secure.
        /// </summary>
        /// <returns>true if secure.  False otherwise.</returns>
        bool IsSecureConnection();

        /// <summary>
        /// Gets a secure URL based on the current request url.
        /// </summary>
        /// <returns>Url to a secure connection.</returns>
        Uri GetSecureUrl();

        /// <summary>
        /// Gets a secure URL based on the passed in Url and the current request.
        /// </summary>
        /// <param name="loUrl">The URL to use to get the secure Url.</param>
        /// <returns>Url to a secure connection.</returns>
        Uri GetSecureUrl(Uri loUrl);

        /// <summary>
        /// Gets the login URL based on the current request.
        /// </summary>
        /// <returns>Url to a login page.</returns>
        Uri GetLoginUrl();

        /// <summary>
        /// Gets the login URL based on the current request that returns to ReturnUrl
        /// </summary>
        /// <param name="lsReturnUrl">The url to return to after login.</param>
        /// <returns>Url to a login page.</returns>
        Uri GetLoginUrl(string lsReturnUrl);

        /// <summary>
        /// Checks to see if the current request is authenticated.
        /// </summary>
        /// <returns>True if authenticated.  False if not.</returns>
        bool IsAuthenticated();

        string GetStorageKeyForRequest(Uri loUrl);

        void SetStorageKeyForClient(Uri loUrl, bool lbOverride, string lsStorageKey);

        void ApplicationBeginRequest();

        void SetStorageKeyForRequest(Uri loUrl, string lsStorageKey);

        void SetTimeZoneIdForRequest();

        long GetTimeSinceBeginRequest();

        bool IsRecaptchaVerified(string lsSecret, string lsResponse, string lsRemoteIP);

        string GetTitle();

        string GetRedirectUrl(Uri loUrl);
    }
}