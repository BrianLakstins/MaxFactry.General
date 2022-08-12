// <copyright file="MaxEnableEmbedAttribute.cs" company="Lakstins Family, LLC">
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
// <change date="3/19/2015" author="Brian A. Lakstins" description="Initial Release">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using MaxFactry.Core;
    using MaxFactry.General.AspNet.PresentationLayer;

    /// <summary>
    /// Attribute that enables CORS for the request
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class MaxEnableEmbedAttribute : ActionFilterAttribute, IResultFilter
    {
        /// <summary>
        /// Initializes a new instance of the MaxEnableCorsAttribute class.
        /// </summary>
        public MaxEnableEmbedAttribute()
            : base()
        {
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            bool lbIsEmbed = IsEmbed;
            if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["IsEmbed"]))
            {
                bool lbIsEmbedNew = MaxConvertLibrary.ConvertToBoolean(this.GetType(), filterContext.HttpContext.Request.QueryString["IsEmbed"]);
                if (!lbIsEmbed && lbIsEmbedNew)
                {
                    if (null != filterContext.HttpContext.Request.ServerVariables["HTTP_REFERER"])
                    {
                        Uri loEmbedUrl = new Uri(filterContext.HttpContext.Request.ServerVariables["HTTP_REFERER"]);
                        if (loEmbedUrl.Host != filterContext.HttpContext.Request.Url.Host)
                        {
                            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeSession, "EmbedTARGET", loEmbedUrl.Host);
                            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProfile, "EmbedTARGET", loEmbedUrl.Host);
                            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "EmbedTARGET", loEmbedUrl.Host);
                        }
                    }
                }

                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeSession, "IsEmbed", lbIsEmbedNew);
                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProfile, "IsEmbed", lbIsEmbedNew);
                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "IsEmbed", lbIsEmbedNew);
                lbIsEmbed = lbIsEmbedNew;
            }

            if (lbIsEmbed)
            {
                ViewResult loResult = filterContext.Result as ViewResult;
                if (null != loResult)
                {
                    loResult.MasterName = MaxFactry.General.PresentationLayer.MaxDesignLibrary.GetThemeView("_LayoutEmbed");
                }
            }

            base.OnResultExecuting(filterContext);
        }

        public static bool IsEmbed
        {
            get
            {
                bool lbIsEmbed = false;

                object loIsEmbed = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, "IsEmbed");
                if (null == loIsEmbed)
                {
                    loIsEmbed = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeSession, "IsEmbed");
                    if (null == loIsEmbed)
                    {
                        loIsEmbed = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProfile, "IsEmbed");
                    }
                }

                if (null != loIsEmbed)
                {
                    lbIsEmbed = MaxConvertLibrary.ConvertToBoolean(typeof(MaxEnableEmbedAttribute), loIsEmbed);
                }

                return lbIsEmbed;
            }
        }

        public static string EmbedTARGET
        {
            get
            {
                string lsTarget = string.Empty;

                object loTarget = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeSession, "EmbedTARGET");
                if (null == loTarget)
                {
                    loTarget = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProfile, "EmbedTARGET");
                }

                if (null != loTarget)
                {
                    lsTarget = MaxConvertLibrary.ConvertToString(typeof(MaxEnableEmbedAttribute), loTarget);
                }

                return lsTarget;
            }
        }
    }
}