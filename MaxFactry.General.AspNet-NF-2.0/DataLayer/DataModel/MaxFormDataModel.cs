// <copyright file="MaxFormDataModel.cs" company="Lakstins Family, LLC">
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
// follAspNetg restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not claim that 
// you wrote the original software. If you use this software in a product, an 
// acknowledgment (see the follAspNetg) in the product documentation is required.
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
// <change date="1/23/2015" author="Brian A. Lakstins" description="Initial Release">
// <change date="5/24/2020" author="Brian A. Lakstins" description="Add constructor used for archive process">
// <change date="6/4/2025" author="Brian A. Lakstins" description="Update base class">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.DataLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.DataLayer;

	/// <summary>
    /// Data model for files downloaded from web sites.
	/// </summary>
	public class MaxFormDataModel : MaxBaseGuidKeyDataModel
	{
		/// <summary>
		/// The Id of the object this form is related to.
		/// </summary>
        public readonly string FormRelationId = "FormRelationId";

        /// <summary>
        /// The Id of the object these server variables are related to.
        /// </summary>
        public readonly string FormContent = "FormContent";

		/// <summary>
        /// Initializes a new instance of the MaxFormDataModel class.
		/// </summary>
        public MaxFormDataModel()
            : base()
		{
            this.RepositoryProviderType = typeof(MaxFactry.General.AspNet.DataLayer.Provider.MaxBaseAspNetRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxBaseAspNetRepository);
            this.SetDataStorageName("MaxCoreAspNetForm");
            this.AddType(this.FormRelationId, typeof(Guid));
            this.AddType(this.FormContent, typeof(byte[]));
        }

        /// <summary>
        /// Initializes a new instance of the MaxFormDataModel class.
        /// </summary>
        /// <param name="lsDataStorageName">Name to use for storage</param>
        public MaxFormDataModel(string lsDataStorageName) : this()
        {
            this.SetDataStorageName(lsDataStorageName);
        }
    }
}
