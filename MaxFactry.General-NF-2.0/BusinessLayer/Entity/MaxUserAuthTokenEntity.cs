// <copyright file="MaxUserAuthTokenEntity.cs" company="Lakstins Family, LLC">
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
    /// Entity used to manage information about UserAuthTokens for the MaxSecurityProvider.
	/// </summary>
	public class MaxUserAuthTokenEntity : MaxBaseIdEntity
	{
		/// <summary>
        /// Initializes a new instance of the MaxUserAuthTokenEntity class
		/// </summary>
        /// <param name="loData">object to hold data</param>
		public MaxUserAuthTokenEntity(MaxData loData)
			: base(loData)
		{
		}

        /// <summary>
        /// Initializes a new instance of the MaxUserAuthTokenEntity class
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxUserAuthTokenEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets or sets the key of the user who created this UserAuthToken
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
        /// Gets or sets the UserAuth Id
        /// </summary>
		public Guid UserAuthId
        {
            get
            {
                return this.GetGuid(this.DataModel.UserAuthId);
            }

            set
            {
                this.Set(this.DataModel.UserAuthId, value);
            }
        }

        /// <summary>
        /// Gets or sets the UserAuthGrant Id
        /// </summary>
		public Guid UserAuthGrantId
        {
            get
            {
                return this.GetGuid(this.DataModel.UserAuthGrantId);
            }

            set
            {
                this.Set(this.DataModel.UserAuthGrantId, value);
            }
        }

        /// <summary>
        /// Gets or sets the Token Hash.
        /// </summary>
		public string TokenHash
        {
			get
			{
				return this.GetString(this.DataModel.TokenHash);
			}

			set
			{
				this.Set(this.DataModel.TokenHash, value);
			}
		}

        /// <summary>
        /// Gets or sets the Token .
        /// </summary>
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

        public string TokenType
        {
			get
			{
				return this.GetString(this.DataModel.TokenType);
			}

			set
			{
				this.Set(this.DataModel.TokenType, value);
			}
		}

        public int Expiration
        {
            get
            {
                return this.GetInt(this.DataModel.Expiration);
            }

            set
            {
                this.Set(this.DataModel.Expiration, value);
            }
        }

        public string TokenResult
        {
            get
            {
                return this.GetString(this.DataModel.TokenResult);
            }

            set
            {
                this.Set(this.DataModel.TokenResult, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxUserAuthTokenDataModel DataModel
        {
            get
            {
                return (MaxUserAuthTokenDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new entity initialized with the data.
        /// </summary>
        /// <returns>A new entity.</returns>
        public static MaxUserAuthTokenEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxUserAuthTokenEntity),
                typeof(MaxUserAuthTokenDataModel)) as MaxUserAuthTokenEntity;
        }

        public string GetClientToken(string lsToken)
        {
            string lsR = string.Empty;
            string lsTokenText = MaxConvertLibrary.ConvertGuidToShortString(typeof(object), this.Id) + "|" + lsToken;
            byte[] laR = System.Text.Encoding.UTF8.GetBytes(lsTokenText);
            lsR = Convert.ToBase64String(laR).Replace("=", string.Empty);
            return lsR;
        }

        public MaxEntityList LoadAllActiveByUserKeyCache(string lsUserKey)
        {
            string lsCacheDataKey = this.GetCacheKey() + "LoadAllActiveByUserKeyCache/" + this.DataModel.UserKey + "/" + lsUserKey;
            MaxDataList loDataList = MaxCacheRepository.Get(this.GetType(), lsCacheDataKey, typeof(MaxDataList)) as MaxDataList;
            if (null == loDataList)
            {
                MaxDataQuery loDataQuery = new MaxDataQuery();
                loDataQuery.StartGroup();
                loDataQuery.AddFilter(this.MaxBaseIdDataModel.IsActive, "=", true);
                loDataQuery.AddCondition("AND");
                loDataQuery.AddFilter(this.DataModel.UserKey, "=", lsUserKey);
                loDataQuery.EndGroup();
                int lnTotal = 0;
                loDataList = MaxBaseIdRepository.Select(this.GetData(), loDataQuery, 0, 0, string.Empty, out lnTotal);
                MaxCacheRepository.Set(this.GetType(), lsCacheDataKey, loDataList);
            }

            MaxEntityList loEntityList = MaxEntityList.Create(this.GetType(), loDataList);
            return loEntityList;
        }

        public static string GetTokenHash(string lsToken)
        {
            return MaxEncryptionLibrary.GetHash(typeof(object), MaxEncryptionLibrary.SHA256Hash, lsToken);
        }

        public static MaxUserAuthTokenEntity GetByToken(string lsClientToken)
        {
            MaxUserAuthTokenEntity loR = MaxUserAuthTokenEntity.Create();
            while (lsClientToken.Length % 4 != 0)
            {
                lsClientToken += "=";
            }

            byte[] laClientToken = Convert.FromBase64String(lsClientToken);
            string lsTokenText = System.Text.Encoding.UTF8.GetString(laClientToken);
            if (lsTokenText.Contains("|"))
            {
                string[] laTokenText = lsTokenText.Split(new char[] { '|' });
                Guid loTokenId = MaxConvertLibrary.ConvertShortStringToGuid(typeof(object), laTokenText[0]);
                string lsTokenHash = GetTokenHash(laTokenText[1]);
                if (loR.LoadByIdCache(loTokenId) && loR.TokenHash == lsTokenHash)
                {
                    return loR;
                }
            }

            return null;
        }

        public static string GenerateToken(bool lbLetterOrDigit)
        {
            string lsR = string.Empty;
            int lnLength = MaxEncryptionLibrary.GetRandomInt(typeof(object), 15, 30);
            while (lsR.Length < lnLength)
            {
                //// Start at 48 (0) and go to 122 (z) for random characters
                int lnChar = MaxEncryptionLibrary.GetRandomInt(typeof(object), 48, 122);
                char loChar = (char)lnChar;
                if (lbLetterOrDigit)
                {
                    if (Char.IsLetterOrDigit(loChar))
                    {
                        lsR += loChar;
                    }
                }
                else
                {
                    lsR += loChar;
                }

            }

            return lsR;
        }

        /// <summary>
        /// Gets a current token for using the api
        /// </summary>
        /// <returns>Context provider</returns>
        public static string GetToken(string lsUrl, MaxIndex loCredentialIndex)
        {
            string lsR = string.Empty;
            string lsUserKey = MaxEncryptionLibrary.GetHash(typeof(object), MaxEncryptionLibrary.MD5Hash, lsUrl + MaxConvertLibrary.SerializeObjectToString(loCredentialIndex));
            MaxEntityList loList = MaxUserAuthTokenEntity.Create().LoadAllActiveByUserKeyCache(lsUserKey);
            for (int lnE = 0; lnE < loList.Count; lnE++)
            {
                MaxUserAuthTokenEntity loEntity = loList[lnE] as MaxUserAuthTokenEntity;
                if (DateTime.UtcNow.AddHours(1) < loEntity.CreatedDate.AddSeconds(loEntity.Expiration))
                {
                    lsR = loEntity.Token;
                }
                else
                {
                    loEntity.IsActive = false;
                    loEntity.Update();
                }
            }

            if (string.IsNullOrEmpty(lsR))
            {
                string lsTokenJson = string.Empty;
                if (loCredentialIndex.Contains("ClientId") && loCredentialIndex.Contains("Secret") && loCredentialIndex.Contains("Scopes"))
                {
                    lsTokenJson = MaxSecurityLibrary.GetAccessToken(typeof(object), new Uri(lsUrl), loCredentialIndex.GetValueString("ClientId"), loCredentialIndex.GetValueString("Secret"), loCredentialIndex.GetValueString("Scopes"));
                }
                else
                {
                    lsTokenJson = MaxSecurityLibrary.GetAccessToken(typeof(object), new Uri(lsUrl), loCredentialIndex);
                }

                if (!string.IsNullOrEmpty(lsTokenJson))
                {
                    MaxUserAuthTokenEntity loTokenEntity = MaxUserAuthTokenEntity.Create();
                    loTokenEntity.UserKey = lsUserKey;
                    loTokenEntity.TokenResult = lsTokenJson;
                    loTokenEntity.Insert();
                    MaxIndex loTokenIndex = MaxConvertLibrary.DeserializeObject(lsTokenJson, typeof(MaxIndex)) as MaxIndex;
                    loTokenEntity.Expiration = MaxConvertLibrary.ConvertToInt(typeof(object), loTokenIndex.GetValueString("expires_in"));
                    loTokenEntity.TokenType = loTokenIndex.GetValueString("token_type");
                    loTokenEntity.Token = loTokenIndex.GetValueString("access_token");
                    loTokenEntity.IsActive = true;
                    loTokenEntity.Update();
                    lsR = loTokenEntity.Token;
                }
            }

            return lsR;
        }

        public static MaxUserAuthTokenEntity AddToken(string lsToken, string lsTokenType, DateTime loExpiration, string lsUserKey, Guid loGrantId, Guid loUserAuthId)
        {
            MaxUserAuthTokenEntity loR = MaxUserAuthTokenEntity.Create();
            loR.Expiration = MaxConvertLibrary.ConvertToInt(typeof(object), (loExpiration - DateTime.UtcNow).TotalSeconds);
            loR.TokenType = lsTokenType;
            loR.TokenHash = MaxUserAuthTokenEntity.GetTokenHash(lsToken);
            loR.UserAuthGrantId = loGrantId;
            loR.UserAuthId = loUserAuthId;
            loR.UserKey = lsUserKey;
            loR.IsActive = true;
            loR.Insert();
            return loR;
        }
    }
}
