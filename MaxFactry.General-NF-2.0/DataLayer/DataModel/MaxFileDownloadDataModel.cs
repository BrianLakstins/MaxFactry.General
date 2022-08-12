// <copyright file="MaxFileDownloadDataModel.cs" company="Lakstins Family, LLC">
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
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.DataLayer;

	/// <summary>
    /// Data model for files downloaded from web sites.
	/// </summary>
	public class MaxFileDownloadDataModel : MaxBaseIdFileDataModel
	{
		/// <summary>
		/// The url that was requested.
		/// </summary>
        public readonly string RequestUrl = "RequestUrl";

        /// <summary>
        /// The url that was actually downloaded.
        /// </summary>
        public readonly string ResponseUrl = "ResponseUrl";

        /// <summary>
        /// The header sent with the file request.
        /// </summary>
        public readonly string RequestHeader = "RequestHeader";

        /// <summary>
        /// The header received when the file was downloaded.
        /// </summary>
        public readonly string ResponseHeader = "ResponseHeader";

        /// <summary>
        /// The header received when the file was downloaded.
        /// </summary>
        public readonly string ResponseCookie = "ResponseCookie";

        /// <summary>
        /// The status code for the download.
        /// </summary>
        public readonly string StatusCode = "StatusCode";

        /// <summary>
        /// The remote server that the file as downloaded from.
        /// </summary>
        public readonly string ResponseHost = "RemoteServer";

		/// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileDataModel class.
		/// </summary>
        public MaxFileDownloadDataModel()
            : base()
		{
            this.RepositoryProviderType = typeof(MaxFactry.General.DataLayer.Provider.MaxGeneralRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxGeneralRepository);
            //this.SetDataStorageName("MaxCoreAspNetFileDownload");
            this.SetDataStorageName("MaxCoreAspNetFileDownload");
            this.AddType(this.RequestUrl, typeof(string));
            this.AddType(this.ResponseUrl, typeof(string));
            this.AddType(this.RequestHeader, typeof(string));
            this.AddType(this.ResponseHeader, typeof(string));
            this.AddType(this.ResponseCookie, typeof(string));
            this.AddType(this.StatusCode, typeof(string));
            this.AddType(this.ResponseHost, typeof(string));
        }
	}
}
