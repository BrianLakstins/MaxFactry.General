// <copyright file="eFSAppPlatformController.cs" company="eFactory Solutions LLC">
// Copyright (c) eFactory Solutions LLC (http://efactorysolutions.com/)
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
// Portions Copyright (c) eFactory Solutions LLC (http://efactorysolutions.com/)
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
// <change date="7/19/2016" author="Brian A. Lakstins" description="Add user to custom parameters for caching.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{

    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;

    public class MaxPartialController : MaxBaseController
    {
        [ChildActionOnly]
        public virtual ActionResult PartialThemeHeader()
        {
            string lsView = MaxFactry.General.PresentationLayer.MaxDesignLibrary.GetThemeView("_PartialHeader");
            return PartialView(lsView.Replace("~/Views/MaxPartial/", string.Empty).Replace(".cshtml", string.Empty));
        }

        [ChildActionOnly]
        public virtual ActionResult PartialThemeFooter()
        {
            string lsView = MaxFactry.General.PresentationLayer.MaxDesignLibrary.GetThemeView("_PartialFooter");
            return PartialView(lsView.Replace("~/Views/MaxPartial/", string.Empty).Replace(".cshtml", string.Empty));
        }

        [ChildActionOnly]
        public virtual ActionResult PartialThemeManageHeader()
        {
            string lsView = MaxFactry.General.PresentationLayer.MaxDesignLibrary.GetThemeView("_PartialManageHeader");
            return PartialView(lsView.Replace("~/Views/MaxPartial/", string.Empty).Replace(".cshtml", string.Empty));
        }

        [ChildActionOnly]
        public virtual ActionResult PartialThemeManageFooter()
        {
            string lsView = MaxFactry.General.PresentationLayer.MaxDesignLibrary.GetThemeView("_PartialManageFooter");
            return PartialView(lsView.Replace("~/Views/MaxPartial/", string.Empty).Replace(".cshtml", string.Empty));
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600, VaryByCustom = "url;user")]
        public virtual ActionResult PartialManageHead()
        {
            return PartialView("Content/_PartialManageHead");
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600, VaryByCustom = "url")]
        public virtual ActionResult PartialHead()
        {
            return PartialView("Content/_PartialHead");
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600, VaryByCustom = "url")]
        public virtual ActionResult PartialManageHeadTitle()
        {
            return PartialView("Content/_PartialManageHeadTitle");
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600, VaryByCustom = "url")]
        public virtual ActionResult PartialHeadTitle()
        {
            return PartialView("Content/_PartialHeadTitle");
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600)]
        public virtual ActionResult PartialAddress()
        {
            return PartialView("Content/_PartialAddress");
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600, VaryByCustom = "url")]
        public virtual ActionResult PartialNavTitle()
        {
            return PartialView("Content/_PartialNavTitle");
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600, VaryByCustom = "url")]
        public virtual ActionResult PartialManageNavSectionLinkList()
        {
            return PartialView("Content/_PartialManageNavSectionLinkList");
        }

        [ChildActionOnly]
        [OutputCache(Duration = 600, VaryByCustom = "url;user")]
        public virtual ActionResult PartialNavLinkList()
        {
            return PartialView("Content/_PartialNavLinkList");
        }

        [ChildActionOnly]
        public virtual ActionResult LayoutEmailLitmusPartialHeader()
        {
            string lsView = MaxFactry.General.PresentationLayer.MaxDesignLibrary.GetThemeView("_LayoutEmailLitmusPartialHeader");
            return PartialView(lsView.Replace("~/Views/MaxPartial/", string.Empty).Replace(".cshtml", string.Empty));
        }

        [ChildActionOnly]
        public virtual ActionResult LayoutEmailLitmusPartialFooter()
        {
            string lsView = MaxFactry.General.PresentationLayer.MaxDesignLibrary.GetThemeView("_LayoutEmailLitmusPartialFooter");
            return PartialView(lsView.Replace("~/Views/MaxPartial/", string.Empty).Replace(".cshtml", string.Empty));
        }

        [ChildActionOnly]
        public virtual ActionResult LayoutEmailSimplePartialFooter()
        {
            string lsView = MaxFactry.General.PresentationLayer.MaxDesignLibrary.GetThemeView("_LayoutEmailSimplePartialFooter");
            return PartialView(lsView.Replace("~/Views/MaxPartial/", string.Empty).Replace(".cshtml", string.Empty));
        }
    }
}
