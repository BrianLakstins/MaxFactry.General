// <copyright file="IMaxUserLogRepositoryProvider.cs" company="Lakstins Family, LLC">
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

namespace MaxFactry.General.DataLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.DataLayer;

	/// <summary>
    /// Interface used to manage with the User log information.
	/// </summary>
    public interface IMaxUserLogRepositoryProvider : IMaxBaseIdRepositoryProvider
	{
        /// <summary>
        /// Selects a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="loData">The user log data.</param>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <param name="ldCreatedDate">Date the log entry was created.</param>
        /// <returns>List of user logs.</returns>
        MaxDataList SelectAllUserLogByLogEntryTypeCreatedDate(MaxData loData, int lnLogEntryType, DateTime ldCreatedDate);

        /// <summary>
        /// Selects a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="loData">The user log data.</param>
        /// <param name="loUserId">The Id of the user.</param>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <returns>List of user logs.</returns>
        MaxDataList SelectAllUserLogByUserIdLogEntryType(MaxData loData, Guid loUserId, int lnLogEntryType);

        /// <summary>
        /// Selects a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="loData">The user log data.</param>
        /// <param name="loUserId">The Id of the user.</param>
        /// <param name="ldCreatedDate">Date the log entry was created.</param>
        /// <returns>List of user logs.</returns>
        MaxDataList SelectAllUserLogByUserIdCreatedDate(MaxData loData, Guid loUserId, DateTime ldCreatedDate);

        /// <summary>
        /// Selects a list of all user logs of the specified type created since a certain date.
        /// </summary>
        /// <param name="loData">The user log data.</param>
        /// <param name="loUserId">The Id of the user.</param>
        /// <param name="lnLogEntryType">Type of log entry.</param>
        /// <param name="ldCreatedDate">Date the log entry was created.</param>
        /// <returns>List of user logs.</returns>
        MaxDataList SelectAllUserLogByUserIdLogEntryTypeCreatedDate(MaxData loData, Guid loUserId, int lnLogEntryType, DateTime ldCreatedDate);
	}
}
