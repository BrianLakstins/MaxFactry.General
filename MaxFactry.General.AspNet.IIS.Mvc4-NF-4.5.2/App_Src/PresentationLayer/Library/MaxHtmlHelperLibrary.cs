// <copyright file="MaxHtmlHelperLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="12/31/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="12/20/2015" author="Brian A. Lakstins" description="Add helper for using Razor files.">
// <change date="6/19/2019" author="Brian A. Lakstins" description="Fix issue with storing views that might be for other storage keys.">
// <change date="10/23/2019" author="Brian A. Lakstins" description="Optimize process for getting content url.">
// <change date="12/4/2019" author="Brian A. Lakstins" description="Move Getting Html from a Razor view to MaxDesignLibrary">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Web;
    using MaxFactry.Core;
    using MaxFactry.General.PresentationLayer;
    using MaxFactry.General.AspNet.BusinessLayer;

    /// <summary>
    /// Helper library for producing HTML
    /// </summary>
    public static class MaxHtmlHelperLibrary
    {
        public static bool MaxIsInRole(string lsRoleList)
        {
            string[] laRoleList = lsRoleList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string lsRole in laRoleList)
            {
                if (HttpContext.Current.User.IsInRole(lsRole))
                {
                    return true;
                }
            }

            return false;
        }

        private static MaxIndex _oRazorCacheIndex = new MaxIndex();

        public static string GetHtml(string lsViewPath, object loModel, MaxIndex loMetaIndex)
        {
            return MaxDesignLibrary.GetHtml(lsViewPath, loModel, loMetaIndex);
        }

        public static string MaxFileGetUrl(string lsName)
        {
            string lsR = MaxFileUploadEntity.Create().GetContentURLCache(lsName);
            return lsR;
        }

        public static string GetRandomSelection(string[] laParams)
        {
            string lsR = string.Empty;
            int lnP = MaxEncryptionLibrary.GetRandomInt(typeof(object), 0, laParams.Length - 1);
            if (0 <= lnP && lnP < laParams.Length)
            {
                lsR = laParams[lnP];
            }

            return lsR;
        }

    }
}