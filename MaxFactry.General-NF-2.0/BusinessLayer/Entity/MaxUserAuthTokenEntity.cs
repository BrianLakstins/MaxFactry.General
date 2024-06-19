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
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class. Use parent methods instead of repository.">
// <change date="6/19/2024" author="Brian A. Lakstins" description="Add user related logging.  Add remote url property.">
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
    /// Entity used to manage information about UserAuthTokens for the MaxSecurityProvider.
    /// </summary>
    public class MaxUserAuthTokenEntity : MaxBaseGuidKeyEntity
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

        public string RemoteUrl
        {
            get
            {
                return this.GetString(this.DataModel.RemoteUrl);
            }

            set
            {
                this.Set(this.DataModel.RemoteUrl, value);
            }
        }

        public bool IsExpired
        {
            get
            {
                bool lbR = true;
                if (DateTime.UtcNow < this.ExpirationDate)
                {
                    lbR = false;
                }

                return lbR;
            }
        }

        public DateTime ExpirationDate
        {
            get
            {
                return this.CreatedDate.AddSeconds(this.Expiration);
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
        /// Gets a token from a remote system.  Stores it locally to reuse until it's expiration.
        /// </summary>
        /// <param name="lsTokenUri">URI to use</param>
        /// <param name="lsClientId">Client Id</param>
        /// <param name="lsClientSecret">Client Secret</param>
        /// <param name="lsScope">Scope</param>
        /// <param name="loRequestContent">Extra content to send to remote source</param>
        /// <returns></returns>
        public static string GetRemoteToken(string lsTokenUri, string lsClientId, string lsClientSecret, string lsScope, MaxIndex loRequestContent)
        {
            string lsR = string.Empty;
            string lsUserKey = MaxEncryptionLibrary.GetHash(typeof(object), MaxEncryptionLibrary.MD5Hash, lsTokenUri + lsClientId + lsClientSecret + lsScope);
            MaxEntityList loList = MaxUserAuthTokenEntity.Create().LoadAllActiveByUserKeyCache(lsUserKey);
            for (int lnE = 0; lnE < loList.Count; lnE++)
            {
                MaxUserAuthTokenEntity loEntity = loList[lnE] as MaxUserAuthTokenEntity;
                //// Tokens need to be valid for at least 30 more seconds to be used
                if (DateTime.UtcNow.AddSeconds(30) < loEntity.CreatedDate.AddSeconds(loEntity.Expiration))
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
                MaxUserAuthTokenEntity loEntity = MaxUserAuthTokenEntity.Create();
                if (loEntity.LoadRemote(lsTokenUri, lsClientId, lsClientSecret, lsScope, loRequestContent))
                {
                    loEntity.RemoteUrl = lsTokenUri;
                    loEntity.UserKey = lsUserKey;
                    loEntity.IsActive = true;
                    loEntity.Insert();
                    lsR = loEntity.Token;
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
            return this.LoadAllActiveByProperty(this.DataModel.UserKey, lsUserKey);
        }

        /// <summary>
        /// Loads a token from a remote source
        /// </summary>
        /// <param name="lsTokenUri">URI to use</param>
        /// <param name="lsClientId">Client Id</param>
        /// <param name="lsClientSecret">Client Secret</param>
        /// <param name="lsScope">Scope</param>
        /// <param name="loRequestContent">Extra content to send to remote source</param>
        /// <returns></returns>
        public virtual bool LoadRemote(string lsTokenUri, string lsClientId, string lsClientSecret, string lsScope, MaxIndex loRequestContent)
        {
            string lsToken = MaxHttpLibrary.GetAccessToken(new Uri(lsTokenUri), lsClientId, lsClientSecret, lsScope);
            this.RemoteUrl = lsTokenUri;
            bool lbR = this.MapResponse(lsToken);
            return lbR;
        }

        /// <summary>
        /// Maps the response from a remote URL to this token
        /// </summary>
        /// <param name="loRemoteResponse">String or stream from remote server</param>
        /// <returns></returns>
        public bool MapResponse(object loRemoteResponse)
        {
            bool lbR = false;
            string lsContent = string.Empty;
            if (loRemoteResponse is string)
            {
                lsContent = loRemoteResponse as string;
            }
            else if (loRemoteResponse is Stream)
            {
                lsContent = new StreamReader((Stream)loRemoteResponse).ReadToEnd();
            }

            //// {"token_type":"Bearer","expires_in":3599,"ext_expires_in":3599,"access_token": ""}
            MaxIndex loTokenIndex = MaxConvertLibrary.DeserializeObject(lsContent, typeof(MaxIndex)) as MaxIndex;
            if (loTokenIndex.Contains("token_type") && loTokenIndex.Contains("expires_in") && loTokenIndex.Contains("access_token"))
            {
                this.TokenResult = lsContent;
                this.TokenType = loTokenIndex.GetValueString("token_type");
                this.Expiration = MaxConvertLibrary.ConvertToInt(typeof(object), loTokenIndex.GetValueString("expires_in"));
                this.Token = loTokenIndex.GetValueString("access_token");
                lbR = true;
            }

            return lbR;
        }

        public override bool Insert()
        {
            bool lbR = base.Insert();
            Guid loUserId = Guid.Empty;
            if (Guid.TryParse(this.UserKey, out loUserId) && loUserId != Guid.Empty)
            {
                MaxUserEntity loUser = MaxUserEntity.Create();
                if (loUser.LoadByIdCache(loUserId))
                {
                    MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
                    if (lbR)
                    {
                        loMaxUserLog.Insert(
                            loUserId,
                            MaxUserLogEntity.LogEntryTypeUserAuthTokenInsert,
                            this.GetType() + ".Insert() - succeeded");
                    }
                    else
                    {
                        loMaxUserLog.Insert(
                            loUserId,
                            MaxUserLogEntity.LogEntryTypeUserAuthTokenInsert,
                            this.GetType() + ".Insert() - failed");
                    }
                }
            }

            return lbR;
        }
    }
}
