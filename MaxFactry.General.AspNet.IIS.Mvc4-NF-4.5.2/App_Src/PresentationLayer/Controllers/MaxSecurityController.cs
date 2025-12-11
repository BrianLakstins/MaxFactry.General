// <copyright file="MaxSecurityController.cs" company="Lakstins Family, LLC">
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
// <change date="6/19/2014" author="Brian A. Lakstins" description="Move code to model.">
// <change date="6/27/2014" author="Brian A. Lakstins" description="Remove dependency on AppId.">
// <change date="10/28/2016" author="Brian A. Lakstins" description="Update to try to fix System.Web.Mvc.HttpAntiForgeryException (0x80004005): The provided anti-forgery token was meant for user '', but the current user is 'username'.">
// <change date="11/18/2020" author="Brian A. Lakstins" description="Update for changes to references.">
// <change date="11/17/2020" author="Brian A. Lakstins" description="Add reset password.">
// <change date="12/10/2020" author="Brian A. Lakstins" description="Generate email content in Model">
// <change date="10/10/2023" author="Brian A. Lakstins" description="Added OAuth2 Login integration">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class.  Updated to use DataKey.">
// <change date="7/16/2024" author="Brian A. Lakstins" description="Add some user logging.  Set some attributes based on time.">
// <change date="9/16/2024" author="Brian A. Lakstins" description="Use underscore to designate attributes that are internal">
// <change date="10/23/2024" author="Brian A. Lakstins" description="Handle redirect url with query string">
// <change date="11/5/2024" author="Brian A. Lakstins" description="Changed stored username format">
// <change date="1/27/2025" author="Brian A. Lakstins" description="Add auth type when logging in">
// <change date="7/10/2025" author="Brian A. Lakstins" description="Fix logging of Auth2 Login">
// <change date="12/11/2025" author="Brian A. Lakstins" description="Add a way to override the return host when running on localhost">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using JWT;
    using JWT.Serializers;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Core;
    using MaxFactry.General.BusinessLayer;
    using MaxFactry.General.PresentationLayer;

    public class MaxSecurityController : MaxBaseControllerSecure
    {
        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult Index()
        {
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult Login()
        {
            MaxSecurityLoginViewModel loModel = new MaxSecurityLoginViewModel();
            if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
            {
                loModel.ReturnUrl = Request.QueryString["ReturnUrl"];
            }

            return this.View(loModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult Login(MaxSecurityLoginViewModel loModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (loModel.ValidateUser())
                    {
                        MaxFactry.General.AspNet.IIS.MaxAppLibrary.SignIn(loModel.UserName);
                        MaxUserEntity loMaxUser = MaxUserEntity.Create();
                        MaxEntityList loMaxUserList = loMaxUser.LoadAllByUsernameCache(loModel.UserName);
                        if (loMaxUserList.Count == 1)
                        {
                            loMaxUser = loMaxUserList[0] as MaxUserEntity;
                            loMaxUser.SetAttribute("_LastIISSignIn", DateTime.UtcNow);
                            loMaxUser.SetAttribute("_AuthType", "Login");
                            loMaxUser.Update();
                        }

                        if (!string.IsNullOrEmpty(loModel.ReturnUrl))
                        {
                            return this.Redirect(loModel.ReturnUrl);
                        }

                        return this.RedirectToAction("LoggedIn");
                    }
                    else
                    {
                        MembershipUser loUser = Membership.GetUser(loModel.UserName);
                        if (null != loUser)
                        {
                            try
                            {
                                string lsPasswordCurrent = loUser.GetPassword();
                                if (string.IsNullOrEmpty(lsPasswordCurrent))
                                {
                                    return this.RedirectToAction("ResetPassword", new RouteValueDictionary { { "Username", loModel.UserName }, { "ReturnUrl", loModel.ReturnUrl } });
                                }
                            }
                            catch (Exception loE)
                            {
                                ModelState.AddModelError(string.Empty, "Exception determining if password needs reset.");
                                MaxFactry.Core.MaxLogLibrary.Log(new MaxFactry.Core.MaxLogEntryStructure(this.GetType(), "Login", Core.MaxEnumGroup.LogError, "Exception determining if password needs reset", loE));
                            }
                        }

                        ModelState.AddModelError(string.Empty, "The user name or password is incorrect.");
                    }
                }
                catch (Exception loE)
                {
                    ModelState.AddModelError(string.Empty, "There was an unexpected error validating the user. [" + loE.ToString() + "]");
                }
            }

            return this.View(loModel);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult Logout()
        {
            MaxFactry.General.AspNet.IIS.MaxAppLibrary.SignOut();
            return this.RedirectToAction("LoggedOut");
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult LoggedOut()
        {
            return this.View();
        }

        [HttpGet]
        public virtual ActionResult Manage()
        {
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult Signup()
        {
            MaxSecuritySignupViewModel loModel = new MaxSecuritySignupViewModel();
            if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
            {
                loModel.ReturnUrl = Request.QueryString["ReturnUrl"];
            }

            return this.View(loModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult Signup(MaxSecuritySignupViewModel loModel)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus loCreateStatus = MembershipCreateStatus.ProviderError;
                try
                {
                    MembershipUser loUser = loModel.CreateUser(out loCreateStatus);
                    if (loCreateStatus.Equals(MembershipCreateStatus.Success))
                    {
                        if (!Request.IsAuthenticated)
                        {
                            MaxFactry.General.AspNet.IIS.MaxAppLibrary.SignIn(loUser.UserName);
                            Guid loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), loUser.ProviderUserKey);
                            MaxUserEntity loMaxUser = MaxUserEntity.Create();
                            if (loMaxUser.LoadByIdCache(loUserId))
                            {
                                loMaxUser.SetAttribute("_LastIISSignIn", DateTime.UtcNow);
                                loMaxUser.SetAttribute("_AuthType", "Signup");
                                loMaxUser.Update();
                            }
                        }

                        if (!string.IsNullOrEmpty(loModel.ReturnUrl))
                        {
                            return this.Redirect(loModel.ReturnUrl);
                        }

                        return this.RedirectToAction("SignedUp");
                    }
                    else 
                    {
                        if (loCreateStatus == MembershipCreateStatus.DuplicateEmail ||
                            loCreateStatus == MembershipCreateStatus.InvalidEmail)
                        {
                            ModelState.AddModelError("Email", MaxMembershipProviderOverride.GetCreateErrorMessage(loCreateStatus));
                        }
                        else if (loCreateStatus == MembershipCreateStatus.DuplicateUserName ||
                            loCreateStatus == MembershipCreateStatus.InvalidUserName)
                        {
                            ModelState.AddModelError("UserName", MaxMembershipProviderOverride.GetCreateErrorMessage(loCreateStatus));
                        }
                        else if (loCreateStatus == MembershipCreateStatus.InvalidAnswer)
                        {
                            ModelState.AddModelError("SecretAnswer", MaxMembershipProviderOverride.GetCreateErrorMessage(loCreateStatus));
                        }
                        else if (loCreateStatus == MembershipCreateStatus.InvalidPassword)
                        {
                            ModelState.AddModelError("Password", MaxMembershipProviderOverride.GetCreateErrorMessage(loCreateStatus));
                        }
                        else if (loCreateStatus == MembershipCreateStatus.InvalidQuestion)
                        {
                            ModelState.AddModelError("SecretQuestion", MaxMembershipProviderOverride.GetCreateErrorMessage(loCreateStatus));
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, MaxMembershipProviderOverride.GetCreateErrorMessage(loCreateStatus));
                        }
                    }
                }
                catch (Exception loE)
                {
                    ModelState.AddModelError(string.Empty, "There was an unexpected error creating the account. [" + loE.ToString() + "]");
                }
            }

            return this.View(loModel);
        }

        [HttpGet]
        public virtual ActionResult LoggedIn()
        {
            return this.View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Admin - App,Admin - Manage Users")]
        public virtual ActionResult ManageUser(string lsUserId)
        {
            return this.ManageUser(new MaxSecurityManageUserViewModel(lsUserId), null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Admin - App,Admin - Manage Users")]
        public virtual ActionResult ManageUser(MaxSecurityManageUserViewModel loModel, string uoProcess)
        {
            if (!string.IsNullOrEmpty(uoProcess))
            {
                if (uoProcess.Equals("cancel", StringComparison.InvariantCultureIgnoreCase))
                {
                    return this.RedirectToAction("ManageUser", new RouteValueDictionary { { "lsUserId", string.Empty } });
                }

                if (ModelState.IsValid)
                {
                    if (uoProcess.Equals("save", StringComparison.InvariantCultureIgnoreCase))
                    {
                        loModel.Save();
                    }
                    else if (uoProcess.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
                    {
                        bool lbIsSuccess = loModel.Delete();
                    }
                }
            }

            return this.View(loModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult SignedUp()
        {
            return this.View();
        }

        [HttpGet]
        public virtual ActionResult ChangePassword()
        {
            MaxSecurityChangePasswordViewModel loModel = new MaxSecurityChangePasswordViewModel();
            return this.View(loModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ChangePassword(MaxSecurityChangePasswordViewModel loModel)
        {
            if (ModelState.IsValid)
            {
                if (loModel.ChangePassword(User.Identity.Name))
                {
                    return this.RedirectToAction("ChangedPassword");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The current password is incorrect or the new password is invalid.");
                }
            }

            return this.View(loModel);
        }

        [HttpGet]
        public virtual ActionResult ChangedPassword()
        {
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult ResetPassword()
        {
            MaxSecurityResetPasswordViewModel loModel = new MaxSecurityResetPasswordViewModel();
            if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
            {
                loModel.ReturnUrl = Request.QueryString["ReturnUrl"];
            }

            if (!string.IsNullOrEmpty(Request.QueryString["UserName"]))
            {
                loModel.UserName = Request.QueryString["UserName"];
                string lsResult = loModel.SendPasswordReset(string.Empty);
                if (!string.IsNullOrEmpty(lsResult))
                {
                    ModelState.AddModelError(string.Empty, "Error sending password reset email: " + lsResult);
                }
            }

            return this.View(loModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult ResetPassword(MaxSecurityResetPasswordViewModel loModel)
        {
            if (!string.IsNullOrEmpty(loModel.Password) || !string.IsNullOrEmpty(loModel.UserAuthCodeConfirm) || !string.IsNullOrEmpty(loModel.PasswordConfirm))
            {
                if (ModelState.IsValid)
                {
                    string lsError = loModel.TryResetPassword();
                    if (string.IsNullOrEmpty(lsError))
                    {
                        if (!string.IsNullOrEmpty(loModel.ReturnUrl))
                        {
                            return this.Redirect(loModel.ReturnUrl);
                        }

                        return this.RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, lsError);
                    }
                }
            }

            return this.View(loModel);
        }

        /// <summary>
        /// Handles a request from another site for OAuth2 authentication with this site.  Makes the user log in first to this site before redirecting back to original request site.
        /// TODO: Test this and make sure it works
        /// Request
        /// From: https://gist.github.com/mziwisky/10079157
        /// PrettyMail responds REDIRECT gmail.com/oauth2/auth?client_id=ABC&redirect_uri=prettymail.com/oauth_response -- note: also common to include ‘scopes’ in query -- i.e., the scope of the information that PrettyMail is asking to access
        /// client_id
        /// redirect_uri
        /// scopes
        /// From: https://alexbilbie.com/guide-to-oauth-2-grants/ - Authorisation Code Grant (section 4.1) - The Flow (Part One)
        /// response_type=code
        /// response_mode=query or fragment or form_post or web_message - https://auth0.com/docs/protocols/protocol-oauth2 https://openid.net/specs/oauth-v2-multiple-response-types-1_0.html
        /// client_id 
        /// redirect_uri 
        /// scope 
        /// state 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult OAuth2()
        {
            try
            {
                string lsFullUri = Request.Url.ToString();
                string lsResponseType = Request.QueryString["response_type"];
                string lsResponseMode = Request.QueryString["response_mode"];
                string lsClientId = Request.QueryString["client_id"];
                string lsRedirectUri = Request.QueryString["redirect_uri"];
                string lsScope = Request.QueryString["scope"];
                string lsState = Request.QueryString["state"];
                string lsNonce = Request.QueryString["nonce"];
                string lsAccessType = Request.QueryString["access_type"];
                string lsApprovalPrompt = Request.QueryString["approval_prompt"];
                string lsIncludeGrantedScopes = Request.QueryString["include_granted_scopes"];
                string lsPrompt = Request.QueryString["prompt"];

                //// TODO: Check format of query information sent

                /*
                gmail makes a session in which it stores provider (PrettyMail, based on client_id -- if client_id doesn’t refer to an authorized oauth_client, render an error) and redirect_uri and then responds:

                a. REDIRECT gmail.com/login (for a login form) if the user isn’t logged in, otherwise

                    b. REDIRECT directly to step (4) 
                */
                //// Save data from session
                MaxUserAuthGrantEntity loGrantEntity = MaxUserAuthGrantEntity.Create();
                loGrantEntity.FullUri = lsFullUri;
                loGrantEntity.ResponseType = lsResponseType;
                loGrantEntity.ClientId = lsClientId;
                loGrantEntity.RedirectUri = lsRedirectUri;
                loGrantEntity.Scope = lsScope;
                loGrantEntity.State = lsState;
                loGrantEntity.Nonce = lsNonce;
                loGrantEntity.AccessType = lsAccessType;
                loGrantEntity.ApprovalPrompt = lsApprovalPrompt;
                loGrantEntity.IncludeGrantedScopes = lsIncludeGrantedScopes;
                loGrantEntity.Prompt = lsPrompt;
                if (!string.IsNullOrEmpty(lsResponseMode))
                {
                    //// TODO: handle other response modes besides query
                    loGrantEntity.ResponseMode = lsResponseMode;
                }

                //// TODO: Check nonce to make sure unique, but if not included at all then allow anyway?

                //// Look up client Id
                MaxUserAuthEntity loEntity = MaxUserAuthEntity.Create();
                MaxEntityList loList = loEntity.LoadAllByClientIdCache(loGrantEntity.ClientId);
                for (int lnE = 0; lnE < loList.Count && Guid.Empty == loGrantEntity.UserAuthId; lnE++)
                {
                    loEntity = loList[lnE] as MaxUserAuthEntity;
                    if (loEntity.IsActive)
                    {
                        //// TODO: Check scope 
                        //// TODO: Check domainlist
                        loGrantEntity.UserAuthId = loEntity.Id;
                    }
                }

                if (Guid.Empty == loGrantEntity.UserAuthId)
                {
                    return this.RedirectToAction("Error");
                }

                //// Check to see if logged in - redirect to login if not
                MembershipUser loUser = Membership.GetUser();
                if (null != loUser && loUser.IsApproved)
                {
                    if (loUser.IsApproved)
                    {
                        loGrantEntity.UserKey = MaxConvertLibrary.ConvertToString(typeof(object), loUser.ProviderUserKey);
                    }
                    else
                    {
                        return this.RedirectToAction("Error");
                    }
                }

                if (string.IsNullOrEmpty(loGrantEntity.UserKey))
                {
                    return this.RedirectToAction("Login", new { ReturnUrl = this.Request.Url });
                }

                MaxEntityList loGrantList = loGrantEntity.LoadAllByUserKeyCache(loGrantEntity.UserKey);
                //// Mark the current grant as approved if one as was approved in the past for the same user and the same scope
                for (int lnE = 0; lnE < loGrantList.Count && !loGrantEntity.IsActive; lnE++)
                {
                    MaxUserAuthGrantEntity loGrantCheckEntity = loGrantList[lnE] as MaxUserAuthGrantEntity;
                    if (loGrantCheckEntity.IsActive && loGrantCheckEntity.Scope == loGrantEntity.Scope)
                    {
                        loGrantCheckEntity.State = loGrantEntity.State;
                        loGrantCheckEntity.FullUri = loGrantEntity.FullUri;
                        loGrantCheckEntity.ClientId = loGrantEntity.ClientId;
                        loGrantCheckEntity.RedirectUri = loGrantEntity.RedirectUri;
                        loGrantCheckEntity.Nonce = loGrantEntity.Nonce;
                        loGrantCheckEntity.AccessType = loGrantEntity.AccessType;
                        loGrantCheckEntity.ApprovalPrompt = loGrantEntity.ApprovalPrompt;
                        loGrantCheckEntity.IncludeGrantedScopes = loGrantEntity.IncludeGrantedScopes;
                        loGrantCheckEntity.Prompt = loGrantEntity.Prompt;
                        loGrantCheckEntity.Update();
                        loGrantEntity = loGrantCheckEntity;
                    }
                }

                //// Only create a grant record once the user is logged in
                if (!loGrantEntity.IsActive)
                {
                    //// Check to see if authorization to access is needed for this user
                    if (!string.IsNullOrEmpty(loUser.Comment) && loUser.Comment.Contains("Oath2Auto"))
                    {
                        loGrantEntity.IsActive = true;
                    }

                    loGrantEntity.Insert();
                }

                if (loGrantEntity.IsActive)
                {
                    if (loGrantEntity.ResponseMode == "query" || string.IsNullOrEmpty(loGrantEntity.ResponseMode))
                    {
                        string lsUrl = loGrantEntity.RedirectUri + "?code=" + loGrantEntity.Code;
                        if (!string.IsNullOrEmpty(loGrantEntity.State))
                        {
                            lsUrl += "&state=" + loGrantEntity.State;
                        }

                        return this.Redirect(lsUrl);
                    }
                    else
                    {
                        return this.RedirectToAction("Error");
                    }
                }

                //// Show Authorization request form for this Grant
                MaxUserAuthGrantViewModel loModel = new MaxUserAuthGrantViewModel(loGrantEntity);
                loModel.Load();
                return View(loModel);
            }
            catch (Exception loE)
            {
                return this.RedirectToAction("Error", loE);
            }
        }

        /// <summary>
        /// Handles a valid login and redirects the user back to their site.
        /// TODO: Test this and make sure it works
        /// From: https://gist.github.com/mziwisky/10079157
        /// generates a one-time-use code that it associates with PrettyMail and the specified user and the requested scope (so it persists it until the next step) and REDIRECTs to the ‘redirect_uri’ it got in the first place, passing along that code: prettymail.com/oauth_response?code=big_long_thing
        /// From: https://alexbilbie.com/guide-to-oauth-2-grants/  - Authorisation Code Grant (section 4.1) = The Flow (Part One)
        /// Response
        /// code 
        /// state 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult OAuth2(MaxUserAuthGrantViewModel loModel)
        {
            if (ModelState.IsValid)
            {
                Guid loId = MaxConvertLibrary.ConvertToGuid(typeof(object), loModel.Id);
                MaxUserAuthGrantEntity loGrantEntity = MaxUserAuthGrantEntity.Create();
                if (loGrantEntity.LoadByDataKeyCache(loId.ToString()))
                {
                    loGrantEntity.IsActive = true;
                    loGrantEntity.Update();
                    if (loGrantEntity.ResponseMode == "query" || string.IsNullOrEmpty(loGrantEntity.ResponseMode))
                    {
                        string lsUrl = loGrantEntity.RedirectUri + "?code=" + loGrantEntity.Code;
                        if (!string.IsNullOrEmpty(loGrantEntity.State))
                        {
                            lsUrl += "&state=" + loGrantEntity.State;
                        }

                        return this.Redirect(lsUrl);
                    }
                    else
                    {
                        return this.RedirectToAction("Error");
                    }
                }
                else
                {
                    return this.RedirectToAction("Error");
                }
            }

            return View(loModel);
        }

        /// <summary>
        /// Uses the query string to redirect a user to an Microsoft OpenID Connect authorizatio url
        /// https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-protocols-oidc
        /// </summary>
        /// <param name="tenant">You can use the {tenant} value in the path of the request to control who can sign in to the application. The allowed values are common, organizations, consumers, and tenant identifiers. For more information, see protocol basics. Critically, for guest scenarios where you sign a user from one tenant into another tenant, you must provide the tenant identifier to correctly sign them into the resource tenant.</param>
        /// <param name="client_id">The Application (client) ID that the Microsoft Entra admin center – App registrations experience assigned to your app.</param>
        /// <param name="response_type">Must include id_token for OpenID Connect sign-in.</param>
        /// <param name="scope">A space-separated list of scopes. For OpenID Connect, it must include the scope openid, which translates to the Sign you in permission in the consent UI. You might also include other scopes in this request for requesting consent.</param>
        /// <param name="domain_hint">	The realm of the user in a federated directory. This skips the email-based discovery process that the user goes through on the sign-in page, for a slightly more streamlined user experience. For tenants that are federated through an on-premises directory like AD FS, this often results in a seamless sign-in because of the existing login session.</param>
        /// <param name="ReturnUrl">The Url that should be shown on the current site after the login process is complete</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult OAuth2OIDC(string host, string tenant, string client_id, string response_type, string scope, string domain_hint, string ReturnUrl)
        {
            try
            {
                MaxUserAuthGrantEntity loEntity = MaxUserAuthGrantEntity.Create();
                loEntity.IsActive = true;
                loEntity.State = Guid.NewGuid().ToString();
                loEntity.Nonce = Guid.NewGuid().ToString();
                loEntity.ClientId = client_id;
                loEntity.Scope = scope;
                loEntity.ResponseType = response_type;
                loEntity.ResponseMode = "form_post";
                loEntity.RedirectUri = ReturnUrl;

                string lsCurrentHost = this.Request.Headers["X-Forwarded-Host"];
                if (string.IsNullOrEmpty(lsCurrentHost))
                {
                    lsCurrentHost = this.Request.Url.DnsSafeHost;
                    if (string.IsNullOrEmpty(lsCurrentHost))
                    {
                        Uri loReturnUrl = new Uri(ReturnUrl);
                        lsCurrentHost = loReturnUrl.DnsSafeHost;
                    }
                }
                if (!string.IsNullOrEmpty(host) && lsCurrentHost.ToLower().Contains("localhost"))
                {
                    lsCurrentHost = host;
                }

                loEntity.FullUri = string.Format("https://{0}/MaxSecurity/OAuth2OIDC", lsCurrentHost);

                string lsUrl = string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/authorize", tenant);
                lsUrl += "?client_id=" + HttpUtility.UrlEncode(loEntity.ClientId);
                lsUrl += "&response_type=" + HttpUtility.UrlEncode(loEntity.ResponseType);
                lsUrl += "&scope=" + HttpUtility.UrlEncode(loEntity.Scope);
                lsUrl += "&domain_hint=" + HttpUtility.UrlEncode(domain_hint);
                lsUrl += "&redirect_uri=" + HttpUtility.UrlEncode(loEntity.FullUri);
                lsUrl += "&state=" + HttpUtility.UrlEncode(loEntity.State);
                lsUrl += "&nonce=" + HttpUtility.UrlEncode(loEntity.Nonce);
                lsUrl += "&response_mode=form_post";

                loEntity.FullUri = lsUrl;
                if (loEntity.Insert())
                {
                    return this.Redirect(lsUrl);
                }

                return this.RedirectToAction("Error");
            }
            catch (Exception loE)
            {
                return this.RedirectToAction("Error", loE);
            }
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-protocols-oidc
        /// </summary>
        /// <param name="loModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult OAuth2OIDC(MaxUserAuthGrantViewModel loModel)
        {
            if (null == loModel)
            {
                loModel = new MaxUserAuthGrantViewModel();
            }

            try
            {
                string lsIdToken = this.Request.Form["id_token"];
                string lsState = this.Request.Form["state"];
                string lsSessionState = this.Request.Form["session_state"];
                string lsTokenType = this.Request.Form["token_type"];
                string lsExpiresIn = this.Request.Form["expires_in"];
                string lsScope = this.Request.Form["scope"];
                string lsAccessToken = this.Request.Form["access_token"];

                MaxUserAuthGrantEntity loEntity = MaxUserAuthGrantEntity.Create();
                if (loEntity.LoadByState(lsState) && loEntity.IsActive)
                {
                    loEntity.IsActive = false;
                    loEntity.Update();
                    //// Parse the Id Token
                    IJsonSerializer loSerializer = new JsonNetSerializer();
                    IBase64UrlEncoder loUrlEncoder = new JwtBase64UrlEncoder();
                    IJwtDecoder loDecoder = new JwtDecoder(loSerializer, loUrlEncoder);
                    IDictionary<string, object> loIdToken = loDecoder.DecodeToObject<IDictionary<string, object>>(lsIdToken, string.Empty, false);
                    //// https://learn.microsoft.com/en-us/azure/active-directory/develop/id-token-claims-reference
                    string lsNonce = loIdToken["nonce"] as string;
                    if (loEntity.Nonce == lsNonce)
                    {
                        if (loIdToken.ContainsKey("oid"))
                        {
                            Guid loObjectId = new Guid(loIdToken["oid"] as string);
                            //// The immutable identifier for an object, in this case, a user account. 
                            ////  This ID uniquely identifies the user across applications - two different applications signing in the same user receives the same value in the oid claim. 
                            ////  Microsoft Graph returns this ID as the id property for a user account. 
                            ////  Because the oid allows multiple apps to correlate users, the profile scope is required to receive this claim. 
                            ////  If a single user exists in multiple tenants, the user contains a different object ID in each tenant - they're considered different accounts, even though the user logs into each account with the same credentials.
                            ////  The oid claim is a GUID and can't be reused.
                            string lsObjectId = MaxConvertLibrary.ConvertGuidToAlphabet64(typeof(object), loObjectId);
                            
                            string lsEmail = string.Empty;
                            if (loIdToken.ContainsKey("email"))
                            {
                                lsEmail = loIdToken["email"] as string;
                            }
                            else if (loIdToken.ContainsKey("preferred_username"))
                            {
                                if (MaxEmailEntity.IsValidEmail(loIdToken["preferred_username"] as string))
                                {
                                    lsEmail = loIdToken["preferred_username"] as string;
                                }
                            }

                            string lsUserName = lsEmail.Replace("@", "+OAuth2" + lsObjectId + "@");

                            MaxUserEntity loUser = MaxUserEntity.Create();
                            MaxEntityList loList = loUser.LoadAllByUsernameCache(lsUserName);
                            MaxUserLogEntity loUserLog = MaxUserLogEntity.Create();
                            if (loList.Count == 0)
                            {
                                loList = loUser.LoadAllByEmailCache(lsEmail);
                                if (loList.Count == 1)
                                {
                                    loUser = loList[0] as MaxUserEntity;
                                    if (loUser.UserName != lsUserName)
                                    {
                                        loUserLog.Insert(loUser.Id, MaxUserLogEntity.LogEntryTypeOther, "Changed from username [" + loUser.UserName + "] to " + "[" + lsUserName + "]");
                                        loUser.UserName = lsUserName;                                        
                                        loUser.Update();
                                    } 

                                    loUserLog.Insert(loUser.Id, MaxUserLogEntity.LogEntryTypeLogin, "Logged in using OAuth2");
                                }
                                else if (loList.Count == 0)
                                {
                                    string lsRandomPassword = MaxFactry.Core.MaxConvertLibrary.ConvertGuidToAlphabet64(typeof(object), Guid.NewGuid());
                                    //// Create the user
                                    MembershipUser loMembershipUser = Membership.CreateUser(lsUserName, lsRandomPassword, lsEmail);
                                }
                                else
                                {
                                    throw new Exception("More than one user with the same email");
                                }

                                //// Log the user on
                                MaxFactry.General.AspNet.IIS.MaxAppLibrary.SignIn(lsUserName);
                                MaxUserEntity loMaxUser = MaxUserEntity.Create();
                                MaxEntityList loMaxUserList = loMaxUser.LoadAllByUsernameCache(lsUserName);
                                if (loMaxUserList.Count == 1)
                                {
                                    loMaxUser = loMaxUserList[0] as MaxUserEntity;
                                    loMaxUser.SetAttribute("_LastOAuth2EmailSignIn", DateTime.UtcNow);
                                    loMaxUser.SetAttribute("_LastIISSignIn", DateTime.UtcNow);
                                    loMaxUser.SetAttribute("_AuthType", "OAuth2Email");
                                    loMaxUser.Update();
                                }
                            }
                            else if (loList.Count == 1)
                            {
                                loUser = loList[0] as MaxUserEntity;
                                loUserLog.Insert(loUser.Id, MaxUserLogEntity.LogEntryTypeLogin, "Logged in using OAuth2");
                                loUser.SetAttribute("_LastOAuth2UsernameSignIn", DateTime.UtcNow);
                                loUser.SetAttribute("_LastIISSignIn", DateTime.UtcNow);
                                loUser.SetAttribute("_AuthType", "OAuth2Username");
                                loUser.Update();

                                //// Log the user on
                                MaxFactry.General.AspNet.IIS.MaxAppLibrary.SignIn(lsUserName);
                            }
                            else
                            {
                                throw new Exception("More than one user with the same username");
                            }

                            loEntity.UserKey = lsUserName + "|" + lsEmail;
                            loEntity.Update();
                            if (!string.IsNullOrEmpty(loEntity.RedirectUri))
                            {
                                string lsRedirectUrl = loEntity.RedirectUri;
                                if (loEntity.RedirectUri.Contains("ReturnUrl="))
                                {
                                    string[] laRedirectUri = loEntity.RedirectUri.Split(new string[] { "ReturnUrl=" }, StringSplitOptions.None);
                                    lsRedirectUrl = laRedirectUri[0] + "ReturnUrl=" + HttpUtility.UrlEncode(laRedirectUri[1]);

                                }

                                return this.Redirect(lsRedirectUrl);
                            }
                        }
                    }
                }
            }
            catch (Exception loE)
            {
                return this.RedirectToAction("Error", loE);
            }

            return View(loModel);
        }
    }
}
