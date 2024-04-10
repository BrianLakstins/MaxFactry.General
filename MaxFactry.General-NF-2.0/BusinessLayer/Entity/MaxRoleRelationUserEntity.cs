// <copyright file="MaxRoleRelationUserEntity.cs" company="Lakstins Family, LLC">
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
// <change date="3/20/2024" author="Brian A. Lakstins" description="Happy birthday to my mom.  Sara Jean Lakstins (Cartwright) - 3/20/1944 to 3/14/2019.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Initial creation.">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
	using System;
	using MaxFactry.Base.BusinessLayer;
	using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.General.DataLayer;

	/// <summary>
    /// Entity used to manage information about users related to roles.
	/// </summary>
	public class MaxRoleRelationUserEntity : MaxBaseRelationGuidKeyEntity
	{
		/// <summary>
        /// Initializes a new instance of the MaxRoleEntity class
		/// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxRoleRelationUserEntity(MaxData loData)
			: base(loData)
		{
		}

        /// <summary>
        /// Initializes a new instance of the MaxRoleEntity class
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxRoleRelationUserEntity(Type loDataModelType)
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

        public Guid UserId
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

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxRoleRelationUserDataModel DataModel
        {
            get
            {
                return (MaxRoleRelationUserDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new entity initialized with the data.
        /// </summary>
        /// <returns>A new entity.</returns>
        public static MaxRoleRelationUserEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxRoleRelationUserEntity),
                typeof(MaxRoleRelationUserDataModel)) as MaxRoleRelationUserEntity;
        }

        protected override string GetDataName(MaxDataModel loDataModel, string lsPropertyName)
        {
            if (lsPropertyName == this.GetPropertyName(() => this.RoleId))
            {
                return this.DataModel.ParentId;
            }
            else if (lsPropertyName == this.GetPropertyName(() => this.UserId))
            {
                return this.DataModel.ChildId;
            }

            return base.GetDataName(loDataModel, lsPropertyName);
        }

        public MaxEntityList LoadAllByRoleIdCache(Guid loRoleId)
        {
            return this.LoadAllByParentIdCache(loRoleId);
        }

        public MaxEntityList LoadAllByUserIdCache(Guid loUserId)
        {
            return this.LoadAllByChildIdCache(loUserId);
        }
    }
}
