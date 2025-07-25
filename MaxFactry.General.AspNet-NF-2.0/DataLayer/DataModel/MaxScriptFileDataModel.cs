﻿// <copyright file="MaxScriptFileDataModel.cs" company="Lakstins Family, LLC">
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
// <change date="7/14/2016" author="Brian A. Lakstins" description="Initial creation">
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
	public class MaxScriptFileDataModel : MaxBaseVersionedDataModel
    {
        /// <summary>
        /// The text content to be stored.
        /// </summary>
        public readonly string Content = "Content";

        /// <summary>
        /// The text content minimized
        /// </summary>
        public readonly string ContentMin = "ContentMin";

        /// <summary>
        /// The the name of the file to use
        /// </summary>
        public readonly string ContentName = "ContentName";

        /// <summary>
        /// The type of script file
        /// </summary>
        public readonly string ScriptType = "ScriptType";

		/// <summary>
        /// Initializes a new instance of the MaxScriptFileDataModel class.
		/// </summary>
        public MaxScriptFileDataModel()
            : base()
		{
            this.RepositoryProviderType = typeof(MaxFactry.General.AspNet.DataLayer.Provider.MaxBaseAspNetRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxBaseAspNetRepository);
            this.SetDataStorageName("MaxCoreAspNetScriptFile");
            this.AddType(this.Content, typeof(MaxLongString));
            this.AddNullable(this.ContentMin, typeof(MaxLongString));
            this.AddNullable(this.ContentName, typeof(MaxShortString));
            this.AddNullable(this.ScriptType, typeof(MaxShortString));
        }
	}
}
