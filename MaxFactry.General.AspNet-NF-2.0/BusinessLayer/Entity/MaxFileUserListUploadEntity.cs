// <copyright file="MaxFileUserListUploadEntity.cs" company="Lakstins Family, LLC">
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
// <change date="9/10/2020" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.BusinessLayer
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer ;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.General.AspNet.DataLayer;
    using MaxFactry.General.BusinessLayer;

    /// <summary>
    /// Entity to represent file uploaded containing a list of users
    /// </summary>
    public class MaxFileUserListUploadEntity : MaxFileUploadEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxFileUserListUploadEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxFileUserListUploadEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        public string Format
        {
            get
            {
                return this.GetString(this.UserListDataModel.Format);
            }

            set
            {
                this.Set(this.UserListDataModel.Format, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxFileUserListUploadDataModel UserListDataModel
        {
            get
            {
                return (MaxFileUserListUploadDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new instance of the MaxFileUserListUploadEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxFileUserListUploadEntity class.</returns>
        public new static MaxFileUserListUploadEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxFileUserListUploadEntity),
                typeof(MaxFileUserListUploadDataModel)) as MaxFileUserListUploadEntity;
        }

    }
}
