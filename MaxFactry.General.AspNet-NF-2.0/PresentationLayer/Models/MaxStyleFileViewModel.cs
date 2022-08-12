// <copyright file="MaxStyleFileViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="7/14/2016" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.PresentationLayer
{
	using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer ;
    using MaxFactry.General.AspNet.BusinessLayer;

	/// <summary>
	/// View model for content.
	/// </summary>
    public class MaxStyleFileViewModel : MaxFactry.Base.PresentationLayer.MaxBaseIdViewModel
	{
        private List<MaxStyleFileViewModel> _oSortedList = null;

        /// <summary>
        /// Initializes a new instance of the MaxStyleFileViewModel class
        /// </summary>
        public MaxStyleFileViewModel()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxStyleFileViewModel class
        /// </summary>
        /// <param name="loEntity">Entity to use as data.</param>
        public MaxStyleFileViewModel(MaxEntity loEntity)
            : base(loEntity)
        {
        }

        public MaxStyleFileViewModel(string lsId) : base(lsId)
        {
        }

        protected override void CreateEntity()
        {
            this.Entity = MaxStyleFileEntity.Create();
        }

        public virtual bool LoadFromName(string lsName)
        {
            MaxStyleFileEntity loEntity = MaxStyleFileEntity.Create().GetCurrent(lsName) as MaxStyleFileEntity;
            if (!Guid.Empty.Equals(loEntity.Id))
            {
                this.Entity = loEntity;
                return this.MapFromEntity();
            }

            return false;
        }

        public string Name
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public string ContentMin
        {
            get;
            set;
        }

        public string ContentName
        {
            get;
            set;
        }

        public string FileVersion
        {
            get;
            set;
        }

        public string StyleType
        {
            get;
            set;
        }

        public List<MaxStyleFileViewModel> GetSortedList()
        {
            if (null == this._oSortedList)
            {
                this._oSortedList = new List<MaxStyleFileViewModel>();
                SortedList<string, MaxStyleFileViewModel> loSortedList = new SortedList<string, MaxStyleFileViewModel>();
                string[] laKey = this.EntityIndex.GetSortedKeyList();
                for (int lnK = 0; lnK < laKey.Length; lnK++)
                {
                    MaxStyleFileViewModel loViewModel = new MaxStyleFileViewModel(this.EntityIndex[laKey[lnK]] as MaxStyleFileEntity);
                    loViewModel.Load();
                    string lsKey = loViewModel.Name.ToLowerInvariant();
                    
                    if (loSortedList.ContainsKey(lsKey))
                    {
                        int lnVersionCheck = MaxConvertLibrary.ConvertToInt(typeof(object), loViewModel.Version);
                        int lnVersionCurrent = MaxConvertLibrary.ConvertToInt(typeof(object), loSortedList[lsKey].Version);
                        if (lnVersionCheck > lnVersionCurrent)
                        {
                            loSortedList[lsKey] = loViewModel;
                        }
                    }
                    else
                    {
                        loSortedList.Add(lsKey, loViewModel);
                    }
                }

                this._oSortedList = new List<MaxStyleFileViewModel>(loSortedList.Values);
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
                MaxStyleFileEntity loEntity = this.Entity as MaxStyleFileEntity;
                if (null != loEntity)
                {
                    string lsName = this.Name.ToLowerInvariant();
                    loEntity.Name = lsName;
                    loEntity.Content = this.Content;
                    loEntity.ContentMin = this.ContentMin;
                    loEntity.ContentName = lsName;
                    loEntity.FileVersion = this.FileVersion;
                    loEntity.StyleType = this.StyleType;
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
            MaxStyleFileEntity loEntity = this.Entity as MaxStyleFileEntity;
            if (null != loEntity)
            {
                if (base.MapFromEntity())
                {
                    this.Name = loEntity.Name;
                    this.Content = loEntity.Content;
                    this.ContentMin = loEntity.ContentMin;
                    this.ContentName = loEntity.ContentName;
                    this.FileVersion = loEntity.FileVersion;
                    this.StyleType = loEntity.StyleType;
                    this.Version = loEntity.Version.ToString();
                    return true;
                }
            }

            return false;
        }

        public override bool Save()
        {
            bool lbR = false;
            this.Id = null;
            this.Entity = MaxStyleFileEntity.Create();
            lbR = base.Save();
            return lbR;
        }

        public override bool Delete()
        {
            bool lbR = false;
            if (this.Version != "File")
            {
                lbR = base.Delete();
            }

            return lbR;
        }
    }
}
