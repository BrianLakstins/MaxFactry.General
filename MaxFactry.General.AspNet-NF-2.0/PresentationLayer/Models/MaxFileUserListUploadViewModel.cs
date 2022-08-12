// <copyright file="MaxFileUserListUploadViewModel.cs" company="Lakstins Family, LLC">
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

namespace MaxFactry.General.AspNet.PresentationLayer
{
	using System;
    using System.Collections.Generic;
    using System.IO;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer ;
    using MaxFactry.General.AspNet.BusinessLayer;

	/// <summary>
	/// View model for content.
	/// </summary>
    public class MaxFileUserListUploadViewModel : MaxFileUploadViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MaxFileUserListUploadViewModel class
        /// </summary>
        public MaxFileUserListUploadViewModel()
            : base()
        {
            this.Entity = MaxFileUserListUploadEntity.Create();
        }

        /// <summary>
        /// Initializes a new instance of the MaxFileUserListUploadViewModel class
        /// </summary>
        /// <param name="loEntity">Entity to use as data.</param>
        public MaxFileUserListUploadViewModel(MaxEntity loEntity)
            : base(loEntity)
        {
        }

        public MaxFileUserListUploadViewModel(string lsId)
        {
            MaxFileUserListUploadEntity loEntity = MaxFileUserListUploadEntity.Create();
            Guid loId = MaxConvertLibrary.ConvertToGuid(this.GetType(), lsId);
            if (loEntity.LoadByIdCache(loId))
            {
                this.Entity = loEntity;
                this.Load();
            }
        }

        public string Format
        {
            get;
            set;
        }

        /// <summary>
        /// Loads the entity based on the Id property.
        /// Maps the current values of properties in the ViewModel to the Entity.
        /// </summary>
        /// <returns>True if successful. False if it cannot be mapped.</returns>
        protected override bool MapToEntity()
        {
            if (base.MapToEntity())
            {
                MaxFileUserListUploadEntity loEntity = this.Entity as MaxFileUserListUploadEntity;
                if (null != loEntity)
                {
                    loEntity.Format = this.Format;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Maps the properties of the Entity to the properties of the ViewModel.
        /// </summary>
        /// <returns>True if the entity exists.</returns>
        protected override bool MapFromEntity()
        {
            if (base.MapFromEntity())
            {
                MaxFileUserListUploadEntity loEntity = this.Entity as MaxFileUserListUploadEntity;
                if (null != loEntity)
                {
                    this.Format = loEntity.Format;
                    return true;
                }
            }

            return false;
        }
    }
}
