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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.General.AspNet.BusinessLayer;
    using MaxFactry.General.AspNet.PresentationLayer;
    using MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer;
    using MaxFactry.General.BusinessLayer;
    using MaxFactry.General.PresentationLayer;
    using MaxFactry.Base.DataLayer;
    using System.Net.NetworkInformation;
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
                            if (loGrantEntity.LoadByIdCache(loGrantId) && loGrantEntity.UserAuthId == loEntity.Id && loGrantEntity.IsActive && loGrantEntity.RedirectUri == lsRedirectUri)
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
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HttpOptions]
        [ActionName("usertoken")]
        public async Task<HttpResponseMessage> UserToken()
        {
            HttpStatusCode loStatus = HttpStatusCode.Unauthorized;
            var loResponseItem = new
            {
                UserName = "UserName",
                Email = "Email",
                Id = "Id",
                AccessToken = "AccessToken",
                ExpiresIn = "ExpiresIn"
            };

            MaxApiResponseViewModel loR = GetResponse(loResponseItem);
            if (this.Request.Method != HttpMethod.Options)
            {
                var loRequestItem = new
                {
                    TokenType = "TokenType",
                    Expiration = "Expiration"
                };

                MaxApiRequestViewModel loRequest = await this.GetRequest();
                MembershipUser loUser = this.GetUser(loRequest);
                if (null != loUser)
                {
                    loStatus = HttpStatusCode.OK;
                    string lsToken = MaxUserAuthTokenEntity.GenerateToken(false);
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

                    MaxUserAuthTokenEntity loTokenEntity = MaxUserAuthTokenEntity.AddToken(lsToken, lsTokenType, loExpiration, loUser.ProviderUserKey.ToString(), Guid.Empty, Guid.Empty);
                    loR.Item.Add(loResponseItem.UserName, loUser.UserName);
                    loR.Item.Add(loResponseItem.Email, loUser.Email);
                    loR.Item.Add(loResponseItem.Id, loUser.ProviderUserKey);
                    loR.Item.Add(loResponseItem.AccessToken, loTokenEntity.GetClientToken(lsToken));
                    loR.Item.Add(loResponseItem.ExpiresIn, loTokenEntity.Expiration);
                }
            }

            return this.GetResponseMessage(loR, loStatus);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpOptions]
        [ActionName("user")]
        public async Task<HttpResponseMessage> UserCurrent()
        {
            HttpStatusCode loStatus = HttpStatusCode.Unauthorized;
            var loResponseItem = new
            {
                UserName = "UserName",
                Email = "Email",
                Id = "Id",
                RoleList = "RoleList"
            };

            MaxApiResponseViewModel loR = GetResponse(loResponseItem);
            if (this.Request.Method != HttpMethod.Options)
            {
                MaxApiRequestViewModel loRequest = await this.GetRequest();
                MembershipUser loUser = this.GetUser(loRequest);
                if (null != loUser)
                {
                    loStatus = HttpStatusCode.OK;
                    try
                    {
                        //// Return a user and roles
                        loR.Item.Add(loResponseItem.UserName, loUser.UserName);
                        loR.Item.Add(loResponseItem.Email, loUser.Email);
                        loR.Item.Add(loResponseItem.Id, MaxConvertLibrary.ConvertToString(typeof(object), loUser.ProviderUserKey).ToLower());
                        string[] laRole = Roles.GetRolesForUser(loUser.UserName);
                        loR.Item.Add(loResponseItem.RoleList, laRole);
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
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        [HttpOptions]
        [ActionName("users")]
        public async Task<HttpResponseMessage> Users()
        {
            HttpStatusCode loStatus = HttpStatusCode.Unauthorized;
            var loResponseItem = new
            {
                RoleList = "RoleList",
                UserName = "UserName",
                Email = "Email",
                Id = "Id",
                LastPasswordChangedDate = "LastPasswordChangedDate",
                Comment = "Comment",
                LastActivityDate = "LastActivityDate",
                LastLoginDate = "LastLoginDate",
                PasswordQuestion = "PasswordQuestion",
                IsPasswordResetNeeded = "IsPasswordResetNeeded"
            };

            MaxApiResponseViewModel loR = GetResponse(loResponseItem);
            if (this.Request.Method != HttpMethod.Options)
            {
                var loRequestItem = new
                {
                    UserName = "UserName",
                    Id = "Id",
                    Password = "Password",
                    Email = "Email",
                    Comment = "Comment",
                    RoleListText = "RoleListText",
                    IsPasswordResetNeeded = "IsPasswordResetNeeded"
                };

                MaxApiRequestViewModel loRequest = await this.GetRequest();
                MembershipUser loUser = this.GetUser(loRequest);
                if (null != loUser)
                {
                    loStatus = HttpStatusCode.Forbidden;
                    List<string> loRoleList = new List<string>(Roles.GetRolesForUser(loUser.UserName));
                    bool lbHasAccess = loRoleList.Contains("Admin - App") || loRoleList.Contains("Admin") || loRoleList.Contains("User Manager");
                    if (lbHasAccess)
                    {
                        loStatus = HttpStatusCode.OK;
                        string lsUserName = loRequest.Item.GetValueString(loRequestItem.UserName);
                        Guid loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), loRequest.Item[loRequestItem.Id]);
                        string lsEmail = loRequest.Item.GetValueString(loRequestItem.Email);
                        MembershipUser loUserRequest = null;
                        if (Guid.Empty != loUserId)
                        {
                            loR.Message.Log += "Loading for user providerkey of " + loUserId.ToString() + "\n";
                            loUserRequest = Membership.GetUser(loUserId);
                        }
                        else if (!string.IsNullOrEmpty(lsUserName))
                        {
                            loR.Message.Log += "Loading for username of " + lsUserName + "\n";
                            loUserRequest = Membership.GetUser(lsUserName);
                            if (null == loUserRequest && !string.IsNullOrEmpty(lsEmail) && MaxBaseEmailEntity.IsValidEmail(lsEmail))
                            {
                                loR.Message.Log += "Loading for username of " + lsUserName + " failed.  Trying email address of " + lsEmail + "\n";
                                lsUserName = Membership.GetUserNameByEmail(lsEmail);
                                if (!string.IsNullOrEmpty(lsUserName))
                                {
                                    loUserRequest = Membership.GetUser(lsUserName);
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(lsEmail) && MaxBaseEmailEntity.IsValidEmail(lsEmail))
                        {
                            loR.Message.Log += "Loading for email of " + lsEmail + "\n";
                            lsUserName = Membership.GetUserNameByEmail(lsEmail);
                            if (!string.IsNullOrEmpty(lsUserName))
                            {
                                loR.Message.Log += "Loading for username of " + lsUserName + "\n";
                                loUserRequest = Membership.GetUser(lsUserName);
                            }
                        }

                        if (this.Request.Method == HttpMethod.Get)
                        {
                            if (null != loUserRequest) //// Get a user 
                            {
                                //// Return a user and roles
                                loR.Item.Add(loResponseItem.UserName, loUserRequest.UserName);
                                loR.Item.Add(loResponseItem.Email, loUserRequest.Email);
                                loR.Item.Add(loResponseItem.Id, MaxConvertLibrary.ConvertToString(typeof(object), loUserRequest.ProviderUserKey).ToLower());
                                loR.Item.Add(loResponseItem.LastPasswordChangedDate, loUserRequest.LastPasswordChangedDate);
                                loR.Item.Add(loResponseItem.Comment, loUserRequest.Comment);
                                loR.Item.Add(loResponseItem.LastActivityDate, loUserRequest.LastActivityDate);
                                loR.Item.Add(loResponseItem.LastLoginDate, loUserRequest.LastLoginDate);
                                loR.Item.Add(loResponseItem.PasswordQuestion, loUserRequest.PasswordQuestion);
                                if (loUserRequest is MaxMembershipUser)
                                {
                                    loR.Item.Add(loResponseItem.IsPasswordResetNeeded, ((MaxMembershipUser)loUserRequest).IsPasswordResetNeeded);
                                }

                                string[] laRole = Roles.GetRolesForUser(loUserRequest.UserName);
                                loR.Item.Add(loResponseItem.RoleList, laRole);
                            }
                            else //// Get all users
                            {
                                MembershipUserCollection loUserList = Membership.GetAllUsers();
                                SortedList<string, MembershipUser> loSortedList = new SortedList<string, MembershipUser>();
                                foreach (MembershipUser loUserFromList in loUserList)
                                {
                                    loSortedList.Add(loUserFromList.UserName + loUserFromList.Email + loUserFromList.ProviderUserKey.ToString(), loUserFromList);
                                }

                                foreach (MembershipUser loUserFromList in loSortedList.Values)
                                {
                                    //// Return all users and roles
                                    MaxIndex loItem = new MaxIndex();
                                    loItem.Add(loResponseItem.UserName, loUserFromList.UserName);
                                    loItem.Add(loResponseItem.Email, loUserFromList.Email);
                                    loItem.Add(loResponseItem.Id, MaxConvertLibrary.ConvertToString(typeof(object), loUserFromList.ProviderUserKey).ToLower());
                                    /* Don't get roles for all users because it requires one database query per user.
                                    string[] laRole = Roles.GetRolesForUser(loUserFromList.UserName);
                                    loItem.Add(loResponseItem.RoleList, laRole);
                                    */
                                    loR.ItemList.Add(loItem);
                                }
                            }
                        }
                        else if (this.Request.Method == HttpMethod.Post && null != loUserRequest)  //// Update a user
                        {
                            try
                            {
                                string lsRoleList = loRequest.Item.GetValueString(loRequestItem.RoleListText);
                                string lsPassword = loRequest.Item.GetValueString(loRequestItem.Password);
                                string lsComment = loRequest.Item.GetValueString(loRequestItem.Comment);
                                bool lbIsPasswordResetNeeded = MaxConvertLibrary.ConvertToBoolean(typeof(object), loRequest.Item.GetValueString(loRequestItem.IsPasswordResetNeeded));
                                bool lbNeedsUpdate = false;
                                if (loUserRequest is MaxMembershipUser && ((MaxMembershipUser)loUserRequest).IsPasswordResetNeeded != lbIsPasswordResetNeeded)
                                {
                                    ((MaxMembershipUser)loUserRequest).IsPasswordResetNeeded = lbIsPasswordResetNeeded;
                                    lbNeedsUpdate = true;
                                }

                                lsUserName = loRequest.Item.GetValueString(loRequestItem.UserName);
                                if (!string.IsNullOrEmpty(lsEmail) && lsEmail.Length > 0 && loUserRequest.Email != lsEmail)
                                {
                                    loUserRequest.Email = lsEmail;
                                    lbNeedsUpdate = true;
                                }

                                if (!string.IsNullOrEmpty(lsComment) && loUserRequest.Comment != lsComment)
                                {
                                    loUserRequest.Comment = lsComment;
                                    lbNeedsUpdate = true;
                                }

                                if (lbNeedsUpdate)
                                {
                                    Membership.UpdateUser(loUserRequest);
                                    loR.Message.Log += "User updated\n";
                                }

                                if (lsPassword.Length > 0)
                                {
                                    if (MaxMembershipUser.SetPassword(loUserRequest, lsPassword))
                                    {
                                        loR.Message.Log += "Password set\n";
                                    }
                                }

                                loRoleList = new List<string>(Roles.GetRolesForUser(loUserRequest.UserName));
                                List<string> loRoleNameList = new List<string>(Roles.GetAllRoles());
                                if (!string.IsNullOrEmpty(lsRoleList))
                                {
                                    string[] laRoleNew = lsRoleList.Split(new char[] { ',' });
                                    loRoleList = new List<string>(Roles.GetRolesForUser(loUserRequest.UserName));
                                    foreach (string lsRole in laRoleNew)
                                    {
                                        if (!loRoleList.Contains(lsRole))
                                        {
                                            if (!loRoleNameList.Contains(lsRole))
                                            {
                                                Roles.CreateRole(lsRole);
                                                loRoleNameList.Add(lsRole);
                                            }

                                            Roles.AddUserToRole(loUserRequest.UserName, lsRole);
                                            loR.Message.Log += "Added user to role " + lsRole + "\n";
                                        }
                                        else
                                        {
                                            loRoleList.Remove(lsRole);
                                        }
                                    }
                                }

                                foreach (string lsRole in loRoleList)
                                {
                                    Roles.RemoveUserFromRole(loUserRequest.UserName, lsRole);
                                    loR.Message.Log += "Removed user from role " + lsRole + "\n";
                                }

                                //// Return updated user and roles
                                loR.Item.Add(loResponseItem.UserName, loUserRequest.UserName);
                                loR.Item.Add(loResponseItem.Email, loUserRequest.Email);
                                loR.Item.Add(loResponseItem.Id, MaxConvertLibrary.ConvertToString(typeof(object), loUserRequest.ProviderUserKey).ToLower());
                                string[] laRole = Roles.GetRolesForUser(loUserRequest.UserName);
                                loR.Item.Add(loResponseItem.RoleList, laRole);
                                loR.Message.Success = "User updated.";
                            }
                            catch (Exception loE)
                            {
                                loR.Message.Error = "Exception updating a user: " + loE.Message;
                                MaxLogLibrary.Log(new MaxLogEntryStructure("MaxSecurityApi", MaxEnumGroup.LogError, "Exception updating a user", loE));
                            }
                        }
                        else if (Request.Method == HttpMethod.Put) //// Add a user
                        {
                            try
                            {
                                string lsRoleList = loRequest.Item.GetValueString(loRequestItem.RoleListText);
                                string lsPassword = loRequest.Item.GetValueString(loRequestItem.Password);
                                lsUserName = loRequest.Item.GetValueString(loRequestItem.UserName);
                                MembershipUser loUserNew = Membership.CreateUser(lsUserName, lsPassword, lsEmail);
                                loR.Message.Log += "Creating user " + lsUserName + "\n";
                                string[] laRoleNew = lsRoleList.Split(new char[] { ',' });
                                foreach (string lsRole in laRoleNew)
                                {
                                    if (!string.IsNullOrEmpty(lsRole))
                                    {
                                        Roles.AddUserToRole(loUserNew.UserName, lsRole);
                                        loR.Message.Log += "Added user to role " + lsRole + "\n";
                                    }
                                }
                                //// Return new user and roles
                                loR.Item.Add(loResponseItem.UserName, loUserNew.UserName);
                                loR.Item.Add(loResponseItem.Email, loUserNew.Email);
                                loR.Item.Add(loResponseItem.Id, MaxConvertLibrary.ConvertToString(typeof(object), loUserNew.ProviderUserKey).ToLower());
                                string[] laRole = Roles.GetRolesForUser(loUserNew.UserName);
                                loR.Item.Add(loResponseItem.RoleList, laRole);
                                loR.Message.Success = "User added.";
                            }
                            catch (Exception loE)
                            {
                                loR.Message.Error = "Exception adding a user: " + loE.Message;
                                MaxLogLibrary.Log(new MaxLogEntryStructure("MaxSecurityApi", MaxEnumGroup.LogError, "Exception adding a user", loE));

                            }
                        }
                        else if (Request.Method == HttpMethod.Delete) //// delete a user
                        {
                            try
                            {
                                if (null != loUserRequest)
                                {
                                    Membership.DeleteUser(loUserRequest.UserName);
                                    loR.Message.Success = "User deleted.";
                                }
                            }
                            catch (Exception loE)
                            {
                                loR.Message.Error = "Exception deleting a user: " + loE.Message;
                                MaxLogLibrary.Log(new MaxLogEntryStructure("MaxSecurityApi", MaxEnumGroup.LogError, "Exception deleting a user", loE));
                            }
                        }
                    }
                }
            }
            else
            {
                loStatus = HttpStatusCode.OK;
            }

            return this.GetResponseMessage(loR, loStatus);
        }

        /// <summary>
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
                IsPasswordResetNeeded = "IsPasswordResetNeeded"
            };

            MaxApiResponseViewModel loR = GetResponse(loResponseItem);
            if (this.Request.Method != HttpMethod.Options)
            {
                var loRequestItem = new
                {
                    UserName = "UserName",
                    Email = "Email",
                    Password = "Password"
                };

                MaxApiRequestViewModel loRequest = await this.GetRequest();
                try
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
                            MembershipUser loUser = Membership.GetUser(lsUserCheck);
                            if (lbIsValid)
                            {
                                MaxFactry.General.AspNet.IIS.MaxAppLibrary.SignIn(lsUserCheck);
                                if (null != loUser && !string.IsNullOrEmpty(loUser.UserName))
                                {
                                    //// Return newly logged in user
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

                                    string[] laRole = Roles.GetRolesForUser(loUser.UserName);
                                    loR.Item.Add(loResponseItem.RoleList, laRole);
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
                catch (Exception loE)
                {
                    loR.Message.Error = "Exception logging in a user: " + loE.Message;
                    MaxLogLibrary.Log(new MaxLogEntryStructure("MaxSecurityApi", MaxEnumGroup.LogError, "Exception logging in a user", loE));
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
    }
}