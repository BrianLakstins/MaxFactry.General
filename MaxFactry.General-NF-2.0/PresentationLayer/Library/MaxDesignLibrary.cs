// <copyright file="MaxDesignLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="12/4/2020" author="Brian A. Lakstins" description="Add ability to GetHtml from a Razor View">
// </changelog>
#endregion

namespace MaxFactry.General.PresentationLayer
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.General.BusinessLayer;

    /// <summary>
    /// Helper library for replacing short codes with other text
    /// </summary>
    public class MaxDesignLibrary : MaxSingleFactory
    {
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxDesignLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Gets the Provider used for most MaxFactory methods
        /// </summary>
        public static IMaxDesignLibraryProvider Provider
        {
            get
            {
                if (null == Instance.BaseProvider)
                {
                    Instance.SetProvider(typeof(MaxFactry.General.PresentationLayer.Provider.MaxDesignLibraryDefaultProvider));
                    if (!(Instance.BaseProvider is IMaxDesignLibraryProvider))
                    {
                        throw new MaxException("Provider for MaxDesignLibrary does not implement IMaxDesignLibraryProvider.");
                    }
                }

                return (IMaxDesignLibraryProvider)Instance.BaseProvider;
            }
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxDesignLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxDesignLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }

        public static string GetThemeView(string lsView)
        {
            return Provider.GetThemeView(lsView);
        }

        public static string GetThemeName()
        {
            string lsR = "Default";
            object loObject = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeAny, "MaxThemeName");
            if (null != loObject && !string.IsNullOrEmpty(loObject.ToString()))
            {
                lsR = loObject.ToString();
            }

            return lsR;
        }

        public static void SetThemeName(string lsName)
        {
            if (!string.IsNullOrEmpty(lsName))
            {
                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "MaxThemeName", lsName);
            }
        }

        /// <summary>
        /// Gets the HTML tag based on an index of attributes.
        /// </summary>
        /// <param name="loAttributeIndex">Index of attributes for the HTML tag.</param>
        /// <returns>HTML tag with attributes set based on index.</returns>
        public static MaxHtmlTagStructure GetMaxHtmlTag(MaxIndex loAttributeIndex)
        {
            return Provider.GetMaxHtmlTag(loAttributeIndex);
        }

        /// <summary>
        /// Gets an index of attributes based on the prefix.
        /// </summary>
        /// <param name="lsPrefix">Prefix to use for filter.</param>
        /// <param name="loIndex">Index of all attributes.</param>
        /// <returns>Filtered index of attributes.</returns>
        public static MaxIndex GetSubIndex(string lsPrefix, MaxIndex loIndex)
        {
            return Provider.GetSubIndex(lsPrefix, loIndex);
        }

        /// <summary>
        /// Gets an HTML tag that can be used as a tool tip.
        /// </summary>
        /// <param name="loIndex">Index of attributes for the tool tip.</param>
        /// <returns>HTML tag structure based on the index.</returns>
        public static MaxHtmlTagStructure GetToolTipTag(MaxIndex loIndex)
        {
            return Provider.GetToolTipTag(loIndex);
        }

        /// <summary>
        /// Gets an HTML tag that can be used as content for a tool tip.
        /// </summary>
        /// <param name="loIndex">Index of attributes for the tool tip content.</param>
        /// <returns>HTML tag structure based on the index.</returns>
        public static MaxHtmlTagStructure GetToolTipContentTag(MaxIndex loIndex)
        {
            return Provider.GetToolTipContentTag(loIndex);
        }

        public static string ConvertToInlineCSS(string lsHtml, string lsCSS)
        {
            return Provider.ConvertToInlineCSS(lsHtml, lsCSS);
        }

        public static string RemoveComments(string lsCode)
        {
            return Provider.RemoveComments(lsCode);
        }

        public static bool ContainsHtml(string lsHtml)
        {
            return Provider.ContainsHtml(lsHtml);
        }

        public static string RemoveHtml(string lsHtml)
        {
            return Provider.RemoveHtml(lsHtml);
        }

        public static MaxIndex GetListFromText(string lsCSS)
        {
            return Provider.GetListFromText(lsCSS);
        }

        public static string GetHtml(string lsView, object loModel, MaxIndex loMetaIndex)
        {
            return Provider.GetHtml(lsView, loModel, loMetaIndex);
        }
    }
}