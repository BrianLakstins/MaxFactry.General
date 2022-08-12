// <copyright file="MaxAppLibraryNet40Provider.cs" company="Lakstins Family, LLC">
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
// <change date="6/21/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="11/17/2020" author="Brian A. Lakstins" description="Add sign in and sign out methods">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Provider
{
    using System;
    using System.Web;
    using System.Web.Security;
    using System.Diagnostics;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;

    /// <summary>
    /// Default provider for MaxAppLibrary
    /// </summary>
    public class MaxAppLibraryDefaultProvider : MaxFactry.General.AspNet.Provider.MaxAppLibraryDefaultProvider, IMaxAppLibraryProvider
    {
        public override void Initialize(string lsName, MaxIndex loConfig)
        {
            base.Initialize(lsName, loConfig);
        }

        public override void RegisterProviders()
        {
            base.RegisterProviders();
            //MaxAspNetLibrary.Instance.ProviderSet(typeof(MaxFactry.App.Base.AspNet.Provider.MaxAspNetLibraryIISProvider));
            //// Configure provider for MaxConfigurationLibrary
            MaxConfigurationLibrary.Instance.ProviderSet(
                typeof(MaxFactry.Core.Provider.MaxConfigurationLibraryAspNetIISProvider));

            //// Override user entity with MembershipUserEntity.
            MaxBusinessLibrary.RegisterEntityProvider(
                typeof(MaxFactry.General.BusinessLayer.MaxUserEntity),
                typeof(MaxFactry.General.BusinessLayer.MaxUserMembershipEntity));

            MaxFactry.General.AspNet.IIS.MaxStartup.Instance.RegisterProviders();
        }

        public override void SetProviderConfiguration(MaxIndex loConfig)
        {
            base.SetProviderConfiguration(loConfig);
            MaxFactry.General.AspNet.IIS.MaxStartup.Instance.SetProviderConfiguration(loConfig);
        }
        
        public override void ApplicationStartup()
        {
            base.ApplicationStartup();
            MaxFactry.General.AspNet.IIS.MaxStartup.Instance.ApplicationStartup();
        }

        /// <summary>
        /// Signs the specified username in
        /// </summary>
        /// <param name="lsUsername">Username to sign in.</param>
        public virtual void SignIn(string lsUsername)
        {
            FormsAuthentication.SetAuthCookie(lsUsername, false);
        }

        /// <summary>
        /// Signs a user out.
        /// </summary>
        public virtual void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}