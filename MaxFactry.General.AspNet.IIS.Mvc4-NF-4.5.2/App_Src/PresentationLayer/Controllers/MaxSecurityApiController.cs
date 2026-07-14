// <copyright file="MaxSecurityApiController.cs" company="Lakstins Family, LLC">
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
// <change date="11/28/2020" author="Brian A. Lakstins" description="Initial creation">
// <change date="12/10/2020" author="Brian A. Lakstins" description="Update ResetPassword process">
// <change date="12/15/2020" author="Brian A. Lakstins" description="Update to follow consistent patterns">
// <change date="12/16/2020" author="Brian A. Lakstins" description="Add status codes based on authentication and authorization of actions">
// <change date="12/18/2020" author="Brian A. Lakstins" description="Adding ability to update user and roles through api">
// <change date="1/17/2021" author="Brian A. Lakstins" description="Don't include roles in user list because of so many database queries">
// <change date="1/19/2021" author="Brian A. Lakstins" description="Fix issue getting user by email address">
// <change date="2/24/2021" author="Brian A. Lakstins" description="Update password reset process.">
// <change date="6/19/2024" author="Brian A. Lakstins" description="Add user related logging.  Return configuration informaion on user requests.">
// <change date="6/28/2024" author="Brian A. Lakstins" description="Add generic User and Role management.  Integrate permission management">
// <change date="7/2/2024" author="Brian A. Lakstins" description="Use GetPermisison method from base to reduce code">
// <change date="7/10/2024" author="Brian A. Lakstins" description="Include permissions with roles">
// <change date="7/16/2024" author="Brian A. Lakstins" description="Set some attributes based on time.">
// <change date="8/26/2024" author="Brian A. Lakstins" description="Updated for changes to base class.">
// <change date="9/16/2024" author="Brian A. Lakstins" description="Add a way to get properties of the user">
// <change date="11/6/2024" author="Brian A. Lakstins" description="Updated token integration">
// <change date="11/19/2025" author="Brian A. Lakstins" description="Allow a user with proper permission to set a users's password">
// <change date="1/27/2025" author="Brian A. Lakstins" description="Add auth type when logging in">
// <change date="4/22/2025" author="Brian A. Lakstins" description="Use Login with GET to get current user">
// <change date="6/4/2025" author="Brian A. Lakstins" description="Fix error message when not needed.">
// <change date="6/10/2025" author="Brian A. Lakstins" description="Fix updating role to user relationships.">
// <change date="6/11/2025" author="Brian A. Lakstins" description="Update for ApplicationKey">
// <change date="8/1/2025" author="Brian A. Lakstins" description="Add error checking for resetting password.">
// <change date="10/17/2025" author="Brian A. Lakstins" description="Fix updating role related permissions">
// <change date="11/4/2025" author="Brian A. Lakstins" description="Swap POST and PUT">
// <change date="11/18/2025" author="Brian A. Lakstins" description="Update fingerprint for post">
// <change date="11/20/2025" author="Brian A. Lakstins" description="Updating Put and Post checks for update and insert">
// <change date="3/18/2026" author="Brian A. Lakstins" description="Consolidate login code.  Add logging in based on Authentication header.">
// <change date="5/21/2026" author="Brian A. Lakstins" description="Standardize User Token management.  Add Client Auth management.">
// <change date="6/23/2026" author="Brian A. Lakstins" description="Update filter usage.">
// <change date="6/23/2026" author="Brian A. Lakstins" description="Allow Admins to update user tokens">
// <change date="7/7/2026" author="Brian A. Lakstins" description="Update filtering">
// <change date="7/8/2026" author="Brian A. Lakstins" description="Filtering adjustments for changes to references">
// <change date="7/14/2026" author="Brian A. Lakstins" description="Store just one record per PermissionId instead of one for each Permission">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{

    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.Core;
    using MaxFactry.General.AspNet.PresentationLayer;
    using MaxFactry.General.BusinessLayer;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Security;
    using System.Xml.Linq;

    [MaxRequireHttps(Order = 1)]
    [System.Web.Http.AllowAnonymous]
    public class MaxSecurityApiController : MaxBaseApiController
    {
        protected override bool HasPermission(MaxApiRequestViewModel loRequest, MaxEntity loEntity, int lnPermission)
        {
            bool lbR = base.HasPermission(loRequest, loEntity, lnPermission);
            if (!lbR)
            {
                try
                {
                    Guid loUserId = this.GetUserId(loRequest);
                    if (loEntity is MaxUserAuthTokenEntity && Guid.Empty != loUserId)
                    {
                        MaxUserAuthTokenEntity loSecurityCheckEntity = MaxUserAuthTokenEntity.Create();
                        if (!string.IsNullOrEmpty(loEntity.DataKey) && loEntity.DataKey != Guid.Empty.ToString() && loSecurityCheckEntity.LoadByDataKeyCache(loEntity.DataKey))
                        {
                            if (loSecurityCheckEntity.UserKey == loUserId.ToString())
                            {
                                lbR = true;
                            }
                        }
                        else
                        {
                            lbR = true;
                        }
                    }
                }
                catch (Exception loE)
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "HasPermission", MaxEnumGroup.LogError, "Exception checking permission", loE));
                }
            }

            return lbR;
        }


        [HttpGet]
        [HttpPost]
        [ActionName("index")]
        public async Task<HttpResponseMessage> Index()
        {
            Guid loId = Guid.NewGuid();
            string lsR = string.Empty;

            lsR = string.Empty;
            try
            {
                Stream loContent = await this.Request.Content.ReadAsStreamAsync();
                StreamReader loReader = new StreamReader(loContent);
                lsR = loReader.ReadToEnd();
                loReader.Close();
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "General API Error.", loE));
            }

            lsR = MaxDataLibrary.GetApplicationKey();

            HttpResponseMessage loR = new HttpResponseMessage();
            loR.Content = new StringContent(lsR);
            loR.Content.Headers.Remove("Content-Type");
            loR.Content.Headers.Add("Content-Type", "text/plain");

            MaxOwinLibrary.SetStorageKeyForClient(HttpContext.Current.Request.Url, false, string.Empty);
            return loR;
        }

        /// <summary>
        /// Used for server to server access to get a token to impersonate a certain user based on MaxUserAuthEntity created that is associated with that user
        /// 
        /// PrettyMail takes the code and directly (i.e., not via a REDIRECT in the user’s browser, but via a server-to-server request) 
        /// queries GMail with both code and shared secret, to prove its identity: 
        /// GET gmail.com/oauth2/token?client_id=ABC&client_secret=XYZ&code=big_long_thing
        /// client_id
        /// client_secret
        /// code
        /// 
        /// https://www.oauth.com/oauth2-servers/access-tokens/authorization-code-request/
        /// grant_type=authorization_code
        /// code
        /// redirect_uri
        /// client_id
        /// client_secret
        /// From: https://alexbilbie.com/guide-to-oauth-2-grants/ - Authorisation Code Grant (section 4.1) - The Flow (Part Two)
        /// grant_type=authorization_code
        /// client_id 
        /// client_secret 
        /// redirect_uri 
        /// code 
        /// Return JSON object
        /// token_type=Bearer
        /// expires_in -> with an integer representing the TTL of the access token (i.e. when the token will expire)
        /// access_token -> the access token itself
        /// refresh_token -> a refresh token that can be used to acquire a new access token when the original expires
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [ActionName("oauth2")]
        public async Task<HttpResponseMessage> OAuth2()
        {
            string lsR = string.Empty;
            string lsContent = string.Empty;
            try
            {
                Stream loContent = await this.Request.Content.ReadAsStreamAsync();
                StreamReader loReader = new StreamReader(loContent);
                lsContent = loReader.ReadToEnd();
                loReader.Close();
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "General API Error.", loE));
            }

            //// grant_type=authorization_code&client_id=client1&client_secret=secret1&redirect_uri=https%253A%252F%252Foauthdebugger.com%252Fdebug
            NameValueCollection loQueryString = HttpUtility.ParseQueryString(this.Request.RequestUri.Query);
            if (!string.IsNullOrEmpty(lsContent))
            {
                loQueryString = HttpUtility.ParseQueryString(lsContent);
            }

            string lsGrantType = loQueryString["grant_type"];
            string lsClientId = loQueryString["client_id"];
            string lsClientSecret = loQueryString["client_secret"];
            string lsRedirectUri = loQueryString["redirect_uri"];
            string lsCode = loQueryString["code"];

            //// TODO: Check format of all content

            if (lsGrantType == "authorization_code")
            {
                MaxUserAuthEntity loEntity = MaxUserAuthEntity.Create();
                MaxEntityList loList = loEntity.LoadAllByClientIdCache(lsClientId);
                for (int lnE = 0; lnE < loList.Count && lsR == string.Empty; lnE++)
                {
                    loEntity = loList[lnE] as MaxUserAuthEntity;
                    if (loEntity.IsActive)
                    {
                        //// TODO: Check scope?
                        //// TODO: Check domainlist?
                        if (loEntity.ClientSecret == lsClientSecret)
                        {
                            MaxUserAuthGrantEntity loGrantEntity = MaxUserAuthGrantEntity.Create();
                            Guid loGrantId = MaxConvertLibrary.ConvertAlphabet64ToGuid(typeof(object), lsCode);
                            if (loGrantEntity.LoadByDataKeyCache(loGrantId.ToString()) && loGrantEntity.UserAuthId == loEntity.Id && loGrantEntity.IsActive && loGrantEntity.RedirectUri == lsRedirectUri)
                            {
                                string lsToken = MaxUserAuthTokenEntity.GenerateToken(false);
                                string lsTokenType = "Bearer";
                                MaxUserAuthTokenEntity loTokenEntity = MaxUserAuthTokenEntity.AddToken(lsToken, lsTokenType, DateTime.UtcNow.AddYears(1), loGrantEntity.UserKey, loGrantEntity.Id, loEntity.Id);
                                string lsAccessToken = loTokenEntity.GetClientToken(lsToken);
                                int lnExpiresIn = loTokenEntity.Expiration;

                                lsToken = MaxUserAuthTokenEntity.GenerateToken(false);
                                loTokenEntity = MaxUserAuthTokenEntity.AddToken(lsToken, "Refresh", DateTime.UtcNow.AddYears(50), loGrantEntity.UserKey, loGrantEntity.Id, loEntity.Id);
                                string lsRefreshToken = loTokenEntity.GetClientToken(lsToken);

                                //// Send token to client in proper format
                                var loToken = new
                                {
                                    token_type = lsTokenType,
                                    expires_in = lnExpiresIn,
                                    access_token = lsAccessToken,
                                    refresh_token = lsRefreshToken
                                };

                                lsR = MaxConvertLibrary.SerializeObjectToString(typeof(object), loToken);
                            }
                        }
                    }
                }
            }
            else if (lsGrantType == "refresh_token")
            {
                //// TODO: Generate a new token using the refesh token info
            }
            else
            {
                //// TODO : Generate invalid response
                ////  https://www.oauth.com/oauth2-servers/access-tokens/access-token-response/

            }


            /*
            gmail verifies and then invalidates the code, and responds with an AccessToken, 
            which PrettyMail can then use (until it expires) to make API requests on the user’s behalf 
            (i.e., to get info about the user, and possibly to perform actions on behalf of the user, if that was in the agreed-upon ‘scope’ of the arrangement)
            */

            HttpResponseMessage loR = new HttpResponseMessage();
            loR.Content = new StringContent(lsR);
            loR.Content.Headers.Remove("Content-Type");
            loR.Content.Headers.Add("Content-Type", "application/json");
            return loR;
        }

        /// <summary>
        /// User management CRUD operations.  This is used for server to server management of users.  It is not intended to be used by a client application directly.  The login action should be used by client applications to log in and manage the current user.
        /// Called Users instead of User to not interfere with this.User property of the controller.  The User property is used to get the current user making the request and Users action is used to manage users in general.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        [HttpPut]
        [HttpDelete]
        [HttpOptions]
        [ActionName("users")]
        public async Task<HttpResponseMessage> Users()
        {
            //this.User is a thing that is used to get the current user making the request.  Don't name an action User because it will interfere with that.  Using Users instead.
            MaxEntity loEntity = MaxUserEntity.Create();
            MaxApiResponseViewModel loR = await this.Process(loEntity);
            return this.GetResponseMessage(loR);
        }

        /// <summary>
        /// CRUD template for an entity
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        [HttpPut]
        [HttpDelete]
        [HttpOptions]
        [ActionName("userrole")]
        public async Task<HttpResponseMessage> UserRole()
        {
            MaxEntity loEntity = MaxRoleEntity.Create();
            MaxApiResponseViewModel loR = await this.Process(loEntity);
            return this.GetResponseMessage(loR);
        }

        /// <summary>
        /// CRUD template for an entity
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        [HttpPut]
        [HttpDelete]
        [HttpOptions]
        [ActionName("usertoken")]
        public async Task<HttpResponseMessage> UserToken()
        {
            MaxEntity loEntity = MaxUserAuthTokenEntity.Create();
            MaxApiResponseViewModel loR = await this.Process(loEntity);
            return this.GetResponseMessage(loR);
        }

        /// <summary>
        /// CRUD template for an entity
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        [HttpPut]
        [HttpDelete]
        [HttpOptions]
        [ActionName("authclient")]
        public async Task<HttpResponseMessage> AuthClient()
        {
            MaxEntity loEntity = MaxAuthClientEntity.Create();
            MaxApiResponseViewModel loR = await this.Process(loEntity);
            return this.GetResponseMessage(loR);
        }
       
        /// <summary>
        /// Attempts to log the user into the application
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPut]
        [HttpOptions]
        [ActionName("login")]
        public async Task<HttpResponseMessage> Login()
        {
            var loResponseItem = new
            {
                Id = "Id",
                Email = "Email",
                UserName = "UserName",
                RoleList = "RoleList",
                LastPasswordChangedDate = "LastPasswordChangedDate",
                Comment = "Comment",
                LastActivityDate = "LastActivityDate",
                LastLoginDate = "LastLoginDate",
                PasswordQuestion = "PasswordQuestion",
                IsPasswordResetNeeded = "IsPasswordResetNeeded",
                UserConfiguration = "UserConfiguration",
                Token = "Token"
            };

            MaxApiResponseViewModel loR = GetResponse(loResponseItem);
            if (this.Request.Method != HttpMethod.Options)
            {
                MaxUserConfigurationEntity loConfiguration = MaxUserConfigurationEntity.Create().GetCurrent();
                loR.Item.Add(loResponseItem.UserConfiguration, loConfiguration.MapIndex(
                        loConfiguration.GetPropertyName(() => loConfiguration.OnlineWindowDuration),
                        loConfiguration.GetPropertyName(() => loConfiguration.MinRequiredNonAlphanumericCharacters),
                        loConfiguration.GetPropertyName(() => loConfiguration.MinRequiredPasswordLength),
                        loConfiguration.GetPropertyName(() => loConfiguration.PasswordStrengthRegularExpression),
                        loConfiguration.GetPropertyName(() => loConfiguration.ApplicationName),
                        loConfiguration.GetPropertyName(() => loConfiguration.EnablePasswordReset),
                        loConfiguration.GetPropertyName(() => loConfiguration.EnablePasswordRetrieval)
                    ));

                var loRequestItem = new
                {
                    UserName = "UserName",
                    Email = "Email",
                    Password = "Password"
                };

                MaxApiRequestViewModel loRequest = await this.GetRequest();
                try
                {
                    bool lbIsValid = false;
                    MembershipUser loUser = loRequest.User;
                    if (this.Request.Method == HttpMethod.Put)
                    {
                        string lsUserName = loRequest.Item.GetValueString(loRequestItem.UserName);
                        string lsEmail = loRequest.Item.GetValueString(loRequestItem.Email);
                        string lsPassword = loRequest.Item.GetValueString(loRequestItem.Password);
                        if ((!string.IsNullOrEmpty(lsUserName) || !string.IsNullOrEmpty(lsEmail)) && !string.IsNullOrEmpty(lsPassword))
                        {
                            if (string.IsNullOrEmpty(lsEmail) && MaxBaseEmailEntity.IsValidEmail(lsUserName))
                            {
                                lsEmail = lsUserName;
                            }

                            string lsUserCheck = lsUserName;
                            lbIsValid = Membership.ValidateUser(lsUserCheck, lsPassword);
                            if (!lbIsValid && !string.IsNullOrEmpty(lsEmail) && MaxBaseEmailEntity.IsValidEmail(lsEmail))
                            {
                                lsUserCheck = Membership.GetUserNameByEmail(lsEmail);
                                if (!string.IsNullOrEmpty(lsUserCheck))
                                {
                                    lbIsValid = Membership.ValidateUser(lsUserCheck, lsPassword);
                                }
                                else
                                {
                                    lsUserCheck = lsUserName;
                                }
                            }

                            if (!string.IsNullOrEmpty(lsUserCheck))
                            {
                                MembershipUser loUserCheck = Membership.GetUser(lsUserCheck);
                                if (lbIsValid)
                                {
                                    MaxSecurityLoginViewModel loLoginModel = new MaxSecurityLoginViewModel();
                                    Guid loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), loUserCheck.ProviderUserKey);
                                    string lsUserNameLoggedIn = loLoginModel.LoginUser(loUserId, "Login-Api");
                                    if (!string.IsNullOrEmpty(lsUserNameLoggedIn))
                                    {
                                        loUser = Membership.GetUser(lsUserNameLoggedIn);                                        
                                    }
                                    else
                                    {
                                        loR.Message.Error = "User login failed.";
                                    }
                                }
                                else
                                {
                                    loR.Message.Error = "A matching username or password was not found.";
                                    if (null != loUserCheck)
                                    {
                                        try
                                        {
                                            string lsPasswordCurrent = loUserCheck.GetPassword();
                                            if (string.IsNullOrEmpty(lsPasswordCurrent))
                                            {
                                                loR.Message.Error = "Password needs to be reset.";
                                            }
                                        }
                                        catch (Exception loE)
                                        {
                                            if (loE.Message != "Password retrieval is disabled" && loE.Message != "Password is hashed and cannot be retrieved.")
                                            {
                                                loR.Message.Error = "Exception logging in a user: " + loE.Message;
                                                MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "Login", MaxEnumGroup.LogError, "Exception logging in a user", loE));
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                loR.Message.Error = "A matching username or password cannot be found.";
                            }
                        }
                        else if (string.IsNullOrEmpty(lsPassword))
                        {
                            loR.Message.Error = "Password is required.";
                        }
                        else
                        {
                            loR.Message.Error = "Username or email is required.";
                        }
                    }
                    else if (this.Request.Method == HttpMethod.Get && null != this.Request.Headers.Authorization)
                    {
                        string lsIdToken = this.Request.Headers.Authorization.Parameter;
                        if (!string.IsNullOrEmpty(lsIdToken))
                        {
                            MaxSecurityLoginViewModel loModel = new MaxSecurityLoginViewModel();
                            if (loModel.IsValidIdToken(lsIdToken) && loModel.ValidateTokenSignature(lsIdToken))
                            {
                                IDictionary<string, object> loIdToken = loModel.ParseToken(lsIdToken);
                                string lsEmail = loModel.GetEmail(loIdToken);
                                string lsUserName = loModel.GetUserName(loIdToken);
                                string lsUserNameLoggedIn = loModel.LoginUser(lsUserName, lsEmail, "OAuth2 Token");
                                if (!string.IsNullOrEmpty(lsUserNameLoggedIn))
                                {
                                    loUser = Membership.GetUser(lsUserNameLoggedIn);
                                }
                            }
                        }
                    }

                    if (null != loUser && !string.IsNullOrEmpty(loUser.UserName))
                    {
                        if (loUser is MaxMembershipUser)
                        {
                            MaxIndex loUserIndex = ((MaxMembershipUser)loUser).GetUser().MapIndex(loRequest.ResponsePropertyList);
                            string[] laKey = loUserIndex.GetSortedKeyList();
                            foreach (string lsKey in laKey)
                            {
                                loR.Item.Add(lsKey, loUserIndex[lsKey]);
                            }
                        }

                        //// Return logged in user
                        loR.Item.Add(loResponseItem.Id, MaxConvertLibrary.ConvertToString(typeof(object), loUser.ProviderUserKey).ToLower());
                        loR.Item.Add(loResponseItem.Email, loUser.Email);
                        loR.Item.Add(loResponseItem.UserName, loUser.UserName);
                        loR.Item.Add(loResponseItem.LastPasswordChangedDate, loUser.LastPasswordChangedDate);
                        loR.Item.Add(loResponseItem.Comment, loUser.Comment);
                        loR.Item.Add(loResponseItem.LastActivityDate, loUser.LastActivityDate);
                        loR.Item.Add(loResponseItem.LastLoginDate, loUser.LastLoginDate);
                        loR.Item.Add(loResponseItem.PasswordQuestion, loUser.PasswordQuestion);
                        if (loUser is MaxMembershipUser)
                        {
                            loR.Item.Add(loResponseItem.IsPasswordResetNeeded, ((MaxMembershipUser)loUser).IsPasswordResetNeeded);
                        }

                        List<MaxIndex> loRoleIndexList = new List<MaxIndex>();
                        MaxEntityList loRoleList = MaxRoleEntity.Create().LoadAllByUserIdCache(MaxConvertLibrary.ConvertToGuid(typeof(object), loUser.ProviderUserKey));
                        for (int lnR = 0; lnR < loRoleList.Count; lnR++)
                        {
                            MaxRoleEntity loRole = loRoleList[lnR] as MaxRoleEntity;
                            MaxIndex loRoleIndex = loRole.MapIndex(
                                loRole.GetPropertyName(() => loRole.DataKey),
                                loRole.GetPropertyName(() => loRole.RoleName),
                                loRole.GetPropertyName(() => loRole.Id),
                                "PermissionKeySelectedList");
                            loRoleIndexList.Add(loRoleIndex);
                        }

                        loR.Item.Add(loResponseItem.RoleList, loRoleIndexList);
                        if (null != loRequest.Token)
                        {
                            loR.Item.Add(loResponseItem.Token, loRequest.Token.MapIndex(
                                loRequest.Token.GetPropertyName(() => loRequest.Token.DataKey),
                                loRequest.Token.GetPropertyName(() => loRequest.Token.ExpirationDate),
                                loRequest.Token.GetPropertyName(() => loRequest.Token.TokenType),
                                loRequest.Token.GetPropertyName(() => loRequest.Token.CreatedDate),
                                loRequest.Token.GetPropertyName(() => loRequest.Token.LastUsedDate)
                                ));
                        }
                    }                     
                }
                catch (Exception loE)
                {
                    loR.Message.Error = "Exception logging in a user: " + loE.Message;
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "Login", MaxEnumGroup.LogError, "Exception logging in a user", loE));
                }
            }

            return this.GetResponseMessage(loR);
        }

        /// <summary>
        /// Logs the user out of the application
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpOptions]
        [ActionName("logout")]
        public async Task<HttpResponseMessage> Logout()
        {
            var loResponseItem = new
            {
            };

            MaxApiResponseViewModel loR = GetResponse(loResponseItem);
            if (this.Request.Method != HttpMethod.Options)
            {
                MaxApiRequestViewModel loRequest = await this.GetRequest();
                if (null != loRequest.User)
                {
                    Guid loUserId = Guid.Empty;
                    if (Guid.TryParse(loRequest.User.ProviderUserKey.ToString(), out loUserId) && loUserId != Guid.Empty)
                    {
                        MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
                        loMaxUserLog.Insert(
                            loUserId,
                            MaxUserLogEntity.LogEntryTypeLogout,
                            this.GetType() + ".Logout()");
                        MaxUserEntity loMaxUser = MaxUserEntity.Create();
                        if (loMaxUser.LoadByIdCache(loUserId))
                        {
                            loMaxUser.SetAttribute("_LastLogout", DateTime.UtcNow);
                            loMaxUser.Update();
                        }
                    }
                }

                try
                {
                    MaxFactry.General.AspNet.IIS.MaxAppLibrary.SignOut();
                    loR.Message.Success = "Logged out.";
                }
                catch (Exception loE)
                {
                    loR.Message.Error = "Exception logging out a user: " + loE.Message;
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "Logout", MaxEnumGroup.LogError, "Exception logging out a user", loE));
                }
            }

            return this.GetResponseMessage(loR);
        }

        [HttpGet]
        [HttpPut]
        [HttpOptions]
        [ActionName("resetpassword")]
        public async Task<HttpResponseMessage> ResetPassword()
        {
            var loResponseItem = new
            {
                CanReset = "CanReset"
            };

            MaxApiResponseViewModel loR = GetResponse(loResponseItem);
            if (this.Request.Method != HttpMethod.Options)
            {
                var loRequestItem = new
                {
                    UserName = "UserName",
                    Code = "Code",
                    NewPassword = "NewPassword",
                    NewPasswordConfirm = "NewPasswordConfirm"
                };

                MaxApiRequestViewModel loRequest = await this.GetRequest();
                try
                {
                    MaxSecurityResetPasswordViewModel loModel = new MaxSecurityResetPasswordViewModel();
                    loModel.UserName = loRequest.Item.GetValueString(loRequestItem.UserName);
                    loModel.Password = loRequest.Item.GetValueString(loRequestItem.NewPassword);
                    loModel.UserAuthCodeConfirm = loRequest.Item.GetValueString(loRequestItem.Code);
                    loModel.PasswordConfirm = loRequest.Item.GetValueString(loRequestItem.NewPasswordConfirm);

                    if (this.Request.Method == HttpMethod.Get)
                    {
                        loR.Message.Error = loModel.SendPasswordReset(string.Empty);
                        if (loR.Message.Error.Length == 0)
                        {
                            loR.Message.Success = "Password Reset email sent";
                            loR.Item[loResponseItem.CanReset] = true;
                        }
                    }
                    else if (this.Request.Method == HttpMethod.Put)
                    {
                        if (this.HasPermission(loRequest, MaxUserEntity.Create(), (int)MaxEnumGroup.PermissionUpdate))
                        {
                            MembershipUser loResetUser = Membership.GetUser(loModel.UserName);
                            if (null == loResetUser)
                            {
                                loR.Message.Error = "The user [" + loModel.UserName + " was not found.";
                            }
                            else if (string.IsNullOrEmpty(loModel.Password))
                            {
                                loR.Message.Error = "The new password is blank.";
                            }
                            else if (MaxMembershipUser.SetPassword(loResetUser, loModel.Password))
                            {
                                loR.Message.Error = string.Empty;
                                MaxUserEntity loUser = MaxUserEntity.Create();
                                string lsDataKey = loResetUser.ProviderUserKey.ToString();
                                if (loUser.LoadByDataKeyCache(lsDataKey))
                                {
                                    loUser.IsPasswordResetNeeded = true;
                                    loUser.Update();
                                }
                            }
                            else
                            {
                                loR.Message.Error = "There was an unknown error changing the password.";
                                if (Membership.MinRequiredPasswordLength > loModel.Password.Length)
                                {
                                    loR.Message.Error = "Minimum password length is " + Membership.MinRequiredPasswordLength;
                                }
                            }
                        }
                        else
                        {
                            loR.Message.Error = loModel.TryResetPassword();
                        }

                        if (loR.Message.Error.Length == 0)
                        {
                            loR.Message.Success = "The password was reset";
                        }
                    }
                }
                catch (Exception loE)
                {
                    loR.Message.Error = "Exception resetting password: " + loE.Message;
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "ResetPassword", MaxEnumGroup.LogError, "Exception resetting password", loE));
                }
            }

            return this.GetResponseMessage(loR);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPut]
        [HttpOptions]
        [ActionName("changepassword")]
        public async Task<HttpResponseMessage> ChangePassword()
        {
            HttpStatusCode loStatus = HttpStatusCode.Unauthorized;
            var loResponseItem = new
            {
            };

            MaxApiResponseViewModel loR = GetResponse(loResponseItem);
            if (this.Request.Method != HttpMethod.Options)
            {
                var loRequestItem = new
                {
                    Password = "Password",
                    NewPassword = "NewPassword",
                    NewPasswordConfirm = "NewPasswordConfirm"
                };

                MaxApiRequestViewModel loRequest = await this.GetRequest();
                try
                {
                    MembershipUser loUser = Membership.GetUser();
                    string lsPassword = loRequest.Item.GetValueString(loRequestItem.Password);
                    if (null != loUser && !string.IsNullOrEmpty(loUser.UserName))
                    {
                        loStatus = HttpStatusCode.OK;
                        if (Membership.ValidateUser(loUser.UserName, lsPassword))
                        {
                            string lsNewPassword = loRequest.Item.GetValueString(loRequestItem.NewPassword);
                            string lsNewPasswordConfirm = loRequest.Item.GetValueString(loRequestItem.NewPasswordConfirm);
                            if (lsNewPassword != lsNewPasswordConfirm)
                            {
                                loR.Message.Error = "The new password confirmation does not match the new password.";
                            }
                            else if (lsNewPassword.Length < 8)
                            {
                                loR.Message.Error = "The new password is too short.  It should be at least 8 characters long.";
                            }
                            else if (!loUser.ChangePassword(lsPassword, lsNewPassword))
                            {
                                loR.Message.Error = "Unknown error changing password";
                            }
                            else
                            {
                                loR.Message.Success = "The password was changed.";
                            }
                        }
                        else
                        {
                            loR.Message.Error = "Current password is not correct.";
                        }
                    }
                    else
                    {
                        loR.Message.Error = "The user is not logged in.";
                    }
                }
                catch (Exception loE)
                {
                    loR.Message.Error = "Exception changing password: " + loE.Message;
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "ChangePassword", MaxEnumGroup.LogError, "Exception changing password", loE));
                }
            }
            else
            {
                loStatus = HttpStatusCode.OK;
            }

            return this.GetResponseMessage(loR, loStatus);
        }

        protected override MaxApiResponseViewModel ProcessPut(MaxApiRequestViewModel loRequest, MaxEntity loOriginalEntity, MaxEntity loMappedEntity, MaxEntityList loMappedEntityList, MaxApiResponseViewModel loResponse)
        {
            if (loMappedEntity is MaxUserAuthTokenEntity && loOriginalEntity is MaxUserAuthTokenEntity)
            {
                //// Don't allow changing AdminUserKey or UserKey after first assignment
                MaxUserAuthTokenEntity loOriginalUserAuthTokenEntity = loOriginalEntity as MaxUserAuthTokenEntity;
                MaxUserAuthTokenEntity loMappedUserAuthTokenEntity = loMappedEntity as MaxUserAuthTokenEntity;               
                if (!string.IsNullOrEmpty(loOriginalUserAuthTokenEntity.AdminUserKey))
                {
                    loMappedUserAuthTokenEntity.AdminUserKey = loOriginalUserAuthTokenEntity.AdminUserKey;
                    loMappedUserAuthTokenEntity.UserKey = loOriginalUserAuthTokenEntity.UserKey;
                }            
            }

            MaxApiResponseViewModel loR = base.ProcessPut(loRequest, loOriginalEntity, loMappedEntity, loMappedEntityList, loResponse);
            if (loMappedEntity is MaxUserEntity)
            {
                loR = this.ProcessUser(loRequest, loMappedEntity as MaxUserEntity, loR);
            }
            else if (loMappedEntity is MaxRoleEntity)
            {
                loR = this.ProcessRole(loRequest, loMappedEntity as MaxRoleEntity, loR);
            }

            return loR;
        }

        protected override MaxApiResponseViewModel ProcessPost(MaxApiRequestViewModel loRequest, MaxEntity loOriginalEntity, MaxEntity loMappedEntity, MaxEntityList loMappedList, MaxApiResponseViewModel loResponse)
        {
            MaxApiResponseViewModel loR = base.ProcessPost(loRequest, loOriginalEntity, loMappedEntity, loMappedList, loResponse);
            if (loMappedEntity is MaxUserEntity)
            {
                loR = this.ProcessUser(loRequest, loMappedEntity as MaxUserEntity, loR);
            }
            else if (loMappedEntity is MaxRoleEntity)
            {
                loR = this.ProcessRole(loRequest, loMappedEntity as MaxRoleEntity, loR);
            }
            else if (loMappedEntity is MaxUserAuthTokenEntity)
            {
                loR.Item.Add("AccessToken", (loMappedEntity as MaxUserAuthTokenEntity).GetClientToken((loMappedEntity as MaxUserAuthTokenEntity).Token));
            }

            return loR;
        }

        protected MaxApiResponseViewModel ProcessUser(MaxApiRequestViewModel loRequest, MaxUserEntity loUser, MaxApiResponseViewModel loResponse)
        {
            MaxApiResponseViewModel loR = loResponse;
            if (null != loR && null != loR.Item && Guid.Empty != loUser.Id && null != loRequest.Item && loRequest.Item.Contains("RoleIdSelectedList"))
            {
                string[] laRoleIdSelectedList = MaxConvertLibrary.DeserializeObject(loRequest.Item.GetValueString("RoleIdSelectedList"), typeof(string[])) as string[];
                if (null != laRoleIdSelectedList)
                {
                    Guid loUserId = loUser.Id;
                    List<string> loRoleIdSelectedList = new List<string>(laRoleIdSelectedList);
                    if (Guid.Empty != loUserId)
                    {
                        MaxRoleRelationUserEntity loRelation = MaxRoleRelationUserEntity.Create();
                        MaxEntityList loRelationList = loRelation.LoadAllByUserIdCache(loUserId);
                        for (int lnR = 0; lnR < loRelationList.Count; lnR++)
                        {
                            loRelation = loRelationList[lnR] as MaxRoleRelationUserEntity;
                            if (loRoleIdSelectedList.Contains(loRelation.RoleId.ToString()))
                            {
                                loRoleIdSelectedList.Remove(loRelation.RoleId.ToString());
                            }
                            else
                            {
                                loRelation.Delete();
                            }
                        }

                        foreach (string lsRoleId in loRoleIdSelectedList)
                        {
                            loRelation = MaxRoleRelationUserEntity.Create();
                            loRelation.RoleId = MaxConvertLibrary.ConvertToGuid(typeof(object), lsRoleId);
                            loRelation.UserId = loUserId;
                            loRelation.Insert();
                        }

                        foreach (string lsRequestProperty in loRequest.RequestPropertyList)
                        {
                            if (lsRequestProperty == "RoleIdSelectedList" || lsRequestProperty == typeof(MaxUserEntity).ToString() + ".RoleIdSelectedList")
                            {
                                loRelationList = loRelation.LoadAllByUserIdCache(loUserId);
                                List<string> loRoleIdList = new List<string>();
                                for (int lnR = 0; lnR < loRelationList.Count; lnR++)
                                {
                                    loRelation = loRelationList[lnR] as MaxRoleRelationUserEntity;
                                    loRoleIdList.Add(loRelation.RoleId.ToString());
                                }

                                loR.Item.Add("RoleIdSelectedList", loRoleIdList.ToArray());
                            }
                        }
                    }
                }
            }

            return loR;
        }

        protected MaxApiResponseViewModel ProcessRole(MaxApiRequestViewModel loRequest, MaxRoleEntity loRole, MaxApiResponseViewModel loResponse)
        {
            MaxApiResponseViewModel loR = loResponse;
            if (null != loR && null != loR.Item && null != loRole && !(loRole.Id == Guid.Empty) && null != loRequest.Item && loRequest.Item.Contains("PermissionKeySelectedList"))
            {
                string[] laPermissionSelectedList = MaxConvertLibrary.DeserializeObject(loRequest.Item.GetValueString("PermissionKeySelectedList"), typeof(string[])) as string[];
                if (null != laPermissionSelectedList)
                {
                    //// The selected list has PermissionId|PermissionValue and may not have the "|".
                    //// The PermissionId is the unique identifier of the permission.
                    //// The PermissionValue is a bitwise value of the permissions assigned to the role.  
                    //// The client may send the same PermissionId multiple times with different PermissionValues.  The server will combine the values into a single value for each PermissionId.
                    List<string> loPermissionSelectedList = new List<string>(laPermissionSelectedList);
                    //// Create an index of the permissions that are selected to make it easier to check if a permission is already assigned to the role.
                    MaxIndex loPermissionIndex = new MaxIndex();
                    foreach (string lsPermissionKey in loPermissionSelectedList)
                    {
                        Guid loPermissionId = MaxConvertLibrary.ConvertToGuid(typeof(object), lsPermissionKey.Substring(0, Guid.Empty.ToString().Length));
                        string lsPermissionId = loPermissionId.ToString();
                        long lnPermission = long.MinValue;
                        if (lsPermissionKey.Contains("|"))
                        {
                            string[] laPermissionParts = lsPermissionKey.Split('|');
                            lnPermission = MaxConvertLibrary.ConvertToLong(typeof(object), laPermissionParts[1]);
                        }
                        else
                        {
                            lnPermission = MaxConvertLibrary.ConvertToLong(typeof(object), lsPermissionKey.Substring(Guid.Empty.ToString().Length));
                        }

                        if (lnPermission > long.MinValue)
                        {
                            if (loPermissionIndex.Contains(lsPermissionId))
                            {
                                loPermissionIndex[lsPermissionId] = (long)loPermissionIndex[lsPermissionId] | lnPermission;
                            }
                            else
                            {
                                loPermissionIndex.Add(lsPermissionId, lnPermission);
                            }
                        }
                    }

                    MaxRoleRelationPermissionEntity loRelation = MaxRoleRelationPermissionEntity.Create();
                    MaxEntityList loRelationList = loRelation.LoadAllByRoleIdCache(loRole.Id);
                    //// Remove permissions from the index that have not changed.  Remove Permissions from the role that have changed, or are not in the selected list.
                    for (int lnR = 0; lnR < loRelationList.Count; lnR++)
                    {
                        loRelation = loRelationList[lnR] as MaxRoleRelationPermissionEntity;
                        if (loPermissionIndex.Contains(loRelation.PermissionId.ToString()))
                        {
                            if (loRelation.Permission == (long)loPermissionIndex[loRelation.PermissionId.ToString()])
                            {
                                loPermissionIndex.Remove(loRelation.PermissionId.ToString());
                            }
                            else
                            {
                                loRelation.Delete();
                            }
                        }
                        else
                        {
                            loRelation.Delete();
                        }
                    }

                    //// Add any new or changed permissions to the role.
                    string[] laPermissionId = loPermissionIndex.GetSortedKeyList();
                    foreach (string lsPermissionId in laPermissionId)
                    {
                        loRelation = MaxRoleRelationPermissionEntity.Create();
                        loRelation.PermissionId = MaxConvertLibrary.ConvertToGuid(typeof(object), lsPermissionId);
                        loRelation.Permission = MaxConvertLibrary.ConvertToLong(typeof(object), loPermissionIndex[lsPermissionId]) | (long)MaxEnumGroup.PermissionGroup;
                        loRelation.RoleId = loRole.Id;
                        loRelation.Insert();
                    }

                    //// Update the list of permissions in the response to reflect the current state of the role.
                    foreach (string lsResponseProperty in loRequest.ResponsePropertyList)
                    {
                        if (lsResponseProperty == "PermissionKeySelectedList" || lsResponseProperty == typeof(MaxRoleEntity).ToString() + ".PermissionKeySelectedList")
                        {
                            List<string> loPermissionList = new List<string>();
                            loRelationList = loRelation.LoadAllByRoleIdCache(loRole.Id);
                            for (int lnE = 0; lnE < loRelationList.Count; lnE++)
                            {
                                loRelation = loRelationList[lnE] as MaxRoleRelationPermissionEntity;
                                loPermissionList.Add(loRelation.PermissionId.ToString() + '|' + loRelation.Permission);
                            }

                            loR.Item.Add("PermissionKeySelectedList", loPermissionList.ToArray());
                        }
                    }
                }
            }

            return loR;
        }

        protected override MaxApiResponseViewModel ProcessLoadList(MaxApiRequestViewModel loRequest, MaxEntity loEntity)
        {
            MaxApiResponseViewModel loR = base.ProcessLoadList(loRequest, loEntity);
            if (loEntity is MaxUserAuthTokenEntity)
            {
                List<MaxIndex> loNotExpiredList = new List<MaxIndex>();
                foreach (MaxIndex loIndex in loR.ItemList)
                {
                    bool lbIsExpired = MaxConvertLibrary.ConvertToBoolean(typeof(object), loIndex["IsExpired"]);
                    if (!lbIsExpired)
                    {
                        loNotExpiredList.Add(loIndex);
                    }
                }

                loR.ItemList = loNotExpiredList;
            }

            return loR;
        }

        protected override MaxEntity MapRequest(MaxEntity loEntity, MaxApiRequestViewModel loRequest)
        {
            MaxEntity loR = base.MapRequest(loEntity, loRequest);
            if (loR is MaxUserAuthTokenEntity)
            {
                if (this.Request.Method == HttpMethod.Get && string.IsNullOrEmpty(loR.DataKey))
                {
                    Guid loUserId = this.GetUserId(loRequest);
                    loR = MaxUserAuthTokenEntity.GetCurrent(loUserId.ToString());
                }
                else
                {
                    string lsCurrentUserKey = this.GetUserId(loRequest).ToString();
                    //// Always set user key when inserted. Allow changing by admin.
                    if (this.Request.Method == HttpMethod.Post || !this.HasPermission(loRequest, loEntity, -1))
                    {
                        ((MaxUserAuthTokenEntity)loR).UserKey = lsCurrentUserKey;
                    }
                }
            }

            return loR;
        }

        protected override MaxIndex GetResponseFilter(MaxApiRequestViewModel loRequest, MaxEntity loEntity)
        {
            MaxIndex loR = base.GetResponseFilter(loRequest, loEntity);
            if (loEntity is MaxUserAuthTokenEntity)
            {
                MaxIndex loResponseFilterList = new MaxIndex();
                Guid loUserId = this.GetUserId(loRequest);
                MaxIndex loFilterPart = new MaxIndex();
                loFilterPart.Add(MaxEntity.FilterName, "UserKey");
                loFilterPart.Add(MaxEntity.FilterValue, loUserId.ToString());
                loResponseFilterList.Add(loFilterPart);
                if (this.HasPermission(loRequest, loEntity, -1)) //// Admin permission
                {
                    loFilterPart = new MaxIndex();
                    loFilterPart.Add(MaxEntity.FilterName, "AdminUserKey");
                    loFilterPart.Add(MaxEntity.FilterValue, "%-%");
                    loFilterPart.Add(MaxEntity.FilterOperator, MaxEntity.FilterOperatorLike);
                    loFilterPart.Add(MaxEntity.FilterCondition, MaxEntity.FilterConditionOr);
                    loResponseFilterList.Add(loFilterPart);
                }

                loR.Add(loResponseFilterList);
            }

            return loR;
        }

        /// <summary>
        /// Gets the list of permissions related to this api
        /// </summary>
        /// <returns></returns>
        protected override MaxIndex GetPermissionList()
        {
            MaxIndex loR = base.GetPermissionList();
            loR.Add(MaxRoleRelationPermissionEntity.GetPermission(MaxUserEntity.Create(), "Users"));
            loR.Add(MaxRoleRelationPermissionEntity.GetPermission(MaxRoleEntity.Create(), "Roles"));
            return loR;
        } 
    }
}