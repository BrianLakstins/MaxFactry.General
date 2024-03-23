// <copyright file="MaxUSStateRepository.cs" company="Lakstins Family, LLC">
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
// <change date="12/18/2014" author="Brian A. Lakstins" description="Updated Provider and DataModel access pattern.">
// <change date="3/20/2024" author="Brian A. Lakstins" description="Happy birthday to my mom.  Sara Jean Lakstins (Cartwright) - 3/20/1944 to 3/14/2019.">
// <change date="3/23/2024" author="Brian A. Lakstins" description="Change parent classs.  Update for changes to parent class.">
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.Base.DataLayer;

    /// <summary>
    /// Repository for managing US State data storage.
    /// </summary>
    public class MaxUSStateRepository : MaxBaseReadRepository
    {
        /// <summary>
        /// Selects data using a provider
        /// </summary>
        /// <param name="loData">Data used to determine the provider.</param>
        /// <param name="laStatus">Array with list of status numbers.</param>
        /// <returns>List of data from select</returns>
        public static MaxDataList SelectAllByStatus(MaxData loData, int[] laStatus)
        {
            MaxUSStateDataModel loDataModel = loData.DataModel as MaxUSStateDataModel;
            if (null == loDataModel)
            {
                throw new MaxException("Error casting [" + loData.DataModel.GetType() + "] for DataModel");
            }

            IMaxBaseReadRepositoryProvider loRepositoryProvider = Instance.GetRepositoryProvider(loData);
            IMaxUSStateRepositoryProvider loProvider = loRepositoryProvider as IMaxUSStateRepositoryProvider;
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loRepositoryProvider.GetType() + "] for Provider");
            }

            MaxDataList loDataList = loProvider.SelectAllByStatus(loDataModel, laStatus);
            return loDataList;
        }
    }
}
