// <copyright file="MaxHttpClientEntity.cs" company="Lakstins Family, LLC">
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
// <change date="8/2/2023" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.General.DataLayer;

    /// <summary>
    /// Entity used to manage remote data through http
    /// </summary>
    public class MaxHttpClientEntity : MaxBaseIdEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxHttpClientEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxHttpClientEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxHttpClientEntity class
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxHttpClientEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        public string Token
        {
            get
            {
                return this.GetString(this.DataModel.Token);
            }

            set
            {
                this.Set(this.DataModel.Token, value);
            }
        }

        public string ClientId
        {
            get
            {
                return this.GetString(this.DataModel.ClientId);
            }

            set
            {
                this.Set(this.DataModel.ClientId, value);
            }
        }

        public string ClientSecret
        {
            get
            {
                return this.GetString(this.DataModel.ClientSecret);
            }

            set
            {
                this.Set(this.DataModel.ClientSecret, value);
            }
        }

        public string Scope
        {
            get
            {
                return this.GetString(this.DataModel.Scope);
            }

            set
            {
                this.Set(this.DataModel.Scope, value);
            }
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

        public DateTime RequestTime
        {
            get
            {
                return this.GetDateTime(this.DataModel.RequestTime);
            }

            set
            {
                this.Set(this.DataModel.RequestTime, value);
            }
        }

        public object RequestContent
        {
            get
            {
                return this.Get(this.DataModel.RequestContent);
            }

            set
            {
                this.Set(this.DataModel.RequestContent, value);
            }
        }

        public DateTime ResponseTime
        {
            get
            {
                return this.GetDateTime(this.DataModel.ResponseTime);
            }

            set
            {
                this.Set(this.DataModel.ResponseTime, value);
            }
        }

        public object Response
        {
            get
            {
                return this.Get(this.DataModel.Response);
            }

            set
            {
                this.Set(this.DataModel.Response, value);
            }
        }

        public object ResponseContent
        {
            get
            {
                return this.Get(this.DataModel.ResponseContent);
            }

            set
            {
                this.Set(this.DataModel.ResponseContent, value);
            }
        }

        public string Log
        {
            get
            {
                return this.GetString(this.DataModel.Log);
            }

            set
            {
                this.Set(this.DataModel.Log, value);
            }
        }

        public object Exception
        {
            get
            {
                return this.GetString(this.DataModel.Exception);
            }

            set
            {
                this.Set(this.DataModel.Exception, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxHttpClientDataModel DataModel
        {
            get
            {
                return (MaxHttpClientDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new entity initialized with the data.
        /// </summary>
        /// <returns>A new entity.</returns>
        public static MaxHttpClientEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxHttpClientEntity),
                typeof(MaxHttpClientDataModel)) as MaxHttpClientEntity;
        }

        public bool LoadRemote()
        {
            return this.LoadRemote(this.RequestUrl, this.RequestContent, this.Token, this.ClientId, this.ClientSecret, this.Scope);
        }

        public bool LoadRemote(string lsUrl)
        {
            return this.LoadRemote(lsUrl, this.RequestContent, this.Token, this.ClientId, this.ClientSecret, this.Scope);
        }

        public bool LoadRemote(string lsUrl, object loRequestContent)
        {
            return this.LoadRemote(lsUrl, loRequestContent, this.Token, this.ClientId, this.ClientSecret, this.Scope);
        }

        public bool LoadRemote(string lsUrl, object loRequestContent, string lsToken)
        {
            return this.LoadRemote(lsUrl, loRequestContent, lsToken, null, null, null);
        }

        public bool LoadRemote(string lsUrl, object loRequestContent, string lsClientId, string lsClientSecret, string lsScope)
        {
            return this.LoadRemote(lsUrl, loRequestContent, null, lsClientId, lsClientSecret, lsScope);
        }

        public bool LoadRemote(string lsUrl, object loRequestContent, string lsToken, string lsClientId, string lsClientSecret, string lsScope)
        {
            bool lbR = false;
            //// Add a Query 
            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.StartGroup();
            loDataQuery.AddFilter(this.DataModel.AlternateId, "=", "Remote");
            loDataQuery.AddCondition("AND");
            loDataQuery.AddFilter(this.DataModel.RequestUrl, "=", lsUrl);
            if (null != loRequestContent)
            {
                loDataQuery.AddCondition("AND");
                loDataQuery.AddFilter(this.DataModel.RequestContent, "=", loRequestContent);
            }

            if (null != lsToken)
            {
                loDataQuery.AddCondition("AND");
                loDataQuery.AddFilter(this.DataModel.Token, "=", lsToken);
            }

            if (null != lsClientId && null != lsClientSecret) 
            {
                loDataQuery.AddCondition("AND");
                loDataQuery.AddFilter(this.DataModel.ClientId, "=", lsClientId);
                loDataQuery.AddCondition("AND");
                loDataQuery.AddFilter(this.DataModel.ClientSecret, "=", lsClientSecret);
                if (null != lsScope)
                {
                    loDataQuery.AddCondition("AND");
                    loDataQuery.AddFilter(this.DataModel.Scope, "=", lsScope);
                }
            }

            loDataQuery.EndGroup();
            int lnTotal = 0;
            MaxDataList loDataList = MaxGeneralRepository.Select(this.Data, loDataQuery, 0, 0, string.Empty, out lnTotal);
            if (loDataList.Count == 1)
            {
                lbR = this.Load(loDataList[0]);
            }

            return lbR;
        }
    }
}
