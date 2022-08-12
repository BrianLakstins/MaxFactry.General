// <copyright file="MaxUSStateDataModel.cs" company="Lakstins Family, LLC">
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
// <change date="11/6/2014" author="Brian A. Lakstins" description="Initial Release">
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer
{
    using System;
    using MaxFactry.Base.DataLayer;

    /// <summary>
    /// Data Model for US States.  
    /// </summary>
    public class MaxUSStateDataModel : MaxBaseIdDataModel
    {
        /// <summary>
        /// Name of the state.
        /// </summary>
        public readonly string Name = "Name";

        /// <summary>
        /// FIPS State Numeric Code.
        /// </summary>
        public readonly string Code = "Code";

        /// <summary>
        /// Two letter abbreviation for the State.
        /// </summary>
        public readonly string Abbreviation = "Abbreviation";

        /// <summary>
        /// Status of the state
        /// 0. One of the 50 States
        /// 1. Under U.S. sovereignty.
        /// 2. An aggregation of nine U.S. territories: Baker Island,Howland Island, Jarvis Island, Johnston Atoll, Kingman Reef, Midway Islands,
        /// Navassa Island. Palmyra Atoll. and Wake Island. Each territory is assigned a FIPS County Code in FIPS PUB 6-3, and may be
        /// individually identified through a combination of the FIPS State Code (74 or UM) and the appropriate FIPS County Code.
        /// 3. A Compact of Free Association with the United States of America is now in full force. It was announced by Presidential proclamation
        /// on November 3, 1986.
        /// 4. Remains a trust territory.
        /// 5. District of Columbia
        /// 6. One of the US Minor Outlying Islands
        /// </summary>
        public readonly string Status = "Status";

        /// <summary>
        /// Initializes a new instance of the MaxUSStateDataModel class
        /// </summary>
        public MaxUSStateDataModel()
        {
            this.RepositoryProviderType = typeof(MaxFactry.General.DataLayer.Provider.MaxUSStateRepositoryProvider);
            this.RepositoryType = typeof(MaxUSStateRepository);
            this.AddType(this.Name, typeof(MaxShortString));
            this.AddType(this.Code, typeof(int));
            this.AddType(this.Abbreviation, typeof(MaxShortString));
            this.AddType(this.Status, typeof(int));
        }
    }
}
