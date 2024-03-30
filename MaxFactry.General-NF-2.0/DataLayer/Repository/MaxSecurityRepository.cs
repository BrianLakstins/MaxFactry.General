// <copyright file="MaxSecurityRepository.cs" company="Lakstins Family, LLC">
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
// <change date="12/21/2016" author="Brian A. Lakstins" description="Updates for changes to core.">
// <change date="3/20/2024" author="Brian A. Lakstins" description="Happy birthday to my mom.  Sara Jean Lakstins (Cartwright) - 3/20/1944 to 3/14/2019.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Change parent classs.  Update for changes to parent class.">
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.DataLayer;

	/// <summary>
	/// Provides session services using MaxFactryLibrary.
    /// TODO: Use Cache
	/// </summary>
	public sealed class MaxSecurityRepository : MaxBaseRepository
	{
        /// <summary>
        /// Selects all users that match the given username.
        /// </summary>
        /// <param name="loData">Data used to determine provider.</param>
        /// <param name="lsUserName">The username of the user.</param>
        /// <param name="lnPageIndex">Page of data to select.</param>
        /// <param name="lnPageSize">Size of the page to select.</param>
        /// <param name="lsOrderBy">Sort information.</param>
        /// <param name="lnTotal">Total matching records.</param>
        /// <returns>List of users.</returns>
        public static MaxDataList SelectAllByUserName(MaxData loData, string lsUserName, int lnPageIndex, int lnPageSize, string lsOrderBy)
		{
            MaxUserDataModel loDataModel = loData.DataModel as MaxUserDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            loData.Set(loDataModel.UserName, lsUserName);
            MaxData loDataFilter = new MaxData(loData);
            loDataFilter.Set(loDataModel.UserName, lsUserName);
            IMaxBaseReadRepositoryProvider loRepositoryProvider = Instance.GetRepositoryProvider(loDataFilter);
            IMaxUserRepositoryProvider loProvider = loRepositoryProvider as IMaxUserRepositoryProvider;
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loRepositoryProvider.GetType() + "] for Provider");
            }

            MaxDataList loDataList = loProvider.SelectAllUserByUserName(loDataFilter, lsUserName, lnPageIndex, lnPageSize, lsOrderBy);
            return loDataList;
        }

        /// <summary>
        /// Selects all users that match the given username.
        /// </summary>
        /// <param name="loData">Data used to determine provider.</param>
        /// <param name="lsEmail">The email of the user.</param>
        /// <param name="lnPageIndex">Page of data to select.</param>
        /// <param name="lnPageSize">Size of the page to select.</param>
        /// <param name="lsOrderBy">Sort information.</param>
        /// <param name="lnTotal">Total matching records.</param>
        /// <returns>List of users.</returns>
        public static MaxDataList SelectAllByEmail(MaxData loData, string lsEmail, int lnPageIndex, int lnPageSize, string lsOrderBy)
		{
            MaxUserDataModel loDataModel = loData.DataModel as MaxUserDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            loData.Set(loDataModel.Email, lsEmail);
            MaxData loDataFilter = new MaxData(loData);
            loDataFilter.Set(loDataModel.Email, lsEmail);
            IMaxBaseReadRepositoryProvider loRepositoryProvider = Instance.GetRepositoryProvider(loDataFilter);
            IMaxUserRepositoryProvider loProvider = loRepositoryProvider as IMaxUserRepositoryProvider;
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loRepositoryProvider.GetType() + "] for Provider");
            }

            MaxDataList loDataList = loProvider.SelectAllUserByEmail(loDataFilter, lsEmail, lnPageIndex, lnPageSize, lsOrderBy);
            return loDataList;
        }

        /// <summary>
        /// Selects all users that match the given username.
        /// </summary>
        /// <param name="loData">Data used to determine provider.</param>
        /// <param name="lsUserName">The username of the user.</param>
        /// <param name="lnPageIndex">Page of data to select.</param>
        /// <param name="lnPageSize">Size of the page to select.</param>
        /// <param name="lsOrderBy">Sort information.</param>
        /// <param name="lnTotal">Total matching records.</param>
        /// <returns>List of users.</returns>
        public static MaxDataList SelectAllByUserNamePartial(MaxData loData, string lsUserName, int lnPageIndex, int lnPageSize, string lsOrderBy)
        {
            MaxData loDataFilter = new MaxData(loData);
            IMaxBaseReadRepositoryProvider loRepositoryProvider = Instance.GetRepositoryProvider(loDataFilter);
            IMaxUserRepositoryProvider loProvider = loRepositoryProvider as IMaxUserRepositoryProvider;
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loRepositoryProvider.GetType() + "] for Provider");
            }

            MaxDataList loDataList = loProvider.SelectAllUserByUserNamePartial(loDataFilter, lsUserName, lnPageIndex, lnPageSize, lsOrderBy);
            return loDataList;
        }

        /// <summary>
        /// Selects all users that match the given username.
        /// </summary>
        /// <param name="loData">Data used to determine provider.</param>
        /// <param name="lsEmail">The email of the user.</param>
        /// <param name="lnPageIndex">Page of data to select.</param>
        /// <param name="lnPageSize">Size of the page to select.</param>
        /// <param name="lsOrderBy">Sort information.</param>
        /// <param name="lnTotal">Total matching records.</param>
        /// <returns>List of users.</returns>
        public static MaxDataList SelectAllByEmailPartial(MaxData loData, string lsEmail, int lnPageIndex, int lnPageSize, string lsOrderBy)
        {
            MaxData loDataFilter = new MaxData(loData);
            IMaxBaseReadRepositoryProvider loRepositoryProvider = Instance.GetRepositoryProvider(loDataFilter);
            IMaxUserRepositoryProvider loProvider = loRepositoryProvider as IMaxUserRepositoryProvider;
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loRepositoryProvider.GetType() + "] for Provider");
            }

            MaxDataList loDataList = loProvider.SelectAllUserByEmailPartial(loDataFilter, lsEmail, lnPageIndex, lnPageSize, lsOrderBy);
            return loDataList;
        }

        /// <summary>
        /// Gets the count of users that match the username.
        /// </summary>
        /// <param name="loData">Data used to determine provider.</param>
        /// <param name="lsUserName">The username of the user.</param>
        /// <returns>Count of users.</returns>
        public static int GetCountByUserName(MaxData loData, string lsUserName)
        {
            MaxUserDataModel loDataModel = loData.DataModel as MaxUserDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            loData.Set(loDataModel.UserName, lsUserName);
            MaxData loDataFilter = new MaxData(loData);
            loDataFilter.Set(loDataModel.UserName, lsUserName);
            IMaxBaseReadRepositoryProvider loRepositoryProvider = Instance.GetRepositoryProvider(loDataFilter);
            IMaxUserRepositoryProvider loProvider = loRepositoryProvider as IMaxUserRepositoryProvider;
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loRepositoryProvider.GetType() + "] for Provider");
            }

            int lnR = loProvider.GetUserCountByUserName(loDataFilter, lsUserName);
            return lnR;
        }

        /// <summary>
        /// Selects a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="loData">Data used to determine provider.</param>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <param name="ldCreatedDate">Date the log entry was created.</param>
        /// <returns>List of user logs.</returns>
        public static MaxDataList SelectAllByLogEntryTypeCreatedDate(MaxData loData, int lnLogEntryType, DateTime ldCreatedDate)
        {
            MaxUserLogDataModel loDataModel = loData.DataModel as MaxUserLogDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            loData.Set(loDataModel.LogEntryType, lnLogEntryType);
            MaxData loDataFilter = new MaxData(loData);
            loDataFilter.Set(loDataModel.LogEntryType, lnLogEntryType);
            IMaxBaseReadRepositoryProvider loRepositoryProvider = Instance.GetRepositoryProvider(loDataFilter);
            IMaxUserLogRepositoryProvider loProvider = loRepositoryProvider as IMaxUserLogRepositoryProvider;
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loRepositoryProvider.GetType() + "] for Provider");
            }

            MaxDataList loDataList = loProvider.SelectAllUserLogByLogEntryTypeCreatedDate(loDataFilter, lnLogEntryType, ldCreatedDate);
            return loDataList;
        }

        /// <summary>
        /// Selects a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="loData">Data used to determine provider.</param>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <param name="ldCreatedDate">Date the log entry was created.</param>
        /// <returns>List of user logs.</returns>
        public static MaxDataList SelectAllByUserIdCreatedDate(MaxData loData, Guid loUserId, DateTime ldCreatedDate)
        {
            MaxUserLogDataModel loDataModel = loData.DataModel as MaxUserLogDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            loData.Set(loDataModel.UserId, loUserId);
            MaxData loDataFilter = new MaxData(loData);
            loDataFilter.Set(loDataModel.UserId, loUserId);
            IMaxBaseReadRepositoryProvider loRepositoryProvider = Instance.GetRepositoryProvider(loDataFilter);
            IMaxUserLogRepositoryProvider loProvider = loRepositoryProvider as IMaxUserLogRepositoryProvider;
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loRepositoryProvider.GetType() + "] for Provider");
            }

            MaxDataList loDataList = loProvider.SelectAllUserLogByUserIdCreatedDate(loDataFilter, loUserId, ldCreatedDate);
            return loDataList;
        }

        /// <summary>
        /// Selects a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="loData">Data used to determine provider.</param>
        /// <param name="loUserId">The Id of the user.</param>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <returns>List of user logs.</returns>
        public static MaxDataList SelectAllByUserIdLogEntryType(MaxData loData, Guid loUserId, int lnLogEntryType)
        {
            MaxUserLogDataModel loDataModel = loData.DataModel as MaxUserLogDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            loData.Set(loDataModel.UserId, loUserId);
            loData.Set(loDataModel.LogEntryType, lnLogEntryType);
            MaxData loDataFilter = new MaxData(loData);
            loDataFilter.Set(loDataModel.UserId, loUserId);
            loDataFilter.Set(loDataModel.LogEntryType, lnLogEntryType);
            IMaxBaseReadRepositoryProvider loRepositoryProvider = Instance.GetRepositoryProvider(loDataFilter);
            IMaxUserLogRepositoryProvider loProvider = loRepositoryProvider as IMaxUserLogRepositoryProvider;
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loRepositoryProvider.GetType() + "] for Provider");
            }

            MaxDataList loDataList = loProvider.SelectAllUserLogByUserIdLogEntryType(loDataFilter, loUserId, lnLogEntryType);
            return loDataList;
        }

        /// <summary>
        /// Selects a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="loData">Data used to determine provider.</param>
        /// <param name="loUserId">The Id of the user.</param>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <param name="ldCreatedDate">Date the log entry was created.</param>
        /// <returns>List of user logs.</returns>
        public static MaxDataList SelectAllByUserIdLogEntryTypeCreatedDate(MaxData loData, Guid loUserId, int lnLogEntryType, DateTime ldCreatedDate)
        {
            MaxUserLogDataModel loDataModel = loData.DataModel as MaxUserLogDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            loData.Set(loDataModel.UserId, loUserId);
            loData.Set(loDataModel.LogEntryType, lnLogEntryType);
            MaxData loDataFilter = new MaxData(loData);
            loDataFilter.Set(loDataModel.UserId, loUserId);
            loDataFilter.Set(loDataModel.LogEntryType, lnLogEntryType);
            IMaxBaseReadRepositoryProvider loRepositoryProvider = Instance.GetRepositoryProvider(loDataFilter);
            IMaxUserLogRepositoryProvider loProvider = loRepositoryProvider as IMaxUserLogRepositoryProvider;
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loRepositoryProvider.GetType() + "] for Provider");
            }

            MaxDataList loDataList = loProvider.SelectAllUserLogByUserIdLogEntryTypeCreatedDate(loDataFilter, loUserId, lnLogEntryType, ldCreatedDate);
            return loDataList;
        }
	}
}
