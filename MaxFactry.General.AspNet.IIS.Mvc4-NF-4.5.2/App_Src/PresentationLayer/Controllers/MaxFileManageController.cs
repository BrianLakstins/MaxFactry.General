// <copyright file="MaxFileController.cs" company="Lakstins Family, LLC">
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
// <change date="7/1/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="7/3/2014" author="Brian A. Lakstins" description="Updated to use core stream functionality.">
// <change date="8/13/2014" author="Brian A. Lakstins" description="Updated to follow management pattern.">
// <change date="7/15/2016" author="Brian A. Lakstins" description="Updated for changes to core.">
// <change date="9/4/2020" author="Brian A. Lakstins" description="Add client tool management">
// <change date="9/14/2020" author="Brian A. Lakstins" description="Add file for user list">
// <change date="9/14/2020" author="Brian A. Lakstins" description="Add file test and import processes for user list">
// </changelog>
#endregion

namespace MaxFactry.Module.File.Mvc4.PresentationLayer
{

    using System;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.General.AspNet.BusinessLayer;
    using MaxFactry.General.AspNet.PresentationLayer;
    using MaxFactry.General.AspNet.IIS.PresentationLayer;
    using MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer;
    using MaxFactry.Core;
    using System.Web.Security;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    [MaxAuthorize(Roles = "Admin,Admin - App")]
    public class MaxFileManageController : MaxManageController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult File()
        {
            MaxFileUploadViewModel loModel = new MaxFileUploadViewModel();
            return View(loModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult File(MaxFileUploadViewModel loModel, string uoProcess, HttpPostedFileBase[] laFile)
        {
            string lsCancelAction = "File";
            string lsSuccessAction = "File";
            string lsSuccessMessage = "Successfully uploaded.";
            object loResult = this.Create(loModel, uoProcess, lsCancelAction, lsSuccessAction, lsSuccessMessage, laFile);

            if (loResult is ActionResult)
            {
                return (ActionResult)loResult;
            }

            return View(loModel);
        }

        [HttpGet]
        public virtual ActionResult FileEdit(string id)
        {
            MaxFileUploadViewModel loModel = new MaxFileUploadViewModel(id);
            return View(loModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult FileEdit(MaxFileUploadViewModel loModel, string uoProcess)
        {
            string lsCancelAction = "File";
            ActionResult loResult = this.Edit(loModel, uoProcess, lsCancelAction);
            if (loResult is ViewResult)
            {
                ViewBag.Message = "Successfully saved.";
            }

            return loResult;
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult FileDownload(string id, string lsName)
        {
            MaxFileUploadViewModel loModel = new MaxFileUploadViewModel(id);
            System.Net.Mime.ContentDisposition loContent = new System.Net.Mime.ContentDisposition();
            loContent.FileName = loModel.FileName;
            if (string.IsNullOrEmpty(loContent.FileName))
            {
                loContent.FileName = loModel.Name;
            }

            loContent.Inline = false;
            Response.AppendHeader("Content-Disposition", loContent.ToString());
            return File(loModel.Content, loModel.MimeType);
        }

        [HttpGet]
        public virtual ActionResult VirtualTextFile(string m)
        {
            return this.Show(new MaxVirtualTextFileIISViewModel(), m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult VirtualTextFile(MaxVirtualTextFileIISViewModel loModel, string uoProcess)
        {
            string lsCancelAction = "VirtualTextFile";
            string lsSuccessAction = "VirtualTextFile";
            string lsSuccessMessage = loModel.Name + " successfully created.";
            object loResult = this.Create(loModel, uoProcess, lsCancelAction, lsSuccessAction, lsSuccessMessage);
            if (loResult is ActionResult)
            {
                return (ActionResult)loResult;
            }

            return View(loModel);
        }

        [HttpGet]
        public virtual ActionResult VirtualTextFileEdit()
        {
            MaxVirtualTextFileIISViewModel loModel = new MaxVirtualTextFileIISViewModel();
            if (!string.IsNullOrEmpty(Request.QueryString["name"]))
            {
                loModel = new MaxVirtualTextFileIISViewModel(Request.QueryString["name"]);
            }

            return View(loModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult VirtualTextFileEdit(MaxVirtualTextFileIISViewModel loModel, string uoProcess)
        {
            if (uoProcess == "ConvertToInlineCSS")
            {
                loModel.Content = MaxFactry.General.PresentationLayer.MaxDesignLibrary.ConvertToInlineCSS(loModel.Content, string.Empty);
                return View(loModel);
            }
            else
            {
                string lsCancelAction = "VirtualTextFile";
                ActionResult loResult = this.Edit(loModel, uoProcess, lsCancelAction);
                if (loResult is ViewResult)
                {
                    return RedirectToAction("VirtualTextFileEdit", new RouteValueDictionary { { "name", loModel.Name } });
                }

                return loResult;
            }
        }

        [HttpGet]
        public virtual ActionResult StyleFile(string m)
        {
            return this.Show(new MaxStyleFileViewModel(), m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult StyleFile(MaxStyleFileViewModel loModel, string uoProcess)
        {
            string lsCancelAction = "StyleFile";
            string lsSuccessAction = "StyleFile";
            string lsSuccessMessage = loModel.Name + " successfully created.";
            object loResult = this.Create(loModel, uoProcess, lsCancelAction, lsSuccessAction, lsSuccessMessage);
            if (loResult is ActionResult)
            {
                return (ActionResult)loResult;
            }

            return View(loModel);
        }

        [HttpGet]
        public virtual ActionResult StyleFileEdit(string id)
        {
            MaxStyleFileViewModel loModel = new MaxStyleFileViewModel(id);
            return View(loModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult StyleFileEdit(MaxStyleFileViewModel loModel, string uoProcess)
        {
            string lsCancelAction = "StyleFile";
            ActionResult loResult = this.Edit(loModel, uoProcess, lsCancelAction);
            if (loResult is ViewResult)
            {
                return RedirectToAction("StyleFileEdit", new RouteValueDictionary { { "id", loModel.Id } });
            }

            return loResult;
        }


        [HttpGet]
        public virtual ActionResult ScriptFile(string m)
        {
            return this.Show(new MaxScriptFileViewModel(), m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult ScriptFile(MaxScriptFileViewModel loModel, string uoProcess)
        {
            string lsCancelAction = "ScriptFile";
            string lsSuccessAction = "ScriptFile";
            string lsSuccessMessage = loModel.Name + " successfully created.";
            object loResult = this.Create(loModel, uoProcess, lsCancelAction, lsSuccessAction, lsSuccessMessage);
            if (loResult is ActionResult)
            {
                return (ActionResult)loResult;
            }

            return View(loModel);
        }

        [HttpGet]
        public virtual ActionResult ScriptFileEdit(string id)
        {
            MaxScriptFileViewModel loModel = new MaxScriptFileViewModel(id);
            return View(loModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult ScriptFileEdit(MaxScriptFileViewModel loModel, string uoProcess)
        {
            string lsCancelAction = "ScriptFile";
            ActionResult loResult = this.Edit(loModel, uoProcess, lsCancelAction);
            if (loResult is ViewResult)
            {
                return RedirectToAction("ScriptFileEdit", new RouteValueDictionary { { "id", loModel.Id } });
            }

            return loResult;
        }


        [HttpGet]
        public virtual ActionResult ClientTool(string m)
        {
            return this.Show(new MaxClientToolViewModel(), m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult ClientTool(MaxClientToolViewModel loModel, string uoProcess)
        {
            string lsCancelAction = "ClientTool";
            string lsSuccessAction = "ClientTool";
            string lsSuccessMessage = loModel.Name + " successfully created.";
            object loResult = this.Create(loModel, uoProcess, lsCancelAction, lsSuccessAction, lsSuccessMessage);
            if (loResult is ActionResult)
            {
                return (ActionResult)loResult;
            }

            return View(loModel);
        }

        [HttpGet]
        public virtual ActionResult ClientToolEdit(string id)
        {
            MaxClientToolViewModel loModel = new MaxClientToolViewModel(id);
            return View(loModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult ClientToolEdit(MaxClientToolViewModel loModel, string uoProcess)
        {
            string lsCancelAction = "ClientTool";
            ActionResult loResult = this.Edit(loModel, uoProcess, lsCancelAction);
            if (loResult is ViewResult)
            {
                return RedirectToAction("ClientToolEdit", new RouteValueDictionary { { "id", loModel.Id } });
            }

            return loResult;
        }


        public ActionResult FileUserList()
        {
            MaxFileUserListUploadViewModel loModel = new MaxFileUserListUploadViewModel();
            return View(loModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FileUserList(MaxFileUserListUploadViewModel loModel, string uoProcess, HttpPostedFileBase[] laFile)
        {
            string lsCancelAction = "FileUserList";
            string lsSuccessAction = "FileUserList";
            string lsSuccessMessage = "Successfully uploaded.";
            object loResult = this.Create(loModel, uoProcess, lsCancelAction, lsSuccessAction, lsSuccessMessage, laFile);

            if (loResult is ActionResult)
            {
                return (ActionResult)loResult;
            }

            return View(loModel);
        }

        [HttpGet]
        public virtual ActionResult FileUserListEdit(string id)
        {
            MaxFileUserListUploadViewModel loModel = new MaxFileUserListUploadViewModel(id);
            return View(loModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult FileUserListEdit(MaxFileUserListUploadViewModel loModel, string uoProcess)
        {
            string lsCancelAction = "FileUserList";
            ActionResult loResult = this.Edit(loModel, uoProcess, lsCancelAction);
            if (loResult is ViewResult)
            {
                ViewBag.Message = "Successfully saved.";
                
                if (uoProcess.Length > ProcessSave.Length)
                {
                    string lsProcess = uoProcess.Substring(ProcessSave.Length);
                    if (lsProcess == "Test" || lsProcess == "Import")
                    {
                        MaxFileUserListUploadEntity loEntity = MaxFileUserListUploadEntity.Create();
                        if (loEntity.LoadByIdCache(MaxConvertLibrary.ConvertToGuid(typeof(object), loModel.Id)))
                        {
                            Stream loContent = loEntity.Content;
                            StreamReader loReader = new StreamReader(loContent);
                            try
                            {
                                string lsError = string.Empty;
                                string lsHeader = loReader.ReadLine();
                                List<string> loFieldList = new List<string>(new string[] { "username", "password", "email" });
                                string[] laHeader = lsHeader.Split(new char[] { '\t' });
                                int lnUsername = -1;
                                int lnPassword = -1;
                                int lnEmail = -1;
                                for (int lnH = 0; lnH < laHeader.Length; lnH++)
                                {
                                    lsHeader = laHeader[lnH];
                                    if (!loFieldList.Contains(lsHeader.ToLower()))
                                    {
                                        lsError = "Header of [" + lsHeader + "] needs to be username, password, or email";
                                    }
                                    else if (lsHeader.ToLower() == loFieldList[0])
                                    {
                                        lnUsername = lnH;
                                    }
                                    else if (lsHeader.ToLower() == loFieldList[1])
                                    {
                                        lnPassword = lnH;
                                    }
                                    else if (lsHeader.ToLower() == loFieldList[2])
                                    {
                                        lnEmail = lnH;
                                    }
                                }

                                int lnLineCount = 0;
                                int lnFoundCount = 0;
                                int lnCreatedCount = 0;
                                while (!loReader.EndOfStream && string.IsNullOrEmpty(lsError))
                                {
                                    string lsContent = loReader.ReadLine();
                                    string[] laContent = lsContent.Split(new char[] { '\t' });
                                    if (laContent.Length != laHeader.Length)
                                    {
                                        lsError = "Header length does not match content length for line " + lnLineCount.ToString();
                                    }
                                    else
                                    {
                                        string lsUserName = string.Empty;
                                        if (lnUsername >= 0)
                                        {
                                            lsUserName = laContent[lnUsername];
                                        }
                                        else if (lnEmail >= 0)
                                        {
                                            lsUserName = laContent[lnEmail];
                                        }

                                        string lsEmail = string.Empty;
                                        if (lnEmail > 0)
                                        {
                                            lsEmail = laContent[lnEmail];
                                        }

                                        string lsPassword = string.Empty;
                                        if (lnPassword >= 0)
                                        {
                                            lsPassword = laContent[lnPassword];
                                        }
                                        else
                                        {
                                            lsPassword = Guid.NewGuid().ToString();
                                        }

                                        string lsUserTest = Membership.GetUserNameByEmail(lsEmail);
                                        MembershipUser loUser = Membership.GetUser(lsUserName);
                                            
                                        if (null == loUser && string.IsNullOrEmpty(lsUserTest))
                                        {
                                            if (lsProcess == "Import")
                                            {
                                                try
                                                {
                                                    Membership.CreateUser(lsUserName, lsPassword, lsEmail);
                                                    lnCreatedCount++;
                                                }
                                                catch (Exception loE)
                                                {
                                                    lsError = loE.Message;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lnFoundCount++;
                                        }
                                    }

                                    lnLineCount++;
                                }

                                if (!string.IsNullOrEmpty(lsError))
                                {
                                    ViewBag.Message = "Error: " + lsError;
                                }
                                else if (lsProcess == "Test")
                                {
                                    ViewBag.Message = lnLineCount + " lines read. " + lnFoundCount + " users found.";
                                }
                                else if (lsProcess == "Import")
                                {
                                    ViewBag.Message = lnFoundCount + " users found. " + lnCreatedCount + " users created";
                                }
                            }
                            finally
                            {
                                loReader.Close();
                                loReader = null;
                                if (null != loContent)
                                {
                                    loContent.Close();
                                    loContent = null;
                                }
                            }
                        }
                    }
                }
            }

            return loResult;
        }
    }
}
