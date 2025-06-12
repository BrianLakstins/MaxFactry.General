// <copyright file="MaxVirtualTextFileEntity.cs" company="Lakstins Family, LLC">
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
// <change date="4/28/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="6/17/2014" author="Brian A. Lakstins" description="Update to use instance data model instead of static.">
// <change date="6/27/2014" author="Brian A. Lakstins" description="Update base to MaxBaseIdEntity.">
// <change date="6/29/2014" author="Brian A. Lakstins" description="Update Repository use.">
// <change date="7/25/2014" author="Brian A. Lakstins" description="Additions to allow editing.">
// <change date="8/8/2014" author="Brian A. Lakstins" description="Update delete to just mark as deleted.">
// <change date="8/21/2014" author="Brian A. Lakstins" description="Update so normal to insert new version.">
// <change date="9/16/2014" author="Brian A. Lakstins" description="Integrated caching.">
// <change date="10/1/2014" author="Brian A. Lakstins" description="Update caching integration.">
// <change date="12/18/2014" author="Brian A. Lakstins" description="Update to follow core data access patterns.">
// <change date="6/8/2015" author="Brian A. Lakstins" description="Update to include storagekey in cache keys.">
// <change date="6/10/2015" author="Brian A. Lakstins" description="Fix caching.">
// <change date="4/20/2016" author="Brian A. Lakstins" description="Updated to use centralized caching.">
// <change date="4/24/2016" author="Brian A. Lakstins" description="Update for no longer returning null from GetCurrent">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class.">
// <change date="6/4/2025" author="Brian A. Lakstins" description="Change base class to remove versioning">
// <change date="6/11/2025" author="Brian A. Lakstins" description="Update cache integration">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.BusinessLayer
{
    using System;
    using System.IO;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer ;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.General.AspNet.DataLayer;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Entity to represent virtual text file in a web site.
    /// </summary>
    public class MaxVirtualTextFileEntity : MaxBaseGuidKeyEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxVirtualTextFileEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxVirtualTextFileEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetString(this.DataModel.Name);
            }

            set
            {
                this.Set(this.DataModel.Name, value);
            }
        }

        /// <summary>
        /// Gets or sets the text content
        /// </summary>
        public string Content
        {
            get
            {
                string lsR = this.GetString(this.DataModel.Content);
                lsR = lsR.Replace("Html.GetContent(", "Html.MaxGetContent(");
                lsR = lsR.Replace("MaxFactry.Application.Base.BusinessLayer.MaxEmailEntity.", "MaxFactry.Base.BusinessLayer.MaxEmailEntity.");
                lsR = lsR.Replace("MaxFactry.Application.AspNet.BusinessLayer.MaxFormEntity.", "MaxFactry.General.AspNet.BusinessLayer.MaxFormEntity.");
                lsR = lsR.Replace("MaxFactry.Application.AspNet.BusinessLayer.MaxFormValueEntity.", "MaxFactry.General.AspNet.BusinessLayer.MaxFormValueEntity.");
                lsR = lsR.Replace("Html.GetContentShortCode(", "Html.MaxGetContentShortCode(");
                lsR = lsR.Replace("Html.GetGoogleAnalytics(", "Html.MaxGetGoogleAnalytics(");
                lsR = lsR.Replace("MaxFactry.Application.AspNet.IIS.Mvc4.PresentationLayer.MaxEnableEmbedAttribute.", "MaxFactry.Base.Mvc4.PresentationLayer.MaxEnableEmbedAttribute.");


                return lsR;
            }

            set
            {
                this.Set(this.DataModel.Content, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxVirtualTextFileDataModel DataModel
        {
            get
            {
                return (MaxVirtualTextFileDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxVirtualTextFileEntity class.</returns>
        public static MaxVirtualTextFileEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxVirtualTextFileEntity),
                typeof(MaxVirtualTextFileDataModel)) as MaxVirtualTextFileEntity;
        }

        public static bool Exists(string lsName)
        {
            string lsCacheKey = GetExistsKey(lsName);
            string lsExists = MaxCacheRepository.Get(typeof(MaxVirtualTextFileEntity), lsCacheKey, typeof(string)) as string;
            if (string.IsNullOrEmpty(lsExists))
            {
                MaxVirtualTextFileEntity loEntity = MaxVirtualTextFileEntity.Create();
                MaxEntityList loList = loEntity.LoadAllActiveCache();
                lsExists = "false";
                for (int lnE = 0; lnE < loList.Count; lnE++)
                {
                    loEntity = loList[lnE] as MaxVirtualTextFileEntity;
                    if (loEntity.Name.Equals(lsName))
                    {
                        lsExists = "true";
                    }
                }               

                MaxCacheRepository.Set(typeof(MaxVirtualTextFileEntity), lsCacheKey, lsExists, DateTime.UtcNow.AddDays(1));
            }

            return MaxConvertLibrary.ConvertToBoolean(typeof(object), lsExists);
        }

        /// <summary>
        /// Gets a string that can be used to sort a list of this entity.
        /// </summary>
        /// <returns>Lowercase version of Name passed to 100 characters.</returns>
        public override string GetDefaultSortString()
        {
            return this.Name.ToLowerInvariant().PadRight(500, ' ') + base.GetDefaultSortString();
        }

        public static string GetExistsKey(string lsName)
        {
            return MaxDataLibrary.GetApplicationKey() + typeof(MaxVirtualTextFileEntity).ToString() + ".LoadAllByName" + lsName;
        }
    }
}
