// <copyright file="MaxSecurityManageUserViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="6/27/2014" author="Brian A. Lakstins" description="Remove dependency on AppId.">
// <change date="1/26/2021" author="Brian A. Lakstins" description="Speed up loading users.  Only load roles when loading a user by id.  Lazy load membership user.">
// <change date="2/24/2021" author="Brian A. Lakstins" description="Update password reset process.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Security;
    using MaxFactry.Base.PresentationLayer;

    /// <summary>
    /// View model for signing up with a new user account.
    /// </summary>
    public class MaxSecurityManageUserViewModel 
	{
        private SortedList<string, MaxSecurityManageUserViewModel> _oSortedList;

        private MembershipUser _oUser = null;

        public MaxSecurityManageUserViewModel()
        {
        }

        public MaxSecurityManageUserViewModel(string lsUserId)
        {
            this.Id = lsUserId;
            this.MapUser();
            if (null != this.User && !string.IsNullOrEmpty(this.User.UserName))
            {
                this.IsAdmin = Roles.IsUserInRole(this.User.UserName, "Admin - App");
                this.IsUserManager = Roles.IsUserInRole(this.User.UserName, "Admin - Manage Users");
            }
        }

        public MaxSecurityManageUserViewModel(MembershipUser loUser)
        {
            this._oUser = loUser;
            this.MapUser();
        }

        protected void MapUser()
        {
            if (null != this.User)
            {
                this.UserName = this.User.UserName;
                this.Email = this.User.Email;
                this.Id = this.User.ProviderUserKey.ToString();
            }
        }

        public string Id
        {
            get;
            set;
        }

        [Display(Name = "User name")]
        [Required]
        public string UserName
        {
            get;
            set;
        }

        [Display(Name = "Email address")]
        [Required]
        public string Email
        {
            get;
            set;
        }

        [Display(Name = "Password")]
        [UIHint("Password")]
        public string Password
        {
            get;
            set;
        }

        [Display(Name = "Comment")]
        public string Comment
        {
            get;
            set;
        }

        [Display(Name = "App Admin")]
        public bool IsAdmin
        {
            get;
            set;
        }

        [Display(Name = "User Manager")]
        public bool IsUserManager
        {
            get;
            set;
        }

        [Display(Name = "Password Reset Needed")]
        public bool IsPasswordResetNeeded
        {
            get;
            set;
        }

        protected MembershipUser User
        {
            get
            {
                if (null == this._oUser)
                {
                    if (!string.IsNullOrEmpty(this.Id))
                    {
                        Guid loId = Guid.Empty;
                        if (Guid.TryParse(this.Id, out loId))
                        {
                            this._oUser = Membership.GetUser(loId);
                        }
                    }

                    if (null == this._oUser)
                    {
                        if (!string.IsNullOrEmpty(this.UserName))
                        {
                            this._oUser = Membership.GetUser(this.UserName);
                        }

                        if (null == this._oUser)
                        {
                            if (!string.IsNullOrEmpty(this.Email))
                            {
                                string lsUserName = Membership.GetUserNameByEmail(this.Email);
                                if (!string.IsNullOrEmpty(lsUserName))
                                {
                                    this._oUser = Membership.GetUser(lsUserName);
                                }
                            }
                        }
                    }
                }

                return this._oUser;
            }
        }

        public List<MaxSecurityManageUserViewModel> List
        {
            get
            {
                if (null == this._oSortedList && string.IsNullOrEmpty(this.Id))
                {
                    this._oSortedList = new SortedList<string, MaxSecurityManageUserViewModel>();
                    MembershipUserCollection loList = Membership.GetAllUsers();
                    foreach (MembershipUser loUser in loList)
                    {
                        this._oSortedList.Add(loUser.UserName.PadRight(100, ' ') + string.Format("{0:yyyyMMddHHmmssffff}", loUser.CreationDate), new MaxSecurityManageUserViewModel(loUser));
                    }
                }

                if (null != this._oSortedList)
                {
                    return new List<MaxSecurityManageUserViewModel>(this._oSortedList.Values);
                }

                return new List<MaxSecurityManageUserViewModel>();
            }
        }

        public void Save()
        {
            if (null != this.User)
            {
                bool lbNeedsUpdate = false;
                if (this.User is MaxMembershipUser && ((MaxMembershipUser)this.User).IsPasswordResetNeeded != this.IsPasswordResetNeeded)
                {
                    ((MaxMembershipUser)this.User).IsPasswordResetNeeded = this.IsPasswordResetNeeded;
                    lbNeedsUpdate = true;
                }

                if (!string.IsNullOrEmpty(this.Email) && this.Email.Length > 0 && this.User.Email != this.Email)
                {
                    this.User.Email = this.Email;
                    lbNeedsUpdate = true;
                }

                if (!string.IsNullOrEmpty(this.Comment) && this.User.Comment != this.Comment)
                {
                    this.User.Comment = this.Comment;
                    lbNeedsUpdate = true;
                }

                if (lbNeedsUpdate)
                {
                    Membership.UpdateUser(this.User);
                }

                if (!string.IsNullOrEmpty(this.Password))
                {
                    MaxMembershipUser.SetPassword(this.User, this.Password);
                }

                if (Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Admin - App"))
                {
                    if (!Roles.RoleExists("Admin - App"))
                    {
                        Roles.CreateRole("Admin - App");
                    }

                    if (this.IsAdmin && !Roles.IsUserInRole(this.User.UserName, "Admin - App"))
                    {
                        Roles.AddUserToRole(this.User.UserName, "Admin - App");
                    }
                    else if (!this.IsAdmin && Roles.IsUserInRole(this.User.UserName, "Admin - App"))
                    {
                        Roles.RemoveUserFromRole(this.User.UserName, "Admin - App");
                    }
                }

                if (Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Admin - App") || Roles.IsUserInRole("Admin - Manage Users"))
                {
                    if (!Roles.RoleExists("Admin - Manage Users"))
                    {
                        Roles.CreateRole("Admin - Manage Users");
                    }

                    if (this.IsUserManager && !Roles.IsUserInRole(this.User.UserName, "Admin - Manage Users"))
                    {
                        Roles.AddUserToRole(this.User.UserName, "Admin - Manage Users");
                    }
                    else if (!this.IsUserManager && Roles.IsUserInRole(this.User.UserName, "Admin - Manage Users"))
                    {
                        Roles.RemoveUserFromRole(this.User.UserName, "Admin - Manage Users");
                    }
                }
            }
        }

        public bool Delete()
        {
            if (null != this.User && !string.IsNullOrEmpty(this.User.UserName))
            {
                return Membership.DeleteUser(this.User.UserName, true);
            }

            return false;
        }
    }
}
