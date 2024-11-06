// <copyright file="MaxApiRequestViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="12/10/2020" author="Brian A. Lakstins" description="Initial creation">
// <change date="12/19/2020" author="Brian A. Lakstins" description="Add accesstoken to help in checking security for a user">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class.  Restructing to use DataKey">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Updated to not use default value for DataKey.  Only use if specified.">
// <change date="11/6/2024" author="Brian A. Lakstins" description="Updated token integration">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Net.Http;
    using System.Web.Security;
    using System.Collections.Generic;
    using MaxFactry.Core;
    using MaxFactry.General.BusinessLayer;
    using System.Xml.Linq;

    /// <summary>
    /// View model for parsing a request to any api call
    /// </summary>
    public class MaxApiRequestViewModel
    {
        private HttpRequestMessage _oRequest = null;

        private MembershipUser _oUser = null;

        private List<string> _oRoleList = null;

        private const string _sDataKey = "DataKey";

        private const string _sId = "Id";

        public MaxApiRequestViewModel(HttpRequestMessage loRequest)
        {
            _oRequest = loRequest;
        }

        public string GetDataKey(int lnNum)
        {
            string lsR = string.Empty;
            if (lnNum < 0)
            {
                lsR = this.Item.GetValueString(_sDataKey);
            }
            else if (this.Item.Contains(_sDataKey + "[" + lnNum + "]"))
            {
                lsR = this.Item.GetValueString(_sDataKey + "[" + lnNum + "]");
            }

            if (string.IsNullOrEmpty(lsR))
            {
                if (lnNum < 0)
                {
                    lsR = this.Item.GetValueString(_sId);
                }
                else if (this.Item.Contains(_sId + "[" + lnNum + "]"))
                {
                    lsR = this.Item.GetValueString(_sId + "[" + lnNum + "]");
                }

                if (!string.IsNullOrEmpty(lsR))
                {
                    Guid loR = MaxConvertLibrary.ConvertToGuid(typeof(object), lsR);
                    if (loR != Guid.Empty)
                    {
                        lsR = loR.ToString();
                    }
                    else
                    {
                        lsR = string.Empty;
                    }
                }
            }

            return lsR;
        }

        public string[] RequestPropertyList { get; set; }

        public string[] ResponseFilterList { get; set; }

        public string[] ResponsePropertyList { get; set; }

        public string Content { get; set; }

        public string AccessToken { get; set; }

        public MaxIndex Item { get; set; }

        public HttpRequestMessage GetRequestMessage()
        {
            return _oRequest;
        }

        public List<string> RoleList
        {
            get
            {
                if (null == _oRoleList)
                {
                    this._oRoleList = new List<string>();
                    if (null != this.User && !string.IsNullOrEmpty(this.User.UserName))
                    {
                        this._oRoleList.AddRange(Roles.GetRolesForUser(this.User.UserName));
                    }
                }

                return _oRoleList;
            }
        }

        public MaxUserAuthTokenEntity Token
        {
            get
            {
                MaxUserAuthTokenEntity loR = null;
                string lsClientToken = this.AccessToken;
                if (null != this._oRequest.Headers.Authorization && this._oRequest.Headers.Authorization.Scheme == "Bearer")
                {
                    lsClientToken = this._oRequest.Headers.Authorization.Parameter;
                }

                if (!string.IsNullOrEmpty(lsClientToken))
                {
                    MaxUserAuthTokenEntity loUserAuthToken = MaxUserAuthTokenEntity.GetByToken(lsClientToken);
                    if (null != loUserAuthToken && loUserAuthToken.IsActive && loUserAuthToken.TokenType == "Bearer" && DateTime.UtcNow < loUserAuthToken.CreatedDate.AddSeconds(loUserAuthToken.Expiration))
                    {
                        loR = loUserAuthToken;
                    }
                }

                return loR;
            }
        }

        public MembershipUser User
        {
            get
            {
                if (null == this._oUser)
                {
                    this._oUser = Membership.GetUser();
                    if (null == this._oUser && null != this.Token)
                    {
                        Guid loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), this.Token.UserKey);
                        this._oUser = Membership.GetUser(loUserId);
                    }
                }

                return this._oUser;
            }
        }
    }
}
