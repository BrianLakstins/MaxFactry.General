// <copyright file="MaxSecurityManageController.cs" company="Lakstins Family, LLC">
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
// <change date="11/3/2020" author="Brian A. Lakstins" description="Initial Release">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{

    using System;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.General.PresentationLayer;
    using MaxFactry.General.AspNet.BusinessLayer;
    using MaxFactry.General.AspNet.PresentationLayer;
    using MaxFactry.General.AspNet.IIS.PresentationLayer;
    using MaxFactry.Core;
    using System.Web.Security;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    [MaxAuthorize(Roles = "Admin,Admin - App")]
    public class MaxSecurityManageController : MaxManageController
    {
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public virtual ActionResult UserAuth(string m)
        {
            return this.Show(new MaxUserAuthViewModel(), m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult UserAuth(MaxUserAuthViewModel loModel, string uoProcess)
        {
            string lsCancelAction = "UserAuth";
            string lsSuccessAction = "UserAuth";
            string lsSuccessMessage = loModel.Name + " successfully created.";
            loModel.UserKey = MaxConvertLibrary.ConvertToString(typeof(object), Membership.GetUser().ProviderUserKey);
            object loResult = this.Create(loModel, uoProcess, lsCancelAction, lsSuccessAction, lsSuccessMessage);
            if (loResult is ActionResult)
            {
                return (ActionResult)loResult;
            }

            return View(loModel);
        }

        [HttpGet]
        public virtual ActionResult UserAuthEdit(string id)
        {
            MaxUserAuthViewModel loModel = new MaxUserAuthViewModel(id);
            return View(loModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult UserAuthEdit(MaxUserAuthViewModel loModel, string uoProcess)
        {
            string lsCancelAction = "UserAuth";
            ActionResult loResult = this.Edit(loModel, uoProcess, lsCancelAction);
            if (loResult is ViewResult)
            {
                ViewBag.Message = "Successfully saved.";
            }

            return loResult;
        }
    }
}
