// <copyright file="MaxShortCodeLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="2/10/2015" author="Brian A. Lakstins" description="Initial Release">
// </changelog>
#endregion

namespace MaxFactry.General.PresentationLayer
{
    using System;
    using MaxFactry.Core;

    /// <summary>
    /// Helper library for replacing short codes with other text
    /// </summary>
    public class MaxShortCodeLibrary : MaxMultipleFactory
    {
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxShortCodeLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxShortCodeLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxShortCodeLibrary();
                            _oInstance.ProviderAdd(typeof(MaxFactry.General.PresentationLayer.Provider.MaxShortCodeLibraryDefaultProvider));
                        }
                    }
                }

                return _oInstance;
            }
        }


        public static string HandleShortCodeIndex(MaxIndex loIndex)
        {
            string lsR = null;
            IMaxProvider[] loList = Instance.GetProviderList();
            for (int lnP = 0; lnP < loList.Length; lnP++)
            {
                if (loList[lnP] is IMaxShortCodeLibraryProvider)
                {
                    string lsResult = ((IMaxShortCodeLibraryProvider)loList[lnP]).HandleShortCodeIndex(loIndex);
                    if (null != lsResult)
                    {
                        lsR += lsResult;
                    }
                }
            }

            if (null == lsR)
            {
                lsR = loIndex["SHORTCODETAG"] as string;
            }

            return lsR;
        }

        public static string HandleShortCodeContent(MaxIndex loIndex, string lsContent)
        {
            string lsR = lsContent;
            if (null != loIndex)
            {
                IMaxProvider[] loList = Instance.GetProviderList();
                for (int lnP = 0; lnP < loList.Length; lnP++)
                {
                    if (loList[lnP] is IMaxShortCodeLibraryProvider)
                    {
                        string lsResult = ((IMaxShortCodeLibraryProvider)loList[lnP]).HandleShortCodeContent(loIndex, lsR);
                        if (null != lsResult)
                        {
                            lsR = lsResult;
                        }
                    }
                }
            }

            return lsR;
        }

        /// <summary>
        /// Parses content for Short Codes and replaces them with other content
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetContentShortCode(string lsContent)
        {
            string lsR = string.Empty;
            MaxIndex loIndex = MaxConvertLibrary.ConvertToShortCodeIndex(typeof(object), lsContent);
            MaxIndex loShortCodeIndex = null;
            for (int lnK = 0; lnK < loIndex.Count; lnK++)
            {
                if (loIndex[lnK] is string)
                {
                    lsR += MaxShortCodeLibrary.HandleShortCodeContent(loShortCodeIndex, loIndex[lnK] as string);
                }
                else if (loIndex[lnK] is MaxIndex)
                {
                    loShortCodeIndex = loIndex[lnK] as MaxIndex;
                    lsR += MaxShortCodeLibrary.HandleShortCodeIndex(loShortCodeIndex);
                    string lsType = loShortCodeIndex["TYPE"] as string;
                    if (lsType.Equals("END", StringComparison.CurrentCultureIgnoreCase))
                    {
                        loShortCodeIndex = null;
                    }
                }
            }

            return lsR;
        }
    }
}