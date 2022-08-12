// <copyright file="MaxHtmlHelperLibraryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="4/17/2019" author="Brian A. Lakstins" description="Start adding short codes that match HTML elements with '-maxfile' appended to them">
// </changelog>
#endregion

namespace MaxFactry.Module.Core.PresentationLayer.Provider
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer;

    /// <summary>
    /// Helper library for producing HTML
    /// </summary>
    public class MaxShortCodeLibraryFileProvider : MaxProvider, MaxFactry.General.PresentationLayer.IMaxShortCodeLibraryProvider
    {
        public string HandleShortCodeIndex(MaxIndex loIndex)
        {
            string lsType = loIndex["TYPE"] as string;
            string lsShortCode = loIndex["SHORTCODENAME"] as string;
            string lsShortCodeTag = loIndex["SHORTCODETAG"] as string;
            string lsR = string.Empty;

            if (lsShortCode.Equals("maxfile"))
            {
                if (null != loIndex["name"])
                {
                    lsR = MaxHtmlHelperLibrary.MaxFileGetUrl(loIndex["name"] as string);
                }
            }

            if (lsShortCode.Equals("maxfileimg") || lsShortCode.Equals("img-maxfile"))
            {
                if (null != loIndex["src"])
                {
                    lsR = "<img src='" + MaxHtmlHelperLibrary.MaxFileGetUrl(loIndex["src"] as string) + "'";
                    string[] laKey = loIndex.GetSortedKeyList();
                    foreach (string lsKey in laKey)
                    {
                        if (!lsKey.Equals("src") && lsKey.ToLower() == lsKey)
                        {
                            lsR += " " + lsKey + "='" + loIndex[lsKey].ToString() + "'";
                        }
                    }

                    lsR += " />";
                }
            }

            if (lsShortCode.Equals("a-maxfile"))
            {
                if (null != loIndex["href"])
                {
                    lsR = "<a href='" + MaxHtmlHelperLibrary.MaxFileGetUrl(loIndex["href"] as string) + "'";
                    string[] laKey = loIndex.GetSortedKeyList();
                    foreach (string lsKey in laKey)
                    {
                        if (!lsKey.Equals("href") && lsKey.ToLower() == lsKey)
                        {
                            lsR += " " + lsKey + "='" + loIndex[lsKey].ToString() + "'";
                        }
                    }

                    lsR += ">";
                }
                else
                {
                    lsR = "</a>";
                }
            }

            return lsR;
        }

        public string HandleShortCodeContent(MaxIndex loIndex, string lsContent)
        {
            string lsR = null;
            return lsR;
        }
    }
}