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
// </changelog>
#endregion

namespace System.Web.Caching
{
    using System;
    using System.Collections.Generic;


#if net4_52
    /// <summary>
    /// MaxFactry implementation of OutputCacheProvider
    /// </summary>
    public class MaxOutputCacheProviderOverride : OutputCacheProvider
    {
        private static Dictionary<string, object> _oItemIndex = new Dictionary<string, object>();

        private static Dictionary<string, DateTime> _oExpireIndex = new Dictionary<string, DateTime>();

        private static object _oLock = new object();

        public override object Get(string lsKey)
        {
            return GetValue(lsKey);
        }

        public override object Add(string lsKey, object loItem, DateTime loExpire)
        {
            return AddValue(lsKey, loItem, loExpire);
        }

        public override void Set(string lsKey, object loItem, DateTime loExpire)
        {
            SetValue(lsKey, loItem, loExpire);
        }

        public override void Remove(string lsKey)
        {
            RemoveValue(lsKey);
        }

        public static Dictionary<string, object> GetItemIndex()
        {
            return new Dictionary<string, object>(_oItemIndex);
        }

        public static object GetValue(string lsKey)
        {
            try
            {
                if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["clearcache"]))
                {
                    RemoveValue(lsKey);
                    return null;
                }
            }
            catch { }

            try
            {
                if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["nocache"]))
                {
                    return null;
                }
            }
            catch { }

            if (_oItemIndex.ContainsKey(lsKey))
            {
                if (_oExpireIndex.ContainsKey(lsKey))
                {
                    if (_oExpireIndex[lsKey].ToUniversalTime() < DateTime.Now)
                    {
                        RemoveValue(lsKey);
                        return null;
                    }
                }

                return _oItemIndex[lsKey];
            }

            return null;
        }

        public static object AddValue(string lsKey, object loItem, DateTime loExpire)
        {
            object loCurrent = GetValue(lsKey);
            if (null != loCurrent)
            {
                if (!_oExpireIndex.ContainsKey(lsKey))
                {
                    lock (_oLock)
                    {
                        if (!_oExpireIndex.ContainsKey(lsKey))
                        {
                            _oExpireIndex[lsKey] = loExpire;
                        }
                    }
                }

                return loCurrent;
            }

            if (!_oItemIndex.ContainsKey(lsKey))
            {
                lock (_oLock)
                {
                    if (!_oItemIndex.ContainsKey(lsKey))
                    {
                        _oItemIndex.Add(lsKey, loItem);
                    }
                }
            }

            if (!_oExpireIndex.ContainsKey(lsKey))
            {
                lock (_oLock)
                {
                    if (!_oExpireIndex.ContainsKey(lsKey))
                    {
                        _oExpireIndex.Add(lsKey, loExpire);
                    }
                }
            }

            return loItem;
        }

        public static void SetValue(string lsKey, object loItem, DateTime loExpire)
        {
            RemoveValue(lsKey);
            if (!_oItemIndex.ContainsKey(lsKey))
            {
                lock (_oLock)
                {
                    if (!_oItemIndex.ContainsKey(lsKey))
                    {
                        _oItemIndex.Add(lsKey, loItem);
                    }
                }
            }

            if (!_oExpireIndex.ContainsKey(lsKey))
            {
                lock (_oLock)
                {
                    if (!_oExpireIndex.ContainsKey(lsKey))
                    {
                        _oExpireIndex.Add(lsKey, loExpire);
                    }
                }
            }
        }

        public static void RemoveValue(string lsKey)
        {
            if (_oItemIndex.ContainsKey(lsKey))
            {
                lock (_oLock)
                {
                    if (_oItemIndex.ContainsKey(lsKey))
                    {
                        if (_oExpireIndex.ContainsKey(lsKey))
                        {
                            _oExpireIndex.Remove(lsKey);
                        }

                        _oItemIndex.Remove(lsKey);
                    }
                }
            }
        }
    }
#endif
}