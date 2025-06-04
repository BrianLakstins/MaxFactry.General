// <copyright file="MaxManageController.cs" company="eFactory Solutions LLC">
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
// <change date="4/13/2016" author="Brian A. Lakstins" description="Fix issue with being able to add multiple items in a row.">
// <change date="7/14/2016" author="Brian A. Lakstins" description="Fix issue with uploading multiple files at a time.">
// <change date="7/22/2020" author="Brian A. Lakstins" description="Add clearing model state after successful edit.">
// <change date="9/14/2020" author="Brian A. Lakstins" description="Add saving when just the start of the process is save">
// <change date="10/12/2020" author="Brian A. Lakstins" description="Make done button save first">
// <change date="1/11/2021" author="Brian A. Lakstins" description="Redirect on Done even if nothing changed">
// <change date="6/4/2025" author="Brian A. Lakstins" description="Updates for changes to base classes">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{

    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using MaxFactry.Base.PresentationLayer;

    public class MaxManageController : MaxBaseControllerSecure
    {
        public readonly static string ProcessCancel = "Cancel";
        public readonly static string ProcessCreate = "Create";
        public readonly static string ProcessDelete = "Delete";
        public readonly static string ProcessDone = "Done";
        public readonly static string ProcessSave = "Save";
        public readonly static string Process = "Process";

        /// <summary>
        /// Saves the results of an edit.
        /// </summary>
        /// <param name="loModel">The entity supported view model.</param>
        /// <param name="lsProcess">The action to take.</param>
        /// <param name="lsCancelRedirect">The method to handle a cancel action.</param>
        /// <returns>A redirect action or view.</returns>
        protected virtual ActionResult Edit(MaxFactry.Base.PresentationLayer.MaxBaseGuidKeyViewModel loModel, string lsProcess, string lsCancelRedirect)
        {
            if (!string.IsNullOrEmpty(lsProcess))
            {
                if (lsProcess.Equals(ProcessCancel, StringComparison.InvariantCultureIgnoreCase))
                {
                    return this.RedirectToAction(lsCancelRedirect);
                }

                if (ModelState.IsValid)
                {
                    bool lbExists = loModel.EntityLoad();
                    if (lsProcess.StartsWith(ProcessSave, StringComparison.InvariantCultureIgnoreCase) ||
                        lsProcess.Equals(ProcessDone, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (loModel.Save())
                        {
                            if (!lbExists)
                            {
                                loModel.EntityLoad();
                            }

                            loModel.Load();
                            ModelState.Clear();
                        }

                        if (lsProcess.Equals(ProcessDone, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return this.RedirectToAction(lsCancelRedirect);
                        }
                    }
                    else if (lsProcess.Equals(ProcessDelete, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (lbExists)
                        {
                            if (loModel.Delete())
                            {
                                return this.RedirectToAction(lsCancelRedirect);
                            }
                        }
                    }
                }
            }

            return this.View(loModel);
        }

        /// <summary>
        /// Shows the information for the ViewModel.
        /// </summary>
        /// <param name="loModel">The ViewModel related to the entity.</param>
        /// <param name="lsMessage">The message to show.</param>
        /// <returns>Normally a creation and list view.</returns>
        protected virtual ActionResult Show(MaxFactry.Base.PresentationLayer.MaxBaseEntityViewModel loModel, string lsMessage)
        {
            ViewBag.Message = lsMessage;
            return this.View(loModel);
        }

        /// <summary>
        /// Creates a new record based on the ViewModel
        /// </summary>
        /// <param name="loModel">The entity supported view model.</param>
        /// <param name="lsProcess">The action to take.</param>
        /// <param name="lsCancelRedirect">The method to handle a cancel action.</param>
        /// <param name="lsSuccessRedirect">The method to handle a successful creation.</param>
        /// <param name="lsSuccessMessage">The message to show for the success.</param>
        /// <returns>A redirect action or view.</returns>
        protected virtual object Create(MaxBaseGuidKeyViewModel loModel, string lsProcess, string lsCancelRedirect, string lsSuccessRedirect, string lsSuccessMessage)
        {
            return this.Create(loModel, lsProcess, lsCancelRedirect, lsSuccessRedirect, lsSuccessMessage, null);
        }

        protected virtual object Create(MaxBaseGuidKeyViewModel loModel, string lsProcess, string lsCancelRedirect, string lsSuccessRedirect, string lsSuccessMessage, HttpPostedFileBase[] laFile)
        {
            if (!string.IsNullOrEmpty(lsProcess) && lsProcess.Equals(ProcessCancel, StringComparison.InvariantCultureIgnoreCase))
            {
                return this.RedirectToAction(lsCancelRedirect);
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(lsProcess))
                {
                    if (lsProcess.Equals(ProcessCreate, StringComparison.InvariantCultureIgnoreCase))
                    {
                        loModel.Id = string.Empty;
                        if (this.Save(loModel, laFile))
                        {
                            return this.RedirectToAction(lsSuccessRedirect, new RouteValueDictionary { { "id", loModel.Id }, { "m", lsSuccessMessage } });
                        }
                    }
                }
            }

            return this.View(loModel);
        }

        protected virtual bool Save(MaxBaseGuidKeyViewModel loModel, HttpPostedFileBase[] laFile)
        {
            bool lbR = false;
            if (null != laFile && laFile.Length > 0 && loModel is MaxBaseIdFileViewModel)
            {
                MaxBaseIdFileViewModel loModelFile = loModel as MaxBaseIdFileViewModel;
                lbR = true;
                foreach (HttpPostedFileBase loFile in laFile)
                {
                    if (null != loFile)
                    {
                        loModelFile.Id = Guid.Empty.ToString();
                        loModelFile.EntityLoad();
                        loModelFile.Load();
                        loModelFile.Content = loFile.InputStream;
                        loModelFile.Name = loFile.FileName;
                        loModelFile.FileName = loFile.FileName;
                        loModelFile.ContentType = loFile.ContentType;
                        loModelFile.ContentLength = loFile.ContentLength.ToString();
                        if (!loModelFile.Save())
                        {
                            lbR = false;
                        }
                    }
                }
            }
            else
            {
                lbR = loModel.Save();
            }

            return lbR;
        }
    }
}
