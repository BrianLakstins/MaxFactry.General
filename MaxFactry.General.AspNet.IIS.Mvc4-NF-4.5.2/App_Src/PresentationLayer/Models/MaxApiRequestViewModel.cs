﻿// <copyright file="MaxApiRequestViewModel.cs" company="Lakstins Family, LLC">
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

    /// <summary>
    /// View model for parsing a request to any api call
    /// </summary>
    public class MaxApiRequestViewModel
    {
        private Guid _oId = Guid.Empty;

        private HttpRequestMessage _oRequest = null;

        private MembershipUser _oUser = null;

        private List<string> _oRoleList = null;

        public MaxApiRequestViewModel(HttpRequestMessage loRequest)
        {
            _oRequest = loRequest;
        }

        public Guid Id
        {
            get
            {
                if (Guid.Empty == this._oId)
                {
                    if (null != this.Item && this.Item.Contains("Id"))
                    {
                        this._oId = MaxConvertLibrary.ConvertToGuid(typeof(object), this.Item["Id"]);
                    }
                }

                return this._oId;
            }
        }

        public string[] RequestFieldList { get; set; }

        public string[] ResponseFieldList { get; set; }

        public string Content { get; set; }

        public string AccessToken { get; set; }

        public MaxIndex Item { get; set; }

        public HttpRequestMessage Request
        {
            get
            {
                return _oRequest;
            }
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

        public MembershipUser User
        {
            get
            {
                if (null == this._oUser)
                {
                    this._oUser = Membership.GetUser();
                    if (null == this._oUser)
                    {
                        string lsClientToken = this.AccessToken;
                        if (null != this._oRequest.Headers.Authorization && this._oRequest.Headers.Authorization.Scheme == "Bearer")
                        {
                            lsClientToken = this._oRequest.Headers.Authorization.Parameter;
                        }

                        if (!string.IsNullOrEmpty(lsClientToken))
                        {
                            MaxUserAuthTokenEntity loTokenEntity = MaxUserAuthTokenEntity.GetByToken(lsClientToken);
                            if (null != loTokenEntity && loTokenEntity.IsActive && loTokenEntity.TokenType == "Bearer" && DateTime.UtcNow < loTokenEntity.CreatedDate.AddSeconds(loTokenEntity.Expiration))
                            {
                                Guid loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), loTokenEntity.UserKey);
                                this._oUser = Membership.GetUser(loUserId);
                            }
                        }
                    }
                }

                return this._oUser;
            }
        }
    }
}
