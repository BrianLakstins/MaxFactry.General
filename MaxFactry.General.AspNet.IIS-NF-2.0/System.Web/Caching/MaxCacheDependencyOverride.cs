// <copyright file="MaxCacheDependencyOverride.cs" company="Lakstins Family, LLC">
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
// <change date="11/8/2014" author="Brian A. Lakstins" description="Initial Release">
// </changelog>
#endregion

namespace System.Web.Caching
{
    using System;

    /// <summary>
    /// MaxFactry implementation of CacheDependency
    /// </summary>
    public class MaxCacheDependencyOverride : CacheDependency
    {
        public MaxCacheDependencyOverride()
            : base()
        {
        }

        public MaxCacheDependencyOverride(string[] filenames)
            : base(filenames)
        {
        }

        public MaxCacheDependencyOverride(string[] filenames, DateTime start)
            : base(filenames, start)
        {

        }

        public MaxCacheDependencyOverride(string[] filenames, string[] cachekeys)
            : base(filenames, cachekeys)
        {

        }

        public MaxCacheDependencyOverride(string[] filenames, string[] cachekeys, System.Web.Caching.CacheDependency dependency)
            : base(filenames, cachekeys, dependency)
        {

        }

        public MaxCacheDependencyOverride(string[] filenames, string[] cachekeys, DateTime start)
            : base(filenames, cachekeys, start)
        {

        }

        public MaxCacheDependencyOverride(string[] filenames, string[] cachekeys, System.Web.Caching.CacheDependency dependency, DateTime start)
            : base(filenames, cachekeys, dependency, start)
        {

        }

        public static MaxCacheDependencyOverride New(string[] filenames, string[] cachekeys, System.Web.Caching.CacheDependency dependency, DateTime start)
        {
            if (null != filenames && filenames.Length > 0 && start >= DateTime.UtcNow && null != cachekeys && cachekeys.Length > 0 && null != dependency)
            {
                return new MaxCacheDependencyOverride(filenames, cachekeys, dependency, start);
            }
            else if (null != filenames && filenames.Length > 0 && start >= DateTime.UtcNow && null != cachekeys && cachekeys.Length > 0)
            {
                return new MaxCacheDependencyOverride(filenames, cachekeys, start);
            }
            else if (null != filenames && filenames.Length > 0 && null != cachekeys && cachekeys.Length > 0)
            {
                return new MaxCacheDependencyOverride(filenames, cachekeys);
            }
            else if (null != filenames && filenames.Length > 0 && start >= DateTime.UtcNow)
            {
                return new MaxCacheDependencyOverride(filenames, start);
            }
            else if (null != filenames)
            {
                return new MaxCacheDependencyOverride(filenames);
            }

            return new MaxCacheDependencyOverride();
        }

        protected override void DependencyDispose()
        {
            base.DependencyDispose();
        }

        public new bool HasChanged
        {
            get
            {
                //return false;
                bool lbR = base.HasChanged;
                return lbR;
            }
        }


        public override string GetUniqueID()
        {
            return base.GetUniqueID();
        }

        public new DateTime UtcLastModified
        {
            get
            {
                DateTime lbR = base.UtcLastModified;
                return lbR;
            }
        }

        public new void SetUtcLastModified(DateTime ldDate)
        {
            base.SetUtcLastModified(ldDate);
        }

    }

}
