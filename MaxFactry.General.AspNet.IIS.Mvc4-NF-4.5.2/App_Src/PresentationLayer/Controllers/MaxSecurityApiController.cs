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
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.General.AspNet.PresentationLayer;
    using MaxFactry.General.BusinessLayer;
    using MaxFactry.Base.DataLayer.Library;
    using System.Web.Security;

    [MaxRequireHttps(Order = 1)]
    [System.Web.Http.AllowAnonymous]
    public class MaxSecurityApiController : MaxBaseApiController
    {
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

            lsR = MaxDataLibrary.GetStorageKey(null);

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
        /// GET is used to get a list of non expired tokens.  The token value is not sent with the list.
        /// DELETE is used to get a list of non expired tokens and delete one token.
        /// POST is used to create a new token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [HttpDelete]
        [HttpOptions]
        [ActionName("usertoken")]
        public async Task<HttpResponseMessage> UserToken()
        {
            HttpStatusCode loStatus = HttpStatusCode.OK;
            var loResponseItem = new
            {
                DataKey = "DataKey",
                UserName = "UserName",
                Email = "Email",
                Id = "Id",
                AccessToken = "AccessToken",
                ExpiresIn = "ExpiresIn",
                CreatedDate = "CreatedDate",
                ExpirationDate = "ExpirationDate"
            };

            MaxApiResponseViewModel loR = GetResponse(loResponseItem);
            if (this.Request.Method != HttpMethod.Options)
            {
                loStatus = HttpStatusCode.Unauthorized;
                var loRequestItem = new
                {
                    DataKey = "DataKey",
                    Id = "Id",
                    TokenType = "TokenType",
                    Expiration = "Expiration"
                };

                MaxApiRequestViewModel loRequest = await this.GetRequest();
                if (null != loRequest.User)
                {
                    loStatus = HttpStatusCode.OK;
                    MaxUserAuthTokenEntity loEntity = MaxUserAuthTokenEntity.Create();
                    MaxEntityList loList = loEntity.LoadAllActiveByUserKeyCache(loRequest.User.ProviderUserKey.ToString());
                    string lsTokenType = "Bearer";
                    DateTime loExpiration = DateTime.UtcNow.AddYears(1);
                    if (loRequest.Item.Contains(loRequestItem.TokenType))
                    {
                        lsTokenType = loRequest.Item.GetValueString(loRequestItem.TokenType);
                    }

                    if (loRequest.Item.Contains(loRequestItem.Expiration))
                    {
                        loExpiration = MaxConvertLibrary.ConvertToDateTimeUtc(typeof(object), loRequest.Item.GetValueString(loRequestItem.Expiration));
                    }

                    Guid loId = MaxConvertLibrary.ConvertToGuid(typeof(object), loRequest.Item.GetValueString(loRequestItem.Id));
                    if (loId != Guid.Empty)
                    {
                        loR.Item.Add(loResponseItem.Id, loId);
                    }

                    MaxUserAuthTokenEntity loCurrentEntity = null;
                    for (int lnE = 0; lnE < loList.Count; lnE++)
                    {
                        loEntity = loList[lnE] as MaxUserAuthTokenEntity;
                        if (Request.Method == HttpMethod.Delete && loEntity.Id == loId)
                        {
                            loEntity.Delete();
                            loR.Message.Success = "Token deleted.";
                        }
                        else if (!loEntity.IsExpired)
                        {
                            MaxIndex loItem = new MaxIndex();
                            loItem.Add(loResponseItem.DataKey, loEntity.DataKey);
                            loItem.Add(loResponseItem.Id, loEntity.Id);
                            loItem.Add(loResponseItem.CreatedDate, MaxConvertLibrary.ConvertToDateTimeFromUtc(typeof(object), loEntity.CreatedDate));
                            loItem.Add(loResponseItem.ExpirationDate, MaxConvertLibrary.ConvertToDateTimeFromUtc(typeof(object), loEntity.ExpirationDate));
                            loR.ItemList.Add(loItem);
                            if (null == loCurrentEntity)
                            {
                                loCurrentEntity = loEntity;
                            }
                            else if (loCurrentEntity.ExpirationDate < loEntity.ExpirationDate)
                            {
                                loCurrentEntity = loEntity;
                            }
                        }
                    }

                    if (Request.Method == HttpMethod.Post || null == loCurrentEntity)
                    {
                        string lsToken = MaxUserAuthTokenEntity.GenerateToken(false);
                        loCurrentEntity = MaxUserAuthTokenEntity.AddToken(lsToken, lsTokenType, loExpiration, loRequest.User.ProviderUserKey.ToString(), Guid.Empty, Guid.Empty);
                        loR.Item.Add(loResponseItem.AccessToken, loCurrentEntity.GetClientToken(lsToken));
                        loR.Message.Success = "Token created.";
                        MaxIndex loItem = new MaxIndex();
                        loItem.Add(loResponseItem.DataKey, loCurrentEntity.DataKey);
                        loItem.Add(loResponseItem.Id, loCurrentEntity.Id);
                        loItem.Add(loResponseItem.CreatedDate, MaxConvertLibrary.ConvertToDateTimeFromUtc(typeof(object), loCurrentEntity.CreatedDate));
                        loItem.Add(loResponseItem.ExpirationDate, MaxConvertLibrary.ConvertToDateTimeFromUtc(typeof(object), loCurrentEntity.ExpirationDate));
                        loR.ItemList.Add(loItem);
                    }

                    loR.Item.Add(loResponseItem.UserName, loRequest.User.UserName);
                    loR.Item.Add(loResponseItem.Email, loRequest.User.Email);
                    loR.Item.Add(loResponseItem.Id, loCurrentEntity.Id);
                    loR.Item.Add(loResponseItem.ExpiresIn, loCurrentEntity.Expiration);
                    loR.Item.Add(loResponseItem.CreatedDate, MaxConvertLibrary.ConvertToDateTimeFromUtc(typeof(object), loCurrentEntity.CreatedDate));
                    loR.Item.Add(loResponseItem.ExpirationDate, MaxConvertLibrary.ConvertToDateTimeFromUtc(typeof(object), loCurrentEntity.ExpirationDate));
                }
            }

            return this.GetResponseMessage(loR, loStatus);
        }

        /// <summary>
        /// Gets the current logged in user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpOptions]
        [ActionName("usercurrent")]
        public async Task<HttpResponseMessage> UserCurrent()
        {
            HttpStatusCode loStatus = HttpStatusCode.Unauthorized;
            var loResponseItem = new
            {
                UserName = "UserName",
                Email = "Email",
                Id = "Id",
                RoleList = "RoleList",
                LastPasswordChangedDate = "LastPasswordChangedDate",
                Comment = "Comment",
                LastActivityDate = "LastActivityDate",
                LastLoginDate = "LastLoginDate",
                PasswordQuestion = "PasswordQuestion",
                IsPasswordResetNeeded = "IsPasswordResetNeeded",
                UserConfiguration = "UserConfiguration"
            };

            MaxApiResponseViewModel loR = GetResponse(loResponseItem);
            if (this.Request.Method != HttpMethod.Options)
            {
                MaxApiRequestViewModel loRequest = await this.GetRequest();
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

                if (null != loRequest.User)
                {
                    loStatus = HttpStatusCode.OK;
                    try
                    {
                        //// Return a user and roles
                        loR.Item.Add(loResponseItem.UserName, loRequest.User.UserName);
                        loR.Item.Add(loResponseItem.Email, loRequest.User.Email);
                        loR.Item.Add(loResponseItem.Id, MaxConvertLibrary.ConvertToString(typeof(object), loRequest.User.ProviderUserKey).ToLower());
                        loR.Item.Add(loResponseItem.LastPasswordChangedDate, loRequest.User.LastPasswordChangedDate);
                        loR.Item.Add(loResponseItem.Comment, loRequest.User.Comment);
                        loR.Item.Add(loResponseItem.LastActivityDate, loRequest.User.LastActivityDate);
                        loR.Item.Add(loResponseItem.LastLoginDate, loRequest.User.LastLoginDate);
                        loR.Item.Add(loResponseItem.PasswordQuestion, loRequest.User.PasswordQuestion);

                        if (loRequest.User is MaxMembershipUser)
                        {
                            loR.Item.Add(loResponseItem.IsPasswordResetNeeded, ((MaxMembershipUser)loRequest.User).IsPasswordResetNeeded);
                        }

                        List<MaxIndex> loRoleIndexList = new List<MaxIndex>();
                        MaxEntityList loRoleList = MaxRoleEntity.Create().LoadAllByUserIdCache(MaxConvertLibrary.ConvertToGuid(typeof(object), loRequest.User.ProviderUserKey));
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
                        loR.Message.Success = "Got current user information";
                    }
                    catch (Exception loE)
                    {
                        loR.Message.Error = "Exception getting user info: " + loE.Message;
                        MaxLogLibrary.Log(new MaxLogEntryStructure("MaxSecurityApi", MaxEnumGroup.LogError, "Exception getting user info", loE));
                    }
                }
            }
            else
            {
                loStatus = HttpStatusCode.OK;
            }

            return this.GetResponseMessage(loR.Item, loStatus);
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
        [ActionName("users")]
        public async Task<HttpResponseMessage> Users()
        {
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
        /// Attempts to log the user into the application
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
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
                UserConfiguration = "UserConfiguration"
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
                    MembershipUser loUser = Membership.GetUser();
                    if (this.Request.Method == HttpMethod.Post)
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
                            bool lbIsValid = Membership.ValidateUser(lsUserCheck, lsPassword);
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
                                loUser = Membership.GetUser(lsUserCheck);
                                if (lbIsValid)
                                {
                                    MaxFactry.General.AspNet.IIS.MaxAppLibrary.SignIn(lsUserCheck);
                                    Guid loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), loUser.ProviderUserKey);
                                    MaxUserEntity loMaxUser = MaxUserEntity.Create();
                                    if (loMaxUser.LoadByIdCache(loUserId))
                                    {
                                        loMaxUser.SetAttribute("LastIISSignIn", DateTime.UtcNow);
                                        loMaxUser.Update();
                                    }
                                }
                                else
                                {
                                    loR.Message.Error = "A matching username or password was not found.";
                                    if (null != loUser)
                                    {
                                        try
                                        {
                                            string lsPasswordCurrent = loUser.GetPassword();
                                            if (string.IsNullOrEmpty(lsPasswordCurrent))
                                            {
                                                loR.Message.Error = "Password needs to be reset.";
                                            }
                                        }
                                        catch (Exception loE)
                                        {
                                            if (loE.Message != "Password retrieval is disabled")
                                            {
                                                loR.Message.Error = "Exception logging in a user: " + loE.Message;
                                                MaxLogLibrary.Log(new MaxLogEntryStructure("MaxSecurityApi", MaxEnumGroup.LogError, "Exception logging in a user", loE));
                                            }
                                        }
                                    }

                                    loUser = null;
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

                    if (null != loUser && !string.IsNullOrEmpty(loUser.UserName))
                    {
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
                    }
                }
                catch (Exception loE)
                {
                    loR.Message.Error = "Exception logging in a user: " + loE.Message;
                    MaxLogLibrary.Log(new MaxLogEntryStructure("MaxSecurityApi", MaxEnumGroup.LogError, "Exception logging in a user", loE));
                }
            }

            return this.GetResponseMessage(loR);
        }

        /// <summary>
        /// Logs the user out of the application
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
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
                            loMaxUser.SetAttribute("LastLogout", DateTime.UtcNow);
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
                    MaxLogLibrary.Log(new MaxLogEntryStructure("MaxSecurityApi", MaxEnumGroup.LogError, "Exception logging out a user", loE));
                }
            }

            return this.GetResponseMessage(loR);
        }

        [HttpGet]
        [HttpPost]
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
                    else if (this.Request.Method == HttpMethod.Post)
                    {
                        loR.Message.Error = loModel.TryResetPassword();
                        if (loR.Message.Error.Length == 0)
                        {
                            loR.Message.Success = "The password was reset";
                        }
                    }
                }
                catch (Exception loE)
                {
                    loR.Message.Error = "Exception resetting password: " + loE.Message;
                    MaxLogLibrary.Log(new MaxLogEntryStructure("MaxSecurityApi", MaxEnumGroup.LogError, "Exception resetting password", loE));
                }
            }

            return this.GetResponseMessage(loR);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
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
                    MaxLogLibrary.Log(new MaxLogEntryStructure("MaxSecurityApi", MaxEnumGroup.LogError, "Exception changing password", loE));
                }
            }
            else
            {
                loStatus = HttpStatusCode.OK;
            }

            return this.GetResponseMessage(loR, loStatus);
        }

        protected override MaxApiResponseViewModel ProcessPost(MaxApiRequestViewModel loRequest, MaxEntity loOriginalEntity, MaxEntity loMappedEntity, MaxEntityList loMappedEntityList, MaxApiResponseViewModel loResponse)
        {
            MaxApiResponseViewModel loR = base.ProcessPost(loRequest, loOriginalEntity, loMappedEntity, loMappedEntityList, loResponse);
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

        protected override MaxApiResponseViewModel ProcessPut(MaxApiRequestViewModel loRequest, MaxEntity loMappedEntity, MaxEntityList loMappedList, MaxApiResponseViewModel loResponse)
        {
            MaxApiResponseViewModel loR = base.ProcessPut(loRequest, loMappedEntity, loMappedList, loResponse);
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

        protected MaxApiResponseViewModel ProcessUser(MaxApiRequestViewModel loRequest, MaxUserEntity loUser, MaxApiResponseViewModel loResponse)
        {
            MaxApiResponseViewModel loR = loResponse;
            if (null != loR && null != loR.Item && loR.Item.Contains(loUser.GetPropertyName(() => loUser.Id)) && null != loRequest.Item && loRequest.Item.Contains("RoleIdSelectedList"))
            {
                string[] laRoleIdSelectedList = MaxConvertLibrary.DeserializeObject(loRequest.Item.GetValueString("RoleIdSelectedList"), typeof(string[])) as string[];
                if (null != laRoleIdSelectedList)
                {
                    Guid loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), loR.Item.GetValueString(loUser.GetPropertyName(() => loUser.Id)));
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
            if (null != loR && null != loR.Item && loR.Item.Contains(loRole.GetPropertyName(() => loRole.Id)) && null != loRequest.Item && loRequest.Item.Contains("PermissionKeySelectedList"))
            {
                string[] laPermissionSelectedList = MaxConvertLibrary.DeserializeObject(loRequest.Item.GetValueString("PermissionKeySelectedList"), typeof(string[])) as string[];
                if (null != laPermissionSelectedList)
                {
                    Guid loRoleId = MaxConvertLibrary.ConvertToGuid(typeof(object), loR.Item.GetValueString(loRole.GetPropertyName(() => loRole.Id)));
                    List<string> loPermissionSelectedList = new List<string>(laPermissionSelectedList);
                    if (Guid.Empty != loRoleId)
                    {
                        MaxRoleRelationPermissionEntity loRelation = MaxRoleRelationPermissionEntity.Create();
                        MaxEntityList loRelationList = loRelation.LoadAllByRoleIdCache(loRoleId);
                        for (int lnR = 0; lnR < loRelationList.Count; lnR++)
                        {
                            loRelation = loRelationList[lnR] as MaxRoleRelationPermissionEntity;
                            string lsKey = loRelation.PermissionId.ToString() + loRelation.Permission.ToString();
                            if (loPermissionSelectedList.Contains(lsKey))
                            {
                                loPermissionSelectedList.Remove(lsKey);
                            }
                            else
                            {
                                loRelation.Delete();
                            }
                        }

                        foreach (string lsPermissionKey in loPermissionSelectedList)
                        {
                            loRelation = MaxRoleRelationPermissionEntity.Create();
                            loRelation.PermissionId = MaxConvertLibrary.ConvertToGuid(typeof(object), lsPermissionKey.Substring(0, Guid.NewGuid().ToString().Length));
                            loRelation.Permission = MaxConvertLibrary.ConvertToLong(typeof(object), lsPermissionKey.Substring(Guid.NewGuid().ToString().Length));
                            loRelation.RoleId = loRoleId;
                            loRelation.Insert();
                        }

                        foreach (string lsRequestProperty in loRequest.RequestPropertyList)
                        {
                            if (lsRequestProperty == "PermissionKeySelectedList" || lsRequestProperty == typeof(MaxRoleEntity).ToString() + ".PermissionKeySelectedList")
                            {
                                List<string> loPermissionList = new List<string>();
                                loRelationList = loRelation.LoadAllByRoleIdCache(loRoleId);
                                for (int lnE = 0; lnE < loRelationList.Count; lnE++)
                                {
                                    loRelation = loRelationList[lnE] as MaxRoleRelationPermissionEntity;
                                    loPermissionList.Add(loRelation.PermissionId.ToString() + loRelation.Permission.ToString());
                                }

                                loR.Item.Add("PermissionKeySelectedList", loPermissionList.ToArray());
                            }
                        }
                    }
                }
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