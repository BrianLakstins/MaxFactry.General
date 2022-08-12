// <copyright file="MaxUSStateEntity.cs" company="Lakstins Family, LLC">
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
// <change date="12/2/2014" author="Brian A. Lakstins" description="Initial Release">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
    using System;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.General.DataLayer;

    /// <summary>
    /// Entity to represent content in a web site.
    /// </summary>
    public class MaxUSStateEntity : MaxEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxUSStateEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxUSStateEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxUSStateEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxUSStateEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets the name of the US State
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetString(this.DataModel.Name);
            }
        }

        /// <summary>
        /// Gets the abbreviation for the US State
        /// </summary>
        public string Abbreviation
        {
            get
            {
                return this.GetString(this.DataModel.Abbreviation);
            }
        }

        /// <summary>
        /// Gets the code for the US State
        /// </summary>
        public int Code
        {
            get
            {
                return this.GetInt(this.DataModel.Code);
            }
        }

        /// <summary>
        /// Gets the integer status of the US State
        /// </summary>
        public int Status
        {
            get
            {
                return this.GetInt(this.DataModel.Status);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxUSStateDataModel DataModel
        {
            get
            {
                return (MaxUSStateDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a US State Entity
        /// </summary>
        /// <returns>Blank US State Entity</returns>
        public static MaxUSStateEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxUSStateEntity),
                typeof(MaxUSStateDataModel)) as MaxUSStateEntity;
        }

        /// <summary>
        /// Loads all entities of this type based on status.
        /// </summary>
        /// <param name="laStatus">Valid status values.</param>
        /// <returns>List of entities</returns>
        public MaxEntityList LoadAllByStatus(int[] laStatus)
        {
            MaxDataList loDataList = MaxUSStateRepository.SelectAllByStatus(this.Data, laStatus);
            MaxEntityList loEntityList = MaxEntityList.Create(this.GetType(), loDataList);
            return loEntityList;
        }

        /// <summary>
        /// Gets a string that can be used to sort a list of this entity.
        /// </summary>
        /// <returns>Lowercase version of Name passed to 100 characters.</returns>
        public override string GetDefaultSortString()
        {
            return this.Name.ToLower() + new string(' ', 100 - this.Name.Length) + base.GetDefaultSortString();
        }
    }
}
