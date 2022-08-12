// <copyright file="MaxClientToolDataModel.cs" company="Lakstins Family, LLC">
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
// <change date="6/4/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="9/4/2020" author="Brian A. Lakstins" description="Update fields used for storage">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.DataLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.DataLayer;

	/// <summary>
    /// Data model for client tools used in a web site.
	/// </summary>
	public class MaxClientToolDataModel : MaxBaseIdDataModel
	{
        public readonly string Name = "Name";

        public readonly string Version = "Version";

        public readonly string Location = "Location";

        public readonly string LocalUrl = "LocalUrl";

        public readonly string LocalMinUrl = "LocalMinUrl";

        public readonly string CDNUrl = "CDNUrl";

        public readonly string CDNMinUrl = "CDNMinUrl";

        public readonly string IncludeFilter = "IncludeFilter";

        public readonly string RequiredNameListText = "RequiredNameListText";

        public readonly string Content = "Content";

        public readonly string Description = "Description";

        /// <summary>
        /// Initializes a new instance of the MaxClientToolDataModel class.
		/// </summary>
        public MaxClientToolDataModel()
            : base()
		{
            this.RepositoryProviderType = typeof(MaxFactry.General.AspNet.DataLayer.Provider.MaxBaseAspNetRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxBaseAspNetRepository);
            this.SetDataStorageName("MaxCoreAspNetClientTool");
            this.AddType(this.Name, typeof(string));
            this.AddType(this.Version, typeof(MaxShortString));
            this.AddType(this.Location, typeof(int));
            this.AddNullable(this.LocalUrl, typeof(string));
            this.AddNullable(this.LocalMinUrl, typeof(string));
            this.AddNullable(this.CDNUrl, typeof(string));
            this.AddNullable(this.CDNMinUrl, typeof(string));
            this.AddNullable(this.IncludeFilter, typeof(string));
            this.AddNullable(this.RequiredNameListText, typeof(MaxLongString));
            this.AddNullable(this.Content, typeof(MaxLongString));
            this.AddNullable(this.Description, typeof(MaxLongString));
        }
    }
}
