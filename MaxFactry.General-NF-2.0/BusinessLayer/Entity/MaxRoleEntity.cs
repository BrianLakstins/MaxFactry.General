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
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.BusinessLayer;
	using MaxFactry.Base.DataLayer;
    using MaxFactry.General.DataLayer;

	/// <summary>
    /// Entity used to manage information about users for the MaxSecurityProvider.
    /// The aspect of this entity depends on the data used when loading the entity (core versus relation).
	/// </summary>
	public class MaxRoleEntity : MaxBaseIdEntity
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
                string lsR = string.Empty;
                if (this.Data.DataModel is MaxRoleDataModel)
                {
                    lsR = this.GetString(this.DataModel.RoleName);
                }
                else if (this.Data.DataModel is MaxRoleUserRelationDataModel)
                {
                    lsR = this.GetString(this.DataModelRelationUser.RoleName);
                }

                return lsR;
			}

			set
			{
                if (this.Data.DataModel is MaxRoleDataModel)
                {
                    this.Set(this.DataModel.RoleName, value);
                }
                else if (this.Data.DataModel is MaxRoleUserRelationDataModel)
                {
                    this.Set(this.DataModelRelationUser.RoleName, value);
                }
			}
		}

        /// <summary>
        /// Gets the userId.  Only available for relationship data.
        /// </summary>
        public Guid UserId
        {
            get
            {
                Guid loR = Guid.Empty;
                if (this.Data.DataModel is MaxRoleUserRelationDataModel)
                {
                    loR = this.GetGuid(this.DataModelRelationUser.ChildId);
                }

                return loR;
            }
        }

        /// <summary>
        /// Gets the RoleId.
        /// </summary>
        public Guid RoleId
        {
            get
            {
                Guid loR = Guid.Empty;
                if (this.Data.DataModel is MaxRoleDataModel)
                {
                    loR = this.GetGuid(this.DataModel.Id);
                }
                else if (this.Data.DataModel is MaxRoleUserRelationDataModel)
                {
                    loR = this.GetGuid(this.DataModelRelationUser.ParentId);
                }

                return loR;
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
        /// Gets the Data Model for this entity relationship to the user entity
        /// </summary>
        protected MaxRoleUserRelationDataModel DataModelRelationUser
        {
            get
            {
                return MaxDataLibrary.GetDataModel(typeof(MaxRoleUserRelationDataModel)) as MaxRoleUserRelationDataModel;
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
            MaxEntityList loR = MaxEntityList.Create(this.GetType());
            string lsCacheAllDataKey = this.GetCacheKey() + "LoadAll";
            MaxDataList loDataAllList = MaxCacheRepository.Get(this.GetType(), lsCacheAllDataKey, typeof(MaxDataList)) as MaxDataList;
            if (null != loDataAllList)
            {
                for (int lnD = 0; lnD < loDataAllList.Count; lnD++)
                {
                    if (loDataAllList[lnD].Get(this.DataModel.RoleName).Equals(lsRoleName))
                    {
                        MaxEntity loEntity = MaxBusinessLibrary.GetEntity(this.GetType(), loDataAllList[lnD]);
                        loR.Add(loEntity);
                    }
                }
            }
            else
            {
                string lsCacheDataKey = this.GetCacheKey() + "LoadAllByRole/" + MaxConvertLibrary.ConvertToString(typeof(object), lsRoleName);
                MaxDataList loDataList = MaxCacheRepository.Get(this.GetType(), lsCacheDataKey, typeof(MaxDataList)) as MaxDataList;
                if (null == loDataList)
                {
                    loDataList = MaxSecurityRepository.SelectAllByProperty(this.Data, this.DataModel.RoleName, lsRoleName);
                    MaxCacheRepository.Set(this.GetType(), lsCacheDataKey, loDataList);
                }

                loR = MaxEntityList.Create(this.GetType(), loDataList);
            }

            return loR;
        }

        /// <summary>
        /// Deletes a role based on the role name
        /// </summary>
        /// <param name="lsRoleName">Name of the Role.</param>
        /// <returns>True if successful.</returns>
        public bool DeleteByRoleName(string lsRoleName)
        {
            MaxDataList loDataList = MaxSecurityRepository.SelectAllByProperty(this.Data, this.DataModel.RoleName, lsRoleName);
            bool lbR = true;
            for (int lnD = 0; lnD < loDataList.Count; lnD++)
            {
                MaxRoleEntity loEntity = MaxRoleEntity.Create();
                loEntity.Load(loDataList[lnD]);
                MaxDataList loRelationList = MaxRoleUserRelationRepository.SelectAllByProperty(new MaxData(this.DataModelRelationUser), this.DataModelRelationUser.ParentId, loEntity.RoleId);
                for (int lnR = 0; lnR < loRelationList.Count; lnR++)
                {
                    MaxRoleUserRelationRepository.Delete(loRelationList[lnR]);
                }

                if (!MaxSecurityRepository.Delete(loDataList[lnD]))
                {
                    lbR = false;
                }

                string lsCacheKey = this.GetCacheKey() + "Load*";
                MaxCacheRepository.Remove(this.GetType(), lsCacheKey);
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
            string lsCacheDataKey = this.GetCacheKey() + "LoadAllByUserId/" + loUserId.ToString();
            MaxDataList loDataList = MaxCacheRepository.Get(this.GetType(), lsCacheDataKey, typeof(MaxDataList)) as MaxDataList;
            if (null == loDataList)
            {
                loDataList = MaxRoleUserRelationRepository.SelectAllByProperty(new MaxData(this.DataModelRelationUser), this.DataModelRelationUser.ChildId, loUserId);
                MaxCacheRepository.Set(this.GetType(), lsCacheDataKey, loDataList);
            }

            loR = MaxEntityList.Create(this.GetType(), loDataList);
            return loR;
        }

        /// <summary>
        /// Loads all role entity relations that match the given role that have not been deleted.
        /// </summary>
        /// <param name="lsRoleName">The role to load.</param>
        /// <returns>List of users.</returns>
        public MaxEntityList LoadAllRelationByRoleNameCache(string lsRoleName)
        {
            MaxEntityList loRoleList = this.LoadAllByRoleCache(lsRoleName);
            MaxEntityList loR = MaxEntityList.Create(typeof(MaxRoleEntity));
            for (int lnR = 0; lnR < loRoleList.Count; lnR++)
            {
                Guid loRoleId = ((MaxRoleEntity)loRoleList[lnR]).RoleId;
                string lsCacheDataKey = this.GetCacheKey() + "LoadAllRelationByRoleName/" + loRoleId.ToString();
                MaxDataList loDataList = MaxCacheRepository.Get(this.GetType(), lsCacheDataKey, typeof(MaxDataList)) as MaxDataList;
                if (null == loDataList)
                {
                    loDataList = MaxRoleUserRelationRepository.SelectAllByProperty(new MaxData(this.DataModelRelationUser), this.DataModelRelationUser.ParentId, loRoleId);
                    MaxCacheRepository.Set(this.GetType(), lsCacheDataKey, loDataList);
                }

                for (int lnD = 0; lnD < loDataList.Count; lnD++)
                {
                    MaxRoleEntity loEntityRelationUser = MaxRoleEntity.Create();
                    loEntityRelationUser.Load(loDataList[lnD]);
                    loR.Add(loEntityRelationUser);
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
            MaxIndex loMaxIndex = new MaxIndex();
            MaxEntityList loEntityList = this.LoadAllByUserIdCache(loUserId);
            for (int lnE = 0; lnE < loEntityList.Count; lnE++)
            {
                loMaxIndex.Add(((MaxRoleEntity)loEntityList[lnE]).RoleId.ToString(), true);
            }

            //// Get all roles so only existing ones can be added to a user.
            MaxEntityList loAllRoleEntityList = this.LoadAllCache();
            //// Go through all Roles and add any that match names, but are not already associated with the user.
            for (int lnE = 0; lnE < loAllRoleEntityList.Count; lnE++)
            {
                MaxRoleEntity loEntity = (MaxRoleEntity)loAllRoleEntityList[lnE];
                if (!loMaxIndex.Contains(loEntity.RoleId.ToString()))
                {
                    foreach (string lsRole in laRoleName)
                    {
                        if (loEntity.RoleName.ToLower().Equals(lsRole.ToLower()))
                        {
                            // Add the role to the user
                            MaxData loData = new MaxData(this.DataModelRelationUser);
                            loData.Set(this.DataModelRelationUser.RoleName, loEntity.RoleName);
                            loData.Set(this.DataModelRelationUser.ParentId, loEntity.RoleId);
                            loData.Set(this.DataModelRelationUser.ChildId, loUserId);
                            loData.Set(this.DataModelRelationUser.StorageKey, MaxDataLibrary.GetStorageKey(loData));
                            MaxRoleUserRelationRepository.Insert(loData);
                        }
                    }
                }
            }

            string lsCacheKey = this.GetCacheKey() + "Load*";
            MaxCacheRepository.Remove(this.GetType(), lsCacheKey);
        }

        /// <summary>
        /// Removes roles from a user.
        /// </summary>
        /// <param name="loUserId">The Id of the user to update.</param>
        /// <param name="laRoleName">A list of the names of the roles.</param>
        public void RemoveRoles(Guid loUserId, string[] laRoleName)
        {
            //// Create an List of RoleId that are already associated with this user.
            MaxIndex loMaxIndex = new MaxIndex();
            MaxEntityList loEntityList = this.LoadAllByUserIdCache(loUserId);
            for (int lnE = 0; lnE < loEntityList.Count; lnE++)
            {
                loMaxIndex.Add(((MaxRoleEntity)loEntityList[lnE]).RoleId.ToString(), true);
            }

            //// Get all roles so only existing ones can be added to a user.
            MaxEntityList loAllRoleEntityList = this.LoadAllCache();
            //// Go through all Roles and add any that match names, but are not already associated with the user.
            for (int lnE = 0; lnE < loAllRoleEntityList.Count; lnE++)
            {
                MaxRoleEntity loEntity = (MaxRoleEntity)loAllRoleEntityList[lnE];
                if (loMaxIndex.Contains(loEntity.RoleId.ToString()))
                {
                    foreach (string lsRole in laRoleName)
                    {
                        if (loEntity.RoleName.ToLower().Equals(lsRole.ToLower()))
                        {
                            // Remote the role from the user
                            MaxData loData = new MaxData(this.DataModelRelationUser);
                            loData.Set(this.DataModelRelationUser.RoleName, loEntity.RoleName);
                            loData.Set(this.DataModelRelationUser.ParentId, loEntity.RoleId);
                            loData.Set(this.DataModelRelationUser.ChildId, loUserId);
                            loData.Set(this.DataModelRelationUser.StorageKey, MaxDataLibrary.GetStorageKey(loData));
                            MaxRoleUserRelationRepository.Delete(loData);
                        }
                    }
                }
            }

            string lsCacheKey = this.GetCacheKey() + "Load*";
            MaxCacheRepository.Remove(this.GetType(), lsCacheKey);
        }
    }
}
