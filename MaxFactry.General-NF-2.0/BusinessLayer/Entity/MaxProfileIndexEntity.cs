// <copyright file="MaxProfileIndexEntity.cs" company="Lakstins Family, LLC">
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
// <change date="12/11/2019" author="Brian A. Lakstins" description="Add method to LoadAll that can ignore PartitionKey">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class. Use parent methods instead of repository.">
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
    /// Entity to represent virtual text file in a web site.
    /// </summary>
    public class MaxProfileIndexEntity : MaxBaseIdIndexEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxProfileIndexEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxProfileIndexEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxProfileIndexEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxProfileIndexEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxProfileIndexDataModel DataModel
        {
            get
            {
                return (MaxProfileIndexDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new instance of the MaxProfileIndexEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxProfileIndexEntity class.</returns>
        public static MaxProfileIndexEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxProfileIndexEntity),
                typeof(MaxProfileIndexDataModel)) as MaxProfileIndexEntity;
        }
    }
}
