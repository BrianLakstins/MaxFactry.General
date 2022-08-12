// <copyright file="MaxUserLogEntity.cs" company="Lakstins Family, LLC">
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
// <change date="12/2/2019" author="Brian A. Lakstins" description="Added process to archive logs after 30 days">
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
    /// Entity used to manage log information about users for the MaxSecurityProvider.
	/// </summary>
	public class MaxUserLogEntity : MaxBaseIdEntity
	{
        /// <summary>
        /// Stores last archive date so it only runs once a day
        /// </summary>
        private static DateTime _dLastArchiveDate = DateTime.MinValue;

        /// <summary>
        /// Value for other log entry type
        /// </summary>
		public const int LogEntryTypeOther = 0;

        /// <summary>
        /// Value for other log entry type
        /// </summary>
        public const int LogEntryTypeLogin = 1;

        /// <summary>
        /// Value for log out log entry type
        /// </summary>
        public const int LogEntryTypeLogout = 2;

        /// <summary>
        /// Value for locked out log entry type
        /// </summary>
        public const int LogEntryTypeLockout = 3;

        /// <summary>
        /// Value for password fail log entry type
        /// </summary>
		public const int LogEntryTypePasswordFail = 4;

        /// <summary>
        /// Value for password change log entry type
        /// </summary>
        public const int LogEntryTypePasswordChange = 5;

        /// <summary>
        /// Value for activity log entry type
        /// </summary>
        public const int LogEntryTypeActivity = 6;

        /// <summary>
        /// Value for unlock log entry type
        /// </summary>
        public const int LogEntryTypeUnlockout = 7;

        /// <summary>
        /// Value for password question and answer log entry type
        /// </summary>
        public const int LogEntryTypePasswordQuestionAnswerChange = 8;

        /// <summary>
        /// Value for user change log entry type
        /// </summary>
        public const int LogEntryTypeUserChange = 9;

        /// <summary>
        /// Value for delete log entry type
        /// </summary>
        public const int LogEntryTypeUserDelete = 9;
        
		/// <summary>
        /// Initializes a new instance of the MaxUserLogEntity class
		/// </summary>
		/// <param name="loData">object to hold data</param>
		public MaxUserLogEntity(MaxData loData)
			: base(loData)
		{
		}

        /// <summary>
        /// Initializes a new instance of the MaxUserLogEntity class
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxUserLogEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets the user id
        /// </summary>
		public Guid UserId
		{
			get
			{
				return this.GetGuid(this.DataModel.UserId);
			}
		}

        /// <summary>
        /// Gets the log entry type
        /// </summary>
		public int LogEntryType
		{
			get
			{
				return this.GetInt(this.DataModel.LogEntryType);
			}
		}

        /// <summary>
        /// Gets the comment
        /// </summary>
		public string Comment
		{
			get
			{
				return this.GetString(this.DataModel.Comment);
			}
		}

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxUserLogDataModel DataModel
        {
            get
            {
                return (MaxUserLogDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new entity initialized with the data.
        /// </summary>
        /// <returns>A new entity.</returns>
        public static MaxUserLogEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxUserLogEntity),
                typeof(MaxUserLogDataModel)) as MaxUserLogEntity;
        }

        /// <summary>
        /// Loads a list of all user logs associated to the User Id.
        /// </summary>
        /// <param name="loUserId">The Id of the user.</param>
        /// <returns>List of user logs.</returns>
		public MaxEntityList LoadAllByUserId(Guid loUserId)
		{
            MaxDataList loDataList = MaxSecurityRepository.SelectAllByProperty(this.Data, this.DataModel.UserId, loUserId);
            MaxEntityList loEntityList = MaxEntityList.Create(this.GetType(), loDataList);
            return loEntityList;
		}

        /// <summary>
        /// Loads a list of all user logs associated to the User Id.
        /// </summary>
        /// <param name="loUserId">The Id of the user.</param>
        /// <returns>List of user logs.</returns>
        public MaxEntityList LoadAllByUserIdCreatedDate(Guid loUserId, DateTime ldCreatedDate)
        {
            MaxDataList loDataList = MaxSecurityRepository.SelectAllByUserIdCreatedDate(this.Data, loUserId, ldCreatedDate);
            MaxEntityList loEntityList = MaxEntityList.Create(this.GetType(), loDataList);
            return loEntityList;
        }

        /// <summary>
        /// Loads a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <param name="ldCreatedDate">Date the log entry was created.</param>
        /// <returns>List of user logs.</returns>
        public MaxEntityList LoadAllByLogEntryTypeCreatedDate(int lnLogEntryType, DateTime ldCreatedDate)
        {
            MaxDataList loDataList = MaxSecurityRepository.SelectAllByLogEntryTypeCreatedDate(this.Data, lnLogEntryType, ldCreatedDate);
            MaxEntityList loEntityList = MaxEntityList.Create(this.GetType(), loDataList);
            return loEntityList;
        }

        /// <summary>
        /// Loads a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="ldCreatedDate">Date the log entry was created.</param>
        /// <returns>List of user logs.</returns>
        public int GetCountActivityByCreatedDate(DateTime ldCreatedDate)
        {
            MaxDataList loDataList = MaxSecurityRepository.SelectAllByLogEntryTypeCreatedDate(this.Data, LogEntryTypeActivity, ldCreatedDate);
            return loDataList.Count;
        }

        /// <summary>
        /// Adds a new password element for the supplied user id, log entry type, and comment.
        /// Needed because UserId, log entry type, and comment are readonly.
        /// </summary>
        /// <param name="loId">The new Id.</param>
        /// <param name="loUserId">The Id of the user.</param>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <param name="lsComment">Comment related to log entry.</param>
        /// <returns>true if inserted.  False if not.</returns>
        public bool Insert(Guid loId, Guid loUserId, int lnLogEntryType, string lsComment)
        {
            this.Set(this.DataModel.UserId, loUserId);
            this.Set(this.DataModel.LogEntryType, lnLogEntryType);
            this.Set(this.DataModel.Comment, lsComment);
            return this.Insert(loId);
        }

        /// <summary>
        /// Runs archive process and then inserts a new record
        /// </summary>
        /// <param name="loId">Id for the new record</param>
        /// <returns>true if inserted.  False if cannot be inserted.</returns>
        public override bool Insert(Guid loId)
        {
            this.ArchiveCreatedOver30();
            bool lbR = base.Insert(loId);
            return lbR;
        }

        /// <summary>
        /// Password entities are not updated.
        /// </summary>
        /// <returns>Throws an exception to prevent using this method.</returns>
        public override bool Update()
        {
            throw new MaxException("Log entities cannot be updated.");
        }

        public int ArchiveCreatedOver30()
        {
            int lnR = 0;
            //// Prevent running archive process more than once per 24 hours
            if (this.CanProcessArchive(new TimeSpan(24, 0, 0)))
            {
                lnR = this.Archive(DateTime.UtcNow.Date.AddDays(-30), DateTime.MinValue, false);
            }

            return lnR;
        }
	}
}
