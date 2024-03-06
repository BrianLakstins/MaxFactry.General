// <copyright file="MaxUserEntity.cs" company="Lakstins Family, LLC">
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
// <change date="11/8/2017" author="Brian A. Lakstins" description="Remove unnecessary setlist calls">
// <change date="11/4/2020" author="Brian A. Lakstins" description="Fix issue where cache was being edited and not updated to data storage">
// <change date="1/16/2021" author="Brian A. Lakstins" description="Update definition of cache keys.">
// <change date="2/24/2021" author="Brian A. Lakstins" description="Update auth code storage">
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
    /// Entity used to manage information about users for the MaxSecurityProvider.
	/// </summary>
	public class MaxUserEntity : MaxBaseIdEntity
	{
		/// <summary>
        /// Initializes a new instance of the MaxUserEntity class
		/// </summary>
        /// <param name="loData">object to hold data</param>
		public MaxUserEntity(MaxData loData)
			: base(loData)
		{
		}

        /// <summary>
        /// Initializes a new instance of the MaxUserEntity class
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxUserEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
		public string UserName
		{
			get
			{
				return this.GetString(this.DataModel.UserName);
			}

			set
			{
				this.Set(this.DataModel.UserName, value);
			}
		}

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
		public string Email
		{
			get
			{
				return this.GetString(this.DataModel.Email);
			}

			set
			{
				this.Set(this.DataModel.Email, value);
			}
		}

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
		public string Comment
		{
			get
			{
				return this.GetString(this.DataModel.Comment);
			}

			set
			{
				this.Set(this.DataModel.Comment, value);
			}
		}

        public bool IsPasswordResetNeeded
        {
            get
            {
                return this.IsOptionFlagSet(1);
            }

            set
            {
                this.SetOptionFlag(1, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxUserDataModel DataModel
        {
            get
            {
                return (MaxUserDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new entity initialized with the data.
        /// </summary>
        /// <returns>A new entity.</returns>
        public static MaxUserEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxUserEntity),
                typeof(MaxUserDataModel)) as MaxUserEntity;
        }

        public string GetUserAuthCode(string lsName)
        {
            string lsCacheKey = this.GetCacheKey() + "AuthCode/" + lsName.ToLower();
            string lsR = MaxCacheRepository.Get(this.GetType(), lsCacheKey, typeof(string)) as string;
            if (string.IsNullOrEmpty(lsR))
            {
                lsR = string.Empty;
                while (lsR.Length < 6)
                {
                    lsR += MaxEncryptionLibrary.GetRandomInt(typeof(object), 0, 9).ToString();
                }

                MaxCacheRepository.Set(this.GetType(), lsCacheKey, lsR);
            }

            return lsR;
        }

        public void ClearUserAuthCode(string lsName)
        {
            string lsCacheKey = this.GetCacheKey() + "AuthCode/" + lsName.ToLower();
            MaxCacheRepository.Remove(this.GetType(), lsCacheKey);
        }

        /// <summary>
        /// Loads all user entities that match the given id that have not been deleted.
        /// </summary>
        /// <param name="loUserId">The Id of the user.</param>
        /// <returns>List of users.</returns>
        public MaxEntityList LoadAllByIdCache(Guid loUserId)
        {
            MaxEntityList loR = MaxEntityList.Create(this.GetType());
            string lsCacheAllDataKey = this.GetCacheKey() + "LoadAll";
            MaxDataList loDataAllList = MaxCacheRepository.Get(this.GetType(), lsCacheAllDataKey, typeof(MaxDataList)) as MaxDataList;
            if (null != loDataAllList)
            {
                for (int lnD = 0; lnD < loDataAllList.Count; lnD++)
                {
                    if (MaxConvertLibrary.ConvertToGuid(typeof(object), loDataAllList[lnD].Get(this.DataModel.Id)).Equals(loUserId))
                    {
                        MaxEntity loEntity = MaxBusinessLibrary.GetEntity(this.GetType(), loDataAllList[lnD].Clone());
                        loR.Add(loEntity);
                    }
                }
            }
            else
            {
                string lsCacheDataKey = this.GetCacheKey() + "LoadAllById/" + MaxConvertLibrary.ConvertToString(typeof(object), loUserId);
                MaxDataList loDataList = MaxCacheRepository.Get(this.GetType(), lsCacheDataKey, typeof(MaxDataList)) as MaxDataList;
                if (null == loDataList)
                {
                    loDataList = MaxSecurityRepository.SelectAllByProperty(this.Data, this.DataModel.Id, loUserId);
                    MaxCacheRepository.Set(this.GetType(), lsCacheDataKey, loDataList);
                }

                loR = MaxEntityList.Create(this.GetType(), loDataList);
            }

            return loR;
        }

        /// <summary>
        /// Loads all user entities that match the given username that have not been deleted.
        /// </summary>
        /// <param name="lsUserName">The username of the user.</param>
        /// <param name="lnPageIndex">Page of data to select.</param>
        /// <param name="lnPageSize">Size of the page to select.</param>
        /// <param name="lsSort">Sort information.</param>
        /// <param name="lnTotal">Total matching records.</param>
        /// <returns>List of users.</returns>
		public MaxEntityList LoadAllByUsernameCache(string lsUserName)
		{
            MaxEntityList loR = this.LoadAllByPropertyCache(this.DataModel.UserName, lsUserName);
            return loR;
		}

        /// <summary>
        /// Loads all user entities that match the given email that have not been deleted.
        /// </summary>
        /// <param name="lsEmail">The email of the user.</param>
        /// <param name="lnPageIndex">Page of data to select.</param>
        /// <param name="lnPageSize">Size of the page to select.</param>
        /// <param name="lsSort">Sort information.</param>
        /// <param name="lnTotal">Total matching records.</param>
        /// <returns>List of users.</returns>
		public MaxEntityList LoadAllByEmailCache(string lsEmail)
		{
            MaxEntityList loR = this.LoadAllByPropertyCache(this.DataModel.Email, lsEmail);
            return loR;
		}

        /// <summary>
        /// Loads all user entities that match the given username that have not been deleted.
        /// </summary>
        /// <param name="lsUserName">The username of the user.</param>
        /// <param name="lnPageIndex">Page of data to select.</param>
        /// <param name="lnPageSize">Size of the page to select.</param>
        /// <param name="lsPropertySort">Sort information.</param>
        /// <param name="lnTotal">Total matching records.</param>
        /// <returns>List of users.</returns>
        public MaxEntityList LoadAllByUsernamePartial(string lsUserName, int lnPageIndex, int lnPageSize, string lsPropertySort, out int lnTotal)
        {
            MaxEntityList loR = new MaxEntityList(this.GetType());
            MaxData loData = MaxUserEntity.Create().Data;
            string lsOrderBy = this.GetOrderBy(loData.DataModel, lsPropertySort);
            MaxDataList loDataList = MaxSecurityRepository.SelectAllByUserNamePartial(MaxUserEntity.Create().Data, lsUserName, lnPageIndex, lnPageSize, lsOrderBy, out lnTotal);
            loR = MaxEntityList.Create(this.GetType(), loDataList);
            loR = this.GetSorted(loR, lsPropertySort, lsOrderBy);
            return loR;
        }

        /// <summary>
        /// Loads all user entities that match the given email that have not been deleted.
        /// </summary>
        /// <param name="lsEmail">The email of the user.</param>
        /// <param name="lnPageIndex">Page of data to select.</param>
        /// <param name="lnPageSize">Size of the page to select.</param>
        /// <param name="lsPropertySort">Sort information.</param>
        /// <param name="lnTotal">Total matching records.</param>
        /// <returns>List of users.</returns>
        public MaxEntityList LoadAllByEmailPartial(string lsEmail, int lnPageIndex, int lnPageSize, string lsPropertySort, out int lnTotal)
        {
            lnTotal = 0;
            MaxEntityList loR = new MaxEntityList(this.GetType());
            MaxData loData = MaxUserEntity.Create().Data;
            string lsOrderBy = this.GetOrderBy(loData.DataModel, lsPropertySort);
            MaxDataList loDataList = MaxSecurityRepository.SelectAllByEmailPartial(loData, lsEmail, lnPageIndex, lnPageSize, lsOrderBy, out lnTotal);
            loR = MaxEntityList.Create(this.GetType(), loDataList);
            loR = this.GetSorted(loR, lsPropertySort, lsOrderBy);
            return loR;
        }

        /// <summary>
        /// Gets the count of all users that match the username (including any that have been deleted).
        /// </summary>
        /// <param name="lsUserName">The username of the user.</param>
        /// <returns>The number of users.</returns>
		public int GetCountByUserName(string lsUserName)
		{
            return MaxSecurityRepository.GetCountByUserName(this.Data, lsUserName);
		}

        /// <summary>
        /// Inserts a new record
        /// </summary>
        /// <param name="loId">Unique Id of this item</param>
        /// <returns>true if inserted.  False if cannot be inserted.</returns>
        public bool InsertMember(Guid loId)
        {
            return this.Insert(loId);
        }

        /// <summary>
        /// Checks the password that is passed to make sure it matches the current password
        /// </summary>
        /// <param name="lsPasswordToCheck">Password to check</param>
        /// <param name="loUserPassword">Password entity with current password information</param>
        /// <returns>true if matching, false if not matching</returns>
        protected bool CheckPassword(string lsPasswordToCheck, MaxUserPasswordEntity loUserPassword)
        {
            bool lbR = loUserPassword.CheckPassword(lsPasswordToCheck);
            MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
            if (lbR)
            {
                loMaxUserLog.Insert(
                    Guid.NewGuid(),
                    loUserPassword.UserId,
                    MaxUserLogEntity.LogEntryTypeLogin,
                    "CheckPassword Succeeded");
            }
            else
            {
                loMaxUserLog.Insert(
                    Guid.NewGuid(),
                    loUserPassword.UserId,
                    MaxUserLogEntity.LogEntryTypePasswordFail,
                    "CheckPassword failed using [" + lsPasswordToCheck + "]");
            }

            return lbR;
        }

        /// <summary>
        /// Validates a user using some external provider.
        /// </summary>
        /// <param name="loUser">The user to validate.</param>
        /// <param name="lsPassword">The password to check.</param>
        /// <returns>True if the password is correct.</returns>
        public virtual bool ValidateUserExternal(string lsPassword)
        {
            return false;
        }

        /// <summary>
        /// Validates a user using some external provider.
        /// </summary>
        /// <param name="lsUsername">The username to validate.</param>
        /// <param name="lsPassword">The password to check.</param>
        /// <returns>True if the password is correct.</returns>
        public virtual bool ValidateUserExternal(string lsUsername, string lsPassword)
        {
            return false;
        }

        public virtual bool LoadCurrent()
        {
            object loValue = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeAny, "MaxSecurityUserId");
            if (null != loValue)
            {
                Guid loId = MaxConvertLibrary.ConvertToGuid(typeof(object), loValue);
                return this.LoadByIdCache(loId);
            }

            return false;
        }

        public static bool IsAuthenticated
        {
            get
            {
                MaxUserEntity loUser = MaxUserEntity.Create();
                if (loUser.LoadCurrent())
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsInRoleList(string lsRoleList)
        {
            MaxUserEntity loUser = MaxUserEntity.Create();
            if (loUser.LoadCurrent())
            {
                string[] laRoleList = lsRoleList.Split(new char[] { ',' });

                MaxEntityList loList = MaxRoleEntity.Create().LoadAllByUserIdCache(loUser.Id);
                for (int lnE = 0; lnE < loList.Count; lnE++)
                {
                    string lsRoleToCheck = ((MaxRoleEntity)loList[lnE]).RoleName;
                    foreach (string lsRole in laRoleList)
                    {
                        if (lsRole.ToLower().Equals(lsRoleToCheck.ToLower()))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool IsCurrentUserInRoleList(string lsRoleList)
        {
            MaxUserEntity loUser = MaxUserEntity.Create();
            if (loUser.LoadCurrent())
            {
                return loUser.IsInRoleList(lsRoleList);
            }

            return false;
        }
	}
}
