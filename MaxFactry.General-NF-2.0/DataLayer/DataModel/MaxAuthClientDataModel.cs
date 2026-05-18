// <copyright file="MaxAuthClientDataModel.cs" company="Lakstins Family, LLC">
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
// <change date="5/14/2026" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer
{
	using MaxFactry.Base.DataLayer;

	/// <summary>
	/// Data model for the authorization tokens
	/// </summary>
    public class MaxAuthClientDataModel : MaxBaseGuidKeyDataModel
    {
        /// <summary>
        /// Key used for the user associated with this Client Authorization.  Could be blank for a system wide client authorization.
        /// </summary>
        public readonly string UserKey = "UserKey";

		/// <summary>
		/// Name of the client associated with this token
		/// </summary>
		public readonly string Name = "Name";

        /// <summary>
        /// Description of the client associated with this token
        /// </summary>
        public readonly string Description = "Description";

		public readonly string ClientPublicKey = "ClientPublicKey";

		public readonly string KeyAlgorithm = "KeyAlgorithm";

        public readonly string Restrictions = "Restrictions";

        /// <summary>
        /// Initializes a new instance of the MaxAuthClientDataModel class.
        /// </summary>
        public MaxAuthClientDataModel() : base()
		{
            this.SetDataStorageName("MaxSecurityAuthClient");
            this.AddNullable(this.UserKey, typeof(string));
            this.AddNullable(this.Name, typeof(string));
            this.AddNullable(this.Description, typeof(MaxLongString));
			this.AddType(this.ClientPublicKey, typeof(string));
            this.AddType(this.KeyAlgorithm, typeof(MaxShortString));
            this.AddType(this.Restrictions, typeof(MaxLongString));

            this.RepositoryProviderType = typeof(MaxFactry.General.DataLayer.Provider.MaxGeneralRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxGeneralRepository);
		}
	}
}
