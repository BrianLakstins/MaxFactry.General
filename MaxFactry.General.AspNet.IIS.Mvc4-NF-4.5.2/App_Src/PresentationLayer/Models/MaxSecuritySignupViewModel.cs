// <copyright file="MaxSecuritySignupViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="9/23/2020" author="Brian A. Lakstins" description="Set username to email if not included.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
	using System;
    using System.Web.Security;
    using System.ComponentModel.DataAnnotations;

	/// <summary>
    /// View model for signing up with a new user account.
	/// </summary>
	public class MaxSecuritySignupViewModel
	{
        /// <summary>
        /// Gets or sets the Username.
        /// </summary>
        [Display(Name = "User name")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Required]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [Display(Name = "Password")]
        [UIHint("Password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the second password.
        /// </summary>
        [Required]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [UIHint("Password")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the secret question.
        /// </summary>
        [Display(Name = "Secret Question")]
        public string SecretQuestion { get; set; }

        /// <summary>
        /// Gets or sets the secret answer.
        /// </summary>
        [Display(Name = "Secret Answer")]
        public string SecretAnswer { get; set; }

        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a secret question and answer are required.
        /// </summary>
        public bool RequireSecretQuestionAndAnswer
        {
            get 
            {
                return Membership.RequiresQuestionAndAnswer;
            }
        }

        public MembershipUser CreateUser(out MembershipCreateStatus loCreateStatus)
        {
            if (string.IsNullOrEmpty(this.UserName))
            {
                this.UserName = this.Email.ToLower();
            }

            MembershipUser loUser = Membership.CreateUser(this.UserName, this.Password, this.Email, this.SecretQuestion, this.SecretAnswer, true, out loCreateStatus);
            if (loCreateStatus == MembershipCreateStatus.Success)
            {
                if (!Roles.RoleExists("Admin"))
                {
                    Roles.CreateRole("Admin");
                    Roles.AddUserToRole(loUser.UserName, "Admin");
                }

                if (!Roles.RoleExists("Admin - App"))
                {
                    Roles.CreateRole("Admin - App");
                    Roles.AddUserToRole(loUser.UserName, "Admin - App");
                }

                if (!Roles.RoleExists("Admin - Manage Users"))
                {
                    Roles.CreateRole("Admin - Manage Users");
                    Roles.AddUserToRole(loUser.UserName, "Admin - Manage Users");
                }

                return loUser;
            }
          
            return null;
        }
    }
}
