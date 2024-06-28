// <copyright file="MaxRoleEntity.cs" company="Lakstins Family, LLC">
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
// <change date="6/4/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="5/23/2017" author="Brian A. Lakstins" description="Updated to cache MaxData instead of MaxEntity">
// <change date="11/8/2017" author="Brian A. Lakstins" description="Remove unnecessary setlist calls">
// <change date="11/10/2017" author="Brian A. Lakstins" description="Fix delete of role.">
// <change date="1/16/2021" author="Brian A. Lakstins" description="Update definition of cache keys.">
// <change date="2/19/2021" author="Brian A. Lakstins" description="Use standard methods to load relationships.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class. Use parent methods instead of repository.">
// <change date="4/28/2024" author="Brian A. Lakstins" description="Integrate with permissions.">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.BusinessLayer;
	using MaxFactry.Base.DataLayer;
    using MaxFactry.General.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    /// <summary>
    /// Entity used to manage information about users for the MaxSecurityProvider.
    /// The aspect of this entity depends on the data used when loading the entity (core versus relation).
    /// </summary>
    public class MaxRoleEntity : MaxBaseGuidKeyEntity
	{
		/// <summary>
        /// Initializes a new instance of the MaxRoleEntity class
		/// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxRoleEntity(MaxData loData)
			: base(loData)
		{
		}

        /// <summary>
        /// Initializes a new instance of the MaxRoleEntity class
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxRoleEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
		public string RoleName
		{
			get
			{
               return this.GetString(this.DataModel.RoleName);
			}

			set
			{
                this.Set(this.DataModel.RoleName, value);
			}
		}

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxRoleDataModel DataModel
        {
            get
            {
                return (MaxRoleDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new entity initialized with the data.
        /// </summary>
        /// <returns>A new entity.</returns>
        public static MaxRoleEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxRoleEntity),
                typeof(MaxRoleDataModel)) as MaxRoleEntity;
        }

        /// <summary>
        /// Loads all role entities that match the given role that have not been deleted.
        /// </summary>
        /// <param name="lsRoleName">The role to load.</param>
        /// <returns>List of users.</returns>
        public MaxEntityList LoadAllByRoleCache(string lsRoleName)
        {
            return this.LoadAllByPropertyCache(this.DataModel.RoleName, lsRoleName);
        }

        /// <summary>
        /// Deletes a role based on the role name
        /// </summary>
        /// <param name="lsRoleName">Name of the Role.</param>
        /// <returns>True if successful.</returns>
        public bool DeleteByRoleName(string lsRoleName)
        {
            bool lbR = true;
            MaxEntityList loList = this.LoadAllByRoleCache(lsRoleName);
            for (int lnE = 0; lnE < loList.Count; lnE++)
            {
                MaxRoleEntity loEntity = loList[lnE] as MaxRoleEntity;
                MaxEntityList loRelationList = MaxRoleRelationUserEntity.Create().LoadAllByRoleIdCache(loEntity.Id);
                for (int lnR = 0; lnR < loRelationList.Count; lnR++)
                {
                    if (lbR)
                    {
                        lbR = loRelationList[lnR].Delete();
                    }
                }

                if (lbR)
                {
                    lbR = loEntity.Delete();
                }
            }

            return lbR;
        }

        /// <summary>
        /// Loads all role entities that match the given role that have not been deleted.
        /// </summary>
        /// <param name="loUserId">The Id of the user to load roles for.</param>
        /// <returns>List of users.</returns>
        public MaxEntityList LoadAllByUserIdCache(Guid loUserId)
        {
            MaxEntityList loR = MaxEntityList.Create(this.GetType());
            MaxEntityList loRelationList = MaxRoleRelationUserEntity.Create().LoadAllByUserIdCache(loUserId);
            for (int lnR = 0; lnR < loRelationList.Count; lnR++)
            {
                MaxRoleRelationUserEntity loRelation = loRelationList[lnR] as MaxRoleRelationUserEntity;
                MaxRoleEntity loEntity = MaxRoleEntity.Create();
                if (loEntity.LoadByIdCache(loRelation.RoleId))
                {
                    loR.Add(loEntity);
                }
            }

            return loR;
        }

        /// <summary>
        /// Adds roles to a user
        /// </summary>
        /// <param name="loUserId">The Id of the user.</param>
        /// <param name="laRoleName">A list of names of roles to add.</param>
        public void AddRoles(Guid loUserId, string[] laRoleName)
        {
            //// Create an List of RoleId that are already associated with this user.
            MaxIndex loRoleIdExistingIndex = new MaxIndex();
            MaxEntityList loRelationList = MaxRoleRelationUserEntity.Create().LoadAllByUserIdCache(loUserId);
            for (int lnR = 0; lnR < loRelationList.Count; lnR++)
            {
                MaxRoleRelationUserEntity loRelation = loRelationList[lnR] as MaxRoleRelationUserEntity;
                loRoleIdExistingIndex.Add(loRelation.RoleId.ToString(), true);
            }

            //// Create an index of roles for quick lookup by name
            MaxEntityList loAllRoleEntityList = this.LoadAllCache();
            MaxIndex loRoleNameIndex = new MaxIndex();
            for (int lnE = 0; lnE < loAllRoleEntityList.Count; lnE++)
            {
                loRoleNameIndex.Add(((MaxRoleEntity)loAllRoleEntityList[lnE]).RoleName, loAllRoleEntityList[lnE]);
            }

            foreach (string lsRole in laRoleName)
            {
                MaxRoleEntity loEntity = loRoleNameIndex[lsRole] as MaxRoleEntity;
                if (null != loEntity && !loRoleIdExistingIndex.Contains(loEntity.Id.ToString()))
                {
                    MaxRoleRelationUserEntity loRelation = MaxRoleRelationUserEntity.Create();
                    loRelation.RoleId = loEntity.Id;
                    loRelation.UserId = loUserId;
                    loRelation.Name = lsRole;
                    loRelation.Insert();
                }
            }
        }

        /// <summary>
        /// Removes roles from a user.
        /// </summary>
        /// <param name="loUserId">The Id of the user to update.</param>
        /// <param name="laRoleName">A list of the names of the roles.</param>
        public void RemoveRoles(Guid loUserId, string[] laRoleName)
        {
            //// Create an index of roles for quick lookup by name
            MaxEntityList loAllRoleEntityList = this.LoadAllCache();
            MaxIndex loRoleNameIndex = new MaxIndex();
            for (int lnE = 0; lnE < loAllRoleEntityList.Count; lnE++)
            {
                MaxRoleEntity loEntity = loAllRoleEntityList[lnE] as MaxRoleEntity;
                loRoleNameIndex.Add(loEntity.RoleName, loEntity);
            }

            MaxEntityList loRelationList = MaxRoleRelationUserEntity.Create().LoadAllByUserIdCache(loUserId);
            for (int lnR = 0; lnR < loRelationList.Count; lnR++)
            {
                MaxRoleRelationUserEntity loRelation = loRelationList[lnR] as MaxRoleRelationUserEntity;
                foreach (string lsRole in laRoleName)
                {
                    MaxRoleEntity loEntity = loRoleNameIndex[lsRole] as MaxRoleEntity;
                    if (loEntity.Id == loRelation.RoleId)
                    {
                        loRelation.Delete();
                    }
                }
            }
        }

        public static List<MaxIndex> GetPermissionList(Guid loId)
        {
            List<MaxIndex> loR = new List<MaxIndex>();
            MaxRoleRelationPermissionEntity loRelation = MaxRoleRelationPermissionEntity.Create();
            MaxEntityList loRelationList = loRelation.LoadAllByRoleIdCache(loId);
            for (int lnE = 0; lnE < loRelationList.Count; lnE++)
            {
                loRelation = loRelationList[lnE] as MaxRoleRelationPermissionEntity;
                loR.Add(loRelation.MapIndex(
                    loRelation.GetPropertyName(() => loRelation.Id),
                    loRelation.GetPropertyName(() => loRelation.DataKey),
                    loRelation.GetPropertyName(() => loRelation.Name),
                    loRelation.GetPropertyName(() => loRelation.PermissionId),
                    loRelation.GetPropertyName(() => loRelation.Permission)));
            }

            return loR;
        }

        public override MaxIndex MapIndex(params string[] laPropertyNameList)
        {
            MaxIndex loR = base.MapIndex(laPropertyNameList);
            foreach (string lsPropertyName in laPropertyNameList)
            {
                if (lsPropertyName == "PermissionSelectedList")
                {
                    List<string> loPermissionList = new List<string>();
                    MaxRoleRelationPermissionEntity loRelation = MaxRoleRelationPermissionEntity.Create();
                    MaxEntityList loRelationList = loRelation.LoadAllByRoleIdCache(this.Id);
                    for (int lnE = 0; lnE < loRelationList.Count; lnE++)
                    {
                        loRelation = loRelationList[lnE] as MaxRoleRelationPermissionEntity;
                        loPermissionList.Add(loRelation.PermissionId.ToString() + loRelation.Permission.ToString());
                    }

                    loR.Add("PermissionSelectedList", loPermissionList.ToArray());
                }
            }

            return loR;
        }
    }
}
