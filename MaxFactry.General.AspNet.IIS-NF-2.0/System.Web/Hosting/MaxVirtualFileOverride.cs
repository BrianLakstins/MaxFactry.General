// <copyright file="MaxVirtualFileOverride.cs" company="Lakstins Family, LLC">
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
// <change date="4/18/2014" author="Brian A. Lakstins" description="Initial Release">
// </changelog>
#endregion

namespace System.Web.Hosting
{
	using System;
    using System.IO;

    /// <summary>
    /// MaxFactry implementation of VirtualFile
    /// </summary>
	public class MaxVirtualFileOverride : VirtualFile
	{
		/// <summary>
		/// Internal storage of provided virtual path
		/// </summary>
        private string _sPath = string.Empty;

        /// <summary>
        /// Initializes a new instance of the MaxVirtualFile class.
        /// </summary>
        /// <param name="lsVirtualPath">Path to the virtual file.</param>
        public MaxVirtualFileOverride(string lsVirtualPath)
            : base(lsVirtualPath)
		{
            this._sPath = lsVirtualPath;
		}

        /// <summary>
        /// Opens a stream to the virtual file
        /// </summary>
        /// <returns>Stream of content of the virtual file.</returns>
		public override Stream Open()
		{
            Stream loR = MaxVirtualPathProviderOverride.Open(this._sPath);
            return loR;
		}
	}
}