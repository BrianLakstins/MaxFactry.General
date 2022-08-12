// <copyright file="MaxUserEntity.cs" company="Lakstins Family, LLC">
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
// <change date="6/6/2015" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
    using System;
    using System.Web;
    using System.Web.Security;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.General.DataLayer;

    /// <summary>
    /// Entity used to manage information about users for the MaxSecurityProvider.
    /// </summary>
    public class MaxUserMembershipEntity : MaxFactry.General.BusinessLayer.MaxUserEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxUserEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxUserMembershipEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxUserEntity class
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxUserMembershipEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        public override bool LoadCurrent()
        {
            MembershipUser loUser = Membership.GetUser();
            if (null != loUser && null != loUser.ProviderUserKey)
            {
                Guid loId = MaxConvertLibrary.ConvertToGuid(typeof(object), loUser.ProviderUserKey);
                if (this.LoadByIdCache(loId))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
