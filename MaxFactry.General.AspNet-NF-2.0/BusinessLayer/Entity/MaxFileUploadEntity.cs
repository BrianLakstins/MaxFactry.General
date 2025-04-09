// <copyright file="MaxFileUploadEntity.cs" company="Lakstins Family, LLC">
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
// <change date="7/12/2016" author="Brian A. Lakstins" description="Integrated with specifying file name.">
// <change date="10/23/2019" author="Brian A. Lakstins" description="Add loading a cached active version by file name.">
// <change date="1/16/2021" author="Brian A. Lakstins" description="Update definition of cache keys.">
// <change date="3/15/2021" author="Brian A. Lakstins" description="Allow external mime type">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class.">
// <change date="3/22/2025" author="Brian A. Lakstins" description="Make sure Name is set when inserting.">
// <change date="4/9/2025" author="Brian A. Lakstins" description="Override SetInitial and SetProperties instead of Insert and Update.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.BusinessLayer
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer ;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.General.AspNet.DataLayer;
    using MaxFactry.General.BusinessLayer;

    /// <summary>
    /// Entity to represent virtual text file in a web site.
    /// </summary>
    public class MaxFileUploadEntity : MaxBaseIdFileEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxFileUploadEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxFileUploadEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        public string UploadName
        {
            get
            {
                return this.GetString(this.DataModel.UploadName);
            }

            set
            {
                this.Set(this.DataModel.UploadName, value);
            }
        }

        public bool IsDownload
        {
            get
            {
                return this.GetBit(this.DataModel.OptionFlagList, 1);
            }

            set
            {
                this.SetBit(this.DataModel.OptionFlagList, 1, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxFileUploadDataModel DataModel
        {
            get
            {
                return (MaxFileUploadDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxVirtualTextFileEntity class.</returns>
        public static MaxFileUploadEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxFileUploadEntity),
                typeof(MaxFileUploadDataModel)) as MaxFileUploadEntity;
        }

        /// <summary>
        /// Gets a string that can be used to sort a list of this entity.
        /// </summary>
        /// <returns>Lowercase version of Name passed to 100 characters.</returns>
        public override string GetDefaultSortString()
        {
            return this.Name.ToLowerInvariant().PadRight(500, ' ') + base.GetDefaultSortString();
        }

        public virtual void GetFromDownloadUrl(string lsUrl, string lsToken)
        {
            MaxFileDownloadEntity loEntity = MaxFileDownloadEntity.Create();
            loEntity.Download(lsUrl, lsToken);
            this.Content = loEntity.Content;
            this.ContentLength = loEntity.ContentLength;
            this.ContentType = loEntity.ContentType;
            this.Name = loEntity.Name;
            this.UploadName = loEntity.ResponseUrl;
        }

        public virtual bool LoadByUploadName(string lsName)
        {
            MaxDataList loDataList = MaxBaseAspNetRepository.SelectAllActiveByProperty(this.Data, this.DataModel.UploadName, lsName);
            if (loDataList.Count > 0)
            {
                this.Load(loDataList[0]);
                return true;
            }

            return false;
        }

        public virtual bool LoadByName(string lsName)
        {
            MaxDataList loDataList = MaxBaseAspNetRepository.SelectAllActiveByProperty(this.Data, this.DataModel.Name, lsName);
            if (loDataList.Count > 0)
            {
                this.Load(loDataList[0]);
                return true;
            }

            return false;
        }

        public virtual bool LoadByFileNameCache(string lsFileName)
        {
            string lsCacheDataKey = this.GetCacheKey() + "LoadAll";
            MaxDataList loDataList = MaxCacheRepository.Get(this.GetType(), lsCacheDataKey, typeof(MaxDataList)) as MaxDataList;
            if (null != loDataList)
            {
                for (int lnD = 0; lnD < loDataList.Count; lnD++)
                {
                    if (MaxConvertLibrary.ConvertToString(typeof(object), loDataList[lnD].Get(this.DataModel.FileName)) == lsFileName)
                    {
                        if (MaxConvertLibrary.ConvertToBoolean(typeof(object), loDataList[lnD].Get(this.DataModel.IsActive)))
                        {
                            this.Load(loDataList[lnD]);
                            return true;
                        }
                    }
                }
            }

            lsCacheDataKey = this.GetCacheKey() + "LoadByFileNameCache/" + lsFileName;
            MaxData loData = MaxCacheRepository.Get(this.GetType(), lsCacheDataKey, typeof(MaxDataList)) as MaxData;
            if (null == loData)
            {
                loDataList = MaxBaseAspNetRepository.SelectAllActiveByProperty(this.Data, this.DataModel.FileName, lsFileName);
                if (loDataList.Count > 0)
                {
                    loData = loDataList[0];
                    MaxCacheRepository.Set(this.GetType(), lsCacheDataKey, loData);
                }
            }
            
            if (null != loData)
            {
                this.Load(loData);
                return true;
            }

            return false;
        }

        public virtual bool LoadByNameCache(string lsName)
        {
            string lsCacheDataKey = this.GetCacheKey() + "LoadAll";
            MaxDataList loDataList = MaxCacheRepository.Get(this.GetType(), lsCacheDataKey, typeof(MaxDataList)) as MaxDataList;
            if (null != loDataList)
            {
                for (int lnD = 0; lnD < loDataList.Count; lnD++)
                {
                    if (MaxConvertLibrary.ConvertToString(typeof(object), loDataList[lnD].Get(this.DataModel.Name)) == lsName)
                    {
                        if (MaxConvertLibrary.ConvertToBoolean(typeof(object), loDataList[lnD].Get(this.DataModel.IsActive)))
                        {
                            this.Load(loDataList[lnD]);
                            return true;
                        }
                    }
                }
            }

            lsCacheDataKey = this.GetCacheKey() + "LoadByNameCache/" + lsName;
            MaxData loData = MaxCacheRepository.Get(this.GetType(), lsCacheDataKey, typeof(MaxDataList)) as MaxData;
            if (null == loData)
            {
                loDataList = MaxBaseAspNetRepository.SelectAllActiveByProperty(this.Data, this.DataModel.Name, lsName);
                if (loDataList.Count > 0)
                {
                    loData = loDataList[0];
                    MaxCacheRepository.Set(this.GetType(), lsCacheDataKey, loData);
                }
            }

            if (null != loData)
            {
                this.Load(loData);
                return true;
            }

            return false;
        }

        protected override void SetProperties()
        {
            this.ContentName = this.FileName;
            this.ContentType = this.GetMimeType();
            if (string.IsNullOrEmpty(this.MimeType))
            {
                this.MimeType = this.GetMimeType();
            }

            base.SetProperties();
        }

        protected override void SetInitial()
        {
            base.SetInitial();
            if (string.IsNullOrEmpty(this.Name))
            {
                if (!string.IsNullOrEmpty(this.UploadName))
                {
                    this.Name = this.UploadName;
                }
                else if (!string.IsNullOrEmpty(this.FileName))
                {
                    this.Name = this.FileName;
                }
                else if (!string.IsNullOrEmpty(this.ContentName))
                {
                    this.Name = this.ContentName;
                }
            }

            if (null == this.UploadName || this.UploadName.Length.Equals(0))
            {
                this.UploadName = this.Name;
            }
        }  

        public string GetContentURLCache(string lsName)
        {
            string lsR = string.Empty;
            string lsCacheKey = this.GetCacheKey() + "GetContentURLCache/" + lsName; ;
            string lsUrl = MaxCacheRepository.Get(typeof(MaxFileUploadEntity), lsCacheKey, typeof(string)) as string;
            if (null == lsUrl)
            {
                MaxFileUploadEntity loEntity = MaxFileUploadEntity.Create();
                if (loEntity.LoadByNameCache(lsName))
                {
                    lsR = loEntity.GetContentUrl();
                }
                else if (loEntity.LoadByFileNameCache(lsName))
                {
                    lsR = loEntity.GetContentUrl();
                }

                MaxCacheRepository.Set(typeof(MaxFileUploadEntity), lsCacheKey, lsR);
            }
            else
            {
                lsR = lsUrl;
            }

            return lsR;
        }
    }
}
