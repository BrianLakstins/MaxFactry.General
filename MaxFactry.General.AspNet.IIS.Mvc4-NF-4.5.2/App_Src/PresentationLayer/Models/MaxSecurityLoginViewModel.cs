// <copyright file="MaxSecurityLoginViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="6/3/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="6/19/2014" author="Brian A. Lakstins" description="Move code from controller.">
// <change date="3/18/2026" author="Brian A. Lakstins" description="Consolidate login code.  Add handling of JWT.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using JWT;
    using JWT.Serializers;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Core;
    using MaxFactry.General.BusinessLayer;
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Security;

    /// <summary>
    /// View model for logging in.
    /// </summary>
	public class MaxSecurityLoginViewModel
	{
        /// <summary>
        /// Gets or sets the Username.
        /// </summary>
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [UIHint("Password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the login should be remembered.
        /// </summary>
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the password can be reset.
        /// </summary>
        public bool EnablePasswordReset
        {
            get
            {
                return Membership.EnablePasswordReset;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the login should be remembered.
        /// </summary>
        public string ReturnUrl { get; set; }

        public bool ValidateUser()
        {
            return Membership.ValidateUser(this.UserName, this.Password);
        }

        public virtual string GetUserName(IDictionary<string, object> loIdToken)
        {
            Guid loObjectId = new Guid(loIdToken["oid"] as string);
            //// The immutable identifier for an object, in this case, a user account. 
            ////  This ID uniquely identifies the user across applications - two different applications signing in the same user receives the same value in the oid claim. 
            ////  Microsoft Graph returns this ID as the id property for a user account. 
            ////  Because the oid allows multiple apps to correlate users, the profile scope is required to receive this claim. 
            ////  If a single user exists in multiple tenants, the user contains a different object ID in each tenant - they're considered different accounts, even though the user logs into each account with the same credentials.
            ////  The oid claim is a GUID and can't be reused.
            string lsObjectId = MaxConvertLibrary.ConvertGuidToAlphabet64(typeof(object), loObjectId);
            string lsEmail = this.GetEmail(loIdToken);
            string lsR = lsEmail.Replace("@", "+OAuth2" + lsObjectId + "@");
            return lsR;
        }

        public virtual string GetEmail(IDictionary<string, object> loIdToken)
        {
            string lsR = string.Empty;
            string lsEmail = string.Empty;
            if (loIdToken.ContainsKey("unique_name"))
            {
                lsEmail = loIdToken["unique_name"] as string;
            }
            else if (loIdToken.ContainsKey("email"))
            {
                lsEmail = loIdToken["email"] as string;
            }
            else if (loIdToken.ContainsKey("preferred_username"))
            {
                lsEmail = loIdToken["preferred_username"] as string;
            }

            if (MaxEmailEntity.IsValidEmail(lsEmail))
            {
                lsR = lsEmail;
            }

            return lsR;
        }

        public virtual bool IsValidIdToken(string lsIdToken)
        {
            bool lbR = false;
            try
            { 
                IDictionary<string, object> loIdToken = this.ParseToken(lsIdToken);
                string lsTenantId = loIdToken["tid"] as string;
                object loTenantList = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "OAuth2OIDCMicrosoftTenantList");
                if (null != loTenantList)
                {
                    string lsTenantList = MaxConvertLibrary.ConvertToString(typeof(object), loTenantList).ToLower();
                    if (lsTenantList.Contains(lsTenantId))
                    {
                        lbR = true;
                    }
                }
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "IsValidIdToken", MaxEnumGroup.LogError, "Error validating token", loE));
            }

            return lbR;
        }

        public virtual IDictionary<string, object> ParseToken(string lsIdToken)
        {
            IJsonSerializer loSerializer = new JsonNetSerializer();
            IBase64UrlEncoder loUrlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder loDecoder = new JwtDecoder(loSerializer, loUrlEncoder);
            IDictionary<string, object> loIdToken = loDecoder.DecodeToObject<IDictionary<string, object>>(lsIdToken, string.Empty, false);
            return loIdToken;
        }

        public virtual bool LoginUser(string lsState, string lsIdToken, out string lsRedirectUrl)
        {
            bool lbR = false;
            lsRedirectUrl = string.Empty;
            if (string.IsNullOrEmpty(lsState))
            {
                throw new MaxException("Missing State");
            }
            else
            {
                IDictionary<string, object> loIdToken = this.ParseToken(lsIdToken);
                string lsNonce = string.Empty;
                if (loIdToken.ContainsKey("nonce"))
                {
                    lsNonce = loIdToken["nonce"] as string;
                }

                if (!string.IsNullOrEmpty(lsNonce))
                {
                    //// This is a redirect login that started on this site and then went to Microsoft for login and is now coming back with the id token and state.  Validate the state and nonce and then log the user in and redirect back to the original url.
                    MaxUserAuthGrantEntity loEntity = MaxUserAuthGrantEntity.Create();
                    if (loEntity.LoadByState(lsState) && loEntity.IsActive)
                    {
                        loEntity.IsActive = false;
                        loEntity.Update();
                        //// https://learn.microsoft.com/en-us/azure/active-directory/develop/id-token-claims-reference                            
                        if (loEntity.Nonce == lsNonce)
                        {
                            string lsEmail = this.GetEmail(loIdToken);
                            string lsUserName = this.GetUserName(loIdToken);
                            string lsUserLoggedInName = this.LoginUser(lsUserName, lsEmail, "OAuth2 Grant");
                            if (!string.IsNullOrEmpty(lsUserLoggedInName))
                            {
                                loEntity.UserKey = lsUserName + "|" + lsEmail;
                                loEntity.Update();
                                if (!string.IsNullOrEmpty(loEntity.RedirectUri))
                                {
                                    lsRedirectUrl = loEntity.RedirectUri;
                                    if (loEntity.RedirectUri.Contains("ReturnUrl="))
                                    {
                                        string[] laRedirectUri = loEntity.RedirectUri.Split(new string[] { "ReturnUrl=" }, StringSplitOptions.None);
                                        lsRedirectUrl = laRedirectUri[0] + "ReturnUrl=" + HttpUtility.UrlEncode(laRedirectUri[1]);
                                    }

                                    lbR = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new MaxException("Missing Nonce in token");
                }
            }

            return lbR;
        }
        
        public virtual string LoginUser(string lsUserName, string lsEmail, string lsAuthType)
        {
            string lsR = string.Empty;
            MaxUserEntity loUser = MaxUserEntity.Create();
            MaxEntityList loList = loUser.LoadAllByUsernameCache(lsUserName);
            MaxUserLogEntity loUserLog = MaxUserLogEntity.Create();
            if (loList.Count == 0)
            {
                Guid loUserId = Guid.Empty;
                loList = loUser.LoadAllByEmailCache(lsEmail);
                if (loList.Count == 1)
                {
                    loUser = loList[0] as MaxUserEntity;
                    if (loUser.UserName != lsUserName)
                    {
                        loUserLog.Insert(loUser.Id, MaxUserLogEntity.LogEntryTypeOther, "Changed from username [" + loUser.UserName + "] to " + "[" + lsUserName + "]");
                        loUser.UserName = lsUserName;
                        loUser.Update();
                        loUserId = loUser.Id;
                    }
                }
                else if (loList.Count == 0)
                {
                    string lsRandomPassword = MaxFactry.Core.MaxConvertLibrary.ConvertGuidToAlphabet64(typeof(object), Guid.NewGuid());
                    //// Create the user
                    MembershipUser loMembershipUser = Membership.CreateUser(lsUserName, lsRandomPassword, lsEmail);
                    if (null != loMembershipUser)
                    {
                        loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), loMembershipUser.ProviderUserKey.ToString());
                    }
                }
                else
                {
                    throw new Exception("More than one user with the same email");
                }

                if (loUserId != Guid.Empty)
                {
                    lsR = this.LoginUser(loUserId, lsAuthType + " using Email");
                }
            }
            else if (loList.Count == 1)
            {
                loUser = loList[0] as MaxUserEntity;                
                lsR = this.LoginUser(loUser.Id, lsAuthType + " using Username");
            }
            else
            {
                throw new Exception("More than one user with the same username");
            }

            return lsR;
        }

        public virtual string LoginUser(Guid loUserId, string lsAuthType)
        {
            string lsR = string.Empty;
            MaxUserEntity loUser = MaxUserEntity.Create();
            if (loUser.LoadByIdCache(loUserId))
            {
                if (loUser.IsActive)
                {
                    MaxUserLogEntity loUserLog = MaxUserLogEntity.Create();
                    loUser.SetAttribute("_LastIISSignIn", DateTime.UtcNow);
                    loUser.SetAttribute("_AuthType", lsAuthType);
                    loUser.Update();
                    loUserLog.Insert(loUser.Id, MaxUserLogEntity.LogEntryTypeLogin, "Logged in using " + lsAuthType);
                    MaxFactry.General.AspNet.IIS.MaxAppLibrary.SignIn(loUser.UserName);
                    lsR = loUser.UserName;
                }
            }

            return lsR;
        }

        public virtual bool ValidateTokenSignature(string lsIdToken)
        {
            bool lbR = false;
            try
            {
                //// Get the tenant id from the token to know which keys to use for validation               
                IDictionary<string, object> loIdToken = this.ParseToken(lsIdToken);
                string lsTenantId = loIdToken["tid"] as string;
                if (!string.IsNullOrEmpty(lsTenantId))
                {
                    // Fetch Microsoft's public keys
                    string lsKeysUrl = string.Format("https://login.microsoftonline.com/{0}/discovery/v2.0/keys", lsTenantId);
                    System.Net.WebClient loClient = new System.Net.WebClient();
                    string lsKeysJson = loClient.DownloadString(lsKeysUrl);

                    // Parse the keys JSON
                    IJsonSerializer loSerializer = new JsonNetSerializer();
                    IDictionary<string, object> loKeysResponse = loSerializer.Deserialize<IDictionary<string, object>>(lsKeysJson);

                    // Get the token header to find the key id (kid)
                    string[] laTokenParts = lsIdToken.Split('.');
                    if (laTokenParts.Length != 3)
                    {
                        return false;
                    }

                    IBase64UrlEncoder loUrlEncoder = new JwtBase64UrlEncoder();
                    string lsHeaderJson = System.Text.Encoding.UTF8.GetString(loUrlEncoder.Decode(laTokenParts[0]));
                    IDictionary<string, object> loHeader = loSerializer.Deserialize<IDictionary<string, object>>(lsHeaderJson);
                    string lsKid = loHeader["kid"] as string;

                    // Find the matching key
                    Newtonsoft.Json.Linq.JArray loKeys = loKeysResponse["keys"] as Newtonsoft.Json.Linq.JArray;
                    foreach (Newtonsoft.Json.Linq.JObject loKey in loKeys)
                    {
                        if (loKey["kid"].ToString() == lsKid)
                        {
                            string lsModulus = loKey["n"].ToString();
                            string lsExponent = loKey["e"].ToString();

                            // Create RSA parameters from the key
                            System.Security.Cryptography.RSAParameters loRsaParams = new System.Security.Cryptography.RSAParameters
                            {
                                Modulus = loUrlEncoder.Decode(lsModulus),
                                Exponent = loUrlEncoder.Decode(lsExponent)
                            };

                            // Create RSA provider and verify signature
                            using (System.Security.Cryptography.RSACryptoServiceProvider loRsa = new System.Security.Cryptography.RSACryptoServiceProvider())
                            {
                                loRsa.ImportParameters(loRsaParams);

                                // Get the signature and data to verify
                                byte[] laSignature = loUrlEncoder.Decode(laTokenParts[2]);
                                byte[] laDataToVerify = System.Text.Encoding.UTF8.GetBytes(laTokenParts[0] + "." + laTokenParts[1]);

                                // Verify using SHA256
                                using (System.Security.Cryptography.SHA256 loSha256 = System.Security.Cryptography.SHA256.Create())
                                {
                                    byte[] laHash = loSha256.ComputeHash(laDataToVerify);
                                    lbR = loRsa.VerifyHash(laHash, System.Security.Cryptography.CryptoConfig.MapNameToOID("SHA256"), laSignature);
                                }
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "ValidateTokenSignature", MaxEnumGroup.LogError, "Error validating token signature", loE));
            }

            return lbR;
        }

        public virtual MaxUserAuthGrantEntity CreateUserAuthGrant(string lsState, string lsNonce, string lsClientId, string lsScope, string lsReposeType, string lsReturnUrl, string lsAuthUrl)
        {
            MaxUserAuthGrantEntity loR = MaxUserAuthGrantEntity.Create();
            loR.IsActive = true;
            loR.State = lsState;
            loR.Nonce = lsNonce;
            loR.ClientId = lsClientId;
            loR.Scope = lsScope;
            loR.ResponseType = lsReposeType;
            loR.ResponseMode = "form_post";
            loR.RedirectUri = lsReturnUrl;
            loR.FullUri = lsAuthUrl;
            return loR;
        }
    }
}
