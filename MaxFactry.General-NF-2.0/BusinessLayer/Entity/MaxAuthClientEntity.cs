// <copyright file="MaxAuthClientEntity.cs" company="Lakstins Family, LLC">
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
// <change date="5/14/2026" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
    using System;
    using System.IO;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.Base.DataLayer.Library.Provider;
    using MaxFactry.General.DataLayer;
    using MaxFactry.General.DataLayer.Provider;

    /// <summary>
    /// Entity used to manage information about AuthClients for the MaxSecurityProvider.
    /// </summary>
    public class MaxAuthClientEntity : MaxBaseGuidKeyEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxAuthClientEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxAuthClientEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxAuthClientEntity class
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxAuthClientEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets or sets the key of the user who created this AuthClient
        /// </summary>
		public string UserKey
        {
            get
            {
                return this.GetString(this.DataModel.UserKey);
            }

            set
            {
                this.Set(this.DataModel.UserKey, value);
            }
        }

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

        public string Description
        {
            get
            {
                return this.GetString(this.DataModel.Description);
            }
            set
            {
                this.Set(this.DataModel.Description, value);
            }
        }

        public string ClientPublicKey
        {
            get
            {
                return this.GetString(this.DataModel.ClientPublicKey);
            }

            set
            {
                this.Set(this.DataModel.ClientPublicKey, value);
            }
        }

        public string KeyAlgorithm
        {
            get
            {
                return this.GetString(this.DataModel.KeyAlgorithm);
            }

            set
            {
                this.Set(this.DataModel.KeyAlgorithm, value);
            }
        }

        public string Restrictions
        {
            get
            {
                return this.GetString(this.DataModel.Restrictions);
            }

            set
            {
                this.Set(this.DataModel.Restrictions, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxAuthClientDataModel DataModel
        {
            get
            {
                return (MaxAuthClientDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new entity initialized with the data.
        /// </summary>
        /// <returns>A new entity.</returns>
        public static MaxAuthClientEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxAuthClientEntity),
                typeof(MaxAuthClientDataModel)) as MaxAuthClientEntity;
        }

        public string GetClientToken(string lsToken)
        {
            string lsR = MaxEncryptionLibrary.Encrypt(
                this.GetType(),
                lsToken,
                "JWK:" + this.ClientPublicKey);
            
            return lsR;
        }

        public MaxEntityList LoadAllActiveByUserKeyCache(string lsUserKey)
        {
            return this.LoadAllActiveByProperty(this.DataModel.UserKey, lsUserKey);
        }

        protected override void SetInitial()
        {
            base.SetInitial();
            this.Set(this.DataModel.Restrictions, "{ \"version\": 1, \"type\": \"unrestricted\"}");
        }

        public override bool Insert()
        {
            bool lbR = base.Insert();
            if (!string.IsNullOrEmpty(this.UserKey))
            {
                Guid loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), this.UserKey);
                if (loUserId != Guid.Empty)
                {
                    MaxUserEntity loUser = MaxUserEntity.Create();
                    if (loUser.LoadByIdCache(loUserId))
                    {
                        MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
                        if (lbR)
                        {
                            loMaxUserLog.Insert(
                                loUserId,
                                MaxUserLogEntity.LogEntryTypeUserAuthClientInsert,
                                this.GetType() + ".Insert() - succeeded");
                        }
                        else
                        {
                            loMaxUserLog.Insert(
                                loUserId,
                                MaxUserLogEntity.LogEntryTypeUserAuthClientInsert,
                                this.GetType() + ".Insert() - failed");
                        }
                    }
                }
            }

            return lbR;
        }
    }
}
