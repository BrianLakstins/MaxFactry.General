// <copyright file="MaxShortCodeLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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

namespace MaxFactry.General.PresentationLayer.Provider
{
    using System;
    using MaxFactry.Core;

    /// <summary>
    /// Default provider for MaxShortCodeLibrary
    /// </summary>
    public class MaxShortCodeLibraryDefaultProvider : MaxProvider, IMaxShortCodeLibraryProvider
    {
        public string HandleShortCodeIndex(MaxIndex loIndex)
        {
            string lsType = loIndex["TYPE"] as string;
            string lsShortCode = loIndex["SHORTCODENAME"] as string;
            string lsShortCodeTag = loIndex["SHORTCODETAG"] as string;
            string lsR = string.Empty;

            if (lsShortCode.Equals("row"))
            {
                if (lsType.Equals("start", StringComparison.CurrentCultureIgnoreCase))
                {
                    lsR += "<div class='row'>";
                }
                else
                {
                    lsR += "</div>";
                }
            }
            else if (lsShortCode.Equals("one_half_column"))
            {
                if (lsType.Equals("start", StringComparison.CurrentCultureIgnoreCase))
                {
                    lsR += "<div class='col-sm-6'>";
                }
                else
                {
                    lsR += "</div>";
                }
            }
            else if (lsShortCode.Equals("table"))
            {
                lsR = string.Empty;
            }

            return lsR;
        }

        public string HandleShortCodeContent(MaxIndex loIndex, string lsContent)
        {
            string lsR = null;
            string lsType = loIndex["TYPE"] as string;
            string lsShortCode = loIndex["SHORTCODENAME"] as string;
            string lsShortCodeTag = loIndex["SHORTCODETAG"] as string;

            if (lsShortCode.Equals("table", StringComparison.CurrentCultureIgnoreCase) && lsType.Equals("start", StringComparison.CurrentCultureIgnoreCase))
            {
                string lsTableType = "table-striped";
                if (null != loIndex["type"])
                {
                    lsTableType = loIndex["type"] as string;
                }

                lsR = lsContent.Replace("<table", "<table class='table " + lsTableType + "'");
            }
            else
            {
                lsR = lsContent;
            }

            return lsR;
        }
    }
}