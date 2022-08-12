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
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
	using System;
    using System.Web.Security;
    using System.ComponentModel.DataAnnotations;

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
    }
}
