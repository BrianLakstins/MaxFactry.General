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
// <change date="7/15/2016" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.PresentationLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Hosting;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.General.AspNet.BusinessLayer;
    using MaxFactry.General.AspNet.PresentationLayer;

    /// <summary>
    /// View model for content.
    /// </summary>
    public class MaxVirtualTextFileIISViewModel : MaxVirtualTextFileViewModel
    {
        private List<MaxVirtualTextFileViewModel> _oSortedList = null;

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileViewModel class
        /// </summary>
        public MaxVirtualTextFileIISViewModel()
            : base()
        {
            this.Entity = MaxVirtualTextFileEntity.Create();
        }

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileViewModel class
        /// </summary>
        /// <param name="loEntity">Entity to use as data.</param>
        public MaxVirtualTextFileIISViewModel(MaxEntity loEntity)
            : base(loEntity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileViewModel class
        /// </summary>
        /// <param name="lsId">Id associated with the entity.</param>
        public MaxVirtualTextFileIISViewModel(string lsVirtualPath)
        {
            this.LoadFromPath(lsVirtualPath);
        }

        public override bool LoadFromPath(string lsVirtualPath)
        {
            bool lbR = base.LoadFromPath(lsVirtualPath);
            if (!lbR)
            {
                MaxVirtualTextFileEntity loEntity = MaxVirtualTextFileEntity.Create();
                loEntity.Name = lsVirtualPath.ToLowerInvariant();
                Stream loStream = VirtualPathProvider.OpenFile(loEntity.Name);
                try
                {
                    StreamReader loReader = new StreamReader(loStream);
                    try
                    {
                        loEntity.Content = loReader.ReadToEnd();
                    }
                    finally
                    {
                        loReader.Close();
                    }
                }
                finally
                {
                    loStream.Close();
                }

                this.Entity = loEntity;
                lbR = this.MapFromEntity();
            }

            return lbR;
        }

        public override List<MaxVirtualTextFileViewModel> GetSortedList()
        {
            if (null == this._oSortedList)
            {
                this._oSortedList = new List<MaxVirtualTextFileViewModel>();
                SortedList<string, MaxVirtualTextFileViewModel> loSortedList = new SortedList<string, MaxVirtualTextFileViewModel>();
                string[] laKey = this.EntityIndex.GetSortedKeyList();
                for (int lnK = 0; lnK < laKey.Length; lnK++)
                {
                    MaxVirtualTextFileIISViewModel loViewModel = new MaxVirtualTextFileIISViewModel(this.EntityIndex[laKey[lnK]] as MaxVirtualTextFileEntity);
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

                if (HostingEnvironment.VirtualPathProvider is System.Web.Hosting.MaxVirtualPathProviderOverride)
                {
                    string[] laVirtualTextPathList = System.Web.Hosting.MaxVirtualPathProviderOverride.GetVirtualPathList();
                    foreach (string lsVirtualTextPath in laVirtualTextPathList)
                    {
                        if (!loSortedList.ContainsKey(lsVirtualTextPath.ToLower()))
                        {
                            MaxVirtualTextFileIISViewModel loViewModel = new MaxVirtualTextFileIISViewModel();
                            loViewModel.Name = lsVirtualTextPath;
                            loViewModel.Version = "File";
                            loSortedList.Add(lsVirtualTextPath.ToLower(), loViewModel);
                        }
                    }
                }

                this._oSortedList = new List<MaxVirtualTextFileViewModel>(loSortedList.Values);
            }

            return this._oSortedList;
        }
    }
}
