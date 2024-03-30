// <copyright file="MaxUserDataModel.cs" company="Lakstins Family, LLC">
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
	/// Data model for the user information associated with the MaxSecurityProvider.
	/// </summary>
    public class MaxUserDataModel : MaxBaseGuidKeyDataModel
    {
		/// <summary>
		/// Username for the user account.
		/// </summary>
        public readonly string UserName = "UserName";

        /// <summary>
        /// Main email address associated with the user account.
        /// </summary>
		public readonly string Email = "Email";

		/// <summary>
		/// Information related to the user account.
		/// </summary>
        public readonly string Comment = "Comment";

		/// <summary>
        /// Initializes a new instance of the MaxUserDataModel class.
		/// </summary>
		public MaxUserDataModel() : base()
		{
            this.SetDataStorageName("MaxSecurityUser");
			this.AddType(this.UserName, typeof(string));
			this.AddType(this.Email, typeof(string));
			this.AddNullable(this.Comment, typeof(string));
            this.RepositoryProviderType = typeof(MaxFactry.General.DataLayer.Provider.MaxSecurityRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxSecurityRepository);
		}
	}
}
