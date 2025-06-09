// <copyright file="MaxUserAuthViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="11/3/2020" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.PresentationLayer
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.PresentationLayer;
    using MaxFactry.General.BusinessLayer;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Manages ClientId and ClientSecret related to a user
    /// </summary>
    public class MaxUserAuthViewModel : MaxBaseIdViewModel
    {
        private List<MaxUserAuthViewModel> _oSortedList = null;

        /// <summary>
        /// Initializes a new instance of the MaxUserAuthViewModel class
        /// </summary>
        public MaxUserAuthViewModel()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxUserAuthViewModel class
        /// </summary>
        /// <param name="loEntity">Entity to use as data.</param>
        public MaxUserAuthViewModel(MaxEntity loEntity)
            : base(loEntity)
        {
        }

        public MaxUserAuthViewModel(string lsId) : base(lsId)
        {
        }

        protected override void CreateEntity()
        {
            this.Entity = MaxUserAuthEntity.Create();
        }

        public virtual string UserKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public virtual string Name
        {
            get;
            set;
        }

        [Display(Name = "Client Id")]
        public virtual string ClientId
        {
            get;
            set;
        }

        [Display(Name = "Client Secret")]
        public virtual string ClientSecret
        {
            get;
            set;
        }

        public virtual string ClientSecretHash
        {
            get;
            set;
        }

        [Display(Name = "Scope List")]
        public virtual List<string> ScopeList
        {
            get;
            set;
        }

        [Display(Name = "Domain List")]
        public virtual List<string> DomainList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a sorted list of all 
        /// Can use Generic List if supported in the framework.
        /// </summary>
        /// <returns>List of ViewModels</returns>
        public List<MaxUserAuthViewModel> GetSortedListForUserKey(string lsUserKey)
        {
            if (null == this._oSortedList)
            {
                this._oSortedList = new List<MaxUserAuthViewModel>();
                string[] laKey = this.EntityIndex.GetSortedKeyList();
                for (int lnK = 0; lnK < laKey.Length; lnK++)
                {
                    MaxUserAuthViewModel loModel = new MaxUserAuthViewModel(this.EntityIndex[laKey[lnK]] as MaxEntity);
                    if (loModel.UserKey == lsUserKey)
                    {
                        this._oSortedList.Add(loModel);
                    }
                }
            }

            return this._oSortedList;
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
                MaxUserAuthEntity loEntity = this.Entity as MaxUserAuthEntity;
                if (null != loEntity)
                {
                    if (!string.IsNullOrEmpty(this.UserKey))
                    {
                        loEntity.UserKey = this.UserKey;
                    }

                    loEntity.Name = this.Name;
                    loEntity.ClientId = this.ClientId;
                    loEntity.ClientSecret = this.ClientSecret;
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
                MaxUserAuthEntity loEntity = this.Entity as MaxUserAuthEntity;
                if (null != loEntity)
                {
                    this.UserKey = loEntity.UserKey;
                    this.Name = loEntity.Name;
                    this.ClientId = loEntity.ClientId;
                    this.ClientSecret = loEntity.ClientSecret;
                    return true;
                }
            }

            return false;
        }
    }
}
