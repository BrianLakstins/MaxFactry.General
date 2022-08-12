// <copyright file="MaxEmailEntity.cs" company="Lakstins Family, LLC">
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
// <change date="9/17/2020" author="Brian A. Lakstins" description="Add loading by relation">
// <change date="5/18/2021" author="Brian A. Lakstins" description="remove loading by relation since in base class">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.General.DataLayer;

    /// <summary>
    /// Entity to represent virtual text file in a web site.
    /// </summary>
    public class MaxEmailEntity : MaxBaseEmailEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxEmailEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxEmailEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxEmailDataModel DataModel
        {
            get
            {
                return (MaxEmailDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxVirtualTextFileEntity class.</returns>
        public static MaxEmailEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxEmailEntity),
                typeof(MaxEmailDataModel)) as MaxEmailEntity;
        }

        /// <summary>
        /// Runs archive process and then inserts a new record
        /// </summary>
        /// <param name="loId">Id for the new record</param>
        /// <returns>true if inserted.  False if cannot be inserted.</returns>
        public override bool Insert(Guid loId)
        {
            this.ArchiveCreatedOver30();
            bool lbR = base.Insert(loId);
            return lbR;
        }

        public int ArchiveCreatedOver30()
        {
            int lnR = 0;
            //// Prevent running archive process more than once per 24 hours
            if (this.CanProcessArchive(new TimeSpan(24, 0, 0)))
            {
                lnR = this.Archive(DateTime.UtcNow.Date.AddDays(-30), DateTime.MinValue, false);
            }

            return lnR;
        }
    }
}
