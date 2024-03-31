// <copyright file="MaxStyleFileEntity.cs" company="Lakstins Family, LLC">
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
// <change date="7/14/2016" author="Brian A. Lakstins" description="Initial creation">
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
    public class MaxStyleFileEntity : MaxBaseIdVersionedEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxStyleFileEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxStyleFileEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxStyleFileEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxStyleFileEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets or sets the text content
        /// </summary>
        public string Content
        {
            get
            {
                string lsR = this.GetString(this.DataModel.Content);

                return lsR;
            }

            set
            {
                this.Set(this.DataModel.Content, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimized text
        /// </summary>
        public string ContentMin
        {
            get
            {
                string lsR = this.GetString(this.DataModel.ContentMin);

                return lsR;
            }

            set
            {
                this.Set(this.DataModel.ContentMin, value);
            }
        }

        /// <summary>
        /// Gets or sets the Content name
        /// </summary>
        public string ContentName
        {
            get
            {
                return this.GetString(this.DataModel.ContentName);
            }

            set
            {
                this.Set(this.DataModel.ContentName, value);
            }
        }

        /// <summary>
        /// Gets or sets the File Version
        /// </summary>
        public string FileVersion
        {
            get
            {
                return this.GetString(this.DataModel.FileVersion);
            }

            set
            {
                this.Set(this.DataModel.FileVersion, value);
            }
        }

        /// <summary>
        /// Gets or sets the Script Type
        /// </summary>
        public string StyleType
        {
            get
            {
                return this.GetString(this.DataModel.StyleType);
            }

            set
            {
                this.Set(this.DataModel.StyleType, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxStyleFileDataModel DataModel
        {
            get
            {
                return (MaxStyleFileDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new instance of the MaxStyleFileEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxStyleFileEntity class.</returns>
        public static MaxStyleFileEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxStyleFileEntity),
                typeof(MaxStyleFileDataModel)) as MaxStyleFileEntity;
        }

        /// <summary>
        /// Gets a string that can be used to sort a list of this entity.
        /// </summary>
        /// <returns>Lowercase version of Name passed to 100 characters.</returns>
        public override string GetDefaultSortString()
        {
            return this.Name.ToLowerInvariant().PadRight(500, ' ') + base.GetDefaultSortString();
        }
    }
}
