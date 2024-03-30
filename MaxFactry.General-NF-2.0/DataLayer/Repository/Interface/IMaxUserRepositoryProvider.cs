// <copyright file="IMaxUserRepositoryProvider.cs" company="Lakstins Family, LLC">
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
    /// Interface used to manage with the User information.
	/// </summary>
    public interface IMaxUserRepositoryProvider : IMaxBaseIdRepositoryProvider
	{
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
		MaxDataList SelectAllUserByUserName(MaxData loData, string lsUserName, int lnPageIndex, int lnPageSize, string lsOrderBy);

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
        MaxDataList SelectAllUserByEmail(MaxData loData, string lsEmail, int lnPageIndex, int lnPageSize, string lsOrderBy);

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
        MaxDataList SelectAllUserByUserNamePartial(MaxData loData, string lsUserName, int lnPageIndex, int lnPageSize, string lsOrderBy);

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
        MaxDataList SelectAllUserByEmailPartial(MaxData loData, string lsEmail, int lnPageIndex, int lnPageSize, string lsOrderBy);

        /// <summary>
        /// Gets the count of users that match the username.
        /// </summary>
        /// <param name="loData">The user data.</param>
        /// <param name="lsUserName">The username of the user.</param>
        /// <returns>Count of users.</returns>
        int GetUserCountByUserName(MaxData loData, string lsUserName);
	}
}
