// <copyright file="MaxRequestLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="5/18/2015" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using MaxFactry.Core;

    /// <summary>
    /// Library used to wrap methods that interact with the HttpRequestBase.
    /// </summary>
    public class MaxModelBinderForMaxIndex : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            MaxIndex loIndex = bindingContext.Model as MaxIndex;
            if (null == loIndex)
            {
                loIndex = new MaxIndex();
            }

            if (bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName))
            {
                List<string> loList = new List<string>();
                foreach (string lsKey in controllerContext.HttpContext.Request.Form.AllKeys)
                {
                    if (lsKey.StartsWith(bindingContext.ModelName))
                    {
                        string lsIndexKey = lsKey.Substring(bindingContext.ModelName.Length);
                        if (lsIndexKey.StartsWith("[") && lsIndexKey.EndsWith("]"))
                        {
                            ValueProviderResult loResult = bindingContext.ValueProvider.GetValue(lsKey);
                            loIndex.Add(lsIndexKey.Substring(1, lsIndexKey.Length - 2), loResult.AttemptedValue);
                        }
                    }

                }
            }

            return loIndex;
        }
    }

}