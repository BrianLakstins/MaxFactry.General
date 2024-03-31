// <copyright file="MaxFormValueEntity.cs" company="Lakstins Family, LLC">
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
// follAspNetg restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not claim that 
// you wrote the original software. If you use this software in a product, an 
// acknowledgment (see the follAspNetg) in the product documentation is required.
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
// <change date="1/23/2015" author="Brian A. Lakstins" description="Initial Release">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.BusinessLayer
{
    using System;
    using MaxFactry.Base.BusinessLayer ;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.General.AspNet.DataLayer;

    /// <summary>
    /// Entity to represent virtual text file in a web site.
    /// </summary>
    public class MaxFormValueEntity : MaxBaseIdEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxFormValueEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxFormValueEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        public Guid FormId
        {
            get
            {
                return this.GetGuid(this.DataModel.FormId);
            }

            set
            {
                this.Set(this.DataModel.FormId, value);
            }
        }

        public string Name
        {
            get
            {
                return this.GetString(this.DataModel.Name);
            }

            set
            {
                this.Set(this.DataModel.Name, value);
            }
        }

        public string Value
        {
            get
            {
                return this.GetString(this.DataModel.Value);
            }

            set
            {
                this.Set(this.DataModel.Value, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxFormValueDataModel DataModel
        {
            get
            {
                return (MaxFormValueDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxVirtualTextFileEntity class.</returns>
        public static MaxFormValueEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxFormValueEntity),
                typeof(MaxFormValueDataModel)) as MaxFormValueEntity;
        }

        public virtual MaxEntityList LoadAllByRelationId(Guid loRelationId)
        {
            MaxDataList loDataList = MaxBaseAspNetRepository.SelectAllByProperty(this.Data, this.DataModel.FormId, loRelationId);
            MaxEntityList loEntityList = MaxEntityList.Create(typeof(MaxFormValueEntity), loDataList);
            return loEntityList;
        }
    }
}
