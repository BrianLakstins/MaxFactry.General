﻿// <copyright file="IMaxUSStateRepositoryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="8/1/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="3/20/2024" author="Brian A. Lakstins" description="Happy birthday to my mom.  Sara Jean Lakstins (Cartwright) - 3/20/1944 to 3/14/2019.">
// <change date="3/23/2024" author="Brian A. Lakstins" description="Change parent classs.  Update for changes to parent class.">
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer
{
    using System;
    using MaxFactry.Base.DataLayer;

    /// <summary>
    /// Interface for MaxUSStateRepository
    /// </summary>
    public interface IMaxUSStateRepositoryProvider : IMaxBaseReadRepositoryProvider
    {
        /// <summary>
        /// Selects data using a provider
        /// </summary>
        /// <param name="loDataModel">Data model used to determine the structure.</param>
        /// <param name="laStatus">Array with list of status numbers.</param>
        /// <returns>List of data from select</returns>
        MaxDataList SelectAllByStatus(MaxUSStateDataModel loDataModel, int[] laStatus);   
    }
}
