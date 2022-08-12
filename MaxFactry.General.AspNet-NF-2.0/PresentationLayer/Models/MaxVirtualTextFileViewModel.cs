// <copyright file="MaxVirtualTextFileViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="6/26/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="4/24/2016" author="Brian A. Lakstins" description="Update for no longer returning null from GetCurrent">
// <change date="5/18/2016" author="Brian A. Lakstins" description="Fix issue with name being case sensitive in database">
// <change date="7/15/2016" author="Brian A. Lakstins" description="Moved to Core.AspNet project and updated methods that can be overridden for IIS.">
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
    public class MaxVirtualTextFileViewModel : MaxFactry.Base.PresentationLayer.MaxBaseIdViewModel
	{
        private List<MaxVirtualTextFileViewModel> _oSortedList = null;

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileViewModel class
        /// </summary>
        public MaxVirtualTextFileViewModel()
            : base()
        {
            this.Entity = MaxVirtualTextFileEntity.Create();
        }

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileViewModel class
        /// </summary>
        /// <param name="loEntity">Entity to use as data.</param>
        public MaxVirtualTextFileViewModel(MaxEntity loEntity)
            : base(loEntity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileViewModel class
        /// </summary>
        /// <param name="lsId">Id associated with the entity.</param>
        public MaxVirtualTextFileViewModel(string lsVirtualPath)
        {
            this.LoadFromPath(lsVirtualPath);
        }

        public virtual bool LoadFromPath(string lsVirtualPath)
        {
            string lsName = lsVirtualPath.ToLowerInvariant();
            MaxVirtualTextFileEntity loEntity = MaxVirtualTextFileEntity.Create().GetCurrent(lsName) as MaxVirtualTextFileEntity;
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

        public virtual List<MaxVirtualTextFileViewModel> GetSortedList()
        {
            if (null == this._oSortedList)
            {
                this._oSortedList = new List<MaxVirtualTextFileViewModel>();
                SortedList<string, MaxVirtualTextFileViewModel> loSortedList = new SortedList<string, MaxVirtualTextFileViewModel>();
                string[] laKey = this.EntityIndex.GetSortedKeyList();
                for (int lnK = 0; lnK < laKey.Length; lnK++)
                {
                    MaxVirtualTextFileViewModel loViewModel = new MaxVirtualTextFileViewModel(this.EntityIndex[laKey[lnK]] as MaxVirtualTextFileEntity);
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

                this._oSortedList = new List<MaxVirtualTextFileViewModel>(loSortedList.Values);
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
                MaxVirtualTextFileEntity loEntity = this.Entity as MaxVirtualTextFileEntity;
                if (null != loEntity)
                {
                    loEntity.Name = this.Name.ToLowerInvariant();
                    loEntity.Content = this.Content;
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
            MaxVirtualTextFileEntity loEntity = this.Entity as MaxVirtualTextFileEntity;
            if (null != loEntity)
            {
                if (base.MapFromEntity())
                {
                    this.Name = loEntity.Name;
                    this.Content = loEntity.Content;
                    if (loEntity.Version > 0)
                    {
                        this.Version = loEntity.Version.ToString();
                    }

                    return true;
                }

                else
                {
                    this.Version = "File";
                    this.Name = loEntity.Name;
                    this.Content = loEntity.Content;
                }
            }

            return false;
        }

        public override bool Save()
        {
            bool lbR = false;
            this.Id = null;
            this.Entity = MaxVirtualTextFileEntity.Create();
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
