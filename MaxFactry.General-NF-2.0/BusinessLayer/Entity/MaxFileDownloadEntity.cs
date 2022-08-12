// <copyright file="MaxFileDownloadEntity.cs" company="Lakstins Family, LLC">
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

namespace MaxFactry.General.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer ;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.General.DataLayer;

    /// <summary>
    /// Entity to represent virtual text file in a web site.
    /// </summary>
    public class MaxFileDownloadEntity : MaxBaseIdFileEntity
    {
        private string _sContentString = null;

        public const string DataNameUrlList = "MaxFileDownloadEntityUrlList";

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxFileDownloadEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxFileDownloadEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        public string RequestUrl
        {
            get
            {
                return this.GetString(this.DataModel.RequestUrl);
            }

            set
            {
                this.Set(this.DataModel.RequestUrl, value);
            }
        }

        public string ResponseUrl
        {
            get
            {
                return this.GetString(this.DataModel.ResponseUrl);
            }
        }

        public string ResponseHeader
        {
            get
            {
                return this.GetString(this.DataModel.ResponseHeader);
            }
        }

        public string ResponseCookie
        {
            get
            {
                return this.GetString(this.DataModel.ResponseCookie);
            }
        }

        public string ResponseHost
        {
            get
            {
                return this.GetString(this.DataModel.ResponseHost);
            }
        }

        public string ContentString
        {
            get
            {
                if (null == this._sContentString)
                {
                    this._sContentString = string.Empty;
                    try
                    {
                        System.Text.Encoding loEncoding = System.Text.Encoding.ASCII;
                        if (this.ContentType.ToLowerInvariant().Contains("charset"))
                        {
                            string[] laContentType = this.ContentType.Split(';');
                            foreach (string lsContentType in laContentType)
                            {
                                if (lsContentType.TrimStart().ToLowerInvariant().StartsWith("charset") && lsContentType.Contains("="))
                                {
                                    string[] laCharset = lsContentType.Split('=');
                                    if (laCharset[1].Trim().ToLowerInvariant().Equals("utf-8"))
                                    {
                                        loEncoding = System.Text.Encoding.UTF8;
                                    }
                                    else if (laCharset[1].Trim().ToLowerInvariant().Equals("utf-7"))
                                    {
                                        loEncoding = System.Text.Encoding.UTF7;
                                    }
                                    else if (laCharset[1].Trim().ToLowerInvariant().Equals("utf-32"))
                                    {
                                        loEncoding = System.Text.Encoding.UTF32;
                                    }
                                    else if (laCharset[1].Trim().ToLowerInvariant().Equals("unicode"))
                                    {
                                        loEncoding = System.Text.Encoding.Unicode;
                                    }
                                }
                            }

                        }

                        StreamReader loReader = new StreamReader(this.Content, loEncoding);
                        this._sContentString = loReader.ReadToEnd();
                        loReader.Dispose();
                        this.Content.Dispose();
                    }
                    catch (Exception loE)
                    {
                        //// Stream content could not be converted to string content.
                        MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error converting stream to string for MaxFileDownload {Id}", loE, this.Id));
                    }
                }

                return this._sContentString;
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxFileDownloadDataModel DataModel
        {
            get
            {
                return (MaxFileDownloadDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxVirtualTextFileEntity class.</returns>
        public static MaxFileDownloadEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxFileDownloadEntity),
                typeof(MaxFileDownloadDataModel)) as MaxFileDownloadEntity;
        }

        /// <summary>
        /// Gets a string that can be used to sort a list of this entity.
        /// </summary>
        /// <returns>Lowercase version of Name passed to 100 characters.</returns>
        public override string GetDefaultSortString()
        {
            return this.Name.ToLowerInvariant().PadRight(500, ' ') + base.GetDefaultSortString();
        }

        public void Download(string lsUrl)
        {
            MaxData loData = MaxGeneralRepository.Download(this.Data, lsUrl);
            this.Load(loData);
        }

        public MaxEntityList LoadAllByResponseHost(string lsResponseHost)
        {
            MaxDataList loDataList = MaxGeneralRepository.SelectAllActiveByProperty(this.Data, this.DataModel.ResponseHost, lsResponseHost);
            MaxEntityList loEntityList = MaxEntityList.Create(this.GetType(), loDataList);
            return loEntityList;
        }

        public virtual bool LoadByName(string lsName)
        {
            MaxDataList loDataList = MaxGeneralRepository.SelectAllActiveByProperty(this.Data, this.DataModel.Name, lsName);
            if (loDataList.Count > 0)
            {
                this.Load(loDataList[0]);
                return true;
            }

            return false;
        }
    }
}
