﻿// <copyright file="MaxFileUploadDataModel.cs" company="Lakstins Family, LLC">
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
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.DataLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.DataLayer;

	/// <summary>
    /// Data model for the virtual files in a web site.
	/// </summary>
	public class MaxFileUploadDataModel : MaxBaseIdFileDataModel
	{
		/// <summary>
		/// The name of the file that was uploaded.
		/// </summary>
        public readonly string UploadName = "UploadName";

		/// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileDataModel class.
		/// </summary>
        public MaxFileUploadDataModel()
            : base()
		{
            this.RepositoryProviderType = typeof(MaxFactry.General.AspNet.DataLayer.Provider.MaxBaseAspNetRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxBaseAspNetRepository);
            //this.SetDataStorageName("MaxSystemWebFileUpload");
            //// Copied 9/15/2015
            this.SetDataStorageName("MaxCoreAspNetFileUpload");
            this.AddType(this.UploadName, typeof(string));
        }
	}
}
