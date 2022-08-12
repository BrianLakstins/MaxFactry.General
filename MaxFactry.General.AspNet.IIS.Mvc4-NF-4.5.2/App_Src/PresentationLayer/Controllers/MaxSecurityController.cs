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
                                MaxFactry.Core.MaxLogLibrary.Log(new MaxFactry.Core.MaxLogEntryStructure("MaxSecurityController.Login", Core.MaxEnumGroup.LogError, "Exception determining if password needs reset", loE));
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
                if (loGrantEntity.LoadByIdCache(loId))
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
    }
}
