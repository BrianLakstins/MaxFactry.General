// <copyright file="MaxConfigurationLibraryWebProvider.cs" company="Lakstins Family, LLC">
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
// <change date="6/27/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="6/29/2014" author="Brian A. Lakstins" description="Added storing of Profile data.">
// <change date="7/8/2014" author="Brian A. Lakstins" description="Updated profile index storage. Added user index storage.  Depends on a Membership Provider being defined.">
// <change date="7/21/2014" author="Brian A. Lakstins" description="Updated to only store profile Id when cookie is confirmed.">
// <change date="8/8/2014" author="Brian A. Lakstins" description="Updated to only access storage when profile cookie has been confirmed.">
// <change date="8/13/2014" author="Brian A. Lakstins" description="Added configuration and logging.">
// <change date="8/21/2014" author="Brian A. Lakstins" description="Fix for null error.">
// <change date="11/27/2014" author="Brian A. Lakstins" description="Update profile handling.">
// <change date="2/3/2015" author="Brian A. Lakstins" description="Make profile cookie server specific.  Get Application scope configuration from AppSettings.">
// <change date="6/10/2015" author="Brian A. Lakstins" description="Review and update handling of profile values since first add to cart was not working.">
// <change date="1/14/2016" author="Brian A. Lakstins" description="Update handling of url when it's not available and profile when no records exist for the profile.">
// <change date="2/24/2016" author="Brian A. Lakstins" description="Update profile cookie to contain host name.">
// <change date="7/22/2016" author="Brian A. Lakstins" description="Update for change to MaxConfigurationLibrary">
// </changelog>
#endregion

namespace MaxFactry.Core.Provider
{
	using System;
    using System.Reflection;
    using System.Web;
    using System.Web.Security;
    using MaxFactry.Core;
    using MaxFactry.Core.Provider;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.General.BusinessLayer;
    
    /// <summary>
    /// MaxFactory Provider for use in web applications.
    /// TODO: Review this and move code to lower level if possible.
    /// </summary>
    public class MaxConfigurationLibraryAspNetIISProvider : MaxConfigurationLibraryGeneralProvider
    {
        private readonly string _sProfileCookieKey = "MaxProfileId1";

        private readonly string _sProcessProfileIdKey = "MaxProfileIndexId";

        private bool HasRequest
        {
            get
            {
                object loValue = this.GetValue(MaxEnumGroup.ScopeProcess, "MaxConfigurationLibraryAspNetIISProvider.HasRequest");
                if (null != loValue && loValue is bool)
                { 
                    return (bool)loValue;
                }

                try
                {
                    if (null != HttpContext.Current.Request)
                    {
                        this.SetValue(MaxEnumGroup.ScopeProcess, "MaxConfigurationLibraryAspNetIISProvider.HasRequest", true);
                        return true;
                    }
                    else
                    {
                        this.SetValue(MaxEnumGroup.ScopeProcess, "MaxConfigurationLibraryAspNetIISProvider.HasRequest", false);
                    }
                }
                catch 
                {
                    this.SetValue(MaxEnumGroup.ScopeProcess, "MaxConfigurationLibraryAspNetIISProvider.HasRequest", false);
                }

                return false;
            }
        }

        /// <summary>
        /// Sets a value based on the scope.
        /// </summary>
        /// <param name="loScope">The scope for the value.</param>
        /// <param name="lsKey">The key to use to retrieve the value.</param>
        /// <param name="loValue">The value being set.</param>
        public override void SetValue(MaxEnumGroup loScope, string lsKey, object loValue)
        {
            if (loScope == MaxEnumGroup.ScopeProcess)
            {
                if (null != HttpContext.Current && null != HttpContext.Current.Items)
                {
                    HttpContext.Current.Items[lsKey] = loValue;
                }
            }
            else if (loScope == MaxEnumGroup.ScopeSession)
            {
                if (null != HttpContext.Current && null != HttpContext.Current.Session && this.HasRequest)
                {
                    string lsServer = HttpContext.Current.Request.Url.DnsSafeHost;
                    HttpContext.Current.Session[lsKey + lsServer] = loValue;
                }
            }
            else if (loScope == MaxEnumGroup.ScopeUser)
            {
                try
                {
                    if (null != Membership.Provider)
                    {
                        if (HttpContext.Current.Request.IsAuthenticated)
                        {
                            string lsValue = string.Empty;
                            if (null != loValue)
                            {
                                lsValue = loValue.ToString();
                            }

                            MembershipUser loUser = Membership.GetUser();
                            if (null != loUser)
                            {
                                object loId = loUser.ProviderUserKey;
                                Guid loUserId = Guid.Empty;
                                if (loId is Guid)
                                {
                                    loUserId = (Guid)loId;
                                }
                                else
                                {
                                    loUserId = new Guid(loId.ToString());
                                }

                                if (!Guid.Empty.Equals(loUserId))
                                {
                                    MaxUserIndexEntity.Create().SaveValue(loUserId, lsKey, lsValue);
                                }
                            }
                        }
                    }
                }
                catch (Exception loE)
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error accessing user scope", loE));
                }
            }
            else
            {
                base.SetValue(loScope, lsKey, loValue);
            }
        }

        /// <summary>
        /// Gets a value in a scope.
        /// </summary>
        /// <param name="loScope">The scope to use.</param>
        /// <param name="lsKey">The key to use to look up the value.</param>
        /// <returns>The value if it is found.  Null if not found.</returns>
        public override object GetValue(MaxEnumGroup loScope, string lsKey)
        {
            object loValue = null;
            if (loScope == MaxEnumGroup.ScopeProcess)
            {
                if (null != HttpContext.Current && null != HttpContext.Current.Items)
                {
                    loValue = HttpContext.Current.Items[lsKey];
                    if (null == loValue && lsKey.Equals("Id"))
                    {
                        loValue = Guid.NewGuid();
                        this.SetValue(loScope, "Id", loValue);
                    }
                }
            }
            else if (loScope == MaxEnumGroup.ScopeSession)
            {
                if (null != HttpContext.Current && null != HttpContext.Current.Session && this.HasRequest)
                {
                    string lsServer = HttpContext.Current.Request.Url.DnsSafeHost;
                    loValue = HttpContext.Current.Session[lsKey + lsServer];
                    if (null == loValue && lsKey.Equals("Id"))
                    {
                        loValue = Guid.NewGuid();
                        this.SetValue(loScope, "Id", loValue);
                    }
                }
            }
            else if (loScope == MaxEnumGroup.ScopeProfile)
            {
                Guid loId = this.GetCurrentProfileId();
                if (lsKey.Equals("Id", StringComparison.InvariantCultureIgnoreCase))
                {
                    loValue = loId;
                }
                else if (this.IsProfileConfirmed(loId))
                {
                    loValue = MaxProfileIndexEntity.Create().GetValue(loId, lsKey);
                    if (loValue is string && string.Empty == (string)loValue)
                    {
                        loValue = null;
                    }
                }
            }
            else if (loScope == MaxEnumGroup.ScopeUser)
            {
                if (null != HttpContext.Current &&
                    null != HttpContext.Current.User &&
                    HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    MembershipUser loUser = Membership.GetUser(HttpContext.Current.User.Identity.Name);
                    if (null != loUser)
                    {
                        object loId = loUser.ProviderUserKey;
                        Guid loUserId = Guid.Empty;
                        if (loId is Guid)
                        {
                            loUserId = (Guid)loId;
                        }
                        else
                        {
                            loUserId = new Guid(loId.ToString());
                        }

                        if (!Guid.Empty.Equals(loUserId))
                        {
                            if (lsKey.Equals("Id"))
                            {
                                loValue = loUserId;
                            }
                            else
                            {
                                loValue = MaxUserIndexEntity.Create().GetValue(loUserId, lsKey);
                            }
                        }
                    }
                }
            }
            else if (loScope == MaxEnumGroup.ScopeApplication)
            {
                loValue = base.GetValue(loScope, lsKey);
                if (lsKey.Equals("MaxDataDirectory", StringComparison.InvariantCultureIgnoreCase) && null != HttpContext.Current)
                {
                    if (null == loValue || (loValue is string && !((string)loValue).Contains("App_Data")))
                    {
                        loValue = HttpContext.Current.Server.MapPath("~/App_Data");
                        base.SetValue(loScope, lsKey, loValue);
                    }
                }
            }
            else
            {
                loValue = base.GetValue(loScope, lsKey);
            }

            return loValue;
        }

        /// <summary>
        /// Gets the list of keys.
        /// </summary>
        /// <param name="loScope">Scope for the key list.</param>
        /// <returns>List of keys.</returns>
        public override string[] GetKeyList(MaxEnumGroup loScope)
        {
            string[] laR = new string[0];
            if (loScope.Equals(MaxEnumGroup.ScopeProcess))
            {
            }
            else if (loScope.Equals(MaxEnumGroup.ScopeSession))
            {
            }
            else if (loScope.Equals(MaxEnumGroup.ScopeProfile))
            {
            }
            else if (loScope.Equals(MaxEnumGroup.ScopeUser))
            {
            }

            return base.GetKeyList(loScope);
        }

        protected string GetProfileIdFromCookie()
        {
            string lsR = string.Empty;
            if (this.HasRequest)
            {
                for (int lnC = 0; lnC < HttpContext.Current.Request.Cookies.Count; lnC++)
                {
                    HttpCookie loCookie = HttpContext.Current.Request.Cookies[lnC];
                    if (loCookie.Name == this._sProfileCookieKey)
                    {
                        string lsProfileCookieText = loCookie.Value;
                        if (lsProfileCookieText.Contains("\\"))
                        {
                            string[] laValue = lsProfileCookieText.Split(new char[] { '\\' });
                            if (string.IsNullOrEmpty(lsR) || laValue[0].Equals(HttpContext.Current.Request.Url.Host, StringComparison.InvariantCultureIgnoreCase))
                            {
                                lsR = laValue[1];
                            }
                        }
                        else
                        {
                            lsR = lsProfileCookieText;
                        }
                    }
                }
            }

            return lsR;
        }

        /// <summary>
        /// Gets the current Profile Id
        /// </summary>
        /// <returns>Id of the Profile</returns>
        protected override Guid GetCurrentProfileId()
        {
            if (null != HttpContext.Current &&
                null != HttpContext.Current.Items)
            {
                object loObject = this.GetValue(MaxEnumGroup.ScopeProcess, this._sProcessProfileIdKey);
                if (null != loObject && loObject is Guid)
                {
                    return (Guid)loObject;
                }

                Guid loId = Guid.Empty;
                if (this.HasRequest)
                {
                    try
                    {
                        //// Check the querystring for the ProfileId
                        string lsProfileId = HttpContext.Current.Request.QueryString["MaxProfileId"];
                        string lsProfileIdFromCookie = this.GetProfileIdFromCookie();
                        if (string.IsNullOrEmpty(lsProfileId) &&
                            !string.IsNullOrEmpty(lsProfileIdFromCookie))
                        {
                            //// use the profile id from the cookie
                            lsProfileId = lsProfileIdFromCookie;
                        }

                        if (!string.IsNullOrEmpty(lsProfileId))
                        {
                            loId = MaxConvertLibrary.ConvertToGuid(typeof(object), lsProfileId);
                        }

                        if (Guid.Empty.Equals(loId))
                        {
                            loId = Guid.NewGuid();
                        }

                        this.SetValue(MaxEnumGroup.ScopeProcess, this._sProcessProfileIdKey, loId);

                        //// Set the profile Id in the cookie, or make sure it is saved in the database since it came from a cookie
                        string lsProfileCookieValue = loId.ToString().Replace("-", string.Empty);
                        if (null == this.GetValue(MaxEnumGroup.ScopeProcess, "ProfileCookieAdded") &&
                            (string.IsNullOrEmpty(lsProfileIdFromCookie)))
                        {
                            try
                            {
                                bool lbCookieSent = false;
                                foreach (string lsName in HttpContext.Current.Response.Cookies.AllKeys)
                                {
                                    if (lsName.Equals(this._sProfileCookieKey))
                                    {
                                        lbCookieSent = true;
                                    }
                                }

                                if (!lbCookieSent)
                                {
                                    string lsValue = lsProfileCookieValue;
                                    string lsHost = string.Empty;
                                    try
                                    {
                                        if (HttpContext.Current.Request.Url.Host.Contains("."))
                                        {
                                            lsValue = HttpContext.Current.Request.Url.Host + "\\" + lsValue;
                                            lsHost = HttpContext.Current.Request.Url.Host;
                                        }
                                    }
                                    catch (Exception loE)
                                    {
                                        MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error getting host name for cookie", loE));
                                    }

                                    HttpCookie loCookie = new HttpCookie(this._sProfileCookieKey, lsValue);
                                    loCookie.Expires = DateTime.UtcNow.AddYears(1);
                                    loCookie.Domain = lsHost;

                                    HttpContext.Current.Response.Cookies.Add(loCookie);
                                    this.SetValue(MaxEnumGroup.ScopeProcess, "ProfileCookieAdded", true);
                                }
                            }
                            catch (Exception loESetProfileCookie)
                            {
                                MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error setting profile cookie", loESetProfileCookie));
                            }
                        }

                        return loId;
                    }
                    catch (Exception loEProfileCookie)
                    {
                        MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error getting profile cookie", loEProfileCookie));
                    }
                }
            }

            return base.GetCurrentProfileId();
        }

        protected bool IsProfileConfirmed(Guid loProfileId)
        {
            bool lbR = false;
            if (null != HttpContext.Current &&
                !Guid.Empty.Equals(loProfileId) &&
                this.HasRequest)
            {
                try
                {
                    string lsLastBrowserInfo = HttpContext.Current.Request.UserHostAddress + "|" + HttpContext.Current.Request.UserAgent;

                    //// Check the querystring for the ProfileId
                    string lsProfileId = HttpContext.Current.Request.QueryString["MaxProfileId"];
                    if (string.IsNullOrEmpty(lsProfileId) &&
                        null != HttpContext.Current.Request.Cookies[this._sProfileCookieKey] &&
                        null == this.GetValue(MaxEnumGroup.ScopeProcess, "ProfileCookieAdded"))
                    {
                        //// Check the cookie for the ProfileId
                        lsProfileId = this.GetProfileIdFromCookie();;
                    }

                    if (!string.IsNullOrEmpty(lsProfileId))
                    {
                        try
                        {
                            Guid loId = MaxConvertLibrary.ConvertToGuid(typeof(object), lsProfileId);
                            if (loId.Equals(loProfileId))
                            {
                                string lsKey = "LastBrowserInfo";
                                string lsValue = MaxProfileIndexEntity.Create().GetValue(loProfileId, lsKey) as string;
                                if (null != lsValue)
                                {
                                    if (lsValue != lsLastBrowserInfo)
                                    {
                                        MaxProfileIndexEntity.Create().SaveValue(loProfileId, lsKey, lsLastBrowserInfo);
                                    }
                                }

                                lbR = true;
                            }
                        }
                        catch (Exception loEProfileId)
                        {
                            MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error getting profile configuration.", loEProfileId));
                        }
                    }
                }
                catch (Exception loE)
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error getting configuration.", loE));
                }
            }

            return lbR;
        }

        private static Type _oExecutingType = null;

        /// <summary>
        /// Gets a type in the executing assembly
        /// </summary>
        /// <returns>Type in executing assembly</returns>
        protected override Type GetExecutingTypeConditional()
        {
            if (null == _oExecutingType)
            {
                Assembly loAssembly = Assembly.GetEntryAssembly();
                if (null == loAssembly)
                {
                    Assembly[] loAssemblyList = AppDomain.CurrentDomain.GetAssemblies();
                    string lsCurrentAssembly = string.Empty;
                    if (null != MaxFactryLibrary.GetValue("CurrentAssemblyName") && MaxFactryLibrary.GetValue("CurrentAssemblyName") is string)
                    {
                        lsCurrentAssembly = (string)MaxFactryLibrary.GetValue("CurrentAssemblyName");
                        foreach (Assembly loAssemblyTest in loAssemblyList)
                        {
                            if (loAssemblyTest.ManifestModule.Name.Equals(lsCurrentAssembly, StringComparison.InvariantCultureIgnoreCase) &&
                                loAssemblyTest.GetTypes().Length > 0)
                            {
                                loAssembly = loAssemblyTest;
                            }
                        }
                    }
                }

                if (null == loAssembly)
                {
                    loAssembly = Assembly.GetExecutingAssembly();
                }

                if (null != loAssembly && loAssembly.GetTypes().Length > 0)
                {
                    _oExecutingType = loAssembly.GetTypes()[0];
                }
            }

            return _oExecutingType;
        }

    }
}