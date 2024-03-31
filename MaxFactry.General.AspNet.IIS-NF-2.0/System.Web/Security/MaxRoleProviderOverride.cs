// <copyright file="MaxRoleProvider.cs" company="Lakstins Family, LLC">
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
// <change date="2/23/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="5/15/2014" author="Brian A. Lakstins" description="Remove local storage of application name and application Id.">
// <change date="6/27/2014" author="Brian A. Lakstins" description="Remove dependency on AppId.">
// <change date="10/17/2014" author="Brian A. Lakstins" description="Updated to not keep config in memory.">
// <change date="12/18/2014" author="Brian A. Lakstins" description="Updates to follow core data access patterns.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class. Use new MaxRoleRelationUserEntity class">
// </changelog>
#endregion

namespace System.Web.Security
{
	using System;
	using System.Collections;
    using System.Web.Security;
	using MaxFactry.Core;
	using MaxFactry.Base.BusinessLayer;
    using MaxFactry.General.BusinessLayer;

	/// <summary>
	/// Provide role services using MaxFactryLibrary.
	/// </summary>
	public class MaxRoleProviderOverride : RoleProvider
	{
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return this.Config.ApplicationName;
            }

            set
            {
                this.Config.ApplicationName = value;
            }
        }

        /// <summary>
        /// Gets the configuration information.
        /// </summary>
        protected MaxUserConfigurationEntity Config
        {
            get
            {
                return MaxUserConfigurationEntity.Create().GetCurrent();
            }
        }

        /// <summary>
        /// Adds a new role to the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to create.</param>
        public override void CreateRole(string roleName)
        {
            if (!this.RoleExists(roleName))
            {
                MaxRoleEntity loEntity = MaxRoleEntity.Create();
                loEntity.RoleName = roleName;
                loEntity.IsActive = true;
                loEntity.Insert();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in the role data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to search for in the data source.</param>
        /// <returns>true if the role name already exists in the data source for the configured applicationName; otherwise, false.</returns>
        public override bool RoleExists(string roleName)
        {
            MaxEntityList loList = MaxRoleEntity.Create().LoadAllByRoleCache(roleName);
            if (loList.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>A string array containing the names of all the roles stored in the data source for the configured applicationName.</returns>
        public override string[] GetAllRoles()
        {
            MaxEntityList loList = MaxRoleEntity.Create().LoadAllCache();
            string lsRoleList = string.Empty;
            string lsDivider = Guid.NewGuid().ToString();
            for (int lnE = 0; lnE < loList.Count; lnE++)
            {
                MaxRoleEntity loEntity = (MaxRoleEntity)loList[lnE];
                lsRoleList += loEntity.RoleName + lsDivider;
            }

            string[] laRoleList = lsRoleList.Split(new string[] { lsDivider }, StringSplitOptions.RemoveEmptyEntries);
            return laRoleList;
        }

        /// <summary>
        /// Adds the specified user names to the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be added to the specified roles.</param>
        /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            MaxRoleEntity loEntity = MaxRoleEntity.Create();
            foreach (string lsUserName in usernames)
            {
                MembershipUser loUser = Membership.GetUser(lsUserName);
                Guid loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), loUser.ProviderUserKey);
                loEntity.AddRoles(loUserId, roleNames);
            }
        }

        /// <summary>
        /// Removes a role from the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to delete.</param>
        /// <param name="throwOnPopulatedRole">If true, throw an exception if roleName has one or more members and do not delete roleName.</param>
        /// <returns>true if the role was successfully deleted; otherwise, false.</returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (throwOnPopulatedRole)
            {
                if (this.GetUsersInRole(roleName).Length > 0)
                {
                    throw new MaxException(string.Format("Cannot delete role {0}.  {0} has one or more members.", roleName));
                }
            }

            return MaxRoleEntity.Create().DeleteByRoleName(roleName);
        }

		/// <summary>
		/// Gets a value indicating whether the specified user is in the specified role for the configured applicationName.
		/// </summary>
		/// <param name="username">The user name to search for.</param>
		/// <param name="roleName">The role to search in.</param>
		/// <returns>true if the specified user is in the specified role for the configured applicationName; otherwise, false.</returns>
		public override bool IsUserInRole(string username, string roleName)
		{
            string[] laRoleName = this.GetRolesForUser(username);
            foreach (string lsRoleName in laRoleName)
            {
                if (lsRoleName.Equals(roleName))
                {
                    return true;
                }
            }

            return false;
		}
		
		/// <summary>
		/// Gets a list of the roles that a specified user is in for the configured applicationName.
		/// </summary>
		/// <param name="username">The user to return a list of roles for.</param>
		/// <returns>A string array containing the names of all the roles that the specified user is in for the configured applicationName.</returns>
		public override string[] GetRolesForUser(string username)
		{
            MembershipUser loUser = Membership.GetUser(username);
            string lsRoleList = string.Empty;
            string lsDivider = Guid.NewGuid().ToString();
            if (null != loUser)
            {
                Guid loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), loUser.ProviderUserKey);
                MaxEntityList loList = MaxRoleEntity.Create().LoadAllByUserIdCache(loUserId);
                for (int lnE = 0; lnE < loList.Count; lnE++)
                {
                    MaxRoleEntity loEntity = (MaxRoleEntity)loList[lnE];
                    lsRoleList += loEntity.RoleName + lsDivider;
                }
            }

            string[] laRoleList = lsRoleList.Split(new string[] { lsDivider }, StringSplitOptions.RemoveEmptyEntries);
            return laRoleList;
		}

        /// <summary>
        /// Gets a list of users in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to get the list of users for.</param>
        /// <returns>A string array containing the names of all the users who are members of the specified role for the configured applicationName.</returns>
        public override string[] GetUsersInRole(string roleName)
        {
            string lsR = string.Empty;
            string lsDivider = Guid.NewGuid().ToString();
            MaxRoleEntity loEntity = MaxRoleEntity.Create();
            MaxEntityList loList = loEntity.LoadAllByRoleCache(roleName);
            if (loList.Count == 1)
            {
                loEntity = loList[0] as MaxRoleEntity;
                MaxRoleRelationUserEntity loRelation = MaxRoleRelationUserEntity.Create();
                MaxEntityList loRelationList = loRelation.LoadAllByRoleIdCache(loEntity.Id);
                for (int lnE = 0; lnE < loRelationList.Count; lnE++)
                {
                    loRelation = loRelationList[lnE] as MaxRoleRelationUserEntity;
                    MembershipUser loUser = Membership.GetUser(loRelation.UserId);
                    if (null != loUser)
                    {
                      lsR += loUser.UserName + lsDivider;
                    }
                }
            }

            string[] laR = lsR.Split(new string[] { lsDivider }, StringSplitOptions.RemoveEmptyEntries);
            return laR;
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The user name pattern to search for.</param>
        /// <returns>A string array containing the names of all the users where the user name matches usernameToMatch and the user is a member of the specified role.</returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            string[] laUsersInRole = this.GetUsersInRole(roleName);
            string lsUserList = string.Empty;
            string lsDivider = Guid.NewGuid().ToString();
            for (int lnU = 0; lnU < laUsersInRole.Length; lnU++)
            {
                if (laUsersInRole[lnU].ToLowerInvariant().Contains(usernameToMatch.ToLowerInvariant()))
                {
                    lsUserList += laUsersInRole[lnU] + lsDivider;
                }
            }

            string[] laUserList = lsUserList.Split(new string[] { lsDivider }, StringSplitOptions.RemoveEmptyEntries);
            return laUserList;
        }

		/// <summary>
		/// Removes the specified user names from the specified roles for the configured applicationName.
		/// </summary>
		/// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
		/// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
		{
            MaxRoleEntity loEntity = MaxRoleEntity.Create();
            foreach (string lsUserName in usernames)
            {
                MembershipUser loUser = Membership.GetUser(lsUserName);
                Guid loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), loUser.ProviderUserKey);
                loEntity.RemoveRoles(loUserId, roleNames);
            }
		}
	}
}
