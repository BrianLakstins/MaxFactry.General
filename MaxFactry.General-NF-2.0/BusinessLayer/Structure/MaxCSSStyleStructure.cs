// <copyright file="SkinTag.cs" company="Lakstins Family, LLC">
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
    using System;
    using System.Collections.Generic;
    using MaxFactry.Core;

    /// <summary>
    /// Single element for a list of extended data
    /// </summary>
    public class MaxCSSStyleStructure
    {
        private string _sName = string.Empty;
        private MaxIndex _oAttributeIndex = new MaxIndex();

        public string Name
        {
            get { return this._sName; }
            set { this._sName = value; }
        }

        public MaxIndex AttributeIndex
        {
            get { return this._oAttributeIndex; }
            set { this._oAttributeIndex = value; }
        }

        public override string ToString()
        {
            String lsR = string.Empty;
            string[] laKey = this.AttributeIndex.GetSortedKeyList();
            foreach (string lsKey in laKey)
            {
                String lsAttributeText = String.Format("{0}:{1};", lsKey, this.AttributeIndex[lsKey]);
                lsR += lsAttributeText;
            }

            return lsR;
        }

        public MaxCSSStyleStructure(string lsStyleValue)
        {
            //// Separate the name from the values
            string[] laStyleValue = lsStyleValue.Split('{');
            //// Set the name
            this.Name = laStyleValue[0].Trim();

            //// Get the attributes associated with this name
            string[] laAttributes = laStyleValue[1].Replace("}", string.Empty).Split(';');
            foreach (string lsAttribute in laAttributes)
            {
                if (lsAttribute.Contains(":"))
                {
                    string lsKey = lsAttribute.Split(':')[0].Trim().ToLower();
                    if (this.AttributeIndex.Contains(lsKey))
                    {
                        this.AttributeIndex.Remove(lsKey);
                    }

                    this.AttributeIndex.Add(lsKey, lsAttribute.Split(':')[1].Trim().ToLower());
                }
            }
        }
    }
}
