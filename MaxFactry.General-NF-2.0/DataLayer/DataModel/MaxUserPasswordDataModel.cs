﻿// <copyright file="MaxUserPasswordDataModel.cs" company="Lakstins Family, LLC">
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
// <change date="3/20/2024" author="Brian A. Lakstins" description="Happy birthday to my mom.  Sara Jean Lakstins (Cartwright) - 3/20/1944 to 3/14/2019.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Change parent class.  Add storage for salt id instead of using id">
// <change date="6/4/2025" author="Brian A. Lakstins" description="Change base class and keys">
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.DataLayer;
    using System.Data;

    /// <summary>
    /// Data model for the password information associated with the MaxSecurityProvider.
    /// </summary>
    public class MaxUserPasswordDataModel : MaxBaseGuidKeyDataModel
	{
        /// <summary>
        /// Random salt value used to encrypt password
        /// </summary>
        public readonly string EncryptionSaltId = "EncryptionSaltId";

        /// <summary>
        /// Id of the user associated with the password.
        /// </summary>
        public readonly string UserId = "UserId";

        /// <summary>
        /// The password associated with this entry.
        /// </summary>
        public readonly string Password = "Password";

		/// <summary>
		/// The format used for the password (clear, encrypted, hashed).
		/// </summary>
        public readonly string PasswordFormat = "PasswordFormat";

        /// <summary>
        /// A question to use to help with password reset.
        /// </summary>
		public readonly string PasswordQuestion = "PasswordQuestion";

		/// <summary>
		/// The answer to the password reset question.
		/// </summary>
        public readonly string PasswordAnswer = "PasswordAnswer";

		/// <summary>
        /// Initializes a new instance of the MaxUserPasswordDataModel class.
		/// </summary>
		public MaxUserPasswordDataModel() : base()
		{
            this.SetDataStorageName("MaxSecurityUserPassword");
            this.RemoveType(this.CreatedDate);
            this.AddType(this.CreatedDate, typeof(DateTime));
            this.AddType(this.EncryptionSaltId, typeof(Guid));
			this.AddType(this.UserId, typeof(Guid));
			this.AddType(this.Password, typeof(string));
			this.AddType(this.PasswordFormat, typeof(int));
			this.AddNullable(this.PasswordQuestion, typeof(string));
            this.AddNullable(this.PasswordAnswer, typeof(string));
            this.RepositoryProviderType = typeof(MaxFactry.General.DataLayer.Provider.MaxSecurityRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxSecurityRepository);

			this.RemoveType(this.AttributeIndexText);
			this.RemoveType(this.OptionFlagList);
		}
	}
}
