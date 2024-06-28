// <copyright file="MaxUserAuthEntity.cs" company="Lakstins Family, LLC">
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
// <change date="11/3/2020" author="Brian A. Lakstins" description="Initial creation">
// <change date="1/16/2021" author="Brian A. Lakstins" description="Update definition of cache keys.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class. Use parent methods instead of repository.">
// <change date="6/19/2024" author="Brian A. Lakstins" description="Add user related logging.">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.BusinessLayer;
	using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.General.DataLayer;

	/// <summary>
    /// Entity used to manage information about UserAuths for the MaxSecurityProvider.
	/// </summary>
	public class MaxUserAuthEntity : MaxBaseGuidKeyEntity
	{
		/// <summary>
        /// Initializes a new instance of the MaxUserAuthEntity class
		/// </summary>
        /// <param name="loData">object to hold data</param>
		public MaxUserAuthEntity(MaxData loData)
			: base(loData)
		{
		}

        /// <summary>
        /// Initializes a new instance of the MaxUserAuthEntity class
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxUserAuthEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets or sets the key of the user who created this UserAuth
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

        /// <summary>
        /// Gets or sets the Name.
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

        public string ClientSecretHash
        {
            get
            {
                return this.GetString(this.DataModel.ClientSecretHash);
            }
        }

        public string[] ScopeList
        {
            get
            {
                string[] laR = this.GetObject(this.DataModel.ScopeListText, typeof(string[])) as string[];
                if (null == laR)
                {
                    laR = new string[] { };
                }

                return laR;
            }

            set
            {
                this.SetObject(this.DataModel.ScopeListText, value);
            }
        }

        public string[] DomainList
        {
            get
            {
                string[] laR = this.GetObject(this.DataModel.DomainListText, typeof(string[])) as string[];
                if (null == laR)
                {
                    laR = new string[] { };
                }

                return laR;
            }

            set
            {
                this.SetObject(this.DataModel.DomainListText, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxUserAuthDataModel DataModel
        {
            get
            {
                return (MaxUserAuthDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new entity initialized with the data.
        /// </summary>
        /// <returns>A new entity.</returns>
        public static MaxUserAuthEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxUserAuthEntity),
                typeof(MaxUserAuthDataModel)) as MaxUserAuthEntity;
        }

        public override bool Insert()
        {
            Guid loSaltId = Guid.NewGuid();
            this.Set(this.DataModel.ClientSecretHashSaltId, loSaltId);
            this.Set(this.DataModel.ClientSecretHash, this.Hash(loSaltId, this.ClientSecret));
            bool lbR = base.Insert();
            Guid loUserId = MaxConvertLibrary.ConvertToGuid(typeof(object), this.UserKey);
            if (loUserId != Guid.Empty)
            {
                MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
                if (lbR)
                {
                    loMaxUserLog.Insert(
                        loUserId,
                        MaxUserLogEntity.LogEntryTypeUserAuthInsert,
                        this.GetType() + ".Insert() - succeeded");
                }
                else
                {
                    loMaxUserLog.Insert(
                        loUserId,
                        MaxUserLogEntity.LogEntryTypeUserAuthInsert,
                        this.GetType() + ".Insert() - failed");
                }
            }


            return lbR;
        }

        /// <summary>
        /// Hashes some text
        /// </summary>
        /// <param name="loSalt">Salt to make the hash unique.</param>
        /// <param name="lsText">The text to hash</param>
        /// <returns>A hashed version of the text</returns>
        protected string Hash(Guid loSalt, string lsText)
        {
            return MaxEncryptionLibrary.GetHash(typeof(MaxUserPasswordEntity), MaxEncryptionLibrary.SHA256Hash, new System.Text.UnicodeEncoding().GetBytes(loSalt.ToString() + lsText));
        }

        /// <summary>
        /// Loads all UserAuth entities that match the given ClientId that have not been deleted.
        /// </summary>
        /// <param name="lsClientId">The ClientId of the UserAuth.</param>
        /// <returns>List of UserAuths.</returns>
        public MaxEntityList LoadAllByClientIdCache(string lsClientId)
        {
            return this.LoadAllByPropertyCache(this.DataModel.ClientId, lsClientId);
        }
	}
}
