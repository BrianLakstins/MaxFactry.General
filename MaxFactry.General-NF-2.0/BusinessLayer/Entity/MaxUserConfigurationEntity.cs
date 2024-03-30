// <copyright file="MaxUserConfigurationEntity.cs" company="Lakstins Family, LLC">
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
// <change date="4/20/2016" author="Brian A. Lakstins" description="Updated to use centralized caching.">
// <change date="1/16/2021" author="Brian A. Lakstins" description="Update definition of cache keys.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class. Use parent methods instead of repository.">
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
    /// Entity used to manage information about configuration for the MaxSecurityProvider.
    /// </summary>
    public class MaxUserConfigurationEntity : MaxBaseEntity
	{
		/// <summary>
        /// Initializes a new instance of the MaxUserConfigurationEntity class.
		/// </summary>
        /// <param name="loData">object with initial data.</param>
		public MaxUserConfigurationEntity(MaxData loData)
			: base(loData)
		{
		}

        /// <summary>
        /// Initializes a new instance of the MaxUserConfigurationEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxUserConfigurationEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

		/// <summary>
		/// Gets or sets a value indicating whether passwords can be retrieved.
		/// </summary>
		public bool EnablePasswordRetrieval
		{
			get
			{
				return this.GetBoolean(this.DataModel.EnablePasswordRetrieval);
			}

			set
			{
                this.Set(this.DataModel.EnablePasswordRetrieval, value);
			}
		}

		/// <summary>
        /// Gets or sets a value indicating whether passwords can be reset.
		/// </summary>
		public bool EnablePasswordReset
		{
			get
			{
                return this.GetBoolean(this.DataModel.EnablePasswordReset);
			}

			set
			{
                this.Set(this.DataModel.EnablePasswordReset, value);
			}
		}

		/// <summary>
        /// Gets or sets a value indicating whether that a question and answer are needed.
		/// </summary>
		public bool RequiresQuestionAndAnswer
		{
			get
			{
                return this.GetBoolean(this.DataModel.RequiresQuestionAndAnswer);
			}

			set
			{
                this.Set(this.DataModel.RequiresQuestionAndAnswer, value);
			}
		}

		/// <summary>
		/// Gets or sets the maximum number of password attempts before something happens.
		/// </summary>
		public int MaxInvalidPasswordAttempts
		{
			get
			{
                return this.GetInt(this.DataModel.MaxInvalidPasswordAttempts);
			}

			set
			{
                this.Set(this.DataModel.MaxInvalidPasswordAttempts, value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating the duration before something changes about password attempts.
		/// </summary>
		public int PasswordAttemptWindow
		{
			get
			{
                return this.GetInt(this.DataModel.PasswordAttemptWindow);
			}

			set
			{
                this.Set(this.DataModel.PasswordAttemptWindow, value);
			}
		}

		/// <summary>
        /// Gets or sets a value indicating whether a unique email is required.
		/// </summary>
		public bool RequiresUniqueEmail
		{
			get
			{
                return this.GetBoolean(this.DataModel.RequiresUniqueEmail);
			}

			set
			{
                this.Set(this.DataModel.RequiresUniqueEmail, value);
			}
		}

		/// <summary>
		/// Gets or sets the format used for storing passwords.
		/// </summary>
		public int MembershipPasswordFormat
		{
			get
			{
                return this.GetInt(this.DataModel.MembershipPasswordFormat);
			}

			set
			{
                this.Set(this.DataModel.MembershipPasswordFormat, value);
			}
		}

		/// <summary>
		/// Gets or sets the Minimum password length.
		/// </summary>
		public int MinRequiredPasswordLength
		{
			get
			{
                return this.GetInt(this.DataModel.MinRequiredPasswordLength);
			}

			set
			{
                this.Set(this.DataModel.MinRequiredPasswordLength, value);
			}
		}

		/// <summary>
		/// Gets or sets the Minimum non alpha numeric characters.
		/// </summary>
		public int MinRequiredNonAlphanumericCharacters
		{
			get
			{
                return this.GetInt(this.DataModel.MinRequiredNonAlphanumericCharacters);
			}

			set
			{
                this.Set(this.DataModel.MinRequiredNonAlphanumericCharacters, value);
			}
		}

		/// <summary>
		/// Gets or sets a pattern to make sure passwords match.
		/// </summary>
		public string PasswordStrengthRegularExpression
		{
			get
			{
                return this.GetString(this.DataModel.PasswordStrengthRegularExpression);
			}

			set
			{
                this.Set(this.DataModel.PasswordStrengthRegularExpression, value);
			}
		}

        /// <summary>
        /// Gets or sets the online window duration.
        /// </summary>
        public int OnlineWindowDuration
        {
            get
            {
                return this.GetInt(this.DataModel.OnlineWindowDuration);
            }

            set
            {
                this.Set(this.DataModel.OnlineWindowDuration, value);
            }
        }

        /// <summary>
        /// Gets or sets the Application Name.
        /// </summary>
        public string ApplicationName
        {
            get
            {
                return this.GetString(this.DataModel.ApplicationName);
            }

            set
            {
                this.Set(this.DataModel.ApplicationName, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxUserConfigurationDataModel DataModel
        {
            get
            {
                return (MaxUserConfigurationDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new entity initialized with the data.
        /// </summary>
        /// <returns>A new entity.</returns>
        public static MaxUserConfigurationEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxUserConfigurationEntity),
                typeof(MaxUserConfigurationDataModel)) as MaxUserConfigurationEntity;
        }

        /// <summary>
        /// Gets the current configuration.  There is only one per storage key.
        /// </summary>
        /// <returns>The current membership configuration.</returns>
        public MaxUserConfigurationEntity GetCurrent()
        {
            MaxUserConfigurationEntity loR = null;
            MaxEntityList loList = this.LoadAllActiveCache();
            if (loList.Count > 0)
            {
                DateTime ldLatest = DateTime.MinValue;
                for (int lnL = 0; lnL < loList.Count; lnL++)
                {
                    MaxUserConfigurationEntity loEntity = loList[lnL] as MaxUserConfigurationEntity;
                    if (loEntity.CreatedDate > ldLatest)
                    {
                        loR = loEntity;
                        ldLatest = loEntity.CreatedDate;
                    }
                    else
                    {
                        loEntity.IsActive = false;
                        loEntity.Update();
                    }
                }
            }

            if (null == loR)
            {
                loR = MaxUserConfigurationEntity.Create();
                loR.EnablePasswordReset = true;
                loR.EnablePasswordRetrieval = false;
                loR.IsActive = true;
                loR.MaxInvalidPasswordAttempts = 5;
                loR.MembershipPasswordFormat = MaxUserPasswordEntity.MembershipPasswordFormatHashed;
                loR.MinRequiredNonAlphanumericCharacters = 0;
                loR.MinRequiredPasswordLength = 8;
                loR.PasswordAttemptWindow = 5;
                loR.PasswordStrengthRegularExpression = string.Empty;
                loR.RequiresQuestionAndAnswer = false;
                loR.RequiresUniqueEmail = true;
                loR.OnlineWindowDuration = 10;
                if (!loR.Insert())
                {
                    throw new MaxException("Unable to save user configuration");
                }
            }

            return loR;
        }
	}
}
