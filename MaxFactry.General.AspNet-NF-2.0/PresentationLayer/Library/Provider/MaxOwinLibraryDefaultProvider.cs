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
// <change date="12/4/2015" author="Brian A. Lakstins" description="fix for opening _blank windows twice.">
// <change date="9/9/2016" author="Brian A. Lakstins" description="Moving cookie name for storage key to library from provider.">
// <change date="10/1/2018" author="Brian A. Lakstins" description="Add current URL to arguments to get storage key.">
// <change date="4/28/2020" author="Brian A. Lakstins" description="Remove code that depends on App configuration.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.PresentationLayer.Provider
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.PresentationLayer;

    /// <summary>
    /// Library used to wrap methods that interact with the HttpRequestBase.
    /// </summary>
    public class MaxOwinLibraryDefaultProvider : MaxProvider, IMaxOwinLibraryProvider
    {
        /// <summary>
        /// Checks to see if the connection is secure.
        /// </summary>
        /// <returns>true if secure.  False otherwise.</returns>
        public virtual bool IsSecureConnection()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a secure URL based on the current request url.
        /// </summary>
        /// <returns>Url to a secure connection.</returns>
        public virtual Uri GetSecureUrl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a secure URL based on the passed in Url and the current request.
        /// </summary>
        /// <param name="loUrl">The URL to use to get the secure Url.</param>
        /// <returns>Url to a secure connection.</returns>
        public virtual Uri GetSecureUrl(Uri loUrl)
        {
            throw new NotImplementedException();

        }

        /// <summary>
        /// Gets the login URL based on the current request.
        /// </summary>
        /// <returns>Url to a login page.</returns>
        public virtual Uri GetLoginUrl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the login URL based on the current request that returns to ReturnUrl
        /// </summary>
        /// <param name="lsReturnUrl">The url to return to after login.</param>
        /// <returns>Url to a login page.</returns>
        public virtual Uri GetLoginUrl(string lsReturnUrl)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks to see if the current request is authenticated.
        /// </summary>
        /// <returns>True if authenticated.  False if not.</returns>
        public virtual bool IsAuthenticated()
        {
            throw new NotImplementedException();
        }

        public virtual string GetStorageKeyFromQueryString()
        {
            throw new NotImplementedException();
        }

        public virtual string GetStorageKeyFromCookie()
        {
            throw new NotImplementedException();
        }

        public virtual string GetStorageKeyFromProfile()
        {
            string lsR = string.Empty;
            //// Attempt to get the Max Storage Key from the Profile.
            object loObjectProfile = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProfile, MaxFactryLibrary.MaxStorageKeyName);
            if (null != loObjectProfile)
            {
                string lsProfileStorageKey = string.Empty;
                if (loObjectProfile is Guid)
                {
                    lsProfileStorageKey = ((Guid)loObjectProfile).ToString();
                }
                else if (loObjectProfile.ToString().Trim().Length > 0)
                {
                    lsProfileStorageKey = loObjectProfile.ToString();
                }

                if (!string.IsNullOrEmpty(lsProfileStorageKey))
                {
                    MaxLogLibrary.Log(MaxEnumGroup.LogDebug, "GetStorageKey from Profile Configuration [" + lsProfileStorageKey + "]", "MaxRequestLibraryDefaultProvider");
                    lsR = lsProfileStorageKey;
                }
            }

            return lsR;
        }

        public virtual string GetStorageKeyFromUrl(Uri loUrl)
        {
            return string.Empty;
        }

        public virtual string GetStorageKeyForRequest(Uri loUrl)
        {
            throw new NotImplementedException();
        }

        public virtual void SetStorageKeyForClient(Uri loUrl, bool lbOverride, string lsStorageKey)
        {
            throw new NotImplementedException();
        }

        public virtual long GetTimeSinceBeginRequest()
        {
            long lnR = 0;
            object loObject = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, "__MaxRequestWatch");
            if (null != loObject && loObject is System.Diagnostics.Stopwatch)
            {
                lnR = ((System.Diagnostics.Stopwatch)loObject).ElapsedMilliseconds;
            }

            return lnR;
        }

        public virtual void SetStorageKeyForRequest(Uri loUrl, string lsStorageKey)
        {
            string lsValue = lsStorageKey;
            if (string.IsNullOrEmpty(lsValue))
            {
                lsValue = this.GetStorageKeyForRequest(loUrl);
            }

            if (!string.IsNullOrEmpty(lsValue))
            {
                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, MaxFactryLibrary.MaxStorageKeyName, lsValue);
            }
        }

        public virtual void SetTimeZoneIdForRequest()
        {
            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, MaxConvertLibrary.MaxTimeZoneIdKey, "Eastern Standard Time");
        }

        public virtual void ApplicationBeginRequest()
        {
            System.Diagnostics.Stopwatch loWatch = System.Diagnostics.Stopwatch.StartNew();
            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "__MaxRequestWatch", loWatch);
            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "__IsRequestStarted", true);
        }

        public bool IsRecaptchaVerified(string lsSecret, string lsResponse, string lsRemoteIP)
        {
            bool lbR = false;
            string lsPostData = String.Format("secret={0}&response={1}", lsSecret, lsResponse);

            byte[] laPostData = System.Text.Encoding.ASCII.GetBytes(lsPostData);

            Uri loUri = new Uri("https://www.google.com/recaptcha/api/siteverify", UriKind.Absolute);

            try
            {
                System.Net.WebRequest loRequest = System.Net.WebRequest.Create(loUri);
                loRequest.ContentType = "application/x-www-form-urlencoded";
                loRequest.ContentLength = laPostData.Length;
                loRequest.Method = "POST";
                System.IO.Stream loDataIn = loRequest.GetRequestStream();
                loDataIn.Write(laPostData, 0, laPostData.Length);
                loDataIn.Close();

                System.Net.WebResponse loResponse = loRequest.GetResponse();

                System.IO.Stream loDataOut = loResponse.GetResponseStream();
                System.IO.StreamReader loReader = new System.IO.StreamReader(loDataOut);
                string lsDataOut = loReader.ReadToEnd();
                loReader.Close();
                loDataOut.Close();
                loResponse.Close();

                if (lsDataOut.Replace(" ", string.Empty).Contains("\"success\":true"))
                {
                    lbR = true;
                }
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error validating Recaptcha", loE));
            }

            return lbR;
        }

        public virtual string GetTitle()
        {
            object loValue = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeAny, "__MaxTitle");
            return MaxConvertLibrary.ConvertToString(typeof(object), loValue);
        }

        public virtual string GetRedirectUrl(Uri loUrl)
        {
            return string.Empty;
        }
    }
}