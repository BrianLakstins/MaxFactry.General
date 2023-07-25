// <copyright file="MaxWebModule.cs" company="Lakstins Family, LLC">
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
// <change date="6/3/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="6/20/2014" author="Brian A. Lakstins" description="Moved encryption provider to MaxFactry_System.Web namespace.">
// <change date="6/23/2014" author="Brian A. Lakstins" description="Updates for testing.">
// <change date="6/27/2014" author="Brian A. Lakstins" description="Remove dependency on AppId.">
// <change date="12/1/2020" author="Brian A. Lakstins" description="Add filter to log any exceptions in web api">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4
{
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Core;
    using MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer;

    public class MaxStartup : MaxFactry.Base.MaxStartup
    {
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static object _oInstance = null;

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public new static MaxStartup Instance
        {
            get
            {
                _oInstance = CreateInstance(typeof(MaxStartup), _oInstance);
                return _oInstance as MaxStartup;
            }
        }

        public override void SetProviderConfiguration(MaxIndex loConfig)
        {
        }

        public override void RegisterProviders()
        {
            MaxFactry.General.PresentationLayer.MaxDesignLibrary.Instance.ProviderSet(typeof(MaxFactry.General.PresentationLayer.Provider.MaxDesignLibraryASPNetIISMvc4Provider));
            MaxFactry.General.PresentationLayer.MaxShortCodeLibrary.Instance.ProviderAdd(
                typeof(MaxFactry.Module.Core.PresentationLayer.Provider.MaxShortCodeLibraryFileProvider));
        }

        public override void ApplicationStartup()
        {
            GlobalConfiguration.Configuration.Filters.Add(new System.Web.Http.AuthorizeAttribute());
            GlobalConfiguration.Configuration.Filters.Add(new MaxExceptionFilterAttribute());

            RouteTable.Routes.MapHttpRoute(
                name: "MaxGeneralApiRoute",
                routeTemplate: "MaxGeneralApi/{action}/{lsId}",
                defaults: new { controller = "MaxGeneralApi", action = "index", lsId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
               name: "MaxSecurityRoute",
               url: "MaxSecurity/{action}/{lsUserId}",
               defaults: new { controller = "MaxSecurity", action = "Index", lsUserId = UrlParameter.Optional }
           );

            RouteTable.Routes.MapRoute(
               name: "MaxSecurityManageRoute",
               url: "MaxSecurityManage/{action}/{lsUserId}",
               defaults: new { controller = "MaxSecurityManage", action = "Index", lsUserId = UrlParameter.Optional }
           );

            RouteTable.Routes.MapRoute(
                name: "MaxSecurityPartialRoute",
                url: "MaxSecurityPartial/{action}",
                defaults: new { controller = "MaxSecurityPartial", action = "Index" }
            );

            RouteTable.Routes.MapRoute(
                name: "MaxSecurityManagePartialRoute",
                url: "MaxSecurityManagePartial/{action}",
                defaults: new { controller = "MaxSecurityManagePartial", action = "Index" }
            );

            RouteTable.Routes.MapRoute(
                name: "MaxFileManageRoute",
                url: "MaxFileManage/{action}/{id}",
                defaults: new { controller = "MaxFileManage", action = "Index", id = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                name: "MaxFileManagePartialRoute",
                url: "MaxFileManagePartial/{action}/{id}",
                defaults: new { controller = "MaxFileManagePartial", action = "Index", id = UrlParameter.Optional }
            );

            RouteTable.Routes.MapHttpRoute(
                name: "MaxSecurityApiRoute",
                routeTemplate: "MaxSecurityApi/{action}",
                defaults: new { controller = "MaxSecurityApi", action = "index" }
            ).RouteHandler = new MaxSessionRouteHandler();

            RouteTable.Routes.Add("MaxFileRoute",
                 new Route("f/{MaxFileId}", new MaxFileHandler()));

            RouteTable.Routes.Add("MaxFileNameRoute",
                 new Route("f/{MaxFileId}/{filename}", new MaxFileHandler()));

            RouteTable.Routes.Add("MaxStyleRoute",
                 new Route("css/{filename}", new MaxStyleHandler()));

            RouteTable.Routes.Add("MaxScriptRoute",
                 new Route("js/{filename}", new MaxScriptHandler()));
        }
    }
}