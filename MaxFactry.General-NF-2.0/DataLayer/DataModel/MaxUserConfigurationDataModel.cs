// <copyright file="MaxUserConfigurationDataModel.cs" company="Lakstins Family, LLC">
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
// <change date="3/30/2024" author="Brian A. Lakstins" description="Change parent class.">
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.DataLayer;

	/// <summary>
    /// Data model for the configuration information associated with the MaxSecurityProvider.
	/// </summary>
	public class MaxUserConfigurationDataModel : MaxBaseDataModel
    {
		/// <summary>
		/// Enable Password Retrieval setting name.
		/// </summary>
		public readonly string EnablePasswordRetrieval = "EnablePasswordRetrieval";

		/// <summary>
		/// Enable Password Reset setting name.
		/// </summary>
		public readonly string EnablePasswordReset = "EnablePasswordReset";

		/// <summary>
		/// Requires Question And Answer setting name.
		/// </summary>
		public readonly string RequiresQuestionAndAnswer = "RequiresQuestionAndAnswer";

		/// <summary>
		/// Max Invalid Password Attempts setting name.
		/// </summary>
		public readonly string MaxInvalidPasswordAttempts = "MaxInvalidPasswordAttempts";

		/// <summary>
		/// Password Attempt Window setting name.
		/// </summary>
		public readonly string PasswordAttemptWindow = "PasswordAttemptWindow";

		/// <summary>
		/// Requires Unique Email setting name.
		/// </summary>
		public readonly string RequiresUniqueEmail = "RequiresUniqueEmail";

		/// <summary>
		/// Membership Password Format setting name.
		/// </summary>
		public readonly string MembershipPasswordFormat = "MembershipPasswordFormat";

		/// <summary>
		/// Min Required Password Length setting name.
		/// </summary>
		public readonly string MinRequiredPasswordLength = "MinRequiredPasswordLength";

		/// <summary>
		/// Min Required Non Alphanumeric Characters setting name.
		/// </summary>
		public readonly string MinRequiredNonAlphanumericCharacters = "MinRequiredNonAlphanumericCharacters";

		/// <summary>
		/// Password Strength Regular Expression setting name.
		/// </summary>
		public readonly string PasswordStrengthRegularExpression = "PasswordStrengthRegularExpression";
 
        /// <summary>
        /// Number of minutes since last activity where user is considered online.
        /// </summary>
        public readonly string OnlineWindowDuration = "OnlineWindowDuration";

        /// <summary>
        /// Name of the application.
        /// </summary>
        public readonly string ApplicationName = "ApplicationName";

        /// <summary>
        /// Initializes a new instance of the MaxUserConfigurationDataModel class.
		/// </summary>
		public MaxUserConfigurationDataModel() : base()
		{
			this.SetDataStorageName("MaxSecurityUserConfiguration");
			this.AddType(this.EnablePasswordRetrieval, typeof(bool));
			this.AddType(this.EnablePasswordReset, typeof(bool));
			this.AddType(this.RequiresQuestionAndAnswer, typeof(bool));
			this.AddType(this.MaxInvalidPasswordAttempts, typeof(int));
			this.AddType(this.PasswordAttemptWindow, typeof(int));
			this.AddType(this.RequiresUniqueEmail, typeof(bool));
			this.AddType(this.MembershipPasswordFormat, typeof(int));
			this.AddType(this.MinRequiredPasswordLength, typeof(int));
			this.AddType(this.MinRequiredNonAlphanumericCharacters, typeof(int));
			this.AddType(this.PasswordStrengthRegularExpression, typeof(string));
            this.AddType(this.OnlineWindowDuration, typeof(int));
            this.AddNullable(this.ApplicationName, typeof(MaxShortString));
            this.RepositoryProviderType = typeof(MaxFactry.General.DataLayer.Provider.MaxSecurityRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxSecurityRepository);
        }
	}
}
