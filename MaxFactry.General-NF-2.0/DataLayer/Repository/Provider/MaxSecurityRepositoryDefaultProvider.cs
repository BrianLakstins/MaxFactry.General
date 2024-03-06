// <copyright file="MaxSecurityRepositoryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer.Provider
{
	using System;
	using MaxFactry.Core;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Provider;

    /// <summary>
    /// Provider for all Membership repositories
    /// </summary>
    public class MaxSecurityRepositoryDefaultProvider : MaxBaseIdRepositoryDefaultProvider,
        IMaxUserRepositoryProvider,
        IMaxUserLogRepositoryProvider
    {
        /// <summary>
        /// Selects all users that match the given username.
        /// </summary>
        /// <param name="loData">The user data.</param>
        /// <param name="lsUserName">The username of the user.</param>
        /// <param name="lnPageIndex">Page of data to select.</param>
        /// <param name="lnPageSize">Size of the page to select.</param>
        /// <param name="lsOrderBy">Sort information.</param>
        /// <param name="lnTotal">Total matching records.</param>
        /// <returns>List of users.</returns>
        public virtual MaxDataList SelectAllUserByUserName(MaxData loData, string lsUserName, int lnPageIndex, int lnPageSize, string lsOrderBy, out int lnTotal)
        {
            MaxUserDataModel loDataModel = loData.DataModel as MaxUserDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.StartGroup();
            loDataQuery.AddFilter(loDataModel.IsDeleted, "=", false);
            if (null != lsUserName && lsUserName.Length > 0)
            {
                loDataQuery.AddCondition("AND");
                loDataQuery.AddFilter(loDataModel.UserName, "=", lsUserName);
            }

            loDataQuery.EndGroup();

            lnTotal = 0;
            MaxDataList loDataList = this.Select(loData, loDataQuery, lnPageIndex, lnPageSize, lsOrderBy, out lnTotal);
            return loDataList;
        }

        /// <summary>
        /// Selects all users that match the given username.
        /// </summary>
        /// <param name="loData">The user data.</param>
        /// <param name="lsEmail">The email of the user.</param>
        /// <param name="lnPageIndex">Page of data to select.</param>
        /// <param name="lnPageSize">Size of the page to select.</param>
        /// <param name="lsOrderBy">Sort information</param>
        /// <param name="lnTotal">Total matching records.</param>
        /// <returns>List of users.</returns>
        public virtual MaxDataList SelectAllUserByEmail(MaxData loData, string lsEmail, int lnPageIndex, int lnPageSize, string lsOrderBy, out int lnTotal)
        {
            MaxUserDataModel loDataModel = loData.DataModel as MaxUserDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.StartGroup();
            loDataQuery.AddFilter(loDataModel.IsDeleted, "=", false);
            if (null != lsEmail && lsEmail.Length > 0)
            {
                loDataQuery.AddCondition("AND");
                loDataQuery.AddFilter(loDataModel.Email, "=", lsEmail);
            }

            loDataQuery.EndGroup();

            lnTotal = 0;
            MaxDataList loDataList = this.Select(loData, loDataQuery, lnPageIndex, lnPageSize, lsOrderBy, out lnTotal);
            return loDataList;
        }

        /// <summary>
        /// Selects all users that match the given username.
        /// </summary>
        /// <param name="loData">The user data.</param>
        /// <param name="lsUserName">The username of the user.</param>
        /// <param name="lnPageIndex">Page of data to select.</param>
        /// <param name="lnPageSize">Size of the page to select.</param>
        /// <param name="lsOrderBy">Sort information</param>
        /// <param name="lnTotal">Total matching records.</param>
        /// <returns>List of users.</returns>
        public virtual MaxDataList SelectAllUserByUserNamePartial(MaxData loData, string lsUserName, int lnPageIndex, int lnPageSize, string lsOrderBy, out int lnTotal)
        {
            MaxUserDataModel loDataModel = loData.DataModel as MaxUserDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.StartGroup();
            loDataQuery.AddFilter(loDataModel.IsDeleted, "=", false);
            if (null != lsUserName && lsUserName.Length > 0)
            {
                loDataQuery.AddCondition("AND");
                loDataQuery.AddFilter(loDataModel.UserName, "LIKE", "%" + lsUserName + "%");
            }

            loDataQuery.EndGroup();            
            lnTotal = 0;
            MaxDataList loDataList = this.Select(loData, loDataQuery, lnPageIndex, lnPageSize, lsOrderBy, out lnTotal);
            return loDataList;
        }

        /// <summary>
        /// Selects all users that match the given username.
        /// </summary>
        /// <param name="loData">The user data.</param>
        /// <param name="lsEmail">The email of the user.</param>
        /// <param name="lnPageIndex">Page of data to select.</param>
        /// <param name="lnPageSize">Size of the page to select.</param>
        /// <param name="lsOrderBy">Sort information</param>
        /// <param name="lnTotal">Total matching records.</param>
        /// <returns>List of users.</returns>
        public virtual MaxDataList SelectAllUserByEmailPartial(MaxData loData, string lsEmail, int lnPageIndex, int lnPageSize, string lsOrderBy, out int lnTotal)
        {
            MaxUserDataModel loDataModel = loData.DataModel as MaxUserDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.StartGroup();
            loDataQuery.AddFilter(loDataModel.IsDeleted, "=", false);
            if (null != lsEmail && lsEmail.Length > 0)
            {
                loDataQuery.AddCondition("AND");
                loDataQuery.AddFilter(loDataModel.Email, "LIKE", "%" + lsEmail + "%");
            }

            loDataQuery.EndGroup(); 
            MaxDataList loDataList = this.Select(loData, loDataQuery, lnPageIndex, lnPageSize, lsOrderBy, out lnTotal);
            return loDataList;
        }

        /// <summary>
        /// Gets the count of users that match the username.
        /// </summary>
        /// <param name="loData">The user data model.</param>
        /// <param name="lsUserName">The username of the user.</param>
        /// <returns>Count of users.</returns>
        public virtual int GetUserCountByUserName(MaxData loData, string lsUserName)
        {
            MaxUserDataModel loDataModel = loData.DataModel as MaxUserDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.AddFilter(loDataModel.UserName, "LIKE", "%" + lsUserName + "%");
            return this.SelectCount(loData, loDataQuery);
        }

        /// <summary>
        /// Selects a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="loData">The user log data.</param>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <param name="ldCreatedDate">Date the log entry was created.</param>
        /// <returns>List of user logs.</returns>
        public virtual MaxDataList SelectAllUserLogByLogEntryTypeCreatedDate(MaxData loData, int lnLogEntryType, DateTime ldCreatedDate)
        {
            MaxUserLogDataModel loDataModel = loData.DataModel as MaxUserLogDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.StartGroup();
            loDataQuery.AddFilter(loDataModel.IsDeleted, "=", false);
            loDataQuery.AddCondition("AND");
            loDataQuery.AddFilter(loDataModel.LogEntryType, "=", lnLogEntryType);
            loDataQuery.AddCondition("AND");
            loDataQuery.AddFilter(loDataModel.CreatedDate, ">", ldCreatedDate);
            loDataQuery.EndGroup();
            int lnTotal = 0;
            MaxDataList loDataList = this.Select(loData, loDataQuery, 0, 0, string.Empty, out lnTotal);
            return loDataList;
        }

        /// <summary>
        /// Selects a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="loData">The user log data.</param>
        /// <param name="loUserId">The Id of the user.</param>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <returns>List of user logs.</returns>
        public virtual MaxDataList SelectAllUserLogByUserIdLogEntryType(MaxData loData, Guid loUserId, int lnLogEntryType)
        {
            MaxUserLogDataModel loDataModel = loData.DataModel as MaxUserLogDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.StartGroup();
            loDataQuery.AddFilter(loDataModel.IsDeleted, "=", false);
            loDataQuery.AddCondition("AND");
            loDataQuery.AddFilter(loDataModel.LogEntryType, "=", lnLogEntryType);
            loDataQuery.AddCondition("AND");
            loDataQuery.AddFilter(loDataModel.UserId, "=", loUserId);
            loDataQuery.EndGroup();
            int lnTotal = 0;
            MaxDataList loDataList = this.Select(loData, loDataQuery, 0, 0, string.Empty, out lnTotal);
            return loDataList;
        }

        /// <summary>
        /// Selects a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="loData">The user log data.</param>
        /// <param name="loUserId">The Id of the user.</param>
        /// <param name="ldCreatedDate">Date the log entry was created.</param>
        /// <returns>List of user logs.</returns>
        public virtual MaxDataList SelectAllUserLogByUserIdCreatedDate(MaxData loData, Guid loUserId, DateTime ldCreatedDate)
        {
            MaxUserLogDataModel loDataModel = loData.DataModel as MaxUserLogDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.StartGroup();
            loDataQuery.AddFilter(loDataModel.IsDeleted, "=", false);
            loDataQuery.AddCondition("AND");
            loDataQuery.AddFilter(loDataModel.UserId, "=", loUserId);
            loDataQuery.AddCondition("AND");
            loDataQuery.AddFilter(loDataModel.CreatedDate, ">", ldCreatedDate);
            loDataQuery.EndGroup();
            int lnTotal = 0;
            MaxDataList loDataList = this.Select(loData, loDataQuery, 0, 0, string.Empty, out lnTotal);
            return loDataList;
        }

        /// <summary>
        /// Selects a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="loData">The user log data.</param>
        /// <param name="loUserId">The Id of the user.</param>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <param name="ldCreatedDate">Date the log entry was created.</param>
        /// <returns>List of user logs.</returns>
        public virtual MaxDataList SelectAllUserLogByUserIdLogEntryTypeCreatedDate(MaxData loData, Guid loUserId, int lnLogEntryType, DateTime ldCreatedDate)
        {
            MaxUserLogDataModel loDataModel = loData.DataModel as MaxUserLogDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            MaxDataQuery loDataQuery = new MaxDataQuery();
            loDataQuery.StartGroup();
            loDataQuery.AddFilter(loDataModel.IsDeleted, "=", false);
            loDataQuery.AddCondition("AND");
            loDataQuery.AddFilter(loDataModel.LogEntryType, "=", lnLogEntryType);
            loDataQuery.AddCondition("AND");
            loDataQuery.AddFilter(loDataModel.UserId, "=", loUserId);
            loDataQuery.AddCondition("AND");
            loDataQuery.AddFilter(loDataModel.CreatedDate, ">", ldCreatedDate);
            loDataQuery.EndGroup();
            int lnTotal = 0;
            MaxDataList loDataList = this.Select(loData, loDataQuery, 0, 0, string.Empty, out lnTotal);
            return loDataList;
        }

        /// <summary>
        /// Selects all entities matching the ParentId
        /// </summary>
        /// <param name="loData">The role/user relation data.</param>
        /// <param name="loParentId">Unique Identifier of the parent</param>
        /// <returns>List of all entities</returns>
        public virtual MaxDataList SelectAllByParentId(MaxData loData, Guid loParentId)
        {
            MaxRelationDataModel loDataModel = loData.DataModel as MaxRelationDataModel;
            MaxData loDataNew = new MaxData(loDataModel);
            loDataNew.Set(loDataModel.ParentId, loParentId);
            MaxDataQuery loDataQuery = new MaxDataQuery();
            int lnTotal = 0;
            MaxDataList loDataList = this.Select(loDataNew, loDataQuery, 0, 0, string.Empty, out lnTotal);
            return loDataList;
        }

        /// <summary>
        /// Selects all entities matching the ChildId
        /// </summary>
        /// <param name="loData">The role/user relation data.</param>
        /// <param name="loChildId">Unique Identifier of the child</param>
        /// <returns>List of all entities</returns>
        public virtual MaxDataList SelectAllByChildId(MaxData loData, Guid loChildId)
        {
            MaxRelationDataModel loDataModel = loData.DataModel as MaxRelationDataModel;
            MaxData loDataNew = new MaxData(loDataModel);
            loDataNew.Set(loDataModel.ChildId, loChildId);
            MaxDataQuery loDataQuery = new MaxDataQuery();
            int lnTotal = 0;
            MaxDataList loDataList = this.Select(loDataNew, loDataQuery, 0, 0, string.Empty, out lnTotal);
            return loDataList;
        }
	}
}
