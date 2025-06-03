// <copyright file="MaxFileEntity.cs" company="Lakstins Family, LLC">
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
// <change date="4/28/2023" author="Brian A. Lakstins" description="Initial creation">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class.">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
    using System;
    using System.IO;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer ;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.General.DataLayer;

    /// <summary>
    /// Entity to represent virtual text file in a web site.
    /// </summary>
    public class MaxFileEntity : MaxBaseIdFileEntity
    {
        private string _sContentString = null;

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxFileEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxFileEntity(Type loDataModelType)
            : base(loDataModelType)
        {
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
                        MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error converting stream to string for MaxFile", loE));
                    }
                }

                return this._sContentString;
            }
        }

        public Guid RelatedId
        {
            get
            {
                return this.GetGuid(this.DataModel.RelatedId);
            }

            set
            {
                this.Set(this.DataModel.RelatedId, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxFileDataModel DataModel
        {
            get
            {
                return (MaxFileDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxVirtualTextFileEntity class.</returns>
        public static MaxFileEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxFileEntity),
                typeof(MaxFileDataModel)) as MaxFileEntity;
        }

        /// <summary>
        /// Gets a string that can be used to sort a list of this entity.
        /// </summary>
        /// <returns>Lowercase version of Name passed to 100 characters.</returns>
        public override string GetDefaultSortString()
        {
            return this.Name.ToLowerInvariant().PadRight(500, ' ') + base.GetDefaultSortString();
        }

        public MaxEntityList LoadAllByRelatedIdCache(Guid loRelatedId)
        {
            MaxEntityList loEntityList = this.LoadAllByPropertyCache(this.DataModel.RelatedId, loRelatedId);
            return loEntityList;
        }
    }
}
