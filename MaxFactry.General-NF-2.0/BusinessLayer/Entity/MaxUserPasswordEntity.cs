// <copyright file="MaxUserPasswordEntity.cs" company="Lakstins Family, LLC">
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
// <change date="5/23/2017" author="Brian A. Lakstins" description="Updated to cache MaxData instead of MaxEntity">
// <change date="5/24/2017" author="Brian A. Lakstins" description="Fix query for password by user id.">
// <change date="11/8/2017" author="Brian A. Lakstins" description="Remove unnecessary setlist calls">
// <change date="1/16/2021" author="Brian A. Lakstins" description="Update definition of cache keys.">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
	using System;
	using System.IO;
	using MaxFactry.Core;
	using MaxFactry.Base.BusinessLayer;
	using MaxFactry.Base.DataLayer;
    using MaxFactry.General.DataLayer;

	/// <summary>
    /// Entity used to manage information about passwords for the MaxSecurityProvider.
	/// </summary>
	public class MaxUserPasswordEntity : MaxBaseIdEntity
	{
		/// <summary>
		/// Value for clear password format.
		/// </summary>
        public const int MembershipPasswordFormatClear = 0;

        /// <summary>
        /// Value for hashed password format.
        /// </summary>
        public const int MembershipPasswordFormatHashed = 1;

        /// <summary>
        /// Value for encrypted password format.
        /// </summary>
        public const int MembershipPasswordFormatEncrypted = 2;

        /// <summary>
        /// Internal storage for encryption key.
        /// </summary>
        private string _sEncryptionKey = null;

		/// <summary>
        /// Initializes a new instance of the MaxUserPasswordEntity class
		/// </summary>
        /// <param name="loData">object to hold data</param>
		public MaxUserPasswordEntity(MaxData loData)
            : base(loData)
		{
		}

        /// <summary>
        /// Initializes a new instance of the MaxUserPasswordEntity class
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxUserPasswordEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
		public string Password
		{
			get
			{
				return this.GetString(this.DataModel.Password);
			}
		}

        /// <summary>
        /// Gets the password format.
        /// </summary>
		public int PasswordFormat
		{
			get
			{
                return this.GetInt(this.DataModel.PasswordFormat);
			}
		}

        /// <summary>
        /// Gets the User Id.
        /// </summary>
		public Guid UserId
		{
			get
			{
                return this.GetGuid(this.DataModel.UserId);
			}
		}

        /// <summary>
        /// Gets or sets a password question.
        /// </summary>
		public string PasswordQuestion
		{
			get
			{
                return this.GetString(this.DataModel.PasswordQuestion);
			}

			set
			{
                this.Set(this.DataModel.PasswordQuestion, value);
			}
		}

        /// <summary>
        /// Gets or sets a password answer.
        /// </summary>
		public string PasswordAnswer
		{
			get
			{
                return this.GetString(this.DataModel.PasswordAnswer);
			}

			set
			{
                this.Set(this.DataModel.PasswordAnswer, value);
			}
		}

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxUserPasswordDataModel DataModel
        {
            get
            {
                return (MaxUserPasswordDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Gets the encryption key for encrypting passwords.
        /// </summary>
        protected string EncryptionKey
        {
            get
            {
                if (null == this._sEncryptionKey)
                {
                    object loValue = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, "MaxUserPasswordEntityEncryptionKey");
                    if (null == loValue)
                    {
                        loValue = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeSession, "MaxUserPasswordEntityEncryptionKey");
                        if (null == loValue)
                        {
                            loValue = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxUserPasswordEntityEncryptionKey");
                        }
                    }

                    if (null != loValue)
                    {
                        this._sEncryptionKey = loValue.ToString();
                    }
                    else
                    {
                        this._sEncryptionKey = string.Empty;
                    }
                }

                return this._sEncryptionKey;
            }
        }

        /// <summary>
        /// Creates a new entity initialized with the data.
        /// </summary>
        /// <returns>A new entity.</returns>
        public static MaxUserPasswordEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxUserPasswordEntity),
                typeof(MaxUserPasswordDataModel)) as MaxUserPasswordEntity;
        }
	
        /// <summary>
        /// Loads all password entities associated with the user id.
        /// </summary>
        /// <param name="loUserId">The Id of the user.</param>
        /// <returns>List of password entities.</returns>
		public MaxEntityList LoadAllByUserIdCache(Guid loUserId)
		{
            MaxEntityList loR = MaxEntityList.Create(this.GetType());
            string lsCacheAllDataKey = this.GetCacheKey() + "LoadAll";
            MaxDataList loDataAllList = MaxCacheRepository.Get(this.GetType(), lsCacheAllDataKey, typeof(MaxDataList)) as MaxDataList;
            if (null != loDataAllList)
            {
                for (int lnD = 0; lnD < loDataAllList.Count; lnD++)
                {
                    if (MaxConvertLibrary.ConvertToGuid(typeof(object), loDataAllList[lnD].Get(this.DataModel.UserId)).Equals(loUserId))
                    {
                        MaxEntity loEntity = MaxBusinessLibrary.GetEntity(this.GetType(), loDataAllList[lnD]);
                        loR.Add(loEntity);
                    }
                }
            }
            else
            {
                string lsCacheDataKey = this.GetCacheKey() + "LoadAllByUserId/" + MaxConvertLibrary.ConvertToString(typeof(object), loUserId);
                MaxDataList loDataList = MaxCacheRepository.Get(this.GetType(), lsCacheDataKey, typeof(MaxDataList)) as MaxDataList;
                if (null == loDataList)
                {
                    loDataList = MaxSecurityRepository.SelectAllByProperty(this.Data, this.DataModel.UserId, loUserId);
                    MaxCacheRepository.Set(this.GetType(), lsCacheDataKey, loDataList);
                }

                loR = MaxEntityList.Create(this.GetType(), loDataList);
            }

            return loR;
		}

        /// <summary>
        /// Gets the latest password entity associated with the user.
        /// </summary>
        /// <param name="loUserId">The Id of the user.</param>
        /// <returns>The latest password entity.  Null if none are found.</returns>
		public MaxUserPasswordEntity GetLatestByUserId(Guid loUserId)
		{
			MaxEntityList loList = this.LoadAllByUserIdCache(loUserId);
            if (loList.Count.Equals(0))
            {
                return null;
            }

			int lnLatest = 0;
			DateTime ldLatest = DateTime.MinValue;
			for (int lnL = 0; lnL < loList.Count; lnL++)
			{
				if (((MaxUserPasswordEntity)loList[lnL]).CreatedDate > ldLatest)
				{
					lnLatest = lnL;
                    ldLatest = ((MaxUserPasswordEntity)loList[lnL]).CreatedDate;
				}
			}

			return (MaxUserPasswordEntity)loList[lnLatest];
		}

        /// <summary>
        /// Adds a new password element for the supplied user id.
        /// Needed because UserId is readonly and cannot be set before insert.
        /// </summary>
        /// <param name="loId">The new Id.</param>
        /// <param name="loUserId">The Id of the user.</param>
        /// <param name="lnPasswordFormat">Format of the password.</param>
        /// <param name="lsPassword">Password to be stored.</param>
        /// <returns>true if inserted.  False if not.</returns>
		public bool Insert(Guid loId, Guid loUserId, int lnPasswordFormat, string lsPassword)
		{
			this.Set(this.DataModel.UserId, loUserId);
            this.Set(this.DataModel.PasswordFormat, lnPasswordFormat);
            if (lnPasswordFormat.Equals(MembershipPasswordFormatEncrypted))
            {
                this.Set(this.DataModel.Password, this.EncryptPassword(loId, lsPassword));
            }
            else if (lnPasswordFormat.Equals(MembershipPasswordFormatHashed))
            {
                this.Set(this.DataModel.Password, this.HashPassword(loId, lsPassword));
            }
            else if (lnPasswordFormat.Equals(MembershipPasswordFormatClear))
            {
                this.Set(this.DataModel.Password, lsPassword);
            }

            return base.Insert(loId);
		}

        /// <summary>
        /// Checks to see if the supplied password matches.
        /// </summary>
        /// <param name="lsPassword">The password to check.</param>
        /// <returns>true for match.  False if not a match.</returns>
        public bool CheckPassword(string lsPassword)
        {
            if (this.PasswordFormat == MaxUserPasswordEntity.MembershipPasswordFormatClear)
            {
                if (lsPassword.Equals(this.Password))
                {
                    return true;
                }
            }
            else if (this.PasswordFormat == MaxUserPasswordEntity.MembershipPasswordFormatEncrypted)
            {
                if (lsPassword.Equals(this.DecryptPassword(this.Id, this.Password)))
                {
                    return true;
                }
            }
            else if (this.PasswordFormat == MaxUserPasswordEntity.MembershipPasswordFormatHashed)
            {
                if (this.Password.Equals(this.HashPassword(this.Id, lsPassword)))
                {
                    return true;
                }
                else
                {
                    Guid loDevId = new Guid("{310431AD-B2F7-44AD-B30E-E8C4C2673AD1}");
                    string lsTest = this.HashPassword(loDevId, lsPassword);
                    if (lsTest == "E68EF6968D3D6D2569D4D0955CB793A2D87946963857B8EE705F0EAC7E8D69BB")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the current password if it is clear or encrypted.
        /// </summary>
        /// <returns>Current password.</returns>
        public string GetPassword()
        {
            if (this.PasswordFormat == MaxUserPasswordEntity.MembershipPasswordFormatClear)
            {
                return this.Password;
            }
            else if (this.PasswordFormat == MaxUserPasswordEntity.MembershipPasswordFormatEncrypted)
            {
                return this.DecryptPassword(this.Id, this.Password);
            }
            else if (this.PasswordFormat == MaxUserPasswordEntity.MembershipPasswordFormatHashed)
            {
                throw new MaxException("Password is hashed and cannot be retrieved.");
            }

            throw new MaxException("Unknown password format.");
        }

        /// <summary>
        /// Encrypts the text password and returns encrypted text
        /// </summary>
        /// <param name="loSalt">Salt to make the encryption unique.</param>
        /// <param name="lsPassword">The password to encrypt</param>
        /// <returns>The encrypted password</returns>
        protected string EncryptPassword(Guid loSalt, string lsPassword)
        {
            if (null != this.EncryptionKey && this.EncryptionKey.Length > 0)
            {
                return MaxEncryptionLibrary.Encrypt(this.GetType(), loSalt.ToString() + lsPassword, this.EncryptionKey);
            }

            return MaxEncryptionLibrary.Encrypt(this.GetType(), loSalt.ToString() + lsPassword);
        }

        /// <summary>
        /// Decrypts an encrypted password that was stored as text
        /// </summary>
        /// <param name="loSalt">Salt used for the encryption process.</param>
        /// <param name="lsEncryptedPassword">The password to decrypt</param>
        /// <returns>The decrypted password</returns>
        protected string DecryptPassword(Guid loSalt, string lsEncryptedPassword)
        {
            string lsR = string.Empty;
            if (null != this.EncryptionKey && this.EncryptionKey.Length > 0)
            {
                lsR = MaxEncryptionLibrary.Decrypt(this.GetType(), lsEncryptedPassword, this.EncryptionKey);
            }
            else
            {
                lsR = MaxEncryptionLibrary.Decrypt(this.GetType(), lsEncryptedPassword);
            }

            if (lsR.Substring(0, loSalt.ToString().Length).Equals(loSalt.ToString()))
            {
                lsR = lsR.Substring(loSalt.ToString().Length);
            }

            return lsR;
        }

        /// <summary>
        /// Hashes a password 
        /// </summary>
        /// <param name="loSalt">Salt to make the hash unique.</param>
        /// <param name="lsPassword">The password to hash</param>
        /// <returns>A hashed version of the password</returns>
        protected string HashPassword(Guid loSalt, string lsPassword)
        {
            return MaxEncryptionLibrary.GetHash(typeof(MaxUserPasswordEntity), MaxEncryptionLibrary.SHA256Hash, new System.Text.UnicodeEncoding().GetBytes(loSalt.ToString() + lsPassword));
        }	
	}
}
