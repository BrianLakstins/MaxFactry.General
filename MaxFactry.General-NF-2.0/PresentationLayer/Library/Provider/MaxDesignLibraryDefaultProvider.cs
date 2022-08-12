// <copyright file="MaxDesignLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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

namespace MaxFactry.General.PresentationLayer.Provider
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.General.BusinessLayer;

    /// <summary>
    /// Default provider for MaxShortCodeLibrary
    /// </summary>
    public class MaxDesignLibraryDefaultProvider : MaxProvider, IMaxDesignLibraryProvider
    {
        /// <summary>
        /// Gets the HTML tag based on an index of attributes.
        /// </summary>
        /// <param name="loAttributeIndex">Index of attributes for the HTML tag.</param>
        /// <returns>HTML tag with attributes set based on index.</returns>
        public MaxHtmlTagStructure GetMaxHtmlTag(MaxIndex loAttributeIndex)
        {
            MaxHtmlTagStructure loStructure = new MaxHtmlTagStructure();
            if (null != loAttributeIndex)
            {
                string[] laKey = loAttributeIndex.GetSortedKeyList();

                foreach (string lsKey in laKey)
                {
                    if (lsKey.Equals("tag", StringComparison.OrdinalIgnoreCase))
                    {
                        loStructure.Tag = loAttributeIndex[lsKey].ToString();
                    }
                    else if (lsKey.Equals("class", StringComparison.OrdinalIgnoreCase))
                    {
                        loStructure.Class = loAttributeIndex[lsKey].ToString();
                    }
                    else if (lsKey.Equals("content", StringComparison.OrdinalIgnoreCase))
                    {
                        loStructure.Content = loAttributeIndex[lsKey].ToString();
                    }
                    else if (lsKey.Equals("id", StringComparison.OrdinalIgnoreCase))
                    {
                        loStructure.Id = loAttributeIndex[lsKey].ToString();
                    }
                    else if (lsKey.Equals("name", StringComparison.OrdinalIgnoreCase))
                    {
                        loStructure.Name = loAttributeIndex[lsKey].ToString();
                    }
                    else if (lsKey.Equals("style", StringComparison.OrdinalIgnoreCase))
                    {
                        loStructure.Style = loAttributeIndex[lsKey].ToString();
                    }
                    else if (lsKey.Equals("value", StringComparison.OrdinalIgnoreCase))
                    {
                        loStructure.Value = loAttributeIndex[lsKey].ToString();
                    }
                    else
                    {
                        loStructure.ReplaceAttribute(lsKey.ToLower(), loAttributeIndex[lsKey].ToString());
                    }
                }
            }

            return loStructure;
        }

        /// <summary>
        /// Gets an index of attributes based on the prefix.
        /// </summary>
        /// <param name="lsPrefix">Prefix to use for filter.</param>
        /// <param name="loIndex">Index of all attributes.</param>
        /// <returns>Filtered index of attributes.</returns>
        public MaxIndex GetSubIndex(string lsPrefix, MaxIndex loIndex)
        {
            MaxIndex loR = new MaxIndex();
            string[] laKey = loIndex.GetSortedKeyList();
            foreach (string lsKey in laKey)
            {
                string lsSubKey = lsKey.ToLower();
                if (lsKey.ToLower().StartsWith(lsPrefix.ToLower() + ";"))
                {
                    lsSubKey = lsKey.Substring(lsPrefix.ToLower().Length + 1).ToLower();
                }

                if (!lsSubKey.Contains(";"))
                {
                    if (loR.Contains(lsSubKey))
                    {
                        if (lsSubKey == "class")
                        {
                            loR[lsSubKey] += " " + loIndex[lsKey];
                        }
                        else
                        {
                            loR[lsSubKey] = loIndex[lsKey];
                        }
                    }
                    else
                    {
                        loR.Add(lsSubKey, loIndex[lsKey]);
                    }
                }
            }

            return loR;
        }

        /// <summary>
        /// Gets an HTML tag that can be used as a tool tip.
        /// </summary>
        /// <param name="loIndex">Index of attributes for the tool tip.</param>
        /// <returns>HTML tag structure based on the index.</returns>
        public MaxHtmlTagStructure GetToolTipTag(MaxIndex loIndex)
        {
            MaxHtmlTagStructure loTooltipTag = new MaxHtmlTagStructure("span");
            loTooltipTag.Class = "tooltipactive btn input-group-addon";
            if (loIndex.Contains("data-placement"))
            {
                loTooltipTag.ReplaceAttribute("data-placement", loIndex["data-placement"].ToString());
            }
            else
            {
                loTooltipTag.ReplaceAttribute("data-placement", "right");
            }

            if (loIndex.Contains("data-toggle"))
            {
                loTooltipTag.ReplaceAttribute("data-toggle", loIndex["data-toggle"].ToString());
            }
            else
            {
                loTooltipTag.ReplaceAttribute("data-toggle", "tooltip");
            }

            if (loIndex.Contains("title"))
            {
                loTooltipTag.ReplaceAttribute("title", loIndex["title"].ToString());
            }
            else
            {
                loTooltipTag.ReplaceAttribute("title", "Required");
            }

            return loTooltipTag;
        }

        /// <summary>
        /// Gets an HTML tag that can be used as content for a tool tip.
        /// </summary>
        /// <param name="loIndex">Index of attributes for the tool tip content.</param>
        /// <returns>HTML tag structure based on the index.</returns>
        public MaxHtmlTagStructure GetToolTipContentTag(MaxIndex loIndex)
        {
            MaxHtmlTagStructure loContentSpan = new MaxHtmlTagStructure("span");
            if (loIndex.Contains("content"))
            {
                loContentSpan.Content = loIndex["content"].ToString();
            }
            else
            {
                if (loIndex.Contains("glyphicon"))
                {
                    loContentSpan.Class += "glyphicon glyphicon-" + loIndex["glyphicon"].ToString();
                }
                else
                {
                    loContentSpan.Class += "glyphicon glyphicon-asterisk";
                }
            }

            if (loIndex.Contains("class"))
            {
                loContentSpan.Class = loIndex["class"].ToString();
            }

            return loContentSpan;
        }


        public virtual string GetThemeView(string lsView)
        {
            throw new NotImplementedException();
        }

        public virtual string ConvertToInlineCSS(string lsHtml, string lsCSS)
        {
            throw new NotImplementedException();
        }

        public virtual string RemoveComments(string lsCode)
        {
            throw new NotImplementedException();
        }

        public virtual bool ContainsHtml(string lsHtml)
        {
            throw new NotImplementedException();
        }

        public virtual string RemoveHtml(string lsHtml)
        {
            throw new NotImplementedException();
        }

        public virtual MaxIndex GetListFromText(string lsCSS)
        {
            throw new NotImplementedException();
        }

        public virtual string GetHtml(string lsView, object loModel, MaxIndex loMetaIndex)
        {
            throw new NotImplementedException();
        }
    }
}