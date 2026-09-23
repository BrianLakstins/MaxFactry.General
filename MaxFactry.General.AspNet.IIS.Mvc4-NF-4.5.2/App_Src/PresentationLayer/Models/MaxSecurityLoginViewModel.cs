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
// <change date="4/16/2026" author="Brian A. Lakstins" description="Add checking of JWT.">
// <change date="9/23/2026" author="Brian A. Lakstins" description="Move token integration to a library">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Core;
    using MaxFactry.General.BusinessLayer;
	using System;
    using System.ComponentModel.DataAnnotations;
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
