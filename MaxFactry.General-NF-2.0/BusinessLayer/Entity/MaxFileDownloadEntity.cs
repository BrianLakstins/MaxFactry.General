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
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class. Use parent methods instead of repository.  Use HttpLibrary instead of respository for remote data.">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
#if net4_52
    using System.Net.Http;
#endif
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer ;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
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

        public bool Download(string lsUrl, string lsToken)
        {
            MaxIndex loRequest = new MaxIndex();
            loRequest.Add(MaxFactry.Base.DataLayer.Library.Provider.MaxHttpLibraryDefaultProvider.RequestContentName.RequestUrl, new Uri(lsUrl));
            loRequest.Add(MaxFactry.Base.DataLayer.Library.Provider.MaxHttpLibraryDefaultProvider.RequestContentName.Token, lsToken);
            MaxIndex loResponse = MaxHttpLibrary.GetResponse(loRequest);
            MaxData loData = new MaxData(this.Data);
            if (null != loResponse && loResponse.Contains(MaxFactry.Base.DataLayer.Library.Provider.MaxHttpLibraryDefaultProvider.ResponseName.Content))
            {
                loData.Set(this.DataModel.Name, loResponse[MaxFactry.Base.DataLayer.Library.Provider.MaxHttpLibraryDefaultProvider.ResponseName.ContentFileName]);
                loData.Set(this.DataModel.Content, loResponse[MaxFactry.Base.DataLayer.Library.Provider.MaxHttpLibraryDefaultProvider.ResponseName.Content]);
                loData.Set(this.DataModel.ContentLength, loResponse[MaxFactry.Base.DataLayer.Library.Provider.MaxHttpLibraryDefaultProvider.ResponseName.ContentLength]);
                loData.Set(this.DataModel.ContentType, loResponse[MaxFactry.Base.DataLayer.Library.Provider.MaxHttpLibraryDefaultProvider.ResponseName.ContentType]);
                loData.Set(this.DataModel.ResponseUrl, loResponse[MaxFactry.Base.DataLayer.Library.Provider.MaxHttpLibraryDefaultProvider.ResponseName.ContentLocation]);
            }

            return this.Load(loData);
        }

        public MaxEntityList LoadAllByResponseHost(string lsResponseHost)
        {
            return this.LoadAllActiveByProperty(this.DataModel.ResponseHost, lsResponseHost);
        }

        public virtual bool LoadByName(string lsName)
        {
            MaxEntityList loList = this.LoadAllActiveByProperty(this.DataModel.Name, lsName);
            if (loList.Count == 1)
            {
                return this.Load(loList[0].GetData());
            }

            return false;
        }

        public virtual MaxEntityList LoadAllRemote(string lsUrl, string lsToken)
        {
            MaxIndex loRequest = new MaxIndex();
            object loContent = MaxHttpLibrary.GetContent(lsUrl, lsToken);
            MaxEntityList loR = this.MapResponse(loContent);
            return loR;
        }

        protected virtual MaxEntityList MapResponse(object loRemoteResponse)
        {
            MaxEntityList loR = new MaxEntityList(this.GetType());
            string lsContent = string.Empty;
            if (loRemoteResponse is string)
            {
                lsContent = loRemoteResponse as string;
            }
            else if (loRemoteResponse is Stream)
            {
                lsContent = new StreamReader((Stream)loRemoteResponse).ReadToEnd();
            }

            /*
            {
                "Message": {
                    "Success": "",
                    "Warning": "",
                    "Log": "",
                    "Error": "User does not have permission for this item type.",
                    "LogList": []
                  },
                  "Item": { },
                  "ItemList": [],
                  "Page": { },
                  "Status": 401
                }


                {
                  "Message": {
                    "Success": "",
                    "Warning": "",
                    "Log": "",
                    "Error": "",
                    "LogList": []
                  },
                  "Item": {},
                  "ItemList": [
                    {
                      "ContentDate": "7/31/2023 12:22 PM",
                      "ContentName": "AgentConsole.zip",
                      "CreatedDate": "7/31/2023 12:22 PM",
                      "FileType": 2023102,
                      "Format": "",
                      "Id": "4c57a5cc-1131-4391-92f6-61ab8b891e8a",
                      "RelatedList": []
                    },
                    {
                      "ContentDate": "7/31/2023 12:21 PM",
                      "ContentName": "Agent.WindowsService.zip",
                      "CreatedDate": "7/31/2023 12:22 PM",
                      "FileType": 2023101,
                      "Format": "",
                      "Id": "f0e49d3f-ee94-4992-ac57-621f56c26555",
                      "RelatedList": []
                    }
                  ],
                  "Page": {},
                  "Status": 200
                }
            */

            if (!string.IsNullOrEmpty(lsContent))
            {
                if (lsContent.Contains("\"Status\": 200"))
                {
                    MaxIndex loIndex = MaxConvertLibrary.DeserializeObject(typeof(object), lsContent, typeof(MaxIndex)) as MaxIndex;
                    object[] loItemList = loIndex["ItemList"] as object[];
                    if (null != loItemList)
                    {
                        for (int lnD = 0; lnD < loItemList.Length; lnD++)
                        {
                            MaxIndex loItem = loItemList[lnD] as MaxIndex;
                            if (null != loItem)
                            {
                                MaxFileDownloadEntity loEntity = MaxFileDownloadEntity.Create();
                                loEntity.ContentDate = MaxConvertLibrary.ConvertToDateTime(typeof(object), loItem.GetValueString("ContentDate"));
                                loEntity.ContentName = loItem.GetValueString("ContentName");
                                loEntity.SetId(new Guid(loItem.GetValueString("Id")));
                                loR.Add(loEntity);
                            }
                        }
                    }
                }
            }

            return loR;
        }

        public virtual bool LoadContentRemote(string lsToken, string lsUrl)
        {
            bool lbR = false;
            MaxIndex loRequest = new MaxIndex();
            loRequest.Add(MaxFactry.Base.DataLayer.Library.Provider.MaxHttpLibraryDefaultProvider.RequestContentName.RequestUrl, lsUrl);
            loRequest.Add(MaxFactry.Base.DataLayer.Library.Provider.MaxHttpLibraryDefaultProvider.RequestContentName.Token, lsToken);
            MaxIndex loResponse = MaxHttpLibrary.GetResponse(loRequest);
            if (null != loResponse)
            {
                this.Content = loResponse[MaxFactry.Base.DataLayer.Library.Provider.MaxHttpLibraryDefaultProvider.ResponseName.Content] as Stream;
                this.ContentLength = MaxConvertLibrary.ConvertToLong(typeof(object), loResponse[MaxFactry.Base.DataLayer.Library.Provider.MaxHttpLibraryDefaultProvider.ResponseName.ContentLength]);
                lbR = true;
            }

            return lbR;
        }
    }
}
