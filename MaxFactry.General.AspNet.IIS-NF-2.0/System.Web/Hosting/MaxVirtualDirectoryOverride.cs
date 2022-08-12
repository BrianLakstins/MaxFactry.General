// <copyright file="MaxVirtualDirectoryOverride.cs" company="Lakstins Family, LLC">
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
    using System.Collections;

    /// <summary>
    /// MaxFactry implementation of VirtualDirectory
    /// </summary>
    public class MaxVirtualDirectoryOverride : VirtualDirectory
	{
		/// <summary>
		/// Internal storage of currently provided path
		/// </summary>
        private string _sPath = string.Empty;

        /// <summary>
        /// Internal storage for list of children
        /// </summary>
        private ArrayList _oChildren = null;

        /// <summary>
        /// Initializes a new instance of the MaxVirtualDirectory class
        /// </summary>
        /// <param name="lsVirtualPath">Virtual path to the file.</param>
        public MaxVirtualDirectoryOverride(string lsVirtualPath)
            : base(lsVirtualPath)
		{
            this._sPath = lsVirtualPath;
		}

        /// <summary>
        /// Children of this directory
        /// </summary>
        public override IEnumerable Children
        {
            get
            {
                if (null == this._oChildren)
                {
                    this._oChildren = new ArrayList();
                    string[] laChildren = MaxVirtualPathProviderOverride.GetChildren(this._sPath);
                    foreach (string lsChild in laChildren)
                    {
                        if (lsChild.StartsWith("D"))
                        {
                            this._oChildren.Add(new MaxVirtualDirectoryOverride(lsChild.Substring(1)));
                        }
                        else if (lsChild.StartsWith("F"))
                        {
                            this._oChildren.Add(new MaxVirtualFileOverride(lsChild.Substring(1)));
                        }
                    }
                }

                return this._oChildren;
            }
        }

        /// <summary>
        /// Children of this directory that are directories
        /// </summary>
        public override IEnumerable Directories
        {
            get 
            {
                ArrayList loR = new ArrayList();
                foreach (object loValue in this.Children)
                {
                    if (loValue is VirtualDirectory)
                    {
                        loR.Add(loValue);
                    }
                }

                return loR;
            }
        }

        /// <summary>
        /// Children of this directory that are files
        /// </summary>
        public override IEnumerable Files
        {
            get
            {
                ArrayList loR = new ArrayList();
                foreach (object loValue in this.Children)
                {
                    if (loValue is VirtualFile)
                    {
                        loR.Add(loValue);
                    }
                }

                return loR;
            }
        }
	}
}