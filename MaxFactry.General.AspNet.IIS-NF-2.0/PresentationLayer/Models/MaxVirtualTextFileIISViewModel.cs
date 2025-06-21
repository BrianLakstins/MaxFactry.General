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
// <change date="6/4/2025" author="Brian A. Lakstins" description="Updates for removal of versioning">
// <change date="6/21/2025" author="Brian A. Lakstins" description="Updates for changed to base">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.PresentationLayer
{
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.PresentationLayer;
    using MaxFactry.Core;
    using MaxFactry.General.AspNet.BusinessLayer;
    using MaxFactry.General.AspNet.PresentationLayer;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Hosting;

    /// <summary>
    /// View model for content.
    /// </summary>
    public class MaxVirtualTextFileIISViewModel : MaxVirtualTextFileViewModel
    {
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
        public MaxVirtualTextFileIISViewModel(string lsName): base(lsName)
        {
        }

        public override bool Load(string lsName)
        {
            bool lbR = base.Load(lsName);
            if (!lbR)
            {
                MaxVirtualTextFileEntity loEntity = MaxVirtualTextFileEntity.Create();
                loEntity.Name = lsName.ToLowerInvariant();
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

        public override List<MaxBaseVersionedViewModel> GetSortedList()
        {
            List<MaxBaseVersionedViewModel> loR = base.GetSortedList();
            if (HostingEnvironment.VirtualPathProvider is System.Web.Hosting.MaxVirtualPathProviderOverride)
            {
                SortedList<string, MaxBaseVersionedViewModel> loSortedList = new SortedList<string, MaxBaseVersionedViewModel>(StringComparer.OrdinalIgnoreCase);
                foreach (MaxBaseVersionedViewModel loViewModel in loR)
                {
                    if (!loSortedList.ContainsKey(loViewModel.Name.ToLower()))
                    {
                        loSortedList.Add(loViewModel.Name.ToLower(), loViewModel);
                    }
                }

                string[] laVirtualTextPathList = System.Web.Hosting.MaxVirtualPathProviderOverride.GetVirtualPathList();
                foreach (string lsVirtualTextPath in laVirtualTextPathList)
                {
                    if (!loSortedList.ContainsKey(lsVirtualTextPath.ToLower()))
                    {
                        MaxBaseVersionedViewModel loViewModel = new MaxBaseVersionedViewModel();
                        loViewModel.Name = lsVirtualTextPath;
                        loViewModel.DataKey = "File";
                        loSortedList.Add(lsVirtualTextPath.ToLower(), loViewModel);
                    }
                }

                loR = new List<MaxBaseVersionedViewModel>(loSortedList.Values);
            }

            return loR;
        }
    }
}
