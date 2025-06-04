// <copyright file="MaxFileUploadViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="6/4/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="6/4/2025" author="Brian A. Lakstins" description="Updates for changes to base including removing versioning">
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
    public class MaxFileUploadViewModel : MaxFactry.Base.PresentationLayer.MaxBaseIdFileViewModel
    {
        private List<MaxFileUploadViewModel> _oSortedList = null;

        /// <summary>
        /// Initializes a new instance of the MaxFileUploadViewModel class
        /// </summary>
        public MaxFileUploadViewModel()
            : base()
        {
            this.Entity = MaxFileUploadEntity.Create();
        }

        /// <summary>
        /// Initializes a new instance of the MaxFileUploadViewModel class
        /// </summary>
        /// <param name="loEntity">Entity to use as data.</param>
        public MaxFileUploadViewModel(MaxEntity loEntity)
            : base(loEntity)
        {
        }

        public MaxFileUploadViewModel(string lsId)
        {
            MaxFileUploadEntity loEntity = MaxFileUploadEntity.Create();
            if (loEntity.LoadByDataKeyCache(lsId))
            {
                this.Entity = loEntity;
                this.Load();
            }
        }

        public string UploadName
        {
            get;
            set;
        }

        public string LocalUrl1
        {
            get
            {
                return "/f/" + this.Id.ToString();
            }
        }

        public string LocalUrl2
        {
            get
            {
                return "/f/" + this.Id.ToString() + "/" + this.Name.ToString();
            }
        }

        public bool IsDownload
        {
            get;
            set;
        }

        public virtual List<MaxFileUploadViewModel> GetSortedList()
        {
            if (null == this._oSortedList)
            {
                this._oSortedList = new List<MaxFileUploadViewModel>();
                string[] laKey = this.EntityIndex.GetSortedKeyList();
                for (int lnK = 0; lnK < laKey.Length; lnK++)
                {
                    MaxFileUploadViewModel loViewModel = new MaxFileUploadViewModel(this.EntityIndex[laKey[lnK]] as MaxFileUploadEntity);
                    loViewModel.Load();
                    this._oSortedList.Add(loViewModel);
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
                MaxFileUploadEntity loEntity = this.Entity as MaxFileUploadEntity;
                if (null != loEntity)
                {
                    loEntity.UploadName = this.UploadName;
                    loEntity.IsDownload = this.IsDownload;
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
                MaxFileUploadEntity loEntity = this.Entity as MaxFileUploadEntity;
                if (null != loEntity)
                {
                    this.UploadName = loEntity.UploadName;
                    this.IsDownload = loEntity.IsDownload;
                    return true;
                }
            }

            return false;
        }
    }
}
