// <copyright file="MaxUserViewModel.cs" company="Lakstins Family, LLC">
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
// </changelog>
#endregion

namespace MaxFactry.General.PresentationLayer
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.PresentationLayer;
    using MaxFactry.General.BusinessLayer;

    /// <summary>
    /// View model base
    /// </summary>
    public class MaxUserViewModel : MaxBaseIdViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MaxUserViewModel class
        /// </summary>
        public MaxUserViewModel()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxUserViewModel class
        /// </summary>
        /// <param name="loEntity">Entity to use as data.</param>
        public MaxUserViewModel(MaxEntity loEntity)
            : base(loEntity)
        {
        }

        public MaxUserViewModel(string lsId) : base(lsId)
        {
        }

        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        public virtual string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the email address related to the user.
        /// </summary>
        public virtual string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Comment related to the user
        /// </summary>
        public virtual string Comment
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
                MaxUserEntity loEntity = this.Entity as MaxUserEntity;
                if (null != loEntity)
                {
                    loEntity.UserName = this.UserName;
                    loEntity.Comment = this.Comment;
                    loEntity.Email = this.Email;
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
                MaxUserEntity loEntity = this.Entity as MaxUserEntity;
                if (null != loEntity)
                {
                    this.Comment = loEntity.Comment;
                    this.Email = loEntity.Email;
                    this.UserName = loEntity.UserName;
                    return true;
                }
            }

            return false;
        }
    }
}
