﻿// <copyright file="MaxResource.cs" company="Lakstins Family, LLC">
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
// <change date="11/9/2022" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS
{
    using System.Reflection;
    using System.Resources;
    using System.Collections;

    public class MaxResource
    {
        public static string GetString(string lsKey)
        {
            string lsR = string.Empty;
            Assembly loAssembly = MaxFactry.Core.MaxFactryLibrary.GetAssembly(typeof(MaxResource));
            string[] laName = loAssembly.GetManifestResourceNames();
            foreach (string lsName in laName)
            {
                ResourceSet loSet = new ResourceSet(loAssembly.GetManifestResourceStream(lsName));
                foreach (DictionaryEntry loEntry in loSet)
                {
                    string lsCurrentKey = MaxFactry.Core.MaxConvertLibrary.ConvertToString(typeof(object), loEntry.Key);
                    if (lsCurrentKey == lsKey)
                    {
                        lsR = MaxFactry.Core.MaxConvertLibrary.ConvertToString(typeof(object), loEntry.Value);
                    }
                }
            }

            return lsR;
        }
    }
}
