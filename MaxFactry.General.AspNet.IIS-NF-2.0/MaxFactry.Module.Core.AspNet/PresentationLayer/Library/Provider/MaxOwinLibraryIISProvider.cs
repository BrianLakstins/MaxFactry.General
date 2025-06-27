// <copyright file="MaxOwinLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// <change date="2/24/2016" author="Brian A. Lakstins" description="Update storage key cookie to not specify server name, so it's always server name specific.">
// <change date="7/22/2016" author="Brian A. Lakstins" description="Update for change to MaxConfigurationLibrary">
// <change date="9/9/2016" author="Brian A. Lakstins" description="Add checking to see if storage cookie has already been sent before sending.">
// <change date="9/24/2018" author="Brian A. Lakstins" description="Remove sending of email for every host name.">
// <change date="10/1/2018" author="Brian A. Lakstins" description="Add current URL to arguments to get storage key.">
// <change date="10/20/2018" author="Brian A. Lakstins" description="Allow checking for Storage Key based on Url without needing Http Request.  Check for headers already being written before sending cookies.">
// <change date="4/10/2020" author="Brian A. Lakstins" description="Remove ability to use development environment from URL to prevent security bypass.">
// <change date="5/31/2020" author="Brian A. Lakstins" description="Add host check in case it can't be parsed.">
// <change date="2/15/2021" author="Brian A. Lakstins" description="Update secure connection definition to help with local testing.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.PresentationLayer.Provider
{
    using System;
    using System.Web;
    using System.Web.Security;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.General.BusinessLayer;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Library used to wrap methods that interact with the HttpRequestBase.
    /// </summary>
    public class MaxOwinLibraryIISProvider : MaxOwinLibraryDefaultProvider
    {
        private static MaxIndex _oHostIndex = new MaxIndex();

        private static MaxIndex _oIANATZIndex = new MaxIndex();

        private static object _oLock = new object();

        private bool HasRequest
        {
            get
            {
                object loValue = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, "MaxConfigurationLibraryAspNetIISProvider.HasRequest");
                if (null != loValue && loValue is bool)
                {
                    return (bool)loValue;
                }

                try
                {
                    if (null != HttpContext.Current.Request)
                    {
                        MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "MaxConfigurationLibraryAspNetIISProvider.HasRequest", true);
                        return true;
                    }
                    else
                    {
                        MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "MaxConfigurationLibraryAspNetIISProvider.HasRequest", false);
                    }
                }
                catch
                {
                    MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "MaxConfigurationLibraryAspNetIISProvider.HasRequest", false);
                }

                return false;
            }
        }

        /// <summary>
        /// Checks to see if the connection is secure.
        /// </summary>
        /// <returns>true if secure.  False otherwise.</returns>
        public override bool IsSecureConnection()
        {
            if (this.Request.IsSecureConnection)
            {
                return true;
            }

            if (this.Request.IsLocal || this.Request.Url.DnsSafeHost.ToLower().EndsWith(".local"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a secure URL based on the current request url.
        /// </summary>
        /// <returns>Url to a secure connection.</returns>
        public override Uri GetSecureUrl()
        {
            return GetSecureUrl(this.Request.Url);
        }

        /// <summary>
        /// Gets a secure URL based on the passed in Url and the current request.
        /// </summary>
        /// <param name="loUrl">The URL to use to get the secure Url.</param>
        /// <returns>Url to a secure connection.</returns>
        public override Uri GetSecureUrl(Uri loUrl)
        {
            UriBuilder loBuilder = new UriBuilder(loUrl);
            loBuilder.Scheme = Uri.UriSchemeHttps;
            loBuilder.Port = 443;
            return loBuilder.Uri;
        }

        /// <summary>
        /// Gets the login URL based on the current request.
        /// </summary>
        /// <returns>Url to a login page.</returns>
        public override Uri GetLoginUrl()
        {
            return GetLoginUrl(this.Request.Url.PathAndQuery);
        }

        /// <summary>
        /// Gets the login URL based on the current request that returns to ReturnUrl
        /// </summary>
        /// <param name="lsReturnUrl">The url to return to after login.</param>
        /// <returns>Url to a login page.</returns>
        public override Uri GetLoginUrl(string lsReturnUrl)
        {
            UriBuilder loLoginBuilder = new UriBuilder(this.Request.Url);
            loLoginBuilder.Path = FormsAuthentication.LoginUrl;
            loLoginBuilder.Query = "returnUrl=" + HttpUtility.UrlEncode(lsReturnUrl);
            return GetSecureUrl(loLoginBuilder.Uri);
        }

        /// <summary>
        /// Checks to see if the current request is authenticated.
        /// </summary>
        /// <returns>True if authenticated.  False if not.</returns>
        public override bool IsAuthenticated()
        {
            if (MaxFactry.Core.MaxFactryLibrary.Environment.Equals(MaxFactry.Core.MaxEnumGroup.EnvironmentDevelopment))
            {
                return true;
            }

            return this.Request.IsAuthenticated;
        }

        public virtual HttpRequest Request
        {
            get
            {
                if (null != HttpContext.Current)
                {
                    if (this.HasRequest)
                    {
                        return HttpContext.Current.Request;
                    }
                }

                return null;
            }

        }

        public override string GetStorageKeyFromQueryString()
        {
            string lsR = string.Empty;
            //// Attempt to get the Max Storage Key from the QueryString.
            if (this.HasRequest)
            {
                string lsStorageKey = this.Request.QueryString["msk"];
                if (!string.IsNullOrEmpty(lsStorageKey))
                {
                    try
                    {
                        Guid loStorageKey = new Guid(lsStorageKey);
                        if (Guid.Empty != loStorageKey)
                        {
                            MaxLogLibrary.Log(MaxEnumGroup.LogInfo, "GetStorageKey from QueryString [" + loStorageKey.ToString() + "]", "MaxRequestLibraryDefaultProvider");
                            lsR = loStorageKey.ToString();
                        }
                    }
                    catch (Exception loE)
                    {
                        MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error in GetStorageKeyFromQueryString", loE));
                    }
                }
            }


            return lsR;
        }

        public override string GetStorageKeyFromCookie()
        {
            string lsR = string.Empty;
            if (this.HasRequest)
            {
                if (null != this.Request.Cookies[MaxFactry.General.AspNet.IIS.MaxAppLibrary.MaxStorageKeyCookieName])
                {
                    //// Attempt to get the Max Storage Key from the Cookie.
                    string lsStorageKeyCookie = this.Request.Cookies[MaxFactry.General.AspNet.IIS.MaxAppLibrary.MaxStorageKeyCookieName].Value;

                    try
                    {
                        Guid loCookieStorageKey = MaxConvertLibrary.ConvertToGuid(typeof(object), lsStorageKeyCookie);
                        if (Guid.Empty != loCookieStorageKey)
                        {
                            MaxLogLibrary.Log(MaxEnumGroup.LogInfo, "GetStorageKey from Cookie [" + loCookieStorageKey.ToString() + "]", "MaxRequestLibraryDefaultProvider");
                            lsR = loCookieStorageKey.ToString();
                        }
                    }
                    catch (Exception loE)
                    {
                        MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error in GetStorageKeyFromCookie", loE));
                    }
                }
            }

            return lsR;
        }

        public override string GetStorageKeyForRequest(Uri loUrl)
        {
            string lsR = string.Empty;
            string lsStorageKey = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, "_MaxStorageKeyForRequest") as string;
            if (null != lsStorageKey)
            {
                lsR = lsStorageKey;
            }
            else
            {
                //// Look up MaxStorageKey based on Url.
                lsR = this.GetStorageKeyFromUrl(loUrl);
                if (string.IsNullOrEmpty(lsR) && null != HttpContext.Current && this.HasRequest)
                {
                    lsR = this.GetStorageKeyFromQueryString();

                    if (string.IsNullOrEmpty(lsR))
                    {
                        lsR = this.GetStorageKeyFromCookie();
                    }

                    if (string.IsNullOrEmpty(lsR))
                    {
                        lsR = this.GetStorageKeyFromProfile();
                    }
                }

                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "_MaxStorageKeyForRequest", lsR);
            }

            return lsR;
        }

        public override void SetStorageKeyForClient(Uri loUrl, bool lbOverride, string lsStorageKey)
        {
            //// Set a cookie for the current MaxStorageKey if it has not already been set.
            if (!this.IsHeadersWritten && this.HasRequest)
            {
                if (null == this.Request.Cookies[MaxFactry.General.AspNet.IIS.MaxAppLibrary.MaxStorageKeyCookieName] || lbOverride)
                {
                    string lsValue = lsStorageKey;
                    if (string.IsNullOrEmpty(lsValue))
                    {
                        lsValue = GetStorageKeyForRequest(loUrl);
                    }

                    if (!string.IsNullOrEmpty(lsValue))
                    {
                        HttpCookie loCookie = new HttpCookie(MaxFactry.General.AspNet.IIS.MaxAppLibrary.MaxStorageKeyCookieName, lsValue);
                        /*
                        if (HttpContext.Current.Request.Url.Host.Contains("."))
                        {
                            loCookie.Domain = this.Request.Url.Host;
                        }
                        */
                        loCookie.Expires = DateTime.UtcNow.AddMonths(1);
                        try
                        {
                            bool lbCookieSent = false;
                            foreach (string lsName in HttpContext.Current.Response.Cookies.AllKeys)
                            {
                                if (lsName.Equals(MaxFactry.General.AspNet.IIS.MaxAppLibrary.MaxStorageKeyCookieName))
                                {
                                    lbCookieSent = true;
                                }
                            }

                            if (!lbCookieSent)
                            {
                                HttpContext.Current.Response.Cookies.Add(loCookie);
                            }
                        }
                        catch (Exception loE)
                        {
                            MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error setting client cookie {Url}", loE, loUrl));
                        }

                        MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogInfo, "SetStorageKeyForClient(" + lbOverride.ToString() + ")->Sending MaxStorageKey Cookie " + lsValue, "MaxHttpApplicationLibrary");
                    }
                }
            }
        }

        public override void SetTimeZoneIdForRequest()
        {
            HttpCookie loCookieTZ = HttpContext.Current.Request.Cookies["MaxTZ"];
            if (null != loCookieTZ)
            {
                string lsIANATimeZone = loCookieTZ.Value;
                string lsTimeZoneId = null;
                if (!_oIANATZIndex.Contains(lsIANATimeZone))
                {
                    lock (_oIANATZIndex)
                    {
                        if (!_oIANATZIndex.Contains(lsIANATimeZone))
                        { 
                            string lsMapping = MaxFactry.Core.MaxFactryLibrary.GetStringResource(typeof(MaxOwinLibraryIISProvider), "Mapping");
                            string[] laMapping = lsMapping.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            for (int lnM = 0; lnM < laMapping.Length && string.IsNullOrEmpty(lsTimeZoneId); lnM++)
                            {
                                if (laMapping[lnM].Contains(lsIANATimeZone))
                                {
                                    string[] laMap = laMapping[lnM].Split(new char[] { ',' });
                                    string[] laName = laMap[2].Split(new char[] { ' ' });
                                    foreach (string lsName in laName)
                                    {
                                        if (loCookieTZ.Value == lsName)
                                        {
                                            lsTimeZoneId = laMap[0];
                                        }
                                    }
                                }
                            }

                            _oIANATZIndex.Add(lsIANATimeZone, lsTimeZoneId);
                        }
                    }
                }

                if (string.IsNullOrEmpty(lsTimeZoneId) && _oIANATZIndex.Contains(lsIANATimeZone))
                {
                    lsTimeZoneId = _oIANATZIndex.GetValueString(lsIANATimeZone);
                }

                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, MaxConvertLibrary.MaxTimeZoneIdKey, lsTimeZoneId);
            }
        }


#if net4_52
        protected bool IsHeadersWritten
        {
            get
            {
                if (null != HttpContext.Current && null != HttpContext.Current.Response)
                {
                    return HttpContext.Current.Response.HeadersWritten;
                }

                return false;
            }
        }
#else
        protected bool IsHeadersWritten
        {
            get
            {
                return false;
            }
        }
#endif

        public override void ApplicationBeginRequest()
        {
            base.ApplicationBeginRequest();
            if (this.HasRequest)
            {
                string lsHost = "unknown";
                try
                {
                    lsHost = this.Request.Url.DnsSafeHost.ToLowerInvariant();
                }
                catch (Exception loE)
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "ApplicationBeginRequest", MaxEnumGroup.LogWarning, "Error Getting this.Request.Url.DnsSafeHost in ApplicationBeginRequest {loE.ToString()}", loE.ToString()));
                }

                //// Set the environment based on the requested url
                if (lsHost.Contains("-qa") &&
                    MaxFactryLibrary.Environment != MaxEnumGroup.EnvironmentQA)
                {
                    MaxFactryLibrary.Environment = MaxEnumGroup.EnvironmentUnknown;
                    MaxFactryLibrary.Environment = MaxEnumGroup.EnvironmentQA;
                }
                else if (lsHost.Contains("-test") &&
                    MaxFactryLibrary.Environment != MaxEnumGroup.EnvironmentTesting)
                {
                    MaxFactryLibrary.Environment = MaxEnumGroup.EnvironmentUnknown;
                    MaxFactryLibrary.Environment = MaxEnumGroup.EnvironmentTesting;
                }
            }
        }
    }
}