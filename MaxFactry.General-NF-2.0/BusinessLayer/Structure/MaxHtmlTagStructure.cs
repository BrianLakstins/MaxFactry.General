// <copyright file="MaxHtmlTagStructure.cs" company="Lakstins Family, LLC">
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
// <change date="6/4/2015" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
    using MaxFactry.Core;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// Structure of a client file associated with a tool registration information.
    /// </summary>
    public class MaxHtmlTagStructure
    {
        /// <summary>
        /// List of attributes for the tag
        /// </summary>
        private MaxIndex _oAttributeIndex = new MaxIndex();

        /// <summary>
        /// Tags that are self closing.
        /// </summary>
        private static string _sSelfClosingTagList = "<area /><base /><br /><col /><command /><embed /><hr /><img /><input /><keygen /><link /><meta /><param /><source /><track /><wbr />";

        /// <summary>
        /// Initializes a new instance of the MaxHtmlTagStructure class
        /// </summary>
        public MaxHtmlTagStructure()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxHtmlTagStructure class
        /// </summary>
        /// <param name="lsTag">HTML tag for this structure.</param>
        public MaxHtmlTagStructure(string lsTag)
        {
            this.Tag = lsTag;
        }

        /// <summary>
        /// Gets or sets the Tag name
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the Id attribute of the tag
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name attribute of the tag
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the class attribute of the tag
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets the value attribute of the tag
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the style attribute of the tag
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Gets or sets the Content of the tag
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Adds an attribute for the tag
        /// </summary>
        /// <param name="lsName">Name of the attribute</param>
        /// <param name="lsValue">Value for the attribute</param>
        public void AddToAttribute(string lsName, string lsValue)
        {
            if (this._oAttributeIndex.Contains(lsName))
            {
                this._oAttributeIndex[lsName] = this._oAttributeIndex[lsName] + " " + lsValue;
            }
            else
            {
                this.ReplaceAttribute(lsName, lsValue);
            }
        }

        /// <summary>
        /// Adds an attribute for the tag
        /// </summary>
        /// <param name="lsName">Name of the attribute</param>
        /// <param name="lsValue">Value for the attribute</param>
        public void ReplaceAttribute(string lsName, string lsValue)
        {
            this._oAttributeIndex.Add(lsName, lsValue);
        }

        /// <summary>
        /// Gets the index of attributes added or updated
        /// </summary>
        /// <returns>Index of attributes</returns>
        public MaxIndex GetAttributeIndex()
        {
            return this._oAttributeIndex;
        }

        /// <summary>
        /// Gets the HTML Tab based on the structure.
        /// </summary>
        /// <param name="loStructure">The HTML tag structure.</param>
        /// <returns>String containing the HMTL tag.</returns>
        public override string ToString()
        {
            StringBuilder loTag = new StringBuilder("<");
            loTag.Append(Tag.ToLower());
            if (!string.IsNullOrEmpty(this.Class))
            {
                loTag.AppendFormat(" class=\"{0}\"", Class);
            }

            if (!string.IsNullOrEmpty(this.Id))
            {
                loTag.AppendFormat(" id=\"{0}\"", Id);
            }

            if (!string.IsNullOrEmpty(Name))
            {
                loTag.AppendFormat(" name=\"{0}\"", Name);
            }

            if (!string.IsNullOrEmpty(Style))
            {
                loTag.AppendFormat(" style=\"{0}\"", Style);
            }

            if (!string.IsNullOrEmpty(Value))
            {
                loTag.AppendFormat(" value=\"{0}\"", Value);
            }

            MaxIndex loIndex = GetAttributeIndex();
            string[] laKey = loIndex.GetSortedKeyList();
            for (int lnK = 0; lnK < laKey.Length; lnK++)
            {
                loTag.AppendFormat(" {0}=\"{1}\"", laKey[lnK], loIndex[laKey[lnK]]);
            }

            if (_sSelfClosingTagList.Contains("<" + Tag.ToLower() + " />"))
            {
                loTag.Append(" />");
            }
            else
            {
                loTag.AppendFormat(">{0}</{1}>", Content, Tag.ToLower());
            }

            return loTag.ToString();
        }

        public static MaxIndex GetListFromText(string lsCSS)
        {
            MaxIndex loList = new MaxIndex();
            string[] laStyleLine = lsCSS.Split('}'); ;
            foreach (string lsStyle in laStyleLine)
            {
                string lsStyleValue = RemoveComments(lsStyle).Replace("\r", string.Empty).Replace("\n", string.Empty);

                if (lsStyleValue.IndexOf('{') > -1)
                {
                    loList.Add(new MaxCSSStyleStructure(lsStyleValue));
                }
            }

            return loList;
        }

        public static string RemoveComments(string lsCode)
        {
            Regex loRegex = new Regex(@"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/");
            return loRegex.Replace(lsCode, string.Empty);
        }

        public static bool ContainsHtml(string lsHtml)
        {
            if (lsHtml.Contains("<"))
            {
                if (lsHtml.Substring(lsHtml.IndexOf("<")).Contains(">"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}