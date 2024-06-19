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
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class. Use parent methods instead of repository.">
// <change date="6/19/2024" author="Brian A. Lakstins" description="Add user related logging types.">
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
    /// Entity used to manage log information about users for the MaxSecurityProvider.
	/// </summary>
	public class MaxUserLogEntity : MaxBaseEntity
	{
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
        public const int LogEntryTypeUserDelete = 10;

        /// <summary>
        /// Value for adding a token for the user
        /// </summary>
        public const int LogEntryTypeUserAuthTokenInsert = 11;

        /// <summary>
        /// Value for adding an auth for a user
        /// </summary>
        public const int LogEntryTypeUserAuthInsert = 12;

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
		public MaxEntityList LoadAllByUserIdCache(Guid loUserId)
		{
            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.AddFilter(this.DataModel.UserId, "=", loUserId);
            MaxData loData = new MaxData(this.Data);
            return this.LoadAllByPageCache(loData, 0, 0, this.DataModel.CreatedDate + " desc", loDataQuery);
		}

        /// <summary>
        /// Loads a list of all user logs associated to the User Id.
        /// </summary>
        /// <param name="loUserId">The Id of the user.</param>
        /// <returns>List of user logs.</returns>
        public MaxEntityList LoadAllByUserIdCreatedDate(Guid loUserId, DateTime ldCreatedDate)
        {
            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.AddFilter(this.DataModel.UserId, "=", loUserId);
            loDataQuery.AddAnd();
            loDataQuery.AddFilter(this.DataModel.CreatedDate, ">=", ldCreatedDate);
            MaxData loData = new MaxData(this.Data);
            return this.LoadAllByPageCache(loData, 0, 0, this.DataModel.CreatedDate + " desc", loDataQuery);
        }

        /// <summary>
        /// Loads a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="ldCreatedDate">Date the log entry was created.</param>
        /// <returns>List of user logs.</returns>
        public int GetCountActivityByCreatedDate(DateTime ldCreatedDate)
        {
            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.AddFilter(this.DataModel.CreatedDate, ">=", ldCreatedDate);
            MaxData loData = new MaxData(this.Data);
            MaxEntityList loList = LoadAllByPageCache(loData, 1, 1, this.DataModel.CreatedDate + " desc", loDataQuery);
            return loList.Total;
        }

        /// <summary>
        /// Adds a new log entry for the supplied user id, log entry type, and comment.
        /// Needed because UserId, log entry type, and comment are readonly.
        /// </summary>
        /// <param name="loId">The new Id.</param>
        /// <param name="loUserId">The Id of the user.</param>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <param name="lsComment">Comment related to log entry.</param>
        /// <returns>true if inserted.  False if not.</returns>
        public bool Insert(Guid loUserId, int lnLogEntryType, string lsComment)
        {
            this.Set(this.DataModel.UserId, loUserId);
            this.Set(this.DataModel.LogEntryType, lnLogEntryType);
            this.Set(this.DataModel.Comment, lsComment);
            return this.Insert();
        }

        /// <summary>
        /// Password entities are not updated.
        /// </summary>
        /// <returns>Throws an exception to prevent using this method.</returns>
        public override bool Update()
        {
            throw new MaxException("Log entries cannot be updated.");
        }

        public override bool Delete() 
        {
            throw new MaxException("Log entries cannot be deleted.");
                
        }
	}
}
