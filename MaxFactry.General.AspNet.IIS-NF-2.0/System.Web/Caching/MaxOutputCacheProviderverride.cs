// <copyright file="MaxOutputCacheProviderverride.cs" company="Lakstins Family, LLC">
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
// <change date="9/30/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="6/16/2025" author="Brian A. Lakstins" description="Add using this MaxCacheRepository">
// <change date="6/17/2025" author="Brian A. Lakstins" description="Add logging">
// </changelog>
#endregion

namespace System.Web.Caching
{
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.Core;
    using System;


#if net4_52
    /// <summary>
    /// MaxFactry implementation of OutputCacheProvider
    /// </summary>
    public class MaxOutputCacheProviderOverride : OutputCacheProvider
    {
        private string GetKey(string lsKey)
        {
            string lsR = this.GetType().ToString() + "/" + MaxDataLibrary.GetApplicationKey() + "/" + lsKey;
            return lsR;
        }

        public override object Get(string lsKey)
        {
            string lsCacheKey = this.GetKey(lsKey);
            try
            {
                if (null != HttpContext.Current && null != HttpContext.Current.Request && null != HttpContext.Current.Request.QueryString &&
                    !String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["clearcache"]))
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "Get", MaxEnumGroup.LogAlert, "Get() called with clearcache query string parameter.  Removing content for key: {0}", lsCacheKey));
                    this.Remove(lsKey);
                    return null;
                }
            }
            catch { }

            try
            {
                if (null != HttpContext.Current && null != HttpContext.Current.Request && null != HttpContext.Current.Request.QueryString &&
                    !String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["nocache"]))
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "Get", MaxEnumGroup.LogAlert, "Get() called with nocache query string parameter.  Returning null for key: {0}", lsCacheKey));
                    return null;
                }
            }
            catch { }

            object loR = MaxCacheRepository.Get(this.GetType(), lsCacheKey, typeof(object));
            return loR;
        }

        public override object Add(string lsKey, object loItem, DateTime ldExpire)
        {
            string lsCacheKey = this.GetKey(lsKey);
            MaxCacheRepository.Set(this.GetType(), lsCacheKey, loItem, ldExpire);
            return loItem;
        }

        public override void Set(string lsKey, object loItem, DateTime ldExpire)
        {
            string lsCacheKey = this.GetKey(lsKey);
            MaxCacheRepository.Set(this.GetType(), lsCacheKey, loItem, ldExpire);
        }

        public override void Remove(string lsKey)
        {
            string lsCacheKey = this.GetKey(lsKey);
            MaxCacheRepository.Remove(this.GetType(), lsCacheKey);
        }
    }
#endif
}