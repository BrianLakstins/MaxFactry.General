// <copyright file="MaxVirtualTextFileDataModel.cs" company="Lakstins Family, LLC">
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
// <change date="4/21/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="6/24/2014" author="Brian A. Lakstins" description="Change storage name.">
// <change date="6/27/2014" author="Brian A. Lakstins" description="Change base to MaxBaseIdDataModel.">
// <change date="12/18/2019" author="Brian A. Lakstins" description="Allow Content to be long string.">
// <change date="6/4/2025" author="Brian A. Lakstins" description="Change base class to remove versioning">
// <change date="6/21/2025" author="Brian A. Lakstins" description="Change base class to add versioning back">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.DataLayer
{
	using MaxFactry.Base.DataLayer;

    /// <summary>
    /// Data model for the virtual files in a web site.
    /// </summary>
	public class MaxVirtualTextFileDataModel : MaxBaseVersionedDataModel
    {
        /// <summary>
        /// Content for the file
        /// </summary>
        public readonly string Content = "Content";

		/// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileDataModel class.
		/// </summary>
        public MaxVirtualTextFileDataModel()
            : base()
		{
            this.RepositoryProviderType = typeof(MaxFactry.General.AspNet.DataLayer.Provider.MaxBaseAspNetRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxBaseAspNetRepository);
            this.SetDataStorageName("MaxCoreAspNetVirtualTextFile");
            this.AddType(this.Content, typeof(MaxLongString));
        }
	}
}
