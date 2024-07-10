// <copyright file="MaxRoleRelationPermissionEntity.cs" company="Lakstins Family, LLC">
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
// <change date="6/28/2024" author="Brian A. Lakstins" description="Initial creation">
// <change date="7/10/2024" author="Brian A. Lakstins" description="Use a string for permission id">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using MaxFactry.Base.BusinessLayer;
	using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.Core;
    using MaxFactry.General.DataLayer;

	/// <summary>
    /// Entity used to manage information about permissions related to roles.
	/// </summary>
	public class MaxRoleRelationPermissionEntity : MaxBaseRelationGuidKeyEntity
	{
        /// <summary>
        /// Initializes a new instance of the MaxRoleRelationPermissionEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxRoleRelationPermissionEntity(MaxData loData)
			: base(loData)
		{
		}

        /// <summary>
        /// Initializes a new instance of the MaxRoleRelationPermissionEntity class
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxRoleRelationPermissionEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        public Guid RoleId
        {
            get
            {
                return this.ParentId;
            }

            set
            {
                this.ParentId = value;
            }
        }

        public Guid PermissionId
        {
            get
            {
                return this.ChildId;
            }

            set
            {
                this.ChildId = value;
            }
        }

        public long Permission
        {
            get
            {
                return this.GetLong(this.DataModel.Permission);
            }

            set
            {
                this.Set(this.DataModel.Permission, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxRoleRelationPermissionDataModel DataModel
        {
            get
            {
                return (MaxRoleRelationPermissionDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new entity initialized with the data.
        /// </summary>
        /// <returns>A new entity.</returns>
        public static MaxRoleRelationPermissionEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxRoleRelationPermissionEntity),
                typeof(MaxRoleRelationPermissionDataModel)) as MaxRoleRelationPermissionEntity;
        }

        /// <summary>
        /// Gets a guid based on an object type
        /// </summary>
        /// <param name="loObject">object to use to get Guid</param>
        /// <returns>Guid Id based on object type</returns>
        public static Guid GetPermissionId(string lsName)
        {
            Guid loR = Guid.Empty;
            try
            {
                MD5 loMD5 = MD5.Create();
                byte[] laHash = loMD5.ComputeHash(Encoding.UTF8.GetBytes(lsName));
                loR = new Guid(laHash);
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(typeof(MaxRoleRelationPermissionEntity), "GetPermissionId", MaxEnumGroup.LogCritical, "Exception getting permission id", loE));
            }

            return loR;
        }

        protected override string GetDataName(MaxDataModel loDataModel, string lsPropertyName)
        {
#if net4_52 || netcore2_1
            if (lsPropertyName == this.GetPropertyName(() => this.RoleId))
            {
                return this.DataModel.ParentId;
            }
#endif 
            return base.GetDataName(loDataModel, lsPropertyName);
        }

        public MaxEntityList LoadAllByRoleIdCache(Guid loRoleId)
        {
            return this.LoadAllByParentIdCache(loRoleId);
        }

        public MaxEntityList LoadAllByRoleNameCache(string lsRoleName)
        {
            MaxEntityList loR = new MaxEntityList(this.GetType());
            MaxEntityList loRoleList = MaxRoleEntity.Create().LoadAllByRoleCache(lsRoleName);
            for (int lnR = 0; lnR < loRoleList.Count; lnR++)
            {
                MaxRoleEntity loRoleEntity = loRoleList[lnR] as MaxRoleEntity;
                MaxEntityList loRoleRelationPermissionList = this.LoadAllByRoleIdCache(loRoleEntity.Id);
                for (int lnE = 0; lnE < loRoleRelationPermissionList.Count; lnE++)
                {
                    loR.Add(loRoleRelationPermissionList[lnE]);
                }
            }

            return loR;
        }
    }
}
