// <copyright file="MaxAppLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="6/21/2015" author="Brian A. Lakstins" description="Initial Release">
// <change date="5/22/2020" author="Brian A. Lakstins" description="Add cookie and querystring names">
// <change date="11/17/2020" author="Brian A. Lakstins" description="Add sign in and sign out methods">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS
{
    using System;
    using System.Web;
    using MaxFactry.Core;
    using MaxFactry.Base.DataLayer;

    /// <summary>
    /// Provider for conversion specifically for dates.
    /// </summary>
    public class MaxAppLibrary : MaxFactry.General.AspNet.MaxAppLibrary
    {
        public const string MaxStorageKeyCookieName = "MaxKey20150622";

        public const string MaxStorageKeyQueryName = "msk";

        /// <summary>
        /// Gets the Provider used for most MaxFactory methods
        /// </summary>
        public new static IMaxAppLibraryProvider Provider
        {
            get
            {
                return MaxFactry.General.AspNet.MaxAppLibrary.Provider as IMaxAppLibraryProvider;
            }
        }

        /// <summary>
        /// Signs the specified username in
        /// </summary>
        /// <param name="lsUsername">Username to sign in.</param>
        public static void SignIn(string lsUsername)
        {
            Provider.SignIn(lsUsername);
        }

        /// <summary>
        /// Signs a user out.
        /// </summary>
        public static void SignOut()
        {
            Provider.SignOut();
        }
    }
}