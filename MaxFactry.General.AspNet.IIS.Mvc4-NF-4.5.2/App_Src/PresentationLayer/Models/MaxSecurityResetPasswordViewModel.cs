// <copyright file="MaxSecurityResetPasswordViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="11/27/2020" author="Brian A. Lakstins" description="Initial creation">
// <change date="12/10/2020" author="Brian A. Lakstins" description="Integrate generation of email content">
// <change date="12/19/2020" author="Brian A. Lakstins" description="Use email or username when looking up user">
// <change date="1/17/2021" author="Brian A. Lakstins" description="Add password length check">
// <change date="2/24/2021" author="Brian A. Lakstins" description="Update password reset process.">
// <change date="2/25/2021" author="Brian A. Lakstins" description="Change password reset">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
	using System;
    using System.Web.Security;
    using System.ComponentModel.DataAnnotations;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.General.BusinessLayer;
    using MaxFactry.General.PresentationLayer;

    /// <summary>
    /// View model for resetting password
    /// </summary>
    public class MaxSecurityResetPasswordViewModel
	{
        private MembershipUser _oUser = null;

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
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password Confirmation")]
        [UIHint("Password Confirmation")]
        public string PasswordConfirm { get; set; }

        /// <summary>
        /// Gets or sets the confirmation of the user AuthCode
        /// </summary>
        [Required]
        [Display(Name = "Code")]
        [UIHint("Code Confirmation")]
        public string UserAuthCodeConfirm { get; set; }

        public string UserAuthCode
        {
            get 
            {
                return MaxUserEntity.Create().GetUserAuthCode(this.User.UserName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the login should be remembered.
        /// </summary>
        public string ReturnUrl { get; set; }

        public string Email
        {
            get
            {
                string lsR = string.Empty;
                if (null != this.User)
                {
                    lsR = this.User.Email;
                }

                return lsR;
            }
        }

        public Guid UserId
        {
            get
            {
                Guid loR = Guid.Empty;
                if (null != this.User)
                {
                    loR = MaxConvertLibrary.ConvertToGuid(typeof(object), this.User.ProviderUserKey);
                }

                return loR;
            }
        }

        public MembershipUser User
        {
            get
            {
                if (null == this._oUser && !string.IsNullOrEmpty(this.UserName))
                {
                    this._oUser = Membership.GetUser(this.UserName);
                    if (null == this._oUser)
                    {
                        if (MaxBaseEmailEntity.IsValidEmail(this.UserName))
                        {
                            string lsUserName = Membership.GetUserNameByEmail(this.UserName);
                            if (!string.IsNullOrEmpty(lsUserName))
                            {
                                this._oUser = Membership.GetUser(lsUserName);
                            }
                        }
                    }
                }
                else if (null == this._oUser && !string.IsNullOrEmpty(this.UserName) && MaxBaseEmailEntity.IsValidEmail(this.UserName))
                {
                    string lsUserName = Membership.GetUserNameByEmail(this.UserName);
                    if (!string.IsNullOrEmpty(lsUserName))
                    {
                        this._oUser = Membership.GetUser(lsUserName);
                    }
                }

                return this._oUser;
            }
        }

        public string SendPasswordReset(string lsTemplate)
        {
            string lsR = string.Empty;
            if (null == this.User)
            {
                lsR = "User could not be found for username or email";
            }
            else if (!MaxEmailEntity.IsValidEmail(this.User.Email))
            {
                lsR = "Email found is not validly formatted for sending an email";
            }
            else
            {
                MaxEmailEntity loEmail = MaxEmailEntity.Create();
                loEmail.RelationType = "Security.PasswordReset";
                loEmail.RelationId = this.UserId;
                loEmail.Subject = "Password Reset";
                loEmail.ToAddressListText = this.User.Email;
                loEmail.ToNameListText = this.User.UserName;
                string lsHtml = string.Empty;
                if (!string.IsNullOrEmpty(lsTemplate))
                {
                    lsHtml = MaxDesignLibrary.GetHtml(lsTemplate, this, null);
                }
                else
                {
                    lsHtml = MaxDesignLibrary.GetHtml("/Views/MaxSecurityApi/MessageResetPassword.cshtml", this, null);
                }

                lsHtml = MaxDesignLibrary.ConvertToInlineCSS(lsHtml, string.Empty);
                loEmail.Content = lsHtml;
                try
                {
                    loEmail.Send();
                    if (!loEmail.Insert())
                    {
                        lsR = "Could not insert email";
                    }

                    if (loEmail.SentCount <= 0)
                    {
                        lsR = "There was a problem sending the email";
                        MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "There was a problem sending a password reset email to {Email}", this.Email));
                    }
                }
                catch (Exception loE)
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error Deliver email to {ToAddressList}", loEmail.ToAddressListText, loE));
                    lsR = loE.Message;
                }
            }

            return lsR;
        }

        public string CheckNewPassword()
        {
            string lsR = string.Empty;
            if (this.Password.Length < 8)
            {
                lsR = "The new password is too short.  It should be at least 8 characters long.";
            }
            else if (this.Password != this.PasswordConfirm)
            {
                lsR = "The new password confirmation does not match the new password.";
            }

            return lsR;
        }

        public string TryResetPassword()
        {
            string lsR = this.CheckNewPassword();
            if (lsR.Length == 0)
            {
                if (string.IsNullOrEmpty(this.UserAuthCodeConfirm))
                {
                    lsR = "The auth code is blank.";
                }
                else if (this.UserAuthCodeConfirm.Trim() != this.UserAuthCode)
                {
                    lsR = "The auth code is not correct.";
                }
                else
                {
                    if (null != this.User)
                    {
                        if (MaxMembershipUser.SetPassword(this.User, this.Password))
                        {
                            lsR = string.Empty;
                            MaxUserEntity.Create().ClearUserAuthCode(this.User.UserName);
                        }
                        else
                        {
                            lsR = "There was an unknown error changing the password.";
                            if (Membership.MinRequiredPasswordLength > this.Password.Length)
                            {
                                lsR = "Minimum password length is " + Membership.MinRequiredPasswordLength;
                            }
                        }
                    }
                    else
                    {
                        lsR = "The user with email or username " + this.UserName + " was not found";
                    }
                }
            }

            return lsR;
        }
    }
}
