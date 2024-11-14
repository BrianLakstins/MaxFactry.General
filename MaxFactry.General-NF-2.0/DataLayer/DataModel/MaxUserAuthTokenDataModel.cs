// <copyright file="MaxUserAuthTokenDataModel.cs" company="Lakstins Family, LLC">
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
// <change date="3/20/2024" author="Brian A. Lakstins" description="Update for change to MaxDataModel">
// <change date="3/20/2024" author="Brian A. Lakstins" description="Happy birthday to my mom.  Sara Jean Lakstins (Cartwright) - 3/20/1944 to 3/14/2019.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Change parent class.">
// <change date="6/19/2024" author="Brian A. Lakstins" description="Add remote url field.">
// <change date="11/14/2024" author="Brian A. Lakstins" description="Add last used field.">
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.DataLayer;

	/// <summary>
	/// Data model for the authorization tokens
	/// </summary>
    public class MaxUserAuthTokenDataModel : MaxBaseGuidKeyDataModel
    {
		/// <summary>
		/// Key used for the user associated with this Authorization
		/// </summary>
		public readonly string UserKey = "UserKey";

		/// <summary>
		/// Id of User Auth associated with this token
		/// </summary>
		public readonly string UserAuthId = "UserAuthId";

		/// <summary>
		/// Id of User Grant Auth associated with this token
		/// </summary>
		public readonly string UserAuthGrantId = "UserAuthGrantId";

		/// <summary>
		/// Hash of random token information
		/// </summary>
		public readonly string TokenHash = "TokenHash";

		/// <summary>
		/// Token text
		/// </summary>
		public readonly string Token = "Token";

		/// <summary>
		/// Seconds since creation time before the token expires
		/// </summary>
		public readonly string Expiration = "Expiration";

		/// <summary>
		/// Type of this token
		/// </summary>
        public readonly string TokenType = "TokenType";

		/// <summary>
		/// Content of request for a token
		/// </summary>
		public readonly string TokenResult = "TokenResult";

        /// <summary>
        /// Remote Url
        /// </summary>
        public readonly string RemoteUrl = "RemoteUrl";

        /// <summary>
        /// Last time this was used
        /// </summary>
        public readonly string LastUsedDate = "LastUsedDate";

        /// <summary>
        /// Initializes a new instance of the MaxUserAuthTokenDataModel class.
        /// </summary>
        public MaxUserAuthTokenDataModel() : base()
		{
            this.SetDataStorageName("MaxSecurityUserAuthToken");
			this.AddType(this.UserKey, typeof(string));
			this.AddNullable(this.UserAuthId, typeof(Guid));
			this.AddNullable(this.UserAuthGrantId, typeof(Guid));
			this.AddNullable(this.TokenHash, typeof(string));
			this.AddNullable(this.Token, typeof(string));
            this.AddAttribute(this.Token, "IsEncrypted", "true");
            this.AddNullable(this.Expiration, typeof(int));
			this.AddNullable(this.TokenType, typeof(string));
			this.AddNullable(this.TokenResult, typeof(MaxLongString));
			this.AddAttribute(this.TokenResult, "IsEncrypted", "true");
            this.AddNullable(this.RemoteUrl, typeof(string));
			this.AddNullable(this.LastUsedDate, typeof(DateTime));

            this.RepositoryProviderType = typeof(MaxFactry.General.DataLayer.Provider.MaxGeneralRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxGeneralRepository);
		}
	}
}
