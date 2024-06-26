﻿// <copyright file="MaxUserAuthDataModel.cs" company="Lakstins Family, LLC">
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
// <change date="3/20/2024" author="Brian A. Lakstins" description="Happy birthday to my mom.  Sara Jean Lakstins (Cartwright) - 3/20/1944 to 3/14/2019.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Change parent class.  Add specific data name for the hash salt id instead of using the Id data name.">
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.DataLayer;

	/// <summary>
	/// Data model for the user information associated with the User Authoriziation
	/// </summary>
    public class MaxUserAuthDataModel : MaxBaseGuidKeyDataModel
    {
		/// <summary>
		/// Key used for the user associated with this Authorization
		/// </summary>
		public readonly string UserKey = "UserKey";

		/// <summary>
		/// Name for this Authorization
		/// </summary>
		public readonly string Name = "Name";

        /// <summary>
        /// ClientId used for this Authorization
        /// </summary>
		public readonly string ClientId = "ClientId";

		/// <summary>
		/// ClientSecret used for this Authorization
		/// </summary>
        public readonly string ClientSecret = "ClientSecret";

        /// <summary>
        /// Salt used to hash client secret
        /// </summary>
        public readonly string ClientSecretHashSaltId = "ClientSecretHashSaltId";

        /// <summary>
        /// Sha256 has of ClientSecret used for this Authorization
        /// </summary>
        public readonly string ClientSecretHash = "ClientSecretHash";

		/// <summary>
		/// List of use scopes this Authorization has access to
		/// </summary>
		public readonly string ScopeListText = "ScopeListText";

		/// <summary>
		/// List of use domains that can be used to access this Authorization
		/// </summary>
		public readonly string DomainListText = "DomainListText";

		/// <summary>
		/// Initializes a new instance of the MaxUserAuthDataModel class.
		/// </summary>
		public MaxUserAuthDataModel() : base()
		{
            this.SetDataStorageName("MaxSecurityUserAuth");
			this.AddType(this.UserKey, typeof(string));
			this.AddType(this.Name, typeof(string));
			this.AddType(this.ClientId, typeof(string));
			this.AddType(this.ClientSecret, typeof(string));
			this.AddAttribute(this.ClientSecret, AttributeIsEncrypted, "true");
            this.AddType(this.ClientSecretHashSaltId, typeof(Guid));
            this.AddType(this.ClientSecretHash, typeof(MaxLongString));
			this.AddNullable(this.ScopeListText, typeof(MaxLongString));
			this.AddNullable(this.DomainListText, typeof(MaxLongString));

			this.RepositoryProviderType = typeof(MaxFactry.General.DataLayer.Provider.MaxSecurityRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxSecurityRepository);
		}
	}
}
