// <copyright file="MaxSecurityLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="9/24/2026" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
    using MaxFactry.Core;

    /// <summary>
    /// Provides static methods to handle business layer security
    /// </summary>
    public class MaxTokenLibrary : MaxSingleFactory
    {
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxTokenLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Gets the Provider used for most MaxFactory methods
        /// </summary>
        public static IMaxTokenLibraryProvider Provider
        {
            get
            {
                if (null == Instance.BaseProvider)
                {
                    Instance.SetProvider(typeof(MaxFactry.General.BusinessLayer.Provider.MaxTokenLibraryDefaultProvider));
                }

                return (IMaxTokenLibraryProvider)Instance.BaseProvider;
            }
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxTokenLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxTokenLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }

        public static MaxIndex ParseToken(string lsToken)
        {
            return Provider.ParseToken(lsToken);
        }

        public static bool IsValidToken(MaxIndex loToken)
        {
            return Provider.IsValidToken(loToken);
        }

        public static bool ValidateTokenSignature(MaxIndex loToken)
        {
            return Provider.ValidateTokenSignature(loToken);
        }

        public static string GetUserName(MaxIndex loToken)
        {
            return Provider.GetUserName(loToken);
        }

        public static string GetEmail(MaxIndex loToken)
        {
            return Provider.GetEmail(loToken);
        }
    }
}
