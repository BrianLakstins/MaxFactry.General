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
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class. Use parent methods instead of repository.">
// <change date="6/19/2024" author="Brian A. Lakstins" description="Remove unneeded method.">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.BusinessLayer;
	using MaxFactry.Base.DataLayer;
    using MaxFactry.General.DataLayer;
    using MaxFactry.Base.DataLayer.Library;

    /// <summary>
    /// Entity used to manage information about users for the MaxSecurityProvider.
    /// </summary>
    public class MaxUserEntity : MaxBaseGuidKeyEntity
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

        public string GetAuthCode(string lsName)
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

        public void ClearAuthCode(string lsName)
        {
            string lsCacheKey = this.GetCacheKey() + "AuthCode/" + lsName.ToLower();
            MaxCacheRepository.Remove(this.GetType(), lsCacheKey);
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
        public MaxEntityList LoadAllByUsernamePartial(string lsUserName, int lnPageIndex, int lnPageSize, string lsPropertySort, params string[] laPropertyNameList)
        {
            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.AddFilter(this.UserName, "LIKE", "%" + lsUserName + "%");
            MaxData loData = new MaxData(this.Data);
            return this.LoadAllByPageCache(loData, lnPageSize, lnPageIndex, lsPropertySort, loDataQuery, laPropertyNameList);
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
        public MaxEntityList LoadAllByEmailPartial(string lsEmail, int lnPageIndex, int lnPageSize, string lsPropertySort, params string[] laPropertyNameList)
        {
            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.AddFilter(this.Email, "LIKE", "%" + lsEmail + "%");
            MaxData loData = new MaxData(this.Data);
            return this.LoadAllByPageCache(loData, lnPageSize, lnPageIndex, lsPropertySort, loDataQuery, laPropertyNameList);
        }

        /// <summary>
        /// Validates a user using some external provider.
        /// </summary>
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
	}
}
