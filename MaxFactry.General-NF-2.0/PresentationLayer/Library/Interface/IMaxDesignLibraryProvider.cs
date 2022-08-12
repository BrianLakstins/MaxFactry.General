// <copyright file="IMaxDesignLibraryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="12/4/2020" author="Brian A. Lakstins" description="Add GetHtml">
// </changelog>
#endregion

namespace MaxFactry.General.PresentationLayer
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.General.BusinessLayer;

    /// <summary>
    /// Interface for MaxShortCodeLibrary
    /// </summary>
    public interface IMaxDesignLibraryProvider
    {

        /// <summary>
        /// Gets the HTML tag based on an index of attributes.
        /// </summary>
        /// <param name="loAttributeIndex">Index of attributes for the HTML tag.</param>
        /// <returns>HTML tag with attributes set based on index.</returns>
        MaxHtmlTagStructure GetMaxHtmlTag(MaxIndex loAttributeIndex);

        /// <summary>
        /// Gets an index of attributes based on the prefix.
        /// </summary>
        /// <param name="lsPrefix">Prefix to use for filter.</param>
        /// <param name="loIndex">Index of all attributes.</param>
        /// <returns>Filtered index of attributes.</returns>
        MaxIndex GetSubIndex(string lsPrefix, MaxIndex loIndex);

        /// <summary>
        /// Gets an HTML tag that can be used as a tool tip.
        /// </summary>
        /// <param name="loIndex">Index of attributes for the tool tip.</param>
        /// <returns>HTML tag structure based on the index.</returns>
        MaxHtmlTagStructure GetToolTipTag(MaxIndex loIndex);

        /// <summary>
        /// Gets an HTML tag that can be used as content for a tool tip.
        /// </summary>
        /// <param name="loIndex">Index of attributes for the tool tip content.</param>
        /// <returns>HTML tag structure based on the index.</returns>
        MaxHtmlTagStructure GetToolTipContentTag(MaxIndex loIndex);

        string GetThemeView(string lsView);

        string ConvertToInlineCSS(string lsHtml, string lsCSS);

        string RemoveComments(string lsCode);

        bool ContainsHtml(string lsHtml);

        string RemoveHtml(string lsHtml);

        MaxIndex GetListFromText(string lsCSS);

        string GetHtml(string lsView, object loModel, MaxIndex loMetaIndex);
    }
}