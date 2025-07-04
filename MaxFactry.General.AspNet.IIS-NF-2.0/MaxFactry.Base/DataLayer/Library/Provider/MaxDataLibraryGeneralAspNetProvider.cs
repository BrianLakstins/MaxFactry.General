﻿// <copyright file="MaxDataLibraryGeneralAspNetProvider.cs" company="Lakstins Family, LLC">
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
// <change date="5/22/2020" author="Brian A. Lakstins" description="Initial creation">
// <change date="3/15/2021" author="Brian A. Lakstins" description="Reduce exceptions by checking for context">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class. Use parent methods instead of repository.">
// <change date="6/11/2025" author="Brian A. Lakstins" description="Update for ApplicationKey">
// </changelog>
#endregion

namespace MaxFactry.Base.DataLayer.Library.Provider
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using MaxFactry.Core;

    /// <summary>
    /// Provides methods to manipulate storage of data
    /// </summary>
    public class MaxDataLibraryGeneralAspNetProvider : MaxDataLibraryDefaultProvider
    {
        /// <summary>
        /// Gets the storage key used to separate the storage of data
        /// </summary>
        /// <param name="loData">The data to be stored using the storage key.</param>
        /// <returns>string used for the storage key</returns>
        public override string GetApplicationKey()
        {
            string lsR = base.GetApplicationKey();
            string lsApplicationKey = this.GetStorageKeyFromQueryString();
            if (null == lsApplicationKey || lsApplicationKey.Length.Equals(0))
            {
                lsApplicationKey = this.GetStorageKeyFromCookie();
            }

            if (null != lsApplicationKey && lsApplicationKey.Length > 0)
            {
                lsR = lsApplicationKey;
            }

            if (lsR.Length == 0)
            {
                MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "GetStorageKeyProvider", MaxEnumGroup.LogError, "GetStorageKey(MaxData loData) ended with blank storagekey."));
            }

            return lsR;
        }

        protected virtual string GetStorageKeyFromCookie()
        {
            //// TODO: Use MaxOwnLibrary after it's been moved to a general library namespace to remove duplicate code.
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
                            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, MaxFactryLibrary.MaxStorageKeyName, lsR);
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

        protected virtual string GetStorageKeyFromQueryString()
        {
            string lsR = string.Empty;
            //// Attempt to get the Max Storage Key from the QueryString.
            if (this.HasRequest)
            {
                string lsStorageKey = this.Request.QueryString[MaxFactry.General.AspNet.IIS.MaxAppLibrary.MaxStorageKeyQueryName];
                if (!string.IsNullOrEmpty(lsStorageKey))
                {
                    try
                    {
                        Guid loStorageKey = new Guid(lsStorageKey);
                        if (Guid.Empty != loStorageKey)
                        {
                            MaxLogLibrary.Log(MaxEnumGroup.LogInfo, "GetStorageKey from QueryString [" + loStorageKey.ToString() + "]", "MaxRequestLibraryDefaultProvider");
                            lsR = loStorageKey.ToString();
                            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, MaxFactryLibrary.MaxStorageKeyName, lsR);
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

        protected virtual HttpRequest Request
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

        private bool HasRequest
        {
            get
            {
                object loValue = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, "MaxDataLibraryGeneralAspNetProvider.HasRequest");
                if (null != loValue && loValue is bool)
                {
                    return (bool)loValue;
                }

                try
                {
                    if (null != HttpContext.Current)
                    {
                        if (null != HttpContext.Current.Request)
                        {
                            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "MaxDataLibraryGeneralAspNetProvider.HasRequest", true);
                            return true;
                        }
                        else
                        {
                            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "MaxDataLibraryGeneralAspNetProvider.HasRequest", false);
                        }
                    }
                }
                catch
                {
                    MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "MaxDataLibraryGeneralAspNetProvider.HasRequest", false);
                }

                return false;
            }
        }
    }
}
